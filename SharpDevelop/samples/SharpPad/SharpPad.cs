//  SharpPad.cs 
//  Copyright (c) 2001 Bryan Livingston <bryan@alphora.com>
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using ICSharpCode.TextEditor;

public class SharpPad : System.Windows.Forms.Form
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.Container components = null;
	private System.Windows.Forms.MainMenu mainMenu1;
	private System.Windows.Forms.MenuItem menuItem1;
	private System.Windows.Forms.MenuItem menuItem3;
	private System.Windows.Forms.MenuItem menuItem5;
	private System.Windows.Forms.MenuItem menuItem6;
	private System.Windows.Forms.MenuItem menuItem8;
	private System.Windows.Forms.MenuItem menuItem9;
	private System.Windows.Forms.MenuItem menuItem10;
	private System.Windows.Forms.MenuItem menuItem11;
	private System.Windows.Forms.MenuItem menuItem12;
	private System.Windows.Forms.MenuItem menuItem13;
	private System.Windows.Forms.MenuItem menuItem14;
	private System.Windows.Forms.MenuItem menuItem15;
	private System.Windows.Forms.MenuItem menuItem16;
	private System.Windows.Forms.MenuItem menuItem17;
	private System.Windows.Forms.MenuItem menuItem18;
	private System.Windows.Forms.MenuItem menuItem19;
	private System.Windows.Forms.MenuItem menuItem20;
	private System.Windows.Forms.MenuItem menuItem21;
	private System.Windows.Forms.MenuItem menuItem22;
	private System.Windows.Forms.MenuItem menuItem23;
	private System.Windows.Forms.MenuItem menuItem24;
	private System.Windows.Forms.MenuItem menuItem25;
	private System.Windows.Forms.MenuItem menuItem26;
	private System.Windows.Forms.MenuItem menuItem27;
	private System.Windows.Forms.MenuItem menuItem28;
	private System.Windows.Forms.MenuItem menuItem30;
	private System.Windows.Forms.MenuItem menuItem31;
	private System.Windows.Forms.MenuItem menuItem32;
	private System.Windows.Forms.MenuItem OpenMenuItem;
	private System.Windows.Forms.MenuItem menuItem33;
	private System.Windows.Forms.OpenFileDialog OpenFileDialog;
	private System.Windows.Forms.MenuItem SaveAsMenuItem;
	private System.Windows.Forms.SaveFileDialog SaveFileDialog;
	private System.Windows.Forms.MenuItem CloseMenuItem;

	
	private TextEditorControl FTextArea;
	
	public SharpPad()
	{ 
		FTextArea = new TextEditorControl();
		InitializeComponent();
		FTextArea.Dock = DockStyle.Fill;
		Controls.Add(FTextArea);
	} 

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	protected override void Dispose( bool disposing )
	{
		if( disposing )
		{
			if (components != null) 
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
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SharpPad));
		this.mainMenu1 = new System.Windows.Forms.MainMenu();
		this.menuItem1 = new System.Windows.Forms.MenuItem();
		this.menuItem5 = new System.Windows.Forms.MenuItem();
		this.OpenMenuItem = new System.Windows.Forms.MenuItem();
		this.menuItem6 = new System.Windows.Forms.MenuItem();
		this.SaveAsMenuItem = new System.Windows.Forms.MenuItem();
		this.menuItem3 = new System.Windows.Forms.MenuItem();
		this.menuItem33 = new System.Windows.Forms.MenuItem();
		this.menuItem25 = new System.Windows.Forms.MenuItem();
		this.menuItem9 = new System.Windows.Forms.MenuItem();
		this.menuItem8 = new System.Windows.Forms.MenuItem();
		this.CloseMenuItem = new System.Windows.Forms.MenuItem();
		this.menuItem10 = new System.Windows.Forms.MenuItem();
		this.menuItem11 = new System.Windows.Forms.MenuItem();
		this.menuItem12 = new System.Windows.Forms.MenuItem();
		this.menuItem13 = new System.Windows.Forms.MenuItem();
		this.menuItem17 = new System.Windows.Forms.MenuItem();
		this.menuItem14 = new System.Windows.Forms.MenuItem();
		this.menuItem15 = new System.Windows.Forms.MenuItem();
		this.menuItem16 = new System.Windows.Forms.MenuItem();
		this.menuItem30 = new System.Windows.Forms.MenuItem();
		this.menuItem18 = new System.Windows.Forms.MenuItem();
		this.menuItem19 = new System.Windows.Forms.MenuItem();
		this.menuItem20 = new System.Windows.Forms.MenuItem();
		this.menuItem21 = new System.Windows.Forms.MenuItem();
		this.menuItem22 = new System.Windows.Forms.MenuItem();
		this.menuItem28 = new System.Windows.Forms.MenuItem();
		this.menuItem31 = new System.Windows.Forms.MenuItem();
		this.menuItem32 = new System.Windows.Forms.MenuItem();
		this.menuItem23 = new System.Windows.Forms.MenuItem();
		this.menuItem24 = new System.Windows.Forms.MenuItem();
		this.menuItem26 = new System.Windows.Forms.MenuItem();
		this.menuItem27 = new System.Windows.Forms.MenuItem();
		this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
		// 
		// mainMenu1
		// 
		this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem1,
																					this.menuItem10,
																					this.menuItem18,
																					this.menuItem23});
		// 
		// menuItem1
		// 
		this.menuItem1.Index = 0;
		this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem5,
																					this.OpenMenuItem,
																					this.menuItem6,
																					this.SaveAsMenuItem,
																					this.menuItem3,
																					this.menuItem33,
																					this.menuItem25,
																					this.menuItem9,
																					this.menuItem8,
																					this.CloseMenuItem});
		this.menuItem1.Text = "&File";
		// 
		// menuItem5
		// 
		this.menuItem5.Index = 0;
		this.menuItem5.Text = "New";
		this.menuItem5.Visible = false;
		// 
		// OpenMenuItem
		// 
		this.OpenMenuItem.Index = 1;
		this.OpenMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
		this.OpenMenuItem.Text = "&Open";
		this.OpenMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
		// 
		// menuItem6
		// 
		this.menuItem6.Index = 2;
		this.menuItem6.Text = "Save";
		this.menuItem6.Visible = false;
		// 
		// SaveAsMenuItem
		// 
		this.SaveAsMenuItem.Index = 3;
		this.SaveAsMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
		this.SaveAsMenuItem.Text = "Save &As";
		this.SaveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItem_Click);
		// 
		// menuItem3
		// 
		this.menuItem3.Index = 4;
		this.menuItem3.Text = "Close";
		this.menuItem3.Visible = false;
		// 
		// menuItem33
		// 
		this.menuItem33.Index = 5;
		this.menuItem33.Text = "-";
		this.menuItem33.Visible = false;
		// 
		// menuItem25
		// 
		this.menuItem25.Index = 6;
		this.menuItem25.Text = "Page Setup";
		this.menuItem25.Visible = false;
		// 
		// menuItem9
		// 
		this.menuItem9.Index = 7;
		this.menuItem9.Text = "Print";
		this.menuItem9.Visible = false;
		// 
		// menuItem8
		// 
		this.menuItem8.Index = 8;
		this.menuItem8.Text = "-";
		// 
		// CloseMenuItem
		// 
		this.CloseMenuItem.Index = 9;
		this.CloseMenuItem.Shortcut = System.Windows.Forms.Shortcut.AltF4;
		this.CloseMenuItem.Text = "E&xit";
		this.CloseMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
		// 
		// menuItem10
		// 
		this.menuItem10.Index = 1;
		this.menuItem10.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem11,
																					this.menuItem12,
																					this.menuItem13,
																					this.menuItem17,
																					this.menuItem14,
																					this.menuItem15,
																					this.menuItem16,
																					this.menuItem30});
		this.menuItem10.Text = "Edit";
		this.menuItem10.Visible = false;
		// 
		// menuItem11
		// 
		this.menuItem11.Index = 0;
		this.menuItem11.Text = "Undo";
		// 
		// menuItem12
		// 
		this.menuItem12.Index = 1;
		this.menuItem12.Text = "Redo";
		// 
		// menuItem13
		// 
		this.menuItem13.Index = 2;
		this.menuItem13.Text = "Select All";
		// 
		// menuItem17
		// 
		this.menuItem17.Index = 3;
		this.menuItem17.Text = "Select None";
		// 
		// menuItem14
		// 
		this.menuItem14.Index = 4;
		this.menuItem14.Text = "Cut";
		// 
		// menuItem15
		// 
		this.menuItem15.Index = 5;
		this.menuItem15.Text = "Copy";
		// 
		// menuItem16
		// 
		this.menuItem16.Index = 6;
		this.menuItem16.Text = "Paste";
		// 
		// menuItem30
		// 
		this.menuItem30.Index = 7;
		this.menuItem30.Text = "Delete";
		// 
		// menuItem18
		// 
		this.menuItem18.Index = 2;
		this.menuItem18.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem19,
																					this.menuItem20,
																					this.menuItem21,
																					this.menuItem22,
																					this.menuItem28,
																					this.menuItem31,
																					this.menuItem32});
		this.menuItem18.Text = "Config";
		this.menuItem18.Visible = false;
		// 
		// menuItem19
		// 
		this.menuItem19.Index = 0;
		this.menuItem19.Text = "Syntax Hignlighting";
		// 
		// menuItem20
		// 
		this.menuItem20.Index = 1;
		this.menuItem20.Text = "Word Wrap";
		// 
		// menuItem21
		// 
		this.menuItem21.Index = 2;
		this.menuItem21.Text = "BufferOptions";
		// 
		// menuItem22
		// 
		this.menuItem22.Index = 3;
		this.menuItem22.Text = "Font";
		// 
		// menuItem28
		// 
		this.menuItem28.Index = 4;
		this.menuItem28.Text = "Tab Size";
		// 
		// menuItem31
		// 
		this.menuItem31.Index = 5;
		this.menuItem31.Text = "Formatting Strategy";
		// 
		// menuItem32
		// 
		this.menuItem32.Index = 6;
		this.menuItem32.Text = "Auto Indent";
		// 
		// menuItem23
		// 
		this.menuItem23.Index = 3;
		this.menuItem23.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem24,
																					this.menuItem26,
																					this.menuItem27});
		this.menuItem23.Text = "View";
		this.menuItem23.Visible = false;
		// 
		// menuItem24
		// 
		this.menuItem24.Index = 0;
		this.menuItem24.Text = "Line Numbers";
		// 
		// menuItem26
		// 
		this.menuItem26.Index = 1;
		this.menuItem26.Text = "Invalid Lines";
		// 
		// menuItem27
		// 
		this.menuItem27.Index = 2;
		this.menuItem27.Text = "Bracket Highlighter";
		// 
		// SaveFileDialog
		// 
		this.SaveFileDialog.FileName = "doc1";
		// 
		// SharpPad
		// 
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.ClientSize = new System.Drawing.Size(672, 441);
		try {
		this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
		} catch (Exception){}
		this.Menu = this.mainMenu1;
		this.Name = "SharpPad";
		this.Text = "SharpPad";

	}
	#endregion

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main() 
	{
		Application.Run(new SharpPad());
	}

	private void OpenMenuItem_Click(object sender, System.EventArgs e)
	{
		//if(OpenFileDialog.ShowDialog() == DialogResult.OK)
			//FTextArea.LoadFile(OpenFileDialog.FileName);
	}

	private void SaveAsMenuItem_Click(object sender, System.EventArgs e)
	{
		if(SaveFileDialog.ShowDialog() == DialogResult.OK) {
			//FTextArea.SaveFile(SaveFileDialog.FileName);
		}
	}

	private void CloseMenuItem_Click(object sender, System.EventArgs e)
	{
		Close();
	}
}
