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

using Gtk;
using MonoDevelop.Gui;

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


		class SelectStylePanelWidget : GladeWidgetExtract {
			public Gtk.Menu ambienceMenu;
			[Glade.Widget] public Gtk.CheckButton extensionButton;
			[Glade.Widget] public Gtk.OptionMenu option;
					
			public SelectStylePanelWidget () : base ("Base.glade", "SelectStylePanel")
			{
				ambienceMenu = new Gtk.Menu ();
				option.Menu = ambienceMenu;
			}
		}

		public SelectStylePanel () : base ()
		{
		}

		SelectStylePanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new SelectStylePanelWidget ());

			widget.extensionButton.Active  = PropertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			string active = PropertyService.GetProperty ("SharpDevelop.UI.CurrentAmbience", "CSharp");

			//FIXME: Yes, I know for is better here
			uint im = 0;
			foreach (IAddInTreeNode childNode in treeNode.ChildNodes.Values) {
				Gtk.MenuItem i = Gtk.MenuItem.NewWithLabel (childNode.Codon.ID);
				widget.ambienceMenu.Append(i);
				MenuToValue[i] = childNode.Codon.ID;
				if (childNode.Codon.ID == active) {
					widget.option.SetHistory (im);
				}
				im++;
			}			
			
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", widget.extensionButton.Active);
			PropertyService.SetProperty("SharpDevelop.UI.CurrentAmbience", (string)MenuToValue[widget.ambienceMenu.Active]);
			return true;
		}
	}
}
