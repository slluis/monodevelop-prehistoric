// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.Properties;

using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default project in the project browser.
	/// </summary>
	public class ProjectBrowserNode : AbstractBrowserNode 
	{
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ProjectBrowserNode";
		IProject project;
		ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
		
		public override IProject Project {
			get {
				return project;
			}
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
				DragDropUtil.DoDrop(fileNode, Project.BaseDirectory, effect);
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
		
		public ProjectBrowserNode(IProject project)
		{
			UserData     = project;
			this.project = project;
			Text         = project.Name;
			contextmenuAddinTreePath = defaultContextMenuPath;
			project.NameChanged += new EventHandler(ProjectNameChanged);
		}
		
		public override void Dispose()
		{
			base.Dispose();
			project.NameChanged -= new EventHandler(ProjectNameChanged);
		}
		
		void ProjectNameChanged(object sender, EventArgs e)
		{
			Text = project.Name;
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0 && newName != project.Name) {
				project.Name = newName;
			}
		}
		
		/// <summary>
		/// Removes a project from a combine
		/// NOTE : This method assumes that its parent is != null and that it is
		/// from the CombineBrowserNode.
		/// </summary>
		public override bool RemoveNode()
		{
			Combine  cmb = Combine;
			IProject prj = project;
			CombineEntry removeEntry = null;
			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			
			Gtk.MessageDialog dialog = new Gtk.MessageDialog ((Gtk.Window)WorkbenchSingleton.Workbench, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Question, Gtk.ButtonsType.OkCancel, String.Format (GettextCatalog.GetString ("Do you really want to remove project {0} from solution {1}"), project.Name, cmb.Name)); 
													if (dialog.Run() != (int)Gtk.ResponseType.Ok) {
				dialog.Destroy ();
				return false;
			}
			dialog.Destroy ();
			
			// remove combineentry
			foreach (CombineEntry entry in cmb.Entries) {
				if (entry is ProjectCombineEntry) {
					if (((ProjectCombineEntry)entry).Project == prj) {
						removeEntry = entry;
						break;
					}
				}
			}
			
			Debug.Assert(removeEntry != null);
			cmb.RemoveEntry (removeEntry);
			
			// remove execute definition
			CombineExecuteDefinition removeExDef = null;
			foreach (CombineExecuteDefinition exDef in cmb.CombineExecuteDefinitions) {
				if (exDef.Entry == removeEntry) {
					removeExDef = exDef;
				}
			}
			Debug.Assert(removeExDef != null);
			cmb.CombineExecuteDefinitions.Remove(removeExDef);
			
			// remove configuration
			foreach (DictionaryEntry dentry in cmb.Configurations) {
				((CombineConfiguration)dentry.Value).RemoveEntry(removeEntry);
			}
			
			CombineBrowserNode cbn = ((CombineBrowserNode)Parent);
			cbn.UpdateNaming();
			return true;
		}
	}
}
