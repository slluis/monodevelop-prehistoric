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
using MonoDevelop.Internal.Project;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using Gtk;

namespace MonoDevelop.Gui.Dialogs {
	
	public class ProjectReferencePanel : VBox, IReferencePanel {
		SelectReferenceDialog selectDialog;

		TreeStore store;
		TreeView  treeView;
		
		public ProjectReferencePanel (SelectReferenceDialog selectDialog) : base (false, 6)
		{
			this.selectDialog = selectDialog;
			
			store = new TreeStore (typeof (string), typeof (string), typeof(Project));
			treeView = new TreeView (store);
			treeView.AppendColumn (GettextCatalog.GetString ("Project Name"), new CellRendererText (), "text", 0);
			treeView.AppendColumn (GettextCatalog.GetString ("Project Directory"), new CellRendererText (), "text", 1);
			
			
			PopulateListView ();
			ScrolledWindow sc = new ScrolledWindow ();
			sc.AddWithViewport (treeView);
			PackStart (sc, true, true, 0);
			
			Button b = new Button (Gtk.Stock.Add);
			b.Clicked += new EventHandler (AddReference);
			
			PackEnd (b, false, false, 0);
			ShowAll ();
		}
		
		void AddReference (TreeModel model, TreePath path, TreeIter iter)
		{
			Project project = (Project) model.GetValue (iter, 2);
			selectDialog.AddReference(ReferenceType.Project,
						  project.Name,
						  project.GetOutputFileName());
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			treeView.Selection.SelectedForeach (new TreeSelectionForeachFunc (AddReference));
		}	
		
		void PopulateListView ()
		{
			Combine openCombine = Runtime.ProjectService.CurrentOpenCombine;
			
			if (openCombine == null) {
				return;
			}
			
			foreach (Project projectEntry in openCombine.GetAllProjects()) {
				store.AppendValues (projectEntry.Name, projectEntry.BaseDirectory, projectEntry);
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
				Project project = (Project)item.Tag;
				selectDialog.AddReference(ReferenceType.Project,
				                          project.Name,
				                          project.GetOutputFileName());
			}
		}
		

	}*/
}
