// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Diagnostics;

using ICSharpCode.TextEditor.Undo;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.SourceEditor.CodeCompletion
{
	public sealed class TextUtilities
	{
		
		/// <remarks>
		/// This function takes a string and converts the whitespace in front of
		/// it to tabs. If the length of the whitespace at the start of the string
		/// was not a whole number of tabs then there will still be some spaces just
		/// before the text starts.
		/// the output string will be of the form:
		/// 1. zero or more tabs
		/// 2. zero or more spaces (less than tabIndent)
		/// 3. the rest of the line
		/// </remarks>
/*		public static string LeadingWhiteSpaceToTabs(string line, int tabIndent) {
			StringBuilder sb = new StringBuilder(line.Length);
			int consecutiveSpaces = 0;
			int i = 0;
			for(i = 0; i < line.Length; i++) {
				if(line[i] == ' ') {
					consecutiveSpaces++;
					if(consecutiveSpaces == tabIndent) {
						sb.Append('\t');
						consecutiveSpaces = 0;
					}
				}
				else if(line[i] == '\t') {
					sb.Append('\t');
					// if we had say 3 spaces then a tab and tabIndent was 4 then
					// we would want to simply replace all of that with 1 tab
					consecutiveSpaces = 0;					
				}
				else {
					break;
				}
			}
			if(i < line.Length) {
				sb.Append(line.Substring(i-consecutiveSpaces));
			}
			return sb.ToString();
		}
*/

		public static bool IsLetterDigitOrUnderscore(char c)
		{
			if(!Char.IsLetterOrDigit(c)) {
				return c == '_';
			}
			return true;
		}
		
		public enum CharacterType {
			LetterDigitOrUnderscore,
			WhiteSpace,
			Other
		}
		
		/// <remarks>
		/// This method returns the expression before a specified offset.
		/// That method is used in code completion to determine the expression given
		/// to the parser for type resolve.
		/// </remarks>
		public static string GetExpressionBeforeOffset(SourceEditorView textArea, int offset)
		{
			while (offset - 1 > 0) {
				switch (textArea.Buffer.Text[offset - 1]) {
					case '}':
						goto done;
//						offset = SearchBracketBackward(document, offset - 2, '{','}');
//						break;
					case ']':
						offset = SearchBracketBackward(textArea, offset - 2, '[',']');
						break;
					case ')':
						offset = SearchBracketBackward(textArea, offset - 2, '(',')');
						break;
					case '.':
						--offset;
						break;
					case '"':
						return "\"\"";
					case '\'':
						return "'a'";
					case '>':
						if (textArea.Buffer.Text[offset - 2] == '-') {
							offset -= 2;
							break;
						}
						goto done;
					default:
						if (Char.IsWhiteSpace(textArea.Buffer.Text[offset - 1])) {
							--offset;
							break;
						}
						int start = offset - 1;
						if (!IsLetterDigitOrUnderscore(textArea.Buffer.Text[start])) {
							goto done;
						}
						
						while (start > 0 && IsLetterDigitOrUnderscore(textArea.Buffer.Text[start - 1])) {
							--start;
						}
						
						Console.WriteLine("{0} -- {1}", offset, start);
						Gtk.TextIter startIter = textArea.Buffer.GetIterAtOffset (start);
						Gtk.TextIter endIter = textArea.Buffer.GetIterAtOffset (offset);
						string word = textArea.Buffer.GetText (startIter, endIter, false).Trim();
						Console.WriteLine("word >{0}<", word);
						switch (word) {
							case "ref":
							case "out":
							case "in":
							case "return":
							case "throw":
							case "case":
								goto done;
						}
						
						if (word.Length > 0 && !IsLetterDigitOrUnderscore(word[0])) {
							goto done;
						}
						offset = start;
						break;
				}
			}
			done:
//			Console.WriteLine("ofs : {0} cart:{1}", offset, document.Caret.Offset);
//			Console.WriteLine("return:" + document.GetText(offset, document.Caret.Offset - offset).Trim());
			Gtk.TextIter start_Iter = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark);
			Gtk.TextIter offset_Iter = textArea.Buffer.GetIterAtOffset (start_Iter.Offset - offset);
			return textArea.Buffer.GetText (start_Iter, offset_Iter, false ).Trim();
		}
		
/*		
		public static CharacterType GetCharacterType(char c) 
		{
			if(IsLetterDigitOrUnderscore(c))
				return CharacterType.LetterDigitOrUnderscore;
			if(Char.IsWhiteSpace(c))
				return CharacterType.WhiteSpace;
			return CharacterType.Other;
		}
		
		public static int GetFirstNonWSChar(IDocument document, int offset)
		{
			while (offset < document.TextLength && Char.IsWhiteSpace(document.GetCharAt(offset))) {
				++offset;
			}
			return offset;
		}
		
		public static int FindWordEnd(IDocument document, int offset)
		{
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int     endPos = line.Offset + line.Length;
			while (offset < endPos && IsLetterDigitOrUnderscore(document.GetCharAt(offset))) {
				++offset;
			}
			
			return offset;
		}
		
		public static int FindWordStart(IDocument document, int offset)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			
			while (offset > line.Offset && !IsLetterDigitOrUnderscore(document.GetCharAt(offset - 1))) {
				--offset;
			}
			
			return offset;
		}
		
		// go forward to the start of the next word
		// if the cursor is at the start or in the middle of a word we move to the end of the word
		// and then past any whitespace that follows it
		// if the cursor is at the start or in the middle of some whitespace we move to the start of the
		// next word
		public static int FindNextWordStart(IDocument document, int offset)
		{
			int originalOffset = offset;
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int     endPos = line.Offset + line.Length;
			// lets go to the end of the word, whitespace or operator
			CharacterType t = GetCharacterType(document.GetCharAt(offset));
			while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == t) {
				++offset;
			}
			
			// now we're at the end of the word, lets find the start of the next one by skipping whitespace
			while (offset < endPos && GetCharacterType(document.GetCharAt(offset)) == CharacterType.WhiteSpace) {
				++offset;
			}

			return offset;
		}
		
		// go back to the start of the word we are on
		// if we are already at the start of a word or if we are in whitespace, then go back
		// to the start of the previous word
		public static int FindPrevWordStart(IDocument document, int offset)
		{
			int originalOffset = offset;
			LineSegment line = document.GetLineSegmentForOffset(offset);
			if (offset > 0) {
				CharacterType t = GetCharacterType(document.GetCharAt(offset - 1));
				while (offset > line.Offset && GetCharacterType(document.GetCharAt(offset - 1)) == t) {
					--offset;
				}
				
				// if we were in whitespace, and now we're at the end of a word or operator, go back to the beginning of it
				if(t == CharacterType.WhiteSpace && offset > line.Offset) {
					t = GetCharacterType(document.GetCharAt(offset - 1));
					while (offset > line.Offset && GetCharacterType(document.GetCharAt(offset - 1)) == t) {
						--offset;
					}
				}
			}
			
			return offset;
		}
		
		public static string GetLineAsString(IDocument document, int lineNumber)
		{
			LineSegment line = document.GetLineSegment(lineNumber);
			return document.GetText(line.Offset, line.Length);
		}
*/
		static bool ScanLineComment(SourceEditorView document, int offset)
		{
			while (offset > 0 && offset < document.Buffer.Text.Length) {
				char ch = document.Buffer.Text[offset];
				switch (ch) {
					case '\r':
					case '\n':
						return false;
					case '/':
						if (document.Buffer.Text[offset + 1] == '/') {
							return true;
						}
						break;
				}
				--offset;
			}
			return false;
		}
		
		public static int SearchBracketBackward(SourceEditorView document, int offset, char openBracket, char closingBracket)
		{
			int brackets = -1;
			
			bool inString = false;
			bool inChar   = false;
			
			bool blockComment = false;
			
			while (offset >= 0 && offset < document.Buffer.Text.Length) {
				char ch = document.Buffer.Text[offset];
				switch (ch) {
					case '/':
						if (blockComment) {
							if (document.Buffer.Text[offset + 1]== '*') {
								blockComment = false;
							}
						}
						if (!inString && !inChar && offset + 1 < document.Buffer.Text.Length) {
							if (offset > 0 && document.Buffer.Text[offset - 1] == '*') {
								blockComment = true;
							}
						}
						break;
					case '"':
						if (!inChar && !blockComment && !ScanLineComment(document, offset)) {
							inString = !inString;
						}
						break;
					case '\'':
						if (!inString && !blockComment && !ScanLineComment(document, offset)) {
							inChar = !inChar;
						}
						break;
					default :
						if (ch == closingBracket) {
							if (!(inString || inChar || blockComment) && !ScanLineComment(document, offset)) {
								--brackets;
							}
						} else if (ch == openBracket) {
							if (!(inString || inChar || blockComment) && !ScanLineComment(document, offset)) {
								++brackets;
								if (brackets == 0) {
									return offset;
								}
							}
						}
						break;
				}
				--offset;
			}
			return - 1;
		}
/*
		public static int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			int brackets = 1;
			
			bool inString = false;
			bool inChar   = false;
			
			bool lineComment  = false;
			bool blockComment = false;
			
			if (offset >= 0) {
				while (offset < document.TextLength) {
					char ch = document.GetCharAt(offset);
					switch (ch) {
						case '\r':
						case '\n':
							lineComment = false;
							break;
						case '/':
							if (blockComment) {
								Debug.Assert(offset > 0);
								if (document.GetCharAt(offset - 1) == '*') {
									blockComment = false;
								}
							}
							if (!inString && !inChar && offset + 1 < document.TextLength) {
								if (!blockComment && document.GetCharAt(offset + 1) == '/') {
									lineComment = true;
								}
								if (!lineComment && document.GetCharAt(offset + 1) == '*') {
									blockComment = true;
								}
							}
							break;
						case '"':
							if (!(inChar || lineComment || blockComment)) {
								inString = !inString;
							}
							break;
						case '\'':
							if (!(inString || lineComment || blockComment)) {
								inChar = !inChar;
							}
							break;
						default :
							if (ch == openBracket) {
								if (!(inString || inChar || lineComment || blockComment)) {
									++brackets;
								}
							} else if (ch == closingBracket) {
								if (!(inString || inChar || lineComment || blockComment)) {
									--brackets;
									if (brackets == 0) {
										return offset;
									}
								}
							}
							break;
					}
					++offset;
				}
			}
			return -1;
		}
		
		/// <remarks>
		/// Returns true, if the line lineNumber is empty or filled with whitespaces.
		/// </remarks>
		public static bool IsEmptyLine(IDocument document, int lineNumber)
		{
			return IsEmptyLine(document, document.GetLineSegment(lineNumber));
		}

		/// <remarks>
		/// Returns true, if the line lineNumber is empty or filled with whitespaces.
		/// </remarks>
		public static bool IsEmptyLine(IDocument document, LineSegment line)
		{
			for (int i = line.Offset; i < line.Offset + line.Length; ++i) {
				char ch = document.GetCharAt(i);
				if (!Char.IsWhiteSpace(ch)) {
					return false;
				}
			}
			return true;
		}*/
	}
}
