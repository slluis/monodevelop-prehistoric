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
	public class TreeViewOptions
	{
		//protected GradientHeaderPanel optionsPanelLabel;
		
		protected ArrayList OptionPanels          = new ArrayList();
		
		protected IProperties properties = null;
		
		protected Font plainFont = null;
		protected Font boldFont  = null;

		Gtk.TreeStore treeStore;
		
		[Glade.Widget] Gtk.TreeView  TreeView;
		[Glade.Widget] Gtk.Label     optionTitle;
		[Glade.Widget] Gtk.Button    okbutton;
		[Glade.Widget] Gtk.Button    cancelbutton;
		[Glade.Widget] Gtk.Notebook  mainBook;
		[Glade.Widget] Gtk.Dialog    TreeViewOptionDialog;
		
		PixbufList    imglist;
		
		ResourceService IconService = (ResourceService)ServiceManager.Services.GetService (typeof (IResourceService));
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		
		public IProperties Properties {
			get {
				return properties;
			}
		}

		protected string Title {
			set {
				TreeViewOptionDialog.Title = value;
			}
		}
		
		protected void AcceptEvent(object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			TreeViewOptionDialog.Hide ();
			//DialogResult = DialogResult.OK;
		}
	
		public int Run ()
		{
			return TreeViewOptionDialog.Run ();
		}
	
		protected bool b = true;
		
		protected void SetOptionPanelTo(IDialogPanelDescriptor descriptor)
		{
			if (descriptor != null && descriptor.DialogPanel != null) {
				descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
				mainBook.CurrentPage = mainBook.PageNum (descriptor.DialogPanel.Control);
				optionTitle.Text = descriptor.Label;
				TreeViewOptionDialog.ShowAll ();
			}
		}		
		
		protected void AddNodes(object customizer, Gtk.TreeIter iter, ArrayList dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = customizer;
					OptionPanels.Add(descriptor.DialogPanel);
					mainBook.AppendPage (descriptor.DialogPanel.Control, new Gtk.Label ("a"));
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
			if (TreeView.Selection.GetSelected (out mdl, out iter)) {
				if (treeStore.IterHasChild (iter)) {
					Gtk.TreeIter new_iter;
					treeStore.IterChildren (out new_iter, iter);
					Gtk.TreePath new_path = treeStore.GetPath (new_iter);
					TreeView.CollapseAll ();
					TreeView.ExpandToPath (new_path);
					TreeView.Selection.SelectPath (new_path);
				} else {
					treeStore.Foreach (new Gtk.TreeModelForeachFunc (killArrows));
					treeStore.SetValue (iter, 2, imglist[3]);
					SetOptionPanelTo ((IDialogPanelDescriptor)treeStore.GetValue (iter, 1));
				}
			}
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
		}
		
		public TreeViewOptions(IProperties properties, IAddInTreeNode node)
		{
			this.properties = properties;
			
			Glade.XML treeViewXml = new Glade.XML (null, "Base.glade", "TreeViewOptionDialog", null);
			treeViewXml.Autoconnect (this);
		
			TreeViewOptionDialog.Title = StringParserService.Parse("${res:Dialog.Options.TreeViewOptions.DialogName}");

			InitImageList ();

			this.InitializeComponent();
			
			if (node != null) {
				AddNodes(properties, Gtk.TreeIter.Zero, node.BuildChildItems(this));
			}
			SelectFirstNode ();
		}

		protected void SelectFirstNode ()
		{
			TreeView.GrabFocus ();
			SelectNode (null, null);
		}
		
		protected void InitializeComponent() 
		{
			treeStore = new Gtk.TreeStore (typeof(string), typeof(IDialogPanelDescriptor), typeof (Gdk.Pixbuf));
			TreeView.Model = treeStore;

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
			TreeView.AppendColumn (empty);
			empty.Visible = false;
			TreeView.ExpanderColumn = empty;
						
			TreeView.AppendColumn (column);
			TreeView.HeadersVisible = false;

			TreeView.Selection.Changed += new EventHandler (SelectNode);
		}
		
		private void CancelEvent (object o, EventArgs args)
		{
			TreeViewOptionDialog.Hide ();
		}
	}
}
