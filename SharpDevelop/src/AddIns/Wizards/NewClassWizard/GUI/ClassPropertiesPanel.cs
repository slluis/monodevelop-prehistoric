// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace NewClassWizard {
		
	public class ClassPropertiesPanel : MyPanelBase	{

		//GUI member variables
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox chkPublic;
		private System.Windows.Forms.CheckBox chkSealed;
		private System.Windows.Forms.CheckBox chkAbstract;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label lblBaseClass;
		private System.Windows.Forms.TextBox txtClassName;
		private System.Windows.Forms.TextBox txtProject;
		private System.Windows.Forms.ErrorProvider errorProvider1;	
		private NamespaceTreeView treeClasses;
				
		public ClassPropertiesPanel() {
			InitializeComponent();
		}			
		
		public override bool ReceiveDialogMessage(DialogMessage message) {
			
			if (message == DialogMessage.Finish) {
				
				try {
					//get the class implementation graph
					CodeFactory factory = new CodeFactory( CodeOptions );
					factory.AddClass( newClassInfo );
					
					IProject project = SelectedProject.project;
					
					if ( project != null )
						factory.Namespace = project.Name;				
					
					else 
						factory.Namespace = "";
	
					using (StringWriter sw = new StringWriter() ) {
	
						factory.GenerateCode( language, sw );
									
						newFileCreator.SaveFile(fileName,
						                 sw.ToString(),
						                 CodeProviderChooser.StringFromLanguage( language ), 
						                 true);					
					}
				} catch ( Exception e ) {
					MessageBox.Show( e.Message, "Code Generation Error" );
				}
				
			}
			return true;
		}					
		
		private void refreshFields()
		{
			NewClassInfo newClass = base.newClassInfo;
			
			txtClassName.Text = newClass.Name;
			chkAbstract.Checked = newClass.IsAbstract;
			chkSealed.Checked = newClass.IsSealed;
			chkPublic.Checked = newClass.IsPublic;

			lblBaseClass.Text = newClass.BaseType.Namespace + "." + newClass.BaseType.Name;			

		}				
		
		private void showSelectedProjectAssemblies() {
			treeClasses.Nodes.Clear();
			
			foreach ( Assembly assembly in SelectedProject.GetAssemblies() ) {
				treeClasses.ShowAssembly( assembly );
			}
			
		}
		
		private string fileName {
			get {
				return txtClassName.Text + CodeProviderChooser.chooseDefaultFileExt( language );
			}
		}
		
		private bool classNameIsValid()
		{
			string errorMsg = String.Empty;

			if ( CodeProviderChooser.IsValidIdentifier( language, txtClassName.Text ) == false ) {
				errorMsg = "This is not a valid " + CodeProviderChooser.LanguageName( language ) + " class name";			
			}
			else if ( !IsFileNameValid( fileName ) ) {
				errorMsg = "A class with this name already exists in this project";
			}
			errorProvider1.SetError( txtClassName, errorMsg );

			return ( errorMsg.Length == 0 );
		}		
		
		private bool IsFileNameValid( string name ) {
			
			IProject project = SelectedProject.project;
		
			if ( project != null ) {				
				return !project.IsFileInProject(name);
			}
			
			return true;
		}
		
		//
		//Control event handlers
		//
		private void chkPublic_CheckedChanged(object sender, System.EventArgs e)
		{
			newClassInfo.IsPublic = chkPublic.Checked;
		}

		private void chkAbstract_CheckedChanged(object sender, System.EventArgs e)
		{
			newClassInfo.IsAbstract = chkAbstract.Checked;
			refreshFields();
		}

		private void chkSealed_CheckedChanged(object sender, System.EventArgs e)
		{
			newClassInfo.IsSealed = chkSealed.Checked;
			refreshFields();
		}

		private void txtClassName_TextChanged(object sender, System.EventArgs e)
		{
			newClassInfo.Name = txtClassName.Text;
			base.EnableFinish = classNameIsValid();
		}		
	
		private void treeClasses_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			Type t = treeClasses.SelectedType;
			if ( t != null )
			{
				newClassInfo.BaseType = t;
			}
			else
			{
				newClassInfo.BaseType = typeof(object);
			}

			refreshFields();
		}
		
		private void Panel_Load(object sender, System.EventArgs e)
		{
			try {
				refreshFields();	
				if ( SelectedProject.project != null )
					txtProject.Text = SelectedProject.project.Name;
				showSelectedProjectAssemblies();
				
			} catch ( UnknownLanguageException er ) {
				base.EnableFinish = false;
				foreach ( Control c in Controls ) {
					c.Enabled = false;
				}
				MessageBox.Show( er.Message, "Unsupported Language" );
			}
		}	
		
		private void InitializeComponent(){
			
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.chkAbstract = new System.Windows.Forms.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.lblBaseClass = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
			this.txtClassName = new System.Windows.Forms.TextBox();
			this.chkPublic = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.chkSealed = new System.Windows.Forms.CheckBox();
			this.txtProject = new System.Windows.Forms.TextBox();
			this.treeClasses = new NewClassWizard.NamespaceTreeView();
			
			this.SuspendLayout();		
			
			//
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(250, 302);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(96, 16);
			this.label10.TabIndex = 2;
			this.label10.Text = "Class Name:";			
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(250, 222);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(120, 16);
			this.label11.TabIndex = 3;
			this.label11.Text = "Base Class:";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(5, 7);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(152, 16);
			this.label13.TabIndex = 10;
			this.label13.Text = "Available Base Classes:";		
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(250, 48);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(120, 16);
			this.label12.TabIndex = 11;
			this.label12.Text = "Add to Project:";			
			// 
			// chkAbstract
			// 
			this.chkAbstract.Location = new System.Drawing.Point(250, 158);
			this.chkAbstract.Name = "chkAbstract";
			this.chkAbstract.Size = new System.Drawing.Size(144, 24);
			this.chkAbstract.TabIndex = 7;
			this.chkAbstract.Text = "Abstract";
			this.chkAbstract.CheckedChanged += new System.EventHandler(this.chkAbstract_CheckedChanged);
			// 
			// chkPublic
			// 
			this.chkPublic.Location = new System.Drawing.Point(250, 194);
			this.chkPublic.Name = "chkPublic";
			this.chkPublic.Size = new System.Drawing.Size(144, 24);
			this.chkPublic.TabIndex = 5;
			this.chkPublic.Text = "Public";
			this.chkPublic.CheckedChanged += new System.EventHandler(this.chkPublic_CheckedChanged);
			// 
			// chkSealed
			// 
			this.chkSealed.Location = new System.Drawing.Point(250, 126);
			this.chkSealed.Name = "chkSealed";
			this.chkSealed.Size = new System.Drawing.Size(144, 24);
			this.chkSealed.TabIndex = 6;
			this.chkSealed.Text = "Sealed";
			this.chkSealed.CheckedChanged += new System.EventHandler(this.chkSealed_CheckedChanged);					
			// 
			// lblBaseClass
			// 
			this.lblBaseClass.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblBaseClass.Location = new System.Drawing.Point(260, 238);
			this.lblBaseClass.Name = "lblBaseClass";
			this.lblBaseClass.Size = new System.Drawing.Size(152, 48);
			this.lblBaseClass.TabIndex = 4;
			this.lblBaseClass.Text = "object";
			// 
			// errorProvider1
			// 
			this.errorProvider1.DataMember = null;
			// 
			// txtClassName
			// 
			this.txtClassName.Location = new System.Drawing.Point(250, 318);
			this.txtClassName.Name = "txtClassName";
			this.txtClassName.Size = new System.Drawing.Size(152, 22);
			this.txtClassName.TabIndex = 1;
			this.txtClassName.Text = "";
			this.txtClassName.TextChanged += new System.EventHandler(this.txtClassName_TextChanged);
			// 
			// txtProject
			// 
			this.txtProject.Location = new System.Drawing.Point(250, 64);
			this.txtProject.Name = "txtProject";
			this.txtProject.Size = new System.Drawing.Size(175, 24);
			this.txtProject.TabIndex = 9;
			this.txtProject.Enabled = false;
			
			// 
			// treeClasses
			// 
			this.treeClasses.HideSelection = false;
			this.treeClasses.HotTracking = true;
			this.treeClasses.Location = new System.Drawing.Point(5, 25);
			this.treeClasses.Name = "treeClasses";
			this.treeClasses.PathSeparator = ".";
			this.treeClasses.Size = new System.Drawing.Size(235, 335);
			this.treeClasses.Sorted = true;
			this.treeClasses.TabIndex = 0;
			this.treeClasses.ShowAbstractClasses = true;
			this.treeClasses.ShowConcreteClasses = true;
			this.treeClasses.ShowSealedClasses = false;
			this.treeClasses.ShowInterfaces = false;			
			this.treeClasses.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeClasses_AfterSelect);

			
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
										this.label13,
										this.chkAbstract,
										this.chkSealed,
										this.chkPublic,
										this.lblBaseClass,
										this.label11,
										this.label10,
										this.label12,
										this.txtClassName, 
										this.txtProject,
										this.treeClasses
										});
			
			this.Load += new System.EventHandler(this.Panel_Load);
			this.ResumeLayout(false);			
			
		}		
	}
}
