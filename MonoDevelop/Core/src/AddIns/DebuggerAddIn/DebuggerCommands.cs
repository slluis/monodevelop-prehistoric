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
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			if (dbgr.IsRunning)
				dbgr.Pause ();
			else
				dbgr.Resume ();
		}
	}

	public class KillApplication : AbstractMenuCommand
	{
		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));

			dbgr.Stop ();
		}
	}

	public class StepOver : AbstractMenuCommand
	{
		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			
			dbgr.StepOver ();
		}
	}

	public class StepInto : AbstractMenuCommand
	{
		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));

			dbgr.StepInto ();
		}
	}

	public class DebugProject : AbstractMenuCommand
	{

		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			IProjectService projServ = (IProjectService)ServiceManager.GetService (typeof (IProjectService));
			
			if (projServ.CurrentOpenCombine != null) {
				//try {
					if (projServ.NeedsCompiling) {
						projServ.BuildActiveCombine ().WaitForCompleted ();
					}
					//					if (projServ.BeforeStartProject != null)
					//						projServ.BeforeStartProject (projServ, null);

#if NET_2_0
					dbgr.AttributeHandler.Rescan();
#endif

					projServ.CurrentOpenCombine.Debug (dbgr.DebugProgressMonitor);
				//} catch {
				//	IMessageService msgServ = (IMessageService)ServiceManager.Services.GetService (typeof (IMessageService));
				//	msgServ.ShowError ("Can't execute the debugger");
				//}
			}

		}

	}

}
