using Gtk;

using System;
using System.Runtime.InteropServices;
	
namespace MonoDevelop.SourceEditor.Gui
{
	public class SourceEditor : ScrolledWindow
	{	
		public SourceEditorBuffer Buffer;
		public SourceEditorView View;
		public SourceEditorDisplayBindingWrapper DisplayBinding;
		
		static Gdk.Pixbuf dragIconPixbuf;
		static Gdk.Pixbuf executionMarkerPixbuf;
		static Gdk.Pixbuf breakPointPixbuf;
		
		static SourceEditor ()
		{
			dragIconPixbuf = new Gdk.Pixbuf (drag_icon_xpm);
			executionMarkerPixbuf = new Gdk.Pixbuf ("../data/resources/icons/ExecutionMarker.png");
			breakPointPixbuf = new Gdk.Pixbuf ("../data/resources/icons/BreakPoint.png");
		}
		
		protected SourceEditor (IntPtr ptr): base (ptr)
		{
		}
		
		public SourceEditor (SourceEditorDisplayBindingWrapper bind)
		{
			ShadowType = Gtk.ShadowType.In;
			DisplayBinding = bind;
			Buffer = new SourceEditorBuffer ();	
			View = new SourceEditorView (Buffer, this);
			Buffer.View = View;
			this.VscrollbarPolicy = PolicyType.Automatic;
			this.HscrollbarPolicy = PolicyType.Automatic;
			
			View.SetMarkerPixbuf ("SourceEditorBookmark", dragIconPixbuf);
			View.SetMarkerPixbuf ("ExecutionMark", executionMarkerPixbuf);
			View.SetMarkerPixbuf ("BreakpointMark", breakPointPixbuf);
			
			Add (View);
		}
		
		public new void Dispose ()
		{
			Buffer.Dispose ();
			Buffer = null;
			Remove (View);
			View.Dispose ();
			View = null;
			DisplayBinding = null;
			base.Dispose ();
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
