// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public class TreeViewOptions {
		
		protected ArrayList OptionPanels = new ArrayList ();
		protected IProperties properties = null;

		Gtk.TreeStore treeStore;
		
		[Glade.Widget] Gtk.TreeView  TreeView;
		[Glade.Widget] Gtk.Label     optionTitle;
		[Glade.Widget] Gtk.Notebook  mainBook;
		[Glade.Widget] Gtk.Image     panelImage;
		[Glade.Widget] Gtk.Dialog    TreeViewOptionDialog;
		
		StringParserService StringParserService = (StringParserService) ServiceManager.Services.GetService (typeof (StringParserService));
		
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
		
		protected void AcceptEvent (object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage (DialogMessage.OK))
					return;
			}
			TreeViewOptionDialog.Hide ();
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
				if (descriptor.DialogPanel.Icon == null) {
					panelImage.Stock = Gtk.Stock.Preferences;
				} else {
					//FIXME: this needs to actually switch over the ImageType and use that instead of this *hack*
					if (descriptor.DialogPanel.Icon.Stock != null)
						panelImage.Stock = descriptor.DialogPanel.Icon.Stock;
					else
						panelImage.Pixbuf = descriptor.DialogPanel.Icon.Pixbuf;
				}
				optionTitle.Markup = "<span weight=\"bold\" size=\"x-large\">" + descriptor.Label + "</span>";
				TreeViewOptionDialog.ShowAll ();
			}
		}		
		
		protected void AddNodes(object customizer, Gtk.TreeIter iter, ArrayList dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = customizer;
					((Gtk.Frame)descriptor.DialogPanel.Control).Shadow = Gtk.ShadowType.None;
					OptionPanels.Add(descriptor.DialogPanel);
					mainBook.AppendPage (descriptor.DialogPanel.Control, new Gtk.Label ("a"));
				}
			
				Gtk.TreeIter i;
				if (iter.Equals (Gtk.TreeIter.Zero)) {
					i = treeStore.AppendValues (descriptor.Label, descriptor);
				} else {
					i = treeStore.AppendValues (iter, descriptor.Label, descriptor);
				}
				
				if (descriptor.DialogPanelDescriptors != null) {
					AddNodes (customizer, i, descriptor.DialogPanelDescriptors);
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
					TreeView.ExpandToPath (new_path);
					TreeView.Selection.SelectPath (new_path);
				} else {
					SetOptionPanelTo ((IDialogPanelDescriptor)treeStore.GetValue (iter, 1));
				}
			}
		}
		public TreeViewOptions (IProperties properties, IAddInTreeNode node)
		{
			this.properties = properties;
			
			Glade.XML treeViewXml = new Glade.XML (null, "Base.glade", "TreeViewOptionDialog", null);
			treeViewXml.Autoconnect (this);
		
			TreeViewOptionDialog.TransientFor = (Gtk.Window)WorkbenchSingleton.Workbench;
			TreeViewOptionDialog.WindowPosition = Gtk.WindowPosition.CenterOnParent;
		
			TreeViewOptionDialog.Title = StringParserService.Parse("${res:Dialog.Options.TreeViewOptions.DialogName}");

			this.InitializeComponent();
			
			if (node != null)
				AddNodes (properties, Gtk.TreeIter.Zero, node.BuildChildItems(this));
			
			SelectFirstNode ();
		}

		protected void SelectFirstNode ()
		{
			TreeView.GrabFocus ();
			SelectNode (null, null);
		}
		
		protected void InitializeComponent () 
		{
			treeStore = new Gtk.TreeStore (typeof (string), typeof (IDialogPanelDescriptor));
			
			TreeView.Model = treeStore;
			TreeView.AppendColumn ("", new Gtk.CellRendererText (), "text", 0);
			TreeView.Selection.Changed += new EventHandler (SelectNode);
		}
		
		private void CancelEvent (object o, EventArgs args)
		{
			TreeViewOptionDialog.Hide ();
		}
	}
}
