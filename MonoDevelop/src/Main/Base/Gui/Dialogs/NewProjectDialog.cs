// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using MonoDevelop.Gui.Widgets;
using Gtk;
using GladeSharp;
using Glade;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	/// <summary>
	/// This class displays a new project dialog and sets up and creates a a new project,
	/// the project types are described in an XML options file
	/// </summary>
	public class NewProjectDialog {
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		
		IconView TemplateView;
		FolderEntry entry_location;
		TreeStore catStore;
		
		[Glade.Widget ("NewProjectDialog")] Dialog dialog;
		[Glade.Widget] Button btn_close, btn_new;
		
		[Glade.Widget] Label lbl_hdr_template, lbl_hdr_location;
		[Glade.Widget] Label lbl_name, lbl_location, lbl_subdirectory;
		[Glade.Widget] Label lbl_will_save_in;
		[Glade.Widget] Label lbl_template_descr;
		
		[Glade.Widget] Gtk.Entry txt_name, txt_subdirectory;
		[Glade.Widget] CheckButton chk_combine_directory;
		
		[Glade.Widget] Gtk.TreeView lst_template_types;
		[Glade.Widget] HBox hbox_template, hbox_for_browser;
		
		ResourceService     resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		FileUtilityService  fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		PropertyService     propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		IconService         iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
		MessageService      messageService = (MessageService)ServiceManager.Services.GetService(typeof(MessageService));
		bool openCombine;
		
		public NewProjectDialog (bool openCombine)
		{
			this.openCombine = openCombine;
			new Glade.XML (null, "Base.glade", "NewProjectDialog", null).Autoconnect (this);
			dialog.TransientFor = (Window) WorkbenchSingleton.Workbench;
			
			InitializeComponents();
			InitializeTemplates();
			InitializeView();
			
			TreeIter first;
			if (catStore.GetIterFirst (out first))
				lst_template_types.Selection.SelectIter (first);
			
			dialog.ShowAll ();
		}
		
		void InitializeView()
		{
			PixbufList imglist = new PixbufList();
			
			imglist.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			
			foreach (DictionaryEntry entry in icons) {
				Gdk.Pixbuf bitmap = iconService.GetBitmap(entry.Key.ToString());
				if (bitmap != null) {
					imglist.Add(bitmap);
					tmp[entry.Key] = ++i;
				} else {
					Console.WriteLine("can't load bitmap " + entry.Key.ToString() + " using default");
				}
			}
		
			// set the correct imageindex for all templates
			icons = tmp;
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
				//	item.ImageIndex = 0;
				} else {
				//	item.ImageIndex = (int)icons[item.Template.Icon];
				}
			}
			
			
			InsertCategories (TreeIter.Zero, categories);
			/*for (int j = 0; j < categories.Count; ++j) {
				if (((Category)categories[j]).Name == propertyService.GetProperty("Dialogs.NewProjectDialog.LastSelectedCategory", "C#")) {
					((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode = (TreeNode)((TreeView)ControlDictionary["categoryTreeView"]).Nodes[j];
					break;
				}
			}*/
		}
		
		void InsertCategories (TreeIter node, ArrayList catarray)
		{
			foreach (Category cat in catarray) {
				TreeIter i;
				if (node.Equals (TreeIter.Zero)) {
					i = catStore.AppendValues (cat.Name, cat);
				} else {
					i = catStore.AppendValues (node, cat.Name, cat);
				}
				InsertCategories(i, cat.Categories);
			}
		}
		
		// TODO : insert sub categories
		Category GetCategory(string categoryname)
		{
			foreach (Category category in categories) {
				if (category.Name == categoryname)
					return category;
			}
			Category newcategory = new Category(categoryname);
			categories.Add(newcategory);
			return newcategory;
		}
		
		void InitializeTemplates()
		{
			foreach (ProjectTemplate template in ProjectTemplate.ProjectTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null)
					icons[titem.Template.Icon] = 0; // "create template icon"
				Category cat = GetCategory(titem.Template.Category);
				cat.Templates.Add(titem);
				//if (cat.Templates.Count == 1)
				//	titem.Selected = true;
				alltemplates.Add(titem);
			}
		}
		
		void CategoryChange(object sender, EventArgs e)
		{
			TreeModel mdl;
			TreeIter  iter;
			if (lst_template_types.Selection.GetSelected (out mdl, out iter)) {
				TemplateView.Clear ();
				foreach (TemplateItem item in ((Category)catStore.GetValue (iter, 1)).Templates) {
					TemplateView.AddIcon (new Gtk.Image (iconService.GetBitmap (item.Template.Icon)), item.Name, item.Template);
				}
				btn_new.Sensitive = false;
			}
		}

		string ProjectSolution {
			get {
				string subdir = txt_subdirectory.Text.Trim ();
				if (subdir != "")
					return Path.Combine (ProjectLocation, subdir);
				
				return ProjectLocation;
			}
		}
		
		string ProjectLocation {
			get {
				if (chk_combine_directory.Active)
					return Path.Combine (entry_location.Path, txt_name.Text);
				
				return entry_location.Path;
			}
		}
		
		// TODO : Format the text
		void PathChanged (object sender, EventArgs e)
		{
			ActivateIfReady ();
			lbl_will_save_in.Text = resourceService.GetString("Dialog.NewProject.ProjectAtDescription") + " " + ProjectSolution;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(IProject project, string filename, string content, bool showFile)
		{
			project.ProjectFiles.Add (new ProjectFile(filename));
			
			StreamWriter sr = File.CreateText (filename);
			sr.Write(stringParserService.Parse(content, new string[,] { {"PROJECT", txt_name.Text}, {"FILE", System.IO.Path.GetFileName(filename)}}));
			sr.Close();
			
			if (showFile) {
				string longfilename = fileUtilityService.GetDirectoryNameWithSeparator (ProjectSolution) + stringParserService.Parse(filename, new string[,] { {"PROJECT", txt_name.Text}});
				IFileService fileService = (IFileService) ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile (longfilename);
			}
		}
		
		public string NewProjectLocation;
		public string NewCombineLocation;
		
		void OpenEvent(object sender, EventArgs e)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (TemplateView.CurrentlySelected != null) {
				propertyService.SetProperty("Dialogs.NewProjectDialog.LastSelectedCategory", ((ProjectTemplate)TemplateView.CurrentlySelected).Name);
				//propertyService.SetProperty("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
			}
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string solution = txt_subdirectory.Text;
			string name     = txt_name.Text;
			string location = entry_location.Path;
			if ((solution != null && solution.Trim () != "" 
				&& (!fileUtilityService.IsValidFileName (solution) || solution.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0)) ||
			    !fileUtilityService.IsValidFileName(name)     || name.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0 ||
			    !fileUtilityService.IsValidFileName(location)) {
				messageService.ShowError("Illegal project name.\nOnly use letters, digits, space, '.' or '_'.");
				return;
			}
			
			propertyService.SetProperty (
				"ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.AutoCreateProjectSubdir",
				chk_combine_directory.Active);
			
			if (TemplateView.CurrentlySelected != null && name.Length != 0) {
					ProjectTemplate item = (ProjectTemplate) TemplateView.CurrentlySelected;
					
					System.IO.Directory.CreateDirectory (ProjectSolution);
					
					
					ProjectCreateInformation cinfo = new ProjectCreateInformation ();
					
					cinfo.CombinePath     = ProjectLocation;
					cinfo.ProjectBasePath = ProjectSolution;
//					cinfo.Description     = stringParserService.Parse(item.Template.Description);
					
					cinfo.ProjectName     = name;
//					cinfo.ProjectTemplate = item.Template;
					
					NewCombineLocation = item.CreateProject (cinfo);
					if (NewCombineLocation == null || NewCombineLocation.Length == 0)
						return;
					
					if (openCombine)
						item.OpenCreatedCombine();
					
					// TODO :: THIS DOESN'T WORK !!!
					NewProjectLocation = System.IO.Path.ChangeExtension(NewCombineLocation, ".prjx");
					
					//DialogResult = DialogResult.OK;
					dialog.Hide ();

#if false // from .98
					if (item.Template.LanguageName != null && item.Template.LanguageName.Length > 0)  {
						
					}
					
					if (item.Template.WizardPath != null) {
						IProperties customizer = new DefaultProperties();
						customizer.SetProperty("Template", item.Template);
						customizer.SetProperty("Creator",  this);
						WizardDialog wizard = new WizardDialog("Project Wizard", customizer, item.Template.WizardPath);
						if (wizard.ShowDialog() == DialogResult.OK) {
							DialogResult = DialogResult.OK;
						}
					}
					
					NewCombineLocation = fileUtilityService.GetDirectoryNameWithSeparator(ProjectLocation) + ((TextBox)ControlDictionary["nameTextBox"]).Text + ".cmbx";
					
					if (File.Exists(NewCombineLocation)) {
						DialogResult result = MessageBox.Show("Combine file " + NewCombineLocation + " already exists, do you want to overwrite\nthe existing file ?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
						switch(result) {
							case DialogResult.Yes:
								cmb.SaveCombine(NewCombineLocation);
								break;
							case DialogResult.No:
								break;
						}
					} else {
						cmb.SaveCombine(NewCombineLocation);
					}
				} else {
					MessageBox.Show(resourceService.GetString("Dialog.NewProject.EmptyProjectFieldWarning"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
#endif
			}
		}
		
		// icon view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (TemplateView.CurrentlySelected != null)
				lbl_template_descr.Text = stringParserService.Parse (((ProjectTemplate)TemplateView.CurrentlySelected).Description);
			else
				lbl_template_descr.Text = String.Empty;
			
			ActivateIfReady ();
		}
		
		void cancelClicked (object o, EventArgs e)
		{
			dialog.Hide ();
		}
		
		public int Run ()
		{
			return dialog.Run ();
		}
		
		void ActivateIfReady ()
		{
			if (TemplateView.CurrentlySelected == null || txt_name.Text.Trim () == "")
				btn_new.Sensitive = false;
			else
				btn_new.Sensitive = true;
		}
		
		void InitializeComponents()
		{
		
			catStore = new Gtk.TreeStore (typeof (string), typeof (Category));
			lst_template_types.Model = catStore;
			lst_template_types.WidthRequest = 160;
			
			lst_template_types.Selection.Changed += new EventHandler (CategoryChange);
			
			TemplateView = new IconView();

			TreeViewColumn catColumn = new TreeViewColumn ();
			catColumn.Title = "categories";
			
			CellRendererText cat_text_render = new CellRendererText ();
			catColumn.PackStart (cat_text_render, true);
			catColumn.AddAttribute (cat_text_render, "text", 0);

			lst_template_types.AppendColumn (catColumn);

			TemplateView = new IconView ();
			hbox_template.PackStart (TemplateView, true, true, 0);

			//string label = stringParserService.Parse ("${res:Dialog.NewProject.NewSolutionLabelText}");
			//label = stringParserService.Parse ("${res:Dialog.NewProject.LocationLabelText}");
			//label = stringParserService.Parse ("${res:Dialog.NewProject.NameLabelText}");
			//label = stringParserService.Parse ("${res:Dialog.NewProject.checkBox1Text}");
			//label = stringParserService.Parse ("${res:Dialog.NewProject.autoCreateSubDirCheckBox}");
			entry_location = new FolderEntry ("Combine Location");
			hbox_for_browser.PackStart (entry_location, true, true, 0);
			
			
			entry_location.DefaultPath = propertyService.GetProperty ("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", fileUtilityService.GetDirectoryNameWithSeparator (Environment.GetFolderPath (Environment.SpecialFolder.Personal)) + "MonoDevelopProjects").ToString ();
			
			PathChanged (null, null);
			
			TemplateView.IconSelected += new EventHandler(SelectedIndexChange);
			TemplateView.IconDoubleClicked += new EventHandler(OpenEvent);
			entry_location.PathChanged += new EventHandler (PathChanged);
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		internal class Category
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			string name;
			
			public Category(string name)
			{
				this.name = name;
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
		/// Holds a new file template
		/// </summary>
		internal class TemplateItem
		{
			ProjectTemplate template;
			string name;
			
			public TemplateItem(ProjectTemplate template)
			{
				name = ((StringParserService)ServiceManager.Services.GetService(typeof(StringParserService))).Parse(template.Name);
				this.template = template;
			}
			
			public string Name {
				get { return name; }
			}
			
			public ProjectTemplate Template {
				get {
					return template;
				}
			}
		}
	}
}
