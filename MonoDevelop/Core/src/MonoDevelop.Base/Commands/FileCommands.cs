// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.ErrorHandlers;

namespace MonoDevelop.Commands
{
	public class CreateNewProject : AbstractMenuCommand
	{
		public override void Run ()
		{
			new NewProjectDialog (true);
		}
	}
	
	public class CreateNewFile : AbstractMenuCommand
	{
		public override void Run ()
		{
			new NewFileDialog ();
		}
	}
	
	public class CloseFile : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.CloseWindow(false, true, 0);
			}
		}
	}
	
	public class CloseAllFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			if ( WorkbenchSingleton.Workbench != null ) {
				WorkbenchSingleton.Workbench.CloseAllViews();
			}
		}
	}

	public class SaveFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null) {
				if (window.ViewContent.IsViewOnly) {
					return;
				}
				
				if (window.ViewContent.ContentName == null) {
					SaveFileAs sfa = new SaveFileAs();
					sfa.Run();
				} else {
					FileAttributes attr = FileAttributes.ReadOnly | FileAttributes.Directory | FileAttributes.Offline | FileAttributes.System;
					// FIXME
					// bug #59731 is if the file is moved out from under us, we were crashing
					// I changed it to make it ask for a new
					// filename instead, but maybe we should
					// detect the move and update the reference
					// to the name instead
					if (!File.Exists (window.ViewContent.ContentName) || (File.GetAttributes(window.ViewContent.ContentName) & attr) != 0) {
						SaveFileAs sfa = new SaveFileAs();
						sfa.Run();
					} else {						
						Runtime.ProjectService.MarkFileDirty(window.ViewContent.ContentName);
						string fileName = window.ViewContent.ContentName;
						// save backup first						
						if((bool) Runtime.Properties.GetProperty ("SharpDevelop.CreateBackupCopy", false)) {
							Runtime.FileUtilityService.ObservedSave (new NamedFileOperationDelegate(window.ViewContent.Save), fileName + "~");
						}
						Runtime.FileUtilityService.ObservedSave (new NamedFileOperationDelegate(window.ViewContent.Save), fileName);
					}
				}
			}
		}
		
		public override bool IsEnabled {
			get {
				IWorkbenchWindow window   = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				IViewContent content = window != null ? window.ActiveViewContent as IViewContent : null;
				if (content != null) {
					return content.IsDirty;
				}
				return false;
			}
		}
	} 

	public class ReloadFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ViewContent.ContentName != null && !window.ViewContent.IsViewOnly) {
				if (Runtime.MessageService.AskQuestion(GettextCatalog.GetString ("Are you sure that you want to reload the file?"))) {
					IXmlConvertable memento = null;
					if (window.ViewContent is IMementoCapable) {
						memento = ((IMementoCapable)window.ViewContent).CreateMemento();
					}
					window.ViewContent.Load(window.ViewContent.ContentName);
					if (memento != null) {
						((IMementoCapable)window.ViewContent).SetMemento(memento);
					}
				}
			}
		}
	}
	
	public class SaveFileAs : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null) {
				if (window.ViewContent.IsViewOnly) {
					return;
				}
				if (window.ViewContent is ICustomizedCommands) {
					if (((ICustomizedCommands)window.ViewContent).SaveAsCommand()) {
						return;
					}
				}
				/*
				using (SaveFileDialog fdiag = new SaveFileDialog()) {
					fdiag.OverwritePrompt = true;
					fdiag.AddExtension    = true;
					
					string[] fileFilters  = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
					fdiag.Filter          = String.Join("|", fileFilters);
					for (int i = 0; i < fileFilters.Length; ++i) {
						if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.ContentName == null ? window.ViewContent.UntitledName : window.ViewContent.ContentName)) >= 0) {
							fdiag.FilterIndex = i + 1;
							break;
						}
					}*/
					FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Save as..."), Gtk.FileChooserAction.Save);
					fdiag.SetFilename (window.ViewContent.ContentName);
					int response = fdiag.Run ();
					string filename = fdiag.Filename;
					fdiag.Hide ();
				
					if (response == (int)Gtk.ResponseType.Ok) {
						if (!Runtime.FileUtilityService.IsValidFileName (filename)) {
							Runtime.MessageService.ShowMessage(String.Format (GettextCatalog.GetString ("File name {0} is invalid"), filename));
							return;
						}

					// save backup first
					if((bool) Runtime.Properties.GetProperty ("SharpDevelop.CreateBackupCopy", false)) {
						Runtime.FileUtilityService.ObservedSave (new NamedFileOperationDelegate(window.ViewContent.Save), filename + "~");
					}
					
					// do actual save
					if (Runtime.FileUtilityService.ObservedSave (new NamedFileOperationDelegate(window.ViewContent.Save), filename) == FileOperationResult.OK) {
						Runtime.FileService.RecentOpen.AddLastFile (filename, null);
					}
				}
			}
		}
	}
	
	public class SaveAllFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.IsViewOnly) {
					continue;
				}
				
				if (content.ContentName == null)
				{
					using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Save File As...")))
					{
						fdiag.SetFilename (System.Environment.GetEnvironmentVariable ("HOME"));
						if (fdiag.Run () == (int) Gtk.ResponseType.Ok)
						{
							string fileName = fdiag.Filename;

							// currently useless
							if (Path.GetExtension(fileName).StartsWith("?") || Path.GetExtension(fileName) == "*")
							{
								fileName = Path.ChangeExtension(fileName, "");
							}

							if (Runtime.FileUtilityService.ObservedSave (new NamedFileOperationDelegate(content.Save), fileName) == FileOperationResult.OK)
							{
								Runtime.MessageService.ShowMessage(fileName, GettextCatalog.GetString ("File saved"));
							}
						}
					
						fdiag.Hide ();
					}
				}
				else
				{
					Runtime.FileUtilityService.ObservedSave (new FileOperationDelegate(content.Save), content.ContentName);
				}
			}
		}
	}
	
	public class OpenCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			using (FileSelector fs = new FileSelector (GettextCatalog.GetString ("File to Open"))) {
				int response = fs.Run ();
				string name = fs.Filename;
				fs.Hide ();

				if (response == (int)Gtk.ResponseType.Ok) {
					switch (Path.GetExtension(name).ToUpper()) {
						case ".CMBX": // Don't forget the 'recent' projects if you chance something here
						case ".PRJX":
							try {
								//Runtime.ProjectService.OpenCombine(name);
								Runtime.FileService.OpenFile(name);
							} catch (Exception ex) {
								CombineLoadError.HandleError(ex, name);
							}
							break;
						default:
							Runtime.MessageService.ShowError(String.Format (GettextCatalog.GetString ("Can't open file {0} as project"), name));
							break;
					}
				}
			}
		}
	}
	
	public class OpenFile : AbstractMenuCommand
	{
		public override void Run()
		{
			//string[] fileFilters  = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
			//bool foundFilter      = false;
			// search filter like in the current selected project
			/*
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			
			if (projectService.CurrentSelectedProject != null) {
				LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
				
				LanguageBindingCodon languageCodon = languageBindingService.GetCodonPerLanguageName(projectService.CurrentSelectedProject.ProjectType);
				for (int i = 0; !foundFilter && i < fileFilters.Length; ++i) {
					for (int j = 0; !foundFilter && j < languageCodon.Supportedextensions.Length; ++j) {
						if (fileFilters[i].IndexOf(languageCodon.Supportedextensions[j]) >= 0) {
							break;
						}
					}
				}
			}
			
			// search filter like in the current open file
			if (!foundFilter) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window != null) {
					for (int i = 0; i < fileFilters.Length; ++i) {
						if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.ContentName == null ? window.ViewContent.UntitledName : window.ViewContent.ContentName)) >= 0) {
							break;
						}
					}
				}
			}*/
			using (FileSelector fs = new FileSelector (GettextCatalog.GetString ("File to Open"))) {
				int response = fs.Run ();
				string name = fs.Filename;
				fs.Hide ();
				if (response == (int)Gtk.ResponseType.Ok) {
					if (Runtime.ProjectService.IsCombineEntryFile (name))
						Runtime.ProjectService.OpenCombine (name);
					else
						Runtime.FileService.OpenFile(name);
				}	
			}
		}
	}
	
	public class ClearCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			Runtime.ProjectService.CloseCombine();
		}
	}
		
	public class ExitWorkbenchCommand : AbstractMenuCommand
	{
		public override void Run()
		{			
			if (((DefaultWorkbench)WorkbenchSingleton.Workbench).Close()) {
				Gtk.Application.Quit();
			}
		}
	}
	
	public class Print : AbstractMenuCommand
	{
		public override void Run()
		{/*
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null) {
				if (window.ViewContent is IPrintable) {
					PrintDocument pdoc = ((IPrintable)window.ViewContent).PrintDocument;
					if (pdoc != null) {
						using (PrintDialog ppd = new PrintDialog()) {
							ppd.Document  = pdoc;
							ppd.AllowSomePages = true;
							if (ppd.ShowDialog() == DialogResult.OK) { // fixed by Roger Rubin
								pdoc.Print();
							}
						}
					} else {
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError("Couldn't create PrintDocument");
					}
				} else {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError("Can't print this window content");
				}
			}*/
		}
	}
	
	public class PrintPreview : AbstractMenuCommand
	{
		public override void Run()
		{
		/*	try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				if (window != null) {
					if (window.ViewContent is IPrintable) {
						using (PrintDocument pdoc = ((IPrintable)window.ViewContent).PrintDocument) {
							if (pdoc != null) {
								PrintPreviewDialog ppd = new PrintPreviewDialog();
								ppd.Owner     = (Form)WorkbenchSingleton.Workbench;
								ppd.TopMost   = true;
								ppd.Document  = pdoc;
								ppd.Show();
							} else {
								IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
								messageService.ShowError("Couldn't create PrintDocument");
							}
						}
					}
				}
			} catch (System.Drawing.Printing.InvalidPrinterException) {
			}*/
		}
	}
	
	public class ClearRecentFiles : AbstractMenuCommand
	{
		public override void Run()
		{			
			try {
				if (Runtime.FileService.RecentOpen.RecentFile != null && Runtime.FileService.RecentOpen.RecentFile.Length > 0 && Runtime.MessageService.AskQuestion(GettextCatalog.GetString ("Are you sure you want to clear recent files list?"), GettextCatalog.GetString ("Clear recent files")))
				{
					Runtime.FileService.RecentOpen.ClearRecentFiles();
				}
			} catch {}
		}
	}
	
	public class ClearRecentProjects : AbstractMenuCommand
	{
		public override void Run()
		{			
			try {
				if (Runtime.FileService.RecentOpen.RecentProject != null && Runtime.FileService.RecentOpen.RecentProject.Length > 0 && Runtime.MessageService.AskQuestion(GettextCatalog.GetString ("Are you sure you want to clear recent projects list?"), GettextCatalog.GetString ("Clear recent projects")))
				{
					Runtime.FileService.RecentOpen.ClearRecentProjects();
				}
			} catch {}
		}
	}
}
