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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;
using System.Resources;
using System.Xml;
using System.Threading;
using System.Text;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ClassScoutTag
	{
		int    line;
		string filename;

		public int Line {
			get {
				return line;
			}
		}

		public string FileName {
			get {
				return filename;
			}
		}

		public ClassScoutTag(int line, string filename)
		{
			this.line     = line;
			this.filename = filename;
		}
	}

	/// <summary>
	/// This class is the project scout tree view
	/// </summary>
	public class ClassScout : TreeView, IPadContent
	{
		//Panel contentPanel = new Panel();
		Gtk.Frame contentPanel;
		int imageIndexOffset = -1;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		ParseInformationEventHandler addParseInformationHandler = null;
		ParseInformationEventHandler removeParseInformationHandler = null;
		Combine parseCombine;
		ArrayList ImageList;
		
		delegate void MyD();
		delegate void MyParseEventD(TreeNodeCollection nodes, ParseInformationEventArgs e);
		
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

		IClassScoutNodeBuilder[] classBrowserNodeBuilders = new IClassScoutNodeBuilder[] {
			new DefaultDotNetClassScoutNodeBuilder()
		};

		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.ClassScoutLabel");
			}
		}

		public string Icon {
			get {
				return "Icons.16x16.Class";
			}
		}

		public Gtk.Widget Control {
			get {
				return contentPanel;
			}
		}

		public void BringToFront() {
			// TODO
		}
		
		public ClassScout() : base (false, TreeNodeComparer.GtkDefault)
		{
			addParseInformationHandler = new ParseInformationEventHandler(OnParseInformationAdded);
			removeParseInformationHandler = new ParseInformationEventHandler(OnParseInformationRemoved);
			
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));

			this.ImageList  = classBrowserIconService.ImageList;

			imageIndexOffset                      = ImageList.Count;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			foreach (Gdk.Pixbuf img in iconService.ImageList) {
				ImageList.Add(img);
			}

			LabelEdit     = false;
			//HotTracking   = false;
			//AllowDrop     = true;
			//HideSelection = false;
			//Dock          = DockStyle.Fill;

			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);

			Gtk.ScrolledWindow sw = new Gtk.ScrolledWindow ();
			sw.Add(this);
			contentPanel = new Gtk.Frame();
			contentPanel.Add(sw);
			
			AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			ambienceService.AmbienceChanged += new EventHandler(AmbienceChangedEvent);

			RowActivated += new Gtk.RowActivatedHandler(OnNodeActivated);
			contentPanel.ButtonReleaseEvent += new Gtk.ButtonReleaseEventHandler(OnButtonRelease);
		}

		void AmbienceChangedEvent(object sender, EventArgs e)
		{
			if (parseCombine != null) {
				DoPopulate();
			}
		}

		public void RedrawContent()
		{
		}

		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			Nodes.Clear();
			Nodes.Add(new TreeNode("Loading..."));
			StartCombineparse(e.Combine);
		}

		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			parserService.ParseInformationAdded -= addParseInformationHandler;
			parserService.ParseInformationRemoved -= removeParseInformationHandler;
			Nodes.Clear();
		}

		void OnParseInformationAdded(object sender, ParseInformationEventArgs e)
		{
			//Invoke(new MyParseEventD(AddParseInformation), new object[] { Nodes, e });
			Gdk.Threads.Enter();
			AddParseInformation(Nodes, e);
			Gdk.Threads.Leave();
			
		}
		
		void OnParseInformationRemoved(object sender, ParseInformationEventArgs e)
		{
			//Invoke(new MyParseEventD(RemoveParseInformation), new object[] { Nodes, e });
			Gdk.Threads.Enter();
			RemoveParseInformation(Nodes, e);
			Gdk.Threads.Leave();
		}

#if !GTK
		protected override void OnDoubleClick(EventArgs e)
#else
		private void OnNodeActivated(object sender, Gtk.RowActivatedArgs args)
#endif
		{
			//base.OnDoubleClick(e);
			TreeNode node = SelectedNode;
			if (node != null) {
				ClassScoutTag tag = node.Tag as ClassScoutTag;
				if (tag != null) {
					IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					fileService.OpenFile(tag.FileName);
					
					IViewContent content = fileService.GetOpenFile(tag.FileName).ViewContent;
					if (content is IPositionable) {
						if (tag.Line > 0) {
							((IPositionable)content).JumpTo(tag.Line - 1, 0);
							content.Control.GrabFocus();
						}
					}
				}
			}
		}
/*
		protected override void OnMouseDown(MouseEventArgs e)
		{
			TreeNode node = (TreeNode)GetNodeAt(e.X, e.Y);

			if (node != null) {
				SelectedNode = node;
			}
			base.OnMouseDown(e);
		}
*/
#if GTK
		private void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
