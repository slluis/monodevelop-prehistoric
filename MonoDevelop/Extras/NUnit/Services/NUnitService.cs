using System;
using System.IO;
using System.Reflection;

using NUnit.Core;
using MonoDevelop.Core.Services;
using MonoDevelop.NUnit;

namespace MonoDevelop.Services
{
	public class NUnitService : AbstractService, EventListener
	{
		Assembly asm;
		bool running = false;
		TestSuite rootTestSuite;

		public event EventHandler AssemblyLoaded;
		public event FixtureLoadedErrorEventHandler FixtureLoadError;
		public event TestEventHandler SuiteFinishedEvent;
		public event TestEventHandler SuiteStartedEvent;
		public event TestEventHandler TestFinishedEvent;
		public event TestEventHandler TestStartedEvent;

		public NUnitService ()
		{
		}

		public bool Running {
			get { return running; }
		}

		public Assembly TestAssembly {
			get { return asm; }
		}

		public TestSuite GetTestSuite (string assemblyName)
		{
			ResolveEventHandler reh = new ResolveEventHandler (TryLoad);
			AppDomain.CurrentDomain.AssemblyResolve += reh;

			TestSuite suite = null;
			try {
				suite = new TestSuiteBuilder ().Build (assemblyName);
			}
			catch (Exception e) {
				if (FixtureLoadError != null)
					FixtureLoadError (this, new FixtureLoadedErrorEventArgs (assemblyName, e));
			}
			finally {
				AppDomain.CurrentDomain.AssemblyResolve -= reh;
			}
			rootTestSuite = suite;
			return suite;
		}

		public void LoadAssembly (string path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			if (!File.Exists (path))
				throw new Exception ("assembly could not be found: " + path);

			asm = Assembly.LoadFrom (path);
			if (AssemblyLoaded != null)
				AssemblyLoaded (this, EventArgs.Empty);
		}

		public void RunFinished (Exception exception)
		{
		}

		public void RunFinished (TestResult[] results)
		{
		}

		public void RunStarted (Test[] tests)
		{
		}

		public void RunTest (Test test)
		{
			if (running) {
				Console.WriteLine ("already running a test");
				return;
			}
			running = true;
			test.Run (this);
			running = false;
		}

		public void RunTests ()
		{
			if (rootTestSuite != null)
				RunTest (rootTestSuite);
		}

		public void SuiteFinished (TestSuiteResult result)
		{
			if (SuiteFinishedEvent != null)
				SuiteFinishedEvent (this, new TestEventArgs (TestAction.SuiteFinished, result));
		}

		public void SuiteStarted (TestSuite suite)
		{
			if (SuiteStartedEvent != null)
				SuiteStartedEvent (this, new TestEventArgs (TestAction.SuiteStarting, suite));
		}

		public void TestFinished (TestCaseResult result)
		{
			if (TestFinishedEvent != null)
				TestFinishedEvent (this, new TestEventArgs (TestAction.TestFinished, result));
		}

		public void TestStarted (TestCase test)
		{
			if (TestStartedEvent != null)
				TestStartedEvent (this, new TestEventArgs (TestAction.TestStarting, test));
		}

		Assembly TryLoad (object sender, ResolveEventArgs a)
		{
			try {
				// NUnit2 uses Assembly.Load on the filename without extension.
				// This is done just to allow loading from a full path name.
				return Assembly.LoadFrom (asm.FullName);
			}
			catch { }
			return null;
		}

		public void UnhandledException (Exception exception)
		{
		}
	}
}

