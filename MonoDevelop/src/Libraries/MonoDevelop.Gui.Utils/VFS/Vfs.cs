//
// Author: John Luke  <jluke@cfl.rr.com>
// License: LGPL
//

using System;
using System.Runtime.InteropServices;

namespace MonoDevelop.Gui.Utils
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
		
		private Vfs ()
		{
		}

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

	public class MimeApplication {
		[Flags]
		public enum ArgumentType {
			URIS,
			PATHS,
			URIS_FOR_NON_FILES,
		}
		[StructLayout(LayoutKind.Sequential)]
		public class Info {
			public string id;
			public string name;
			public string command;
			public bool can_open_multiple_files;
			public ArgumentType expects_uris;
			public IntPtr supported_uri_schemes;
			public bool requires_terminal;

			public IntPtr reserved1;
			public IntPtr reserved2;
		}

		[DllImport ("libgnomevfs-2")]
			extern static Info gnome_vfs_mime_get_default_application ( string mime_type );
		[DllImport ("libgnomevfs-2")]
			extern static void gnome_vfs_mime_application_free ( Info info );

		public static void Exec (string mime_type, string uri)
		{
			Info info;
			info = gnome_vfs_mime_get_default_application (mime_type);
			if (info == null)
			{
				// Can we please stop hard coding Nautilus!?
				System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo ("nautilus", "\"" + uri + "\"");
				psi.UseShellExecute = false;
				
				System.Diagnostics.Process.Start (psi);
			} else {
				System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo (info.command, "\"" + uri + "\"");
				psi.UseShellExecute = false;
				System.Diagnostics.Process.Start (psi);
			}
//FIXME:  Memory leak, causes crashes, dunno why...needs fixed
//			gnome_vfs_mime_application_free (info);
		}

	}


}
