// created on 6/24/2004 at 7:43 PM

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class DockBar : VBox
	{
		DockMaster master;
		ArrayList items;
		Tooltips tooltips;
		
		public DockBar (Dock dock)
		{
			items = new ArrayList ();
			tooltips = new Tooltips ();
			Master = dock.Master;
		}
		
		public DockMaster Master {
			get { return master; }
			set { this.Attach (value); }
		}
		
		public void AddItem (DockItem item)
		{
			// check if already there
			if (items.Contains (item)) {
				Console.WriteLine ("WARNING: Item has already been added to the dockbar");
				return;
			}

			items.Add (item);

			// create a button for the item
			DockBarButton button = new DockBarButton (item);
			this.PackStart (button, false, false, 0);
			tooltips.SetTip (button, item.Name, item.Name);
			item.DockBar = this;
			item.DockBarButton = button;
			button.Clicked += OnDockButtonClicked;
			this.ShowAll ();
		}
		
		public void Attach (DockMaster master)
		{
			if (master == null)
				return;

			master.LayoutChanged -= OnLayoutChanged;

			this.master = master;
			master.LayoutChanged += OnLayoutChanged;
		}

		public override void Destroy ()
		{
			if (master != null) {
				master.LayoutChanged -= OnLayoutChanged;
				master = null;
			}

			if (tooltips != null) {
				tooltips = null;
			}

			base.Destroy ();
		}
		
		public void RemoveItem (DockItem item)
		{
			// we can only remove if it is there
			if (items.Contains (item)) {
				items.Remove (item);
				this.Remove (item.DockBarButton);
				// item.DockBarButton = null;
			}
			else {
				Console.WriteLine ("WARNING: {0} has not been added to the dockbar", item.Name);
			}
		}
		
		void UpdateDockItems ()
		{
			if (master == null)
				return;

			foreach (object o in master.DockObjects)
			{
				DockItem item = o as DockItem;
				if (item == null)
					continue;

				// in items but shouldn't be, remove it
				if (items.Contains (item) && !item.Iconified)
					this.RemoveItem (item);
				// not in items but should be, add it
				else if (!items.Contains (item) && item.Iconified)
					this.AddItem (item);
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
