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
using MonoDevelop.Gui;
using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// This class displays a new project dialog and sets up and creates a a new project,
	/// the project types are described in an XML options file
	/// </summary>
	public class NewProjectDialog : Dialog
	{
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		
		PixbufList cat_imglist;
		TreeStore catStore;
		Gtk.TreeView catView;
		IconView TemplateView;
		Button okButton;
		Button cancelButton;
		Button browseButton;
		CheckButton seperatedirButton;
		CheckButton createdirButton;
		Entry nameEntry;
		Entry pathEntry;
		Entry newEntry;
		Label infoLabel;
		Label createInLabel;
		
		ResourceService     resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		FileUtilityService  fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		PropertyService     propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		IconService         iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
		MessageService      messageService = (MessageService)ServiceManager.Services.GetService(typeof(MessageService));
		bool openCombine;
		
		public NewProjectDialog (bool openCombine) : base ("New Project", (Window) WorkbenchSingleton.Workbench, DialogFlags.DestroyWithParent)
		{
			this.BorderWidth = 6;
			this.HasSeparator = false;
			
			this.openCombine = openCombine;
			InitializeComponents();
			
			InitializeTemplates();
			InitializeView();
			
			pathEntry.Text = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", fileUtilityService.GetDirectoryNameWithSeparator(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)) + "MonoDevelopProjects").ToString();
			ShowAll ();
		}
		
		void InitializeView()
		{
			PixbufList smalllist = new PixbufList();
			PixbufList imglist = new PixbufList();
			
			smalllist.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			imglist.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
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
					i = catStore.AppendValues (cat.Name, cat, cat_imglist[1]);
				} else {
					i = catStore.AppendValues (node, cat.Name, cat, cat_imglist[1]);
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

		bool FixCatIcons (TreeModel mdl, TreePath path, TreeIter iter)
		{
			((TreeStore) mdl).SetValue (iter, 2, cat_imglist[1]);
			return false;
		}
		
		void CategoryChange(object sender, EventArgs e)
		{
			TreeModel mdl;
			TreeIter  iter;
			if (catView.Selection.GetSelected (out mdl, out iter)) {
				((TreeStore) mdl).Foreach (new TreeModelForeachFunc (FixCatIcons));
				((TreeStore) mdl).SetValue (iter, 2, cat_imglist[0]);
				TemplateView.Clear ();
				foreach (TemplateItem item in ((Category)catStore.GetValue (iter, 1)).Templates) {
					TemplateView.AddIcon (new Gtk.Image (iconService.GetBitmap (item.Template.Icon)), item.Name, item.Template);
				}
				okButton.Sensitive = false;
			}
		}
		
		void OnBeforeExpand(object sender, EventArgs e)
		{
			//e.Node.ImageIndex = 1;
		}
		
		void OnBeforeCollapse(object sender, EventArgs e)
		{
			//e.Node.ImageIndex = 0;
		}
		
		void CheckedChange(object sender, EventArgs e)
		{
			newEntry.Sensitive = seperatedirButton.Active;
			
			if (!newEntry.Sensitive) { // unchecked created own directory for solution
				NameTextChanged(null, null);    // set the value of the ((TextBox)ControlDictionary["solutionNameTextBox"]) to ((TextBox)ControlDictionary["nameTextBox"])
			}
		}
		
		void NameTextChanged(object sender, EventArgs e)
		{
			if (!seperatedirButton.Active) {
				newEntry.Text = nameEntry.Text;
			}
		}
		
		string ProjectSolution {
			get {
				string name = String.Empty;
				if (seperatedirButton.Active) {
					name += System.IO.Path.DirectorySeparatorChar + newEntry.Text;
				}
				return ProjectLocation + name;
			}
		}
		
		string ProjectLocation {
			get {
				string location = pathEntry.Text.TrimEnd('\\', '/', System.IO.Path.DirectorySeparatorChar);
				string name     = nameEntry.Text;
				return location + (createdirButton.Active ? System.IO.Path.DirectorySeparatorChar + name : "");
			}
		}
		
		// TODO : Format the text
		void PathChanged(object sender, EventArgs e)
		{
			createInLabel.Text = resourceService.GetString("Dialog.NewProject.ProjectAtDescription")+ " " + ProjectSolution;
		}
		
		void IconSizeChange(object sender, EventArgs e)
		{
			//((ListView)ControlDictionary["templateListView"]).View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(IProject project, string filename, string content, bool showFile)
		{
			project.ProjectFiles.Add(new ProjectFile(filename));
			
			StreamWriter sr = File.CreateText(filename);
			sr.Write(stringParserService.Parse(content, new string[,] { {"PROJECT", nameEntry.Text}, {"FILE", System.IO.Path.GetFileName(filename)}}));
			sr.Close();
			
			if (showFile) {
				string longfilename = fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + stringParserService.Parse(filename, new string[,] { {"PROJECT", nameEntry.Text}});
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(longfilename);
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
			string solution = newEntry.Text;
			string name     = nameEntry.Text;
			string location = pathEntry.Text;
			if (!fileUtilityService.IsValidFileName(solution) || solution.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0 ||
			    !fileUtilityService.IsValidFileName(name)     || name.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0 ||
			    !fileUtilityService.IsValidFileName(location)) {
				messageService.ShowError("Illegal project name.\nOnly use letters, digits, space, '.' or '_'.");
				return;
			}
			
			propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.AutoCreateProjectSubdir", createdirButton.Active);
			if (TemplateView.CurrentlySelected != null && pathEntry.Text.Length > 0 && newEntry.Text.Length > 0) {
					ProjectTemplate item = (ProjectTemplate)TemplateView.CurrentlySelected;
					
					System.IO.Directory.CreateDirectory(ProjectSolution);
					
					
					ProjectCreateInformation cinfo = new ProjectCreateInformation();
					
					cinfo.CombinePath     = ProjectLocation;
					cinfo.ProjectBasePath = ProjectSolution;
//					cinfo.Description     = stringParserService.Parse(item.Template.Description);
					
					cinfo.ProjectName     = nameEntry.Text;
//					cinfo.ProjectTemplate = item.Template;
					
					NewCombineLocation = item.CreateProject(cinfo);
					if (NewCombineLocation == null || NewCombineLocation.Length == 0) {
						return;
					}
					if (openCombine) {
						item.OpenCreatedCombine();
					}
					
					// TODO :: THIS DOESN'T WORK !!!
					NewProjectLocation = System.IO.Path.ChangeExtension(NewCombineLocation, ".prjx");
					
					//DialogResult = DialogResult.OK;
					Hide ();
					/*
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
				*/		//THIS IS WHERE THE ORIGINAL COMMENT ENDED -- TODD
			}
		}
		
		void BrowseDirectories(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog("Choose project location");
			int response = fd.Run ();
			fd.Hide ();
			
			if (response == (int) ResponseType.Ok)
			{
				pathEntry.Text = fd.Filename;
			}
		}
		
		// icon view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (TemplateView.CurrentlySelected != null) {
				infoLabel.Text = stringParserService.Parse(((ProjectTemplate)TemplateView.CurrentlySelected).Description);
				okButton.Sensitive = true;
			} else {
				infoLabel.Text = String.Empty;
				okButton.Sensitive = false;
			}
		}
		
		void cancelClicked (object o, EventArgs e)
		{
			Hide ();
		}
		
		void InitializeComponents()
		{
		
			catStore = new Gtk.TreeStore (typeof (string), typeof(Category), typeof (Gdk.Pixbuf));
			ScrolledWindow swindow1 = new ScrolledWindow();
			swindow1.VscrollbarPolicy = PolicyType.Automatic;
			swindow1.HscrollbarPolicy = PolicyType.Automatic;
			swindow1.ShadowType = ShadowType.In;
			catView = new Gtk.TreeView (catStore);
			catView.WidthRequest = 160;
			catView.HeadersVisible = false;
			
			TemplateView = new IconView();

			TreeViewColumn catColumn = new TreeViewColumn ();
			catColumn.Title = "categories";
			CellRendererPixbuf cat_pix_render = new CellRendererPixbuf ();
			catColumn.PackStart (cat_pix_render, false);
			catColumn.AddAttribute (cat_pix_render, "pixbuf", 2);
			
			CellRendererText cat_text_render = new CellRendererText ();
			catColumn.PackStart (cat_text_render, true);
			catColumn.AddAttribute (cat_text_render, "text", 0);

			catView.AppendColumn (catColumn);

			TemplateView = new IconView();

			okButton = new Button (Stock.New);
			okButton.Clicked += new EventHandler (OpenEvent);
			okButton.Sensitive = false;

			cancelButton = new Button (Stock.Close);
			cancelButton.Clicked += new EventHandler (cancelClicked);

			infoLabel = new Gtk.Label ();
			Frame infoLabelFrame = new Frame ();
			infoLabelFrame.Add(infoLabel);
			
			HBox viewbox = new HBox (false, 6);
			swindow1.Add(catView);
			viewbox.PackStart (swindow1,false,true,0);
			viewbox.PackStart(TemplateView, true, true,0);

			this.AddActionWidget (cancelButton, (int)Gtk.ResponseType.Cancel);
			this.AddActionWidget (okButton, (int)Gtk.ResponseType.Ok);

			Table entryTable = new Table (3, 3, false);
			entryTable.RowSpacing = 6;			

			string label = stringParserService.Parse ("${res:Dialog.NewProject.NewSolutionLabelText}");
			entryTable.Attach (new Label (label), 0, 1, 0, 1);
			label = stringParserService.Parse ("${res:Dialog.NewProject.LocationLabelText}");
			entryTable.Attach (new Label (label), 0, 1, 1, 2);
			label = stringParserService.Parse ("${res:Dialog.NewProject.NameLabelText}");
			entryTable.Attach (new Label (label), 0, 1, 2, 3);

			nameEntry = new Entry ();
			entryTable.Attach (nameEntry, 1, 3, 0, 1);

			HBox path_btnBox = new HBox (false, 0);
			
			pathEntry = new Entry ();
			path_btnBox.PackStart (pathEntry);
			browseButton = new Button ("...");
			path_btnBox.PackStart (browseButton, false, false, 6);
			entryTable.Attach (path_btnBox, 1, 3, 1, 2);

			newEntry = new Entry ();
			newEntry.Sensitive = false;
			entryTable.Attach (newEntry, 1, 2, 2, 3);

			VBox checkBox = new VBox (false, 6);
			
			label = stringParserService.Parse ("${res:Dialog.NewProject.checkBox1Text}");
			seperatedirButton = new CheckButton (label);
			checkBox.PackStart (seperatedirButton);

			label = stringParserService.Parse ("${res:Dialog.NewProject.autoCreateSubDirCheckBox}");
			createdirButton = new CheckButton (label);
			checkBox.PackStart (createdirButton);

			entryTable.Attach (checkBox, 2, 3, 2, 3);
			
			createInLabel = new Gtk.Label (ProjectLocation);
			
			this.VBox.PackStart (viewbox);
			this.VBox.PackStart (infoLabelFrame, false, false, 6);
			this.VBox.PackStart (entryTable, false, false, 6);
			this.VBox.PackStart (createInLabel, false, false, 6);

			cat_imglist = new PixbufList();
			cat_imglist.Add(iconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			cat_imglist.Add(iconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
	
			catView.Selection.Changed += new EventHandler (CategoryChange);
			TemplateView.IconSelected += new EventHandler(SelectedIndexChange);
			TemplateView.IconDoubleClicked += new EventHandler(OpenEvent);
			
			newEntry.Changed += new EventHandler (PathChanged);
			nameEntry.Changed += new EventHandler (NameTextChanged);
			nameEntry.Changed += new EventHandler (PathChanged);
			pathEntry.Changed += new EventHandler (PathChanged);
			browseButton.Clicked += new EventHandler (BrowseDirectories);
			seperatedirButton.Toggled += new EventHandler (CheckedChange);
			seperatedirButton.Toggled += new EventHandler (PathChanged);
			createdirButton.Toggled += new EventHandler (PathChanged);			
			this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
//			
//			CheckedChange(this, EventArgs.Empty);
//			IconSizeChange(this, EventArgs.Empty);
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
