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
			string icon;
			try {
				if (filename == "Documentation")
					icon = "gnome-fs-regular";
				else
					icon = Gnome.Icon.LookupSync (iconTheme, thumbnailFactory, filename, "", Gnome.IconLookupFlags.None, out result);
			} catch {
				icon = "gnome-fs-regular";
			}
			Gdk.Pixbuf pix = GetPixbufForType (icon);
			return pix.ScaleSimple (height, width, Gdk.InterpType.Bilinear);
		}

		public static Gdk.Pixbuf GetPixbufForType (string type)
		{
			Gdk.Pixbuf bf = (Gdk.Pixbuf) iconHash [type];
			if (bf == null) {
				int i;
				string p_filename = "gnome-fs-regular";
				try {
					p_filename = iconTheme.LookupIcon (type, 24, new Gnome.IconData (), out i);
					if (p_filename == "") {
						return new Gdk.Pixbuf ("../data/resources/icons/gnome-fs-regular.png");
					}
				} catch {
					return new Gdk.Pixbuf ("../data/resources/icons/gnome-fs-regular.png");
				}
				bf = new Gdk.Pixbuf (p_filename);
				iconHash [type] = bf;
			}
			return bf;
		}
	}
}
