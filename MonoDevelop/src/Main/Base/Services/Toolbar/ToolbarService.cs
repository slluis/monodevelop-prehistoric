// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.CodeDom.Compiler;

using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.AddIns;

using MonoDevelop.Core.Services;

using Gtk;

namespace MonoDevelop.Services
{
	public class ToolbarService : AbstractService
	{
		readonly static string toolBarPath     = "/SharpDevelop/Workbench/ToolBar";
		
		IAddInTreeNode node;
		
		public ToolbarService()
		{
			this.node  = AddInTreeSingleton.AddInTree.GetTreeNode(toolBarPath);
		}
		
		public Gtk.Toolbar[] CreateToolbars()
		{
			ToolbarItemCodon[] codons = (ToolbarItemCodon[])(node.BuildChildItems(this)).ToArray(typeof(ToolbarItemCodon));
			
			Gtk.Toolbar[] toolBars = new Gtk.Toolbar[codons.Length];
			
			for (int i = 0; i < codons.Length; ++i) {
				toolBars[i] = CreateToolBarFromCodon(WorkbenchSingleton.Workbench, codons[i]);
			}
			return toolBars;
		}
		
		public Gtk.Toolbar CreateToolBarFromCodon(object owner, ToolbarItemCodon codon)
		{
			Gtk.Toolbar bar = new Gtk.Toolbar();
			bar.ToolbarStyle = Gtk.ToolbarStyle.Icons;
			
			ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
			foreach (ToolbarItemCodon childCodon in codon.SubItems) {
				SdToolbarCommand item = null;
				
				if (childCodon.ToolTip != null) {
					if (childCodon.ToolTip == "-") {
						bar.AppendSpace();
						continue;
					} else {
						item = new SdToolbarCommand(childCodon.Conditions, owner, childCodon.ToolTip);
						Gtk.Image img = resourceService.GetImage(childCodon.Icon, Gtk.IconSize.LargeToolbar);
						item.Add (img);
						item.Relief = ReliefStyle.None;
						
						//if (img.StorageType == Gtk.ImageType.Stock) {
						//	item.Stock = img.Stock;
						//	item.IconSize = (int)Gtk.IconSize.SmallToolbar;
						//} else {
						//	item.Pixbuf = img.Pixbuf;
						//}
						item.ShowAll();
					}
				} else {
					continue;
				}
				if (childCodon.Class != null) {
					((SdToolbarCommand)item).Command = (ICommand)childCodon.AddIn.CreateObject(childCodon.Class);
				}
				//bar.AppendItem(item.Text, item.Text, item.Text, item, new Gtk.SignalFunc(item.ToolbarClicked));
				bar.AppendWidget (item, item.Text, item.Text);
			}
			return bar;
		}
	}
}
