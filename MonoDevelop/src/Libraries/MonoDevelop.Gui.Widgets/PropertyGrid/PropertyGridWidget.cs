using System;
using System.Collections;

using Gtk;
using Gdk;
using Gnome;

namespace MonoDevelop.Gui.Widgets
{

	public class PropertyGridWidget : ScrolledWindow
	{
		HBox intFrame;
		ArrayList propertyGroups;

		public PropertyGridWidget () : base ()
		{
			HBox intFrame = new HBox (true, 0);
			propertyGroups = new ArrayList ();
			PropertyGridGroup a = new PropertyGridGroup ("Appearence");
			Gtk.Entry test = new Gtk.Entry ();
			test.WidthChars = 10;
			Gtk.Entry test2 = new Gtk.Entry ();
			test2.WidthChars = 10;
			Gtk.Entry test3 = new Gtk.Entry ();
			test3.WidthChars = 10;
			a.AddGridItem ("test", test);
			a.AddGridItem ("test2", test2);
			a.AddGridItem ("testing elipsizing", test3);
			intFrame.PackStart (a);
			AddWithViewport (intFrame);
		}
		
		public ArrayList PropertyGroups {
			get { return propertyGroups; }
		}
		
	}

}
