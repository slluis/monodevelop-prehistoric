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

	}
}

