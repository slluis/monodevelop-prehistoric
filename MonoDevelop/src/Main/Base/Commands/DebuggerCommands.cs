using System;

using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Commands
{

	public class ToggleRunning : AbstractMenuCommand
	{
		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.Services.GetService (typeof (DebuggingService));
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
			DebuggingService dbgr = (DebuggingService)ServiceManager.Services.GetService (typeof (DebuggingService));

			dbgr.Stop ();
		}
	}

	public class StepOver : AbstractMenuCommand
	{
		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.Services.GetService (typeof (DebuggingService));
			
			dbgr.StepOver ();
		}
	}

	public class StepInto : AbstractMenuCommand
	{
		public override void Run ()
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.Services.GetService (typeof (DebuggingService));

			dbgr.StepInto ();
		}
	}

	public class DebugProject : AbstractMenuCommand
	{

		public override void Run ()
		{

			IProjectService projServ = (IProjectService)ServiceManager.Services.GetService (typeof (IProjectService));
			
			if (projServ.CurrentOpenCombine != null) {
				//try {
					if (projServ.NeedsCompiling) {
						projServ.CompileCombine ();
					}
					projServ.OnBeforeStartProject ();
					projServ.CurrentOpenCombine.Debug ();
				//} catch {
				//	IMessageService msgServ = (IMessageService)ServiceManager.Services.GetService (typeof (IMessageService));
				//	msgServ.ShowError ("Can't execute the debugger");
				//}
			}

		}

	}

}
