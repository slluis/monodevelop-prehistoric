using System;
using System.Runtime.InteropServices;

namespace MonoDevelop.Gui.Utils
{

	public class Glue
	{
	
		private Glue ()
		{
		}

		[DllImport ("monodevelop")]
		static extern void lmd_propagate_eventkey (IntPtr Handle, ref Gdk.EventKey key);

		public static void SimulateKeyPress (IntPtr Handle, ref Gdk.EventKey key)
		{
			lmd_propagate_eventkey (Handle, ref key);
		}

		[DllImport ("gtk-x11-2.0")]
		static extern IntPtr gtk_check_version (uint maj, uint min, uint mic);

		// check for gtk 2.4 or newer
		public bool IsGtk24 {
			get {
				string res = Marshal.PtrToStringAuto (gtk_check_version (2, 4, 0));
				if (res == null || res == String.Empty)
					return true;
				else
					return false;
			}
		}
	
	}

}
