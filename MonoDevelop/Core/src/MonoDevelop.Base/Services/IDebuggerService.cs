// IDebuggingService - Interface for the debugger to remove the depend on the
//                     debugger.
//
// Author: Todd Berman <tberman@sevenl.net>
//
// (C) 2004 Todd Berman

using System;

namespace MonoDevelop.Services
{

	public interface IDebuggableEditor {
		void ExecutingAt (int lineNumber);
		void ClearExecutingAt (int lineNumber);
	}

	public interface IDebuggingService {
		bool IsRunning { get; }
		bool AddBreakpoint (string filename, int linenum);
		void RemoveBreakpoint (string filename, int linenum);
		bool ToggleBreakpoint (string filename, int linenum);
		
		event EventHandler PausedEvent;
		event EventHandler ResumedEvent;
		event EventHandler StartedEvent;
		event EventHandler StoppedEvent;

		void Pause ();
		void Resume ();
		void Run (string[] args);
		void Stop ();

		void StepInto ();
		void StepOver ();

		string[] Backtrace { get; }

		string CurrentFilename { get; }
		int CurrentLineNumber { get; }

		string LookupValue (string expr);
	}
}
