// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using ICSharpCode.Core.Properties;

namespace MonoDevelop.EditorBindings.Search {
	/// <summary>
	///  Only for fallback purposes.
	/// </summary>
	public class BruteForceSearchStrategy : ISearchStrategy
	{
		string searchPattern;
		
		bool MatchCaseSensitive(ITextIterator iter, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= iter.Length || iter.GetCharAt(offset + i) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool MatchCaseInsensitive(ITextIterator iter, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= iter.Length || Char.ToUpper(iter.GetCharAt(offset + i)) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		int InternalFindNext(ITextIterator textIterator, SearchOptions options)
		{
			while (textIterator.MoveAhead(1)) {
				if (options.IgnoreCase ? MatchCaseInsensitive(textIterator, textIterator.Position, searchPattern) :
				                         MatchCaseSensitive(textIterator, textIterator.Position, searchPattern)) {
					if (!options.SearchWholeWordOnly || textIterator.IsWholeWordAt (textIterator.Position, searchPattern.Length)) {
						return textIterator.Position;
					}
				}
			}
			return -1;
		}
		
		public void CompilePattern(SearchOptions options)
		{
			searchPattern = options.IgnoreCase ? options.SearchPattern.ToUpper() : options.SearchPattern;
		}
		
		public SearchResult FindNext(ITextIterator textIterator, SearchOptions options)
		{
			int offset = InternalFindNext (textIterator, options);
			return offset >= 0 ? new SearchResult (offset, searchPattern.Length) : null;
		}
	}
}
