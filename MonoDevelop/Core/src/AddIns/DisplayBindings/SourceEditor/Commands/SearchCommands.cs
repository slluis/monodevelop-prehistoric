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
using MonoDevelop.Commands;

namespace MonoDevelop.Commands
{
	public enum SearchCommands
	{
		Find,
		FindNext,
		FindPrevious,
		Replace,
		FindInFiles,
		FindSelection,
		FindBox,
		ReplaceInFiles
	}

	public class FindInFilesHandler : CommandHandler
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

		protected override void Run ()
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
	
	public class ReplaceInFilesHandler : CommandHandler
	{
		protected override void Run()
		{
			FindInFilesHandler.SetSearchPattern ();
			
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
	
}
