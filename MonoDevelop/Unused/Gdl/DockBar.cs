// created on 6/24/2004 at 7:43 PM

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class DockBar : VBox
	{
		DockMaster master;
		Tooltips tooltips = new Tooltips ();
		
		public DockBar (Dock dock)
		{
			this.Attach (dock.Master);
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
			Console.WriteLine ("adding item to dockbar");
			DockBarButton button = new DockBarButton (item);
			button.DockButtonClicked += OnDockButtonClicked;
			// check if already there
			tooltips.SetTip (button, item.Name, item.Name);
			item.DockBar = this;
			item.DockBarButton = button;
			this.PackStart (button, false, false, 0);
			button.ShowAll ();
		}
		
		public void Attach (DockMaster master)
		{
			if (this.master != null)
				master.LayoutChanged -= OnLayoutChanged;

			this.master = master;
			master.DockBar = this;
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
			DockItem item = ((DockBarButton) o).DockItem;
			item.DockBar = null;
			item.ShowItem ();
			this.RemoveItem (item);
			item.Master.Controller.QueueResize ();
		}
	}
}
