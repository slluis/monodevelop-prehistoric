// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;

using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;

using MonoDevelop.Gui.Widgets;
using Gtk;
using Glade;

namespace MonoDevelop.Gui.Dialogs {
	/// <summary>
	/// This class displays a new project dialog and sets up and creates a a new project,
	/// the project types are described in an XML options file
	/// </summary>
	public class NewProjectDialog {
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		
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
		
		FileUtilityService  fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
		PropertyService     propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
		MessageService      messageService = (MessageService)ServiceManager.GetService(typeof(MessageService));
		DispatchService     dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
		bool openCombine;
		
		public NewProjectDialog (bool openCombine)
		{
			this.openCombine = openCombine;
			new Glade.XML (null, "Base.glade", "NewProjectDialog", null).Autoconnect (this);
			dialog.TransientFor = (Window) WorkbenchSingleton.Workbench;			

			dispatcher.BackgroundDispatch (new MessageHandler (InitializeTemplates));
		}
		
		void InitializeView()
		{
			InsertCategories (TreeIter.Zero, categories);
			/*for (int j = 0; j < categories.Count; ++j) {
				if (((Category)categories[j]).Name == propertyService.GetProperty("Dialogs.NewProjectDialog.LastSelectedCategory", "C#")) {
					((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode = (TreeNode)((TreeView)ControlDictionary["categoryTreeView"]).Nodes[j];
					break;
				}
			}*/
			catStore.SetSortColumnId (0, SortType.Ascending);
			TreeIter first;
			if (catStore.GetIterFirst (out first))
				lst_template_types.Selection.SelectIter (first);
			dialog.ShowAll ();
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
				Category cat = GetCategory(titem.Template.Category);
				cat.Templates.Add(titem);
				//if (cat.Templates.Count == 1)
				//	titem.Selected = true;
				alltemplates.Add(titem);
			}
			dispatcher.GuiDispatch (new MessageHandler (InitializeComponents));
		}
		
