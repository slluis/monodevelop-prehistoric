// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Pads;

using MonoDevelop.SourceEditor.Gui;

using Gtk;

namespace ICSharpCode.TextEditor.Document
{
	public class SearchReplaceInFilesManager
	{
		public static ReplaceInFilesDialog ReplaceDialog;

		static IFind find                  = new DefaultFind();
		static SearchOptions searchOptions = new SearchOptions("SharpDevelop.SearchAndReplace.SearchAndReplaceInFilesProperties");
		
		static PropertyService      propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		static string              currentFileName = String.Empty;
		static SourceEditor        currentDocument = null;
		
		public static SearchOptions SearchOptions {
			get {
				return searchOptions;
			}
		}
		
		static SearchReplaceInFilesManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
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
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			
			// check if the current document is up to date
			if (currentFileName != result.FileName) {
				// if not, create new document
				currentFileName = result.FileName;
				currentDocument = result.CreateDocument(); 
			}
			
			// get line out of the document and display it in the task list
			//int lineNumber = currentDocument.GetLineNumberForOffset(Math.Min(currentDocument.TextLength, result.Offset));
			TextIter resultIter = currentDocument.Buffer.GetIterAtOffset (result.Offset);
			int lineNumber = resultIter.Line;

			TextIter start_line = resultIter, end_line = resultIter;
			start_line.LineOffset = 0;
			end_line.ForwardToLineEnd ();
			//LineSegment line = currentDocument.GetLineSegment(lineNumber);
			taskService.Tasks.Add(new Task(result.FileName, currentDocument.Buffer.GetText(start_line.Offset, end_line.Offset - start_line.Offset), resultIter.LineOffset, lineNumber));
		}
		
		static bool InitializeSearchInFiles()
		{
			Debug.Assert(searchOptions != null);
			
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			taskService.Tasks.Clear();
			
			InitializeDocumentIterator(null, null);
			InitializeSearchStrategy(null, null);
			find.Reset();
			find.SearchStrategy.CompilePattern(searchOptions);
			
			currentFileName = String.Empty;
			currentDocument = null;
			
			return true;
		}
		
		static void FinishSearchInFiles()
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			taskService.NotifyTaskChange();
			
			OpenTaskView taskView = WorkbenchSingleton.Workbench.GetPad(typeof(OpenTaskView)) as OpenTaskView;
			if (taskView != null) taskView.BringToFront();
		}
		
		public static void ReplaceAll()
		{
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			while (true) {
				ISearchResult result = find.FindNext(searchOptions);
				if (result == null) {
					break;
				}
				
				find.Replace(result.Offset, 
				             result.Length, 
				             result.TransformReplacePattern(SearchOptions.ReplacePattern));
				
				DisplaySearchResult(result);
			}
			
			FinishSearchInFiles();
		}
		
		public static void FindAll()
		{
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			while (true) {
				ISearchResult result = find.FindNext(searchOptions);
				if (result == null) {
					break;
				}
				
				DisplaySearchResult(result);
			}
			
			FinishSearchInFiles();
		}
	}
}
