// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class SelectStylePanel : AbstractOptionPanel
	{
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
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

			PropertyService p = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
			//FIXME: Hashtables are wrong here.
			//FIXME: Yes, this is a dirty hack.
			//FIXME: Lets use something else.
			Hashtable MenuToValue = new Hashtable ();
			Hashtable ValueToMenu = new Hashtable ();
			public Gtk.Menu ambienceMenu;
			[Glade.Widget] public Gtk.CheckButton extensionButton;
			[Glade.Widget] public Gtk.CheckButton hiddenButton;
			[Glade.Widget] public Gtk.OptionMenu option;
					
			public SelectStylePanelWidget () : base ("Base.glade", "SelectStylePanel")
			{
				ambienceMenu = new Gtk.Menu ();
				option.Menu = ambienceMenu;
				extensionButton.Active  = p.GetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
				hiddenButton.Active  = p.GetProperty("ICSharpCode.SharpDevelop.Gui.FileScout.ShowHidden", false);
				IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
				string active = p.GetProperty ("SharpDevelop.UI.CurrentAmbience", "CSharp");
				
				//FIXME: Yes, I know for is better here
				uint im = 0;
				foreach (IAddInTreeNode childNode in treeNode.ChildNodes.Values) 
				{
					Gtk.MenuItem i = Gtk.MenuItem.NewWithLabel (childNode.Codon.ID);
					ambienceMenu.Append(i);
					MenuToValue[i] = childNode.Codon.ID;
					if (childNode.Codon.ID == active) {
						option.SetHistory (im);
					}
					im++;
				}
				
			}
			
			public void Store()
			{
				p.SetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", extensionButton.Active);
				p.SetProperty("ICSharpCode.SharpDevelop.Gui.FileScout.ShowHidden", hiddenButton.Active);
				p.SetProperty("SharpDevelop.UI.CurrentAmbience", (string)MenuToValue[ambienceMenu.Active]);
			}
		}
	}
}
