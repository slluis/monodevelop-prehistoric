// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Text;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.HtmlControl;
using ICSharpCode.Core.Services;
using MonoDevelop.SourceEditor.Gui;

using Gtk;
using GtkSharp;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ColorDialog : ColorSelectionDialog
	{
		public ColorDialog () : base ("Insert a color hex string")
		{
			this.ColorSelection.HasPalette = true;
			this.ColorSelection.HasOpacityControl = false;		
			this.TransientFor = (Window) WorkbenchSingleton.Workbench;
		}
		
		public string ColorStr ()
		{
			Gdk.Color color = this.ColorSelection.CurrentColor;
			StringBuilder s = new StringBuilder ();
			ushort[] vals = { color.Red, color.Green, color.Blue };
			char[] hexchars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
			s.Append ("#");
			foreach (ushort val in vals) {
				/* Convert to a range of 0-255, then lookup the
				 * digit for each half-byte */
				byte rounded = (byte) (val >> 8);
				s.Append (hexchars[(rounded & 0xf0) >> 4]);
				s.Append (hexchars[rounded & 0x0f]);
			}
			return s.ToString ();
		}
	}

	public class ShowColorDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null)
				return;

			SourceEditorDisplayBindingWrapper w = window.ViewContent as SourceEditorDisplayBindingWrapper;
			if (w == null)
				return;

			//FIXME:  
                        // - Return color name (not color value) if it IsKnownColor,
			//   but it's still hasn't been implemented for System.Drawing.Color
			ColorDialog dialog = new ColorDialog ();
			if (dialog.Run () == (int) ResponseType.Ok)
			{
				w.InsertAtCursor (dialog.ColorStr ());
			}

			dialog.Hide ();
			dialog.Dispose ();
		}
	}
	
	public class QuickDocumentation : AbstractMenuCommand
	{
		public override void Run()
		{
			Console.WriteLine ("Not ported to the new editor yet");
			/*
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textAreaControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			
			int startLine = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.ActiveTextAreaControl.Caret.Offset);
			int endLine   = startLine;
			
			LineSegment line = textAreaControl.Document.GetLineSegment(startLine);
			string curLine   = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
			if (!curLine.StartsWith("///")) {
				return;
			}
			
			while (startLine > 0) {
				line    = textAreaControl.Document.GetLineSegment(startLine);
				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
				if (curLine.StartsWith("///")) {
					--startLine;
				} else {
					break;
				}
			}
			
			while (endLine < textAreaControl.Document.TotalNumberOfLines - 1) {
				line    = textAreaControl.Document.GetLineSegment(endLine);
				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
				if (curLine.StartsWith("///")) {
					++endLine;
				} else {
					break;
				}
			}
			
			StringBuilder documentation = new StringBuilder();
			for (int lineNr = startLine + 1; lineNr < endLine; ++lineNr) {
				line    = textAreaControl.Document.GetLineSegment(lineNr);
				curLine = textAreaControl.Document.GetText(line.Offset, line.Length).Trim();
				documentation.Append(curLine.Substring(3));
				documentation.Append('\n');
			}
			string xml  = "<member>" + documentation.ToString() + "</member>";
			
			string html = String.Empty;
			
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				html = ICSharpCode.SharpDevelop.Internal.Project.ConvertXml.ConvertData(xml,
				                   propertyService.DataDirectory +
				                   Path.DirectorySeparatorChar + "ConversionStyleSheets" +
				                   Path.DirectorySeparatorChar + "ShowXmlDocumentation.xsl",
				                   null);
			} catch (Exception e) {
				//MessageBox.Show(e.ToString());
			}
			//new ToolWindowForm(textAreaControl, html).Show();
			*/
		}
		
		class ToolWindowForm //: Form
		{/*
			public ToolWindowForm(TextEditorControl textEditorControl, string html)
			{
				Point caretPos  = textEditorControl.ActiveTextAreaControl.Caret.Position;
				Point visualPos = new Point(textEditorControl.ActiveTextAreaControl.TextArea.TextView.GetDrawingXPos(caretPos.Y, caretPos.X) + textEditorControl.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.X,
				          (int)((1 + caretPos.Y) * textEditorControl.ActiveTextAreaControl.TextArea.TextView.FontHeight) - textEditorControl.ActiveTextAreaControl.TextArea.VirtualTop.Y - 1 + textEditorControl.ActiveTextAreaControl.TextArea.TextView.DrawingPosition.Y);
				//Location = textEditorControl.ActiveTextAreaControl.TextArea.PointToScreen(visualPos);  //FIXME: Again, should we have this method?!?
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				HtmlControl hc = new HtmlControl();
				hc.Html = html;
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				hc.CascadingStyleSheet = propertyService.DataDirectory +
				                   Path.DirectorySeparatorChar + "resources" +
				                   Path.DirectorySeparatorChar + "css" +
				                   Path.DirectorySeparatorChar + "MsdnHelp.css";
				//hc.Dock = DockStyle.Fill;
				hc.BeforeNavigate += new BrowserNavigateEventHandler(BrowserNavigateCancel);
				//Controls.Add(hc);
								
				//ShowInTaskbar   = false;
				//FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				//StartPosition   = FormStartPosition.Manual;
			}
			
			void BrowserNavigateCancel(object sender, BrowserNavigateEventArgs e)
			{
				e.Cancel = true;
			}
			
			protected  void OnDeactivate(EventArgs e)
			{
				//Close();
			}
			
			protected  bool ProcessDialogKey()
			{
				
				//if (keyData == Keys.Escape) {
				//	Close();
				//	return true;
				//}
				//return base.ProcessDialogKey(keyData);
				
				//return false;
			}
			*/
		}
	}
	
	public class SplitTextEditor : AbstractMenuCommand
	{
		public override void Run()
		{
			Console.WriteLine ("Not implemented in the new Editor");
			/*
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			if (textEditorControl != null) {
				//textEditorControl.Split();
			}
			*/
		}
	}

}

