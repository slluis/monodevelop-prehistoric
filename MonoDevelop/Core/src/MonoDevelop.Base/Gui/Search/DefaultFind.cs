// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Diagnostics;

namespace MonoDevelop.Gui.Search
{
	public class DefaultFind : IFind
	{
		ISearchStrategy             searchStrategy      = null;
		IDocumentIterator           documentIterator    = null;
		ITextIterator               textIterator        = null;
		IDocumentInformation        info = null;
		bool						cancelled;
		int							searchedFiles;
		int							matches;
		
		public IDocumentInformation CurrentDocumentInformation {
			get {
				return info;
			}
		}
		
		public ITextIterator TextIterator {
			get {
				return textIterator;
			}
		}
		
		public ISearchStrategy SearchStrategy {
			get {
				return searchStrategy;
			}
			set {
				searchStrategy = value;
			}
		}
		
		public IDocumentIterator DocumentIterator {
			get {
				return documentIterator;
			}
			set {
				documentIterator = value;
			}
		}
		
		public int SearchedFileCount {
			get { return searchedFiles; }
		}
		
		public int MatchCount {
			get { return matches; }
		}
		
		public void Reset()
		{
			documentIterator.Reset();
			textIterator = null;
			cancelled = false;
			searchedFiles = 0;
			matches = 0;
		}
		
		public void Replace (ISearchResult result, string pattern)
		{
			if (CurrentDocumentInformation != null && TextIterator != null) {
				TextIterator.Position = result.Offset;
				TextIterator.Replace (result.Length, pattern);
			}
		}
		
		public ISearchResult FindNext(SearchOptions options) 
		{
			// insanity check
			Debug.Assert(searchStrategy      != null);
			Debug.Assert(documentIterator    != null);
			Debug.Assert(options             != null);
			
			while (!cancelled)
			{
				if (info != null && textIterator != null && documentIterator.CurrentFileName != null) {
					if (info.FileName != documentIterator.CurrentFileName) { // create new iterator, if document changed
						info         = documentIterator.Current;
						textIterator = info.GetTextIterator ();
					} 

					ISearchResult result = searchStrategy.FindNext (textIterator, options);
					if (result != null) {
						matches++;
						return result;
					}
				}
				
				if (textIterator != null) textIterator.Close ();
					
				// not found or first start -> move forward to the next document
				if (documentIterator.MoveForward()) {
					searchedFiles++;
					info = documentIterator.Current;
					textIterator = info.GetTextIterator ();
				}
				else
					cancelled = true;
			}
			cancelled = false;
			return null;
		}
		
		public void Cancel ()
		{
			cancelled = true;
		}
	}
}
