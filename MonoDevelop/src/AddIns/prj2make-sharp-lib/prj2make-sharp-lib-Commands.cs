using System;
using System.IO;

using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;
using MonoDevelop.Prj2Make;
using MonoDevelop.Prj2Make.Schema.Prjx;
using MonoDevelop.Prj2Make.Schema.Csproj;

namespace MonoDevelop.Commands
{
	public class ImportPrj : AbstractMenuCommand
	{
		static PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		
		public override void Run()
		{
			using (Gtk.FileSelection fs = new Gtk.FileSelection (GettextCatalog.GetString ("File to Open"))) {
				bool conversionSuccessfull = false;
				SlnMaker slnMkObj = null;
				string defaultFolder = PropertyService.GetProperty(
						"MonoDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", 
					System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
						"MonoDevelopProjects")).ToString();
				fs.Complete (defaultFolder);
				int response = fs.Run ();
				string name = fs.Filename;
				fs.Hide ();
				IProjectService proj = null;

				if (response == (int)Gtk.ResponseType.Ok) {
					switch (Path.GetExtension(name).ToUpper()) {
						case ".SLN": 
							slnMkObj = new SlnMaker();
							// Load the sln and parse it
							slnMkObj.MsSlnToCmbxHelper(name);
							conversionSuccessfull = true;
							name = slnMkObj.CmbxFileName;
							break;
						case ".CSPROJ":
							slnMkObj = new SlnMaker();
							// Load the csproj and parse it
							slnMkObj.CreatePrjxFromCsproj(name);
							conversionSuccessfull = true;
							name = slnMkObj.PrjxFileName;
							break;
						default:
							IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
							messageService.ShowError(String.Format (GettextCatalog.GetString ("Can't open file {0} as project"), name));
							break;
					}
					if (conversionSuccessfull == true) {
						try {
							proj = (IProjectService)ServiceManager.Services.GetService (typeof (IProjectService));
							proj.OpenCombine(name);
						} catch (Exception ex) {
							Console.WriteLine(ex.Message);
						}
					}
				}
			}
		}
	}

}
