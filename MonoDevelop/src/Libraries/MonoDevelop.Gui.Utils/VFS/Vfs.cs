using System;
using System.Runtime.InteropServices;

namespace MonoDevelop.GuiUtils
{
	public class Vfs
	{
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_init ();
		
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_initialized ();
	
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_shutdown ();
		
		[DllImport ("gnomevfs-2")]
		static extern string gnome_vfs_get_mime_type (string uri);
		
		[DllImport ("gnomevfs-2")]
		static extern string gnome_vfs_get_mime_type_for_data (string data, int length);
		
		[DllImport ("gnomevfs-2")]
		static extern string gnome_vfs_mime_get_icon (string mime_type);
		
		[DllImport ("gnomevfs-2")]
		static extern bool gnome_vfs_mime_type_is_known (string mime_type);
		
		// gnome_program_init calls this for you
		public static bool Init ()
		{
			return gnome_vfs_init ();
		}
		
		public static string GetIcon (string mimetype)
		{
			return gnome_vfs_mime_get_icon (mimetype);
		}
		
		public static string GetMimeType (string filename)
		{
			return gnome_vfs_get_mime_type (filename);
		}
		
		public static string GetMimeTypeFromData (string data)
		{
			return gnome_vfs_get_mime_type_for_data (data, data.Length);
		}
		
		public static bool IsKnownType (string mimetype)
		{
			return gnome_vfs_mime_type_is_known (mimetype);
		}
		
		public static bool Shutdown ()
		{
			return gnome_vfs_shutdown ();
		}
		
		public static bool Initialized
		{
			get { return gnome_vfs_init (); }
		}
	}
}
