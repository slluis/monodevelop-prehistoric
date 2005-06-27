// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace MonoDevelop.Gui.Search
{
	public class DefaultSearchResult : ISearchResult
	{
		IDocumentInformation documentInformation;
		int offset;
		int length;
		int line;
		int column;
		
		public DefaultSearchResult (ITextIterator iter, int length)
		{
			offset = iter.Position;
			line = iter.Line;
			column = iter.Column;
			this.length = length;
			documentInformation = iter.DocumentInformation;
		}
		
		public string FileName {
			get {
				return documentInformation.FileName;
			}
		}
		
		public IDocumentInformation DocumentInformation {
			get {
				return documentInformation;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
		}
		
		public int Length {
			get {
				return length;
			}
		}
		
		public int Line {
			get { return line; }
		}
		
		public int Column {
			get { return column; }
		}
		
		public virtual string TransformReplacePattern (string pattern)
		{
			return pattern;
		}
		
		public override string ToString()
		{
			return String.Format("[DefaultLocation: FileName={0}, Offset={1}, Length={2}]",
			                     FileName,
			                     Offset,
			                     Length);
		}
	}
}
