// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;
using System.Xml;
using System.Resources;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Services;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class implements a project browser.
	/// </summary>
	public class ProjectBrowserView : TreeView, IPadContent, IMementoCapable
	{
		static readonly string nodeBuilderPath = "/SharpDevelop/Views/ProjectBrowser/NodeBuilders";

		AbstractBrowserNode highlightedNode = null;

		public static Font PlainFont = null;
		Font               boldFont  = null;
		//Panel contentPanel = new Panel();
		Gtk.Frame contentPanel = new Gtk.Frame();
		static ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));

		public Gtk.Widget Control {
			get {
				return contentPanel;
			}
		}

		public void BringToFront() {
			// TODO FIXME
		}
		
		public string Title {
			get {
				return "Combine";
			}
		}

		public string Icon {
			get {
				return MonoDevelop.Gui.Stock.CombineIcon;
			}
		}

		public void RedrawContent()
		{
			BeginUpdate();
			AbstractBrowserNode.ShowExtensions = propertyService.GetProperty("MonoDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			foreach (AbstractBrowserNode node in Nodes) {
				node.UpdateNaming();
			}
			EndUpdate();
		}
		
		static ProjectBrowserView()
		{
			//projectBrowserImageList = new ImageList();
			//projectBrowserImageList.ColorDepth = ColorDepth.Depth32Bit;
		}
		public ProjectBrowserView() : base (true, TreeNodeComparer.GtkProjectNode)
		{
			//LabelEdit     = true;
			//AllowDrop     = true;
			//HideSelection = false;
			//Dock          = DockStyle.Fill;
			
			//ImageList = projectBrowserImageList;
			//LabelEdit = false;

			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += new EventHandler(ActiveWindowChanged);
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CombineOpened += new CombineEventHandler(OpenCombine);
			projectService.CombineClosed += new CombineEventHandler(CloseCombine);
			propertyService.PropertyChanged += new PropertyEventHandler (TrackPropertyChange);

			//PlainFont = new Font(Font, FontStyle.Regular);
			//boldFont  = new Font(Font, FontStyle.Bold);

			//Font = boldFont;
			//contentPanel.Controls.Add(this);
			
			Gtk.ScrolledWindow sw = new Gtk.ScrolledWindow ();
			sw.Add(this);
			contentPanel = new Gtk.Frame();
			contentPanel.Add(sw);
			RowActivated += new Gtk.RowActivatedHandler(OnNodeActivated);
			contentPanel.ButtonReleaseEvent += new Gtk.ButtonReleaseEventHandler(OnButtonRelease);
		}
		
		void TrackPropertyChange (object o, MonoDevelop.Core.Properties.PropertyEventArgs e)
		{
			if (e.OldValue != e.NewValue && e.Key == "MonoDevelop.Gui.ProjectBrowser.ShowExtensions") {
				RedrawContent ();
			}
		}
		
		public void RefreshTree(Combine combine)
		{
			DisposeProjectNodes();
			Nodes.Clear();
			TreeNode treeNode = BuildCombineTreeNode(combine);
			Nodes.Add (treeNode);
			
			combine.StartupPropertyChanged += new EventHandler(StartupPropertyChanged);
			StartupPropertyChanged(null, null);
			// .NET bugfix : have to expand the node to ensure the refresh
			// (Refresh won't work) tested 08/16/2002 Mike
			treeNode.Expand();

			// TODO: maybe exand all should be true
			this.ExpandRow (new Gtk.TreePath ("0"), false);
		}
		
		void OpenCombine(object sender, CombineEventArgs e)
		{
			try {
				RefreshTree(e.Combine);
			} catch (InvalidOperationException) {
				//this.Invoke(new CombineEventHandler(OpenCombine), new object[] {sender, e}); // FIXME PEDRO
			}
		}

		void CloseCombine(object sender, CombineEventArgs e)
		{
			try {
				DisposeProjectNodes();
				Nodes.Clear();
			} catch (InvalidOperationException) {
				//this.Invoke(new CombineEventHandler(CloseCombine), new object[] {sender, e}); // FIXME PEDRO
			}
		}

		void StartupPropertyChanged(object sender, EventArgs e)
		{
			Combine combine = ((AbstractBrowserNode)Nodes[0]).Combine;
			if (highlightedNode != null) {
				//highlightedNode.NodeFont = PlainFont; // FIXME PEDRO
			}

			if (combine.SingleStartupProject) {
				foreach (AbstractBrowserNode node in Nodes[0].Nodes) {
					if (node is ProjectBrowserNode) {
						if (combine.SingleStartProjectName == node.Project.Name) {
							highlightedNode = node;
							//node.NodeFont = null;// FIXME PEDRO
						}
					} else if (node is CombineBrowserNode) {
						if (combine.SingleStartProjectName == node.Combine.Name) {
							highlightedNode = node;
							//node.NodeFont = null;// FIXME PEDRO
						}
					}
				}
			} else {
				highlightedNode   = (AbstractBrowserNode)Nodes[0];
				//highlightedNode.NodeFont = boldFont; // FIXME PEDRO
			}
		}

		
		protected override void OnEdit (TreeNode node, string new_text)
		{
			// we set the label ourself
			((AbstractBrowserNode) node).AfterLabelEdit (new_text);
			
			// save changes
			IProjectService projectService = (IProjectService) ServiceManager.Services.GetService (typeof(IProjectService));
			projectService.SaveCombine();
		}

		void DisposeProjectNodes()
		{
			if (Nodes.Count == 1) {
				Stack stack = new Stack();
				stack.Push(Nodes[0]);
				while (stack.Count > 0) {
					TreeNode node = (TreeNode)stack.Pop();
					if (node is IDisposable) {
						((IDisposable)node).Dispose();
					}
					foreach (TreeNode childNode in node.Nodes) {
						stack.Push(childNode);
					}
				}
			}
		}


		/// <summary>
		/// Searches AbstractBrowserNodeCollection recursively for a given file name.
		/// Note that the UserData properties for the files have to set to FileInformation
		/// or to ReferenceInformation for this method to work.
		/// </summary>
		AbstractBrowserNode GetNodeFromCollectionTreeByFileName(TreeNodeCollection collection, string fileName)
		{
			foreach (AbstractBrowserNode node in collection) {
				if (node.UserData is ProjectFile && ((ProjectFile)node.UserData).Name == fileName) {
					return node;
				}
				if (node.UserData is ProjectReference && ((ProjectReference)node.UserData).GetReferencedFileName(node.Project) == fileName) {
					return node;
				}
				
				AbstractBrowserNode childnode = GetNodeFromCollectionTreeByFileName(node.Nodes, fileName);
				if (childnode != null) {
					return childnode;
				}
			}
			return null;
		}


		/// <summary>
		/// Selectes the current active workbench window in the Project Browser Tree and ensures
		/// the visibility of this node.
		/// </summary>
		void ActiveWindowChanged(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;

				AbstractBrowserNode node = GetNodeFromCollectionTreeByFileName(Nodes, fileName);
				if (node != null) {
					node.EnsureVisible();
					SelectedNode = node;
				}
			}
		}

		/// <summary>
		/// If you want to edit a node label. Select the node you want to edit and then
		/// call this method, instead of using the LabelEdit Property and the BeginEdit
		/// Method directly.
		/// </summary>
		public void StartLabelEdit()
		{
			AbstractBrowserNode selectedNode = (AbstractBrowserNode)SelectedNode;
			if (selectedNode != null && selectedNode.CanLabelEdited) {
				//LabelEdit = true;
				selectedNode.BeginEdit ();
			}
		}

		/// <summary>
		/// Updates the combine tree, this method should be called, if the combine has
		/// changed (added a project/combine)
		/// </summary>
		public void UpdateCombineTree()
		{
			XmlElement storedTree = new TreeViewMemento(this).ToXmlElement(new XmlDocument());
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			CloseCombine(this,new CombineEventArgs(projectService.CurrentOpenCombine));
			OpenCombine(this, new CombineEventArgs(projectService.CurrentOpenCombine));
			((TreeViewMemento)new TreeViewMemento().FromXmlElement(storedTree)).Restore(this);
			ActiveWindowChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// This method builds a ProjectBrowserNode Tree out of a given combine.
		/// </summary>
		public static AbstractBrowserNode BuildProjectTreeNode(IProject project)
		{
			IProjectNodeBuilder[] nodeBuilders = (IProjectNodeBuilder[])(AddInTreeSingleton.AddInTree.GetTreeNode(nodeBuilderPath).BuildChildItems(null)).ToArray(typeof(IProjectNodeBuilder));
			IProjectNodeBuilder   projectNodeBuilder = null;
			foreach (IProjectNodeBuilder nodeBuilder in nodeBuilders) {
				if (nodeBuilder.CanBuildProjectTree(project)) {
					projectNodeBuilder = nodeBuilder;
					break;
				}
			}
			if (projectNodeBuilder != null) {
				return projectNodeBuilder.BuildProjectTreeNode(project);
			}

			throw new NotImplementedException("can't create node builder for project type " + project.ProjectType);
		}

		/// <summary>
		/// This method builds a ProjectBrowserNode Tree out of a given combine.
		/// </summary>
		public static AbstractBrowserNode BuildCombineTreeNode(Combine combine)
		{
			CombineBrowserNode combineNode = new CombineBrowserNode(combine);
			
			// build subtree
			foreach (CombineEntry entry in combine.Entries) {
				TreeNode node = null;
				if (entry.Entry is IProject) {
					node = BuildProjectTreeNode((IProject)entry.Entry);
				} else {
					node = BuildCombineTreeNode((Combine)entry.Entry);
				}
				combineNode.Nodes.Add (node);
			}
			
			return combineNode;
		}

/*		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.F2:
					StartLabelEdit();
					break;
				default:
					return base.ProcessDialogKey(keyData);
			}
			return true;
		}
*/
		private void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3 || SelectedNode == null) {
				return;
			}
			AbstractBrowserNode node = (AbstractBrowserNode) SelectedNode;

			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CurrentSelectedProject = node.Project;
			projectService.CurrentSelectedCombine = node.Combine;
			//PropertyPad.SetDesignableObject(node.UserData);
			
			MenuService menuService = (MenuService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
			menuService.ShowContextMenu(this, node.ContextmenuAddinTreePath, this);
		}
