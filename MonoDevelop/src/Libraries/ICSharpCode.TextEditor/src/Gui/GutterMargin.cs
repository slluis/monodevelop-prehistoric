// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using ICSharpCode.TextEditor.Document;

using Gdk;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class views the line numbers and folding markers.
	/// </summary>
	public class GutterMargin : AbstractMargin
	{
		
		public static Cursor RightLeftCursor;
		
		static GutterMargin()
		{
			Stream cursorStream = Assembly.GetCallingAssembly().GetManifestResourceStream("RightArrow.cur");
			//RightLeftCursor = new Cursor(cursorStream);
			cursorStream.Close();
		}
		
		
		public override Cursor Cursor {
			get {
				return RightLeftCursor;
			}
		}
		
		public override Size Size {
			get {
				return new Size((int)(textArea.TextView.GetWidth('w') * Math.Max(3, (int)Math.Log10(textArea.Document.TotalNumberOfLines) + 1)),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.ShowLineNumbers;
			}
		}
		
		
		
		public GutterMargin(TextArea textArea) : base(textArea)
		{
		}
		
		public override void Paint(Gdk.Drawable wnd, System.Drawing.Rectangle rect)
		{
			using (Gdk.GC gc = new Gdk.GC (wnd)) {
			using (Pango.Layout ly = new Pango.Layout (TextArea.PangoContext)) {
				ly.FontDescription = FontContainer.DefaultFont;
				
				HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
				Gdk.Color bg = new Gdk.Color (lineNumberPainterColor.BackgroundColor);
				Gdk.Color fg_text = new Gdk.Color (lineNumberPainterColor.Color);
				Gdk.Color fg_rect = TextArea.Style.White;
				
				
				//FIXME: This doesnt allow different fonts and what not
				int fontHeight = TextArea.TextView.FontHeight;
		
				for (int y = 0; y < (DrawingPosition.Height + textArea.TextView.VisibleLineDrawingRemainder) / fontHeight + 1; ++y) {
					int ypos = drawingPosition.Y + fontHeight * y  - textArea.TextView.VisibleLineDrawingRemainder;
					System.Drawing.Rectangle backgroundRectangle = new System.Drawing.Rectangle(drawingPosition.X, ypos, drawingPosition.Width, fontHeight);
					//if (rect.IntersectsWith(backgroundRectangle)) {
						
						gc.RgbBgColor = bg;
						gc.RgbFgColor = fg_rect;
						wnd.DrawRectangle (gc, true, backgroundRectangle);
						int curLine = y + textArea.TextView.FirstVisibleLine;
						if (curLine < textArea.Document.TotalNumberOfLines) {
							gc.RgbFgColor = fg_text;
							ly.SetText ((curLine + 1).ToString ());
							wnd.DrawLayout (gc, drawingPosition.X, ypos, ly);
						}
					//}
				}
			}}
		}
	}
}
