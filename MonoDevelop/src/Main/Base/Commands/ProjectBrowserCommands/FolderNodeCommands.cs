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

using Gtk;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddFilesToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			
			if (browser == null || browser.SelectedNode == null) {
				return;
			}
			
			AbstractBrowserNode node = (AbstractBrowserNode)browser.SelectedNode;
			
			using (FileSelection fdiag  = new FileSelection ("Add a file")) {
			fdiag.SelectMultiple = true;
			
			int result = fdiag.Run ();
			try {
				if (result != (int) ResponseType.Ok)
					return;
				
				foreach (string file in fdiag.Selections) {
					if (file.StartsWith(node.Project.BaseDirectory)) {
						ProjectBrowserView.MoveCopyFile (file, node, true, true);
					} else {
						MessageDialog md = new MessageDialog (
							(Window) WorkbenchSingleton.Workbench,
							DialogFlags.Modal | DialogFlags.DestroyWithParent,
							MessageType.Question, ButtonsType.None,
							"The file is outside the project directory, what should I do?");
						md.AddButton ("Copy", 1);
						md.AddButton ("Move", 2);
						md.AddButton ("Cancel", ResponseType.Cancel);
						
						int ret = md.Run ();
						md.Destroy ();
						
						if (ret < 0)
							return;
						
						ProjectBrowserView.MoveCopyFile (file, node, ret == 2, false);
					}
				}
			} finally {
				fdiag.Hide ();
			}
			}
		}
	}
	
	public class AddNewFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			
			if (browser == null || browser.SelectedNode == null) {
				return;
			}
			
			AbstractBrowserNode node = (AbstractBrowserNode)browser.SelectedNode;
			string baseFolderPath = NewFolderEvent.SearchBasePath(node);
			
			if (baseFolderPath == null || baseFolderPath.Length == 0) {
				return;
			}
			
			NewFileDialog nfd = new NewFileDialog ();
			if (nfd.Run() == (int)Gtk.ResponseType.Ok) {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					
				int count = 1;
					
				string baseName  = Path.GetFileNameWithoutExtension(window.ViewContent.UntitledName);
				string extension = Path.GetExtension(window.ViewContent.UntitledName);
					
				// first try the default untitled name of the viewcontent filename
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				string fileName = fileUtilityService.GetDirectoryNameWithSeparator(baseFolderPath) + baseName +  extension;
					
				// if it is already in the project, or it does exists we try to get a name that is
				// untitledName + Numer + extension
				while (node.Project.IsFileInProject(fileName) || File.Exists(fileName)) {
					fileName = fileUtilityService.GetDirectoryNameWithSeparator(baseFolderPath) + baseName + count.ToString() + extension;
					++count;
				}
					
				// now we have a valid filename which we could use
				window.ViewContent.Save(fileName);
					
				ProjectFile newFileInformation = new ProjectFile(fileName, BuildAction.Compile);
					
				AbstractBrowserNode newNode = new FileNode(newFileInformation);
				newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
					
				// Assume that the parent node of a 'leaf' (e.g. file) is
				// a folder or project
				AbstractBrowserNode parentNode = node;
				if (!(parentNode is ProjectBrowserNode || parentNode is DirectoryNode)) {
					parentNode = (AbstractBrowserNode)node.Parent;
				}
					
				parentNode.Nodes.Add(newNode);
				parentNode.Project.ProjectFiles.Add(newFileInformation);
					
				newNode.EnsureVisible();
				browser.SelectedNode = newNode;
				browser.StartLabelEdit();
					
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				projectService.SaveCombine();
			}
		}
	}
	
	public class NewFolderEvent : AbstractMenuCommand
	{
		public static string SearchBasePath(AbstractBrowserNode node)
		{
			while (node != null) {
				if (node is ProjectBrowserNode) {
					return node.Project.BaseDirectory;
				} else if (node is DirectoryNode) {
					return ((DirectoryNode)node).FolderName;
				}
				node = (AbstractBrowserNode)node.Parent;
			}
			return null;
		}
		
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			
			if (browser == null || browser.SelectedNode == null) {
				return;
			}
			AbstractBrowserNode selectedNode = (AbstractBrowserNode)browser.SelectedNode;
			
			string baseFolderPath = SearchBasePath(selectedNode);
			
			if (baseFolderPath != null && baseFolderPath.Length > 0) {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				
				string directoryName = fileUtilityService.GetDirectoryNameWithSeparator(baseFolderPath) + resourceService.GetString("ProjectComponent.NewFolderString");
				int    index         = -1;
				
				if (Directory.Exists(directoryName)) {
					while (Directory.Exists(directoryName + (++index + 1))) ;
				}
				
				if (index >= 0) {
					directoryName += index + 1;
				}
				
				DirectoryNode newDirectoryNode = new DirectoryNode(directoryName);
				Directory.CreateDirectory(newDirectoryNode.FolderName);
						
				// Assume that the parent node of a 'leaf' (e.g. file) is
				// a folder or project
				AbstractBrowserNode parentNode = selectedNode;
				if (!(parentNode is ProjectBrowserNode || parentNode is DirectoryNode)) {
					parentNode = (AbstractBrowserNode)selectedNode.Parent;
				}
				
				parentNode.Nodes.Add(newDirectoryNode);
				
				ProjectFile newFolder = new ProjectFile(newDirectoryNode.FolderName);
				newFolder.Subtype = Subtype.Directory;
				parentNode.Project.ProjectFiles.Add(newFolder);
				
				newDirectoryNode.EnsureVisible();
				browser.SelectedNode = newDirectoryNode;
				browser.StartLabelEdit();
			}
		}
	}
}
