using System;
using System.Drawing;
using Gtk;
using Gdk;

namespace MonoDevelop.Gui.Widgets
{
	public class TabLabel : HBox
	{
		private static GLib.GType gtype;
		private Label title;
		private Gtk.Image icon;
		private Button btn;
		
		public TabLabel (Label label, Gtk.Image icon) : base (GType)
		{
			this.icon = icon;
			this.PackStart (icon, false, true, 2);

			title = label;
			this.PackStart (title, false, true, 0);
			
			btn = new Button ();
			btn.Add (new Gtk.Image ("../data/resources/icons/MonoDevelop.Close.png"));
			btn.Relief = ReliefStyle.None;
			btn.RequestSize = new Size (16, 16);
			this.PackStart (btn, true, true, 2);

			this.ShowAll ();
		}
		
		public Label Label
		{
			get { return title; }
			set { title = value; }
		}
		
		public Gtk.Image Icon
		{
			get { return icon; }
			set { icon = value; }
		}

		public Button Button
		{
			get { return btn; }
		}
		
		public static new GLib.GType GType
		{
			get
			{
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (TabLabel));
				return gtype;
			}
		}
	}
}
