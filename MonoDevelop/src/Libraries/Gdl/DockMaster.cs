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
		private ArrayList toplevelDocks = new ArrayList ();
		private DockObject controller = null;
		private int dockNumber = 1;
		private int number = 1;
		private string defaultTitle;
		private Gdk.GC rootXorGC;
		private bool rectDrawn;
		private Dock rectOwner;
		private DockRequest request;
		private uint idle_layout_changed_id;
		private Hashtable lockedItems = new Hashtable ();
		private Hashtable unlockedItems = new Hashtable ();

		public DockMaster () 
		{
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
		
		public void Add (DockObject obj)
		{
			if (obj == null)
				return;

			if (!obj.IsAutomatic) {
				/* create a name for the object if it doesn't have one */
				if (obj.Name == null)
					obj.Name = "__dock_" + number++;

				/* add the object to our hash list */
				if (dockObjects.Contains (obj.Name))
					Console.WriteLine ("Unable to add object, name \"{0}\" taken", obj.Name);
				else
					dockObjects.Add (obj.Name, obj);
			}
			
			if (obj is Dock) {
				/* if this is the first toplevel we are adding, name it controller */
				if (toplevelDocks.Count == 0)
					controller = obj;
				
				/* add dock to the toplevel list */
				if (((Dock)obj).Floating)
					toplevelDocks.Insert (0, obj);
				else
					toplevelDocks.Add (obj);
				
				/* we are interested in the dock request this toplevel
				 * receives to update the layout */
				obj.Docked += OnItemDocked;
			} else if (obj is DockItem) {
				DockItem item = obj as DockItem;
				
				/* we need to connect the item's events */
				item.Detached += OnItemDetached;
				item.Docked += OnItemDocked;
				item.DockItemDragBegin += OnDragBegin;
				item.DockItemMotion += OnDragMotion;
				item.DockItemDragEnd += OnDragEnd;

				/* register to "locked" notification if the item has a grip,
				 * and add the item to the corresponding hash */
				item.PropertyChanged += OnItemPropertyChanged;

				/* post a layout_changed emission if the item is not automatic
				 * (since it should be added to the items model) */
				if (!item.IsAutomatic) {
					// FIXME: Emit a LayoutChanged event?
				}
			}
		}
		
		public void Remove (DockObject obj)
		{
#if false
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
#endif
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
		
		private void OnItemDetached (object o, DetachedArgs args)
		{
		}
		
		private void OnItemDocked (object o, DockedArgs args)
		{
		}
		
		private void OnItemPropertyChanged (object o, string name)
		{
		}
		
		private void OnDragBegin (DockItem item)
		{
			Console.WriteLine ("DockMaster.OnDragBegin");
		
			/* Set the target to itself so it won't go floating with just a click. */
			request = new DockRequest ();
			request.Applicant = item;
			request.Target = item;
			request.Position = DockPlacement.Floating;

			rectDrawn = false;
			rectOwner = null;
		}
		
		private void OnDragEnd (DockItem item, bool cancelled)
		{
			Console.WriteLine ("DockMaster.OnDragEnd");
		
			if (item != request.Applicant)  {
				Console.WriteLine ("Dragged item is not the same as the one we started with");
				return;
			}
			
			/* Erase previously drawn rectangle */
			if (rectDrawn)
				XorRect ();
			
			/* cancel conditions */
			if (cancelled || request.Applicant == request.Target)
				return;

			request.Target.Dock (request.Applicant,
					     request.Position,
					     null);
			
			EmitLayoutChangedEvent ();
		}
		
		private void OnDragMotion (DockItem item, int rootX, int rootY)
		{
			Console.WriteLine ("DockMaster.OnDragMotion");
		
			Dock dock = null;
			int winX, winY;
			int x, y;
			bool mayDock = false;
			DockRequest myRequest = request;

			if (item != request.Applicant)  {
				Console.WriteLine ("Dragged item is not the same as the one we started with");
				return;
			}
			
			/* first look under the pointer */
			Gdk.Window window = Gdk.Window.AtPointer (out winX, out winY);
			if (window != null) {
				if (window.UserData == IntPtr.Zero) {
					Console.WriteLine ("The Gdk.Window should contain the Widget owner");
					return;
				}
				
				/* ok, now get the widget who owns that window and see if we can
				   get to a Dock by walking up the hierarchy */
				Widget widget = GLib.Object.GetObject (window.UserData, false) as Widget;
				while (widget != null && (!(widget is Dock) ||
				       (widget is DockObject && ((DockObject)widget).Master != this)))
					widget = widget.Parent;

				if (widget != null) {
					int winW, winH, depth;
					
					/* verify that the pointer is still in that dock
					   (the user could have moved it) */
					widget.GdkWindow.GetGeometry (out winX, out winY,
								      out winW, out winH,
								      out depth);
					widget.GdkWindow.GetOrigin (out winX, out winY);
					if (rootX >= winX && rootX < winX + winW &&
					    rootY >= winY && rootY < winY + winH)
						dock = widget as Dock;
				}
			}
			
			if (dock != null) {
				/* translate root coordinates into dock object coordinates
				   (i.e. widget coordinates) */
				dock.GdkWindow.GetOrigin (out winX, out winY);
				x = rootX - winX;
				y = rootY - winY;
				mayDock = dock.OnDockRequest (x, y, ref myRequest);
			} else {
				/* try to dock the item in all the docks in the ring in turn */
				foreach (Dock topDock in toplevelDocks) {
					/* translate root coordinates into dock object
					   coordinates (i.e. widget coordinates) */
					topDock.GdkWindow.GetOrigin (out winX, out winY);
					x = rootX - winX;
					y = rootY - winY;
					mayDock = topDock.OnDockRequest (x, y, ref myRequest);
					if (mayDock)
						break;
				}
			}

			if (!mayDock) {
				dock = null;
				
				myRequest.Target = Dock.GetTopLevel (item);
				myRequest.Position = DockPlacement.Floating;
				
				Gdk.Rectangle rect = new Gdk.Rectangle ();
				rect.Width = item.PreferredWidth;
				rect.Height = item.PreferredHeight;
				rect.X = rootX - item.DragOffX;
				rect.Y = rootY - item.DragOffY;
				myRequest.Rect = rect;
				myRequest.Extra = myRequest.Rect;
			}
			
			if (!(myRequest.Rect.X == request.Rect.X &&
			      myRequest.Rect.Y == request.Rect.Y &&
			      myRequest.Rect.Width == request.Rect.Width &&
			      myRequest.Rect.Height == request.Rect.Height &&
			      dock == rectOwner)) {
			      
				/* erase the previous rectangle */
				if (rectDrawn)
					XorRect ();
			}
			
			request = myRequest;
			rectOwner = dock;
			
			/* draw the previous rectangle */
			if (!rectDrawn)
				XorRect ();
		}

		private void XorRect ()
		{
			Console.WriteLine ("DockMaster.XorRect");
		
			rectDrawn = !rectDrawn;

			if (rectOwner != null) {
				rectOwner.XorRect (request.Rect);
				return;
			}
			
			Gdk.Rectangle rect = request.Rect;
			Gdk.Window window = Gdk.Global.DefaultRootWindow;
			
			if (rootXorGC == null) {
				Gdk.GCValues values = new Gdk.GCValues ();
				values.Function = Gdk.Function.Invert;
				values.SubwindowMode = Gdk.SubwindowMode.IncludeInferiors;

				rootXorGC = new Gdk.GC (window);
				rootXorGC.SetValues (values, Gdk.GCValuesMask.Function |
						     Gdk.GCValuesMask.Subwindow);
			}
			
			rootXorGC.SetLineAttributes (1, Gdk.LineStyle.OnOffDash,
						     Gdk.CapStyle.NotLast,
						     Gdk.JoinStyle.Bevel);

			rootXorGC.SetDashes (1, new sbyte[] {1, 1}, 2);
			
			window.DrawRectangle (rootXorGC, false, rect.X, rect.Y,
					      rect.Width, rect.Height);
			
			rootXorGC.SetDashes (0, new sbyte[] {1, 1}, 2);

			window.DrawRectangle (rootXorGC, false, rect.X + 1, rect.Y + 1,
					      rect.Width - 2, rect.Height - 2);
		}
	}
}
