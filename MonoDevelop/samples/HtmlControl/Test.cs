using System;
using Gtk;
using GtkSharp;
using ICSharpCode.SharpDevelop.Gui.HtmlControl;

class HtmlTest
{
	Window win;
	HtmlControl html;
	Button go;
	Entry url;
	
	static void Main ()
	{
		Application.Init ();
		new HtmlTest ();
		Application.Run ();
	}

	HtmlTest ()
	{
		win = new Window ("HtmlControl test");
		win.SetDefaultSize (600, 450);
		win.DeleteEvent += new DeleteEventHandler (OnWinDelete);

		VBox vbox = new VBox (false, 0);

		Toolbar tbar = new Toolbar ();
		tbar.ToolbarStyle = ToolbarStyle.Icons;
		
		Image back = new Image (Stock.GoBack, IconSize.SmallToolbar);
		//tbar.AppendItem ("Back", "Go Back", "", back, OnBackClicked);
		Button forward = new Button (Stock.GoForward);
		tbar.AppendWidget (forward, "Go Forward", "");
		Button stop = new Button (Stock.Stop);
		tbar.AppendWidget (stop, "Stop", "");
		Button refresh = new Button (Stock.Refresh);
		tbar.AppendWidget (refresh, "Refresh", "");
		
		vbox.PackStart (tbar, false, true, 0);

		HBox hbox = new HBox (false, 0);

		url = new Entry ();
		url.Activated += new EventHandler (OnUrlActivated);
		hbox.PackStart (url, true, true, 0);		

		go = new Button (Stock.Ok);
		go.Clicked += new EventHandler (OnGoClicked);
		hbox.PackStart (go, false, false, 0);
		vbox.PackStart (hbox, false, false, 0);

		html = new HtmlControl ();
		//html.Title += new EventHandler (OnHtmlTitle);
		// this loads html from a string
		html.Html = "<html><body>testing</body></html>";
		
		// this loads html from a Url
		// html.Url = "http://localhost";
		
		// set the stylesheet
		html.CascadingStyleSheet = "";
		
		html.Show ();
		vbox.PackStart (html, true, true, 0);

		Statusbar status = new Statusbar ();
		vbox.PackStart (status, false, true, 0);
		
		win.Add (vbox);
		win.ShowAll ();
		html.DelayedInitialize ();
	}

	void OnWinDelete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}

	void OnGoClicked (object o, EventArgs args)
	{
		html.Url = url.Text;
	}

	void OnUrlActivated (object o, EventArgs args)
	{
		OnGoClicked (o, args);
	}

	void OnHtmlTitle (object o, EventArgs args)
	{
		//win.Title = "";
	}

	void OnBackClicked (object o, EventArgs args)
	{
		html.GoBack ();
	}
}
