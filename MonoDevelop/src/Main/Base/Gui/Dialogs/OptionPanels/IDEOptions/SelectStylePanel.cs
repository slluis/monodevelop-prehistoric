// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class SelectStylePanel : AbstractOptionPanel
	{
		
		//FIXME: Hashtables are wrong here.
		//FIXME: Yes, this is a dirty hack.
		//FIXME: Lets use something else.
		Hashtable MenuToValue = new Hashtable ();
		Hashtable ValueToMenu = new Hashtable ();

		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));

		Gtk.Menu ambienceMenu;
		Gtk.CheckButton extensionButton;
		Gtk.OptionMenu option;
		
		public SelectStylePanel () : base ()
		{
			extensionButton = Gtk.CheckButton.NewWithLabel ("Show Extensions in project scout");
			ambienceMenu = new Gtk.Menu ();

			option = new Gtk.OptionMenu ();
			option.Menu = ambienceMenu;

			Gtk.VBox mainbox = new Gtk.VBox (false, 2);

			mainbox.PackStart (extensionButton, false, false, 2);
			mainbox.PackStart (option, false, false, 2);

			this.Add (mainbox);
			
		}
		
		public override void LoadPanelContents()
		{
			
			extensionButton.Active  = PropertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			string active = PropertyService.GetProperty ("SharpDevelop.UI.CurrentAmbience", "CSharp");

			//FIXME: Yes, I know for is better here
			uint im = 0;
			foreach (IAddInTreeNode childNode in treeNode.ChildNodes.Values) {
				Gtk.MenuItem i = Gtk.MenuItem.NewWithLabel (childNode.Codon.ID);
				ambienceMenu.Append(i);
				MenuToValue[i] = childNode.Codon.ID;
				if (childNode.Codon.ID == active) {
					option.SetHistory (im);
				}
				im++;
			}			
			
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", extensionButton.Active);
			PropertyService.SetProperty("SharpDevelop.UI.CurrentAmbience", (string)MenuToValue[ambienceMenu.Active]);
			return true;
		}
	}
}
