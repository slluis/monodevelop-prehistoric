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
	public class ProvidedDocumentInformation
	{
		SourceEditor        document;
		SourceEditorBuffer  textBuffer;
		string              fileName;
		int                 currentOffset;
		
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
//				if (document != null) {
//					return document.Caret.Offset;
//				}
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
		
		public void Replace(int offset, int length, string pattern)
		{
			if (document != null) {
				document.Replace(offset, length, pattern);
			} else {
				textBuffer.Replace(offset, length, pattern);
			}
			
			if (offset <= CurrentOffset) {
				CurrentOffset = CurrentOffset - length + pattern.Length;
			}
		}
		
		public void SaveBuffer()
		{
			if (document != null) {
				
			} else {
				StreamWriter streamWriter = File.CreateText(this.fileName);
				streamWriter.Write(textBuffer.GetText(0, textBuffer.Length));
				streamWriter.Close();
			}
		}
		
		public SourceEditor CreateDocument()
		{
			if (document != null) {
				return document;
			}
			
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			w.Load (fileName);
			return (SourceEditor) w.Control;	
		}		
		
		public ProvidedDocumentInformation (SourceEditor document, string fileName)
		{
			this.document   = document;
			this.textBuffer = document.Buffer;
			this.fileName   = fileName;
//			this.currentOffset = document.Caret.Offset;
		}
		
		public ProvidedDocumentInformation(SourceEditorBuffer textBuffer, string fileName, int currentOffset)
		{
			this.textBuffer    = textBuffer;
			this.fileName      = fileName;
			this.currentOffset = currentOffset;
		}
	}
}
