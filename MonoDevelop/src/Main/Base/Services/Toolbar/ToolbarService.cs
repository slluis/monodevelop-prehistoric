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

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Services;

using Gtk;

namespace ICSharpCode.SharpDevelop.Services
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
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			foreach (ToolbarItemCodon childCodon in codon.SubItems) {
				SdToolbarCommand item = null;
				
				if (childCodon.ToolTip != null) {
					if (childCodon.ToolTip == "-") {
						bar.AppendSpace();
						continue;
					} else {
						item = new SdToolbarCommand(childCodon.Conditions, owner, childCodon.ToolTip);
						Gdk.Pixbuf pb = resourceService.GetBitmap(childCodon.Icon);
						item.Pixbuf = pb;
						item.ShowAll();
					}
				} else {
					continue;
				}
				if (childCodon.Class != null) {
					((SdToolbarCommand)item).Command = (ICommand)childCodon.AddIn.CreateObject(childCodon.Class);
				}
				bar.AppendItem(item.Text, item.Text, item.Text, item, new Gtk.SignalFunc(item.ToolbarClicked));
			}
			return bar;
		}
	}
}
