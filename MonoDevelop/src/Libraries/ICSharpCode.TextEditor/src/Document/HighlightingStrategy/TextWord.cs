// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;

namespace ICSharpCode.TextEditor.Document
{
	
	public enum TextWordType {
		Word,
		Space,
		Tab
	}
	
	/// <summary>
	/// This class represents single words with color information, two special versions of a word are 
	/// spaces and tabs.
	/// </summary>
	public class TextWord
	{
		HighlightColor  color;
		
		int          offset;
		int          length;
		
		static TextWord spaceWord = new TextWord (TextWordType.Space);
		static TextWord tabWord   = new TextWord (TextWordType.Tab);
		
		static public TextWord Space {
			get {
				return spaceWord;
			}
		}
		
		static public TextWord Tab {
			get {
				return tabWord;
			}
		}
		
		public int  Length {
			get {
				return length;
			}
		}
		
		public bool HasDefaultColor {
			get {
				return  offset & ~(1 << 31) != 0;
			}
		}
		
		public TextWordType Type {
			get {
				return 
					this == spaceWord ? TextWordType.Space :
					this == tabWord   ? TextWordType.Tab   :
					                    TextWordType.Word  ;
			}
		}
		
//		string       myword = null;
		public string GetWord (IDocument d, LineSegment l)
		{
			return d.GetText (l.Offset + offset, length);
//				if (myword == null) {
//					myword = document.GetText(word.Offset + offset, length);
//				}
//				return myword;
		}
		
		public Pango.FontDescription Font {
			get {
				return color.Font;
			}
		}
		
		public Color Color {
			get {
				return color.Color;
			}
		}
		
		public HighlightColor SyntaxColor {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public bool IsWhiteSpace {
			get {
				return this == spaceWord || this == tabWord;
			}
		}
		
		// TAB
		private TextWord(TextWordType type)
		{
			length = 1;
		}
		
		public TextWord(int offset, int length, HighlightColor color, bool hasDefaultColor)
		{
			Debug.Assert(color != null);
			
			this.offset = offset;
			this.length = length;
			this.color = color;
			if (hasDefaultColor)
				this.offset |= (1 << 31);
		}
		
		/// <summary>
		/// Converts a <see cref="TextWord"/> instance to string (for debug purposes)
		/// </summary>
		public override string ToString()
		{
			return "[TextWord: , Font = " + Font.Family + ", Color = " + Color + "]";
		}
	}
}
