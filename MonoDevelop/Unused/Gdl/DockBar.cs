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
		
		public DockBar ()
		{
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
			DockBarButton button = new DockBarButton (item);
			button.DockButtonClicked += OnDockButtonClicked;
			// check if already there
			foreach (DockBarButton dbb in this.Children) {
				if (item == dbb.DockItem) {
					return;
				}
			}
			tooltips.SetTip (button, item.Name, item.Name);
			item.DockBar = this;
			item.DockBarButton = button;
			this.PackStart (button, false, false, 0);
			this.ShowAll ();
		}
		
		public void Attach (DockMaster master)
		{
			if (this.master != null)
				master.LayoutChanged -= OnLayoutChanged;

			this.master = master;
			master.LayoutChanged += OnLayoutChanged;
		}
		
		public void RemoveItem (DockItem item)
		{
			// we can only remove if it is there
			foreach (DockBarButton dbb in this.Children) {
				if (dbb == item.DockBarButton) {
					this.Remove (item.DockBarButton);
					return;
				}
			}
		}
		
		void UpdateDockItems ()
		{
			foreach (object o in master.DockObjects)
			{
				DockItem item = o as DockItem;
				if (item == null)
					continue;

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
