using Gtk;
using GtkSharp;
using Gdk;

using System;
using System.IO;
using System.Runtime.InteropServices;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns;using MonoDevelop.SourceEditor.CodeCompletion;
using MonoDevelop.SourceEditor.InsightWindow;
using MonoDevelop.EditorBindings.Properties;
using MonoDevelop.EditorBindings.FormattingStrategy;

namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditorView : SourceView, IFormattableDocument {
		
		private static GLib.GType type;
			
		                SourceEditorBuffer buf;
		public readonly SourceEditor       ParentEditor;

		CompletionWindow completionWindow;
		
		static SourceEditorView ()
		{
			type = RegisterGType (typeof (SourceEditorView));
		}
		
		public SourceEditorView (SourceEditorBuffer buf, SourceEditor parent) : base (type)
		{
			this.ParentEditor = parent;
			Buffer = this.buf = buf;
			AutoIndent = true;
			SmartHomeEnd = true;
			ShowLineNumbers = true;
			ShowLineMarkers = true;
		}
		
		protected override bool OnKeyPressEvent (ref Gdk.EventKey evnt)
		{
			Gdk.Key key = evnt.Key;
			uint state = evnt.state;
			state &= 1101u;
			const uint Normal = 0, Shift = 1, Control = 4, ShiftControl = 5, Alt = 8;
			
			switch (state) {
			case Normal:
				switch (key) {
					case Gdk.Key.Tab:
						if (IndentSelection ())
							return true;
						break;
				}
				break;
			case Shift:
				switch (key) {
					case Gdk.Key.ISO_Left_Tab:
						if (UnIndentSelection ())
							return true;
						break;
				}
				break;
			}

			switch ((char)key) {
				//FIXME: ' ' needs to do extra parsing
				case ' ':
				case '.':
					completionWindow = new CompletionWindow (this, ParentEditor.DisplayBinding.ContentName, new CodeCompletionDataProvider ());
					completionWindow.ShowCompletionWindow ((char)key);
					break;
				case '(':
					try {
						InsightWindow insightWindow = new InsightWindow(this, ParentEditor.DisplayBinding.ContentName);
						
						insightWindow.AddInsightDataProvider(new MethodInsightDataProvider());
						insightWindow.ShowInsightWindow();
					} catch (Exception e) {
						Console.WriteLine("EXCEPTION: " + e);
					}
					break;
				case '[':
					try {
						InsightWindow insightWindow = new InsightWindow(this, ParentEditor.DisplayBinding.ContentName);
						
						insightWindow.AddInsightDataProvider(new IndexerInsightDataProvider());
						insightWindow.ShowInsightWindow();
					} catch (Exception e) {
						Console.WriteLine("EXCEPTION: " + e);
					}
					break;
					
				default:
					if (fmtr != null) {
						TextIter itr = Buffer.GetIterAtMark (Buffer.InsertMark);
						int offset = itr.Offset;
						int delta = fmtr.FormatLine (this, itr.Line, itr.Offset, (char)key);
						if (delta != 0) {
							Buffer.PlaceCursor (Buffer.GetIterAtOffset (offset + delta));
							return true;
						}
					}
					break;
			}
		
			return base.OnKeyPressEvent (ref evnt);
		}
		
#region Indentation
		public bool IndentSelection ()
		{
			TextIter begin, end;
			if (! buf.GetSelectionBounds (out begin, out end))
				return false;
			
			int y0 = begin.Line, y1 = end.Line;
			if (y0 == y1)
				return false;
			
			using (AtomicUndo a = new AtomicUndo (buf)) {
				IndentLines (y0, y1);
				SelectLines (y0, y1);
			}
			
			return true;
		}
		
		public bool UnIndentSelection ()
		{
			TextIter begin, end;
			if (! buf.GetSelectionBounds (out begin, out end))
				return false;
			
			int y0 = begin.Line, y1 = end.Line;
			if (y0 == y1)
				return false;
			
			using (AtomicUndo a = new AtomicUndo (buf)) {
				UnIndentLines (y0, y1);
				SelectLines (y0, y1);
			}
			
			return true;
		}
		
		void IndentLines (int y0, int y1)
		{
			string indent = InsertSpacesInsteadOfTabs ? "\t" : new string (' ', (int) TabsWidth);
			
			for (int l = y0; l <= y1; l ++)
				Buffer.Insert (Buffer.GetIterAtLine (l), indent);
		}
		
		// WORKAROUND until we get this method returning char in gtk#
		[DllImport("libgtk-win32-2.0-0.dll")]
		static extern char gtk_text_iter_get_char (ref Gtk.TextIter raw);
		
		void UnIndentLines (int y0, int y1)
		{
			for (int l = y0; l <= y1; l ++) {
				TextIter start = Buffer.GetIterAtLine (l);
				TextIter end = start;
				
				char c = gtk_text_iter_get_char (ref end);
				
				if (c == '\t') {
					end.ForwardChar ();
					buf.Delete (start, end);
					
				} else if (c == ' ') {
					int cnt = 0;
					int max = (int) TabsWidth;
					
					while (cnt <= max && gtk_text_iter_get_char (ref end) == ' ' && ! end.EndsLine ()) {
						cnt ++;
						end.ForwardChar ();
					}
					
					if (cnt == 0)
						return;
					
					buf.Delete (start, end);
				}
			}
		}
		
		void SelectLines (int y0, int y1)
		{
			Buffer.PlaceCursor (Buffer.GetIterAtLine (y0));
			
			TextIter end = Buffer.GetIterAtLine (y1);
			end.ForwardToLineEnd ();
			Buffer.MoveMark ("selection_bound", end);
		}
#endregion

#region IFormattableDocument
		string IFormattableDocument.GetLineAsString (int ln)
		{
			TextIter begin = Buffer.GetIterAtLine (ln);
			TextIter end = begin;
			end.ForwardToLineEnd ();
			
			return begin.GetText (end);
		}
		
		void IFormattableDocument.BeginAtomicUndo ()
		{
			Buffer.BeginUserAction ();
		}
		void IFormattableDocument.EndAtomicUndo ()
		{
			Buffer.EndUserAction ();
		}
		
		// Need the ref here
		[DllImport("libgtk-win32-2.0-0.dll")]
		static extern void gtk_text_buffer_delete (IntPtr raw, ref Gtk.TextIter start, ref Gtk.TextIter end);
		
		void IFormattableDocument.ReplaceLine (int ln, string txt)
		{
			TextIter begin = Buffer.GetIterAtLine (ln);
			TextIter end = begin;
			end.ForwardToLineEnd ();
			
			gtk_text_buffer_delete (Buffer.Handle, ref begin, ref end);
			Buffer.Insert (begin, txt);
		}
		
		IndentStyle IFormattableDocument.IndentStyle {
			get { return TextEditorProperties.IndentStyle; }
		}
		bool IFormattableDocument.AutoInsertCurlyBracket {
			get { return TextEditorProperties.AutoInsertCurlyBracket; }
		}
		
		string IFormattableDocument.TextContent { get { return Buffer.Text; } }
		int IFormattableDocument.TextLength { get { return Buffer.EndIter.Offset; } }
		char IFormattableDocument.GetCharAt (int offset)
		{
			TextIter it = Buffer.GetIterAtOffset (offset);
			return gtk_text_iter_get_char (ref it);
		}
		
		void IFormattableDocument.Insert (int offset, string text)
		{
			Buffer.Insert (Buffer.GetIterAtOffset (offset), text);
		}
		
		string IFormattableDocument.IndentString {
			get { return !InsertSpacesInsteadOfTabs ? "\t" : new string (' ', (int) TabsWidth); }
		}
		
		int IFormattableDocument.GetClosingBraceForLine (int ln, out int openingLine)
		{
			int offset = MonoDevelop.SourceEditor.CodeCompletion.TextUtilities.SearchBracketBackward
				(this, Buffer.GetIterAtLine (ln).Offset - 1, '{', '}');
			
			openingLine = offset == -1 ? -1 : Buffer.GetIterAtOffset (offset).Line;
			return offset;
		}
		void IFormattableDocument.GetLineLengthInfo (int ln, out int offset, out int len)
		{
			TextIter begin = Buffer.GetIterAtLine (ln);
			offset = begin.Offset;
			len = begin.CharsInLine;
		}
		
		internal IFormattingStrategy fmtr;
		

#endregion
	}
}
