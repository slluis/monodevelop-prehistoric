using Gtk;
using GtkSharp;
using Gdk;

using System;
using System.IO;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditor : ScrolledWindow {
		
		public readonly SourceEditorBuffer Buffer;
		public readonly SourceEditorView View;
		public readonly SourceEditorDisplayBindingWrapper DisplayBinding;
		
		public SourceEditor (SourceEditorDisplayBindingWrapper bind) : base ()
		{
			ShadowType = Gtk.ShadowType.In;
			DisplayBinding = bind;
			Buffer = new SourceEditorBuffer ();
			
			View = new SourceEditorView (Buffer, this);
			

			Buffer.Highlight = true;
			
			View.SetMarkerPixbuf ("SourceEditorBookmark", new Gdk.Pixbuf (drag_icon_xpm));
			View.SetMarkerPixbuf ("BreakpointMark", new Gdk.Pixbuf ("../data/resources/icons/BreakPoint.png"));
			View.SetMarkerPixbuf ("ExecutionMark", new Gdk.Pixbuf ("../data/resources/icons/ExecutionMarker.png"));
			
			Add (View);
		}

		public void ExecutingAt (int linenumber)
		{
			Console.WriteLine ("Inside mainwidget");
			View.ExecutingAt (linenumber);
		}		

		public string Text {
			get { return Buffer.Text; }
			set { Buffer.Text = value; }
		}

		public void Replace (int offset, int length, string pattern)
		{
			Buffer.Replace (offset, length, pattern);
		}
		
		private static readonly string [] drag_icon_xpm = new string [] {
			"36 48 9 1",
			" 	c None",
			".	c #020204",
			"+	c #8F8F90",
			"@	c #D3D3D2",
			"#	c #AEAEAC",
			"$	c #ECECEC",
			"%	c #A2A2A4",
			"&	c #FEFEFC",
			"*	c #BEBEBC",
			"               .....................",
			"              ..&&&&&&&&&&&&&&&&&&&.",
			"             ...&&&&&&&&&&&&&&&&&&&.",
			"            ..&.&&&&&&&&&&&&&&&&&&&.",
			"           ..&&.&&&&&&&&&&&&&&&&&&&.",
			"          ..&&&.&&&&&&&&&&&&&&&&&&&.",
			"         ..&&&&.&&&&&&&&&&&&&&&&&&&.",
			"        ..&&&&&.&&&@&&&&&&&&&&&&&&&.",
			"       ..&&&&&&.*$%$+$&&&&&&&&&&&&&.",
			"      ..&&&&&&&.%$%$+&&&&&&&&&&&&&&.",
			"     ..&&&&&&&&.#&#@$&&&&&&&&&&&&&&.",
			"    ..&&&&&&&&&.#$**#$&&&&&&&&&&&&&.",
			"   ..&&&&&&&&&&.&@%&%$&&&&&&&&&&&&&.",
			"  ..&&&&&&&&&&&.&&&&&&&&&&&&&&&&&&&.",
			" ..&&&&&&&&&&&&.&&&&&&&&&&&&&&&&&&&.",
			"................&$@&&&@&&&&&&&&&&&&.",
			".&&&&&&&+&&#@%#+@#@*$%$+$&&&&&&&&&&.",
			".&&&&&&&+&&#@#@&&@*%$%$+&&&&&&&&&&&.",
			".&&&&&&&+&$%&#@&#@@#&#@$&&&&&&&&&&&.",
			".&&&&&&@#@@$&*@&@#@#$**#$&&&&&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&@%&%$&&&&&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&.",
			".&&&&&&&&$#@@$&&&&&&&&&&&&&&&&&&&&&.",
			".&&&&&&&&&+&$+&$&@&$@&&$@&&&&&&&&&&.",
			".&&&&&&&&&+&&#@%#+@#@*$%&+$&&&&&&&&.",
			".&&&&&&&&&+&&#@#@&&@*%$%$+&&&&&&&&&.",
			".&&&&&&&&&+&$%&#@&#@@#&#@$&&&&&&&&&.",
			".&&&&&&&&@#@@$&*@&@#@#$#*#$&&&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&&&$%&%$&&&&&&&&.",
			".&&&&&&&&&&$#@@$&&&&&&&&&&&&&&&&&&&.",
			".&&&&&&&&&&&+&$%&$$@&$@&&$@&&&&&&&&.",
			".&&&&&&&&&&&+&&#@%#+@#@*$%$+$&&&&&&.",
			".&&&&&&&&&&&+&&#@#@&&@*#$%$+&&&&&&&.",
			".&&&&&&&&&&&+&$+&*@&#@@#&#@$&&&&&&&.",
			".&&&&&&&&&&$%@@&&*@&@#@#$#*#&&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&$%&%$&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&.",
			".&&&&&&&&&&&&&&$#@@$&&&&&&&&&&&&&&&.",
			".&&&&&&&&&&&&&&&+&$%&$$@&$@&&$@&&&&.",
			".&&&&&&&&&&&&&&&+&&#@%#+@#@*$%$+$&&.",
			".&&&&&&&&&&&&&&&+&&#@#@&&@*#$%$+&&&.",
			".&&&&&&&&&&&&&&&+&$+&*@&#@@#&#@$&&&.",
			".&&&&&&&&&&&&&&$%@@&&*@&@#@#$#*#&&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&&&&&$%&%$&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&.",
			".&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&.",
			"...................................."
		};
	}
}
