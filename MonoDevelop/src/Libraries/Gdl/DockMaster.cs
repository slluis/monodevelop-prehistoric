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
				if (value >= 0)
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
			request.Target.Docking (request.Applicant, request.Position, request.Extra);
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
			bool may_dock = false;
			
			Gdk.Window window = Gdk.Window.AtPointer (out win_x, out win_y);
			if (window != null) {
				IntPtr widg = window.UserData;
				if (widg != IntPtr.Zero) {
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
				foreach (Dock top_dock in toplevel_docks) {
					top_dock.GdkWindow.GetOrigin (out win_x, out win_y);
					x = root_x - win_x;
					y = root_y - win_y;
					may_dock = top_dock.DockRequest (x, y, my_request);
					if (may_dock)
						break;
				}
			}
			if (!may_dock) {
				dock = null;
				Gtk.Requisition req = DockItem.PreferredSize ((DockItem)request.Applicant);
				my_request.Target = Dock.GetTopLevel (request.Applicant);
				my_request.Position = DockPlacement.Floating;
				Gdk.Rectangle rect = new Gdk.Rectangle ();
				rect.Width = req.Width;
				rect.Height = req.Height;
				rect.X = root_x - ((DockItem)request.Applicant).DragOffX;
				rect.Y = root_y - ((DockItem)request.Applicant).DragOffY;
				my_request.Rect = rect;
				my_request.Extra = my_request.Rect;
			}
			
			if (!(my_request.Rect.X == request.Rect.X &&
			      my_request.Rect.Y == request.Rect.Y &&
			      my_request.Rect.Width == request.Rect.Width &&
			      my_request.Rect.Height == request.Rect.Height &&
			      dock == this.rect_owner)) {
				if (this.rect_drawn) {
					XorRect ();
				}
			}
			
			request = my_request;
			this.rect_owner = dock;
			
			if (!this.rect_drawn) {
				XorRect ();
			}
		}
		
		public void XorRect ()
		{
			if (this.drag_request == null)
				return;
			this.rect_drawn = !(this.rect_drawn);
			if (this.rect_owner != null) {
				this.rect_owner.XorRect (this.drag_request.Rect);
				return;
			}
			
			Gdk.Rectangle rect = this.drag_request.Rect;
			Gdk.Window window = Gdk.Global.DefaultRootWindow;
			if (this.root_xor_gc == null) {
				Gdk.GCValues values = new Gdk.GCValues ();
				values.Function = Gdk.Function.Invert;
				values.SubwindowMode = Gdk.SubwindowMode.IncludeInferiors;
				this.root_xor_gc = new Gdk.GC (window);
				this.root_xor_gc.SetValues (values, Gdk.GCValuesMask.Function | Gdk.GCValuesMask.Subwindow);
			}
			this.root_xor_gc.SetLineAttributes (1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.NotLast, Gdk.JoinStyle.Bevel);
			this.root_xor_gc.SetDashes (1, new sbyte[] {1, 1}, 2);
			window.DrawRectangle (this.root_xor_gc, false, rect.X, rect.Y, rect.Width, rect.Height);
			this.root_xor_gc.SetDashes (0, new sbyte[] {1, 1}, 2);
			window.DrawRectangle (this.root_xor_gc, false, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
		}
		
		public void Add (DockObject objekt)
		{
			if (objekt == null)
				return;
			if (!objekt.IsAutomatic) {
				if (objekt.Name == null)
					objekt.Name = "__dock_" + this.number++;
				DockObject found_object = (DockObject)this.dock_objects[objekt.Name];
				if (found_object != null) {
					Console.WriteLine ("Unable to add object, name taken");
				} else {
					this.dock_objects[objekt.Name] = objekt;
				}
			}
			
			if (objekt is Dock) {
				if (this.toplevel_docks == null) {
					this.controller = objekt;
					this.toplevel_docks = new ArrayList ();
				}
				if (((Dock)objekt).Floating) {
					this.toplevel_docks.Insert (0, objekt);
				} else {
					this.toplevel_docks.Add (objekt);
				}
				/* PORT THIS:
				        g_signal_connect (object, "dock",
                          G_CALLBACK (item_dock_cb), master);
				*/
			} else if (objekt is DockItem) {
				/* PORT THIS:
				        g_signal_connect (object, "dock_drag_begin",
                          G_CALLBACK (gdl_dock_master_drag_begin), master);
        g_signal_connect (object, "dock_drag_motion",
                          G_CALLBACK (gdl_dock_master_drag_motion), master);
        g_signal_connect (object, "dock_drag_end",
                          G_CALLBACK (gdl_dock_master_drag_end), master);
        g_signal_connect (object, "dock",
                          G_CALLBACK (item_dock_cb), master);
        g_signal_connect (object, "detach",
                          G_CALLBACK (item_detach_cb), master);
                          
                                  if (GDL_DOCK_ITEM_HAS_GRIP (object)) {
            g_signal_connect (object, "notify::locked",
                              G_CALLBACK (item_notify_cb), master);
            item_notify_cb (object, NULL, master);
        }
        
        if (!GDL_DOCK_OBJECT_AUTOMATIC (object)) {
            if (!master->_priv->idle_layout_changed_id)
                master->_priv->idle_layout_changed_id =
                    g_idle_add (idle_emit_layout_changed, master);
        }
				*/
				
			}
		}
		
		public void Remove (DockObject objekt)
		{
			if (objekt == null)
				return;
			if (objekt is DockItem && ((DockItem)objekt).HasGrip) {
				int locked = this.Locked;
				if (this.locked_items.Contains (objekt)) {
					this.locked_items.Remove (objekt);
					if (this.Locked != locked) {
						//g_object_notify (G_OBJECT (master /*this*/), "locked");
					}
				}
				if (this.unlocked_items.Contains (objekt)) {
					this.locked_items.Remove (objekt);
					if (this.Locked != locked) {
						//g_object_notify (G_OBJECT( master /*this*/), "locked");
					}
				}
			}
			
			if (objekt is Dock) {
				if (this.toplevel_docks.Contains (objekt))
					this.toplevel_docks.Remove (objekt);
				if (objekt == this.controller) {
					DockObject new_controller = null;
					ArrayList reversed = toplevel_docks;
					reversed.Reverse ();
					foreach (DockObject item in reversed) {
						if (!item.IsAutomatic) {
							new_controller = item;
							break;
						}
					}
					if (new_controller != null) {
						this.controller = new_controller;
					} else {
						this.controller = null;
					}
				}
			}
			
			/*PORT THIS:
    g_signal_handlers_disconnect_matched (object, G_SIGNAL_MATCH_DATA, 
                                          0, 0, NULL, NULL, master);*/
			if (objekt.Name != null) {
				if (this.dock_objects.Contains (objekt.Name)) {
					this.dock_objects.Remove (objekt.Name);
				}
			}
			
			if (!objekt.IsAutomatic) {
				if (this.idle_layout_changed_id == 0) {
					this.idle_layout_changed_id = 0; //g_idle_add (idle_emit_layout_changed);
				}
			}
		}
		
		public DockObject GetObject (string name)
		{
			if (name == null)
				return null;
			return (DockObject)this.dock_objects[name];
		}
		
		public DockObject Controller {
			get { return this.controller; }
			set {
				if (value != null) {
					if (value.IsAutomatic)
						Console.WriteLine ("New controller is automatic, only manual dock objects should be named controller");
					if (!this.toplevel_docks.Contains (value))
						this.Add (value);
					this.controller = value;
				} else {
					this.controller = null;
				}
			}
		}
	}
}