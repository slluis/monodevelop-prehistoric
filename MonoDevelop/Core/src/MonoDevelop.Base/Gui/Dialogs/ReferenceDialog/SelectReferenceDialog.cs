// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using Gtk;

namespace MonoDevelop.Gui.Dialogs
{
	public interface IReferencePanel
	{
	}
	
	public class SelectReferenceDialog
	{

		               Gtk.TreeStore refTreeStore;
		[Glade.Widget] Gtk.Dialog    AddReferenceDialog;
		[Glade.Widget] Gtk.TreeView  ReferencesTreeView;
		[Glade.Widget] Gtk.Button    okbutton;
		[Glade.Widget] Gtk.Button    cancelbutton;
		[Glade.Widget] Gtk.Button    RemoveReferenceButton;
		[Glade.Widget] Gtk.Notebook  mainBook;
		GacReferencePanel gacRefPanel;

		Project configureProject;
		ProjectReferencePanel projectRefPanel;
		
		public ProjectReferenceCollection ReferenceInformations {
			get {
				ProjectReferenceCollection referenceInformations = new ProjectReferenceCollection();
				Gtk.TreeIter looping_iter;
				if (refTreeStore.GetIterFirst (out looping_iter) == false) {
					return referenceInformations;
				}
				do {
					//Debug.Assert(item.Tag != null);
					referenceInformations.Add ((ProjectReference) refTreeStore.GetValue(looping_iter, 3));
				} while (refTreeStore.IterNext (ref looping_iter));
				return referenceInformations;
			}
		}

		public int Run ()
		{
			return AddReferenceDialog.Run ();
		}

		public void Hide ()
		{
			AddReferenceDialog.Hide ();
		}
		
		public SelectReferenceDialog(Project configureProject)
		{
			this.configureProject = configureProject;
			
			Glade.XML refXML = new Glade.XML (null, "Base.glade", "AddReferenceDialog", null);
			refXML.Autoconnect (this);
			
			refTreeStore = new Gtk.TreeStore (typeof (string), typeof(string), typeof(string), typeof(ProjectReference));
			ReferencesTreeView.Model = refTreeStore;

			ReferencesTreeView.AppendColumn (GettextCatalog.GetString("Reference Name"), new CellRendererText (), "text", 0);
			ReferencesTreeView.AppendColumn (GettextCatalog.GetString ("Type"), new CellRendererText (), "text", 1);
			ReferencesTreeView.AppendColumn (GettextCatalog.GetString ("Location"), new CellRendererText (), "text", 2);
			
			gacRefPanel = new GacReferencePanel (this);
			projectRefPanel = new ProjectReferencePanel (this, configureProject);
			
			foreach (ProjectReference refInfo in configureProject.ProjectReferences) {
				switch (refInfo.ReferenceType) {
					case ReferenceType.Assembly:
					case ReferenceType.Project:
						AddNonGacReference (refInfo);
						break;
					case ReferenceType.Gac:
						AddGacReference (refInfo, configureProject);
						break;
				}
			}
			mainBook.RemovePage (mainBook.CurrentPage);
			mainBook.AppendPage (gacRefPanel, new Gtk.Label (GettextCatalog.GetString ("Global Assembly Cache")));
			mainBook.AppendPage (projectRefPanel, new Gtk.Label (GettextCatalog.GetString ("Projects")));
			mainBook.AppendPage (new AssemblyReferencePanel (this), new Gtk.Label (GettextCatalog.GetString (".Net Assembly")));
			//comTabPage.Controls.Add(new COMReferencePanel(this));
			ReferencesTreeView.Selection.Changed += new EventHandler (onChanged);
			AddReferenceDialog.ShowAll ();
			onChanged (null, null);
		}

		void onChanged (object o, EventArgs e)
		{
			if (ReferencesTreeView.Selection.CountSelectedRows () > 0)
				RemoveReferenceButton.Sensitive = true;
			else
				RemoveReferenceButton.Sensitive = false;
		}

		void AddNonGacReference (ProjectReference refInfo)
		{
			refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), refInfo.ReferenceType.ToString (), System.IO.Path.GetFullPath (refInfo.GetReferencedFileName ()), refInfo);
		}

		void AddGacReference (ProjectReference refInfo, Project referencedProject)
		{
			gacRefPanel.SignalRefChange (refInfo.Reference, true);
			projectRefPanel.SignalRefChange (refInfo.Reference, true);
			refTreeStore.AppendValues (System.IO.Path.GetFileNameWithoutExtension (refInfo.GetReferencedFileName ()), refInfo.ReferenceType.ToString (), refInfo.Reference, refInfo);
		}

		public void RemoveReference (ReferenceType referenceType, string referenceName, string referenceLocation)
		{
			Gtk.TreeIter looping_iter;
			refTreeStore.GetIterFirst (out looping_iter);
			do {
				if (referenceLocation == (string)refTreeStore.GetValue (looping_iter, 2)) {
					refTreeStore.Remove (ref looping_iter);
					return;
				}
			} while (refTreeStore.IterNext (ref looping_iter));
		}
		
		public void AddReference(ReferenceType referenceType, string referenceName, string referenceLocation)
		{
			Gtk.TreeIter looping_iter;
			refTreeStore.GetIterFirst (out looping_iter);
			do {
				try {
					if (referenceLocation == (string)refTreeStore.GetValue (looping_iter, 2) && referenceName == (string)refTreeStore.GetValue (looping_iter, 0)) {
						return;
					}
				} catch {
				}
			} while (refTreeStore.IterNext (ref looping_iter));
			
			ProjectReference tag;
			switch (referenceType) {
				case ReferenceType.Typelib:
					tag = new ProjectReference(referenceType, referenceName + "|" + referenceLocation);
					break;
				case ReferenceType.Project:
					tag = new ProjectReference(referenceType, referenceName);
					break;
				default:
					tag = new ProjectReference(referenceType, referenceLocation);
					break;
					
			}
			Gtk.TreeIter ni = refTreeStore.AppendValues (referenceName, referenceType.ToString (), referenceLocation, tag);
			ReferencesTreeView.ScrollToCell (refTreeStore.GetPath (ni), null, false, 0, 0);
		}
		
		void SelectReference(object sender, EventArgs e)
		{
			//IReferencePanel refPanel = (IReferencePanel)referenceTabControl.SelectedTab.Controls[0];
			//refPanel.AddReference(null, null);
		}
		
		void RemoveReference(object sender, EventArgs e)
		{
			Gtk.TreeIter iter;
			Gtk.TreeModel mdl;
			if (ReferencesTreeView.Selection.GetSelected (out mdl, out iter)) {
				switch (((ProjectReference)refTreeStore.GetValue (iter, 3)).ReferenceType) {
					case ReferenceType.Gac:
						gacRefPanel.SignalRefChange ((string)refTreeStore.GetValue (iter, 2), false);
						break;
					case ReferenceType.Project:
						projectRefPanel.SignalRefChange ((string)refTreeStore.GetValue (iter, 0), false);
						break;
				}
				Gtk.TreeIter newIter = iter;
				if (refTreeStore.IterNext (ref newIter)) {
					ReferencesTreeView.Selection.SelectIter (newIter);
					refTreeStore.Remove (ref iter);
				} else {
					Gtk.TreePath path = refTreeStore.GetPath (iter);
					if (path.Prev ()) {
						ReferencesTreeView.Selection.SelectPath (path);
						refTreeStore.Remove (ref iter);
					} else {
						refTreeStore.Remove (ref iter);
					}
				}
			}
		}
	}
}
