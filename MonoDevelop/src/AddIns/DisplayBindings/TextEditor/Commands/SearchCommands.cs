// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class Find : AbstractMenuCommand
	{
		public static void SetSearchPattern()
		{
//			// Get Highlighted value and set it to FindDialog.searchPattern
//			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
//			
//			if (window != null && (window.ViewContent is ITextEditorControlProvider)) {
//				TextAreaControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextAreaControl;				
//				string selectedText = textarea.Document.SelectedText;
//				if (selectedText != null && selectedText.Length > 0) {
//					SearchReplaceManager.SearchOptions.SearchPattern = selectedText;
//				}
//			}
		}
		
		public override void Run()
		{
			SetSearchPattern();
			if (SearchReplaceManager.ReplaceDialog != null) {
				if (SearchReplaceManager.ReplaceDialog.replaceMode == false) {
					SearchReplaceManager.ReplaceDialog.SetSearchPattern(SearchReplaceManager.SearchOptions.SearchPattern);
					SearchReplaceManager.ReplaceDialog.Present ();
				} else {
					SearchReplaceManager.ReplaceDialog.Destroy ();
					ReplaceDialog rd = new ReplaceDialog (false);
					rd.ShowAll ();
				}
			} else {
				ReplaceDialog rd = new ReplaceDialog(false);
				rd.ShowAll();
			}
		}
	}
	
	public class FindNext : AbstractMenuCommand
	{
		public override void Run()
		{
			SearchReplaceManager.FindNext();
		}
	}
	
	public class Replace : AbstractMenuCommand
	{
		public override void Run()
		{ 
			Find.SetSearchPattern();
			
			if (SearchReplaceManager.ReplaceDialog != null) {
				if (SearchReplaceManager.ReplaceDialog.replaceMode == true) {
					SearchReplaceManager.ReplaceDialog.SetSearchPattern(SearchReplaceManager.SearchOptions.SearchPattern);
					SearchReplaceManager.ReplaceDialog.Present ();
				} else {
					SearchReplaceManager.ReplaceDialog.Destroy ();
					ReplaceDialog rd = new ReplaceDialog (true);
					rd.ShowAll ();
				}
			} else {
				ReplaceDialog rd = new ReplaceDialog(true);
				rd.ShowAll();
			}
		}
	}
	
	public class FindInFiles : AbstractMenuCommand
	{
		public static void SetSearchPattern()
		{
//			// Get Highlighted value and set it to FindDialog.searchPattern
//			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
//			
//			if (window != null && (window.ViewContent is ITextEditorControlProvider)) {
//				TextAreaControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextAreaControl;				
//				string selectedText = textarea.Document.SelectedText;
//				if (selectedText != null && selectedText.Length > 0) {
//					SearchReplaceInFilesManager.SearchOptions.SearchPattern = selectedText;
//				}
//			}			
		}
		public override void Run()
		{
			SetSearchPattern();
			//using (ReplaceInFilesDialog rd = new ReplaceInFilesDialog(false)) {
			//	rd.Owner = (Form)WorkbenchSingleton.Workbench;
			//	rd.ShowDialog();
			//}
		}
	}
	
	public class ReplaceInFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			FindInFiles.SetSearchPattern();
			
			//using (ReplaceInFilesDialog rd = new ReplaceInFilesDialog(true)) {
			//	rd.Owner = (Form)WorkbenchSingleton.Workbench;
			//	rd.ShowDialog();
			//}
		}
	}
	
	public class GotoLineNumber : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!GotoLineNumberDialog.IsVisible) {
				GotoLineNumberDialog gnd = new GotoLineNumberDialog();
				gnd.Run ();
				gnd.Hide ();
			}
		}
	}
	
	public class GotoMatchingBrace : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoMatchingBrace();
			}
		}
	}
}
