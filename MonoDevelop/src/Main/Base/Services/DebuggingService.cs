// DebuggingService.cs - Debugging service frontend for MonoDebugger
//
//  Author: Mike Kestner <mkesner@ximian.com>
//
// Copyright (c) 2004 Novell, Inc.

using System;
using System.Collections;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;

using Mono.Debugger;

namespace MonoDevelop.Services
{

	public interface IDebuggableEditor {
		void ExecutingAt (int lineNumber);
	}

	public class DebuggingService : AbstractService
	{
		Process proc;
		Hashtable breakpoints = new Hashtable ();
		DebuggerBackend backend;

		public DebuggingService()
		{
		}
		
		public override void UnloadService ()
		{
			if (Debugging)
				proc.Kill ();

			base.UnloadService ();
		}

		private bool Debugging {
			get {
				return backend != null && proc != null && proc.HasTarget;
			}
		}

		private bool IsRunning {
			get {
				return Debugging && !proc.IsStopped;
			}
		}

		private Breakpoint CreateBreakpoint (string name)
		{
			SimpleBreakpoint point = new SimpleBreakpoint (name, null);
			point.BreakpointHitEvent += new BreakpointEventHandler (OnBreakpointHit);
			return point;
		}

		public bool AddBreakpoint (string filename, int linenum)
		{
			string key = filename + ":" + linenum;
			BreakpointHandle brkptnum = null;
			if (Debugging) {
				Breakpoint point = CreateBreakpoint (key);
				SourceLocation loc = backend.FindLocation(filename, linenum);
				if (loc == null)
					return false;
				brkptnum = loc.InsertBreakpoint (proc, point);
			}

			breakpoints.Add (key, brkptnum);
			return true;
		}

		public void RemoveBreakpoint (string filename, int linenum)
		{
			string key = filename + ":" + linenum;
			if (Debugging)
				((BreakpointHandle)breakpoints [key]).RemoveBreakpoint (proc);

			breakpoints.Remove (key);
		}

		public bool ToggleBreakpoint (string filename, int linenum)
		{
			if (!breakpoints.ContainsKey (filename + ":" + linenum))
				return AddBreakpoint (filename, linenum);
			else
				RemoveBreakpoint (filename, linenum);
			return true;
		}

		public void ToggleRunning ()
		{
			if (!Debugging)
				return;

			if (proc.IsStopped)
				proc.Continue (false);
			else
				proc.Stop ();
		}

		public void Run (string[] argv)
		{
			if (Debugging)
				return;

			backend = new DebuggerBackend ();
			proc = backend.Run (ProcessStart.Create (null, argv));
			string[] keys = new string [breakpoints.Keys.Count];
			breakpoints.Keys.CopyTo (keys, 0);
			foreach (string key in keys) {
				Breakpoint point = CreateBreakpoint (key);
				string[] toks = point.Name.Split (':');
				string filename = toks [0];
				int linenumber = Int32.Parse (toks [1]);
				Console.WriteLine ("Looking up " + filename + " " + linenumber);
				SourceLocation loc = backend.FindLocation(filename, linenumber);
				if (loc == null) {
					Console.WriteLine ("Couldn't find breakpoint location " + key);
					return;
				}
				breakpoints [key] = loc.InsertBreakpoint (proc, point);
				if (breakpoints [key] == null)
					Console.WriteLine ("Couldn't insert breakpoint " + key);
			}
			proc.Continue (false);
		}

		public void Stop ()
		{
			if (!Debugging)
				return;

			proc.Kill ();
			proc = null;
			backend = null;
		}

		private void OnBreakpointHit (Breakpoint point)
		{
			if (this.BreakpointHit == null) 
				return;

			string[] toks = point.Name.Split (':');
			string filename = toks [0];
			int linenumber = Int32.Parse (toks [1]);

			IFileService fs = (IFileService)ServiceManager.Services.GetService (typeof (IFileService));
			fs.OpenFile (filename);

			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow is IDebuggableEditor)
			{
				((IDebuggableEditor)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow).ExecutingAt (linenumber);
			}	

			BreakpointHitArgs args = new BreakpointHitArgs (filename, linenumber);
			BreakpointHit (this, args);
			Console.WriteLine ("hit breakpoint " + point.Name);
		}

		public event DebuggingService.BreakpointHitHandler BreakpointHit;

		public delegate void BreakpointHitHandler (object o, BreakpointHitArgs args);

		public class BreakpointHitArgs : EventArgs {

			string filename;
			int linenumber;

			public BreakpointHitArgs (string filename, int linenumber)
			{
				this.filename = filename;
				this.linenumber = linenumber;
			}

			public string Filename {
				get {
					return filename;
				}
			}

			public int LineNumber {
				get {
					return linenumber;
				}
			}
		}
	}
}
