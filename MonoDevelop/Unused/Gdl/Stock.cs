namespace Gdl {

	using System;
	using System.Collections;
	using System.Runtime.InteropServices;
	using Gtk;

	public class Stock {

		static Gtk.IconFactory stock = new Gtk.IconFactory ();

		public static string Close {
			 get { return "gdl-close"; }
		}
		public static string MenuLeft {
			 get { return "gdl-menu-left"; }
		}
		public static string MenuRight {
			 get { return "gdl-menu-right"; }
		}
		
		static Stock ()
		{
			AddIcon ("gdl-close", "stock-close-12.png");
			AddIcon ("gdl-menu-left", "stock-menu-left-12.png");
			AddIcon ("gdl-menu-right", "stock-menu-right-12.png");
			
			stock.AddDefault ();
		}
		
		static void AddIcon (string stockid, string file)
		{
			Gtk.IconSet iconset = stock.Lookup (stockid);
			
			if (iconset == null) {
				iconset = new Gtk.IconSet ();
				Gdk.Pixbuf img = new Gdk.Pixbuf (file);
				IconSource source = new IconSource ();
				source.Size = Gtk.IconSize.Menu;
				source.SizeWildcarded = false;
				source.Pixbuf = img;
				iconset.AddSource (source);
				stock.Add (stockid, iconset);
			}
		}
	}
}
