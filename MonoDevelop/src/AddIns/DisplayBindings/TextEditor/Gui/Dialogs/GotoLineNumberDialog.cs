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
using Glade;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class GotoLineNumberDialog
	{
		public static bool IsVisible = false;
	
		[Widget] Dialog GotoLineDialog;
		[Widget] Entry line_number_entry;
		
		public GotoLineNumberDialog ()
		{
			new Glade.XML (null, "texteditoraddin.glade", "GotoLineDialog", null).Autoconnect (this);
			
			GotoLineDialog.ShowAll ();
		}
		
		public void Run ()
		{
			GotoLineDialog.Run ();
		}
		
		public void Hide ()
		{
			GotoLineDialog.Hide ();
		}
		
		void on_btn_close_clicked (object sender, EventArgs e)
		{
			GotoLineDialog.Hide ();
		}
		
		void on_btn_go_to_line_clicked (object sender, EventArgs e)
		{
			try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				
				if (window != null && window.ViewContent is ITextEditorControlProvider) {
					TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
				
					int i = Math.Min(textarea.Document.TotalNumberOfLines, Math.Max(1, Int32.Parse(line_number_entry.Text)));
					textarea.ActiveTextAreaControl.Caret.Line = i - 1;
					textarea.ActiveTextAreaControl.ScrollToCaret();
				}
			} catch (Exception) {
				
			} finally {
				GotoLineDialog.Hide ();
			}
		}
	}
}
