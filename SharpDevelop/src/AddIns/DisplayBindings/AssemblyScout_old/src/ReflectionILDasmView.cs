// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using System.Drawing;
using ICSharpCode.Core.Services;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Internal.Reflection
{
	public class ReflectionILDasmView : UserControl
	{
		CheckBox chk   = new CheckBox();
		RichTextBox tb = new RichTextBox();
		ReflectionTree tree;
		
		public ReflectionILDasmView(ReflectionTree tree)
		{
			this.tree = tree;
			
			Dock = DockStyle.Fill;
			
			tb.Location = new Point(0, 24);
			tb.Size = new Size(Width, Height - 24);
			tb.Font = new System.Drawing.Font("Courier New", 10);
			tb.ScrollBars = RichTextBoxScrollBars.Both;
			tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			
			tb.WordWrap  = false;
			tb.ReadOnly  = true;
			
			chk.Location = new Point(0, 0);
			chk.Size = new Size(250, 16);
			chk.Text = tree.ress.GetString("ObjectBrowser.ILDasm.Enable");
			
			chk.CheckedChanged += new EventHandler(Check);
			Check(null, null);
			
			Controls.Add(tb);
			Controls.Add(chk);
			
			tree.AfterSelect += new TreeViewEventHandler(SelectNode);
		}
		
		void Check(object sender, EventArgs e)
		{
			if(chk.Checked) {
				tb.BackColor = SystemColors.Window;
			} else {
				tb.BackColor = SystemColors.Control;
				tb.Text = "";
			}
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			if (!chk.Checked) return;
			
			ReflectionNode node = (ReflectionNode)e.Node;
			
			Assembly assembly = null;
			string item = " /item=";
			
			if (node.Attribute is Assembly) {
				assembly = (Assembly)node.Attribute;
			} else if (node.Attribute is Type) {
				Type type = (Type)node.Attribute;
				item += type.FullName;
				assembly = type.Module.Assembly;
			} else if (node.Attribute is MethodBase) {
				MethodBase method = (MethodBase) node.Attribute;
				item += method.DeclaringType.FullName + "::" + method.Name;
				assembly = method.DeclaringType.Module.Assembly;
			} else {
				tb.Text = tree.ress.GetString("ObjectBrowser.ILDasm.NoView");
				return;
			}
			tb.Text = GetILDASMOutput(assembly, item).Replace("\n", "\r\n");
		}

		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

		private string GetILDASMOutput(Assembly assembly, string item)
		{
			try {
				string args = '"' + assembly.Location + '"' + item + " /NOBAR /TEXT";
                RegistryKey regKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\.NETFramework");
                string cmd = (string)regKey.GetValue("sdkInstallRoot");
                ProcessStartInfo psi = new ProcessStartInfo(fileUtilityService.GetDirectoryNameWithSeparator(cmd) +
                                                            "bin\\ildasm.exe", args);
				
				psi.RedirectStandardError  = true;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardInput  = true;
				psi.UseShellExecute        = false;
				psi.CreateNoWindow         = true;
				
				Process process = Process.Start(psi);
				string output   = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
								
				int cutpos = output.IndexOf(".namespace");
				
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				cutpos = output.IndexOf(".class");
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				cutpos = output.IndexOf(".method");
				if (cutpos != -1) {
					return output.Substring(cutpos);
				}
				
				return output;
			} catch (Exception) {
				return tree.ress.GetString("ObjectBrowser.ILDasm.NotInstalled");
			}
		}
	}
}
