using System;

using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Debugger.Commands
{

	public class ToggleRunning : AbstractMenuCommand
	{
		public override void Run ()
		{
			if (Runtime.DebuggingService.IsRunning)
				Runtime.DebuggingService.Pause ();
			else
				Runtime.DebuggingService.Resume ();
		}
	}

	public class KillApplication : AbstractMenuCommand
	{
		public override void Run ()
		{
			Runtime.DebuggingService.Stop();
		}
	}

	public class StepOver : AbstractMenuCommand
	{
		public override void Run ()
		{
			Runtime.DebuggingService.StepOver();
		}
	}

	public class StepInto : AbstractMenuCommand
	{
		public override void Run ()
		{
			Runtime.DebuggingService.StepInto();
		}
	}

	public class DebugProject : AbstractMenuCommand
	{

		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)Runtime.DebuggingService;
			
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				if (Runtime.ProjectService.NeedsCompiling) {
					Runtime.ProjectService.BuildActiveCombine ().WaitForCompleted ();
				}
#if NET_2_0
				dbgr.AttributeHandler.Rescan();
#endif

				Runtime.ProjectService.CurrentOpenCombine.Debug (dbgr.DebugProgressMonitor);
			}

		}

	}

}
