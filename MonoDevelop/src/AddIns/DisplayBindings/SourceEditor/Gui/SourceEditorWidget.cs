using Gtk;
using GtkSharp;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditor : ScrolledWindow {
		
		public readonly SourceEditorBuffer Buffer;
		public readonly SourceView View;
		
		public SourceEditor ()
		{
			Buffer = new SourceEditorBuffer ();
			
			View = new SourceView (Buffer);
			
			View.AutoIndent = true;
			View.SmartHomeEnd = true;
			View.ShowLineNumbers = true;
			View.ShowLineMarkers = true;
			Buffer.Highlight = true;
			
			Add (View);
		}
		
		public string Text {
			get { return Buffer.Text; }
			set { Buffer.Text = value; }
		}
	}
}