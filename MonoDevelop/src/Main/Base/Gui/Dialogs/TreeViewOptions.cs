// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/*
	public class GradientHeaderPanel : Label
	{
		public GradientHeaderPanel(int fontSize) : this()
		{
			Font = new Font("Tahoma", fontSize);
		}
		
		public GradientHeaderPanel() : base()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			ResizeRedraw = true;
			Text = String.Empty;
		}

		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			
			g.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(Width, Height), 
			                                        SystemColors.Window, SystemColors.Control),
							new Rectangle(0, 0, Width, Height));
		}
	}*/
	
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public class TreeViewOptions : Gtk.Window
	{
		//protected GradientHeaderPanel optionsPanelLabel;
		
		protected ArrayList OptionPanels          = new ArrayList();
		
		protected IProperties properties = null;
		
		protected Font plainFont = null;
		protected Font boldFont  = null;

		Gtk.TreeStore treeStore;
		Gtk.TreeView  treeView;
		Gtk.Label     topLabel;
		Gtk.Button    okButton;
		Gtk.Button    cancelButton;
		Gtk.Frame     optionPanel;

		PixbufList    imglist;
	
		ResourceService IconService = (ResourceService)ServiceManager.Services.GetService (typeof (IResourceService));
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
	
		public IProperties Properties {
			get {
				return properties;
			}
		}
		
		protected void AcceptEvent(object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			Hide ();
			//DialogResult = DialogResult.OK;
		}
		
		/*protected void ResetImageIndex(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes) {
				if (node.Nodes.Count > 0) {
					ResetImageIndex(node.Nodes);
				} else {
					node.ImageIndex         = 2;
					node.SelectedImageIndex = 3;
				}
			}
		}*/
		
		protected bool b = true;
		
		/*protected void BeforeExpandNode(object sender, TreeViewCancelEventArgs e)
		{
			if (!b) {
				return;
			}
			b = false;
			((TreeView)ControlDictionary["optionsTreeView"]).BeginUpdate();
			// search first leaf node (leaf nodes have no children)
			TreeNode node = e.Node.FirstNode;
			while (node.Nodes.Count > 0) {
				node = node.FirstNode;
			}
			((TreeView)ControlDictionary["optionsTreeView"]).CollapseAll();
			node.EnsureVisible();
			node.ImageIndex = 3;
			((TreeView)ControlDictionary["optionsTreeView"]).EndUpdate();
			SetOptionPanelTo(node);
			b = true;
		}*/
		
		/*protected void BeforeSelectNode(object sender, TreeViewCancelEventArgs e)
		{
			ResetImageIndex(((TreeView)ControlDictionary["optionsTreeView"]).Nodes);
			if (b) {
				CollapseOrExpandNode(e.Node);
			}
		}*/
		
		/*protected void HandleClick(object sender, EventArgs e)
		{
			if (((TreeView)ControlDictionary["optionsTreeView"]).GetNodeAt(((TreeView)ControlDictionary["optionsTreeView"]).PointToClient(Control.MousePosition)) == ((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode && b) {
				CollapseOrExpandNode(((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode);
			}
		}*/
		
		/*void CollapseOrExpandNode(TreeNode node)
		{
			if (node.Nodes.Count > 0) {  // only folders
				if (node.IsExpanded) {
					node.Collapse();
				}  else {
					node.Expand();			
				}
			}
		}*/
		
		protected void SetOptionPanelTo(IDialogPanelDescriptor descriptor)
		{
			if (descriptor != null && descriptor.DialogPanel != null) {
				descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
				foreach (Gtk.Widget widg in optionPanel.Children) {
					optionPanel.Remove (widg);
				}
				optionPanel.Add (descriptor.DialogPanel.Control);
				topLabel.Text = descriptor.Label;
				ShowAll ();
			}
		}
		
		/*void TreeMouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = ((TreeView)ControlDictionary["optionsTreeView"]).GetNodeAt(((TreeView)ControlDictionary["optionsTreeView"]).PointToClient(Control.MousePosition));
			if (node != null) {
				if (node.Nodes.Count == 0) ((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = node;
			}
		}*/
		
		protected void AddNodes(object customizer, Gtk.TreeIter iter, ArrayList dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = customizer;
					OptionPanels.Add(descriptor.DialogPanel);
				}
			
				Gtk.TreeIter i;
				if (iter.Equals (Gtk.TreeIter.Zero)) {
					i = treeStore.AppendValues  (descriptor.Label, descriptor, imglist[2]);
				} else {
					i = treeStore.AppendValues(iter, descriptor.Label, descriptor, imglist[2]);
				}
				if (descriptor.DialogPanelDescriptors != null) {
					AddNodes(customizer, i, descriptor.DialogPanelDescriptors);
				}
			}
		}
		
		protected void SelectNode(object sender, EventArgs e)
		{
			Gtk.TreeModel mdl;
			Gtk.TreeIter  iter;
			if (treeView.Selection.GetSelected (out mdl, out iter)) {
				if (treeStore.IterHasChild (iter)) {
					Gtk.TreeIter new_iter;
					treeStore.IterChildren (out new_iter, iter);
					Gtk.TreePath new_path = treeStore.GetPath (new_iter);
					treeView.CollapseAll ();
					treeView.ExpandToPath (new_path);
					treeView.Selection.SelectPath (new_path);
				} else {
					treeStore.Foreach (new Gtk.TreeModelForeachFunc (killArrows));
					treeStore.SetValue (iter, 2, imglist[3]);
					SetOptionPanelTo ((IDialogPanelDescriptor)treeStore.GetValue (iter, 1));
				}
			}
			
			//SetOptionPanelTo(((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode);
		}

		bool killArrows (Gtk.TreeModel mdl, Gtk.TreePath path, Gtk.TreeIter iter)
		{
			treeStore.SetValue (iter, 2, imglist[2]);
			return false;
		}
		
		protected void InitImageList()
		{
			imglist = new PixbufList();
			imglist.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Add(IconService.GetBitmap("Icons.16x16.Empty") );
			imglist.Add(IconService.GetBitmap("Icons.16x16.SelectionArrow"));
			
			//((TreeView)ControlDictionary["optionsTreeView"]).ImageList = imglist;
		}
		
		/*protected void ShowOpenFolderIcon(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Nodes.Count > 0) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
		}*/
		
		/*protected void ShowClosedFolderIcon(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Nodes.Count > 0) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}*/
		
		public TreeViewOptions(IProperties properties, IAddInTreeNode node) : base ("Options...")
		{
			this.properties = properties;
			
			this.Title = StringParserService.Parse("${res:Dialog.Options.TreeViewOptions.DialogName}");

			InitImageList ();

			this.InitializeComponent();
			
			//plainFont = new Font(((TreeView)ControlDictionary["optionsTreeView"]).Font, FontStyle.Regular);
			//boldFont  = new Font(((TreeView)ControlDictionary["optionsTreeView"]).Font, FontStyle.Bold);
			
			if (node != null) {
				AddNodes(properties, Gtk.TreeIter.Zero, node.BuildChildItems(this));
			}
		}
		
		protected void InitializeComponent() 
		{
			
			treeStore = new Gtk.TreeStore (typeof(string), typeof(IDialogPanelDescriptor), typeof (Gdk.Pixbuf));
			treeView = new Gtk.TreeView (treeStore);

			Gtk.TreeViewColumn column = new Gtk.TreeViewColumn ();
			column.Title = "items";
			
			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			pix_render.PixbufExpanderOpen = imglist[1];
			pix_render.PixbufExpanderClosed = imglist[0];

			column.PackStart (pix_render, false);
			column.AddAttribute (pix_render, "pixbuf", 2);
			Gtk.CellRendererText text_render = new Gtk.CellRendererText ();
			column.PackStart (text_render, true);
			column.AddAttribute (text_render, "text", 0);
			
			
			Gtk.TreeViewColumn empty = new Gtk.TreeViewColumn ("a", new Gtk.CellRendererText (), "string", 0);
			treeView.AppendColumn (empty);
			empty.Visible = false;
			treeView.ExpanderColumn = empty;
						
			treeView.AppendColumn (column);
			treeView.HeadersVisible = false;

			Gtk.VBox mainbox = new Gtk.VBox (false, 2);
			
			Gtk.HBox dispbox = new Gtk.HBox (false, 2);

			dispbox.PackStart (treeView);

			Gtk.VBox vbox = new Gtk.VBox (false, 2);
			
			topLabel = new Gtk.Label ("");
			vbox.PackStart (topLabel, false, false, 2);

			optionPanel = new Gtk.Frame ();
			optionPanel.Shadow = Gtk.ShadowType.None;
			
			vbox.PackStart (optionPanel);
			
			dispbox.PackStart (vbox);

			mainbox.PackStart (dispbox);
			
			Gtk.HButtonBox buttonBox = new Gtk.HButtonBox ();
			buttonBox.Layout = Gtk.ButtonBoxStyle.End;

			okButton = new Gtk.Button (Gtk.Stock.Ok);
			cancelButton = new Gtk.Button (Gtk.Stock.Cancel);

			buttonBox.PackStart (okButton);
			buttonBox.PackStart (cancelButton);

			mainbox.PackStart (buttonBox, false, false, 2);

			this.Add (mainbox);

			okButton.Clicked += new EventHandler (AcceptEvent);
			cancelButton.Clicked += new EventHandler (CancelEvent);
			
			//((TreeView)ControlDictionary["optionsTreeView"]).Click          += new EventHandler(HandleClick);
			//((TreeView)ControlDictionary["optionsTreeView"]).AfterSelect    += new TreeViewEventHandler(SelectNode);
			//((TreeView)ControlDictionary["optionsTreeView"]).BeforeSelect   += new TreeViewCancelEventHandler(BeforeSelectNode);
			treeView.Selection.Changed += new EventHandler (SelectNode);
			//((TreeView)ControlDictionary["optionsTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(BeforeExpandNode);
			//((TreeView)ControlDictionary["optionsTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(ShowOpenFolderIcon);
			//((TreeView)ControlDictionary["optionsTreeView"]).BeforeCollapse += new TreeViewCancelEventHandler(ShowClosedFolderIcon);
			//((TreeView)ControlDictionary["optionsTreeView"]).MouseDown      += new MouseEventHandler(TreeMouseDown);
		}
		
		private void CancelEvent (object o, EventArgs args)
		{
			this.Hide ();
		}
	}
}