		void CategoryChange(object sender, EventArgs e)
		{
			TreeModel mdl;
			TreeIter  iter;
			if (lst_template_types.Selection.GetSelected (out mdl, out iter)) {
				TemplateView.Clear ();
				foreach (TemplateItem item in ((Category)catStore.GetValue (iter, 1)).Templates) {
					TemplateView.AddIcon (ResourceService.GetStockId (item.Template.Icon), Gtk.IconSize.Dnd, item.Name, item.Template);
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
			lbl_will_save_in.Text = GettextCatalog.GetString("Project will be saved at") + " " + ProjectSolution;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(IProject project, string filename, string content, bool showFile)
		{
			project.ProjectFiles.Add (new ProjectFile(filename));
			
			StreamWriter sr = System.IO.File.CreateText (filename);
			sr.Write(stringParserService.Parse(content, new string[,] { {"PROJECT", txt_name.Text}, {"FILE", System.IO.Path.GetFileName(filename)}}));
			sr.Close();
			
			if (showFile) {
				string longfilename = fileUtilityService.GetDirectoryNameWithSeparator (ProjectSolution) + stringParserService.Parse(filename, new string[,] { {"PROJECT", txt_name.Text}});
				IFileService fileService = (IFileService) MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
				fileService.OpenFile (longfilename);
			}
		}
		
		public string NewProjectLocation;
		public string NewCombineLocation;
		
		void OpenEvent(object sender, EventArgs e)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
			if (TemplateView.CurrentlySelected != null) {
				propertyService.SetProperty("Dialogs.NewProjectDialog.LastSelectedCategory", ((ProjectTemplate)TemplateView.CurrentlySelected).Name);
				//propertyService.SetProperty("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
			}
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
			string solution = txt_subdirectory.Text;
			string name     = txt_name.Text;
			string location = entry_location.Path;

			IProjectService projService = (IProjectService)ServiceManager.GetService(typeof(IProjectService));			
						
			if(solution.Equals("")) solution = name; //This was empty when adding after first combine
			
			//The one below seemed to be failing sometimes.
			if(solution.IndexOfAny("$#@!%^&*/?\\|'\";:}{".ToCharArray()) > -1) {
				messageService.ShowError(GettextCatalog.GetString ("Illegal project name. \nOnly use letters, digits, space, '.' or '_'."));
				dialog.Respond(Gtk.ResponseType.Reject);
				dialog.Hide();
				return;
			}

			if ((solution != null && solution.Trim () != "" 
				&& (!fileUtilityService.IsValidFileName (solution) || solution.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0)) ||
			    !fileUtilityService.IsValidFileName(name)     || name.IndexOf(System.IO.Path.DirectorySeparatorChar) >= 0 ||
			    !fileUtilityService.IsValidFileName(location)) {
				messageService.ShowError(GettextCatalog.GetString ("Illegal project name.\nOnly use letters, digits, space, '.' or '_'."));
				return;
			}

			if(projService.ExistsEntryWithName(name)) {
				messageService.ShowError(GettextCatalog.GetString ("A Project with that name is already in your Project Space"));
				dialog.Respond(Gtk.ResponseType.Reject);
				dialog.Hide();
				return;
			}
			
			propertyService.SetProperty (
				"MonoDevelop.Gui.Dialogs.NewProjectDialog.AutoCreateProjectSubdir",
				chk_combine_directory.Active);
			
			if (TemplateView.CurrentlySelected != null && name.Length != 0) {
				ProjectTemplate item = (ProjectTemplate) TemplateView.CurrentlySelected;
				
				try
				{
					System.IO.Directory.CreateDirectory (ProjectSolution);
				}
				catch (IOException ioException)
				{
					messageService.ShowError (String.Format (GettextCatalog.GetString ("Could not create file {0}. File already exists."), ProjectSolution));
				}
				catch (UnauthorizedAccessException accessException)
				{
					messageService.ShowError (String.Format (GettextCatalog.GetString ("You do not have permission to create to {0}"), ProjectSolution));
					return;
				}
				
				ProjectCreateInformation cinfo = new ProjectCreateInformation ();
				
				cinfo.CombinePath     = ProjectLocation;
				cinfo.ProjectBasePath = ProjectSolution;
//				cinfo.Description     = stringParserService.Parse(item.Template.Description);
				
				cinfo.ProjectName     = name;
//				cinfo.ProjectTemplate = item.Template;
				
				NewCombineLocation = item.CreateProject (cinfo);
				if (NewCombineLocation == null || NewCombineLocation.Length == 0)
					return;
				
				if (openCombine)
					item.OpenCreatedCombine();
				
				// TODO :: THIS DOESN'T WORK !!!
				NewProjectLocation = System.IO.Path.ChangeExtension(NewCombineLocation, ".prjx");
				
				//DialogResult = DialogResult.OK;
				if (OnOked != null)
					OnOked (null, null);
				dialog.Destroy ();
			}
		}

		public event EventHandler OnOked;
		
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
			dialog.Destroy ();
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

			entry_location = new FolderEntry (GettextCatalog.GetString ("Combine Location"));
			hbox_for_browser.PackStart (entry_location, true, true, 0);
			
			
			entry_location.DefaultPath = propertyService.GetProperty ("MonoDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", fileUtilityService.GetDirectoryNameWithSeparator (Environment.GetEnvironmentVariable ("HOME")) + "MonoDevelopProjects").ToString ();
			
			PathChanged (null, null);
			
			TemplateView.IconSelected += new EventHandler(SelectedIndexChange);
			TemplateView.IconDoubleClicked += new EventHandler(OpenEvent);
			entry_location.PathChanged += new EventHandler (PathChanged);
			InitializeView ();
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
				name = ((StringParserService)ServiceManager.GetService(typeof(StringParserService))).Parse(template.Name);
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
