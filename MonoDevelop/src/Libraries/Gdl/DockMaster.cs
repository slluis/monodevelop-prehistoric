// created on 05/06/2004 at 11:14 A
using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class DockMaster
	{
		private object obj;
		private Hashtable dockObjects = new Hashtable ();
		private ArrayList toplevelDocks = null;
		private DockObject controller = null;
		private int dockNumber = 1;
		private int number = 1;
		private string defaultTitle;
		private Gdk.GC root_xor_gc;
		private bool rectDrawn;
		private Dock rectOwner;
		private DockRequest dragRequest;
		private uint idle_layout_changed_id;
		private Hashtable lockedItems = new Hashtable ();
		private Hashtable unlockedItems = new Hashtable ();

		public DockMaster () 
		{
			Console.WriteLine ("Creating a new DockMaster");
		}
		
		public string DefaultTitle {
			get {
				return defaultTitle;
			}
			set {
				defaultTitle = value;
			}
		}
		
		public int DockNumber {
			get {
				return dockNumber;
			}
			set {
				dockNumber = value;
			}
		}
		
		public ICollection DockObjects {
			get {
				return dockObjects.Values;
			}
		}
		
		public int Locked {
			get {
				if (unlockedItems.Count == 0)
					return 1;
				if (lockedItems.Count == 0)
					return 0;
				return -1;
			}
			set {
				if (value >= 0)
					LockUnlock (value > 0);
			}
		}
		
		public ArrayList TopLevelDocks {
			get {
				return toplevelDocks;
			}
		}
		
		protected void ForeachLockUnlock (DockItem item, bool locked)
		{
			item.Locked = locked;
			if (item.IsCompound) {
				/*PORT THIS: Container.Foreach doesnt take the arg i need it to take.
				        gtk_container_foreach (GTK_CONTAINER (item),
                               (GtkCallback) ForeachLockUnlock,
                               (gpointer) locked);*/
			}
		}
		
		public void LockUnlock (bool locked)
		{
			foreach (Dock dock in toplevelDocks) {
				if (dock.Root != null && dock.Root is DockItem)
					ForeachLockUnlock ((DockItem)dock.Root, locked);
			}
			/*PORT THIS:
			    // just to be sure hidden items are set too
    gdl_dock_master_foreach (master,
                             (GFunc) ForeachLockUnlock,
                             (gpointer) locked);*/
		}
		
		
		public void DragBegin (DockItem item)
		{
			if (item == null)
				return;

			if (dragRequest == null)
				dragRequest = new DockRequest ();

			DockRequest request = dragRequest;
			request.Applicant = item;
			request.Target = item;
			request.Position = DockPlacement.Floating;
			request.Extra = null;
			rectDrawn = false;
			rectOwner = null;
		}
		
		public void DragEnd (DockItem item, bool cancelled)
		{
			if (item == null)
				return;

			DockRequest request = dragRequest;
			if (item != request.Applicant)
				return;
			if (rectDrawn)
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
			DockRequest request = dragRequest;
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
				may_dock = dock.OnDockRequest (x, y, my_request);
			} else {
				foreach (Dock top_dock in toplevelDocks) {
					top_dock.GdkWindow.GetOrigin (out win_x, out win_y);
					x = root_x - win_x;
					y = root_y - win_y;
					may_dock = top_dock.OnDockRequest (x, y, my_request);
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
			      dock == rectOwner)) {
				if (rectDrawn) {
					XorRect ();
				}
			}
			
			request = my_request;
			rectOwner = dock;
			
			if (!rectDrawn) {
				XorRect ();
			}
		}
		
		public void XorRect ()
		{
			if (dragRequest == null)
				return;
			rectDrawn = !(rectDrawn);
			if (rectOwner != null) {
				rectOwner.XorRect (dragRequest.Rect);
				return;
			}
			
			Gdk.Rectangle rect = dragRequest.Rect;
			Gdk.Window window = Gdk.Global.DefaultRootWindow;
			if (root_xor_gc == null) {
				Gdk.GCValues values = new Gdk.GCValues ();
				values.Function = Gdk.Function.Invert;
				values.SubwindowMode = Gdk.SubwindowMode.IncludeInferiors;
				root_xor_gc = new Gdk.GC (window);
				root_xor_gc.SetValues (values, Gdk.GCValuesMask.Function | Gdk.GCValuesMask.Subwindow);
			}
			root_xor_gc.SetLineAttributes (1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.NotLast, Gdk.JoinStyle.Bevel);
			root_xor_gc.SetDashes (1, new sbyte[] {1, 1}, 2);
			window.DrawRectangle (root_xor_gc, false, rect.X, rect.Y, rect.Width, rect.Height);
			root_xor_gc.SetDashes (0, new sbyte[] {1, 1}, 2);
			window.DrawRectangle (root_xor_gc, false, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
		}
		
		public void Add (DockObject obj)
		{
			if (obj == null)
				return;

			if (!obj.IsAutomatic) {
				if (obj.Name == null)
					obj.Name = "__dock_" + number++;

				DockObject foundObject = (DockObject)dockObjects[obj.Name];
				if (foundObject != null)
					Console.WriteLine ("Unable to add object, name taken");
				else
					dockObjects[obj.Name] = obj;
			}
			
			if (obj is Dock) {
				if (toplevelDocks == null) {
					controller = obj;
					toplevelDocks = new ArrayList ();
				}
				
				if (((Dock)obj).Floating)
					toplevelDocks.Insert (0, obj);
				else
					toplevelDocks.Add (obj);
				
				/* PORT THIS:
				        g_signal_connect (object, "dock",
                          				  G_CALLBACK (item_dock_cb), master);
				*/
			} else if (obj is DockItem) {
				DockItem dock_item = obj as DockItem;
				dock_item.DockItemDragBegin += new DockItemDragBeginHandler (DragBegin);
				dock_item.DockItemMotion += new DockItemMotionHandler (DragMotion);
				dock_item.DockItemDragEnd += new DockItemDragEndHandler (DragEnd);
				/* PORT THIS:
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
		
		public void Remove (DockObject obj)
		{
			if (obj == null)
				return;
			if (obj is DockItem && ((DockItem)obj).HasGrip) {
				int locked = Locked;
				if (lockedItems.Contains (obj)) {
					lockedItems.Remove (obj);
					if (Locked != locked) {
						//g_object_notify (G_OBJECT (master /*this*/), "locked");
					}
				}
				if (unlockedItems.Contains (obj)) {
					lockedItems.Remove (obj);
					if (Locked != locked) {
						//g_object_notify (G_OBJECT( master /*this*/), "locked");
					}
				}
			}
			
			if (obj is Dock) {
				if (toplevelDocks.Contains (obj))
					toplevelDocks.Remove (obj);
				if (obj == controller) {
					DockObject new_controller = null;
					ArrayList reversed = toplevelDocks;
					reversed.Reverse ();
					foreach (DockObject item in reversed) {
						if (!item.IsAutomatic) {
							new_controller = item;
							break;
						}
					}
					if (new_controller != null) {
						controller = new_controller;
					} else {
						controller = null;
					}
				}
			}
			
			if (obj is DockItem) {
				DockItem dock_item = obj as DockItem;
				dock_item.DockItemDragBegin -= DragBegin;
				dock_item.DockItemDragEnd -= DragEnd;
				dock_item.DockItemMotion -= DragMotion;
			}
			
			/*PORT THIS:
    g_signal_handlers_disconnect_matched (object, G_SIGNAL_MATCH_DATA, 
                                          0, 0, NULL, NULL, master);*/
			if (obj.Name != null) {
				if (dockObjects.Contains (obj.Name)) {
					dockObjects.Remove (obj.Name);
				}
			}
			
			if (!obj.IsAutomatic) {
				if (idle_layout_changed_id == 0) {
					idle_layout_changed_id = 0; //g_idle_add (idle_emit_layout_changed);
				}
			}
		}
		
		public DockObject GetObject (string name)
		{
			if (name == null)
				return null;
			return (DockObject)dockObjects[name];
		}
		
		public DockObject Controller {
			get { return controller; }
			set {
				if (value != null) {
					if (value.IsAutomatic)
						Console.WriteLine ("New controller is automatic, only manual dock objects should be named controller");
					if (!toplevelDocks.Contains (value))
						Add (value);
					controller = value;
				} else {
					controller = null;
				}
			}
		}
		
		internal void EmitLayoutChangedEvent ()
		{
			// FIXME: emit the LayoutChanged event here.
		}
	}
}
