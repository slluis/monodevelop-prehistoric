// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;

using MonoDevelop.Core.Properties;

using MonoDevelop.Core.Services;

using MonoDevelop.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default directory in the project browser.
	/// </summary>
	public class DirectoryNode : FolderNode 
	{
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/DefaultDirectoryNode";
				
		string folderName;
		
		/// <summary>
		/// This property gets the name of a directory for a 
		/// 'directory' folder. 
		/// </summary>
		public string FolderName {
			get {
				return folderName;
			}
			set {
				folderName = value;
				canLabelEdited = true;
			}
		}
		
		public DirectoryNode(string folderName) : base(Path.GetFileName(folderName))
		{
			this.folderName = folderName;
			canLabelEdited  = true;
			contextmenuAddinTreePath = defaultContextMenuPath;
			
			OpenedImage = Stock.OpenFolderBitmap;
			ClosedImage = Stock.ClosedFolderBitmap;
		}
		
/*
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			if (dataObject.GetDataPresent(typeof(FileNode)) && DragDropUtil.CanDrag((FileNode)dataObject.GetData(typeof(FileNode)), this)) {				
				return proposedEffect;
			}
			if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				return proposedEffect;
			}
			return DragDropEffects.None;
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			if (dataObject.GetDataPresent(typeof(FileNode))) {
				FileNode fileNode = DragDropUtil.DoDrag((FileNode)dataObject.GetData(typeof(FileNode)), this, effect);
				DragDropUtil.DoDrop(fileNode, folderName, effect);
			} else if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
				foreach (string file in files) {
					try {
						ProjectBrowserView.MoveCopyFile(file, this, effect == DragDropEffects.Move, false);
					} catch (Exception ex) {
						Console.WriteLine(ex.ToString());
					}
				}
			} else {
				throw new System.NotImplementedException();
			}
		}
*/
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0) {
				
				string oldFoldername = folderName;
				string newFoldername = Path.GetDirectoryName(oldFoldername) + Path.DirectorySeparatorChar + newName;
				
				if (oldFoldername != newFoldername) {
					try {
						
						IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
						FileUtilityService fileUtilityService = (FileUtilityService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(FileUtilityService));
						if (fileUtilityService.IsValidFileName(newFoldername)) {
							fileService.RenameFile(oldFoldername, newFoldername);
							Text       = newName;
							folderName = newFoldername;
						}
					} catch (System.IO.IOException) {   // assume duplicate file
						IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
						messageService.ShowError(GettextCatalog.GetString ("File or directory name is already in use, choose a different one."));
					} catch (System.ArgumentException) { // new file name with wildcard (*, ?) characters in it
						IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
						messageService.ShowError(GettextCatalog.GetString ("The file name you have chosen contains illegal characters. Please choose a different file name."));
					}
				}
			}
		}
		
		/// <summary>
		/// Removes a folder from a project.
		/// Note : The FolderName property must be set for this method to work.
		/// </summary>
		public override bool RemoveNode()
		{
			if (FolderName != null && FolderName.Length == 0) {
				return false;
			}
			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			
			Gtk.MessageDialog dialog = new Gtk.MessageDialog ((Gtk.Window)WorkbenchSingleton.Workbench, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Question, Gtk.ButtonsType.OkCancel, String.Format (GettextCatalog.GetString ("Do you want to remove folder {0} from project {1}?"), Text, Project.Name));

			if (dialog.Run() != (int)Gtk.ResponseType.Ok) {
				dialog.Destroy ();
				return false;
			}
			dialog.Destroy ();
			
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			//switch (ret) {
				//case 0:
					projectService.RemoveFileFromProject(FolderName);
				//	break;
				//case 1:
				//	fileService.RemoveFile(FolderName);
				//	break;
			//}
			return true;
		}
	}
}
