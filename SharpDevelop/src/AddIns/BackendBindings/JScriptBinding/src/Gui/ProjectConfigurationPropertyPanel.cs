// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace JScriptBinding
{
	/// <summary>
	/// Summary description for Form5.
	/// </summary>
	public class ProjectConfigurationPropertyPanel : AbstractOptionPanel
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBox1;
		
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox xmlCheckBox = new CheckBox();
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox checkBox6;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		JScriptCompilerParameters compilerParameters = null;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null)
					return true;
				compilerParameters.Debugmode = checkBox5.Checked;
				compilerParameters.MainClass = textBox4.Text;
				compilerParameters.WarningLevel = trackBar1.Value;
				compilerParameters.Optimize = checkBox3.Checked;
				compilerParameters.UnsafeCode = checkBox2.Checked;
				compilerParameters.GenerateOverflowChecks = checkBox1.Checked;
				compilerParameters.DefineSymbols = textBox1.Text;
				compilerParameters.PauseConsoleOutput = checkBox4.Checked;
				compilerParameters.RunWithWarnings = !checkBox6.Checked;
				
				compilerParameters.GenerateXmlDocumentation = xmlCheckBox.Checked;
				compilerParameters.CompileTarget = (CompileTarget)comboBox1.SelectedIndex;
				
				compilerParameters.OutputAssembly = textBox2.Text;
				compilerParameters.OutputDirectory = textBox3.Text;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (JScriptCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			checkBox5.Checked = compilerParameters.Debugmode;
			textBox4.Text = compilerParameters.MainClass;
			trackBar1.Value = compilerParameters.WarningLevel;
			checkBox3.Checked = compilerParameters.Optimize;
			checkBox2.Checked = compilerParameters.UnsafeCode;
			checkBox1.Checked = compilerParameters.GenerateOverflowChecks;
			textBox1.Text = compilerParameters.DefineSymbols;
			checkBox4.Checked = compilerParameters.PauseConsoleOutput;
			checkBox6.Checked = !compilerParameters.RunWithWarnings;
			xmlCheckBox.Checked = compilerParameters.GenerateXmlDocumentation;
			comboBox1.SelectedIndex = (int)compilerParameters.CompileTarget;
			
			textBox2.Text = compilerParameters.OutputAssembly;
			textBox3.Text = compilerParameters.OutputDirectory;
		}
			
		
		void SelectFolder(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			
			if (fdiag.DisplayDialog(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription")) == DialogResult.OK) {
				textBox3.Text = fdiag.Path;
			}
		}

		/// <summary>
		/// 	
		/// </summary>
		
		public ProjectConfigurationPropertyPanel()
		{
			InitializeComponent();
			CustomizationObjectChanged += new EventHandler(SetValues);
			
			comboBox1.Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe"));
			//comboBox1.Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe"));
			comboBox1.Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Library"));
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
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.checkBox6 = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.checkBox6,
																					this.label11,
																					this.label10,
																					this.label9,
																					this.label8,
																					this.label7,
																					this.checkBox5,
																					this.textBox4,
																					this.label6,
																					this.label3,
																					this.trackBar1,
																					this.comboBox1,
																					this.label1,
																					this.checkBox3,
																					this.checkBox2,
																					this.label2,
																					this.checkBox1,
																					xmlCheckBox,
																					this.textBox1});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(376, 232);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CodeGenerationGroupBox");
			
			//
			// checkBox5
			// 
			this.checkBox5.Location = new System.Drawing.Point(192, 112);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(176, 16);
			this.checkBox5.TabIndex = 8;
			this.checkBox5.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.DebugInformationCheckBox");
			
			this.xmlCheckBox.Location = new System.Drawing.Point(192, 112 + 16);
			this.xmlCheckBox.Name = "xmlCheckBox";
			this.xmlCheckBox.Size = new System.Drawing.Size(176, 16);
			this.xmlCheckBox.TabIndex = 40;
			this.xmlCheckBox.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.GenerateXmlCheckBox");
			
			//
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(112, 64);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(256, 20);
			this.textBox4.TabIndex = 3;
			this.textBox4.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 64);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 23);
			this.label6.TabIndex = 99;
			this.label6.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.MainClassLabel");
			
			//
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 192);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 99;
			this.label3.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.WarningLevelLabel");
			
			//
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(112, 176);
			this.trackBar1.Maximum = 4;
			this.trackBar1.LargeChange = 1;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(256, 42);
			this.trackBar1.TabIndex = 9;
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.DropDownWidth = 152;
			this.comboBox1.Location = new System.Drawing.Point(112, 16);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(152, 21);
			this.comboBox1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 99;
			this.label1.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTargetLabel");
			
			//
			// checkBox3
			// 
			this.checkBox3.Location = new System.Drawing.Point(192, 96);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(176, 16);
			this.checkBox3.TabIndex = 7;
			this.checkBox3.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OptimizeCheckBox");
			
			//
			// checkBox2
			// 
			this.checkBox2.Location = new System.Drawing.Point(16, 112);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(168, 16);
			this.checkBox2.TabIndex = 5;
			this.checkBox2.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.UnsafeCodeCheckBox");
			
			//
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 16);
			this.label2.TabIndex = 99;
			this.label2.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.DefineSymbolsLabel");
			
			//
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(16, 96);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(168, 16);
			this.checkBox1.TabIndex = 4;
			this.checkBox1.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.OverflowCheckBox");
			
			//
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(112, 40);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(256, 20);
			this.textBox1.TabIndex = 2;
			this.textBox1.Text = "";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.checkBox4,
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
			// checkBox4
			// 
			this.checkBox4.Location = new System.Drawing.Point(8, 72);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(268, 16);
			this.checkBox4.TabIndex = 4;
			this.checkBox4.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.PauseConsoleOutputCheckBox");
			
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
			this.label4.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.AssemblyNameLabel");
			
			//
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(120, 208);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(16, 16);
			this.label7.TabIndex = 99;
			this.label7.Text = "0";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(178, 208);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(16, 16);
			this.label8.TabIndex = 99;
			this.label8.Text = "1";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(235, 208);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(16, 16);
			this.label9.TabIndex = 99;
			this.label9.Text = "2";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(291, 208);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(16, 16);
			this.label10.TabIndex = 99;
			this.label10.Text = "3";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(348, 208);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(16, 16);
			this.label11.TabIndex = 99;
			this.label11.Text = "4";
			// 
			// checkBox6
			// 
			this.checkBox6.Location = new System.Drawing.Point(16, 128);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new System.Drawing.Size(168, 16);
			this.checkBox6.TabIndex = 6;
			this.checkBox6.Text = resourceService.GetString("Dialog.Options.PrjOptions.Configuration.WarningsAsErrorsCheckBox");
			
			//
			// Form5
			// 
			this.ClientSize = new System.Drawing.Size(392, 341);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.Name = "Form5";
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
