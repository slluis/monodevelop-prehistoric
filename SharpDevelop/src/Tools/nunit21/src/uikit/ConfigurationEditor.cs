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
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.Util;

namespace NUnit.UiKit
{
	/// <summary>
	/// ConfigurationEditor form is designed for adding, deleting
	/// and renaming configurations from a project.
	/// </summary>
	public class ConfigurationEditor : System.Windows.Forms.Form
	{
		#region Instance Variables

		private NUnitProject project;
		private TestLoader loader;

		private int selectedIndex = -1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListBox configListBox;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button renameButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button activeButton;
		private System.Windows.Forms.HelpProvider helpProvider1;
		private System.Windows.Forms.Button closeButton;

		#endregion

		#region Construction and Disposal

		public ConfigurationEditor( NUnitProject project )
		{
			InitializeComponent();
			this.project = project;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConfigurationEditor));
			this.configListBox = new System.Windows.Forms.ListBox();
			this.removeButton = new System.Windows.Forms.Button();
			this.renameButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.activeButton = new System.Windows.Forms.Button();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.SuspendLayout();
			// 
			// configListBox
			// 
			this.helpProvider1.SetHelpString(this.configListBox, "Selects the configuration to operate on.");
			this.configListBox.ItemHeight = 16;
			this.configListBox.Location = new System.Drawing.Point(8, 8);
			this.configListBox.Name = "configListBox";
			this.helpProvider1.SetShowHelp(this.configListBox, true);
			this.configListBox.Size = new System.Drawing.Size(168, 244);
			this.configListBox.TabIndex = 0;
			this.configListBox.SelectedIndexChanged += new System.EventHandler(this.configListBox_SelectedIndexChanged);
			// 
			// removeButton
			// 
			this.helpProvider1.SetHelpString(this.removeButton, "Removes the selected configuration");
			this.removeButton.Location = new System.Drawing.Point(192, 8);
			this.removeButton.Name = "removeButton";
			this.helpProvider1.SetShowHelp(this.removeButton, true);
			this.removeButton.Size = new System.Drawing.Size(96, 32);
			this.removeButton.TabIndex = 1;
			this.removeButton.Text = "&Remove";
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// renameButton
			// 
			this.helpProvider1.SetHelpString(this.renameButton, "Allows renaming the selected configuration");
			this.renameButton.Location = new System.Drawing.Point(192, 48);
			this.renameButton.Name = "renameButton";
			this.helpProvider1.SetShowHelp(this.renameButton, true);
			this.renameButton.Size = new System.Drawing.Size(96, 32);
			this.renameButton.TabIndex = 2;
			this.renameButton.Text = "Re&name";
			this.renameButton.Click += new System.EventHandler(this.renameButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.helpProvider1.SetHelpString(this.closeButton, "Closes this dialog");
			this.closeButton.Location = new System.Drawing.Point(192, 216);
			this.closeButton.Name = "closeButton";
			this.helpProvider1.SetShowHelp(this.closeButton, true);
			this.closeButton.Size = new System.Drawing.Size(96, 32);
			this.closeButton.TabIndex = 4;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// addButton
			// 
			this.helpProvider1.SetHelpString(this.addButton, "Allows adding a new configuration");
			this.addButton.Location = new System.Drawing.Point(192, 88);
			this.addButton.Name = "addButton";
			this.helpProvider1.SetShowHelp(this.addButton, true);
			this.addButton.Size = new System.Drawing.Size(96, 32);
			this.addButton.TabIndex = 5;
			this.addButton.Text = "&Add...";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// activeButton
			// 
			this.helpProvider1.SetHelpString(this.activeButton, "Makes the selected configuration active");
			this.activeButton.Location = new System.Drawing.Point(192, 128);
			this.activeButton.Name = "activeButton";
			this.helpProvider1.SetShowHelp(this.activeButton, true);
			this.activeButton.Size = new System.Drawing.Size(96, 32);
			this.activeButton.TabIndex = 6;
			this.activeButton.Text = "&Make Active";
			this.activeButton.Click += new System.EventHandler(this.activeButton_Click);
			// 
			// ConfigurationEditor
			// 
			this.AcceptButton = this.closeButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(298, 268);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.activeButton,
																		  this.addButton,
																		  this.closeButton,
																		  this.renameButton,
																		  this.removeButton,
																		  this.configListBox});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConfigurationEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ConfigurationEditor";
			this.Load += new System.EventHandler(this.ConfigurationEditor_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region Static Methods

		public static void Edit( NUnitProject project )
		{
			ConfigurationEditor editor = new ConfigurationEditor( project );
			editor.ShowDialog();
		}

		public static void AddConfiguration( NUnitProject project )
		{
			AddConfigurationDialog dlg = new AddConfigurationDialog( project );
			dlg.ShowDialog();
		}

		#endregion

		#region UI Event Handlers

		private void ConfigurationEditor_Load(object sender, System.EventArgs e)
		{
			FillListBox();
			if ( configListBox.Items.Count > 0 )
				configListBox.SelectedIndex = selectedIndex = 0;
		}

		private void removeButton_Click(object sender, System.EventArgs e)
		{	
			string name = project.Configs[selectedIndex].Name;
			
			if ( project.Configs.Count == 1 )
			{
				string msg = "Removing the last configuration will make the project unloadable until you add another configuration.\r\rAre you sure you want to remove the configuration?";
				
				if( UserMessage.Ask( msg, "Remove Configuration" ) == DialogResult.No )
					return;
			}

			project.Configs.RemoveAt( selectedIndex );
			FillListBox();
		}

		private void renameButton_Click(object sender, System.EventArgs e)
		{
			RenameConfiguration( project.Configs[selectedIndex].Name );
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			AddConfiguration( project );
			FillListBox();
		}

		private void activeButton_Click(object sender, System.EventArgs e)
		{
			project.SetActiveConfig( selectedIndex );
			//AppUI.TestLoader.LoadConfig( project.Configs[selectedIndex].Name );
			FillListBox();
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void configListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			selectedIndex = configListBox.SelectedIndex;
			activeButton.Enabled = selectedIndex >= 0 && project.Configs[selectedIndex].Name != project.ActiveConfigName;
			renameButton.Enabled = addButton.Enabled = selectedIndex >= 0;
			removeButton.Enabled = selectedIndex >= 0 && configListBox.Items.Count > 0;
		}

		#endregion

		#region Helper Methods

		private void RenameConfiguration( string oldName )
		{
			RenameConfigurationDialog dlg	= new RenameConfigurationDialog( project, oldName );
			if ( dlg.ShowDialog() == DialogResult.OK )
			{
				project.Configs[oldName].Name = dlg.ConfigurationName;
				FillListBox();
			}
		}

		private void FillListBox()
		{
			configListBox.Items.Clear();
			int count = 0;

			foreach( ProjectConfig config in project.Configs )
			{
				string name = config.Name;

				if ( name == project.ActiveConfigName )
					name += " (active)";
				
				configListBox.Items.Add( name );
				count++;
			}	
		
			if ( count > 0 )
			{
				if( selectedIndex >= count )
					selectedIndex = count - 1;

				configListBox.SelectedIndex = selectedIndex;
			}
			else selectedIndex = -1;
		}

		#endregion

	}
}
