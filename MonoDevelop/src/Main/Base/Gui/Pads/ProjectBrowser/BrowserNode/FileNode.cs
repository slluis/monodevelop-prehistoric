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

using Gtk;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default file in the project browser.
	/// </summary>
	public class FileNode : AbstractBrowserNode
	{
		public readonly static string ProjectFileContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ProjectFileNode";
		public readonly static string DefaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/DefaultFileNode";
		
		/// <summary>
		/// Generates a Drag & Drop data object. If this property returns null
		/// the node indicates that it can't be dragged.
		/// </summary>
/*		public override DataObject DragDropDataObject {
			get {
				return new DataObject(this);
			}
		}
		
		// Let the folder the file is into handle the drag&drop effect and the 
		// drag & drop event. This makes the file class more useable		
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			Debug.Assert(Parent != null);
			return ((AbstractBrowserNode)Parent).GetDragDropEffect(dataObject, proposedEffect);
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			Debug.Assert(Parent != null);
			((AbstractBrowserNode)Parent).DoDragDrop(dataObject, effect);
		}
*/
	
		public FileNode(ProjectFile fileInformation)
		{
			UserData                 = fileInformation;
			contextmenuAddinTreePath = DefaultContextMenuPath;
			
			SetNodeLabel();
			SetNodeIcon();
		}
		
		protected virtual void SetNodeIcon()
		{
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			Image = iconService.GetImageForFile(((ProjectFile)userData).Name);
		}
		
		protected virtual void SetNodeLabel()
		{
			string text;
			if (ShowExtensions) {
				text = Path.GetFileName(((ProjectFile)userData).Name);
			} else {
				text = Path.GetFileNameWithoutExtension(((ProjectFile)userData).Name);
			}
			if (text != Text) {
				Text = text;
			}
		}
		
		public override void UpdateNaming()
		{
			SetNodeLabel();
			base.UpdateNaming();
		}
		
		public override void ActivateItem()
		{
			if (userData != null && userData is ProjectFile) {
				IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				
				fileService.OpenFile (((ProjectFile)userData).Name);
			}
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0) {
				if (userData == null || !(userData is ProjectFile)) {
					return;
				}
				string oldname = ((ProjectFile)userData).Name;
				
				// this forces an extension: bug# 59677
				//string oldExtension = Path.GetExtension(oldname);
				
				//if (Path.GetExtension(newName).Length == 0) {
				//	newName += oldExtension;
				//}
				
				string newname = Path.GetDirectoryName(oldname) + Path.DirectorySeparatorChar + newName;
				if (oldname != newname) {
					ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
					try {
						IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
						FileUtilityService fileUtilityService = (FileUtilityService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(FileUtilityService));
						if (fileUtilityService.IsValidFileName(newname)) {
							fileService.RenameFile(oldname, newname);
							SetNodeLabel();
							SetNodeIcon();
							UpdateBacking ();
						}
					} catch (System.IO.IOException) {   // assume duplicate file
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(GettextCatalog.GetString ("File or directory name is already in use, choose a different one."));
					} catch (System.ArgumentException) { // new file name with wildcard (*, ?) characters in it
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(GettextCatalog.GetString ("The file name you have chosen contains illegal characters. Please choose a different file name."));
					}
				}
			}
		}
		
		/// <summary>
		/// Removes a file from a project
		/// </summary>
		public override bool RemoveNode()
		{
			DateTime old = DateTime.Now;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			using (MessageDialog dialog = new Gtk.MessageDialog ((Window) WorkbenchSingleton.Workbench, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, String.Format (GettextCatalog.GetString ("Are you sure you want to remove file {0} from project {1}?"), Path.GetFileName (((ProjectFile)userData).Name), Project.Name))) {
			
				if (dialog.Run() != (int)Gtk.ResponseType.Yes) {
					dialog.Hide ();
					return false;
				}
			
				dialog.Hide ();
			}
			//switch (sharpMessageBox.ShowMessageBox()) {
			//	case -1:
			//	case 2:
			//		return false;
			//	case 0:
					IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					projectService.RemoveFileFromProject(((ProjectFile)userData).Name);
			//		break;
			//	case 1:
			//		IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			//		fileService.RemoveFile(((ProjectFile)userData).Name);
			//		break;
			//}
			return true;
		}
	}
}