/*		
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{ // set current project & current combine
			base.OnAfterSelect(e);
			AbstractBrowserNode node = (AbstractBrowserNode)e.Node;

			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CurrentSelectedProject = node.Project;
			projectService.CurrentSelectedCombine = node.Combine;
			PropertyPad.SetDesignableObject(node.UserData);
			
			MenuService menuService = (MenuService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
			//ContextMenu = menuService.CreateContextMenu(this, node.ContextmenuAddinTreePath);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			AbstractBrowserNode node = (AbstractBrowserNode)GetNodeAt(e.X, e.Y);

			if (node != null) {
				SelectedNode = node;
			}
		}

		// open file with the enter key
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == '\r') {
				OnDoubleClick(e);
			}
		}
*/
		private void OnNodeActivated(object sender, Gtk.RowActivatedArgs args)
		{
			if (SelectedNode != null && SelectedNode is AbstractBrowserNode) {
				((AbstractBrowserNode)SelectedNode).ActivateItem();
			}
		}
/*
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{ // show open folder icons
			base.OnBeforeExpand(e);
			if (e.Node != null && e.Node is AbstractBrowserNode) {
				((AbstractBrowserNode)e.Node).BeforeExpand();
			}
		}

		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			if (e.Node != null && e.Node is AbstractBrowserNode) {
				((AbstractBrowserNode)e.Node).BeforeCollapse();
			}
		}
*/	
		
		//static ImageList projectBrowserImageList  = null;
		//static Hashtable projectBrowserImageIndex = new Hashtable();

