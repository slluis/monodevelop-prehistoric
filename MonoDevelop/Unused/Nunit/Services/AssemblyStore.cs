using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using Gtk;
using NUnit.Core;

namespace MonoDevelop.Services.Nunit
{
	public class AssemblyStore : TreeStore, EventListener
    {
        string assemblyName;
        Hashtable iters;
        TestSuite rootTS;
        TestResult lastResult;
        int totalTests;
        int currentTest;

        bool runningTest;
        bool cancelled;
        EventListener listener;
        Test test;
        System.Threading.ManualResetEvent idle;
        Queue pending;
        System.Threading.Thread th;
        string location;

        Exception exception;
		event FixtureAddedEventHandler FixtureAdded;
        event FixtureLoadErrorHandler FixtureLoadError;
        public event EventHandler FinishedRunning;
        public event EventHandler FinishedLoad;
        public event EventHandler IdleCallback;

        public AssemblyStore (string assemblyName) : base (typeof (int), typeof (string))
        {
            if (assemblyName == null)
                throw new ArgumentNullException ("assemblyName");

            this.assemblyName = assemblyName;
            location = "";
        }
	
		public string Location {
            get { return location; }
        }

        public bool Running {
            get { return runningTest; }
        }

        public bool CancelRequest {
            set {
                if (runningTest) {
                    th.Abort ();
                }
            }
        }

        public bool Cancelled {
            get { return cancelled; }
        }

		public TestResult LastResult {
            get { return lastResult; }
        }
                                                                                
        public int SearchColumn {
            get { return 1; }
        }

        public void Load ()
        {
            // I don't like this...
            Assembly a;
            try {
                a = Assembly.Load (assemblyName);
                location = a.Location;
            } catch (Exception e) {
                Console.WriteLine (e);
                try {
                    a = Assembly.LoadFrom (assemblyName);
                    location = a.Location;
                } catch {
                    location = "";
				}
			}

			GLib.Idle.Add (new GLib.IdleHandler (Populate));
		}

		public void RunFinished (Exception e)
		{
		}

		public void RunFinished (TestResult[] results)
		{
		}

		public void RunStarted (Test[] tests)
		{
		}

		public void UnhandledException (Exception e)
		{
		}

		void GrayOut (TreeIter iter)
        {
            SetValue (iter, 0, (int) CircleColor.None);
            TreeIter child;
            if (!IterChildren (out child, iter))
                return;

            do {
                GrayOut (child);
            } while (IterNext (ref child));
        }

        public void RunTestAtIter (TreeIter iter, EventListener listener, ref int ntests)
        {
            if (iter.Equals (TreeIter.Zero))
                return;

            RunTestAtPath (GetPath (iter), listener, ref ntests);
        }

		public void RunTestAtPath (TreePath path, EventListener listener, ref int ntests)
        {
            if (runningTest)
                throw new InvalidOperationException ("Already running some test(s).");

            cancelled = false;
            if (idle == null) {
                idle = new ManualResetEvent (false);
                pending = new Queue ();
            }

            TreeIter iter;
            GetIter (out iter, path);
            GrayOut (iter);

            if (iter.Equals (TreeIter.Zero))
                return;

            string p = GetPath (iter).ToString ();
            if (p == null || p == "")
			    return;
                                                                                
            test = LookForTestByPath (p, null);
            if (test == null)
				return;

            ntests = test.CountTestCases ();
            runningTest = true;
            this.listener = listener;
            th = new System.Threading.Thread (new ThreadStart (InternalRunTest));
            th.IsBackground = true;
            th.Start ();
            GLib.Idle.Add (new GLib.IdleHandler (Updater));
        }

        public new void Clear ()
        {
            base.Clear ();
            iters = null;
            lastResult = null;
        }

		void DoPending ()
        {
            QueuedEvent [] events;
            lock (pending) {
                events = new QueuedEvent [pending.Count];
                pending.CopyTo (events, 0);
                pending.Clear ();
            }

            foreach (QueuedEvent e in events)
                e.DoCallback ();
        }

        bool Updater ()
        {
            if (!idle.WaitOne (0, true)) {
                if (IdleCallback != null)
                    IdleCallback (this, EventArgs.Empty);
                                                                                
                return true;
            }

            DoPending ();
			if (runningTest == false) {
                DoPending ();
                OnFinishedRunning ();
            }

            idle.Reset ();
            return runningTest;
        }

        void InternalRunTest ()
        {
            lastResult = null;
            try {
                lastResult = test.Run (this);
            } catch (ThreadAbortException) {
                Thread.ResetAbort ();
                cancelled = true;
            } finally {
                runningTest = false;
                idle.Set ();
            }
        }

		Test LookForTestByPath (string path, Test t)
        {
            string [] parts = path.Split (':');
            if (t == null) {
                if (parts.Length > 1)
                    return LookForTestByPath (String.Join (":", parts, 1, parts.Length - 1), rootTS);

                return rootTS;
            }

            Test ret;
            if (parts.Length == 1) {
                ret = (Test) t.Tests [Int32.Parse (path)];
                return ret;
            }

            ret = (Test) t.Tests [Int32.Parse (parts [0])];
			return LookForTestByPath (String.Join (":", parts, 1, parts.Length - 1), ret);

        }

