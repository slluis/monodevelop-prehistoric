// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;

using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Undo;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
{
	public class DefaultSearchResult : ISearchResult
	{
		IDocumentInformation documentInformation;
		int    offset;
		int    length;
		
		public string FileName {
			get {
				return documentInformation.FileName;
			}
		}
		
		public IDocumentInformation DocumentInformation {
			set {
				documentInformation = value;
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
		
		public virtual string TransformReplacePattern(string pattern)
		{
			return pattern;
		}
		
		public DefaultSearchResult(int offset, int length)
		{
			this.offset   = offset;
			this.length   = length;
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
