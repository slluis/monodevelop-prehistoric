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

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

using Gtk;

namespace ICSharpCode.SharpDevelop.Commands.ProjectBrowser
{
	public class AddResourceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			FolderNode         node    = browser.SelectedNode as FolderNode;
			
			if (node != null) {
				IProject project = ((ProjectBrowserNode) node.Parent).Project;
				
			show_dialog:
									
				Gtk.FileSelection fs = new Gtk.FileSelection ("File to Open");
				fs.SelectMultiple = true;
				fs.Filename = project.BaseDirectory;
				int response = fs.Run ();
				string [] files = fs.Selections;
				
				fs.Destroy ();
				
				if (response != (int)Gtk.ResponseType.Ok)
					return;
				
				foreach (string file in files) {
					if (! File.Exists (file)) {
						IMessageService messageService = (IMessageService) ServiceManager.Services.GetService (typeof (IMessageService));
						messageService.ShowError (String.Format ("Resource file `{0}' does not exist", file));
						goto show_dialog;
					}
				}
				
				
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				
				foreach (string fileName in files) {
					ProjectFile fileInformation = projectService.AddFileToProject(project, fileName, BuildAction.EmbedAsResource);
					
					AbstractBrowserNode newResNode = new FileNode(fileInformation);
					newResNode.IconImage = resourceService.GetBitmap ("Icons.16x16.ResourceFileIcon");
					node.Nodes.Add(newResNode);
				}
				node.Expand();
				projectService.SaveCombine();
			}
		}
	}
	
}
