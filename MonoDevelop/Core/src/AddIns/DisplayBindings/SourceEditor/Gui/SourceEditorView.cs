using Gtk;
using Gdk;
using Global = Gtk.Global;

using System;
using System.IO;
using System.Runtime.InteropServices;

using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Internal.Parser;
using MonoDevelop.Core.Services;
using MonoDevelop.SourceEditor.CodeCompletion;
using MonoDevelop.SourceEditor.InsightWindow;
using MonoDevelop.EditorBindings.Properties;
using MonoDevelop.EditorBindings.FormattingStrategy;
using MonoDevelop.Gui.Utils;
using MonoDevelop.Gui;
using MonoDevelop.Services;

using GtkSourceView;

namespace MonoDevelop.SourceEditor.Gui
{
	public class SourceEditorView : SourceView, IFormattableDocument
	{	
		public readonly SourceEditor ParentEditor;
		internal IFormattingStrategy fmtr;
		public SourceEditorBuffer buf;
		int lineToMark = -1;
		bool codeCompleteEnabled;
		bool autoHideCompletionWindow = true;
		bool autoInsertTemplates;

		public bool EnableCodeCompletion {
			get { return codeCompleteEnabled; }
			set { codeCompleteEnabled = value; }
		}

		public bool AutoInsertTemplates {
			get { return autoInsertTemplates; }
			set { autoInsertTemplates = value; }
		}

		public SourceEditorView (SourceEditorBuffer buf, SourceEditor parent)
		{
			this.ParentEditor = parent;
			this.TabsWidth = 4;
			Buffer = this.buf = buf;
			AutoIndent = true;
			SmartHomeEnd = true;
			ShowLineNumbers = true;
			ShowLineMarkers = true;
			buf.PlaceCursor (buf.StartIter);
			GrabFocus ();
			buf.MarkSet += new MarkSetHandler (BufferMarkSet);
			buf.Changed += new EventHandler (BufferChanged);
		}
		
		public new void Dispose ()
		{
			buf.MarkSet -= new MarkSetHandler (BufferMarkSet);
			buf.Changed -= new EventHandler (BufferChanged);
			base.Dispose ();
		}
		
		void BufferMarkSet (object s, MarkSetArgs a)
		{
			if (a.Mark.Name == "insert") {
				if (autoHideCompletionWindow)
					CompletionListWindow.HideWindow ();
				buf.HideHighlightLine ();
			}
		}
		
		protected override bool OnFocusOutEvent (EventFocus e)
		{
			CompletionListWindow.HideWindow ();
			return base.OnFocusOutEvent (e);
		}
		
		void BufferChanged (object s, EventArgs args)
		{
			if (autoHideCompletionWindow)
				CompletionListWindow.HideWindow ();
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton e)
		{
			CompletionListWindow.HideWindow ();
			
			if (!ShowLineMarkers)
				return base.OnButtonPressEvent (e);
			
			if (e.Window == GetWindow (Gtk.TextWindowType.Left)) {
				int x, y;
				WindowToBufferCoords (Gtk.TextWindowType.Left, (int) e.X, (int) e.Y, out x, out y);
				TextIter line;
				int top;

				GetLineAtY (out line, y, out top);
				
				if (e.Button == 1) {
					buf.ToggleBookmark (line.Line);
				} else if (e.Button == 3) {
					Gtk.Menu popup = new Gtk.Menu ();
					Gtk.CheckMenuItem bookmarkItem = new Gtk.CheckMenuItem (GettextCatalog.GetString ("Bookmark"));
					bookmarkItem.Active = buf.IsBookmarked (line.Line);
					bookmarkItem.Toggled += new EventHandler (bookmarkToggled);
					popup.Append (bookmarkItem);

					if (ServiceManager.GetService (typeof (IDebuggingService)) != null) {
						Gtk.CheckMenuItem breakpointItem = new Gtk.CheckMenuItem (GettextCatalog.GetString ("Breakpoint"));
	
						breakpointItem.Active = buf.IsBreakpoint (line.Line);
	
						breakpointItem.Toggled += new EventHandler (breakpointToggled);
						popup.Append (breakpointItem);
					}

					popup.ShowAll ();
					lineToMark = line.Line;
					popup.Popup (null, null, null, IntPtr.Zero, 3, e.Time);
				}
			}
			return base.OnButtonPressEvent (e);
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
			IDebuggingService dbgr = (IDebuggingService)ServiceManager.GetService (typeof (IDebuggingService));
			if (dbgr != null) {
				dbgr.ToggleBreakpoint (ParentEditor.DisplayBinding.ContentName, lineToMark + 1);
				lineToMark = -1;
			}
		}

		public void ShowBreakpointAt (int linenumber)
		{
			if (!buf.IsMarked (linenumber, SourceMarkerType.BreakpointMark))
				buf.ToggleMark (linenumber, SourceMarkerType.BreakpointMark);
		}
		
