using System;
using System.Runtime.InteropServices;

namespace MonoDevelop.Gui
{
	public class Vfs
	{
		[DllImport ("gnomevfs-2")]
		static extern string gnome_vfs_mime_type_from_name (string filename);
		
		[DllImport ("gnomevfs-2")]
		static extern string gnome_vfs_mime_type_from_name_or_default (string filename, string defaultval);

		public static string GetMimeType (string filename)
		{
			return gnome_vfs_mime_type_from_name (filename);
		}
		
		public static string GetMimeTypeDefault (string filename, string defaultval)
		{
			return gnome_vfs_mime_type_from_name_or_default (filename, defaultval);
		}
	}
}
