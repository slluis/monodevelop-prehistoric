// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;

namespace ResEdit
{
	/// <summary>
	/// This class displays a bitmap in a window, the window and the bitmap can be resized.
	/// </summary>
	class BinaryView : Form
	{
		TextBox text = new TextBox();
		CheckBox chk = new CheckBox();
		Label lbl    = new Label();
		
		byte[] Bytes;
		string regText;
		
		ASCIIEncoding enc = new ASCIIEncoding();
		Regex rgex   = new Regex(@"\p{Cc}");
			
		public BinaryView(byte[] bytes, string caption)
		{
			Bytes           = bytes;
			regText         = enc.GetString(bytes).Replace("\x0", ".");
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			
			ClientSize      = new Size(Screen.PrimaryScreen.Bounds.Width - 200, Screen.PrimaryScreen.Bounds.Height - 250);
			StartPosition   = FormStartPosition.CenterScreen;
			TopMost         = true;
			MaximizeBox     = false;
			MinimizeBox     = false;
			Owner           = (Form)WorkbenchSingleton.Workbench;
			Text            = caption;
			DockPadding.Top = 24;
			
			text.Dock       = DockStyle.Fill;
			text.Font       = resourceService.LoadFont("Courier New", 10);
			text.ScrollBars = ScrollBars.Both;
			text.ReadOnly   = true;
			text.Multiline  = true;
			text.Text       = regText;
			text.BackColor  = SystemColors.Window;
			
			chk.Location    = new Point(8, 4);
			chk.Size        = new Size(ClientSize.Width - 16, 16);
			chk.Text        = "Show as Hex Dump";
			chk.CheckedChanged += new EventHandler(CheckEvt);
			
			Controls.Add(text);
			Controls.Add(chk);
			text.Select();
		}
		
		public void CheckEvt(object sender, EventArgs e)
		{
			if (chk.Checked) {
				// Hex Dump
				StringBuilder sb = new StringBuilder();
				
				string bytes = BitConverter.ToString(Bytes).Replace("-", " ");
				string stext = rgex.Replace(regText, ".");
				
				text.Text = "";
				int max = Bytes.Length;
				int last = max % 16;
				
				int i = 0;
				int count = 16;
				do {
					sb.Append(String.Format("{0:X8}  ", i) +
						bytes.Substring(i*3, (count * 3) - 1) + "  " +
						stext.Substring(i, count) + "\r\n");
					i += 16;
					if (i >= (max - last)) {
						count = last;
					}
				} while (i < max);
								
				text.Text = sb.ToString();
			} else {
				// Regular Text
				text.Text = regText;
			}
		}
		
	}
}
