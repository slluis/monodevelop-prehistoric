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
			((DefaultParserService)Runtime.ParserService).StartParserThread();
		}
	}

	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		public override void Run()
		{
			// register string tag provider (TODO: move to add-in tree :)
			Runtime.StringParserService.RegisterStringTagProvider(new MonoDevelop.Commands.SharpDevelopStringTagProvider());
			
			// load previous combine
			if ((bool)Runtime.Properties.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				RecentOpen recentOpen = Runtime.FileService.RecentOpen;

				if (recentOpen.RecentProject != null && recentOpen.RecentProject.Length > 0) { 
					Runtime.ProjectService.OpenCombine(recentOpen.RecentProject[0].ToString());
				}
			}
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				//FIXME: use mimetypes
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
						try {
							Runtime.ProjectService.OpenCombine (file);
						} catch (Exception e) {
							CombineLoadError.HandleError(e, file);
						}
						
						break;
					default:
						try {
							Runtime.FileService.OpenFile (file);
						
						} catch (Exception e) {
							Console.WriteLine("unable to open file {0} exception was :\n{1}", file, e.ToString());
						}
						break;
				}
			}
			
			((Gtk.Window)WorkbenchSingleton.Workbench).ShowAll ();
			WorkbenchSingleton.Workbench.SetMemento ((IXmlConvertable)Runtime.Properties.GetProperty (workbenchMemento, new WorkbenchMemento ()));
			((Gtk.Window)WorkbenchSingleton.Workbench).Visible = true;
			WorkbenchSingleton.Workbench.RedrawAllComponents ();
			((Gtk.Window)WorkbenchSingleton.Workbench).Present ();
		
			// finally run the workbench window ...
			Gtk.Application.Run ();
		}
	}
}
