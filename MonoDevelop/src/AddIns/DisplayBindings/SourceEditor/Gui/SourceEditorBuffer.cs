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
		
		bool IClipboardHandler.EnableCut {
			get { return HasSelection; }
		}
		
		bool IClipboardHandler.EnableCopy {
			get { return HasSelection; }
		}
		
		bool IClipboardHandler.EnablePaste {
			get {
				return clipboard.WaitIsTextAvailable ();
			}
		}
		
		bool IClipboardHandler.EnableDelete {
			get { return HasSelection; }
		}
		
		bool IClipboardHandler.EnableSelectAll {
			get { return true; }
		}
		
		void IClipboardHandler.Cut (object sender, EventArgs e)
		{
			CutClipboard (clipboard, true);
		}
		
		void IClipboardHandler.Copy (object sender, EventArgs e)
		{
			CopyClipboard (clipboard);
		}
		
		void IClipboardHandler.Paste (object sender, EventArgs e)
		{
			PasteClipboard (clipboard);
		}
		
		void IClipboardHandler.Delete (object sender, EventArgs e)
		{
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
			TextIter insert = GetIterAtMark (InsertMark);
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
				if (name == "SourceEditorBookmark") {
					gtk_source_buffer_delete_marker (Handle, data);
					found_marker = true;
				}
				
				current = gtksharp_slist_get_next (current);
			}
			
			if (lst != IntPtr.Zero)
				g_slist_free (lst);
			
			if (found_marker)
				return;
			
			gtk_source_buffer_create_marker (Handle, null, "SourceEditorBookmark", ref begin_line);
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
			TextIter begin = StartIter;
			TextIter end = EndIter;
			IntPtr lst = gtk_source_buffer_get_markers_in_region (Handle, ref begin, ref end);
			
			IntPtr current = lst;
			while (current != IntPtr.Zero) {
				
				IntPtr data = gtksharp_slist_get_data (current);
				IntPtr nm = gtk_source_marker_get_marker_type (data);
				string name = GLibSharp.Marshaller.PtrToStringGFree (nm);
				if (name == "SourceEditorBookmark")
					gtk_source_buffer_delete_marker (Handle, data);
				
				current = gtksharp_slist_get_next (current);
			}
			
			if (lst != IntPtr.Zero)
				g_slist_free (lst);
		}
#endregion

	}
}
