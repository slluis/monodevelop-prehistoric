using System;
using System.Collections;
using MonoDevelop.Gui;
using Gtk;
using GtkSharp;

namespace MonoDevelop.Gui
{
	class ItemGroup : VBox
	{
		public object obj;
		public string name;
		public Image icon;
		public event EventHandler IconClicked;
		EventBox text_eventbox, icon_eventbox;
		Label label;
                                                 
		public ItemGroup (string name, object obj, Image icon) : base (false, 0)
		{
			this.icon = icon;
			icon_eventbox = new EventBox();
			icon_eventbox.Add(this.icon);
			this.PackStart(icon_eventbox, false, true, 0);
			this.name = name;
			text_eventbox = new EventBox();
			label = new Label(this.name);
			label.Wrap = true;
			label.WidthRequest = 100;
			label.Justify = Gtk.Justification.Center;
			label.Xpad = 2;
			label.Ypad = 2;
			text_eventbox.Add(label);
			this.PackStart(text_eventbox, false, true, 0);
			this.obj = obj;
			this.icon_eventbox.ButtonPressEvent += new GtkSharp.ButtonPressEventHandler(icon_clicked);
			this.text_eventbox.ButtonPressEvent += new GtkSharp.ButtonPressEventHandler(icon_clicked);
			this.ShowAll();
		}
				
		public void Selected ()
		{
			text_eventbox.ModifyBg (Gtk.StateType.Normal,Style.Backgrounds[3]);
			label.ModifyFg (Gtk.StateType.Normal,Style.Foregrounds[3]);
		}
		
		public void UnSelected ()
		{
			text_eventbox.ModifyBg (Gtk.StateType.Normal,Style.Backgrounds[0]);
			label.ModifyFg (Gtk.StateType.Normal,Style.Foregrounds[0]);
		}

		void icon_clicked (object o, ButtonPressEventArgs args)
		{
			this.Selected ();
			if (IconClicked != null)
			{
				IconClicked (this, new EventArgs ());
			}
		}
	}
		
	public class IconView : ScrolledWindow
	{
		ArrayList iList;
		Table MainView;
		public object CurrentlySelected;
		public event EventHandler IconSelected;
				
		public IconView ()
		{
			iList = new ArrayList ();
			MainView = new Table (0, 0, false);
			MainView.ModifyFg (Gtk.StateType.Normal, new Gdk.Color (255, 255, 255));
			EventBox box1 = new EventBox ();
			box1.Add (MainView);
			this.AddWithViewport (box1);
			this.WidthRequest = 350;
			this.HeightRequest = 200;
		}

		public void AddIcon (Image icon, string name, object obj)
		{
			ItemGroup tmp = new ItemGroup (name, obj, icon);
			iList.Add (tmp);
			tmp.IconClicked += new EventHandler (IconClicked);
		}
		
		void IconClicked (object o, EventArgs e)
		{
			foreach (ItemGroup group in iList)
			{
				if (group != o)
				{
					group.UnSelected ();
				}
				else
				{
					CurrentlySelected = group.obj;
				}
			}
			
			if (IconSelected != null)
			{
				IconSelected (this, new EventArgs ());
			}
		}

		public void Clear ()
		{
			foreach (ItemGroup group in iList)
			{
				MainView.Remove (group);
			}

			iList.Clear ();
		}

		public void PopulateTable ()
		{
			uint x = 0;
			uint y = 0;

			foreach (ItemGroup group in iList)
			{
				if (x >= 3) {
					x = 0;
					y++;
				}

				MainView.Attach (group, x, x + 1, y, y + 1, Gtk.AttachOptions.Fill, Gtk.AttachOptions.Fill, 5, 5);
                x++;
			}

			this.ShowAll();
		}
	}
}
