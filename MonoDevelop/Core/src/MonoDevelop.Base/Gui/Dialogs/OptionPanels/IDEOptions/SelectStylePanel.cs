// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Properties;
using MonoDevelop.Gui.Components;
using MonoDevelop.Services;
using MonoDevelop.Core.AddIns;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
	public class SelectStylePanel : AbstractOptionPanel
	{
		SelectStylePanelWidget widget;
		const string selectStyleProperty = "SharpDevelop.UI.SelectStyleOptions";

		public override void LoadPanelContents()
		{
			Add (widget = new SelectStylePanelWidget ());
		}
		
		public override bool StorePanelContents()
		{
			widget.Store ();
			return true;
		}

		class SelectStylePanelWidget : GladeWidgetExtract 
		{
			//FIXME: Hashtables are wrong here.
			//FIXME: Yes, this is a dirty hack.
			//FIXME: Lets use something else.
			Hashtable MenuToValue = new Hashtable ();
			Hashtable ValueToMenu = new Hashtable ();
			public Gtk.Menu ambienceMenu;
			[Glade.Widget] public Gtk.OptionMenu option;
			
			
			[Glade.Widget] public Gtk.CheckButton extensionButton;
			[Glade.Widget] public Gtk.CheckButton hiddenButton;
			
					
			public SelectStylePanelWidget () : base ("Base.glade", "SelectStylePanel")
			{
				extensionButton.Active  = Runtime.Properties.GetProperty("MonoDevelop.Gui.ProjectBrowser.ShowExtensions", true);
				hiddenButton.Active  = Runtime.Properties.GetProperty("MonoDevelop.Gui.FileScout.ShowHidden", false);

// commented out by jba - 23 feb 04, deosn't seem to be used
//				ambienceMenu = new Gtk.Menu ();
//				option.Menu = ambienceMenu;
//				IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
//				
//				string active = p.GetProperty ("SharpDevelop.UI.CurrentAmbience", "CSharp");
//				
//				//FIXME: Yes, I know for is better here
//				uint im = 0;
//				foreach (IAddInTreeNode childNode in treeNode.ChildNodes.Values) 
//				{
//					Gtk.MenuItem i = Gtk.MenuItem.NewWithLabel (childNode.Codon.ID);
//					ambienceMenu.Append(i);
//					MenuToValue[i] = childNode.Codon.ID;
//					if (childNode.Codon.ID == active) {
//						option.SetHistory (im);
//					}
//					im++;
//				}
				
			}
			
			public void Store()
			{
				Runtime.Properties.SetProperty("MonoDevelop.Gui.ProjectBrowser.ShowExtensions", extensionButton.Active);
				Runtime.Properties.SetProperty("MonoDevelop.Gui.FileScout.ShowHidden", hiddenButton.Active);
//				p.SetProperty("SharpDevelop.UI.CurrentAmbience", (string)MenuToValue[ambienceMenu.Active]);
			}
		}
	}
}
