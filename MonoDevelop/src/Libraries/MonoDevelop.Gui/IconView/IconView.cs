using System;
using System.Collections;

using Gtk;
using GtkSharp;
using Gnome;
using GnomeSharp;

namespace MonoDevelop.Gui {
	public class IconView : ScrolledWindow {
		IconList iconList;
		
		Hashtable userData = new Hashtable ();
	
		public object CurrentlySelected;
		public event EventHandler IconSelected;
		public event EventHandler IconDoubleClicked;
				
		public IconView ()
		{
			iconList = new IconList (100, null, 0);
			iconList.IconSelected += new IconSelectedHandler (HandleIconSelected);
			
			this.Add (iconList);
			this.WidthRequest = 350;
			this.HeightRequest = 200;
		}

		public void AddIcon (Image icon, string name, object obj)
		{
			int itm = iconList.AppendPixbuf (icon.Pixbuf, "/dev/null", name);
			userData.Add (itm, obj);
		}
		
		void HandleIconSelected (object o, IconSelectedArgs args)
		{
			CurrentlySelected = userData [args.Num];
			
			if (IconSelected != null)
				IconSelected (this, new EventArgs ());

			if (args.Event != null && args.Event.Type == Gdk.EventType.TwoButtonPress)
				if (IconDoubleClicked != null)
					IconDoubleClicked (this, new EventArgs ());
		}

		public void Clear ()
		{
			iconList.Clear ();
			userData.Clear ();
		}
	}
}
