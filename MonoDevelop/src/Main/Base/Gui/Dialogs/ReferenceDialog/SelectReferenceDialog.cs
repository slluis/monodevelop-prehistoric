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
		[Glade.Widget] Gtk.Button    RemoveReferencesButton;
		[Glade.Widget] Gtk.Notebook  mainBook;
		GacReferencePanel gacRefPanel;

		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		IProject configureProject;
		
		public ArrayList ReferenceInformations {
			get {
				ArrayList referenceInformations = new ArrayList();
				Gtk.TreeIter looping_iter;
				if (refTreeStore.GetIterFirst (out looping_iter) == false) {
					return referenceInformations;
				}
				do {
					//Debug.Assert(item.Tag != null);
					referenceInformations.Add(refTreeStore.GetValue(looping_iter, 3));
				} while (refTreeStore.IterNext (out looping_iter));
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
		
		public SelectReferenceDialog(IProject configureProject)
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
			
			foreach (ProjectReference refInfo in configureProject.ProjectReferences) {
				switch (refInfo.ReferenceType) {
					case ReferenceType.Assembly:
					case ReferenceType.Project:
						AddNonGacReference (refInfo);
						break;
					case ReferenceType.Gac:
						AddGacReference (refInfo);
						break;
				}
			}
			mainBook.RemovePage (mainBook.CurrentPage);
			mainBook.AppendPage (gacRefPanel, new Gtk.Label (GettextCatalog.GetString ("System Assemblies")));
			mainBook.AppendPage (new ProjectReferencePanel (this), new Gtk.Label (GettextCatalog.GetString ("Projects")));
			// FIXME il8n the assembly tab name			
			mainBook.AppendPage (new AssemblyReferencePanel (this), new Gtk.Label (GettextCatalog.GetString (".Net Assembly")));
			//comTabPage.Controls.Add(new COMReferencePanel(this));
			AddReferenceDialog.ShowAll ();
		}

		void AddNonGacReference (ProjectReference refInfo)
		{
			refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), refInfo.ReferenceType.ToString (), System.IO.Path.GetFullPath (refInfo.GetReferencedFileName (configureProject)), refInfo);
		}

		void AddGacReference (ProjectReference refInfo)
		{
			gacRefPanel.SignalRefChange (System.IO.Path.GetFullPath (refInfo.GetReferencedFileName (configureProject)), true);
			refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), refInfo.ReferenceType.ToString (), System.IO.Path.GetFullPath (refInfo.GetReferencedFileName (configureProject)), refInfo);
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
			} while (refTreeStore.IterNext (out looping_iter));
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
			} while (refTreeStore.IterNext (out looping_iter));
			
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
			refTreeStore.AppendValues (referenceName, referenceType.ToString (), referenceLocation, tag);
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
				}
				refTreeStore.Remove (ref iter);
			}
		}
	}
}
