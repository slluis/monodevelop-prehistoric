// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Diagnostics;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads;
using MonoDevelop.Gui.Pads.ProjectPad;

using Gtk;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddFilesToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			if (nav == null) return;
			
			Project project = nav.GetParentDataItem (typeof(Project), true) as Project;
			
			using (FileSelector fdiag  = new FileSelector (GettextCatalog.GetString ("Add files"))) {
				fdiag.SelectMultiple = true;
				
				int result = fdiag.Run ();
				try {
					if (result != (int) ResponseType.Ok)
						return;
					
					foreach (string file in fdiag.Filenames) {
						if (file.StartsWith (project.BaseDirectory)) {
							MoveCopyFile (project, nav, file, true, true);
						} else {
							using (MessageDialog md = new MessageDialog (
																		 (Window) WorkbenchSingleton.Workbench,
																		 DialogFlags.Modal | DialogFlags.DestroyWithParent,
																		 MessageType.Question, ButtonsType.None,
																		 String.Format (GettextCatalog.GetString ("{0} is outside the project directory, what should I do?"), file))) {
								md.AddButton (Gtk.Stock.Copy, 1);
								md.AddButton (GettextCatalog.GetString ("_Move"), 2);
								md.AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
								
								int ret = md.Run ();
								md.Hide ();
								
								if (ret < 0)
									return;

								try {
									MoveCopyFile (project, nav, file, ret == 2, false);
								}
								catch {
									Runtime.MessageService.ShowError (GettextCatalog.GetString ("An error occurred while attempt to move/copy that file. Please check your permissions."));
								}
							}
						}
					}
				} finally {
					fdiag.Hide ();
				}
			}
		}
		
		public static void MoveCopyFile (Project project, ITreeNavigator nav, string filename, bool move, bool alreadyInPlace)
		{
			if (Runtime.FileUtilityService.IsDirectory (filename))
			    return;

			ProjectFolder folder = nav.GetParentDataItem (typeof(ProjectFolder), true) as ProjectFolder;
			
			string name = System.IO.Path.GetFileName (filename);
			string baseDirectory = folder != null ? folder.Path : project.BaseDirectory;
			string newfilename = alreadyInPlace ? filename : Path.Combine (baseDirectory, name);

			if (filename != newfilename) {
				File.Copy (filename, newfilename);
				if (move)
					Runtime.FileService.RemoveFile (filename);
			}
			
			if (project.IsCompileable (newfilename)) {
				project.AddFile (newfilename, BuildAction.Compile);
			} else {
				project.AddFile (newfilename, BuildAction.Nothing);
			}

			Runtime.ProjectService.SaveCombine();
		}
	}
	
	public class AddNewFileEvent : AbstractMenuCommand
	{
		string baseFolderPath;
		SolutionPad browser;
		Project project;

		public override void Run()
		{
			browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			project = nav.GetParentDataItem (typeof(Project), true) as Project;
			if (project == null) return;
			
			ProjectFolder folder = nav.GetParentDataItem (typeof(ProjectFolder), true) as ProjectFolder;
			baseFolderPath = folder != null ? folder.Path : project.BaseDirectory;
			
			NewFileDialog nfd = new NewFileDialog ();
			nfd.OnOked += new EventHandler (newfileOked);
		}

		void newfileOked (object o, EventArgs e)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			int count = 1;
				
			string baseName  = Path.GetFileNameWithoutExtension(window.ViewContent.UntitledName);
			string extension = Path.GetExtension(window.ViewContent.UntitledName);
				
			// first try the default untitled name of the viewcontent filename
			string fileName = Path.Combine (baseFolderPath, baseName +  extension);
				
			// if it is already in the project, or it does exists we try to get a name that is
			// untitledName + Numer + extension
			while (project.IsFileInProject (fileName) || System.IO.File.Exists (fileName)) {
				fileName = Path.Combine (baseFolderPath, baseName + count.ToString() + extension);
				++count;
			}

			// now we have a valid filename which we could use
			window.ViewContent.Save (fileName);
				
			ProjectFile newFileInformation = new ProjectFile(fileName, BuildAction.Compile);
			project.ProjectFiles.Add (newFileInformation);
			Runtime.ProjectService.SaveCombine();

			browser.AddNodeInsertCallback (newFileInformation, new TreeNodeCallback (OnFileInserted));
		}
		
		void OnFileInserted (ITreeNavigator nav)
		{
			browser.StealFocus ();
			nav.Selected = true;
			browser.StartLabelEdit ();
		}
	}
	
	public class NewFolderEvent : AbstractMenuCommand
	{
		SolutionPad browser;
		
		public override void Run()
		{
			browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			if (nav == null) return;
			
			Project project = nav.GetParentDataItem (typeof(Project), true) as Project;
			if (project == null) return;

			ProjectFolder folder = nav.GetParentDataItem (typeof(ProjectFolder), true) as ProjectFolder;
			string baseFolderPath = folder != null ? folder.Path : project.BaseDirectory;

			string directoryName = Path.Combine (baseFolderPath, GettextCatalog.GetString("New Folder"));
			int index = -1;

			if (Directory.Exists(directoryName)) {
				while (Directory.Exists(directoryName + (++index + 1))) ;
			}
			
			if (index >= 0) {
				directoryName += index + 1;
			}
			
			Directory.CreateDirectory (directoryName);
			
			ProjectFile newFolder = new ProjectFile (directoryName);
			newFolder.Subtype = Subtype.Directory;
			project.ProjectFiles.Add (newFolder);

			browser.AddNodeInsertCallback (newFolder, new TreeNodeCallback (OnFolderInserted));
		}
		
		void OnFolderInserted (ITreeNavigator nav)
		{
			browser.StealFocus ();
			nav.Selected = true;
			browser.StartLabelEdit();
		}
	}
	
	public class IncludeFileToProject : AbstractMenuCommand
	{
		public override void Run ()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			if (nav == null) return;
			
			Project project = nav.GetParentDataItem (typeof(Project), true) as Project;
			
			if (nav.DataItem is SystemFile) {
				SystemFile file = (SystemFile) nav.DataItem;
				if (project.IsCompileable (file.Path))
					project.AddFile (file.Path, BuildAction.Compile);
				else
					project.AddFile (file.Path, BuildAction.Nothing);
			}
		}
	}
}
