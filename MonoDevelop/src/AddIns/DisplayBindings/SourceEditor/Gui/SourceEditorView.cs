using Gtk;
using Gdk;

using System;
using System.IO;
using System.Runtime.InteropServices;

using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Core.Services;
using MonoDevelop.SourceEditor.CodeCompletion;
using MonoDevelop.SourceEditor.InsightWindow;
using MonoDevelop.EditorBindings.Properties;
using MonoDevelop.EditorBindings.FormattingStrategy;
using MonoDevelop.Gui.Utils;
using MonoDevelop.Services;

namespace MonoDevelop.SourceEditor.Gui
{
	public class SourceEditorView : SourceView, IFormattableDocument
	{	
		private static GLib.GType gtype;
		public readonly SourceEditor ParentEditor;
		internal IFormattingStrategy fmtr;
		public SourceEditorBuffer buf;
		int lineToMark = -1;
		CompletionWindow completionWnd;
		bool codeCompleteEnabled;

		CompletionWindow completionWindow {
			set {
				if (completionWnd != null) {
					completionWnd.Destroy ();
					completionWnd = null;
				}
				completionWnd = value;
			}
			get { return completionWnd; }
		}

		public static new GLib.GType GType
		{
			get
			{
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (SourceEditorView));
				return gtype;
			}
		}

		public bool EnableCodeCompletion {
			get { return codeCompleteEnabled; }
			set { codeCompleteEnabled = value; }
		}
		
		public SourceEditorView (SourceEditorBuffer buf, SourceEditor parent) : base (GType)
		{
			this.ParentEditor = parent;
			this.TabsWidth = 4;
			Buffer = this.buf = buf;
			AutoIndent = true;
			SmartHomeEnd = true;
			ShowLineNumbers = true;
			ShowLineMarkers = true;
			ButtonPressEvent += new ButtonPressEventHandler (buttonPress);
			buf.PlaceCursor (buf.StartIter);
			GrabFocus ();

		}

		void buttonPress (object o, ButtonPressEventArgs e)
		{
			if (!ShowLineMarkers)
				return;
			
			if (e.Event.Window == GetWindow (Gtk.TextWindowType.Left)) {
				int x, y;
				WindowToBufferCoords (Gtk.TextWindowType.Left, (int) e.Event.X, (int) e.Event.Y, out x, out y);
				TextIter line;
				int top;

				GetLineAtY (out line, y, out top);
				
				if (e.Event.Button == 1) {
					buf.ToggleBookmark (line.Line);
				} else if (e.Event.Button == 3) {
					Gtk.Menu popup = new Gtk.Menu ();
					Gtk.CheckMenuItem bookmarkItem = Gtk.CheckMenuItem.NewWithLabel (GettextCatalog.GetString ("Bookmark"));
					Gtk.CheckMenuItem breakpointItem = Gtk.CheckMenuItem.NewWithLabel (GettextCatalog.GetString ("Breakpoint"));

					bookmarkItem.Active = buf.IsBookmarked (line.Line);
					breakpointItem.Active = buf.IsBreakpoint (line.Line);

					bookmarkItem.Toggled += new EventHandler (bookmarkToggled);
					breakpointItem.Toggled += new EventHandler (breakpointToggled);
					popup.Append (bookmarkItem);
					popup.Append (breakpointItem);
					popup.ShowAll ();
					lineToMark = line.Line;
					popup.Popup (null, null, null, IntPtr.Zero, 3, e.Event.Time);
				}
			}
		}

		public void bookmarkToggled (object o, EventArgs e)
		{
			if (lineToMark == -1) return;
			buf.ToggleMark (lineToMark, SourceMarkerType.SourceEditorBookmark);
			lineToMark = -1;
		}

		public void breakpointToggled (object o, EventArgs e)
		{
			if (lineToMark == -1) return;
			IDebuggingService dbgr = (IDebuggingService)ServiceManager.Services.GetService (typeof (IDebuggingService));
			if (dbgr != null) {
				bool canToggle = dbgr.ToggleBreakpoint (ParentEditor.DisplayBinding.ContentName, lineToMark + 1);
				if (canToggle)
					buf.ToggleMark (lineToMark, SourceMarkerType.BreakpointMark);
				lineToMark = -1;
			}
		}

		public void ExecutingAt (int linenumber)
		{
			buf.ToggleMark (linenumber, SourceMarkerType.ExecutionMark);
			buf.MarkupLine (linenumber);	
		}

		public void ClearExecutingAt (int linenumber)
		{
			buf.ToggleMark (linenumber, SourceMarkerType.ExecutionMark);
			buf.UnMarkupLine (linenumber);
		}

		public void SimulateKeyPress (ref Gdk.EventKey evnt)
		{
			Gtk.Global.PropagateEvent (this, evnt);
		}

		void DeleteLine ()
		{
			//remove the current line
			TextIter iter = buf.GetIterAtMark (buf.InsertMark);
			iter.LineOffset = 0;
			TextIter end_iter = buf.GetIterAtLine (iter.Line);
			end_iter.LineOffset = end_iter.CharsInLine;
			buf.Delete (iter, end_iter);
		}

