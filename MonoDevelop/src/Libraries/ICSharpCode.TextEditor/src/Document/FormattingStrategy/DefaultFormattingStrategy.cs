// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Text;


namespace ICSharpCode.TextEditor.Document {
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class DefaultFormattingStrategy : IFormattingStrategy {
		/// <summary>
		/// returns the whitespaces which are before a non white space character in the line line
		/// as a string.
		/// </summary>
		protected string GetIndentation (IDocument d, int lineNumber)
		{
			if (lineNumber < 0 || lineNumber > d.TotalNumberOfLines)
				throw new ArgumentOutOfRangeException ("lineNumber");
			
			string lineText = TextUtilities.GetLineAsString (d, lineNumber);
			StringBuilder whitespaces = new StringBuilder ();
			
			foreach (char ch in lineText) {
				if (! Char.IsWhiteSpace (ch))
					break;
				whitespaces.Append (ch);
			}
			
			return whitespaces.ToString ();
		}
		
		/// <summary>
		/// Could be overwritten to define more complex indenting.
		/// </summary>
		protected virtual int AutoIndentLine (IDocument d, int lineNumber)
		{
			string indentation = lineNumber != 0 ? GetIndentation (d, lineNumber - 1) : "";
			
			if (indentation.Length > 0) {
				string newLineText = indentation + TextUtilities.GetLineAsString (d, lineNumber).Trim ();
				LineSegment oldLine = d.GetLineSegment (lineNumber);
				d.Replace (oldLine.Offset, oldLine.Length, newLineText);
			}
			
			return indentation.Length;
		}
		
		/// <summary>
		/// Could be overwritten to define more complex indenting.
		/// </summary>
		protected virtual int SmartIndentLine (IDocument d, int line)
		{
			return AutoIndentLine (d, line); // smart = autoindent in normal texts
		}
		
		/// <summary>
		/// This function formats a specific line after <code>ch</code> is pressed.
		/// </summary>
		/// <returns>
		/// the caret delta position the caret will be moved this number
		/// of bytes (e.g. the number of bytes inserted before the caret, or
		/// removed, if this number is negative)
		/// </returns>
		public virtual int FormatLine (IDocument d, int line, int cursorOffset, char ch)
		{
			if (ch == '\n')
				return IndentLine (d, line);
			
			return 0;
		}
		
		/// <summary>
		/// This function sets the indentation level in a specific line
		/// </summary>
		/// <returns>
		/// the number of inserted characters.
		/// </returns>
		public int IndentLine (IDocument d, int line)
		{
			switch (d.TextEditorProperties.IndentStyle) {
				case IndentStyle.Auto  : return AutoIndentLine (d, line);
				case IndentStyle.Smart : return SmartIndentLine (d, line);
				case IndentStyle.None  :
				default                : return 0;
			}
		}
		
		/// <summary>
		/// This function sets the indentlevel in a range of lines.
		/// </summary>
		public void IndentLines (IDocument d, int begin, int end)
		{
			int redocounter = 0;
			
			for (int i = begin; i <= end; ++i) {
				if (IndentLine(d, i) > 0)
					++redocounter;
			}
			
			if (redocounter > 0)
				d.UndoStack.UndoLast(redocounter);
		}
	}
}
