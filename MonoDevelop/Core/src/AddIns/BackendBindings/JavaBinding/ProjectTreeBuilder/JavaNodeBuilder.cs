// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Xml;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Pads.ProjectBrowser;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Services;

namespace JavaBinding
{
	public class JavaNodeBuilder : IProjectNodeBuilder
	{
		public bool CanBuildProjectTree(Project project)
		{
			DotNetProject dp = project as DotNetProject; 
			return dp != null && dp.LanguageName == JavaLanguageBinding.LanguageName;
		}
		
		public AbstractBrowserNode BuildProjectTreeNode(Project project)
		{
			ProjectBrowserNode projectNode = new ProjectBrowserNode(project);
			
			//projectNode.IconImage = iconService.GetImageForProjectType(project.ProjectType);
			
			// create 'empty' directories			
			for (int i = 0; i < project.ProjectFiles.Count; ++i) {
				if (project.ProjectFiles[i].Subtype == Subtype.Directory) {
					string directoryName = Runtime.FileUtilityService.AbsoluteToRelativePath (project.BaseDirectory, project.ProjectFiles[i].Name);

					// if directoryname starts with ./ oder .\
					if (directoryName.StartsWith(".")) {
						directoryName =  directoryName.Substring(2);
					}
					
					AbstractBrowserNode currentPathNode = GetPath(directoryName, projectNode, true);
					
					DirectoryNode newFolderNode  = new DirectoryNode(project.ProjectFiles[i].Name);
					//newFolderNode.OpenedImage = resourceService.GetBitmap ("Icons.16x16.OpenFolderBitmap");
					//newFolderNode.ClosedImage = resourceService.GetBitmap ("Icons.16x16.ClosedFolderBitmap");
					
					currentPathNode.Nodes.Add(newFolderNode);
				}
			}
			
			// create file tree
			for (int i = 0; i < project.ProjectFiles.Count; ++i) {
				if (project.ProjectFiles[i].Subtype != Subtype.Directory) {
					ProjectFile fileInformation = project.ProjectFiles[i];
					
					string relativeFile = Runtime.FileUtilityService.AbsoluteToRelativePath (project.BaseDirectory, fileInformation.Name);
					
					switch (fileInformation.BuildAction) {
						case BuildAction.Exclude:
							break;
						default:
							AbstractBrowserNode currentPathNode = GetPath(relativeFile, projectNode, true);
							AbstractBrowserNode newNode = new FileNode(fileInformation);
							newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
							currentPathNode.Nodes.Add(newNode);
							break;
					}
				}
			}
			
			return projectNode;
		}

		AbstractBrowserNode GetNodeFromCollection (TreeNodeCollection collection, string title)
		{
			foreach (AbstractBrowserNode node in collection) {
				if (node.Text == title) {
					return node;
				}
			}
			return null;
		}
		
		public AbstractBrowserNode GetPath(string filename, AbstractBrowserNode root, bool create)
		{
			string directory    = Path.GetDirectoryName(filename);
			string[] treepath   = directory.Split(new char[] { Path.DirectorySeparatorChar });
			AbstractBrowserNode curpathnode = root;
			
			foreach (string path in treepath) {
				if (path.Length == 0 || path[0] == '.') {
					continue;
				}
				
				AbstractBrowserNode node = null;
				//AbstractBrowserNode node = GetNodeFromCollection(curpathnode.Nodes, path);
				
				if (node == null) {
					if (create) {
						DirectoryNode newFolderNode  = new DirectoryNode(Runtime.FileUtilityService.GetDirectoryNameWithSeparator (ConstructFolderName (curpathnode)) + path);
						curpathnode.Nodes.Add(newFolderNode);
						curpathnode = newFolderNode;
						continue;
					} else {
						return null;
					}
				}
				curpathnode = node;
			}
			
			return curpathnode;
		}
		
		public string ConstructFolderName(AbstractBrowserNode folderNode)
		{
			if (folderNode is DirectoryNode) {
				return ((DirectoryNode)folderNode).FolderName;
			}
			
			if (folderNode is ProjectBrowserNode) {
				return ((ProjectBrowserNode)folderNode).Project.BaseDirectory;
			}
			
			throw new ApplicationException("Folder name construction failed, got unexpected parent node :" +  folderNode);
		}
		
	}
}
