// project created on 16.07.2002 at 18:07
using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.StartPage {
	
	public class ShowStartPageCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (view is StartPageView) {
					view.WorkbenchWindow.SelectWindow();
					return;
				}
			}
//			if (SharpDevelopMain.CommandLineArgs != null) {
				WorkbenchSingleton.Workbench.ShowView(new StartPageView());
//			}
		}
	}
}
