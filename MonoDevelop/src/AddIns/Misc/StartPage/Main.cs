// project created on 16.07.2002 at 18:07
using System;
using System.IO;
using System.Drawing;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Gui;
using MonoDevelop.Core.Services;

namespace MonoDevelop.StartPage {
	
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
