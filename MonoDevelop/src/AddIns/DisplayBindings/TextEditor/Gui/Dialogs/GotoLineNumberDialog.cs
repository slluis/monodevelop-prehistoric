// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Resources;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.TextEditor;

using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class GotoLineNumberDialog : Dialog
	{
		public static bool IsVisible = false;
	
		Entry gotoEntry;
		
		public GotoLineNumberDialog ()
		{
			this.Title = "Goto Line Number...";
			this.BorderWidth = 6;
			this.HasSeparator = false;
			
			VBox mainbox = new VBox (false, 2);
			HBox entrybox = new HBox (false, 2);

			ResourceService rs = (ResourceService)ServiceManager.Services.GetService (typeof (IResourceService));
			string text = rs.GetString ("Dialog.GotoLineNumber.label1Text");
			Label label = new Label (text.Replace ("&", ""));

			gotoEntry = new Entry ();

			entrybox.PackStart (label, false, false, 2);
			entrybox.PackStart (gotoEntry, false, true, 2);
			
			Button okButton = new Button (Stock.JumpTo);
			okButton.Clicked += new EventHandler (closeEvent);

			Button cancelButton = new Button (Stock.Cancel);
			cancelButton.Clicked += new EventHandler (cancelEvent);

			this.ActionArea.PackStart (cancelButton);
			this.ActionArea.PackStart (okButton);

			mainbox.PackStart (entrybox);
			
			this.VBox.PackStart (mainbox);
			this.ShowAll ();
		}
		
		void cancelEvent(object sender, EventArgs e)
		{
			this.Hide();
		}
		
		void closeEvent(object sender, EventArgs e)
		{
			try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				
				if (window != null && window.ViewContent is ITextEditorControlProvider) {
					TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
				
					int i = Math.Min(textarea.Document.TotalNumberOfLines, Math.Max(1, Int32.Parse(gotoEntry.Text)));
					textarea.ActiveTextAreaControl.Caret.Line = i - 1;
					textarea.ActiveTextAreaControl.ScrollToCaret();
				}
			} catch (Exception) {
				
			} finally {
				this.Hide();
			}
		}
	}
}
