using System;
using Gtk;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Widgets
{
	public class Navbar : Toolbar
	{
		static GLib.GType gtype;
		Button back = new Button ();
		Button forward = new Button ();
		Button stop = new Button ();
		Button reload = new Button ();
		Button go = new Button ();
		Entry address = new Entry ();

		public static new GLib.GType GType
		{
			get {
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (Navbar));
				return gtype;
			}
		}

		public Navbar () : base (GType)
		{
			back.Relief = ReliefStyle.None;
			back.Add (new Image (Stock.GoBack, IconSize.SmallToolbar));
			back.Clicked += OnBackClicked;

			forward.Relief = ReliefStyle.None;
			forward.Add (new Image (Stock.GoForward, IconSize.SmallToolbar));
			forward.Clicked += OnForwardClicked;

			stop.Relief = ReliefStyle.None;
			stop.Add (new Image (Stock.Stop, IconSize.SmallToolbar));
			stop.Clicked += OnStopClicked;

			reload.Relief = ReliefStyle.None;
			reload.Add (new Image (Stock.Refresh, IconSize.SmallToolbar));
			reload.Clicked += OnReloadClicked;

			go.Relief = ReliefStyle.None;
			go.Add (new Image (Stock.Ok, IconSize.SmallToolbar));
			go.Clicked += OnGoUrl;

			address.WidthChars = 50;
			address.Activated += OnGoUrl;

			this.AppendWidget (back, GettextCatalog.GetString ("Go back"), "");
			this.AppendWidget (forward, GettextCatalog.GetString ("Go forward"), "");
			this.AppendWidget (stop, GettextCatalog.GetString ("Stop loading"), "");
			this.AppendWidget (reload, GettextCatalog.GetString ("Reload page"), "");
			this.AppendWidget (address, GettextCatalog.GetString ("Address"), "");
			this.AppendWidget (go, GettextCatalog.GetString ("Load address"), "");
		}

		public string Url {
			get {
				return address.Text;
			}
			set {
				address.Text = value;
			}
		}

		void OnGoUrl (object o, EventArgs args)
		{
			if (Go != null)
				Go (this, EventArgs.Empty);
		}

		void OnBackClicked (object o, EventArgs args)
		{
			if (Back != null)
				Back (this, EventArgs.Empty);
		}

		void OnForwardClicked (object o, EventArgs args)
		{
			if (Forward != null)
				Forward (this, EventArgs.Empty);
		}

		void OnStopClicked (object o, EventArgs args)
		{
			if (Stop != null)
				Stop (this, EventArgs.Empty);
		}

		void OnReloadClicked (object o, EventArgs args)
		{
			if (Reload != null)
				Reload (this, EventArgs.Empty);
		}

		public event EventHandler Back;
		public event EventHandler Forward;
		public event EventHandler Stop;
		public event EventHandler Reload;
		public event EventHandler Go;
	}
}

