using System;
using System.Collections;
using System.Reflection;
using Gtk;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Commands;

using NUnit.Framework;
using NUnit.Core;

namespace MonoDevelop.NUnit
{
	public class TestPad : AbstractPadContent
	{
		string assemblyName;
		ScrolledWindow sw;
		TreeView view;
		TreeStore store;
		Hashtable iters;
		TestSuite rootTestSuite;

		IStatusBarService statusBarService;
		NUnitService nunitService;

		int currentTest, totalTests, finishedTests;
		int ignoredTests, errorTests;

		event FixtureAddedEventHandler FixtureAddedEvent;

		public TestPad () : base ("NUnit")
		{
			sw = new ScrolledWindow ();
			CreateView ();
			sw.Add (view);
			sw.ShowAll ();

			// color, name
			store = new TreeStore (typeof (Gdk.Pixbuf), typeof (string));

			// services
			statusBarService = ServiceManager.GetService (typeof (IStatusBarService)) as IStatusBarService;
			nunitService = ServiceManager.GetService (typeof (NUnitService)) as NUnitService;

			// register events
			this.FixtureAddedEvent += OnFixtureAdded;
			nunitService.AssemblyLoaded += OnAssemblyLoaded;
			nunitService.TestStartedEvent += OnTestStarted;
			nunitService.TestFinishedEvent += OnTestFinished;
		}

		public override Widget Control {
			get { return sw; }
		}

		void CreateView ()
		{
			view = new TreeView ();
			view.HeadersVisible = false;

			CellRendererPixbuf pr = new CellRendererPixbuf ();
			CellRendererText tr = new CellRendererText ();
			TreeViewColumn column = new TreeViewColumn ();
			column.PackStart (pr, false);
			column.AddAttribute (pr, "pixbuf", 0);
			column.PackStart (tr, false);
			column.AddAttribute (tr, "text", 1);
			view.AppendColumn (column);

			view.RowActivated += OnRowActivated;
			view.PopupMenu += OnPopup;
			view.ButtonReleaseEvent += OnButtonReleased;
		}

		TreeIter AddFixture (TreeIter parent, string fullname)
		{
			string [] parts = fullname.Split ('.');
			string index = String.Empty;

			foreach (string s in parts)
			{
				if (index.Length == 0)
					index = s;
				else
					index += String.Format (".{0}", s);

				if (iters.ContainsKey (index)) {
					parent = (TreeIter) iters [index];
					continue;
				}

				parent = store.AppendValues (parent, CircleImage.None, s);
				iters[index] = parent;
			}

			return parent;
		}

		void AddTestSuite (TreeIter parent, TestSuite suite)
		{
			TreeIter next;
			foreach (Test t in suite.Tests)
			{
				next = AddFixture (parent, t.FullName);
				while (GLib.MainContext.Iteration ());

				if (t.IsSuite)
					AddTestSuite (next, (TestSuite) t);
				else if (FixtureAddedEvent != null)
					FixtureAddedEvent (this, new FixtureAddedEventArgs (++currentTest, totalTests));
			}
		}

		void OnAssemblyLoaded (object sender, EventArgs a)
		{
			GLib.Idle.Add (new GLib.IdleHandler (Populate));
		}

		void OnButtonReleased (object sender, ButtonReleaseEventArgs a)
		{
			if (a.Event.Button == 3)
				ShowPopup ();
		}

		void OnFinishedLoad (object sender, EventArgs a)
		{
			string msg = String.Format (GettextCatalog.GetString ("{0} tests loaded."), totalTests);
			statusBarService.SetProgressFraction (0.0);
			statusBarService.SetMessage (msg);
			// FIXME: set run menu items sensitive
			TreeIter first;
			if (store.GetIterFirst (out first))
				view.Selection.SelectIter (first);
		}

		void OnFinishedRunning (object sender, EventArgs a)
		{
			statusBarService.SetMessage ("");
		}

		void OnFixtureAdded (object sender, FixtureAddedEventArgs a)
		{
			string msg = String.Format (GettextCatalog.GetString ("Loading test {0} of {1}"), a.Current, a.Total);
			statusBarService.SetProgressFraction (a.Current / a.Total);
			statusBarService.SetMessage (msg);
		}

		void OnLoadActivated (object sender, EventArgs a)
		{
			new NUnitLoadAssembly ().Run ();
		}

		void OnPopup (object sender, EventArgs a)
		{
			ShowPopup ();
		}

		void ShowPopup ()
		{
			Menu menu = new Menu ();

			MenuItem load = new MenuItem ("Load test assembly");
			load.Activated += OnLoadActivated;
			menu.Append (load);

			MenuItem run = new MenuItem ("Run tests");
			run.Activated += OnRunActivated;
			menu.Append (run);

			//MenuItem save = new MenuItem ("Save Results");

			menu.ShowAll ();
			menu.Popup ();
		}

		void OnRowActivated (object sender, RowActivatedArgs a)
		{
			if (!nunitService.Running) {
				finishedTests = 0;
				RunTestAtPath (a.Path);
			}
		}

		void OnRunActivated (object sender, EventArgs a)
		{
			new NUnitRunTests ().Run ();
		}

		void OnTestFinished (object sender, TestEventArgs a)
		{
			SetIconFromResult (a.Result);
			statusBarService.SetMessage ("");
		}
	
		void OnTestStarted (object sender, TestEventArgs a)
		{
			statusBarService.SetProgressFraction (++finishedTests / totalTests);
			statusBarService.SetMessage ("Test: " + a.Test.FullName);
		}
	
		bool Populate ()
		{
			Assembly test = nunitService.TestAssembly;
			store.Clear ();
			assemblyName = test.FullName;
			TreeIter root = store.AppendValues (CircleImage.None, assemblyName);
			iters = new Hashtable ();
			iters[assemblyName] = root;

			rootTestSuite = nunitService.GetTestSuite (assemblyName);

			currentTest = 0;
			totalTests = rootTestSuite.CountTestCases ();
			AddTestSuite (root, rootTestSuite);
			OnFinishedLoad (null, null);

			view.Model = store;
			return false;
		}

		void RunTestAtPath (TreePath path)
		{
			Test test = GetTestFromPath (path.ToString (), null);
			totalTests = test.CountTestCases ();
			nunitService.RunTest (test);
		}

		Test GetTestFromPath (string path, Test t)
		{
            string [] parts = path.Split (':');
            if (t == null) {
                if (parts.Length > 1)
                    return GetTestFromPath (String.Join (":", parts, 1, parts.Length - 1), rootTestSuite);

                return rootTestSuite;
            }

            Test ret;
            if (parts.Length == 1) {
                ret = (Test) t.Tests [Int32.Parse (path)];
                return ret;
            }

            ret = (Test) t.Tests [Int32.Parse (parts [0])];
			return GetTestFromPath (String.Join (":", parts, 1, parts.Length - 1), ret);
		}

		void SetIconFromResult (TestResult result)
		{
			string fullname = result.Test.FullName;

			if (iters.ContainsKey (fullname)) {
				TreeIter iter = (TreeIter) iters [fullname];
				if (!result.Executed)
					store.SetValue (iter, 0, CircleImage.NotRun);
				else if (result.IsFailure)
					store.SetValue (iter, 0, CircleImage.Failure);
				else if (result.IsSuccess)
					store.SetValue (iter, 0, CircleImage.Success);
				else
					store.SetValue (iter, 0, CircleImage.None);
			}
		}
	}
}

