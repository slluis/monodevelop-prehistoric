using System;
using Gtk;
using GtkSharp;
using ICSharpCode.SharpDevelop.Gui.HtmlControl;

class T
{
	static void Main ()
	{
		Application.Init ();
		Window win = new Window ("HtmlControl test");
		win.DeleteEvent += new DeleteEventHandler (OnWinDelete);

		HtmlControl html = new HtmlControl ();
		// this loads html from a string
		html.Html = "<html><body>testing</body></html>";
		// this loads html from a Url
		//html.Url = "http://localhost";
		win.Add (html.Control);
		win.ShowAll ();
		html.DelayedInitialize ();
		Application.Run ();
	}

	static void OnWinDelete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}
}
