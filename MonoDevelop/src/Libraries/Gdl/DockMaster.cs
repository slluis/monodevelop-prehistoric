// created on 05/06/2004 at 11:14 A
using System;
using System.Collections;

using Gtk;

namespace Gdl
{
	public class DockMaster
	{
		private object obj;
		private Hashtable dock_objects = new Hashtable ();
		private ArrayList toplevel_docks = null;
		private DockObject controller = null;
		private int dock_number = 1;
		private int number = 1;
		private string default_title;
		private Gdk.GC root_xor_gc;
		private bool rect_drawn;
		private Dock rect_owner;
		private DockRequest drag_request;
		private uint idle_layout_changed_id;
		private Hashtable locked_items = new Hashtable ();
		private Hashtable unlocked_items = new Hashtable ();
		
		public string DefaultTitle {
			get { return default_title; }
			set { default_title = value; }
		}
		
		public int Locked {
			get {
				if (unlocked_items.Count == 0)
					return 1;
				if (locked_items.Count == 0)
					return 0;
				return -1;
			}
			set {
				if (int >= 0)
					this.LockUnlock (value > 0);
			}
		}
		
		protected void foreach_lock_unlock (DockItem item, bool locked)
		{
			item.Locked = locked;
			if (item.IsCompound) {
				/*PORT THIS: Container.Foreach doesnt take the arg i need it to take.
				        gtk_container_foreach (GTK_CONTAINER (item),
                               (GtkCallback) foreach_lock_unlock,
                               (gpointer) locked);*/
			}
		}
		
		public void LockUnlock (bool locked)
		{
			foreach (Gdl.Dock dock in toplevel_docks) {
				if (dock.Root != null && dock.Root is DockItem)
					foreach_lock_unlock ((DockItem)dock.Root, locked);
			}
			/*PORT THIS:
			    // just to be sure hidden items are set too
    gdl_dock_master_foreach (master,
                             (GFunc) foreach_lock_unlock,
                             (gpointer) locked);*/
		}
		
		
		public void DragBegin (DockItem item)
		{
			if (item == null)
				return;
			if (this.drag_request == null)
				this.drag_request = new DockRequest ();
			DockRequest request = this.drag_request;
			request.Applicant = item;
			request.Target = item;
			request.Position = DockPlacement.Floating;
			request.Extra = null;
			this.rect_drawn = false;
			this.rect_owner = null;
		}
		
		public void DragEnd (DockItem item, bool cancelled)
		{
			if (item == null)
				return;
			DockRequest request = this.drag_request;
			if (item != request.Applicant)
				return;
			if (this.rect_drawn)
				XorRect ();
			if (cancelled || request.Applicant == request.Target)
				return;
			request.Target.Dock (request.Applicant, request.Position, request.Extra);
			//emit LayoutChanged here
		}
		
		public void DragMotion (DockItem item, int root_x, int root_y)
		{
			if (item == null)
				return;
			DockRequest request = this.drag_request;
			if (request.Applicant == item)
				return;
			DockRequest my_request = new DockRequest (request);
			int win_x, win_y;
			int x, y;
			Dock dock = null;
			bool may_dock;
			
			Gdk.Window window = Gdk.Window.AtPointer (out win_x, out win_y);
			if (window != null) {
				IntPtr widg = window.GetUserData ();
				if (widg != null) {
					Gtk.Widget widget = GLib.Object.GetObject (widg, false) as Gtk.Widget;
					if (widget != null) {
						while (widget != null && (!(widget is Dock) || (widget is DockObject && ((DockObject)widget).Master == this)))
							widget = widget.Parent;
						if (widget != null) {
							int win_w, win_h, winx, winy, depth;
							widget.GdkWindow.GetGeometry (out winx, out winy, out win_w, out win_h, out depth);
							widget.GdkWindow.GetOrigin (out win_x, out win_y);
							if (root_x >= win_x && root_x < win_x + win_w && root_y >= win_y && root_y < win_y + win_h)
								dock = widget as Dock;
						}
					}
				}
			}
			
			if (dock != null) {
				dock.GdkWindow.GetOrigin (out win_x, out win_y);
				x = root_x - win_x;
				y = root_y - win_y;
				may_dock = dock.DockRequest (x, y, my_request);
			} else {
				foreach (Dock top_dock in toplevel_docks)
		}
	}
}