#else
		protected override void OnMouseUp(MouseEventArgs e)
#endif
		{
			//if (e.Button == MouseButtons.Right && SelectedNode != null && SelectedNode is AbstractClassScoutNode) {
			if (args.Event.Button == 3 && SelectedNode != null && SelectedNode is AbstractClassScoutNode) {
				AbstractClassScoutNode selectedBrowserNode = (AbstractClassScoutNode)SelectedNode;
				if (selectedBrowserNode.ContextmenuAddinTreePath != null && selectedBrowserNode.ContextmenuAddinTreePath.Length > 0) {
					MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
					menuService.ShowContextMenu(this, selectedBrowserNode.ContextmenuAddinTreePath, this);
				}
			}

			//base.OnMouseUp(e);
		}

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
		
		void StartCombineparse(Combine combine)
		{
			parseCombine = combine;
			System.Threading.Thread t = new Thread(new ThreadStart(StartPopulating));
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}
		
		void StartPopulating()
		{
			ParseCombine(parseCombine);
			//Invoke(new MyD(DoPopulate));
			Gdk.Threads.Enter();
			DoPopulate();
			Gdk.Threads.Leave();
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			parserService.ParseInformationAdded += addParseInformationHandler;
			parserService.ParseInformationRemoved += removeParseInformationHandler;
		}

		public void ParseCombine(Combine combine)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					ParseProject(((ProjectCombineEntry)entry).Project);
				} else {
					ParseCombine(((CombineCombineEntry)entry).Combine);
				}
			}
		}

		void ParseProject(IProject p)
		{
			if (p.ProjectType == "C#") {
	 			foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.BuildAction == BuildAction.Compile) {
						IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
						parserService.ParseFile(finfo.Name);
					}
	 			}
			}
		}

		void DoPopulate()
		{
			BeginUpdate();
			Nodes.Clear();
			try {
				Populate(parseCombine, Nodes);
			} catch (Exception e) {
				//MessageBox.Show(e.ToString(), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Gtk.MessageDialog md = new Gtk.MessageDialog (null,
					Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Error,
					Gtk.ButtonsType.Ok, e.ToString());	
				md.Run ();
				md.Destroy();
			}
			EndUpdate();
		}

		public void Populate(Combine combine, TreeNodeCollection nodes)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			TreeNode combineNode = new TreeNode(combine.Name);
			//combineNode.SelectedImageIndex = combineNode.ImageIndex = classBrowserIconService.CombineIndex;
			combineNode.Image = classBrowserIconService.ImageList[classBrowserIconService.CombineIndex];
			
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					Populate(((ProjectCombineEntry)entry).Project, combineNode.Nodes);
				} else {
					Populate(((CombineCombineEntry)entry).Combine, combineNode.Nodes);
				}
			}
			
			nodes.Add(combineNode);
		}

		void Populate(IProject p, TreeNodeCollection nodes)
		{
			// only C# is currently supported.
			bool builderFound = false;
			foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
				if (classBrowserNodeBuilder.CanBuildClassTree(p)) {
					TreeNode prjNode = classBrowserNodeBuilder.BuildClassTreeNode(p, imageIndexOffset);
					nodes.Add(prjNode);
					prjNode.Tag = p;
					builderFound = true;
					break;
				}
			}

			// no builder found -> create 'dummy' node
			if (!builderFound) {
				TreeNode prjNode = new TreeNode(p.Name);
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
				//prjNode.SelectedImageIndex = prjNode.ImageIndex = imageIndexOffset + iconService.GetImageIndexForProjectType(p.ProjectType);
				prjNode.Image = iconService.GetImageForProjectType(p.ProjectType);
				prjNode.Nodes.Add(new TreeNode("No class builder found"));
				prjNode.Tag = p;
				nodes.Add(prjNode);
			}
		}

		void AddParseInformation(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
			BeginUpdate();
			foreach (TreeNode node in nodes) {
				if (node.Tag is IProject) {
					IProject p = (IProject)node.Tag;
					if (p.IsFileInProject(e.FileName)) {
						foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
							classBrowserNodeBuilder.AddToClassTree(node, e);
							break;
						}
					}
				} else {
					AddParseInformation(node.Nodes, e);
				}
			}
			EndUpdate();
		}
		
		void RemoveParseInformation(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
			BeginUpdate();
			foreach (TreeNode node in nodes) {
				if (node.Tag is IProject) {
					IProject p = (IProject)node.Tag;
					if (p.IsFileInProject(e.FileName)) {
						foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
							classBrowserNodeBuilder.RemoveFromClassTree(node, e);
							break;
						}
					}
				} else {
					RemoveParseInformation(node.Nodes, e);
				}
			}
			EndUpdate();
		}
	}
}
