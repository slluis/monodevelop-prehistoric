using System;
using Gtk;

namespace MonoDevelop.Gui.Widgets
{

	public class Docker : DrawingArea {

		public Docker () : base ()
		{
			this.ExposeEvent += new ExposeEventHandler (OnExpose);
		}

		void OnExpose (object o, ExposeEventArgs e)
		{

		}
	}

}
