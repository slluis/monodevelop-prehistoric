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
			Console.WriteLine ("new dockbar");
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
			Console.WriteLine ("adding item to dockbar");
			Button button = new Button ();
			button.Relief = ReliefStyle.None;
			Image image = new Image (item.StockId, IconSize.SmallToolbar);
			button.Add (image);
			button.Clicked += OnDockButtonClicked;
			// check if already there
			tooltips.SetTip (button, item.Name, item.Name);
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
			DockItem item = (DockItem) o;  //FIXME: o is a Button
			DockObject controller = item.Master.Controller;
			item.DockBar = null;
			// remove Iconified flag
			item.Flags |= (int) DockObjectFlags.Iconified;
			item.Show ();
			this.RemoveItem (item);
			controller.QueueResize ();
		}
	}
}