// created on 6/24/2004 at 7:43 PM

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class DockBar : VBox
	{
		DockMaster master;
		
		public DockBar (Dock dock)
		{
			master = dock.Master;
		}
		
		public DockMaster Master {
			get {
				return master;
			}
			set {
				master = value;
			}
		}
		
		public void AddItem (DockItem item)
		{
			Button button = new Button ();
			Image image = new Image (item.StockId);
			button.Add (image);
			button.Clicked += OnDockButtonClicked;
			// check if already there
			// set tooltip
			item.DockBar = this;
			item.DockBarButton = button;
			this.PackStart (button, false, false, 0);
			button.ShowAll ();
		}
		
		public void Attach (DockMaster master)
		{
			this.master = master;
			master.LayoutChanged += OnLayoutChanged;
		}
		
		public void RemoveItem (DockItem item)
		{
			// check if there
			this.Remove (item.DockBarButton);
		}
		
		void UpdateDockItems ()
		{
			foreach (DockItem item in master.DockObjects)
			{
				if (item.Iconified)
					this.AddItem (item);
				else
					this.RemoveItem (item);
			}
		}
		
		void OnLayoutChanged (object o, EventArgs args)
		{
			UpdateDockItems ();
		}
		
		void OnDockButtonClicked (object o, EventArgs args)
		{
			// show
			// remove
			// resize
		}
	}
}