
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
			Image = Runtime.Gui.Icons.GetImageForFile (((ProjectFile)userData).Name);
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
			if (userData != null && userData is ProjectFile)
				Runtime.FileService.OpenFile (((ProjectFile)userData).FilePath);
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0) {
				if (userData == null || !(userData is ProjectFile)) {
					return;
				}
				string oldname = ((ProjectFile)userData).Name;
				
				string newname = Path.GetDirectoryName(oldname) + Path.DirectorySeparatorChar + newName;
				if (oldname != newname) {
					try {
						if (Runtime.FileUtilityService.IsValidFileName(newname)) {
							Runtime.FileService.RenameFile(oldname, newname);
							SetNodeLabel();
							SetNodeIcon();
							UpdateBacking ();
						}
					} catch (System.IO.IOException) {   // assume duplicate file
						Runtime.MessageService.ShowError (GettextCatalog.GetString ("File or directory name is already in use, choose a different one."));
					} catch (System.ArgumentException) { // new file name with wildcard (*, ?) characters in it
						Runtime.MessageService.ShowError (GettextCatalog.GetString ("The file name you have chosen contains illegal characters. Please choose a different file name."));
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
			
			bool yes = Runtime.MessageService.AskQuestion (String.Format (GettextCatalog.GetString ("Are you sure you want to remove file {0} from project {1}?"), Path.GetFileName (((ProjectFile)userData).Name), Project.Name));
			if (!yes)
				return false;

			Runtime.ProjectService.RemoveFileFromProject (((ProjectFile)userData).Name);
			return true;
		}
	}
}
