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
using MonoDevelop.TextEditor;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
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

		static MessageService MessageService = (MessageService)ServiceManager.Services.GetService (typeof (MessageService));
		
		public static SearchOptions SearchOptions {
			get {
				return searchOptions;
			}
		}
		
		static SearchReplaceManager()
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
		
		// TODO: Transform Replace Pattern
		public static void Replace()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				SourceEditor textarea = (SourceEditor) ((SourceEditorDisplayBindingWrapper)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).Control;
				string text = textarea.Buffer.GetSelectedText ();
				if (text.ToLower () == SearchOptions.SearchPattern.ToLower ()) {
					int offset = textarea.Buffer.GetLowerSelectionBounds ();
					
					((IClipboardHandler)textarea.Buffer).Delete (null, null);
					
					textarea.Buffer.Insert(offset, SearchOptions.ReplacePattern);
					textarea.Buffer.PlaceCursor (textarea.Buffer.GetIterAtOffset (offset + SearchOptions.ReplacePattern.Length));
					textarea.View.ScrollMarkOnscreen (textarea.Buffer.InsertMark);
				}
			}
			FindNext();
		}
		
		public static void MarkAll()
		{
			SourceEditor textArea = null;
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				textArea = (SourceEditor) ((SourceEditorDisplayBindingWrapper)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).Control;
				textArea.Buffer.PlaceCursor (textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark));
			}
			find.Reset();
			find.SearchStrategy.CompilePattern(searchOptions);
			while (true) {
				ISearchResult result = SearchReplaceManager.find.FindNext(searchOptions);
				
				if (result == null) {
					//MessageBox.Show((Form)WorkbenchSingleton.Workbench, "Mark all done", "Finished");
					MessageService.ShowMessage ("Mark all completed");
					find.Reset();
					return;
				} else {
					textArea = OpenTextArea(result.FileName); 
					
					Gtk.TextIter resultIter = textArea.Buffer.GetIterAtOffset (result.Offset);
					textArea.Buffer.PlaceCursor (resultIter);
					
					int lineNr = resultIter.Line;
					
					if (!textArea.Buffer.IsBookmarked(lineNr)) {
						textArea.Buffer.ToggleBookmark(lineNr);
					}
				}
			}
		}
		
		public static void ReplaceAll()
		{
			SourceEditor textArea = null;
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				textArea = (SourceEditor) ((SourceEditorDisplayBindingWrapper)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).Control;
				textArea.Buffer.PlaceCursor (textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark));
			}
			find.Reset();
			find.SearchStrategy.CompilePattern(searchOptions);
			
			while (true) {
				ISearchResult result = SearchReplaceManager.find.FindNext(SearchReplaceManager.searchOptions);
				
				if (result == null) {
					//MessageBox.Show((Form)WorkbenchSingleton.Workbench, "Replace all done", "Finished");
					MessageService.ShowMessage ("Replace all finished");
					find.Reset();
					return;
				} else {
					textArea = OpenTextArea(result.FileName); 
					textArea.Buffer.PlaceCursor (textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark));
					textArea.View.ScrollMarkOnscreen (textArea.Buffer.InsertMark);
					string transformedPattern = result.TransformReplacePattern(SearchOptions.ReplacePattern);
					find.Replace(result.Offset,
					             result.Length, 
					             transformedPattern);
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
			
			find.SearchStrategy.CompilePattern(searchOptions);
			ISearchResult result = find.FindNext(searchOptions);
			
			if (result == null) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				//FIXME: This needs to be a msg or whatever
				/*MessageBox.Show((Form)WorkbenchSingleton.Workbench,
				                resourceService.GetString("Dialog.NewProject.SearchReplace.SearchStringNotFound"),
				                "Not Found", 
				                MessageBoxButtons.OK, 
				                MessageBoxIcon.Information);*/
				MessageService.ShowMessage ("Not Found");
				find.Reset();
			} else {
				SourceEditor textArea = OpenTextArea(result.FileName);
				
				if (lastResult != null  && lastResult.FileName == result.FileName && textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark).Offset != lastResult.Offset + lastResult.Length) {
					find.Reset();
				}
				int startPos = Math.Min(textArea.Buffer.Text.Length, Math.Max(0, result.Offset));
				int endPos   = Math.Min(textArea.Buffer.Text.Length, startPos + result.Length);
														textArea.Buffer.MoveMark ("insert", textArea.Buffer.GetIterAtOffset (endPos));
				textArea.Buffer.MoveMark ("selection_bound", textArea.Buffer.GetIterAtOffset (startPos));
				textArea.View.ScrollMarkOnscreen (textArea.Buffer.InsertMark);
			}
			
			lastResult = result;
		}
		
		static SourceEditor OpenTextArea(string fileName) 
		{
			if (fileName != null) {
				IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(fileName);
			}
			
			return (SourceEditor) ((SourceEditorDisplayBindingWrapper)WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent).Control;
		}
	}	
}
