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
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads.ProjectBrowser;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class SetAsStartupProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView  browser = (ProjectBrowserView)Owner;
			ProjectBrowserNode  node    = browser.SelectedNode as ProjectBrowserNode;
			
			if (node != null) {
				Combine combine                = node.Combine;
				combine.SingleStartProjectName = node.Project.Name;
				combine.SingleStartupProject   = true;
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				projectService.SaveCombine();
			}
		}
	}
}
