using Gtk;
using GtkSharp;
using GLib;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns.Codons;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {

	public enum SourceMarkerType {
		SourceEditorBookmark,
		BreakpointMark,
		ExecutionMark
	}

	// This gives us a nice way to avoid the try/finally
	// which is really long.
	struct NoUndo : IDisposable {
		SourceEditorBuffer b;
		
		public NoUndo (SourceEditorBuffer b) {
			this.b = b;
			b.BeginNotUndoableAction ();
		}
		
		public void Dispose ()
		{
			b.EndNotUndoableAction ();
		}
	}
	
	// This gives us a nice way to avoid the try/finally
	// which is really long.
	struct AtomicUndo : IDisposable {
		SourceEditorBuffer b;
		
		public AtomicUndo (SourceEditorBuffer b) {
			this.b = b;
			b.BeginUserAction ();
		}
		
		public void Dispose ()
		{
			b.EndUserAction ();
		}
	}
	
	public class SourceEditorBuffer : SourceBuffer, IClipboardHandler {
		
		SourceLanguagesManager slm = new SourceLanguagesManager ();
		
		public SourceEditorBuffer () : base (new SourceTagTable ())
		{
		}
		
		public void LoadFile (string file, string mime)
		{
			LoadText (File.OpenText (file).ReadToEnd (), mime);		
			Modified = false;
		}
		
		public void LoadFile (string file)
		{
			using (NoUndo n = new NoUndo (this))
				Text = File.OpenText (file).ReadToEnd ();
			
			Modified = false;
		}
		
		public void LoadText (string text, string mime)
		{
			
			SourceLanguage lang = slm.GetLanguageFromMimeType (mime);
			if (lang != null) 
				Language = lang;
			
			using (NoUndo n = new NoUndo (this))
				Text = text;
			
			Modified = false;
		}
		
		public void LoadText (string text)
		{
			using (NoUndo n = new NoUndo (this))
				Text = text;
			
			Modified = false;
		}
		
		public void Save (string fileName)
		{
			TextWriter s = new StreamWriter (fileName, false);
			s.Write (Text);
			s.Close ();
			Modified = false;
		}

#region IClipboardHandler
		bool HasSelection {
			get {
				TextIter dummy, dummy2;
				return GetSelectionBounds (out dummy, out dummy2);
			}
		}

		public string GetSelectedText () {
			if (HasSelection)
			{
				TextIter select1, select2;
				GetSelectionBounds (out select1, out select2);
				return GetText (select1, select2, true);
			}
			return String.Empty;
		}
		
		bool IClipboardHandler.EnableCut {
			get { return true; }
		}
		
		bool IClipboardHandler.EnableCopy {
			get { return true; }
		}
		
		bool IClipboardHandler.EnablePaste {
			get {
				return true;
				//return clipboard.WaitIsTextAvailable ();
			}
		}
		
		bool IClipboardHandler.EnableDelete {
			get { return true; }
		}
		
		bool IClipboardHandler.EnableSelectAll {
			get { return true; }
		}
		
		void IClipboardHandler.Cut (object sender, EventArgs e)
		{
			if (HasSelection)
				CutClipboard (clipboard, true);
		}
		
		void IClipboardHandler.Copy (object sender, EventArgs e)
		{
			if (HasSelection)
				CopyClipboard (clipboard);
		}
		
		void IClipboardHandler.Paste (object sender, EventArgs e)
		{
			if (clipboard.WaitIsTextAvailable ())
				PasteClipboard (clipboard);
		}
		
		void IClipboardHandler.Delete (object sender, EventArgs e)
		{
			if (HasSelection)
				DeleteSelection (true, true);
		}
		
		void IClipboardHandler.SelectAll (object sender, EventArgs e)
		{
			// Sadly, this is not in our version of the bindings:
			//
			//gtk_text_view_select_all (GtkWidget *widget,
			//			  gboolean select)
			//{
			//	gtk_text_buffer_get_bounds (buffer, &start_iter, &end_iter);
			//	gtk_text_buffer_move_mark_by_name (buffer, "insert", &start_iter);
			//	gtk_text_buffer_move_mark_by_name (buffer, "selection_bound", &end_iter);
			
			MoveMark ("insert", StartIter);
			MoveMark ("selection_bound", EndIter);
		}
		
		Gtk.Clipboard clipboard = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
#endregion

#region Bookmark Operations
		
		//
		// Ok, the GtkSourceView people made this extremely dificult because they took over
		// the TextMark type for their SourceMarker, So i have to marshall for them. It is annoying
		// I filed a bug.
		//
		// Again, this is retarded
		//
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_buffer_get_markers_in_region (IntPtr raw, ref Gtk.TextIter begin, ref Gtk.TextIter end);
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_buffer_create_marker(IntPtr raw, string name, string type, ref Gtk.TextIter where);

		[DllImport("gtksourceview-1.0")]
		static extern void gtk_source_buffer_delete_marker(IntPtr raw, IntPtr marker);
		
		[DllImport("gtksharpglue")]
		static extern IntPtr gtksharp_slist_get_data (IntPtr l);

		[DllImport("gtksharpglue")]
		static extern IntPtr gtksharp_slist_get_next (IntPtr l);
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_marker_get_marker_type(IntPtr raw);
		
		[DllImport("libglib-2.0-0.dll")]
		static extern void g_slist_free (IntPtr l);
		
		[DllImport("gtksourceview-1.0")]
		static extern void gtk_source_buffer_get_iter_at_marker (IntPtr raw, ref Gtk.TextIter iter, IntPtr marker);
		
		public void ToggleBookmark ()
		{
			ToggleBookmark (GetIterAtMark (InsertMark).Line);
		}
		
		public bool IsBookmarked (int linenum)
		{
			return IsMarked (linenum, SourceMarkerType.SourceEditorBookmark);
		}

		public bool IsBreakpoint (int linenum)
		{
			return IsMarked (linenum, SourceMarkerType.BreakpointMark);
		}
		
		public bool IsMarked (int linenum, SourceMarkerType type)
		{
			TextIter insert = GetIterAtLine (linenum);
			TextIter begin_line = insert, end_line = insert;
			begin_line.LineOffset = 0;

			while (! end_line.EndsLine ())
				end_line.ForwardChar ();
			
			IntPtr lst = gtk_source_buffer_get_markers_in_region (Handle, ref begin_line, ref end_line);

			bool fnd_marker = false;
			
			IntPtr current = lst;
			while (current != IntPtr.Zero)
			{
				IntPtr data = gtksharp_slist_get_data (current);
				IntPtr nm = gtk_source_marker_get_marker_type (data);

				string name = GLibSharp.Marshaller.PtrToStringGFree (nm);
				if (name == type.ToString ()) {
					fnd_marker = true;
					break;
				}
				current = gtksharp_slist_get_next (current);
			}

			if (lst != IntPtr.Zero)
				g_slist_free (lst);

			return fnd_marker;

		}

		public void ToggleBookmark (int linenum)
		{
			ToggleMark (linenum, SourceMarkerType.SourceEditorBookmark);
		}
		
		public void ToggleMark (int linenum, SourceMarkerType type)
		{
			TextIter insert = GetIterAtLine (linenum);
			TextIter begin_line = insert, end_line = insert;
			begin_line.LineOffset = 0;
			
			while (! end_line.EndsLine ())
				end_line.ForwardChar ();
			
			
			IntPtr lst = gtk_source_buffer_get_markers_in_region (Handle, ref begin_line, ref end_line);

			bool found_marker = false;
			
			//
			// Ok, again, this is absurd. The problem is that the buffer owns the
			// reference to the marker. So, if we use the nice Gtk# stuff, we get
			// a problem when we dispose it later. Thus we must basically write this
			// in C. It sucks. It really sucks.
			//
			
			IntPtr current = lst;
			while (current != IntPtr.Zero) {
				
				IntPtr data = gtksharp_slist_get_data (current);
				IntPtr nm = gtk_source_marker_get_marker_type (data);
				string name = GLibSharp.Marshaller.PtrToStringGFree (nm);
				if (name == type.ToString ()) {
					gtk_source_buffer_delete_marker (Handle, data);
					found_marker = true;
				}
				
				current = gtksharp_slist_get_next (current);
			}
			
			if (lst != IntPtr.Zero)
				g_slist_free (lst);
			
			if (found_marker)
				return;
			
			switch (type.ToString ()) {
			case "ExecutionMark":
				begin_line.LineOffset = 2;
				break;
			case "BreakpointMark":
				begin_line.LineOffset = 1;
				break;
			}

			gtk_source_buffer_create_marker (Handle, null, type.ToString (), ref begin_line);
		}
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_buffer_get_prev_marker(IntPtr raw, ref Gtk.TextIter iter);
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_buffer_get_last_marker(IntPtr raw);
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_marker_prev (IntPtr raw);
		
		public void PrevBookmark ()
		{
			TextIter loc = GetIterAtMark (InsertMark);
			int ln = loc.Line;
			
			IntPtr prevMarker = gtk_source_buffer_get_prev_marker (Handle, ref loc);
			IntPtr firstMarker = prevMarker;
			bool first = true;
			while (true) {
				// Thats a wrap!
				if (prevMarker == IntPtr.Zero)
					prevMarker = gtk_source_buffer_get_last_marker (Handle);
				
				// no markers
				if (prevMarker == IntPtr.Zero)
					return;
				
				IntPtr nm = gtk_source_marker_get_marker_type (prevMarker);
				string name = GLibSharp.Marshaller.PtrToStringGFree (nm);
				if (name == "SourceEditorBookmark") {
					gtk_source_buffer_get_iter_at_marker (Handle, ref loc, prevMarker);
					
					if (! first || loc.Line != ln)
						break;
				}
				
				prevMarker = gtk_source_marker_prev (prevMarker);
				
				first = false;
			}
			
			PlaceCursor (loc);
		}
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_buffer_get_first_marker (IntPtr raw);
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_buffer_get_next_marker(IntPtr raw, ref Gtk.TextIter iter);
		
		[DllImport("gtksourceview-1.0")]
		static extern IntPtr gtk_source_marker_next(IntPtr raw);
		
		public void NextBookmark ()
		{
			TextIter loc = GetIterAtMark (InsertMark);
			int ln = loc.Line;
			
			IntPtr nextMarker = gtk_source_buffer_get_next_marker (Handle, ref loc);
			IntPtr firstMarker = nextMarker;
			bool first = true;
			while (true) {
				// Thats a wrap!
				if (nextMarker == IntPtr.Zero)
					nextMarker = gtk_source_buffer_get_first_marker (Handle);
				
				// no markers
				if (nextMarker == IntPtr.Zero)
					return;
				
				IntPtr nm = gtk_source_marker_get_marker_type (nextMarker);
				string name = GLibSharp.Marshaller.PtrToStringGFree (nm);
				if (name == "SourceEditorBookmark") {
					gtk_source_buffer_get_iter_at_marker (Handle, ref loc, nextMarker);
					
					if (! first || loc.Line != ln)
						break;
				}
				
				nextMarker = gtk_source_marker_next (nextMarker);
				
				first = false;
			}
			
			PlaceCursor (loc);
		}

		public void ClearBookmarks ()
		{
			ClearMarks (SourceMarkerType.SourceEditorBookmark);
		}
		
		public void ClearMarks (SourceMarkerType type)
		{
			TextIter begin = StartIter;
			TextIter end = EndIter;
			IntPtr lst = gtk_source_buffer_get_markers_in_region (Handle, ref begin, ref end);
			
			IntPtr current = lst;
			while (current != IntPtr.Zero) {
				
				IntPtr data = gtksharp_slist_get_data (current);
				IntPtr nm = gtk_source_marker_get_marker_type (data);
				string name = GLibSharp.Marshaller.PtrToStringGFree (nm);
				if (name == type.ToString ())
					gtk_source_buffer_delete_marker (Handle, data);
				
				current = gtksharp_slist_get_next (current);
			}
			
			if (lst != IntPtr.Zero)
				g_slist_free (lst);
		}
#endregion

#region ITextBufferStrategy compat interface, this should be removed ASAP

		public int Length
		{
			get { return EndIter.Offset + 1; }
		}

		public char GetCharAt (int offset)
		{
			if (offset < 0)
				offset = 0;
			TextIter begin_iter = GetIterAtOffset (offset);
			TextIter next_iter = begin_iter;
			next_iter.ForwardChar ();
			string text = GetText (begin_iter, next_iter, true);
			if (text != null && text.Length >= 1)
				return text[0];
			return ' ';
		}

		public string GetText (int start, int length)
		{
			TextIter begin_iter = GetIterAtOffset (start);
			TextIter end_iter = GetIterAtOffset (start + length);
			return GetText (begin_iter, end_iter, true);
		}

		public void Insert (int offset, string text)
		{
			TextIter put = GetIterAtOffset (offset);
			Insert (put, text);
		}

		public int GetLowerSelectionBounds ()
		{
			if (HasSelection)
			{
				TextIter select1, select2;
				GetSelectionBounds (out select1, out select2);
				return select1.Offset > select2.Offset ? select2.Offset : select1.Offset;
			}
			return 0;
		}

		public void Delete (int offset, int length)
		{
			TextIter start = GetIterAtOffset (offset);
			TextIter end = GetIterAtOffset (offset + length);

			Delete (start, end);
		}

		public void Replace (int offset, int length, string pattern)
		{
			Delete (offset, length);
			Insert (offset, pattern);
		}

		public static SourceEditorBuffer CreateTextBufferFromFile (string filename)
		{
			SourceEditorBuffer buff = new SourceEditorBuffer ();
			buff.LoadFile (filename);
			return buff;
		}

#endregion
	}
}
