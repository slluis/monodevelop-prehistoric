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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.TextEditor;
using MonoDevelop.TextEditor.Actions;
using MonoDevelop.TextEditor.Document;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui;
using MonoDevelop.SourceEditor.Gui;
using SourceEditor_ = MonoDevelop.SourceEditor.Gui.SourceEditor;

namespace MonoDevelop.DefaultEditor.Commands
{
	
	public abstract class AbstractEditActionMenuCommand : AbstractMenuCommand
	{
		public abstract IEditAction EditAction
		{
			get;
		}
		
		public override void Run()
		{
			Console.WriteLine ("Not implemented in the new editor");
			/*IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditor = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			EditAction.Execute(textEditor.ActiveTextAreaControl.TextArea);*/
		}
	}
	
	public class Find : AbstractMenuCommand
	{
		public static void SetSearchPattern ()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

			if (window != null && window.ViewContent is SourceEditorDisplayBindingWrapper)
			{
				SourceEditor_ editor = ((SourceEditorDisplayBindingWrapper)window.ViewContent).Editor;
				string selectedText = editor.Buffer.GetSelectedText ();
				
				if (selectedText != null && selectedText.Length > 0)
					SearchReplaceManager.SearchOptions.SearchPattern = selectedText.Split ('\n')[0];
			}
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
				ReplaceDialog rd = new ReplaceDialog (false);
				rd.ShowAll();
			}
		}
	}
	
	public class FindNext : AbstractMenuCommand
	{
		public override void Run ()
		{
			SearchReplaceManager.FindNext ();
		}
	}
	
	public class Replace : AbstractMenuCommand
	{
		public override void Run ()
		{ 
			Find.SetSearchPattern ();
			
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
		public static void SetSearchPattern ()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null && window.ViewContent is SourceEditorDisplayBindingWrapper)
			{
				SourceEditor_ editor = ((SourceEditorDisplayBindingWrapper)window.ViewContent).Editor;
				string selectedText = editor.Buffer.GetSelectedText ();
				if (selectedText != null && selectedText.Length > 0)
					SearchReplaceInFilesManager.SearchOptions.SearchPattern = selectedText.Split ('\n')[0];
			}
		}

		
				
								
		public override void Run ()
		{
			SetSearchPattern ();
			if (SearchReplaceInFilesManager.ReplaceDialog != null) {
				if (SearchReplaceInFilesManager.ReplaceDialog.replaceMode == false) {
					SearchReplaceInFilesManager.ReplaceDialog.SetSearchPattern(SearchReplaceInFilesManager.SearchOptions.SearchPattern);
					SearchReplaceInFilesManager.ReplaceDialog.Present ();
				} else {
					SearchReplaceInFilesManager.ReplaceDialog.Destroy ();
					ReplaceInFilesDialog rd = new ReplaceInFilesDialog (false);
					rd.ShowAll ();
				}
			} else {
				ReplaceInFilesDialog rd = new ReplaceInFilesDialog(false);
				rd.ShowAll();
			}
		}
	}
	
	public class ReplaceInFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			FindInFiles.SetSearchPattern ();
			
			if (SearchReplaceInFilesManager.ReplaceDialog != null) {
				if (SearchReplaceInFilesManager.ReplaceDialog.replaceMode == true) {
					SearchReplaceInFilesManager.ReplaceDialog.SetSearchPattern(SearchReplaceInFilesManager.SearchOptions.SearchPattern);
					SearchReplaceInFilesManager.ReplaceDialog.Present ();
				} else {
					SearchReplaceInFilesManager.ReplaceDialog.Destroy ();
					ReplaceInFilesDialog rd = new ReplaceInFilesDialog (true);
					rd.ShowAll ();
				}
			} else {
				ReplaceInFilesDialog rd = new ReplaceInFilesDialog (true);
				rd.ShowAll ();
			}
		}
	}
	
	public class GotoLineNumber : AbstractMenuCommand
	{
		public override void Run ()
		{
			if (!GotoLineNumberDialog.IsVisible)
				using (GotoLineNumberDialog gnd = new GotoLineNumberDialog ())
					gnd.Run ();
		}
	}
	
	public class GotoMatchingBrace : AbstractMenuCommand
	{
		public override void Run ()
		{
			IWorkbenchWindow wnd = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (wnd == null) return;
			SourceEditorDisplayBindingWrapper o = wnd.ViewContent as SourceEditorDisplayBindingWrapper;
			if (o == null) return;

			o.GotoMatchingBrace ();
		}
	}
}
