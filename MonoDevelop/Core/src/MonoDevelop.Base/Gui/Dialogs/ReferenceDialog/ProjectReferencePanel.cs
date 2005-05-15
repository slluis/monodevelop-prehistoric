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
			
			store = new TreeStore (typeof (string), typeof (string), typeof(Project), typeof(bool), typeof(Gdk.Pixbuf));
			treeView = new TreeView (store);
			
			TreeViewColumn firstColumn = new TreeViewColumn ();
			
			firstColumn.Title = GettextCatalog.GetString ("Project");
			CellRendererToggle tog_render = new CellRendererToggle ();
			tog_render.Xalign = 0;
			tog_render.Toggled += new Gtk.ToggledHandler (AddReference);
			firstColumn.PackStart (tog_render, false);
			firstColumn.AddAttribute (tog_render, "active", 3);

			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			firstColumn.PackStart (pix_render, false);
			firstColumn.AddAttribute (pix_render, "pixbuf", 4);
			
			CellRendererText text_render = new CellRendererText ();
			firstColumn.PackStart (text_render, true);
			firstColumn.AddAttribute (text_render, "text", 0);
			
			treeView.AppendColumn (firstColumn);
			treeView.AppendColumn (GettextCatalog.GetString ("Directory"), new CellRendererText (), "text", 1);
			
			PopulateListView ();
			ScrolledWindow sc = new ScrolledWindow ();
			sc.ShadowType = Gtk.ShadowType.In;
			sc.Add (treeView);
			PackStart (sc, true, true, 0);
			ShowAll ();
			BorderWidth = 6;
		}
		
		public void AddReference(object sender, Gtk.ToggledArgs e)
		{
			Gtk.TreeIter iter;
			store.GetIterFromString (out iter, e.Path);
			Project project = (Project) store.GetValue (iter, 2);
			
			if ((bool)store.GetValue (iter, 3) == false) {
				store.SetValue (iter, 3, true);
				selectDialog.AddReference(ReferenceType.Project,
							  project.Name,
							  project.GetOutputFileName());
				
			} else {
				store.SetValue (iter, 3, false);
				selectDialog.RemoveReference(ReferenceType.Project,
							  project.Name,
							  project.GetOutputFileName());
			}
		}
		
		public void SignalRefChange (string refLoc, bool newstate)
		{
			Gtk.TreeIter looping_iter;
			store.GetIterFirst (out looping_iter);
			do {
				Project project = (Project) store.GetValue (looping_iter, 2);
				if (project.Name == refLoc) {
					store.SetValue (looping_iter, 3, newstate);
					return;
				}
			} while (store.IterNext (ref looping_iter));
		}
		
		void PopulateListView ()
		{
			Combine openCombine = Runtime.ProjectService.CurrentOpenCombine;
			
			if (openCombine == null) {
				return;
			}
			
			foreach (Project projectEntry in openCombine.GetAllProjects()) {
				string iconName = Runtime.Gui.Icons.GetImageForProjectType (projectEntry.ProjectType);
				Gdk.Pixbuf icon = treeView.RenderIcon (iconName, Gtk.IconSize.Menu, "");
				store.AppendValues (projectEntry.Name, projectEntry.BaseDirectory, projectEntry, false, icon);
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