        TreeIter AddFixture (TreeIter parent, string fullName)
        {
            string typeName = fullName;
            string [] parts = typeName.Split ('.');
            string index = "";

            foreach (string s in parts) {
                if (index == "")
                    index = s;
                else
                    index += "." + s;

                if (iters.ContainsKey (index)) {
                    parent = (TreeIter) iters [index];
                    continue;
                }

                parent = AppendValues (parent, (int) CircleColor.None, s);
				iters [index] = parent;
            }

            return parent;
        }

        void AddSuite (TreeIter parent, TestSuite suite)
        {
            TreeIter next;
            foreach (Test t in suite.Tests) {
                next = AddFixture (parent, t.FullName);
                while (GLib.MainContext.Iteration ());
                if (t.IsSuite)
                    AddSuite (next, (TestSuite) t);
                else if (FixtureAdded != null)
                    FixtureAdded (this, new FixtureAddedEventArgs (++currentTest, totalTests));

            }
        }

		bool Populate ()
        {
            Clear ();
            iters = new Hashtable ();
            TreeIter first;
            Append (out first);
            SetValue (first, 0, (int) CircleColor.None);
            SetValue (first, 1, assemblyName);
            iters [assemblyName] = first;
            ResolveEventHandler reh = new ResolveEventHandler (TryLoad);
            AppDomain.CurrentDomain.AssemblyResolve += reh;

            try {
                rootTS = new TestSuiteBuilder ().Build (assemblyName);
            } catch (Exception e) {
                if (FixtureLoadError != null) {
                    exception = e;
                    GLib.Idle.Add (new GLib.IdleHandler (TriggerError));
                }
                return false;
            } finally {
                AppDomain.CurrentDomain.AssemblyResolve -= reh;
            }

			currentTest = 0;
            totalTests = rootTS.CountTestCases ();
            AddSuite (first, rootTS);
            OnFinishedLoad ();

            return false;
        }

        void OnFinishedLoad ()
        {
            if (FinishedLoad != null)
                FinishedLoad (this, EventArgs.Empty);
        }

        void OnFinishedRunning ()
        {
            if (FinishedRunning != null)
                FinishedRunning (this, EventArgs.Empty);
        }

		bool TriggerError ()
        {
            FixtureLoadError (this, new FixtureLoadErrorEventArgs (assemblyName, exception));
            exception = null;
            return false;
        }

        Assembly TryLoad (object sender, ResolveEventArgs args)
        {
            try {
                // NUnit2 uses Assembly.Load on the filename without extension.
                // This is done just to allow loading from a full path name.
                return Assembly.LoadFrom (assemblyName);
            } catch { }

            return null;
        }

		// Interface NUnit.Core.EventListener
        void EventListener.TestStarted (TestCase testCase)
        {
            if (listener != null) {
                Monitor.Enter (pending);
                pending.Enqueue (new QueuedTestStart (new TestStartHandler (listener.TestStarted), testCase));
                Monitor.Exit (pending);
                idle.Set ();
            }
        }

        void EventListener.TestFinished (TestCaseResult result)
        {
            Monitor.Enter (pending);
            if (listener != null)
                pending.Enqueue (new QueuedTestFinish (new TestFinishHandler (listener.TestFinished), result));

            pending.Enqueue (new QueuedTestEvent (new TestCaseResultHandler (SetIconFromResult), result));
            Monitor.Exit (pending);
            idle.Set ();
		}

		void EventListener.SuiteStarted (TestSuite suite)
        {
            if (listener != null) {
                Monitor.Enter (pending);
                pending.Enqueue (new QueuedSuiteStart (new SuiteStartHandler (listener.SuiteStarted), suite));
                Monitor.Exit (pending);
                idle.Set ();
            }
        }

        void EventListener.SuiteFinished (TestSuiteResult result)
        {
            System.Threading.Monitor.Enter (pending);
            if (listener != null)
                pending.Enqueue (new QueuedSuiteFinish (new SuiteFinishHandler (listener.SuiteFinished), result));

            pending.Enqueue (new QueuedTestEvent (new TestCaseResultHandler (SetIconFromResult), result));
            Monitor.Exit (pending);
            idle.Set ();
        }

		void SetIconFromResult (TestResult result)
        {
            CircleColor color;
            if (!result.Executed)
                color = CircleColor.NotRun;
            else if (result.IsFailure)
                color = CircleColor.Failure;
            else if (result.IsSuccess)
                color = CircleColor.Success;
            else {
                Console.WriteLine ("Warning: unexpected combination.");
                color = CircleColor.None;
            }

            string fullname = result.Test.FullName;
            if (iters.ContainsKey (fullname)) {
                TreeIter iter = (TreeIter) iters [fullname];
                SetValue (iter, 0, (int) color);
            } else {
                Console.WriteLine ("Don't know anything about {0}", fullname);
            }
        }	
	}
}