		public void ClearBreakpointAt (int linenumber)
		{
			if (buf.IsMarked (linenumber, SourceMarkerType.BreakpointMark))
				buf.ToggleMark (linenumber, SourceMarkerType.BreakpointMark);
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
			Global.PropagateEvent (this, evnt);
		}

		void DeleteLine ()
		{
			//remove the current line
			TextIter iter = buf.GetIterAtMark (buf.InsertMark);
			iter.LineOffset = 0;
			TextIter end_iter = buf.GetIterAtLine (iter.Line);
			end_iter.LineOffset = end_iter.CharsInLine;
			buf.Delete (ref iter, ref end_iter);
		}

		void TriggerCodeComplete ()
		{
			
			TextIter iter = buf.GetIterAtMark (buf.InsertMark);
			char triggerChar = '\0';
			TextIter triggerIter = TextIter.Zero;
			do {
				if (iter.Char == null || iter.Char.Length == 0) {
					break;
				}
				switch (iter.Char[0]) {
				case ' ':
				case '\t':
				case '.':
				case '(':
				case '[':
					triggerIter = iter;
					triggerChar = iter.Char[0];
					break;
				}
				if (!triggerIter.Equals (TextIter.Zero))
					break;
				iter.BackwardChar ();
			} while (iter.LineOffset != 0);

			if (triggerIter.Equals (TextIter.Zero))
				return;
			triggerIter.ForwardChar ();
			
//			CompletionWindow.ShowWindow (triggerChar, triggerIter, true, new CodeCompletionDataProvider (true), this);
			CompletionListWindow.ShowWindow (triggerChar, triggerIter, new CodeCompletionDataProvider (true), this);
		}

		bool MonodocResolver ()
		{
			TextIter insertIter = buf.GetIterAtMark (buf.InsertMark);
			TextIter triggerIter = TextIter.Zero;
			try {
				do {
					switch (insertIter.Char[0]) {
					case ' ':
					case '\t':
					case '.':
					case '(':
					case '[':
						triggerIter = insertIter;
						break;
					}
					if (!triggerIter.Equals (TextIter.Zero))
						break;
					insertIter.ForwardChar ();
				} while (insertIter.LineOffset != 0);
				triggerIter.ForwardChar ();
			} catch {
				return false;
			}
			insertIter = triggerIter;
			IParserService parser = (IParserService)ServiceManager.GetService (typeof (IParserService));
			string fileName = ParentEditor.DisplayBinding.ContentName;
			IExpressionFinder expressionFinder = parser.GetExpressionFinder(fileName);
			string expression    = expressionFinder == null ? TextUtilities.GetExpressionBeforeOffset(this, insertIter.Offset) : expressionFinder.FindExpression(buf.GetText(buf.StartIter, insertIter, true), insertIter.Offset - 2);
			if (expression == null) return false;
			Console.WriteLine ("Expression: {" + expression + "}");
			string type = parser.MonodocResolver (ParentEditor.DisplayBinding.Project, expression, insertIter.Line + 1, insertIter.LineOffset + 1, fileName, buf.Text);
			if (type == null || type.Length == 0)
				return false;

			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.ContentName == GettextCatalog.GetString ("Documentation")) {
					((HelpViewer)content).LoadUrl (type);
					content.WorkbenchWindow.SelectWindow ();
					return true;
				}
			}
			HelpViewer new_content = new HelpViewer ();
			new_content.LoadUrl (type);
			WorkbenchSingleton.Workbench.ShowView (new_content, true);
			
