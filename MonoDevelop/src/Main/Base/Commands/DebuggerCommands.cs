using System;

using MonoDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace MonoDevelop.Commands
{

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
					IMessageService msgServ = (IMessageService)ServiceManager.Services.GetService (typeof (IMessageService));
					msgServ.ShowError ("Can't execute the debugger");
				//}
			}

		}

	}

}
