using Gtk;
using GtkSharp;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditor : ScrolledWindow {
		SourceView sv;
		internal SourceBuffer buffer;
		SourceLanguagesManager slm = new SourceLanguagesManager ();
		
		public SourceEditor ()
		{
			buffer = new SourceBuffer (new SourceTagTable ());
			sv = new SourceView (buffer);
			
			sv.AutoIndent = true;
			sv.SmartHomeEnd = true;
			sv.ShowLineNumbers = true;
			sv.ShowLineMarkers = true;
			buffer.Highlight = true;
			
			Add (sv);
		}
		
		public void LoadFile (string file, string mime)
		{
			LoadText (File.OpenText (file).ReadToEnd (), mime);
		}
		
		public void LoadFile (string file)
		{
			buffer.Text = File.OpenText (file).ReadToEnd ();
		}
		
		public void LoadText (string text, string mime)
		{
			buffer.Text = text;
			buffer.Language = slm.GetLanguageFromMimeType (mime);
		}
		
		public void LoadText (string text)
		{
			buffer.Text = text;
		}
		
		public string Text {
			get { return buffer.Text; }
			set { buffer.Text = value; }
		}
	}
}