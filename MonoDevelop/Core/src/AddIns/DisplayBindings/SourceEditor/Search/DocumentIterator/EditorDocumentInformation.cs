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
using SourceEditor_ = MonoDevelop.SourceEditor.Gui.SourceEditor;

namespace MonoDevelop.TextEditor.Document
{
	public class EditorDocumentInformation: IDocumentInformation
	{
		SourceEditor_        document;
		SourceEditorBuffer  textBuffer;
		string              fileName;
		int                 currentOffset;
		
		public EditorDocumentInformation (SourceEditor_ document, string fileName)
		{
			this.document   = document;
			this.textBuffer = document.Buffer;
			this.fileName   = fileName;
//			this.currentOffset = document.Caret.Offset;
		}
		
		public SourceEditorBuffer TextBuffer {
			get {
				return textBuffer;
			}
			set {
				textBuffer = value;
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
//				if (document != null) {
//					document.Caret.Offset = value;
//				} else {
					currentOffset = value;
//				}
			}
		}
		
		public int EndOffset {
			get {
				//if (document != null) {
				//	return SearchReplaceUtilities.CalcCurrentOffset(document);
				//}
				return currentOffset;
			}
		}
		
		internal void Replace (int offset, int length, string pattern)
		{
			document.Replace(offset, length, pattern);
			
			if (offset <= CurrentOffset) {
				CurrentOffset = CurrentOffset - length + pattern.Length;
			}
		}
		
		public ITextIterator GetTextIterator ()
		{
			int startOffset = document.Buffer.GetIterAtMark (document.Buffer.InsertMark).Offset;
			return new ForwardTextIterator (this, startOffset);
		}
	}
}
