// project created on 16.07.2002 at 18:07
using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;

namespace MyPlugin {
	
	public class TestPlugin : AbstractPadContent
	{
		Panel panel     = new Panel();
		Label testLabel = new Label();
		
		public override Control Control {
			get {
				return panel;
			}
		}
		
		public TestPlugin() : base("TestPanel")
		{
			testLabel.Text     = "Hello World!";
			testLabel.Location = new Point(8, 8);
			panel.Controls.Add(testLabel);
		}
	}
}
