using System;

using Gtk;
using Gnome;

namespace MonoDevelop.Gui.Widgets
{

	public class PropertyGridItem
	{

		ELabel name;
		Frame label;
		Gtk.Widget editor;

		public PropertyGridItem (string text, Gtk.Widget itemEditor)
		{
			name = new ELabel (text);
			EventBox evnt = new EventBox ();
			evnt.Add (name);
			label = new Gtk.Frame ();
			evnt.ModifyBg (Gtk.StateType.Normal, new Gdk.Color (System.Drawing.Color.White));
			label.Add (evnt);
			editor = itemEditor;
		}

		public Gtk.Widget Label {
			get { return label; }
		}

		public Gtk.Widget Editor {
			get { return editor; }
		}
	}
}
