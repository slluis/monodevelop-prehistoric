// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	
	public class ProjectReferencePanel : VBox, IReferencePanel {
		SelectReferenceDialog selectDialog;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));

		TreeStore store;
		TreeView  treeView;
		
		public ProjectReferencePanel (SelectReferenceDialog selectDialog) : base (false, 6)
		{
			this.selectDialog = selectDialog;
			
			store = new TreeStore (typeof (string), typeof (string), typeof(IProject));
			treeView = new TreeView (store);
			treeView.AppendColumn (resourceService.GetString ("Dialog.SelectReferenceDialog.ProjectReferencePanel.NameHeader"), new CellRendererText (), "text", 0);
			treeView.AppendColumn (resourceService.GetString ("Dialog.SelectReferenceDialog.ProjectReferencePanel.DirectoryHeader"), new CellRendererText (), "text", 1);
			
			
			PopulateListView ();
			ScrolledWindow sc = new ScrolledWindow ();
			sc.AddWithViewport (treeView);
			PackStart (sc, true, true, 0);
			
			Button b = new Button (Stock.Add);
			b.Clicked += new EventHandler (AddReference);
			
			PackEnd (b, false, false, 0);
		}
		
		void AddReference (TreeModel model, TreePath path, TreeIter iter)
		{
			IProject project = (IProject) model.GetValue (iter, 2);
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			
			selectDialog.AddReference(ReferenceType.Project,
						  project.Name,
						  binding.GetCompiledOutputName(project));
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			treeView.Selection.SelectedForeach (new TreeSelectionForeachFunc (AddReference));
		}	
		
		void PopulateListView ()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			Combine openCombine = projectService.CurrentOpenCombine;
			
			if (openCombine == null) {
				return;
			}
			
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				store.AppendValues (projectEntry.Project.Name, projectEntry.Project.BaseDirectory, projectEntry.Project);
			}
		}
	}
	
/*	public class ProjectReferencePanel : ListView//, IReferencePanel
	{
		SelectReferenceDialog selectDialog;
		
		public ProjectReferencePanel(SelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			ColumnHeader nameHeader = new ColumnHeader();
			nameHeader.Text  = resourceService.GetString("Dialog.SelectReferenceDialog.ProjectReferencePanel.NameHeader");
			nameHeader.Width = 160;
			Columns.Add(nameHeader);
			
			ColumnHeader directoryHeader = new ColumnHeader();
			directoryHeader.Text  = resourceService.GetString("Dialog.SelectReferenceDialog.ProjectReferencePanel.DirectoryHeader");
			directoryHeader.Width = 70;
			Columns.Add(directoryHeader);
			
			View = View.Details;
			Dock = DockStyle.Fill;
			FullRowSelect = true;
			
			ItemActivate += new EventHandler(AddReference);
			PopulateListView();
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			foreach (ListViewItem item in SelectedItems) {
				IProject project = (IProject)item.Tag;
				LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
				ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
				
				selectDialog.AddReference(ReferenceType.Project,
				                          project.Name,
				                          binding.GetCompiledOutputName(project));
			}
		}
		

	}*/
}
