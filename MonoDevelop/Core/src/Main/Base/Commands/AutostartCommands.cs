// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ?Â¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Gui;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.ErrorHandlers;

using MonoDevelop.Internal.Parser;
using MonoDevelop.Services;
using SA = MonoDevelop.SharpAssembly.Assembly;

namespace MonoDevelop.Commands
{
	public class InitializeWorkbenchCommand : AbstractCommand
	{
		public override void Run()
		{
			DefaultWorkbench w = new DefaultWorkbench();
			WorkbenchSingleton.Workbench = w;
			w.InitializeWorkspace();
			w.UpdateViews(null, null);
			WorkbenchSingleton.CreateWorkspace();
			((Gtk.Window)w).Visible = false;
		}
	}
	
	public class StartParserServiceThread : AbstractCommand
	{
		public override void Run()
		{
			DefaultParserService parserService = (DefaultParserService)ServiceManager.GetService(typeof(DefaultParserService));
			parserService.StartParserThread();
		}
	}

	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		public override void Run()
		{
			// register string tag provider (TODO: move to add-in tree :)
			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			stringParserService.RegisterStringTagProvider(new MonoDevelop.Commands.SharpDevelopStringTagProvider());
			
			PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
			
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			
			// load previous combine
			if ((bool)propertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				object recentOpenObj = propertyService.GetProperty("MonoDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is MonoDevelop.Services.RecentOpen) {
					MonoDevelop.Services.RecentOpen recOpen = (MonoDevelop.Services.RecentOpen)recentOpenObj;
					if (recOpen.RecentProject != null && recOpen.RecentProject.Length > 0) { 
						projectService.OpenCombine(recOpen.RecentProject[0].ToString());
					}
				}
			}
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				//FIXME: use mimetypes
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
						try {
							projectService.OpenCombine(file);
						} catch (Exception e) {
							CombineLoadError.HandleError(e, file);
						}
						
						break;
					default:
						try {
							IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
							fileService.OpenFile(file);
						
						} catch (Exception e) {
							Console.WriteLine("unable to open file {0} exception was :\n{1}", file, e.ToString());
						}
						break;
				}
			}
			
			((Gtk.Window)WorkbenchSingleton.Workbench).ShowAll ();
			WorkbenchSingleton.Workbench.SetMemento ((IXmlConvertable)propertyService.GetProperty (workbenchMemento, new WorkbenchMemento ()));
			((Gtk.Window)WorkbenchSingleton.Workbench).Visible = true;
			WorkbenchSingleton.Workbench.RedrawAllComponents ();
			((Gtk.Window)WorkbenchSingleton.Workbench).Present ();
		
			// finally run the workbench window ...
			Gtk.Application.Run ();
		}
	}
}
