// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Windows.Forms;
using System.Reflection;

using ICSharpCode.Core.AddIns.Codons;

namespace NewClassWizard {
		
	public class InterfacesPanel : MyPanelBase {
		
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Button btnAddInterface;
		private System.Windows.Forms.Button btnRemoveInterface;
		private System.Windows.Forms.ListBox lstInterfaces;
		private NamespaceTreeView treeInterfaces;
		
		public InterfacesPanel() {
			InitializeComponent();
		}	
		
		private void refreshFields()
		{
			lstInterfaces.Items.Clear();

			foreach ( Type t in newClassInfo.ImplementedInterfaces.GetInterfaces() )
			{
				lstInterfaces.Items.Add( t );
			}

			if ( lstInterfaces.Items.Count == 0 )
			{
				btnRemoveInterface.Enabled = false;
			}
			else
			{
				lstInterfaces.SelectedIndex = 0;
				btnRemoveInterface.Enabled = true;
			}

		}
		
		private void showSelectedProjectAssemblies() {
			treeInterfaces.Nodes.Clear();
			
			foreach ( Assembly assembly in SelectedProject.GetAssemblies() ) {
				treeInterfaces.ShowAssembly( assembly );
			}
			
		}		
		
		private void Panel_Load(object sender, System.EventArgs e)
		{
			showSelectedProjectAssemblies();
		}			
		
		
		private void btnAddInterface_Click(object sender, System.EventArgs e)
		{
			if ( treeInterfaces.SelectedType != null )
			{
				newClassInfo.ImplementedInterfaces.Add( treeInterfaces.SelectedType );
				refreshFields();
			}
		}

		private void btnRemoveInterface_Click(object sender, System.EventArgs e)
		{
			if ( lstInterfaces.SelectedItem != null )
			{
				newClassInfo.ImplementedInterfaces.Remove( (Type)lstInterfaces.SelectedItem );
				refreshFields();
			}
		}
		
		private void treeInterfaces_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			btnAddInterface.Enabled = ( treeInterfaces.SelectedType != null );
		}		
		
		private void InitializeComponent(){
			
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.lstInterfaces = new System.Windows.Forms.ListBox();
			this.btnRemoveInterface = new System.Windows.Forms.Button();
			this.btnAddInterface = new System.Windows.Forms.Button();			
			this.treeInterfaces = new NewClassWizard.NamespaceTreeView();
			
			this.SuspendLayout();		
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(240, 80);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(168, 16);
			this.label15.TabIndex = 5;
			this.label15.Text = "Implemented Interfaces:";
			// 
			// lstInterfaces
			// 
			this.lstInterfaces.ItemHeight = 16;
			this.lstInterfaces.Location = new System.Drawing.Point(240, 96);
			this.lstInterfaces.Name = "lstInterfaces";
			this.lstInterfaces.Size = new System.Drawing.Size(200, 180);
			this.lstInterfaces.TabIndex = 4;
			// 
			// btnRemoveInterface
			// 
			this.btnRemoveInterface.Location = new System.Drawing.Point(205, 184);
			this.btnRemoveInterface.Name = "btnRemoveInterface";
			this.btnRemoveInterface.Size = new System.Drawing.Size(30, 24);
			this.btnRemoveInterface.TabIndex = 3;
			this.btnRemoveInterface.Text = "<<";
			this.btnRemoveInterface.Click += new System.EventHandler(this.btnRemoveInterface_Click);
			// 
			// btnAddInterface
			// 
			this.btnAddInterface.Location = new System.Drawing.Point(205, 144);
			this.btnAddInterface.Name = "btnAddInterface";
			this.btnAddInterface.Size = new System.Drawing.Size(30, 24);
			this.btnAddInterface.TabIndex = 2;
			this.btnAddInterface.Text = ">>";
			this.btnAddInterface.Click += new System.EventHandler(this.btnAddInterface_Click);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(5, 7);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(144, 16);
			this.label14.TabIndex = 1;
			this.label14.Text = "Available Interfaces:";			
			// 
			// treeInterfaces
			// 
			this.treeInterfaces.HideSelection = false;
			this.treeInterfaces.HotTracking = true;
			this.treeInterfaces.Location = new System.Drawing.Point(5, 25);
			this.treeInterfaces.Name = "treeInterfaces";
			this.treeInterfaces.PathSeparator = ".";
			this.treeInterfaces.Size = new System.Drawing.Size(195, 335);
			this.treeInterfaces.Sorted = true;
			this.treeInterfaces.TabIndex = 0;
			this.treeInterfaces.ShowAbstractClasses = false;
			this.treeInterfaces.ShowConcreteClasses = false;
			this.treeInterfaces.ShowSealedClasses = false;
			this.treeInterfaces.ShowInterfaces = true;			
			this.treeInterfaces.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeInterfaces_AfterSelect);
			
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																	this.label15,
																	this.lstInterfaces,
																	this.btnRemoveInterface,
																	this.btnAddInterface,
																	this.label14,
																	this.treeInterfaces});

			
			this.Load += new System.EventHandler(this.Panel_Load);
			this.ResumeLayout(false);			
			
		}			
	}
}
