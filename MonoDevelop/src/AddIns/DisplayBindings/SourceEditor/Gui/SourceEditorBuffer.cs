using Gtk;
using GtkSharp;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditorBuffer : SourceBuffer {
		
		SourceLanguagesManager slm = new SourceLanguagesManager ();
		
		public SourceEditorBuffer () : base (new SourceTagTable ())
		{
		}
		
		public void LoadFile (string file, string mime)
		{
			LoadText (File.OpenText (file).ReadToEnd (), mime);
		}
		
		public void LoadFile (string file)
		{
			Text = File.OpenText (file).ReadToEnd ();
		}
		
		public void LoadText (string text, string mime)
		{
			Text = text;
			Language = slm.GetLanguageFromMimeType (mime);
		}
		
		public void LoadText (string text)
		{
			Text = text;
		}
		
		public void Save (string fileName)
		{
			TextWriter s = new StreamWriter (fileName, false);
			s.Write (Text);
			s.Close ();
		}
	}
}