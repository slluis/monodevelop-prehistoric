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
			return GetPixbufForType (icon).ScaleSimple (height, width, Gdk.InterpType.Bilinear);
		}

		public static Gdk.Pixbuf GetPixbufForType (string type)
		{
			Gdk.Pixbuf bf = (Gdk.Pixbuf) iconHash [type];
			if (bf == null) {
				int i;
				string p_filename = "gnome-fs-regular";
				try {
					p_filename = iconTheme.LookupIcon (type, 24, new Gnome.IconData (), out i);
				} catch {
					return null;
				}
				bf = new Gdk.Pixbuf (p_filename);
				iconHash [type] = bf;
			}
			return bf;
		}
	}
}
