// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

using Gtk;
using MonoDevelop.GuiUtils;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	///  This class is for creating a new "empty" file
	/// </summary>
	public class NewFileDialog : Dialog, INewFileCreator
	{
		ArrayList alltemplates = new ArrayList ();
		ArrayList categories   = new ArrayList ();
		Hashtable icons        = new Hashtable ();

		PixbufList cat_imglist;

		TreeStore catStore;
		TreeStore templateStore;
		Gtk.TreeView catView;
		Gtk.TreeView templateView;
		IconView TemplateView;
		Button okButton;
		Button cancelButton;
		Label infoLabel;

		//IconService iconService = (IconService)ServiceManager.Services.GetService (typeof(IconService));
		ResourceService iconService = (ResourceService)ServiceManager.Services.GetService (typeof(IResourceService));
		
		public NewFileDialog () : base ()
		{
			this.Title = "New file";
			this.TransientFor = (Window) WorkbenchSingleton.Workbench;
			this.BorderWidth = 6;
			this.HasSeparator = false;
			
			try {
				InitializeComponents();
				InitializeTemplates();
				InitializeView();
				
				//((TreeView)ControlDictionary["categoryTreeView"]).Select();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
			ShowAll ();
		}
		
		void InitializeView()
		{
			PixbufList smalllist  = new PixbufList();
			PixbufList imglist    = new PixbufList();
			
			smalllist.Add(iconService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			imglist.Add(iconService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			foreach (DictionaryEntry entry in icons) {
				Gdk.Pixbuf bitmap = iconService.GetBitmap(entry.Key.ToString());
				if (bitmap != null) {
					smalllist.Add(bitmap);
					imglist.Add(bitmap);
					tmp[entry.Key] = ++i;
				} else {
					Console.WriteLine("can't load bitmap " + entry.Key.ToString() + " using default");
				}
			}
			
			icons = tmp;
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
					//item.ImageIndex = 0;
				} else {
					//item.ImageIndex = (int)icons[item.Template.Icon];
				}
			}
			
			InsertCategories(TreeIter.Zero, categories);
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			/*for (int j = 0; j < categories.Count; ++j) {
				if (((Category)categories[j]).Name == propertyService.GetProperty("Dialogs.NewFileDialog.LastSelectedCategory", "C#")) {
					((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode = (TreeNode)((TreeView)ControlDictionary["categoryTreeView"]).Nodes[j];
					break;
				}
			}*/
		}
		
		void InsertCategories(TreeIter node, ArrayList catarray)
		{
			foreach (Category cat in catarray) {
				if (node.Equals(Gtk.TreeIter.Zero)) {
					node = catStore.AppendValues (cat.Name, cat.Categories, cat.Templates, cat_imglist[1]);
				} else {
					node = catStore.AppendValues (cat.Name, cat.Categories, cat.Templates, cat_imglist[1]);
				}
				InsertCategories(node, cat.Categories);
			}
		}
		
		// TODO : insert sub categories
		Category GetCategory(string categoryname)
		{
			foreach (Category category in categories) {
				if (category.Name == categoryname) {
					return category;
				}
			}
			Category newcategory = new Category(categoryname);
			categories.Add(newcategory);
			return newcategory;
		}
		
		void InitializeTemplates()
		{
			foreach (FileTemplate template in FileTemplate.FileTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				Category cat = GetCategory(titem.Template.Category);
				cat.Templates.Add(titem); 
				
				if (cat.Selected == false && template.WizardPath == null) {
					cat.Selected = true;
				}
				if (!cat.HasSelectedTemplate && titem.Template.Files.Count == 1) {
					if (((FileDescriptionTemplate)titem.Template.Files[0]).Name.StartsWith("Empty")) {
						//titem.Selected = true;
						cat.HasSelectedTemplate = true;
					}
				}
				alltemplates.Add(titem);
			}
		}
		
		bool FixCatIcons (TreeModel mdl, TreePath path, TreeIter iter) {
			((TreeStore)mdl).SetValue (iter, 3, cat_imglist[1]);
			return false;
		}
		
		// tree view event handlers
		void CategoryChange(object sender, EventArgs e)
		{
			TreeModel mdl;
			TreeIter  iter;
			if (catView.Selection.GetSelected (out mdl, out iter)) {
				((TreeStore)mdl).Foreach (new TreeModelForeachFunc (FixCatIcons));
				((TreeStore)mdl).SetValue (iter, 3, cat_imglist[0]);
				//templateStore.Clear ();
                                TemplateView.Clear ();
				foreach (TemplateItem item in (ArrayList)((Gtk.TreeStore)mdl).GetValue (iter, 2)) {
					//templateStore.AppendValues (item.Name, item.Template);
					
					TemplateView.AddIcon(new Gtk.Image(iconService.GetBitmap (item.Template.Icon)), item.Name, item.Template);
				}
				okButton.Sensitive = false;
			}
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			TreeModel mdl;
			TreeIter iter;
			FileTemplate item = (FileTemplate)TemplateView.CurrentlySelected;

			if (item != null)
			{
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService (typeof (IResourceService));
                        	if (item.Description.StartsWith("${")) {
                                	infoLabel.Text = resourceService.GetString (item.Description);
                        	} else {
                                	infoLabel.Text = item.Description;
                        	}
				okButton.Sensitive = true;
			}
//			if (templateView.Selection.GetSelected (out mdl, out iter)) {
//				StringParserService sps = (StringParserService)ServiceManager.Services.GetService (typeof(StringParserService));
//				string text = sps.Parse (((FileTemplate)((Gtk.TreeStore)mdl).GetValue (iter, 1)).Description);
//				infoLabel.Text = text;
//				okButton.Sensitive = true;
//			}
		}
		
		// button events
		
		void CheckedChange(object sender, EventArgs e)
		{
			//((ListView)ControlDictionary["templateListView"]).View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(string filename, string content, string languageName, bool showFile)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.NewFile(filename, languageName, content);
		}
		
		void OpenEvent(object sender, EventArgs e)
		{
			TreeModel mdl;
			TreeIter iter;

			//FIXME: we need to set this up
			//PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			//propertyService.SetProperty("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
			//propertyService.SetProperty("Dialogs.NewFileDialog.LastSelectedCategory", ((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode.Text);
			
			//if (templateView.Selection.GetSelected (out mdl, out iter)) {
			if (TemplateView.CurrentlySelected != null) {
				FileTemplate item = (FileTemplate)TemplateView.CurrentlySelected;
				//FileTemplate item = (FileTemplate)((Gtk.TreeStore)mdl).GetValue (iter, 1);
				
				if (item.WizardPath != null) {
					IProperties customizer = new DefaultProperties();
					customizer.SetProperty("Template", item);
					customizer.SetProperty("Creator",  this);
					WizardDialog wizard = new WizardDialog("File Wizard", customizer, item.WizardPath);
					//if (wizard.ShowDialog() == DialogResult.OK) {
						//DialogResult = DialogResult.OK;
					//}
				} else {
					foreach (FileDescriptionTemplate newfile in item.Files) {
						SaveFile(newfile.Name, newfile.Content, item.LanguageName, true);
					}
				}
				
				Respond ((int)Gtk.ResponseType.Ok);
				Hide ();
			}
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		internal class Category
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			string name;
			public bool Selected = false;
			public bool HasSelectedTemplate = false;
			public Category(string name)
			{
				this.name = name;
				//ImageIndex = 1;
			}
			
			public string Name {
				get {
					return name;
				}
			}
			public ArrayList Categories {
				get {
					return categories;
				}
			}
			public ArrayList Templates {
				get {
					return templates;
				}
			}
		}
		
		/// <summary>
		///  Represents a new file template
		/// </summary>
		class TemplateItem
		{
			FileTemplate template;
			string name;
			
			public TemplateItem(FileTemplate template)
			{
				this.template = template;
				StringParserService sps = (StringParserService)ServiceManager.Services.GetService (typeof(StringParserService));
				this.name = sps.Parse(template.Name);
			}

			public string Name {
				get {
					return name;
				}
			}
			
			public FileTemplate Template {
				get {
					return template;
				}
			}
		}

		void cancelClicked (object o, EventArgs e) {
			Hide ();
		}
		
		void InitializeComponents()
		{
			
			catStore = new Gtk.TreeStore (typeof(string), typeof(ArrayList), typeof(ArrayList), typeof(Gdk.Pixbuf));
			catStore.SetSortColumnId (0, SortType.Ascending);
			
			templateStore = new Gtk.TreeStore (typeof(string), typeof(FileTemplate));

			ScrolledWindow swindow1 = new ScrolledWindow();
			swindow1.VscrollbarPolicy = PolicyType.Automatic;
			swindow1.HscrollbarPolicy = PolicyType.Automatic;
			swindow1.ShadowType = ShadowType.In;
			catView = new Gtk.TreeView (catStore);
			catView.WidthRequest = 160;
			catView.HeadersVisible = false;
			templateView = new Gtk.TreeView (templateStore);
			TemplateView = new IconView();

			TreeViewColumn catColumn = new TreeViewColumn ();
			catColumn.Title = "categories";
			//CellRendererPixbuf cat_pix_render = new CellRendererPixbuf ();
			//catColumn.PackStart (cat_pix_render, false);
			//catColumn.AddAttribute (cat_pix_render, "pixbuf", 3);
			
			CellRendererText cat_text_render = new CellRendererText ();
			catColumn.PackStart (cat_text_render, true);
			catColumn.AddAttribute (cat_text_render, "text", 0);

			catView.AppendColumn (catColumn);

			TreeViewColumn templateColumn = new TreeViewColumn ();
			templateColumn.Title = "template";
			CellRendererText tmpl_text_render = new CellRendererText ();
			templateColumn.PackStart (tmpl_text_render, true);
			templateColumn.AddAttribute (tmpl_text_render, "text", 0);
			templateView.AppendColumn (templateColumn);

			//okButton = new Gtk.Button ("Create");
			okButton = new Button (Stock.New);
			okButton.Clicked += new EventHandler (OpenEvent);

			//cancelButton = new Gtk.Button ("Cancel");
			cancelButton = new Button (Stock.Close);
			cancelButton.Clicked += new EventHandler (cancelClicked);

			infoLabel = new Label ("");
			Frame infoLabelFrame = new Frame();
			infoLabelFrame.Add(infoLabel);

			//VBox mainbox = new VBox (false, 5);
			//mainbox.BorderWidth = 5;
			//mainbox.Spacing = 5;
			
			HBox viewbox = new HBox (false, 6);
			swindow1.Add(catView);
			viewbox.PackStart (swindow1,false,true,0);
			//viewbox.PackStart (templateView);
			viewbox.PackStart(TemplateView, true, true,0);

			this.AddActionWidget (cancelButton, (int)Gtk.ResponseType.Cancel);
			this.AddActionWidget (okButton, (int)Gtk.ResponseType.Ok);

			this.VBox.PackStart (viewbox);
			this.VBox.PackStart (infoLabelFrame, false, false, 6);

			cat_imglist = new PixbufList();
			cat_imglist.Add(iconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			cat_imglist.Add(iconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
	
			catView.Selection.Changed += new EventHandler (CategoryChange);
			//templateView.Selection.Changed += new EventHandler (SelectedIndexChange);
			TemplateView.IconSelected += new EventHandler(SelectedIndexChange);
			TemplateView.IconDoubleClicked += new EventHandler(OpenEvent);

			//ControlDictionary["openButton"].Click += new EventHandler(OpenEvent);
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			//((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked = propertyService.GetProperty("Dialogs.NewProjectDialog.LargeImages", true);
			//((RadioButton)ControlDictionary["largeIconsRadioButton"]).CheckedChanged += new EventHandler(CheckedChange);
			//((RadioButton)ControlDictionary["largeIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			//((RadioButton)ControlDictionary["largeIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.LargeIconsIcon");
			
			//((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked = !propertyService.GetProperty("Dialogs.NewProjectDialog.LargeImages", true);
			//((RadioButton)ControlDictionary["smallIconsRadioButton"]).CheckedChanged += new EventHandler(CheckedChange);
			//((RadioButton)ControlDictionary["smallIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			//((RadioButton)ControlDictionary["smallIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.SmallIconsIcon");
			
		
			//ToolTip tooltip = new ToolTip();
			//tooltip.SetToolTip(ControlDictionary["largeIconsRadioButton"], StringParserService.Parse("${res:Global.LargeIconToolTip}"));
			//tooltip.SetToolTip(ControlDictionary["smallIconsRadioButton"], StringParserService.Parse("${res:Global.SmallIconToolTip}"));
			//tooltip.Active = true;
			//Owner         = (Form)WorkbenchSingleton.Workbench;
			//StartPosition = FormStartPosition.CenterParent;
			//Icon          = null;
			
			//CheckedChange(this, EventArgs.Empty);
		}
	}
}
