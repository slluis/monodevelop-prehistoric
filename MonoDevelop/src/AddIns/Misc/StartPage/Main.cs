// project created on 16.07.2002 at 18:07
using System;
using System.IO;
using System.Drawing;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;

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
			//if (SharpDevelopMain.CommandLineArgs != null) {
				//StartPageView spv = new StartPageView ();
				//WorkbenchSingleton.Workbench.ShowView(spv);

				//PropertyService ps = (PropertyService) ServiceManager.Services.GetService (typeof(PropertyService));
				//string base_uri = ps.DataDirectory + Path.DirectorySeparatorChar +                                                                                
				//".." + Path.DirectorySeparatorChar +
				//"data" + Path.DirectorySeparatorChar +
				//"resources" + Path.DirectorySeparatorChar +                             "startpage";
				//spv.DelayedInitialize ("file://" + base_uri);
			//}
		}
	}
}
