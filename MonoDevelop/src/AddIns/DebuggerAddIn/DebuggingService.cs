// DebuggingService.cs - Debugging service frontend for MonoDebugger
//
//  Author: Mike Kestner <mkesner@ximian.com>
//
// Copyright (c) 2004 Novell, Inc.

using System;
using System.Collections;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns;

using MonoDevelop.Services;
using MonoDevelop.Gui;

using Mono.Debugger;

/*
 * Some places we should be doing some error handling we used to toss
 * exceptions, now we error out silently, this needs a real solution.
 */

namespace MonoDevelop.Services
{

	public class DebuggingService : AbstractService, IDebuggingService
	{
		Process proc;
		Hashtable breakpoints = new Hashtable ();
		DebuggerBackend backend;
		Breakpoint point;

		public DebuggingService()
		{
			DebuggerBackend.Initialize ();
		}
		
		void Cleanup ()
		{
			if (!Debugging)
				return;

			if (StoppedEvent != null)
				StoppedEvent (this, new EventArgs ());
			backend.Dispose ();
			backend = null;
			proc = null;
		}

		public override void UnloadService ()
		{
			Cleanup ();
			base.UnloadService ();
		}

		private bool Debugging {
			get {
				return backend != null && proc != null && proc.HasTarget;
			}
		}

		public bool IsRunning {
			get {
				return Debugging && !proc.IsStopped;
			}
		}

		private Breakpoint CreateBreakpoint (string name)
		{
			SimpleBreakpoint point = new SimpleBreakpoint (name);
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

		private void initialized_event (ThreadManager manager, Process process)
		{
			this.proc = process;

			proc.TargetEvent += new TargetEventHandler (target_event);

			Gtk.Timeout.Add (1, new Gtk.Function (EmitStarted));
		}

		private void target_event (object sender, TargetEventArgs args)
		{
			switch (args.Type) {
			case TargetEventType.TargetExited:
			case TargetEventType.TargetSignaled:
				Gtk.Timeout.Add (1, new Gtk.Function (KillApplication));
				break;
			case TargetEventType.TargetStopped:
			case TargetEventType.TargetRunning:
				Gtk.Timeout.Add (1, new Gtk.Function (ChangeState));
				break;
			case TargetEventType.TargetHitBreakpoint:
			default:
				break;
			}
		}

		void insert_breakpoints ()
		{
			string[] keys = new string [breakpoints.Keys.Count];
			breakpoints.Keys.CopyTo (keys, 0);
			foreach (string key in keys) {
				Breakpoint point = CreateBreakpoint (key);
				string[] toks = point.Name.Split (':');
				string filename = toks [0];
				int linenumber = Int32.Parse (toks [1]);
				SourceLocation loc = backend.FindLocation(filename, linenumber);
				if (loc == null) {
					Console.WriteLine ("Couldn't find breakpoint location " + key + " " + backend.Modules.Length);
					return;
				}
				breakpoints [key] = loc.InsertBreakpoint (proc, point);
				if (breakpoints [key] == null)
					//throw new Exception ("Couldn't insert breakpoint " + key);
					return;
			}
		}

		bool EmitStarted ()
		{
			insert_breakpoints ();

			if (StartedEvent != null)
				StartedEvent (this, new EventArgs ());

			ChangeState ();

			return false;
		}

		bool ChangeState ()
		{
			if (IsRunning) {
				if (ResumedEvent != null) {
					ResumedEvent (this, new EventArgs ());
				}
			} else if (PausedEvent != null) {
				PausedEvent (this, new EventArgs ());
			}
			return false;
		}

		public event EventHandler PausedEvent;
		public event EventHandler ResumedEvent;
		public event EventHandler StartedEvent;
		public event EventHandler StoppedEvent;

		bool KillApplication ()
		{
			Cleanup ();
			return false;
		}

		public void Pause ()
		{
			if (!Debugging)
				//throw new Exception ("Debugger not running.");
				return;

			if (proc.IsStopped)
				return;

			proc.Stop ();
		}

		public void Resume ()
		{
			if (!Debugging)
				//throw new Exception ("Debugger not running.");
				return;

			if (!proc.IsStopped)
				return;

			proc.Continue (false);
		}

		public void Run (string[] argv)
		{
			if (Debugging)
				return;

			backend = new DebuggerBackend ();
			backend.ThreadManager.InitializedEvent += new ThreadEventHandler (initialized_event);
			backend.Run (new ProcessStart (null, argv));
		}

		public void Stop ()
		{
			Cleanup ();
		}

		public void StepInto ()
		{
			if (!Debugging)
				//throw new Exception ("Can't step without running debugger.");
				return;

			if (IsRunning)
				//throw new Exception ("Can't step unless paused.");
				return;

			proc.StepLine (false);
		}

		public void StepOver ()
		{
			if (!Debugging)
				//throw new Exception ("Can't step without running debugger.");
				return;

			if (IsRunning)
				//throw new Exception ("Can't step unless paused.");
				return;

			proc.NextLine (false);
		}

		public string[] Backtrace {
			get {
				Backtrace trace = proc.GetBacktrace ();
				string[] result = new string [trace.Length];
				int i = 0;
				foreach (StackFrame frame in trace.Frames)
					result [i++] = frame.SourceAddress.Name;

				return result;
			}
		}

		public object CurrentFrame {
			get {
				if (IsRunning)
					return null;
				return proc.CurrentFrame;
			}
		}

		public string CurrentFilename {
			get {
				if (IsRunning)
					return String.Empty;

				if (proc.CurrentFrame.SourceAddress.MethodSource.IsDynamic)
					return String.Empty;

				return proc.CurrentFrame.SourceAddress.MethodSource.SourceFile.FileName;
			}
		}

		public int CurrentLineNumber {
			get {
				if (IsRunning)
					return -1;

				return proc.CurrentFrame.SourceAddress.Row;
			}
		}

		public string LookupValue (string expr)
		{
			return "";
		}

		private void OnBreakpointHit (Breakpoint pointFromDbg, StackFrame frame)
		{
			point = pointFromDbg;
			Gtk.Timeout.Add (1, new Gtk.Function (MainThreadNotify));
		}

		bool MainThreadNotify ()
		{
			string[] toks = point.Name.Split (':');
			string filename = toks [0];
			int linenumber = Int32.Parse (toks [1]);

			if (this.BreakpointHit == null)
				return false;
			
			BreakpointHitArgs args = new BreakpointHitArgs (filename, linenumber);
			BreakpointHit (this, args);
			return false;
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