/*		public static int GetImageIndexForImage(Image image)
		{
			if (projectBrowserImageIndex[image] == null) {
				projectBrowserImageList.Images.Add(image);
				projectBrowserImageIndex[image] = projectBrowserImageList.Images.Count - 1;
				return projectBrowserImageList.Images.Count - 1;
			}

			return (int)projectBrowserImageIndex[image];
		}
*/

		public IXmlConvertable CreateMemento()
		{
			return new TreeViewMemento(this);
		}

		public void SetMemento(IXmlConvertable memento)
		{
			((TreeViewMemento)memento).Restore(this);
		}

		// Drag & Drop handling
		
/*		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			base.OnItemDrag(e);
			AbstractBrowserNode node = e.Item as AbstractBrowserNode;
			if (node != null) {
				DataObject dataObject = node.DragDropDataObject;

				if (dataObject != null) {
					DoDragDrop(dataObject, DragDropEffects.All);
				}
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			e.Effect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);

			Point clientcoordinate   = PointToClient(new Point(e.X, e.Y));
			AbstractBrowserNode node = (AbstractBrowserNode)GetNodeAt(clientcoordinate);

			DragDropEffects effect = DragDropEffects.None;

			if ((e.KeyState & 8) > 0) { // CTRL key pressed.
				effect = DragDropEffects.Copy;
			} else {
				effect = DragDropEffects.Move;
			}
			e.Effect = node.GetDragDropEffect(e.Data, effect);

			if (e.Effect != DragDropEffects.None) {
				((Form)WorkbenchSingleton.Workbench).Activate();
				Select();
				SelectedNode = node;
			}
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);

			Point clientcoordinate   = PointToClient(new Point(e.X, e.Y));
			AbstractBrowserNode node = (AbstractBrowserNode)GetNodeAt(clientcoordinate);

			if (node == null) {
				return;
			}
			node.DoDragDrop(e.Data, e.Effect);
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			projectService.SaveCombine();
		}
*/	
		static ProjectBrowserNode GetRootProjectNode(AbstractBrowserNode node)
		{
			while (node != null) {
				if(node is ProjectBrowserNode) {
					return (ProjectBrowserNode)node;
				}
				node = (AbstractBrowserNode)node.Parent;
			}
			return null;
		}

		public static void MoveCopyFile(string filename, AbstractBrowserNode node, bool move, bool alreadyInPlace)
		{
			//			FileType type      = FileUtility.GetFileType(filename);
			bool     directory = fileUtilityService.IsDirectory(filename);
			if (
//			    type == FileType.Dll ||
//			    type == FileType.Resource ||
			    directory) { // insert reference
			    return;
			    }

			    Debug.Assert(directory || File.Exists(filename), "ProjectBrowserEventHandler.MoveCopyFile : source file doesn't exist");

			// search "folder" in which the node contains
			while (!(node is DirectoryNode || node is ProjectBrowserNode))  {
				node = (AbstractBrowserNode)node.Parent;
		       	if (node == null) {
		       		return;
		       	}
			}

			string name        = System.IO.Path.GetFileName(filename);
			string baseDirectory = node is DirectoryNode ? ((DirectoryNode)node).FolderName : node.Project.BaseDirectory;
			string newfilename = alreadyInPlace ? filename : fileUtilityService.GetDirectoryNameWithSeparator(baseDirectory) + name;

			string oldrelativename = fileUtilityService.AbsoluteToRelativePath(baseDirectory, filename);
			string newrelativename = fileUtilityService.AbsoluteToRelativePath(baseDirectory, newfilename);

			AbstractBrowserNode oldparent = DefaultDotNetNodeBuilder.GetPath(oldrelativename, GetRootProjectNode(node), false);          // TODO : change this for more projects
			AbstractBrowserNode newparent = DefaultDotNetNodeBuilder.GetPath(newrelativename, GetRootProjectNode(node), alreadyInPlace);

			AbstractBrowserNode oldnode   = null; // if oldnode is == null the old file doesn't exist in current tree

			if (oldparent != null) {
				foreach (AbstractBrowserNode childnode in oldparent.Nodes) {
					if (childnode.Text == name) {
						oldnode = childnode;
						break;
					}
				}
			}

			if (oldnode != null && oldnode is DirectoryNode) { // TODO can't move folders yet :(
			                                                                                                             return;
			}

			if (oldparent == newparent && oldnode != null) { // move/copy to the same location
				return;
			}

			if (move) {
				if (filename != newfilename) {
					File.Copy(filename, newfilename);
					IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					fileService.RemoveFile(filename);
				}
				if (oldnode != null) {
					oldparent.Nodes.Remove(oldnode);
				}
			} else {
				if (filename != newfilename) {
					File.Copy(filename, newfilename);
				}
			}

			ProjectFile fInfo;
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			if (newparent.Project.IsCompileable(newfilename)) {
				fInfo = projectService.AddFileToProject(newparent.Project, newfilename, BuildAction.Compile);
			} else {
				fInfo = projectService.AddFileToProject(newparent.Project, newfilename, BuildAction.Nothing);
			}

			AbstractBrowserNode pbn = new FileNode(fInfo);
			newparent.Nodes.Add (pbn);
			
			pbn.EnsureVisible();
			projectService.SaveCombine();
		}


		// ********* Own events
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}

		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}
}
