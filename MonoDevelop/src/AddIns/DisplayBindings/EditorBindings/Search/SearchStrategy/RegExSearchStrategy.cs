// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text.RegularExpressions;

using ICSharpCode.Core.Properties;

namespace MonoDevelop.EditorBindings.Search {
	public class RegExSearchStrategy : ISearchStrategy
	{
		Regex regex = null;
		
		public void CompilePattern(SearchOptions options)
		{
			RegexOptions regexOptions = RegexOptions.Compiled;
			if (options.IgnoreCase) {
				regexOptions |= RegexOptions.IgnoreCase;
			}
			regex = new Regex(options.SearchPattern, regexOptions);
		}
		
		public SearchResult FindNext(ITextIterator textIterator, SearchOptions options)
		{
			string document = textIterator.FullDocumentText;
			
			while (textIterator.MoveAhead(1)) {
				Match m = regex.Match(document, textIterator.Position);
				if (m == null || m.Index <= 0 || m.Length <= 0) {
					
				} else {
					int delta = m.Index - textIterator.Position;
					if (delta <= 0 || textIterator.MoveAhead(delta)) {
						return new SearchResult (m.Index, m.Length);
					} else {
						return null;
					}
				}
			}
			
			return null;
		}
	}
}
