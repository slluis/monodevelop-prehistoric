// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core.Services;

namespace ResEdit
{
	/// <summary>
	/// This dialog is for editing a key entry
	/// </summary>
	public class RenameEntry : Form
	{
		private System.ComponentModel.Container components;
		private Button button2;
		private Button button1;
		
		private TextBox textBox1;
		private Label label1;
		private GroupBox groupBox1;
		
		public string Value;
		
		public RenameEntry(string name) 
		{
			InitializeComponent();
			Icon          = null;
			textBox1.Text = name;
			textBox1.Focus();
		}
		/* TODO : NEW DISPOSE FUNCTION
		public override void Dispose() 
		{
			base.Dispose();
			components.Dispose();
		}*/
		
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.groupBox1 = new GroupBox();
			this.button1 = new Button();
			this.textBox1 = new TextBox();
			this.button2 = new Button();
			this.label1 = new Label();
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			groupBox1.Location = new System.Drawing.Point(8, 8);
			groupBox1.TabStop = false;
			groupBox1.Text = resourceService.GetString("ResourceEditor.RenameEntry.EntryGroupBox");
			
			groupBox1.Size = new System.Drawing.Size(272, 72);
			
			button1.Location = new System.Drawing.Point(8, 88);
			button1.DialogResult = DialogResult.OK;
			button1.Size = new System.Drawing.Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = resourceService.GetString("Global.OKButtonText");
			button1.Click += new System.EventHandler(button1_Click);
			
			textBox1.Location = new System.Drawing.Point(8, 32);
			textBox1.TabIndex = 0;
			textBox1.Size = new System.Drawing.Size(256, 20);
			
			button2.Location = new System.Drawing.Point(96, 88);
			button2.DialogResult = DialogResult.Cancel;
			button2.Size = new System.Drawing.Size(75, 23);
			button2.TabIndex = 2;
			button2.Text = resourceService.GetString("Global.CancelButtonText");
			
			label1.Location = new System.Drawing.Point(8, 16);
			label1.Text = resourceService.GetString("ResourceEditor.RenameEntry.NameLabel");
			label1.Size = new System.Drawing.Size(40, 16);
			label1.TabIndex = 0;
			this.Text = resourceService.GetString("ResourceEditor.RenameEntry.DialogName");
			this.MaximizeBox = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = button2;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ShowInTaskbar = false;
			this.AcceptButton = button1;
			this.TopMost = true;
			this.MinimizeBox = false;
			this.ControlBox = false;
			this.ClientSize = new System.Drawing.Size(290, 119);
			
			groupBox1.Controls.Add(textBox1);
			groupBox1.Controls.Add(label1);
			this.Controls.Add(button2);
			this.Controls.Add(button1);
			this.Controls.Add(groupBox1);
			StartPosition = FormStartPosition.CenterScreen;
		}
		
		protected void button1_Click(object sender, System.EventArgs e)
		{
			Value  = textBox1.Text;
		}
	}
}
