// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using MonoDevelop.Gui;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Dialogs;

namespace MonoDevelop.Gui.Search
{
	public enum DocumentIteratorType {
		None,
		CurrentDocument,
		AllOpenFiles,
		WholeCombine,
		Directory // only used for search in files
	}
	
	public enum SearchStrategyType {
		None,
		Normal,
		RegEx,
		Wildcard
	}
	
	public class SearchReplaceManager
	{
		public static ReplaceDialog ReplaceDialog     = null;

		static IFind find                  = new DefaultFind();
		static SearchOptions searchOptions = new SearchOptions("SharpDevelop.SearchAndReplace.SearchAndReplaceProperties");

		public static SearchOptions SearchOptions {
			get {
				return searchOptions;
			}
		}
		
		static SearchReplaceManager()
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
		
		// TODO: Transform Replace Pattern
		public static void Replace()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				IEditable editable = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent as IEditable;
				if (editable.SelectedText == SearchOptions.SearchPattern.ToLower ()) {
					editable.SelectedText = SearchOptions.ReplacePattern;
				}
			}
			FindNext();
		}
		
		public static void MarkAll()
		{
			find.Reset();
			try {
				find.SearchStrategy.CompilePattern(searchOptions);
			} catch {
				Runtime.MessageService.ShowMessage (GettextCatalog.GetString ("Search pattern is invalid"), DialogPointer);
				return;
			}
			while (true) {
				ISearchResult result = SearchReplaceManager.find.FindNext(searchOptions);
				
				if (result == null) {
					Runtime.MessageService.ShowMessage(GettextCatalog.GetString ("Mark all completed"), DialogPointer ); 
					find.Reset();
					return;
				} else {
					IBookmarkBuffer textArea = OpenView (result.FileName) as IBookmarkBuffer; 
					if (textArea != null) {
						object pos = textArea.GetPositionFromOffset (result.Offset);
						textArea.SetBookmarked (pos, true);
					}
				}
			}
		}
		
		public static void ReplaceAll()
		{
			find.Reset();
			try {
				find.SearchStrategy.CompilePattern(searchOptions);
			} catch {
				Runtime.MessageService.ShowMessage (GettextCatalog.GetString ("Search pattern is invalid"), DialogPointer);
				return;
			}
			
			while (true) {
				ISearchResult result = SearchReplaceManager.find.FindNext(SearchReplaceManager.searchOptions);
				
				if (result == null) {
					Runtime.MessageService.ShowMessage( string.Format (GettextCatalog.GetString ("Replace all finished. {0} matches found."), find.MatchCount), DialogPointer );
					find.Reset();
					return;
				} else {
					string transformedPattern = result.TransformReplacePattern(SearchOptions.ReplacePattern);
					find.Replace (result, transformedPattern);
				}
			}
		}
		
		static ISearchResult lastResult = null;
		public static void FindNext()
		{
			if (find == null || 
			    searchOptions.SearchPattern == null || 
			    searchOptions.SearchPattern.Length == 0) {
				return;
			}
			
			// Restart the search if the file or cursor position has changed
			// since the last FindNext.

			if (lastResult != null) {
				if (find.DocumentIterator.CurrentFileName != lastResult.FileName)
					find.Reset ();
				else {
					ITextBuffer textArea = OpenView (lastResult.FileName) as ITextBuffer;
					if (textArea == null || (lastResult != null && textArea.GetOffsetFromPosition (textArea.CursorPosition) != lastResult.Offset + lastResult.Length)) {
						find.Reset();
					}
				}
			}
			else
				find.Reset ();
				
			try {
				find.SearchStrategy.CompilePattern(searchOptions);
			} catch {
				Runtime.MessageService.ShowMessage (GettextCatalog.GetString ("Search pattern is invalid"), DialogPointer);
				return;
			}

			ISearchResult result = find.FindNext(searchOptions);
			
			if (result == null) {
				Runtime.MessageService.ShowMessage(GettextCatalog.GetString ("Search string not Found:") + "\n" + SearchOptions.SearchPattern, DialogPointer ); 
				find.Reset();
			} else {
				ITextBuffer textArea = OpenView (result.FileName) as ITextBuffer;
				if (textArea != null) {
					Console.WriteLine ("textArea.Text:" + (textArea.Text==null));
					int startPos = Math.Min (textArea.Text.Length, Math.Max(0, result.Offset));
					int endPos   = Math.Min (textArea.Text.Length, startPos + result.Length);
					
					textArea.ShowPosition (textArea.GetPositionFromOffset (endPos));
					textArea.Select (textArea.GetPositionFromOffset (endPos), textArea.GetPositionFromOffset (startPos));
				}
			}
			
			lastResult = result;
		}
		
		static object OpenView (string fileName) 
		{
			Runtime.FileService.OpenFile (fileName).WaitForCompleted ();
			return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent;
		}
		
		public static Gtk.Dialog DialogPointer 
		{
			get {
				if ( ReplaceDialog != null ) { 
					return ReplaceDialog.DialogPointer;
				}
				return null;
			}
		}
		
	}	
}
