using Gtk;
using GtkSharp;

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
	public class SourceEditorBuffer : SourceBuffer, IClipboardHandler {
		
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
			Language = slm.GetLanguageFromMimeType (mime);
			
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
	}
}