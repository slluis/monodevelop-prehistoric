// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
{
	public class ForwardTextIterator : ITextIterator
	{
		enum TextIteratorState {
			Resetted,
			Iterating,
			Done,
		}
		
		TextIteratorState state;
		
		EditorDocumentInformation docInfo;
		SourceEditorBuffer  textBuffer;
		int currentOffset;
		int endOffset;
		
		public SourceEditorBuffer TextBuffer {
			get {
				return textBuffer;
			}
		}
		
		public char Current {
			get {
				switch (state) {
					case TextIteratorState.Resetted:
						throw new System.InvalidOperationException("Call moveAhead first");
					case TextIteratorState.Iterating:
						return textBuffer.GetCharAt(currentOffset);
					case TextIteratorState.Done:
						throw new System.InvalidOperationException("TextIterator is at the end");
					default:
						throw new System.InvalidOperationException("unknown text iterator state");
				}
			}
		}
		
		public int Position {
			get {
				if (state == TextIteratorState.Done) return -1;
				else return currentOffset;
			}
			set {
				if (value == -1) {
					state = TextIteratorState.Done;
					currentOffset = endOffset;
					return;
				}
				if (state == TextIteratorState.Done)
					state = TextIteratorState.Iterating;
				currentOffset = value;
			}
		}
		
		
		public ForwardTextIterator (EditorDocumentInformation docInfo, int endOffset)
		{
			Debug.Assert(endOffset >= 0 && endOffset < textBuffer.Length);
			
			this.docInfo = docInfo;
			this.textBuffer = docInfo.TextBuffer;
			this.endOffset = endOffset;
			Reset();
		}
		
		public char GetCharRelative(int offset)
		{
			if (state != TextIteratorState.Iterating) {
				throw new System.InvalidOperationException();
			}
			
			if (currentOffset + offset < 0 || currentOffset + offset >= textBuffer.Length) 
				return char.MinValue;
				
			int realOffset = (currentOffset + (1 + Math.Abs(offset) / textBuffer.Length) * textBuffer.Length + offset) % textBuffer.Length;
			
			return textBuffer.GetCharAt(realOffset);
		}
		
		public bool MoveAhead(int numChars)
		{
			Debug.Assert(numChars > 0);
			
			switch (state) {
				case TextIteratorState.Resetted:
					currentOffset = endOffset;
					state = TextIteratorState.Iterating;
					return true;
				case TextIteratorState.Done:
					return false;
				case TextIteratorState.Iterating:
					int oldOffset = currentOffset;
					currentOffset = (currentOffset + numChars) % textBuffer.Length;
					bool finish = (oldOffset > currentOffset || oldOffset < endOffset) && currentOffset >= endOffset;
					if (finish) {
						state = TextIteratorState.Done;
						currentOffset = endOffset;
						return false;
					}
					return true;
				default:
					throw new Exception("Unknown text iterator state");
			}
		}
		
		public string ReadToEnd ()
		{
			if (state == TextIteratorState.Done) return "";
			
			string doc = "";
			if (currentOffset >= endOffset) {
				doc = textBuffer.GetText (currentOffset, textBuffer.Length - currentOffset);
				currentOffset = 0;
			}
				
			doc += textBuffer.GetText (currentOffset, endOffset - currentOffset);
			currentOffset = endOffset;
			return doc;
		}

		public void Replace (int length, string pattern)
		{
			docInfo.Replace (currentOffset, length, pattern);
			
			if (currentOffset <= endOffset) {
				endOffset = endOffset - length + pattern.Length;
			}
			
			currentOffset = currentOffset - length + pattern.Length;
		}

		public void Reset()
		{
			state         = TextIteratorState.Resetted;
			currentOffset = endOffset;
		}
		
		public void Close ()
		{
		}
		
		public override string ToString()
		{
			return String.Format("[ForwardTextIterator: currentOffset={0}, endOffset={1}, state={2}]", currentOffset, endOffset, state);
		}
	}
}
