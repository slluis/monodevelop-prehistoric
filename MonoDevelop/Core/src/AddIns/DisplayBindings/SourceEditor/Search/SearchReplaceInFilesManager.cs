// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

using MonoDevelop.Gui;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads;

using MonoDevelop.SourceEditor.Gui;

using Gtk;

namespace MonoDevelop.TextEditor.Document
{
	public class SearchReplaceInFilesManager
	{
		public static ReplaceInFilesDialog ReplaceDialog;

		static IFind find                  = new DefaultFind();
		static SearchOptions searchOptions = new SearchOptions("SharpDevelop.SearchAndReplace.SearchAndReplaceInFilesProperties");
		
		static string              currentFileName = String.Empty;
		static SourceEditorBuffer  currentDocument = null;
		static DateTime timer;
		static bool searching;
		static bool cancelled;
		static string searchError;
		
		public static SearchOptions SearchOptions {
			get {
				return searchOptions;
			}
		}
		
		static SearchReplaceInFilesManager()
		{
			searchOptions.SearchStrategyTypeChanged   += new EventHandler(InitializeSearchStrategy);
			searchOptions.DocumentIteratorTypeChanged += new EventHandler(InitializeDocumentIterator);
			InitializeDocumentIterator(null, null);
			InitializeSearchStrategy(null, null);
		}
		
		static void InitializeSearchStrategy(object sender, EventArgs e)
		{
			find.SearchStrategy = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
		}
		
		static void InitializeDocumentIterator(object sender, EventArgs e)
		{
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType);
		}
		
		/// <remarks>
		/// This method displays the search result in the task view
		/// </remarks>
		static void DisplaySearchResult(ISearchResult result)
		{
			// check if the current document is up to date
			if (currentFileName != result.FileName || currentDocument == null) {
				// if not, create new document
				currentFileName = result.FileName;
				try {
					currentDocument = SourceEditorBuffer.CreateTextBufferFromFile (result.FileName);
				}
				catch
				{
					string msg = string.Format (GettextCatalog.GetString ("Match at offset {0}"), result.Offset);
					Runtime.TaskService.AddTask (new Task(result.FileName, msg, -1, -1));
					return;
				}
			}
			
			// get line out of the document and display it in the task list
			TextIter resultIter = currentDocument.GetIterAtOffset (result.Offset);
			int lineNumber = resultIter.Line;

			TextIter start_line = resultIter, end_line = resultIter;
			start_line.LineOffset = 0;
			end_line.ForwardToLineEnd ();
			Runtime.TaskService.AddTask (new Task (result.FileName, currentDocument.GetText(start_line.Offset, end_line.Offset - start_line.Offset), resultIter.LineOffset, lineNumber));
		}
		
		static bool InitializeSearchInFiles()
		{
			Debug.Assert(searchOptions != null);
			
			Runtime.TaskService.ClearTasks();
			
			InitializeDocumentIterator(null, null);
			InitializeSearchStrategy(null, null);
			find.Reset();
			find.SearchStrategy.CompilePattern(searchOptions);
			
			currentFileName = String.Empty;
			currentDocument = null;
			
			return true;
		}
		
		static void FinishSearchInFiles ()
		{
			string msg;
			if (searchError != null)
				msg = string.Format (GettextCatalog.GetString ("The search could not be finished: {0}"), searchError);
			else if (cancelled)
				msg = GettextCatalog.GetString ("Search cancelled.");
			else
				msg = string.Format (GettextCatalog.GetString ("Search completed. {0} matches found in {1} files."), find.MatchCount, find.SearchedFileCount);
				
			Runtime.TaskService.AddTask (new Task(null, msg, -1, -1));
			Runtime.TaskService.ShowTasks ();

			Console.WriteLine ("Search time: " + (DateTime.Now - timer).TotalSeconds);
			searching = false;
			
			// tell the user search is done.
/*			Runtime.MessageService.ShowMessage (GettextCatalog.GetString ("Search completed"));
			Console.WriteLine ("Done");
*/		}
		
		public static void ReplaceAll()
		{
			if (searching) {
				if (!Runtime.MessageService.AskQuestion (GettextCatalog.GetString ("There is a search already in progress. Do you want to cancel it?")))
					return;
				CancelSearch ();
			}
			
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			string msg = string.Format (GettextCatalog.GetString ("Replacing '{0}' in {1}."), searchOptions.SearchPattern, searchOptions.SearchDirectory);
			Runtime.TaskService.AddTask (new Task(null, msg, -1, -1));
			
			timer = DateTime.Now;
			Runtime.DispatchService.BackgroundDispatch (new MessageHandler(ReplaceAllThread));
		}
		
		public static void ReplaceAllThread()
		{
			searching = true;
			searchError = null;
			
			while (!cancelled) 
			{
				try
				{
					ISearchResult result = find.FindNext(searchOptions);
					if (result == null) {
						break;
					}
					
					find.Replace(result, result.TransformReplacePattern(SearchOptions.ReplacePattern));
					DisplaySearchResult (result);
				}
				catch (Exception ex) 
				{
					searchError = ex.Message;
					break;
				}
			}
			
			FinishSearchInFiles ();
		}
		
		public static void FindAll()
		{
			if (searching) {
				if (!Runtime.MessageService.AskQuestion (GettextCatalog.GetString ("There is a search already in progress. Do you want to cancel it?")))
					return;
				CancelSearch ();
			}
			
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			string msg = string.Format (GettextCatalog.GetString ("Looking for '{0}' in {1}."), searchOptions.SearchPattern, searchOptions.SearchDirectory);
			Runtime.TaskService.AddTask (new Task(null, msg, -1, -1));
			
			timer = DateTime.Now;
			Runtime.DispatchService.BackgroundDispatch (new MessageHandler(FindAllThread));
		}
		
		public static void FindAllThread()
		{
			searching = true;
			searchError = null;
			
			while (!cancelled) 
			{
				try
				{
					ISearchResult result = find.FindNext (searchOptions);
					if (result == null) {
						break;
					}

					DisplaySearchResult (result);
				}
				catch (Exception ex)
				{
					searchError = ex.Message;
					break;
				}
			}
			
			FinishSearchInFiles ();
		}
		
		public static void CancelSearch ()
		{
			if (!searching) return;
			cancelled = true;
			find.Cancel ();
			
			while (searching) {
				if (Gtk.Application.EventsPending ())
					Gtk.Application.RunIteration ();
				Thread.Sleep (10);
			}
				
			cancelled = false;
		}

		public static Gtk.Dialog DialogPointer
		{
			get {
				if ( ReplaceDialog != null ){ 
					return ReplaceDialog.DialogPointer;
				}
				return null;
			}
		}
		
	}
}
