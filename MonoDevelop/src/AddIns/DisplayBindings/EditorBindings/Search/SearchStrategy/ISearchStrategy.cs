// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace MonoDevelop.EditorBindings.Search {
	/// <summary>
	/// This interface is the basic interface which all 
	/// search algorithms must implement.
	/// </summary>
	public interface ISearchStrategy
	{
		/// <remarks>
		/// Only with a call to this method the search strategy must
		/// update their pattern information. This method will be called 
		/// before the FindNext function.
		/// </remarks>
		void CompilePattern (SearchOptions options);
		
		/// <remarks>
		/// The find next method should search the next occurrence of the 
		/// compiled pattern in the text using the textIterator and options.
		/// </remarks>
		SearchResult FindNext (ITextIterator textIterator, SearchOptions options);
	}
	
	public class SearchResult {
		public readonly int Position, Length;
		public SearchResult (int pos, int len)
		{
			this.Position = pos;
			this.Length = len;
		}
	}
}
