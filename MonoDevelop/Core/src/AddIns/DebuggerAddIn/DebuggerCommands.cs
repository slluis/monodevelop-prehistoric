using System;

using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;
using MonoDevelop.Commands;


namespace MonoDevelop.Debugger
{
	public enum DebugCommands
	{
		ToggleRunning,
		StepOver,
		StepInto
	}
}

namespace MonoDevelop.Debugger.Commands
{
	public class ToggleRunning : CommandHandler
	{
		protected override void Run ()
		{
			if (Runtime.DebuggingService.IsRunning)
				Runtime.DebuggingService.Pause ();
			else
				Runtime.DebuggingService.Resume ();
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = ((DebuggingService)Runtime.DebuggingService).Debugging;
		}
	}

	public class StepOver : CommandHandler
	{
		protected override void Run ()
		{
			Runtime.DebuggingService.StepOver();
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = ((DebuggingService)Runtime.DebuggingService).Debugging;
		}
	}

	public class StepInto : CommandHandler
	{
		protected override void Run ()
		{
			Runtime.DebuggingService.StepInto();
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = ((DebuggingService)Runtime.DebuggingService).Debugging;
		}
	}
}
