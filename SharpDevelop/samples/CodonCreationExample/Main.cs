// project created on 16.07.2002 at 18:07
using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

namespace MyPlugin {
	
	public class RunMyPluginCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			string[] text = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/plugins/MyPlugin").BuildChildItems(this)).ToArray(typeof(string));
			
			MessageBox.Show(String.Concat(text), "TestPluginBox");
		}
	}
}
