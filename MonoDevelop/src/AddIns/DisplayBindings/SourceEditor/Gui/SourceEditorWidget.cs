using Gtk;
using GtkSharp;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditor : ScrolledWindow {
		
		public readonly SourceEditorBuffer Buffer;
		SourceView sv;
		
		public SourceEditor ()
		{
			Buffer = new SourceEditorBuffer ();
			
			sv = new SourceView (Buffer);
			
			sv.AutoIndent = true;
			sv.SmartHomeEnd = true;
			sv.ShowLineNumbers = true;
			sv.ShowLineMarkers = true;
			Buffer.Highlight = true;
			
			Add (sv);
		}
		
		public string Text {
			get { return Buffer.Text; }
			set { Buffer.Text = value; }
		}
	}
}