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
using MonoDevelop.Services;

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
		
		public Gtk.Toolbar CreateToolBarFromCodon (object owner, ToolbarItemCodon codon)
		{
			Gtk.Toolbar bar = new Gtk.Toolbar ();
			bar.ToolbarStyle = Gtk.ToolbarStyle.Icons;
			
			foreach (ToolbarItemCodon childCodon in codon.SubItems)
			{
				if (childCodon.ToolTip != null) {
					if (childCodon.ToolTip == "-")
						bar.Insert (new SeparatorToolItem (), -1);
					else
						bar.Insert (new SdToolbarCommand (childCodon, owner), -1);
				}
			}
			return bar;
		}
	}
}

