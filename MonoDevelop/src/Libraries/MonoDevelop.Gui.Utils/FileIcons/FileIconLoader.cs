using System;
using System.Collections;

using Gnome;

namespace MonoDevelop.Gui.Utils
{

	public class FileIconLoader
	{

		static Gnome.IconTheme iconTheme;
		static Gnome.ThumbnailFactory thumbnailFactory;
		static Hashtable iconHash;

		static FileIconLoader ()
		{
			iconTheme = new Gnome.IconTheme ();
			thumbnailFactory = new Gnome.ThumbnailFactory (ThumbnailSize.Normal);
			iconHash = new Hashtable ();
		}

		private FileIconLoader ()
		{
		}

		public static Gdk.Pixbuf GetPixbufForFile (string filename, int width, int height)
		{
			Gnome.IconLookupResultFlags result;
			string icon = Gnome.Icon.LookupSync (iconTheme, thumbnailFactory, filename, "", Gnome.IconLookupFlags.None, out result);
			Gdk.Pixbuf big_pixbuf = (Gdk.Pixbuf) iconHash [icon];
			if (big_pixbuf == null) {
				big_pixbuf = GetPixbufForType (icon);
				iconHash [icon] = big_pixbuf;
			}
			return big_pixbuf.ScaleSimple (height, width, Gdk.InterpType.Bilinear);
		}

		public static Gdk.Pixbuf GetPixbufForType (string type)
		{
			int i;
			string p_filename = iconTheme.LookupIcon (type, 24, new Gnome.IconData (), out i);
			return new Gdk.Pixbuf (p_filename);
		}
	}
}