			return true;
		}

		void ScrollUp () {
			ParentEditor.Vadjustment.Value -= (ParentEditor.Vadjustment.StepIncrement / 5);
			if (ParentEditor.Vadjustment.Value < 0.0d)
				ParentEditor.Vadjustment.Value = 0.0d;

			ParentEditor.Vadjustment.ChangeValue();
		}

		void ScrollDown () {
			double maxvalue = ParentEditor.Vadjustment.Upper - ParentEditor.Vadjustment.PageSize;
			double newvalue = ParentEditor.Vadjustment.Value + (ParentEditor.Vadjustment.StepIncrement / 5);

			if (newvalue > maxvalue)
				ParentEditor.Vadjustment.Value = maxvalue;
			else
				ParentEditor.Vadjustment.Value = newvalue;

			ParentEditor.Vadjustment.ChangeValue();
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if (CompletionListWindow.ProcessKeyEvent (evnt))
				return true;
			
			autoHideCompletionWindow = false;
			bool res = ProcessPressEvent (evnt);
			autoHideCompletionWindow = true;
			return res;
		}
		
		bool ProcessPressEvent (Gdk.EventKey evnt)
		{
			Gdk.Key key = evnt.Key;
			uint state = (uint)evnt.State;
			state &= 1101u;
			const uint Normal = 0, Shift = 1, Control = 4; /*ShiftControl = 5, Alt = 8*/
			
			switch (state) {
			case Normal:
				switch (key) {
				case Gdk.Key.End:
					if (buf.GotoSelectionEnd ())
						return true;
					break;
				case Gdk.Key.Home:
					if (buf.GotoSelectionStart ())
						return true;
					break;
				case Gdk.Key.Tab:
					if (IndentSelection ())
						return true;
					break;
				case Gdk.Key.F1:
				case Gdk.Key.KP_F1:
					if (MonodocResolver ())
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
				case Gdk.Key.Up:
					ScrollUp ();
					return true;
				case Gdk.Key.Down:
					ScrollDown ();
					return true;
				}
				break;
			}

			switch ((char)key) {
			case ' ':
				if (AutoInsertTemplates)
				{
					string word = GetWordBeforeCaret ();
					if (word != null) {
						/*if (word.ToLower () == "new") {
							if (EnableCodeCompletion) {
								completionWindow = new CompletionWindow (this, ParentEditor.DisplayBinding.ContentName, new CodeCompletionDataProvider (true));
								completionWindow.ShowCompletionWindow ((char)key, buf.GetIterAtMark (buf.InsertMark), true);
							}
						}*/
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
				if (EnableCodeCompletion && PeekCharIsWhitespace ()) {
//					CompletionWindow.ShowWindow ((char)key, buf.GetIterAtMark (buf.InsertMark), false, new CodeCompletionDataProvider (), this);
					CompletionListWindow.ShowWindow ((char)key, buf.GetIterAtMark (buf.InsertMark), new CodeCompletionDataProvider (), this);
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
				if (System.Char.IsWhiteSpace (doc, offset)) break;
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
			TextIter startIter = buf.GetIterAtOffset (start);
			TextIter offsetIter = buf.GetIterAtOffset (offset);
			buf.Delete (ref startIter, ref offsetIter);
			return start;
		}

		public string GetLeadingWhiteSpace (int line) {
			string lineText = ((IFormattableDocument)this).GetLineAsString (line);
			int index = 0;
			while (index < lineText.Length && System.Char.IsWhiteSpace (lineText[index])) {
    				index++;
    			}
 	   		return index > 0 ? lineText.Substring (0, index) : "";
		}
		
		public void InsertTemplate (CodeTemplate template)
		{
			TextIter iter = buf.GetIterAtMark (buf.InsertMark);
			int newCaretOffset = iter.Offset;
			string word = GetWordBeforeCaret ().Trim ();
			int beginLine = iter.Line;
			int endLine = beginLine;
			if (word.Length > 0)
				newCaretOffset = DeleteWordBeforeCaret ();
			
			string leadingWhiteSpace = GetLeadingWhiteSpace (beginLine);

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
						endLine++;
						break;
					default:
						buf.InsertAtCursor (template.Text[i].ToString ());
						newCaretOffset++;
						break;
				}
			}
			
			if (endLine > beginLine) {
				IndentLines (beginLine+1, endLine, leadingWhiteSpace);
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

			// If last line isn't selected, it's illogical to indent it.
			if (end.StartsLine())
				y1--;

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

			// If last line isn't selected, it's illogical to indent it.
			if (end.StartsLine())
				y1--;

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
			IndentLines (y0, y1, InsertSpacesInsteadOfTabs ? new string (' ', (int) TabsWidth) : "\t");
		}

		void IndentLines (int y0, int y1, string indent)
		{
			for (int l = y0; l <= y1; l ++) {
				TextIter it = Buffer.GetIterAtLine (l);
				if (!it.EndsLine())
					Buffer.Insert (ref it, indent);
			}
		}
		
		void UnIndentLines (int y0, int y1)
		{
			for (int l = y0; l <= y1; l ++) {
				TextIter start = Buffer.GetIterAtLine (l);
				TextIter end = start;
				
				char c = start.Char[0];
				
				if (c == '\t') {
					end.ForwardChar ();
					buf.Delete (ref start, ref end);
					
				} else if (c == ' ') {
					int cnt = 0;
					int max = (int) TabsWidth;
					
					while (cnt <= max && end.Char[0] == ' ' && ! end.EndsLine ()) {
						cnt ++;
						end.ForwardChar ();
					}
					
					if (cnt == 0)
						return;
					
					buf.Delete (ref start, ref end);
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
		
		void IFormattableDocument.ReplaceLine (int ln, string txt)
		{
			TextIter begin = Buffer.GetIterAtLine (ln);
			TextIter end = begin;
			end.ForwardToLineEnd ();
			
			Buffer.Delete (ref begin, ref end);
			Buffer.Insert (ref begin, txt);
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
			return it.Char[0];
		}
		
		void IFormattableDocument.Insert (int offset, string text)
		{
			TextIter insertIter = Buffer.GetIterAtOffset (offset);
			Buffer.Insert (ref insertIter, text);
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

		bool PeekCharIsWhitespace ()
		{
			TextIter start = buf.GetIterAtMark (buf.InsertMark);
			TextIter end = buf.GetIterAtLine (start.Line);
			end.LineOffset = start.LineOffset + 1;
			string text = buf.GetText (start, end, true);
			// if it is not whitespace or the start of a comment
			// we disable completion except for by ctl+space
			if (text.Length == 1)
				return System.Char.IsWhiteSpace (text[0]) || text[0] == '/';
			return true;
		}
#endregion
	}
}
