//
// NunitPad - Pad to embed nunit-gtk
//
// Author: John Luke <jluke@cfl.rr.com>
//
// (C) 2004 John Luke

using System;
using System.Collections;
using System.IO;
using System.Text;
using Gtk;

using MonoDevelop.Gui;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Services.Nunit;
using NUnit.Core;

namespace MonoDevelop.Nunit.Gui
{
	public class TestTree : AbstractPadContent, EventListener
	{
		NunitService ns = (NunitService) ServiceManager.GetService (typeof (NunitService));
		IStatusBarService sbs = (IStatusBarService) ServiceManager.GetService (typeof (IStatusBarService));

		ScrolledWindow sw = new ScrolledWindow ();
		Notebook notebook;

		Label [] nbLabels;
		Label failuresLabel = new Label ();
		Label stderrLabel = new Label ();
		Label stdoutLabel = new Label ();
		Label notRunLabel = new Label ();
		Label runStatus = new Label ();

		TreeView failures = new TreeView ();
		TreeView notRun = new TreeView ();
		TextView stdoutTV = new TextView ();
		TextView stderrTV = new TextView ();
		TreeView assemblyView = new TreeView ();

		AssemblyStore store;
		TreeStore notRunStore;
		TreeStore failuresStore;
		TreeViewColumn nameCol;

		Hashtable errorIters;
		TextWriter origStdout = Console.Out;
        TextWriter origStderr = Console.Error;
        StringWriter stdout = new StringWriter ();
        StringWriter stderr = new StringWriter ();

		CellRendererPixbuf pr;
		CellRendererText tr;

		int ntests;
		int finishedTests;
        int ignoredTests;
        int errorTests;
		long startTime;
		long lastTick = -1;
		
		// FIXME: obviously the icon is a placeholder
		public TestTree () : base ("NUnit", Gtk.Stock.Help)
		{
			pr = new CellRendererPixbuf ();
			tr = new CellRendererText ();
			assemblyView.PopupMenu += new PopupMenuHandler (OnPopupMenu);
			store = ns.AssemblyStore;


			nameCol = new TreeViewColumn ();
            nameCol.PackStart (pr, false);
            nameCol.SetCellDataFunc (pr, CircleRenderer.CellDataFunc, IntPtr.Zero, null);
            nameCol.PackStart (tr, false);
            nameCol.AddAttribute (tr, "text", 1);
            assemblyView.AppendColumn (nameCol);

			sw.Add (assemblyView);
		}
		
		public override Gtk.Widget Control
		{
			get { return sw; }
		}

		void OnPopupMenu (object o, PopupMenuArgs args)
		{
			ShowPopup ();
		}

		void ShowPopup ()
		{
			Menu menu = new Menu ();
			MenuItem load = new MenuItem ("Load assembly");
			load.Activated += new EventHandler (OnLoadActivated);
			menu.Append (load);
			menu.Popup (null, null, null, IntPtr.Zero, 3, Global.CurrentEventTime);
			menu.ShowAll ();
		}

		void OnLoadActivated (object o, EventArgs args)
		{
		}

		// assemblyView events
        void OnTestActivated (object sender, RowActivatedArgs args)
        {
            if (store == null)
                return;

            TreeView tv = (TreeView) sender;
            tv.ExpandRow (args.Path, true);
            if (!store.Running) {
                PrepareRun ();
                store.RunTestAtPath (args.Path, this, ref ntests);
            }
        }

		void PrepareRun ()
        {
            if (errorIters != null)
                errorIters.Clear ();

            if (notRunStore != null)
                notRunStore.Clear ();

            if (failuresStore != null)
                failuresStore.Clear ();

            stdoutTV.Buffer.Clear ();
            stderrTV.Buffer.Clear ();
            foreach (Label l in nbLabels)
                SetColorLabel (l, false);

            errorTests = 0;
            ignoredTests = 0;

            ntests = -1;
            finishedTests = 0;
            //sbs.ProgressMonitor.Worked (0.0, "");

			sbs.SetMessage ("Running tests...");
            SetStringWriters ();
            ToggleMenues (false);
            startTime = DateTime.Now.Ticks;
            ClockUpdater (this, EventArgs.Empty);
        }

		public void RunFinished (Exception e)
		{
		}

		public void RunFinished (TestResult[] tests)
		{
		}

		public void RunStarted (Test[] tests)
		{
		}

		public void UnhandledException (Exception e)
		{
		}

		void SetStringWriters ()
        {
            Console.SetOut (stdout);
            Console.SetError (stderr);
        }

		void ToggleMenues (bool saveAs)
        {/*
            if (btnStop.Sensitive) {
                btnOpen.Sensitive = true;
                btnSaveAs.Sensitive = saveAs;
                menuSaveAs.Sensitive = saveAs;
                btnRun.Sensitive = true;
                btnExit.Sensitive = true;
                btnStop.Sensitive = false;
                menubar.Sensitive = true;
            } else {
                btnOpen.Sensitive = false;
                btnSaveAs.Sensitive = false;
                menuSaveAs.Sensitive = false;
                btnRun.Sensitive = false;
                btnExit.Sensitive = false;
                btnStop.Sensitive = true;
                menubar.Sensitive = false;
            } */
        }

