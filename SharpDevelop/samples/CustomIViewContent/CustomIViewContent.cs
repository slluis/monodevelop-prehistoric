using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace MyPlugin {
	
	public class MyCustomViewContent : AbstractViewContent
	{
		Panel panel     = new Panel();
		Label testLabel = new Label();
		
		public override Control Control {
			get {
				return panel;
			}
		}
		
		// must be overriden, but *may* be useless for
		// 'custom'  views
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override void SaveFile(string fileName)
		{
		}
		
		public override void LoadFile(string fileName)
		{
		}
		
		// the redraw should get new add-in tree information
		// and update the view, the language or layout manager
		// may have changed.
		public override void RedrawContent()
		{
		}
		
		// The Dispose must be overriden, there is no default implementation
		// (because in this case I wouldn't override dipose, I would forget it ...)
		public override void Dispose()
		{
			testLabel.Dispose();
			panel.Dispose();
		}
		
		
		
		public MyCustomViewContent()
		{
			testLabel.Text     = "Hello World!";
			testLabel.Location = new Point(8, 8);
			panel.Controls.Add(testLabel);
			
			ContentName = "can't give good names";
		}
		
	}

}
