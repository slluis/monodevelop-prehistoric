// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads.ProjectBrowser;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class RemoveEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (node.IsEditing) { // TODO : throw remove key to the browser component.
				return;
			}
			
			if (node != null && node.Parent != null) {
				if (node.RemoveNode()) {
					node.Parent.Nodes.Remove(node);
					projectService.SaveCombine();
				}
			}
		}
	}
	
	public class RenameEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			browser.StartLabelEdit();
		}
	}
	
	public class OpenFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			AbstractBrowserNode node    = browser.SelectedNode as AbstractBrowserNode;
			
			if (node != null) {
				node.ActivateItem();
			}
		}
	}
}

