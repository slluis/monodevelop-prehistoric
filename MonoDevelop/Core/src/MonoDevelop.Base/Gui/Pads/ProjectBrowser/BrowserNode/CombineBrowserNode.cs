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

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
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
		
		public CombineBrowserNode(Combine combine)
		{
			UserData     = combine;
			this.combine = combine;
			UpdateNaming();
			Image = Stock.CombineIcon;
			
			contextmenuAddinTreePath = defaultContextMenuPath;
			combine.NameChanged += new CombineEntryRenamedEventHandler (UpdateCombineName);
		}
		
		public override void BeforeLabelEdit()
		{
			Text = combine.Name;
		}
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0 && ContainsNoInvalidChars (newName)) {
				combine.Name = newName;
			}
			UpdateCombineName (null, null);
		}
		
		bool ContainsNoInvalidChars (string name)
		{
			if (name.IndexOfAny (new char [] { '\'', '(', ')', '"', '{', '}', '|' } ) != -1) {
				Runtime.MessageService.ShowError (String.Format (GettextCatalog.GetString ("Solution name may not contain any of the following characters: {0}"), "', (, ), \", {, }, |"));
				return false;
			}
			return true;
		}
		
		public override void UpdateNaming()
		{
			UpdateCombineName(this, null);
			base.UpdateNaming();
		}
		
		public void UpdateCombineName (object sender, CombineEntryRenamedEventArgs e)
		{
			switch (combine.Entries.Count) {
				case 0:
					Text = String.Format (GettextCatalog.GetString ("Solution {0}"), combine.Name);
					break;
				case 1:
					Text = String.Format (GettextCatalog.GetString ("Solution {0} (1 entry)"), combine.Name);
					break;
				default:
					Text = String.Format (GettextCatalog.GetString ("Solution {0} ({1} entries)"), combine.Name, combine.Entries.Count.ToString ());
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
			
			bool yes = Runtime.MessageService.AskQuestion (String.Format (GettextCatalog.GetString ("Do you really want to remove solution {0} from solution {1}?"), combine.Name, cmbNode.Combine.Name));

			if (!yes)
				return false;
			
			cmbNode.Combine.RemoveEntry (Combine);
			
			// remove execute definition
			CombineExecuteDefinition removeExDef = null;
			foreach (CombineExecuteDefinition exDef in cmbNode.Combine.CombineExecuteDefinitions) {
				if (exDef.Entry == Combine) {
					removeExDef = exDef;
				}
			}
			Debug.Assert(removeExDef != null);
			cmbNode.Combine.CombineExecuteDefinitions.Remove(removeExDef);
			
			// remove configuration
			foreach (DictionaryEntry dentry in cmbNode.Combine.Configurations) {
				((CombineConfiguration)dentry.Value).RemoveEntry (Combine);
			}
			
			cmbNode.UpdateNaming();
			return true;
		}
	}
}
