using Gtk;
using GtkSharp;
using Gdk;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditorView : SourceView {
		
		private static GLib.GType type;
			
		SourceEditorBuffer buf;
		
		static SourceEditorView ()
		{
			type = RegisterGType (typeof (SourceEditorView));
		}
		
		public SourceEditorView (SourceEditorBuffer buf) : base (type)
		{
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
			
			base.OnKeyPressEvent (ref evnt);
			return false;
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
	}
}