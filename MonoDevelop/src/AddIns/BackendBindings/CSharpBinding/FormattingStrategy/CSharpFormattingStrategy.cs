// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace CSharpBinding.FormattingStrategy {
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class CSharpFormattingStrategy : DefaultFormattingStrategy {
		
		/// <summary>
		/// Define CSharp specific smart indenting for a line :)
		/// </summary>
		protected override int SmartIndentLine (IDocument d, int lineNr)
		{
			if (lineNr > 0) {
				LineSegment lineAbove = d.GetLineSegment (lineNr - 1);
				string  lineAboveText = d.GetText (lineAbove.Offset, lineAbove.Length).Trim ();
				
				LineSegment curLine = d.GetLineSegment (lineNr);
				string  curLineText = d.GetText (curLine.Offset, curLine.Length).Trim ();
				
				if ((lineAboveText.EndsWith (")") && curLineText.StartsWith ("{")) ||       // after for, while, etc.
				    (lineAboveText.EndsWith ("else") && curLineText.StartsWith ("{")))      // after else
				{
					string indentation = GetIndentation (d, lineNr - 1);
					d.Replace (curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				// indent closing bracket.
				if (curLineText.StartsWith ("}")) {
					int closingBracketOffset = TextUtilities.SearchBracketBackward (d, curLine.Offset + d.GetText (curLine.Offset, curLine.Length).IndexOf ('}') - 1, '{', '}');
					
					// no closing bracket found -> autoindent
					if (closingBracketOffset == -1)
						return AutoIndentLine (d, lineNr);
					
					string indentation = GetIndentation (d, d.GetLineNumberForOffset (closingBracketOffset));
					
					d.Replace (curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				// expression ended, reset to valid indent.
				if (lineAboveText.EndsWith (";")) {
					int closingBracketOffset = TextUtilities.SearchBracketBackward (d, curLine.Offset + d.GetText (curLine.Offset, curLine.Length).IndexOf ('}') - 1, '{', '}');
					
					// no closing bracket found -> autoindent
					if (closingBracketOffset == -1)
						return AutoIndentLine (d, lineNr);
					
					int closingBracketLineNr = d.GetLineNumberForOffset (closingBracketOffset);
					LineSegment closingBracketLine = d.GetLineSegment (closingBracketLineNr);
					string  closingBracketLineText = d.GetText (closingBracketLine.Offset, closingBracketLine.Length).Trim ();
					
					string indentation = GetIndentation (d, closingBracketLineNr);
					
					// special handling for switch statement formatting.
					if (closingBracketLineText.StartsWith ("switch")) {
						if (! (lineAboveText.StartsWith ("break;") || lineAboveText.StartsWith ("goto") || lineAboveText.StartsWith ("return")))
							indentation += ICSharpCode.TextEditor.Actions.Tab.GetIndentationString (d);
					}
					indentation += ICSharpCode.TextEditor.Actions.Tab.GetIndentationString (d);
					
					d.Replace (curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (lineAboveText.EndsWith ("{") || // indent opening bracket.
				    lineAboveText.EndsWith (":") || // indent case xyz:
				    (lineAboveText.EndsWith (")") &&  // indent single line if, for ... etc
				    (lineAboveText.StartsWith ("if") ||
				     lineAboveText.StartsWith ("while") ||
				     lineAboveText.StartsWith ("for"))) ||
				     lineAboveText.EndsWith ("else")) {
						string indentation = GetIndentation (d, lineNr - 1) + ICSharpCode.TextEditor.Actions.Tab.GetIndentationString (d);
						d.Replace (curLine.Offset, curLine.Length, indentation + curLineText);
						return indentation.Length;
				} else {
					// try to indent linewrap
					ArrayList bracketPos = new ArrayList ();
					
					// search for a ( bracket that isn't closed
					for (int i = 0; i < lineAboveText.Length; ++i) {
						if (lineAboveText [i] == '(')
							bracketPos.Add (i);
						else if (lineAboveText [i] == ')' && bracketPos.Count > 0)
							bracketPos.RemoveAt (bracketPos.Count - 1);
					}
					
					if (bracketPos.Count > 0) {
						int bracketIndex = (int)bracketPos [bracketPos.Count - 1];
						string indentation = GetIndentation (d, lineNr - 1);
						
						// insert enough spaces to match brace start in the next line
						for (int i = 0; i <= bracketIndex; ++i)
							indentation += " ";
						
						d.Replace (curLine.Offset, curLine.Length, indentation + curLineText);
						return indentation.Length;
					}
				}
			}
			return AutoIndentLine (d, lineNr);
		}
		
		bool NeedCurlyBracket (string text)
		{
			int curlyCounter = 0;
			
			bool inString = false;
			bool inChar   = false;
			
			bool lineComment  = false;
			bool blockComment = false;
			
			for (int i = 0; i < text.Length; ++i) {
				switch (text [i]) {
					case '\r':
					case '\n':
						lineComment = false;
						break;
					case '/':
						if (blockComment) {
							Debug.Assert (i > 0);
							if (text [i - 1] == '*')
								blockComment = false;
								
						}
						
						if (!inString && !inChar && i + 1 < text.Length) {
							if (!blockComment && text [i + 1] == '/')
								lineComment = true;
							
							if (!lineComment && text [i + 1] == '*')
								blockComment = true;
						}
						break;
					case '"':
						if (!(inChar || lineComment || blockComment))
							inString = !inString;
						
						break;
					case '\'':
						if (!(inString || lineComment || blockComment))
							inChar = !inChar;
						
						break;
					case '{':
						if (!(inString || inChar || lineComment || blockComment))
							++curlyCounter;
						
						break;
					case '}':
						if (!(inString || inChar || lineComment || blockComment))
							--curlyCounter;
						
						break;
				}
			}
			return curlyCounter > 0;
		}
		
		bool IsInsideStringOrComment (IDocument d, LineSegment curLine, int cursorOffset)
		{
			// scan cur line if it is inside a string or single line comment (//)
			bool isInsideString  = false;
			
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = d.GetCharAt (i);
				if (ch == '"')
					isInsideString = !isInsideString;
				
				if (ch == '/' && i + 1 < cursorOffset && d.GetCharAt (i + 1) == '/')
					return true;
			}
			
			return isInsideString;
		}

		bool IsInsideDocumentationComment (IDocument d, LineSegment curLine, int cursorOffset)
		{
			// scan cur line if it is inside a string or single line comment (//)
			bool isInsideString  = false;
			
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = d.GetCharAt (i);
				if (ch == '"')
					isInsideString = !isInsideString;
					
				if (!isInsideString) {
					if (ch == '/' && i + 2 < cursorOffset && d.GetCharAt (i + 1) == '/' && d.GetCharAt (i + 2) == '/')
						return true;
				}
			}
			
			return false;
		}
		
		// used for comment tag formater/inserter
		public override int FormatLine (IDocument d, int lineNr, int cursorOffset, char ch)
		{
			LineSegment curLine   = d.GetLineSegment (lineNr);
			LineSegment lineAbove = lineNr > 0 ? d.GetLineSegment (lineNr - 1) : null;
			
			if (ch != '\n' && ch != '>' && IsInsideStringOrComment (d, curLine, cursorOffset))
				return 0;
			
			switch (ch) {
				case '>':
					if (IsInsideDocumentationComment (d, curLine, cursorOffset)) {
						string curLineText  = d.GetText (curLine.Offset, curLine.Length);
						int column = cursorOffset - curLine.Offset;
						int index = Math.Min (column - 1, curLineText.Length - 1);
						if (curLineText [index] == '/')
							break;
						
						while (index >= 0 && curLineText [index] != '<')
							--index;
						
						if (index > 0) {
							bool skipInsert = false;
							for (int i = index; i < curLineText.Length && i < column; ++i) {
								if (i < curLineText.Length && curLineText [i] == '/' && curLineText [i + 1] == '>')
									skipInsert = true;
								
								if (curLineText [i] == '>')
									break;
							}
							
							if (skipInsert)
								break;
						
							StringBuilder commentBuilder = new StringBuilder ("");
							for (int i = index; i < curLineText.Length && i < column && !Char.IsWhiteSpace (curLineText [i]); ++i)
								commentBuilder.Append (curLineText [i]);
								
							string tag = commentBuilder.ToString ().Trim ();
							if (!tag.EndsWith (">"))
								tag += ">";
							
							if (!tag.StartsWith ("/"))
								d.Insert (cursorOffset, "</" + tag.Substring (1));
						}
					}
					break;
				case '}':
				case '{':
					return IndentLine (d, lineNr);
				case '\n':
					if (lineNr <= 0)
						return IndentLine (d, lineNr);
					
					if (d.TextEditorProperties.AutoInsertCurlyBracket) {
						string oldLineText = TextUtilities.GetLineAsString (d, lineNr - 1);
						if (oldLineText.EndsWith ("{") && NeedCurlyBracket (d.TextContent)) {
							d.Insert (cursorOffset, "\n}");
							IndentLine (d, lineNr + 1);
						}
					}
					
					string  lineAboveText = d.GetText (lineAbove.Offset, lineAbove.Length);
					
					LineSegment    nextLine      = lineNr + 1 < d.TotalNumberOfLines ? d.GetLineSegment (lineNr + 1) : null;
					string  nextLineText  = lineNr + 1 < d.TotalNumberOfLines ? d.GetText (nextLine.Offset, nextLine.Length) : "";
					
					if (lineAbove.HighlightSpanStack != null && lineAbove.HighlightSpanStack.Count > 0) {				
						if (!((Span)lineAbove.HighlightSpanStack.Peek ()).StopEOL) {	// case for /* style comments
							int index = lineAboveText.IndexOf ("/*");
							
							if (index > 0) {
								string indentation = GetIndentation (d, lineNr - 1);
								for (int i = indentation.Length; i < index; ++ i)
									indentation += ' ';
								
								d.Replace (curLine.Offset, cursorOffset - curLine.Offset, indentation + " * ");
								return indentation.Length + 3;
							}
							
							index = lineAboveText.IndexOf ("*");
							if (index > 0) {
								string indentation = GetIndentation (d, lineNr - 1);
								for (int i = indentation.Length; i < index; ++ i)
									indentation += ' ';
								
								d.Replace (curLine.Offset, cursorOffset - curLine.Offset, indentation + "* ");
								return indentation.Length + 2;
							}
						} else {
							// don't handle // lines, because they're only one lined comments
							int indexAbove = lineAboveText.IndexOf ("///");
							int indexNext  = nextLineText.IndexOf ("///");
							
							if (indexAbove > 0 && (indexNext != -1 || indexAbove + 4 < lineAbove.Length)) {
								string indentation = GetIndentation (d, lineNr - 1);
								for (int i = indentation.Length; i < indexAbove; ++ i)
									indentation += ' ';
								
								d.Replace (curLine.Offset, cursorOffset - curLine.Offset, indentation + "/// ");
								return indentation.Length + 4;
							}
						}
					}
					return IndentLine (d, lineNr);
			}
			return 0;
		}
	}
}
