// project created on 16.07.2002 at 18:07
using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui;

namespace MyPlugin {
	
	public class RunMyPluginCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ShowView(new MyCustomViewContent());
		}
	}
}
