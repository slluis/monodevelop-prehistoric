using System;
using Gtk;

namespace MonoDevelop.Gui.Widgets
{

	public class Docker : DrawingArea {

		public Docker () : base ()
		{
			this.ExposeEvent += new GtkSharp.ExposeEventHandler (OnExpose);
		}

		void OnExpose (object o, GtkSharp.ExposeEventArgs e)
		{

		}
	}

}
