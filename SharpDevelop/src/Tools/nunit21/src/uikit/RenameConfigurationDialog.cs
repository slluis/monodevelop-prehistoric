#region Copyright (c) 2002-2003, James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole, Philip A. Craig
/************************************************************************************
'
' Copyright  2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' Copyright  2000-2002 Philip A. Craig
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' Portions Copyright  2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' or Copyright  2000-2002 Philip A. Craig
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'***********************************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// Summary description for AssemblyNameDialog.
	/// </summary>
	public class RenameConfigurationDialog : System.Windows.Forms.Form
	{
		#region Instance Variables

		/// <summary>
		///  The project in which we are renaming a configuration
		/// </summary>
		private NUnitProject project;

		/// <summary>
		/// The new name to give the configuration
		/// </summary>
		private string configurationName;

		/// <summary>
		/// The original name of the configuration
		/// </summary>
		private string originalName;

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox configurationNameTextBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Construction and Disposal

		public RenameConfigurationDialog( NUnitProject project, string configurationName )
		{
			InitializeComponent();
			this.project = project;
			this.configurationName = configurationName;
			this.originalName = configurationName;
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

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.configurationNameTextBox = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// configurationNameTextBox
			// 
			this.configurationNameTextBox.Location = new System.Drawing.Point(16, 16);
			this.configurationNameTextBox.Name = "configurationNameTextBox";
			this.configurationNameTextBox.Size = new System.Drawing.Size(264, 22);
			this.configurationNameTextBox.TabIndex = 0;
			this.configurationNameTextBox.Text = "";
			this.configurationNameTextBox.TextChanged += new System.EventHandler(this.configurationNameTextBox_TextChanged);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(56, 48);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(160, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			// 
			// RenameConfigurationDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(292, 80);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.cancelButton,
																		  this.okButton,
																		  this.configurationNameTextBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "RenameConfigurationDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Rename Configuration";
			this.Load += new System.EventHandler(this.ConfigurationNameDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region Properties & Methods

		public string ConfigurationName
		{
			get{ return configurationName; }
			set{ configurationName = value; }
		}
		
		private void ConfigurationNameDialog_Load(object sender, System.EventArgs e)
		{
			if ( configurationName != null )
			{
				configurationNameTextBox.Text = configurationName;
				configurationNameTextBox.SelectAll();
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			configurationName = configurationNameTextBox.Text;		
			if ( project.Configs.Contains( configurationName ) )
				// TODO: Need general error message display
				UserMessage.DisplayFailure( "A configuration with that name already exists", "Configuration Name Error" );
			else
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void configurationNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			okButton.Enabled = 
				configurationNameTextBox.TextLength > 0 &&
				configurationNameTextBox.Text != originalName;
		}

		#endregion
	}
}
