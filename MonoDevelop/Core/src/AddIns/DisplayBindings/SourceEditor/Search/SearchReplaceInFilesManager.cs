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
		
		static PropertyService      propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
		
		static string              currentFileName = String.Empty;
		static SourceEditorBuffer  currentDocument = null;
		static DateTime timer;
		static bool searching;
		static bool cancelled;
		
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
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			
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
					taskService.AddTask (new Task(result.FileName, msg, -1, -1));
					return;
				}
			}
			
			// get line out of the document and display it in the task list
			TextIter resultIter = currentDocument.GetIterAtOffset (result.Offset);
			int lineNumber = resultIter.Line;

			TextIter start_line = resultIter, end_line = resultIter;
			start_line.LineOffset = 0;
			end_line.ForwardToLineEnd ();
			taskService.AddTask (new Task(result.FileName, currentDocument.GetText(start_line.Offset, end_line.Offset - start_line.Offset), resultIter.LineOffset, lineNumber));
		}
		
		static void DisplaySearchResultCallback (object data)
		{
			DisplaySearchResult ((ISearchResult) data);
		}
		
		static bool InitializeSearchInFiles()
		{
			Debug.Assert(searchOptions != null);
			
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			taskService.ClearTasks();
			
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
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			string msg;
			if (cancelled) msg = GettextCatalog.GetString ("Search cancelled.");
			else msg = string.Format (GettextCatalog.GetString ("Search completed. {0} matches found in {1} files."), find.MatchCount, find.SearchedFileCount);
			taskService.AddTask (new Task(null, msg, -1, -1));
			
			// present the taskview to show the search results
			OpenTaskView taskView = WorkbenchSingleton.Workbench.GetPad(typeof(OpenTaskView)) as OpenTaskView;
			if (taskView != null) {
				taskView.BringToFront();
			}
			
			Console.WriteLine ("Search time: " + (DateTime.Now - timer).TotalSeconds);
			searching = false;
			
			// tell the user search is done.
/*			MessageService MessageService = (MessageService)ServiceManager.GetService (typeof (MessageService));
			MessageService.ShowMessage (GettextCatalog.GetString ("Search completed"));
			Console.WriteLine ("Done");
*/		}
		
		public static void ReplaceAll()
		{
			if (searching) {
				MessageService MessageService = (MessageService)ServiceManager.GetService (typeof (MessageService));
				if (!MessageService.AskQuestion (GettextCatalog.GetString ("There is a search already in progress. Do you want to cancel it?")))
					return;
				CancelSearch ();
			}
			
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			string msg = string.Format (GettextCatalog.GetString ("Replacing '{0}' in {1}."), searchOptions.SearchPattern, searchOptions.SearchDirectory);
			taskService.AddTask (new Task(null, msg, -1, -1));
			
			timer = DateTime.Now;
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.BackgroundDispatch (new MessageHandler(ReplaceAllThread));
		}
		
		public static void ReplaceAllThread()
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			searching = true;
			
			while (!cancelled) {
				ISearchResult result = find.FindNext(searchOptions);
				if (result == null) {
					break;
				}
				
				find.Replace(result, result.TransformReplacePattern(SearchOptions.ReplacePattern));
				
				dispatcher.GuiDispatch (new StatefulMessageHandler (DisplaySearchResultCallback), result);
			}
			
			dispatcher.GuiDispatch (new MessageHandler (FinishSearchInFiles));
		}
		
		public static void FindAll()
		{
			if (searching) {
				MessageService MessageService = (MessageService)ServiceManager.GetService (typeof (MessageService));
				if (!MessageService.AskQuestion (GettextCatalog.GetString ("There is a search already in progress. Do you want to cancel it?")))
					return;
				CancelSearch ();
			}
			
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			string msg = string.Format (GettextCatalog.GetString ("Looking for '{0}' in {1}."), searchOptions.SearchPattern, searchOptions.SearchDirectory);
			taskService.AddTask (new Task(null, msg, -1, -1));
			
			timer = DateTime.Now;
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.BackgroundDispatch (new MessageHandler(FindAllThread));
		}
		
		public static void FindAllThread()
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			searching = true;
			
			while (!cancelled) {
				ISearchResult result = find.FindNext (searchOptions);
				if (result == null) {
					break;
				}

				dispatcher.GuiDispatch (new StatefulMessageHandler (DisplaySearchResultCallback), result);
			}
			
			dispatcher.GuiDispatch (new MessageHandler (FinishSearchInFiles));
		}
		
		static void CancelSearch ()
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
	}
}
