// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

namespace JavaBinding
{
	/// <summary>
	/// Summary description for Form5.
	/// </summary>
	public class ProjectConfigurationPropertyPanel : AbstractOptionPanel
	{
		/*private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;		
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;

		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;				
		private System.Windows.Forms.Button button1;
		
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox6;
		private System.Windows.Forms.CheckBox checkBox7;
		
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;

		private System.Windows.Forms.TextBox textBox5;	//Compiler Path
		private System.Windows.Forms.TextBox textBox6;	//Classpath
		private System.Windows.Forms.TextBox textBox7;	//MainClass
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		JavaCompilerParameters compilerParameters = null;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null)
					return true;
				compilerParameters.GenWarnings = checkBox7.Checked;			
				compilerParameters.Deprecation = checkBox6.Checked;			
				compilerParameters.Debugmode = checkBox5.Checked;			
				compilerParameters.Optimize = checkBox3.Checked;						
				compilerParameters.OutputAssembly = textBox2.Text;
				compilerParameters.OutputDirectory = textBox3.Text;
				
				compilerParameters.CompilerPath = textBox5.Text;
				compilerParameters.ClassPath = textBox6.Text;
				compilerParameters.MainClass = textBox7.Text;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (JavaCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			checkBox3.Checked = compilerParameters.Optimize;
			checkBox5.Checked = compilerParameters.Debugmode;
			checkBox6.Checked = compilerParameters.Deprecation;
			checkBox7.Checked = compilerParameters.GenWarnings;
			textBox2.Text = compilerParameters.OutputAssembly;
			textBox3.Text = compilerParameters.OutputDirectory;				
			
			textBox5.Text = compilerParameters.CompilerPath;
			textBox6.Text = compilerParameters.ClassPath;				
			textBox7.Text = compilerParameters.MainClass;				
		}
		
		void SelectFolder(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			
			if (fdiag.DisplayDialog(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription")) == DialogResult.OK) {
				textBox3.Text = fdiag.Path;
			}
		}
		
		public ProjectConfigurationPropertyPanel()
		{
			InitializeComponent();						
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
				{
					if(components != null)
						{
							components.Dispose();
						}
				}
				base.Dispose( disposing );
		}
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBox6 = new System.Windows.Forms.CheckBox();
			this.checkBox5 = new System.Windows.Forms.CheckBox();

			this.checkBox3 = new System.Windows.Forms.CheckBox();
			
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			
			this.checkBox7 = new System.Windows.Forms.CheckBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.textBox7 = new System.Windows.Forms.TextBox();
			
			this.label6 = new System.Windows.Forms.Label();			
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			//
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {	this.textBox5,
			                                 this.textBox6,
			                                 this.textBox7,
			                                 this.label6,
			                                 this.label7,
			                                 this.label8,
			                                 this.checkBox5,
			                                 this.checkBox6,
			                                 this.checkBox7,
			                                 this.checkBox3});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(376, 232);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CodeGenerationGroupBox");
			
			//
			// checkBox6
			// 
			this.checkBox6.Location = new System.Drawing.Point(192, 128);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new System.Drawing.Size(176, 16);
			this.checkBox6.TabIndex = 8;
			this.checkBox6.Text = "Deprecation";
			//this.checkBox5.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.DebugInformationCheckBox");
			
			//
			// checkBox7
			// 
			this.checkBox7.Location = new System.Drawing.Point(192, 146);
			this.checkBox7.Name = "checkBox7";
			this.checkBox7.Size = new System.Drawing.Size(176, 16);
			this.checkBox7.TabIndex = 8;
			this.checkBox7.Text = "Generate Warnings";
			//this.checkBox5.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.DebugInformationCheckBox");
			
			//
			// checkBox5
			// 
			this.checkBox5.Location = new System.Drawing.Point(192, 112);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(176, 16);
			this.checkBox5.TabIndex = 8;
			this.checkBox5.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.DebugInformationCheckBox");
			
			//
			// checkBox3
			// 
			this.checkBox3.Location = new System.Drawing.Point(192, 96);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(176, 16);
			this.checkBox3.TabIndex = 7;
			this.checkBox3.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OptimizeCheckBox");
			
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(18, 50);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 23);
			this.label6.TabIndex = 99;
			this.label6.Text = "Compiler Path";	//resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OutputPathLabel");
			
			//
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(186, 50);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(182, 20);
			this.textBox5.TabIndex = 1;
			this.textBox5.Text = "";
			
			//
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(18, 70);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 23);
			this.label7.TabIndex = 99;
			this.label7.Text = "Class Path"; 	//resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OutputPathLabel");
			
			//
			// textBox6
			// 
			this.textBox6.Location = new System.Drawing.Point(186, 70);
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(182, 20);
			this.textBox6.TabIndex = 1;
			this.textBox6.Text = "";
			
			//
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(18, 170);
			this.label8.Name = "label7";
			this.label8.Size = new System.Drawing.Size(80, 23);
			this.label8.TabIndex = 99;
			this.label8.Text = "Main Class"; 	//resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OutputPathLabel");
			
			
			//
			// textBox7
			// 
			this.textBox7.Location = new System.Drawing.Point(186, 170);
			this.textBox7.Name = "textBox7";
			this.textBox7.Size = new System.Drawing.Size(182, 20);
			this.textBox7.TabIndex = 1;
			this.textBox7.Text = "";
			
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {   //this.checkBox4,
																					this.button1,
																					this.textBox3,
																					this.label5,
																					this.textBox2,
																					this.label4});
			this.groupBox2.Location = new System.Drawing.Point(8, 240);			
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(376, 96);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OutputGroupBox");
			
			//
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(344, 40);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(24, 24);
			this.button1.TabIndex = 3;
			this.button1.Text = "...";
			this.button1.Click += new EventHandler(SelectFolder);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(96, 40);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(240, 20);
			this.textBox3.TabIndex = 2;
			this.textBox3.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 40);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 23);
			this.label5.TabIndex = 99;
			this.label5.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OutputPathLabel");
			
			//
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(96, 16);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(272, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 16);
			this.label4.TabIndex = 99;
			this.label4.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.WarningsAsErrorsCheckBox");
			
			//
			// Form5
			// 
			this.ClientSize = new System.Drawing.Size(392, 341);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {   this.groupBox2,
																		  this.groupBox1});
			this.Name = "Form5";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		*/
	}
}
