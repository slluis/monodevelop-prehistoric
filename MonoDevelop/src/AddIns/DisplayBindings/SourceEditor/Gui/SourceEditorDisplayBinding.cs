using System;
using System.IO;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns.Codons;

using Gtk;

namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditorDisplayBinding : IDisplayBinding {
		
		public virtual bool CanCreateContentForFile (string fileName)
		{
			return true;
		}
		
		public virtual bool CanCreateContentForLanguage (string language)
		{
			return true;
		}
		
		public virtual IViewContent CreateContentForFile (string fileName)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			w.Load (fileName);
			return w;
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			// HACK HACK
			if (language == "C#")
				language = "text/x-csharp";
			else
				language = null;
			
			w.LoadString (language, content);
			return w;
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content, string new_file_name)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			// HACK HACK
			if (language == "C#")
				language = "text/x-csharp";
			else
				language = null;
			
			w.LoadString (language, content);
			return w;
		}	
	}
	
	public class SourceEditorDisplayBindingWrapper : AbstractViewContent,
		IEditable, IClipboardHandler
	{
		internal SourceEditor se;
		
		public override Gtk.Widget Control {
			get {
				return se;
			}
		}
		
		public override string TabPageText {
			get {
				return "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			}
		}
		
		public SourceEditorDisplayBindingWrapper ()
		{
			se = new SourceEditor ();
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override void Save (string fileName)
		{
			se.Buffer.Save (fileName);
		}
		
		public override void Load (string fileName)
		{
			if (fileName.EndsWith (".cs"))
				se.Buffer.LoadFile (fileName, "text/x-csharp");
			else
				se.Buffer.LoadFile (fileName);
			
			ContentName = fileName;
		}
		
		public void LoadString (string mime, string val)
		{
			if (mime != null)
				se.Buffer.LoadText (val, mime);
			else
				se.Buffer.LoadText (val);
		}
		
#region IEditable
		public IClipboardHandler ClipboardHandler {
			get { return this; }
		}
		
		public string Text {
			get { return se.Buffer.Text; }
			set { se.Buffer.Text = value; }
		}
		
		public void Undo ()
		{
			se.Buffer.Undo ();
		}
		
		public void Redo ()
		{
			se.Buffer.Redo ();
		}
#endregion
#region IClipboardHandler
		//
		// TODO: All of this ;-)
		//
		
		bool HasSelection {
			get {
				TextIter dummy, dummy2;
				return se.Buffer.GetSelectionBounds (out dummy, out dummy2);
			}
		}
		
		public bool EnableCut {
			get { return HasSelection; }
		}
		
		public bool EnableCopy {
			get { return HasSelection; }
		}
		
		public bool EnablePaste {
			get {
				return clipboard.WaitIsTextAvailable ();
			}
		}
		
		public bool EnableDelete {
			get { return HasSelection; }
		}
		
		public bool EnableSelectAll {
			get { return true; }
		}
		
		public void Cut (object sender, EventArgs e)
		{
			se.Buffer.CutClipboard (clipboard, true);
		}
		
		public void Copy (object sender, EventArgs e)
		{
			se.Buffer.CopyClipboard (clipboard);
		}
		
		public void Paste (object sender, EventArgs e)
		{
			se.Buffer.PasteClipboard (clipboard);
		}
		
		public void Delete (object sender, EventArgs e)
		{
			se.Buffer.DeleteSelection (true, true);
		}
		
		public void SelectAll (object sender, EventArgs e)
		{
			// Sadly, this is not in our version of the bindings:
			//
			//gtk_text_view_select_all (GtkWidget *widget,
			//			  gboolean select)
			//{
			//	gtk_text_buffer_get_bounds (buffer, &start_iter, &end_iter);
			//	gtk_text_buffer_move_mark_by_name (buffer, "insert", &start_iter);
			//	gtk_text_buffer_move_mark_by_name (buffer, "selection_bound", &end_iter);
			
			se.Buffer.MoveMark ("insert", se.Buffer.StartIter);
			se.Buffer.MoveMark ("selection_bound", se.Buffer.EndIter);
		}
		
		Gtk.Clipboard clipboard = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
#endregion
	}
}
