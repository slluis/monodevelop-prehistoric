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
	public class SourceEditorDisplayBinding : IDisplayBinding
	{
		
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
			TextWriter s = new StreamWriter (fileName, false);
			s.Write (se.Text);
			s.Close ();
		}
		
		public override void Load (string fileName)
		{
			if (fileName.EndsWith (".cs"))
				se.LoadFile (fileName, "text/x-csharp");
			else
				se.LoadFile (fileName);
			
			ContentName = fileName;
		}
		
		public void LoadString (string mime, string val)
		{
			if (mime != null)
				se.LoadText (val, mime);
			else
				se.LoadText (val);
		}
		
#region IEditable
		public IClipboardHandler ClipboardHandler {
			get { return this; }
		}
		
		public string Text {
			get { return se.Text; }
			set { se.Text = value; }
		}
		
		public void Undo ()
		{
			se.buffer.Undo ();
		}
		
		public void Redo ()
		{
			se.buffer.Redo ();
		}
#endregion
#region IClipboardHandler
		//
		// TODO: All of this ;-)
		//
		
		bool HasSelection {
			get {
				TextIter dummy, dummy2;
				return se.buffer.GetSelectionBounds (out dummy, out dummy2);
			}
		}
		
		public bool EnableCut {
			get { return HasSelection; }
		}
		
		public bool EnableCopy {
			get { return HasSelection; }
		}
		
		public bool EnablePaste {
			get { return true; }
		}
		
		public bool EnableDelete {
			get { return HasSelection; }
		}
		
		public bool EnableSelectAll {
			get { return true; }
		}
		
		public void Cut (object sender, EventArgs e)
		{
		}
		
		public void Copy (object sender, EventArgs e)
		{
		}
		
		public void Paste (object sender, EventArgs e)
		{
		}
		
		public void Delete (object sender, EventArgs e)
		{
			se.buffer.DeleteSelection (true, true);
		}
		
		public void SelectAll (object sender, EventArgs e)
		{
		}
#endregion
	}
}
