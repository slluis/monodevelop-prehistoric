using Gtk;

using System;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui
{
	public class SourceEditor : ScrolledWindow
	{	
		public readonly SourceEditorBuffer Buffer;
		public readonly SourceEditorView View;
		public readonly SourceEditorDisplayBindingWrapper DisplayBinding;
		private static GLib.GType gtype;
		
		public static new GLib.GType GType
		{
			get
			{
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (SourceEditor));
				return gtype;
			}
		}
		
		public SourceEditor (SourceEditorDisplayBindingWrapper bind) : base (GType)
		{
			ShadowType = Gtk.ShadowType.In;
			DisplayBinding = bind;
			Buffer = new SourceEditorBuffer ();	
			View = new SourceEditorView (Buffer, this);
			Buffer.Highlight = true;
			Buffer.View = View;
			this.VscrollbarPolicy = PolicyType.Automatic;
			this.HscrollbarPolicy = PolicyType.Automatic;
			
			View.SetMarkerPixbuf ("SourceEditorBookmark", new Gdk.Pixbuf (drag_icon_xpm));
			View.SetMarkerPixbuf ("ExecutionMark", new Gdk.Pixbuf ("../data/resources/icons/ExecutionMarker.png"));
			View.SetMarkerPixbuf ("BreakpointMark", new Gdk.Pixbuf ("../data/resources/icons/BreakPoint.png"));
			
			Add (View);
		}

		public void ExecutingAt (int linenumber)
		{
			View.ExecutingAt (linenumber);
		}

		public void ClearExecutingAt (int linenumber)
		{
			View.ClearExecutingAt (linenumber);
		}

		public string Text
		{
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
