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

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Widgets;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Gui.Pads
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

		ClassInformationEventHandler changeClassInformationHandler = null;
		Combine parseCombine;
		ArrayList ImageList;
		
		delegate void MyD();
		delegate void MyParseEventD(TreeNodeCollection nodes, ParseInformationEventArgs e);
		
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

		IClassScoutNodeBuilder[] classBrowserNodeBuilders = new IClassScoutNodeBuilder[] {
			new DefaultDotNetClassScoutNodeBuilder()
		};

		public string Id {
			get { return "ClassScout"; }
		}
		
		public string DefaultPlacement {
			get { return "Left"; }
		}

		public string Title {
			get {
				return GettextCatalog.GetString("Classes");
			}
		}

		public string Icon {
			get {
				return MonoDevelop.Gui.Stock.Class;
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
			changeClassInformationHandler = (ClassInformationEventHandler) Runtime.DispatchService.GuiDispatch (new ClassInformationEventHandler (OnClassInformationChanged));
			
			LabelEdit     = false;

			Runtime.ProjectService.CombineOpened += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler(OnCombineOpen));
			Runtime.ProjectService.CombineClosed += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler(OnCombineClosed));

			Gtk.ScrolledWindow sw = new Gtk.ScrolledWindow ();
			sw.Add(this);
			contentPanel = new Gtk.Frame();
			contentPanel.Add(sw);
			
			Runtime.Ambience.AmbienceChanged += new EventHandler(AmbienceChangedEvent);

			RowActivated += new Gtk.RowActivatedHandler(OnNodeActivated);
			PopupMenu += OnPopupMenu;
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
			Nodes.Add(new TreeNode(GettextCatalog.GetString ("Loading...")));
			StartCombineparse(e.Combine);
		}

		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			Runtime.ParserService.ClassInformationChanged -= changeClassInformationHandler;
			Nodes.Clear();
		}
		
		void OnClassInformationChanged (object sender, ClassInformationEventArgs e)
		{
			ChangeClassInformation (Nodes, e);
		}

		private void OnNodeActivated(object sender, Gtk.RowActivatedArgs args)
		{
			//base.OnDoubleClick(e);
			TreeNode node = SelectedNode;
			if (node != null) {
				ClassScoutTag tag = node.Tag as ClassScoutTag;
				if (tag != null)
					Runtime.FileService.OpenFile(tag.FileName,new FileOpeningFinished(OnFileOpened));
			}
		}
		
		private void OnFileOpened()
		{
			TreeNode node = SelectedNode;
			ClassScoutTag tag = node.Tag as ClassScoutTag;
			IWorkbenchWindow window = Runtime.FileService.GetOpenFile(tag.FileName);
			if (window == null) {
				return;
			}
			IViewContent content = window.ViewContent;
			if (content is IPositionable) {
				((IPositionable)content).JumpTo(Math.Max(0, tag.Line), 0);
			}
		}
		protected override void OnBeforeExpand (TreeViewCancelEventArgs e)
		{
			TreeNode nod = e.Node;
			if (nod.Tag == null || nod.Tag is Project)
			{
				while (nod != null && nod.Tag == null)
					nod = nod.Parent;
					
				if (nod == null) return;
				
				Project p = (Project)nod.Tag;
				foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
					if (classBrowserNodeBuilder.CanBuildClassTree(p)) {
						classBrowserNodeBuilder.ExpandNode (e.Node);
						break;
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
		private void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 3 && SelectedNode != null && SelectedNode is AbstractClassScoutNode) {
				ShowPopup ();
			}
		}

		void OnPopupMenu (object o, Gtk.PopupMenuArgs args)
		{
			if (SelectedNode != null && SelectedNode is AbstractClassScoutNode) {
				ShowPopup ();
			}
		}

		void ShowPopup ()
		{
			AbstractClassScoutNode selectedBrowserNode = (AbstractClassScoutNode) SelectedNode;
			if (selectedBrowserNode.ContextmenuAddinTreePath != null && selectedBrowserNode.ContextmenuAddinTreePath.Length > 0) {
				Runtime.Gui.Menus.ShowContextMenu(this, selectedBrowserNode.ContextmenuAddinTreePath, this);
			}
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
			//ParseCombine(parseCombine);
			//Invoke(new MyD(DoPopulate));
			Gdk.Threads.Enter();
			DoPopulate();
			Gdk.Threads.Leave();
			Runtime.ParserService.ClassInformationChanged += changeClassInformationHandler;
		}

		public void ParseCombine(Combine combine)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is Project) {
					ParseProject((Project)entry);
				} else {
					ParseCombine((Combine)entry);
				}
			}
		}

		void ParseProject(Project p)
		{
			if (p.ProjectType == "C#") {
	 			foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.BuildAction == BuildAction.Compile) {
						Runtime.ParserService.ParseFile (finfo.Name);
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
			TreeNode combineNode = new TreeNode(combine.Name);
			combineNode.Image = Stock.CombineIcon;
			
			lock (combine.Entries) {
				foreach (CombineEntry entry in combine.Entries) {
					if (entry is Project) {
						Populate((Project)entry, combineNode.Nodes);
					} else {
						Populate((Combine)entry, combineNode.Nodes);
					}
				}
			}
				
			nodes.Add(combineNode);
		}

		void Populate(Project p, TreeNodeCollection nodes)
		{
			// only C# is currently supported.
			bool builderFound = false;
			foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
				if (classBrowserNodeBuilder.CanBuildClassTree(p)) {
					TreeNode prjNode = classBrowserNodeBuilder.BuildClassTreeNode(p);
					nodes.Add(prjNode);
					prjNode.Tag = p;
					builderFound = true;
					break;
				}
			}

			// no builder found -> create 'dummy' node
			if (!builderFound) {
				TreeNode prjNode = new TreeNode(p.Name);
				//prjNode.SelectedImageIndex = prjNode.ImageIndex = imageIndexOffset + iconService.GetImageIndexForProjectType(p.ProjectType);
				string lang = (p is DotNetProject) ? ((DotNetProject)p).LanguageName : "";
				prjNode.Image = Runtime.Gui.Icons.GetImageForProjectType(lang);
				prjNode.Nodes.Add(new TreeNode(GettextCatalog.GetString ("No class builder found")));
				prjNode.Tag = p;
				nodes.Add(prjNode);
			}
		}

		void ChangeClassInformation(TreeNodeCollection nodes, ClassInformationEventArgs e)
		{
			BeginUpdate();
			foreach (TreeNode node in nodes) {
				if (node.Tag is Project) {
					Project p = (Project)node.Tag;
					if (p.IsFileInProject(e.FileName)) {
						foreach (IClassScoutNodeBuilder classBrowserNodeBuilder in classBrowserNodeBuilders) {
							classBrowserNodeBuilder.UpdateClassTree(node, e);
							break;
						}
					}
				} else {
					ChangeClassInformation(node.Nodes, e);
				}
			}
			EndUpdate();
		}
	}
}
