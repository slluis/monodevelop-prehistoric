// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using MonoDevelop.Gui;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
{
	public class FileDocumentInformation: IDocumentInformation
	{
		string fileName;
		int currentOffset;
		ForwardTextFileIterator iterator;
		
		public FileDocumentInformation(string fileName, int currentOffset)
		{
			this.fileName      = fileName;
			this.currentOffset = currentOffset;
		}
		
		public SourceEditorBuffer TextBuffer {
			get {
				return null;
			}
			set {
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public int CurrentOffset {
			get {
				return currentOffset;
			}
			set {
				currentOffset = value;
			}
		}
		
		public int EndOffset {
			get {
				return currentOffset;
			}
		}
		
		public ITextIterator GetTextIterator ()
		{
			if (iterator == null)
				iterator = new ForwardTextFileIterator (fileName);
			return iterator;
		}
	}
}
