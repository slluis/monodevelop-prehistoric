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
using System.Collections;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;
using Stock = MonoDevelop.Gui.Stock;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default combine in the project browser.
	/// </summary>
	public class CombineBrowserNode : AbstractBrowserNode 
	{
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/CombineBrowserNode";
		
		Combine combine;
		
		public override Combine Combine {
			get {
				return combine;
			}
		}
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		public CombineBrowserNode(Combine combine)
		{
			UserData     = combine;
			this.combine = combine;
			UpdateNaming();
			Image = Stock.CombineIcon;
			
			contextmenuAddinTreePath = defaultContextMenuPath;
			combine.NameChanged += new EventHandler(UpdateCombineName);
		}
		
		public override void BeforeLabelEdit()
		{
			Text = combine.Name;
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0) {
				combine.Name = newName;
			}
		}
		
		public override void UpdateNaming()
		{
			UpdateCombineName(this, EventArgs.Empty);
			base.UpdateNaming();
		}
		
		public void UpdateCombineName(object sender, EventArgs e)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			switch (combine.Entries.Count) {
				case 0:
					Text = stringParserService.Parse(resourceService.GetString("ProjectComponent.CombineNameStringNoEntry"), new string[,] { {"Combinename", combine.Name}, {"Entries", combine.Entries.Count.ToString()}});
					break;
				case 1:
					Text = stringParserService.Parse(resourceService.GetString("ProjectComponent.CombineNameStringSingleEntry"), new string[,] { {"Combinename", combine.Name}, {"Entries", combine.Entries.Count.ToString()}});
					break;
				default:
					Text = stringParserService.Parse(resourceService.GetString("ProjectComponent.CombineNameString"), new string[,] { {"Combinename", combine.Name}, {"Entries", combine.Entries.Count.ToString()}});
					break;
			}
		}
		
		/// <summary>
		/// Removes a combine from a combine
		/// NOTE : This method assumes that its parent is == null or that it is
		/// from the type 'CombineBrowserNode'.
		/// </summary>
		public override bool RemoveNode()
		{
			if (Parent == null) {
				return false;
			}
			
			CombineBrowserNode cmbNode = (CombineBrowserNode)Parent;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			Gtk.MessageDialog dialog = new Gtk.MessageDialog ((Gtk.Window)WorkbenchSingleton.Workbench, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Question, Gtk.ButtonsType.OkCancel, stringParserService.Parse(resourceService.GetString("ProjectComponent.RemoveCombine.Question"), new string[,] { {"COMBINE", combine.Name}, {"PARENTCOMBINE", cmbNode.Combine.Name}}));
			
			if (dialog.Run() != (int)Gtk.ResponseType.Ok) {
				dialog.Destroy ();
				return false;
			}
			dialog.Destroy ();
			
			CombineEntry removeEntry = null;
			
			// remove combineentry
			foreach (CombineEntry entry in cmbNode.Combine.Entries) {
				if (entry is CombineCombineEntry) {
					if (((CombineCombineEntry)entry).Combine == Combine) {
						removeEntry = entry;
						break;
					}
				}
			}
			
			Debug.Assert(removeEntry != null);
			cmbNode.Combine.Entries.Remove(removeEntry);
			
			// remove execute definition
			CombineExecuteDefinition removeExDef = null;
			foreach (CombineExecuteDefinition exDef in cmbNode.Combine.CombineExecuteDefinitions) {
				if (exDef.Entry == removeEntry) {
					removeExDef = exDef;
				}
			}
			Debug.Assert(removeExDef != null);
			cmbNode.Combine.CombineExecuteDefinitions.Remove(removeExDef);
			
			// remove configuration
			foreach (DictionaryEntry dentry in cmbNode.Combine.Configurations) {
				((CombineConfiguration)dentry.Value).RemoveEntry(removeEntry);
			}
			
			cmbNode.UpdateNaming();
			return true;
		}
	}
}