		void CheckWriters ()
        {
            StringBuilder sb = stdout.GetStringBuilder ();
            if (sb.Length != 0) {
                InsertOutText (stdoutTV, sb.ToString ());
                sb.Length = 0;
                SetColorLabel (stdoutLabel, true);
            }

            sb = stderr.GetStringBuilder ();
            if (sb.Length != 0) {
                stderrTV.Buffer.InsertAtCursor (sb.ToString ());
                sb.Length = 0;
                SetColorLabel (stderrLabel, true);
            }
        }

		void ClockUpdater (object o, EventArgs args)
        {
            long now = DateTime.Now.Ticks;
            if (!store.Running || now - lastTick >= 10000 * 100) { // 100ms
                lastTick = now;
                string fmt = new TimeSpan (now - startTime).ToString ();
                int i = fmt.IndexOf ('.');
                if (i > 0 && fmt.Length - i > 2)
                    fmt = fmt.Substring (0, i + 2);

                //clock.Text = String.Format ("Elapsed time: {0}", fmt);
            }
        }

		// Interface NUnit.Core.EventListener
		void EventListener.TestStarted (TestCase testCase)
        {
            //frameLabel.Text = "Test: " + testCase.FullName;
        }

		void EventListener.TestFinished (TestCaseResult result)
        {
            //sbs.ProgressMonitor.Worked (++finishedTests / (double) ntests, String.Format ("{0}/{1}", finishedTests, ntests));

            if (result.Executed == false) {
                AddIgnored (result.Test.FullName, result.Test.IgnoreReason);
            } else if (result.IsFailure) {
                AddError (result);
			}

            CheckWriters ();
            UpdateRunStatus ();
            ClockUpdater (this, EventArgs.Empty);
        }

		void EventListener.SuiteStarted (TestSuite suite)
        {
            //frameLabel.Text = "Suite: " + suite.FullName;
        }

        void EventListener.SuiteFinished (TestSuiteResult result)
        {
            ClockUpdater (this, EventArgs.Empty);
        }

		void AddError (TestCaseResult result)
        {
            errorTests++;
            if (failuresStore == null) {
                failuresStore = new TreeStore (typeof (string));
                CellRendererText tr = new CellRendererText ();
                TreeViewColumn col = new TreeViewColumn ();
                col.PackStart (tr, false);
                col.AddAttribute (tr, "text", 0);
                failures.AppendColumn (col);
                failures.Model = failuresStore;
                failures.ShowAll ();
            }

            if (errorIters == null)
                errorIters = new Hashtable ();

            int dot;
            TreeIter main = TreeIter.Zero;
            TreeIter iter;
            string fullname = result.Test.FullName;
            if ((dot = fullname.LastIndexOf ('.')) != -1) {
                string key = fullname.Substring (0, dot);
				if (!errorIters.ContainsKey (key)) {
                    main = failuresStore.AppendValues (key);
                    errorIters [key] = main;
                } else {
                    main = (TreeIter) errorIters [key];
                    failuresStore.SetValue (main, 0, key);
                }
            } else {
                main = failuresStore.AppendValues (fullname);
                errorIters [fullname] = main;
            }

            iter = failuresStore.AppendValues (main, result.Test.Name);
            iter = failuresStore.AppendValues (iter, result.Message);
            iter = failuresStore.AppendValues (iter, result.StackTrace);

            SetColorLabel (failuresLabel, true);
        }

		void AddIgnored (string name, string reason)
        {
            ignoredTests++;
            if (notRunStore == null) {
                notRunStore = new TreeStore (typeof (string));
                CellRendererText tr = new CellRendererText ();
                TreeViewColumn col = new TreeViewColumn ();
                col.PackStart (tr, false);
                col.AddAttribute (tr, "text", 0);
                notRun.AppendColumn (col);
                notRun.Model = notRunStore;
                notRun.ShowAll ();
            }

            TreeIter iter;
            iter = notRunStore.AppendValues (name);
            iter = notRunStore.AppendValues (iter, reason);

            SetColorLabel (notRunLabel, true);
        }

		void UpdateRunStatus ()
        {
            runStatus.Markup = String.Format ("Tests: {0} Ignored: {1} Failures: {2}",
            	finishedTests, ignoredTests, errorTests);
        }

		void SetColorLabel (Label label, bool color)
        {
            string text = label.Text;
            if (color)
                label.Markup = String.Format ("<span foreground=\"blue\">{0}</span>", text);
            else
                label.Markup = text;
        }

		void InsertOutText (TextView tv, string str)
        {
            TextBuffer buf = tv.Buffer;
            buf.InsertAtCursor (str);
        }
	}
}