		void TriggerCodeComplete ()
		{
			TextIter iter = buf.GetIterAtMark (buf.InsertMark);
			char triggerChar = '.';
			TextIter triggerIter = TextIter.Zero;
			do {
				//FIXME: This code is placeholder until you can
				//just switch on iter.Char
				string s = buf.GetText (iter, buf.GetIterAtOffset (iter.Offset + 1), true);
				switch (s) {
				case " ":
					triggerIter = iter;
					triggerChar = ' ';
					break;
				case ".":
					triggerIter = iter;
					break;
				}
				if (!triggerIter.Equals (TextIter.Zero))
					break;
				iter.BackwardChar ();
			} while (iter.LineOffset != 0);

			if (triggerIter.Equals (TextIter.Zero)) return;
			triggerIter.ForwardChar ();
			completionWindow = new CompletionWindow (this, ParentEditor.DisplayBinding.ContentName, new CodeCompletionDataProvider ());

			completionWindow.ShowCompletionWindow (triggerChar, triggerIter, true);
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			Gdk.Key key = evnt.Key;
			uint state = (uint)evnt.State;
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
					case Gdk.Key.Delete:
						DeleteLine ();
						return true;
				}
				break;
			case Control:
				switch (key) {
					case Gdk.Key.space:
						TriggerCodeComplete ();
						return true;
					case Gdk.Key.l:
						DeleteLine ();
						return true;
				}
				break;
			}

			switch ((char)key) {
				case ' ':
					if (1 == 1) {
						string word = GetWordBeforeCaret ();
						if (word != null) {
							CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(ParentEditor.DisplayBinding.ContentName);
							
							if (templateGroup != null) {
								foreach (CodeTemplate template in templateGroup.Templates) {
									if (template.Shortcut == word) {
										InsertTemplate(template);
										return false;
									}
								}
							}
						}
					}
					goto case '.';

				case '.':
					bool retval = base.OnKeyPressEvent (evnt);
					if (EnableCodeCompletion) {
						completionWindow = new CompletionWindow (this, ParentEditor.DisplayBinding.ContentName, new CodeCompletionDataProvider ());
						completionWindow.ShowCompletionWindow ((char)key, buf.GetIterAtMark (buf.InsertMark), false);
					}
					return retval;
				/*case '(':
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
					break;*/
			}
		
			return base.OnKeyPressEvent (evnt);
		}

		public int FindPrevWordStart (string doc, int offset)
		{
			for ( offset-- ; offset >= 0 ; offset--)
			{
				if (Char.IsWhiteSpace (doc, offset)) break;
			}
			return ++offset;
		}

		public string GetWordBeforeCaret ()
		{
			int offset = buf.GetIterAtMark (buf.InsertMark).Offset;
			int start = FindPrevWordStart (buf.Text, offset);
			return buf.Text.Substring (start, offset - start);
		}
		
		public int DeleteWordBeforeCaret ()
		{
			int offset = buf.GetIterAtMark (buf.InsertMark).Offset;
			int start = FindPrevWordStart (buf.Text, offset);
			buf.Delete (buf.GetIterAtOffset (start), buf.GetIterAtOffset (offset));
			return start;
		}


		public void InsertTemplate (CodeTemplate template)
		{
			int newCaretOffset = buf.GetIterAtMark (buf.InsertMark).Offset;
			string word = GetWordBeforeCaret ().Trim ();
			
			if (word.Length > 0)
				newCaretOffset = DeleteWordBeforeCaret ();
			
			int finalCaretOffset = newCaretOffset;
			
			for (int i =0; i < template.Text.Length; ++i) {
				switch (template.Text[i]) {
					case '|':
						finalCaretOffset = newCaretOffset;
						break;
					case '\r':
						break;
					case '\t':
						buf.InsertAtCursor ('\t'.ToString ());
						newCaretOffset++;
						break;
					case '\n':
						buf.InsertAtCursor ('\n'.ToString ());
						newCaretOffset++;
						break;
					default:
						buf.InsertAtCursor (template.Text[i].ToString ());
						newCaretOffset++;
						break;
				}
			}
			
			buf.PlaceCursor (buf.GetIterAtOffset (finalCaretOffset));
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
			string indent = InsertSpacesInsteadOfTabs ? new string (' ', (int) TabsWidth) : "\t";
			
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
		
		IndentStyle IFormattableDocument.IndentStyle
		{
			get { return TextEditorProperties.IndentStyle; }
		}
		
		bool IFormattableDocument.AutoInsertCurlyBracket
		{
			get { return TextEditorProperties.AutoInsertCurlyBracket; }
		}
		
		string IFormattableDocument.TextContent
		{ get { return ParentEditor.DisplayBinding.Text; } }
		
		int IFormattableDocument.TextLength
		{ get { return Buffer.EndIter.Offset; } }
		
		char IFormattableDocument.GetCharAt (int offset)
		{
			TextIter it = Buffer.GetIterAtOffset (offset);
			return gtk_text_iter_get_char (ref it);
		}
		
		void IFormattableDocument.Insert (int offset, string text)
		{
			Buffer.Insert (Buffer.GetIterAtOffset (offset), text);
		}
		
		string IFormattableDocument.IndentString
		{
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
#endregion
	}
}
