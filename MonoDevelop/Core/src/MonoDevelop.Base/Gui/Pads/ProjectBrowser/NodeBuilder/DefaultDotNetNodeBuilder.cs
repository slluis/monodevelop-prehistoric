// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Widgets;
using Stock = MonoDevelop.Gui.Stock;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	public class DefaultDotNetNodeBuilder : IProjectNodeBuilder
	{
		public bool CanBuildProjectTree(Project project)
		{
			return true;
		}

		public static bool IsWebReference(AbstractBrowserNode node)
		{
			if (node != null) {
				if (node is ProjectBrowserNode)
					return false;
				if (node.Text == GettextCatalog.GetString ("Web References"))
					return true;
				return IsWebReference((AbstractBrowserNode)node.Parent);
			}

			return false;
		}

		public AbstractBrowserNode BuildProjectTreeNode(Project project)
		{
			ProjectBrowserNode projectNode = new ProjectBrowserNode(project);
			string lang = (project is DotNetProject) ? ((DotNetProject)project).LanguageName : "";
			projectNode.Image = Runtime.Gui.Icons.GetImageForProjectType (lang);

			FolderNode resourceNode = new NamedFolderNode(GettextCatalog.GetString ("Resource Files"), 0);
			resourceNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ResourceFolderNode";
			resourceNode.OpenedImage = Stock.OpenResourceFolder;
			resourceNode.ClosedImage = Stock.ClosedResourceFolder;
			projectNode.Nodes.Add (resourceNode);

			FolderNode referenceNode = new NamedFolderNode(GettextCatalog.GetString ("References"), 1);
			referenceNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ReferenceFolderNode";
			referenceNode.OpenedImage = Stock.OpenReferenceFolder;
			referenceNode.ClosedImage = Stock.ClosedReferenceFolder;
			projectNode.Nodes.Add (referenceNode);

			// build a hash of projectFile items
			System.Collections.Hashtable fileHash = new Hashtable();

			// add items to the list by dependency order
			bool bDone = false;
			while(fileHash.Count != project.ProjectFiles.Count && bDone != true) {
				bool bAtLeastOneLoaded = false;
				bDone = true;
				foreach(ProjectFile projectFile in project.ProjectFiles) {
					if(fileHash.ContainsKey(projectFile.Name)) {
						continue;
					}

					bDone = false;

					if(projectFile.DependsOn != null && projectFile.DependsOn != String.Empty) {
						if(!fileHash.ContainsKey(projectFile.DependsOn)) {
							// cannot load yet
							continue;
						}
					}

					// flag to true in the hash
					bAtLeastOneLoaded = true;
					fileHash.Add(projectFile.Name, true);
				}
				if(!bAtLeastOneLoaded) {
					// we have dependencies that cannot be resolved
					// so we will add them without dependson
					foreach(ProjectFile projectFile in project.ProjectFiles) {
						if(!fileHash.ContainsKey(projectFile.Name)) {
							projectFile.DependsOn = String.Empty;
						}
					}
					break;
				}
			}

			// now we can load the files
			foreach(ProjectFile projectFile in project.ProjectFiles) {
				switch(projectFile.Subtype) {
					case Subtype.Code:
						// add a source file
						switch (projectFile.BuildAction) {
							case BuildAction.Exclude:
								// should we add?
								break;
							case BuildAction.EmbedAsResource:
								// add as a resource
								AbstractBrowserNode newResNode = new FileNode(projectFile);
								resourceNode.Nodes.Add(newResNode);
								break;
							default:
								// add everything else
								AddProjectFileNode(project, projectNode, projectFile);
								break;
						}
						break;
					default:
						// add everything else
						AddProjectFileNode(project, projectNode, projectFile);
						break;
				}
			}

			/*
			// create 'empty' directories
			for (int i = 0; i < project.ProjectFiles.Count; ++i) {
				if(project.ProjectFiles[i].Subtype == Subtype.WebReferences ) {
					string directoryName   = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, project.ProjectFiles[i].Name);
					// if directoryname starts with ./ oder .\
					if (directoryName.StartsWith(".")) {
						directoryName =  directoryName.Substring(2);
					}
					string parentDirectory = Path.GetFileName(directoryName);
					AbstractBrowserNode currentPathNode = GetPath(directoryName, projectNode, true);

					DirectoryNode newFolderNode  = new DirectoryNode(project.ProjectFiles[i].Name);
					newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenWebReferenceFolder");
					newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedWebReferenceFolder");
					currentPathNode.Nodes.Add(newFolderNode);

				}
				else if (project.ProjectFiles[i].Subtype == Subtype.Directory) {
					string directoryName   = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, project.ProjectFiles[i].Name);

					// if directoryname starts with ./ oder .\
					if (directoryName.StartsWith(".")) {
						directoryName =  directoryName.Substring(2);
					}

					string parentDirectory = Path.GetFileName(directoryName);

					AbstractBrowserNode currentPathNode = GetPath(directoryName, projectNode, true);

					DirectoryNode newFolderNode  = new DirectoryNode(project.ProjectFiles[i].Name);
					newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap");
					newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap");

					currentPathNode.Nodes.Add(newFolderNode);

				}
			}

			// create file tree
			for (int i = 0; i < project.ProjectFiles.Count; ++i) {
				if (project.ProjectFiles[i].Subtype != Subtype.Directory) {
					ProjectFile fileInformation = project.ProjectFiles[i];

					string relativeFile = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, fileInformation.Name);

					string fileName     = Path.GetFileName(fileInformation.Name);

					switch (fileInformation.BuildAction) {

						case BuildAction.Exclude:
							break;

						case BuildAction.EmbedAsResource:
							AbstractBrowserNode newResNode = new FileNode(fileInformation);
							resourceNode.Nodes.Add(newResNode);
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
			*/

			InitializeReferences(referenceNode, project);
			return projectNode;
		}

		public static void AddProjectFileNode(Project project, AbstractBrowserNode projectNode, ProjectFile projectFile) {

			if(projectNode.TreeView != null)
				projectNode.TreeView.BeginUpdate();

			// only works for relative paths right now!
			AbstractBrowserNode parentNode = null;
			string relativeFile = Runtime.FileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, projectFile.Name);
			parentNode = projectNode;

			if(projectFile.DependsOn != String.Empty && projectFile.DependsOn != null) {
				// make sure the dependant node exists
				AbstractBrowserNode dependNode = GetPath(Runtime.FileUtilityService.AbsoluteToRelativePath(project.BaseDirectory,projectFile.DependsOn), projectNode, false);
				if(dependNode == null) {
					// dependsOn does not exist, do what?
				}

			}

			switch(projectFile.Subtype) {
				case Subtype.Code:
					// add a source file
					switch (projectFile.BuildAction) {

						case BuildAction.Exclude:
							break;

						case BuildAction.EmbedAsResource:
							// no resources
							break;

						default:
							AbstractBrowserNode currentPathNode1;
							currentPathNode1 = GetPath (relativeFile, parentNode, true);

							AbstractBrowserNode newNode = new FileNode(projectFile);
							newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
							//parentNode.Nodes.Add(newNode);
							
							currentPathNode1.Nodes.Add (newNode);
							break;
					}
					break;
				case Subtype.Directory:
					{
						// add a directory
						string directoryName = relativeFile;

						// if directoryname starts with ./ oder .\ //
						if (directoryName.StartsWith(".")) {
							directoryName =  directoryName.Substring(2);
						}

						AbstractBrowserNode currentPathNode;
						currentPathNode = GetPath(directoryName, parentNode, false);

						if(currentPathNode == null) {
							currentPathNode = parentNode;
							DirectoryNode newFolderNode  = new DirectoryNode(projectFile.Name);
							//if(IsWebReference(currentPathNode)) {
							//	newFolderNode.OpenedImage = Stock.OpenWebReferenceFolder;
							//	newFolderNode.ClosedImage = Stock.ClosedWebReferenceFolder;
							//} else {
								newFolderNode.OpenedImage = Stock.OpenFolderBitmap;
								newFolderNode.ClosedImage = Stock.ClosedFolderBitmap;
							//}
							currentPathNode.Nodes.Add(newFolderNode);
						}
					}
					break;
#if false
				case Subtype.WebReferences:
					{
						// add a web directory
						string directoryName   = relativeFile;
						// if directoryname starts with ./ oder .\ //
						if (directoryName.StartsWith(".")) {
							directoryName =  directoryName.Substring(2);
						}

						DirectoryNode newFolderNode  = new DirectoryNode(projectFile.Name);
						newFolderNode.OpenedImage = Stock.OpenWebReferenceFolder;
						newFolderNode.ClosedImage = Stock.ClosedWebReferenceFolder;
						
						string parentDirectory = Path.GetFileName(directoryName);
						projectNode.Nodes.Add (newFolderNode);
					}
					break;

				case Subtype.WebForm:
					{
						// add the source file with the cool icon
						// set reference to the special context menu path
						AbstractBrowserNode currentPathNode1;
						currentPathNode1 = GetPath(relativeFile, parentNode, true);

						AbstractBrowserNode newNode = new FileNode(projectFile);
						// Dont have this
						//newNode.IconImage = Stock.WebForm;
						newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
						//parentNode.Nodes.Add(newNode);
						
						currentPathNode1.Nodes.Add (newNode);
						// codeBehind?
					}

					break;
				case Subtype.WinForm:
					{
						// add the source file with the cool icon
						// set reference to the special context menu path
						AbstractBrowserNode currentPathNode1;
						currentPathNode1 = GetPath(relativeFile, parentNode, true);

						AbstractBrowserNode newNode = new FileNode (projectFile);
						newNode.IconImage = Stock.WinForm;
						newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
						//parentNode.Nodes.Add(newNode);
						
						currentPathNode1.Nodes.Add (newNode);
					}

					break;
				case Subtype.XmlForm:
					// not supported yet
					break;
				case Subtype.Dataset:
					// not supported yet
					break;
				case Subtype.WebService:
					// not supported yet
					break;
#endif
				default:
					Runtime.LoggingService.Info ("UNHANDLED TYPE {0}, DefaultDotNetNodeBuilder.cs", projectFile.Subtype);
					// unknown file type
					break;
			}
			if (projectNode.TreeView != null)
				projectNode.TreeView.EndUpdate ();
		}

		public static AbstractBrowserNode GetProjectNode(AbstractBrowserNode childNode) {
			// find and return the project node if it exists
			AbstractBrowserNode parentNode = childNode;
			while(parentNode != null) {
				if(parentNode is ProjectBrowserNode) {
					break;
				}
				parentNode = (AbstractBrowserNode)parentNode.Parent;
			}
			// this could be null!!!
			return parentNode;
		}

		public static AbstractBrowserNode GetNodeFromCollection(TreeNodeCollection collection, string title)
		{
			foreach (AbstractBrowserNode node in collection) {
				if (node.Text == title) {
					return node;
				}
			}
			return null;
		}


		public static AbstractBrowserNode GetPath(string filename, AbstractBrowserNode root, bool create)
		{
			string directory    = Path.GetDirectoryName(filename);
			string[] treepath   = directory.Split(new char[] { Path.DirectorySeparatorChar });
			AbstractBrowserNode curpathnode = root;

			foreach (string path in treepath) {
				if (path.Length == 0 || path[0] == '.') {
					continue;
				}

				AbstractBrowserNode node = GetNodeFromCollection(curpathnode.Nodes, path);

				if (node == null) {
					if (create) {
						DirectoryNode newFolderNode  = new DirectoryNode(Runtime.FileUtilityService.GetDirectoryNameWithSeparator(ConstructFolderName(curpathnode)) + path);
						curpathnode.Nodes.Add (newFolderNode);
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

		static string ConstructFolderName(AbstractBrowserNode folderNode)
		{
			if (folderNode is DirectoryNode) {
				return ((DirectoryNode)folderNode).FolderName;
			}

			if (folderNode is ProjectBrowserNode) {
				return ((ProjectBrowserNode)folderNode).Project.BaseDirectory;
			}

			throw new ApplicationException("Folder name construction failed, got unexpected parent node :" +  folderNode);
		}

		public static AbstractBrowserNode GetNodeByName(string name) {
			return null;
		}

		public static void InitializeReferences(AbstractBrowserNode parentNode, Project project)
		{
			parentNode.Nodes.Clear();
			foreach (ProjectReference referenceInformation in project.ProjectReferences) {
				AbstractBrowserNode newReferenceNode = new ReferenceNode (referenceInformation);
				newReferenceNode.Image = Stock.Reference;
				parentNode.Nodes.Add (newReferenceNode);
			}
		}
		
	}
}
