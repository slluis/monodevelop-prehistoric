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

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Services;

using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public interface IReferencePanel
	{
	}
	
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	public class SelectReferenceDialog
	{
#if false
		private System.Windows.Forms.ListView referencesListView;
		private System.Windows.Forms.Button selectButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.TabPage gacTabPage;
		private System.Windows.Forms.TabPage projectTabPage;
		private System.Windows.Forms.TabPage browserTabPage;
		TabPage comTabPage = new TabPage();
		private System.Windows.Forms.Label referencesLabel;
		private System.Windows.Forms.ColumnHeader referenceHeader;
		private System.Windows.Forms.ColumnHeader typeHeader;
		private System.Windows.Forms.ColumnHeader locationHeader;
		private System.Windows.Forms.TabControl referenceTabControl;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button helpButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
#endif

		               Gtk.TreeStore refTreeStore;
		[Glade.Widget] Gtk.Dialog    AddReferenceDialog;
		[Glade.Widget] Gtk.TreeView  ReferencesTreeView;
		[Glade.Widget] Gtk.Button    okbutton;
		[Glade.Widget] Gtk.Button    cancelbutton;
		[Glade.Widget] Gtk.Button    RemoveReferencesButton;
		[Glade.Widget] Gtk.Notebook  mainBook;

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
			
			AddReferenceDialog.Title = resourceService.GetString("Dialog.SelectReferenceDialog.DialogName");
			
			refTreeStore = new Gtk.TreeStore (typeof (string), typeof(string), typeof(string), typeof(ProjectReference));
			ReferencesTreeView.Model = refTreeStore;

			ReferencesTreeView.AppendColumn (resourceService.GetString("Dialog.SelectReferenceDialog.ReferenceHeader"), new CellRendererText (), "text", 0);
			ReferencesTreeView.AppendColumn (resourceService.GetString ("Dialog.SelectReferenceDialog.TypeHeader"), new CellRendererText (), "text", 1);
			ReferencesTreeView.AppendColumn (resourceService.GetString ("Dialog.SelectReferenceDialog.LocationHeader"), new CellRendererText (), "text", 2);
			
			
			foreach (ProjectReference refInfo in configureProject.ProjectReferences) {
				switch (refInfo.ReferenceType) {
					case ReferenceType.Assembly:
						refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), refInfo.ReferenceType.ToString (), System.IO.Path.GetFullPath (refInfo.GetReferencedFileName (configureProject)), refInfo);
						break;
					case ReferenceType.Gac:
						refTreeStore.AppendValues (System.IO.Path.GetFileName (refInfo.Reference), refInfo.ReferenceType.ToString (), System.IO.Path.GetFullPath (refInfo.GetReferencedFileName (configureProject)), refInfo);
						break;
				}
			}
			//InitializeComponent();
		
			mainBook.RemovePage (mainBook.CurrentPage);
			mainBook.AppendPage (new GacReferencePanel (this), new Gtk.Label (resourceService.GetString("Dialog.SelectReferenceDialog.GacTabPage")));
			mainBook.AppendPage (new ProjectReferencePanel (this), new Gtk.Label (resourceService.GetString("Dialog.SelectReferenceDialog.ProjectTabPage")));
			//gacTabPage.Controls.Add(new GacReferencePanel(this));
			//projectTabPage.Controls.Add(new ProjectReferencePanel(this));
			//browserTabPage.Controls.Add(new AssemblyReferencePanel(this));
			//comTabPage.Controls.Add(new COMReferencePanel(this));
			AddReferenceDialog.ShowAll ();
		}
		
		public void RemoveReference (ReferenceType referenceType, string referenceName, string referenceLocation)
		{
			Gtk.TreeIter looping_iter;
			refTreeStore.GetIterFirst (out looping_iter);
			do {
				if (referenceLocation == (string)refTreeStore.GetValue (looping_iter, 2) && referenceName == (string)refTreeStore.GetValue (looping_iter, 0)) {
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
			
			foreach (ProjectReference refInfo in configureProject.ProjectReferences) {
				if (refInfo.ReferenceType == referenceType) {
					switch (referenceType) {
						case ReferenceType.Typelib:
						case ReferenceType.Gac:
						case ReferenceType.Assembly:
							if (refInfo.Reference == referenceLocation) {
								return;
							}
							break;
						case ReferenceType.Project:
							if (refInfo.Reference == referenceName) {
								return;
							}
							break;
						default:
							Debug.Assert(false, "Unknown reference type" + referenceType);
							break;
					}
				}
			}
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
			//ArrayList itemsToDelete = new ArrayList();
			
			//foreach (ListViewItem item in referencesListView.SelectedItems) {
			//	itemsToDelete.Add(item);
			//}
			
			//foreach (ListViewItem item in itemsToDelete) {
			//	referencesListView.Items.Remove(item);
			//}
			Gtk.TreeIter iter;
			Gtk.TreeModel mdl;
			if (ReferencesTreeView.Selection.GetSelected (out mdl, out iter)) {
				refTreeStore.Remove (ref iter);
			}
		}

#if false
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.referenceTabControl = new System.Windows.Forms.TabControl();
			this.referencesListView = new System.Windows.Forms.ListView();
			this.selectButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.gacTabPage = new System.Windows.Forms.TabPage();
			this.projectTabPage = new System.Windows.Forms.TabPage();
			this.browserTabPage = new System.Windows.Forms.TabPage();
			this.referencesLabel = new System.Windows.Forms.Label();
			this.referenceHeader = new System.Windows.Forms.ColumnHeader();
			this.typeHeader = new System.Windows.Forms.ColumnHeader();
			this.locationHeader = new System.Windows.Forms.ColumnHeader();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.helpButton = new System.Windows.Forms.Button();
			this.referenceTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// referenceTabControl
			// 
			this.referenceTabControl.Controls.AddRange(new System.Windows.Forms.Control[] {
																							  this.gacTabPage,
																							  this.projectTabPage,
																							  this.browserTabPage,
																							  this.comTabPage
			});
			this.referenceTabControl.Location = new System.Drawing.Point(8, 8);
			this.referenceTabControl.SelectedIndex = 0;
			this.referenceTabControl.Size = new System.Drawing.Size(472, 224);
			this.referenceTabControl.TabIndex = 0;
			// 
			// referencesListView
			// 
			this.referencesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								 this.referenceHeader,
																								 this.typeHeader,
																								 this.locationHeader});
			this.referencesListView.Location = new System.Drawing.Point(8, 256);
			this.referencesListView.Size = new System.Drawing.Size(472, 97);
			this.referencesListView.TabIndex = 3;
			this.referencesListView.View = System.Windows.Forms.View.Details;
			this.referencesListView.FullRowSelect = true;
			
			
			// 
			// selectButton
			// 
			this.selectButton.Location = new System.Drawing.Point(488, 32);
			this.selectButton.TabIndex = 1;
			this.selectButton.Text = resourceService.GetString("Dialog.SelectReferenceDialog.SelectButton");
			this.selectButton.Click += new EventHandler(SelectReference);
			this.selectButton.FlatStyle = FlatStyle.System;
			
			// 
			// removeButton
			// 
			this.removeButton.Location = new System.Drawing.Point(488, 256);
			this.removeButton.TabIndex = 4;
			this.removeButton.Text = resourceService.GetString("Global.RemoveButtonText");
			this.removeButton.Click += new EventHandler(RemoveReference);
			this.removeButton.FlatStyle = FlatStyle.System;
			
			// 
			// gacTabPage
			// 
			this.gacTabPage.Location = new System.Drawing.Point(4, 22);
			this.gacTabPage.Size = new System.Drawing.Size(464, 198);
			this.gacTabPage.TabIndex = 0;
			this.gacTabPage.Text = resourceService.GetString("Dialog.SelectReferenceDialog.GacTabPage");
			// 
			// projectTabPage
			// 
			this.projectTabPage.Location = new System.Drawing.Point(4, 22);
			this.projectTabPage.Size = new System.Drawing.Size(464, 198);
			this.projectTabPage.TabIndex = 1;
			this.projectTabPage.Text = resourceService.GetString("Dialog.SelectReferenceDialog.ProjectTabPage");
			// 
			// browserTabPage
			// 
			this.browserTabPage.Location = new System.Drawing.Point(4, 22);
			this.browserTabPage.Size = new System.Drawing.Size(464, 198);
			this.browserTabPage.TabIndex = 2;
			this.browserTabPage.Text = resourceService.GetString("Dialog.SelectReferenceDialog.BrowserTabPage");
			
			this.comTabPage.Location = new System.Drawing.Point(4, 22);
			this.comTabPage.Size = new System.Drawing.Size(464, 198);
			this.comTabPage.TabIndex = 2;
			this.comTabPage.Text = "COM";
			
			//
			// referencesLabel
			// 
			this.referencesLabel.Location = new System.Drawing.Point(8, 240);
			this.referencesLabel.Size = new System.Drawing.Size(472, 16);
			this.referencesLabel.TabIndex = 2;
			this.referencesLabel.Text = resourceService.GetString("Dialog.SelectReferenceDialog.ReferencesLabel");
			// 
			// referenceHeader
			// 
			this.referenceHeader.Text = resourceService.GetString("Dialog.SelectReferenceDialog.ReferenceHeader");
			this.referenceHeader.Width = 183;
			// 
			// typeHeader
			// 
			this.typeHeader.Text = resourceService.GetString("Dialog.SelectReferenceDialog.TypeHeader");
			this.typeHeader.Width = 57;
			// 
			// locationHeader
			// 
			this.locationHeader.Text = resourceService.GetString("Dialog.SelectReferenceDialog.LocationHeader");
			this.locationHeader.Width = 228;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(312, 368);
			this.okButton.TabIndex = 5;
			this.okButton.Text = resourceService.GetString("Global.OKButtonText");
			this.okButton.FlatStyle = FlatStyle.System;
			
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(400, 368);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			this.cancelButton.FlatStyle = FlatStyle.System;
			
			//
			// helpButton
			// 
			this.helpButton.Location = new System.Drawing.Point(488, 368);
			this.helpButton.TabIndex = 7;
			this.helpButton.Text = resourceService.GetString("Global.HelpButtonText");
			this.helpButton.FlatStyle = FlatStyle.System;
			
			//
			// SelectReferenceDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(570, 399);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.helpButton,
																		  this.cancelButton,
																		  this.okButton,
																		  this.referencesLabel,
																		  this.removeButton,
																		  this.selectButton,
																		  this.referencesListView,
																		  this.referenceTabControl});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.Text = resourceService.GetString("Dialog.SelectReferenceDialog.DialogName");
			this.referenceTabControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
#endif
	}
}
