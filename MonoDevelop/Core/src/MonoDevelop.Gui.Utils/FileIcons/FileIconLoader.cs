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
				const string default_icon_location = "../data/resources/icons/gnome-fs-regular.png";
				string p_filename = "";
				try {
					int i;
					p_filename = iconTheme.LookupIcon (type, 24, new Gnome.IconData (), out i);
					if (p_filename.Equals ("")) {
						p_filename = default_icon_location;
					}
				} catch {
					p_filename = default_icon_location;
				}
				try {
					bf = new Gdk.Pixbuf (p_filename);
				} catch {
					bf = new Gdk.Pixbuf (default_icon_location);
				}
				iconHash [type] = bf;
			}
			return bf;
		}
	}
}
