// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text.RegularExpressions;

using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Undo;

namespace MonoDevelop.TextEditor.Document
{
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
		
		public ISearchResult FindNext(ITextIterator textIterator, SearchOptions options)
		{
			if (!textIterator.MoveAhead(1)) return null;
			
			int pos = textIterator.Position;
			string document = textIterator.ReadToEnd ();
			textIterator.Position = pos;
			
			Match m = regex.Match (document, 0);
			if (m == null || m.Index <= 0 || m.Length <= 0) {
				return null;
			} else {
				if (textIterator.MoveAhead (m.Index)) {
					return new DefaultSearchResult (textIterator.Position, m.Length);
				} else {
					return null;
				}
			}
		}
	}
}
