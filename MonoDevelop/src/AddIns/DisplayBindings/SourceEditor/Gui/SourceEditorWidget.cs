using Gtk;
using GtkSharp;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditor : ScrolledWindow {
		SourceView sv;
		SourceBuffer sb;
		SourceLanguagesManager slm = new SourceLanguagesManager ();
		
		public SourceEditor ()
		{
			sb = new SourceBuffer (new SourceTagTable ());
			sv = new SourceView (sb);
			
			sv.AutoIndent = true;
			sv.SmartHomeEnd = true;
			sv.ShowLineNumbers = true;
			sv.ShowLineMarkers = true;
			sb.Highlight = true;
			
			Add (sv);
		}
		
		public void LoadFile (string file, string mime)
		{
			LoadText (File.OpenText (file).ReadToEnd (), mime);
		}
		
		public void LoadText (string text, string mime)
		{
			sb.Text = text;
			sb.Language = slm.GetLanguageFromMimeType (mime);
		}
		
		public string Text { get { return sb.Text; } }
	}
}