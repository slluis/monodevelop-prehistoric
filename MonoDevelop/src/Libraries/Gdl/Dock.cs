// created on 05/06/2004 at 11:21 A

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class Dock : DockObject
	{
	
		public Dock ()
		{
			this.Flags |= (int)Gtk.WidgetFlags.NoWindow;
			this.DockObjectFlags &= ~(DockObjectFlags.Automatic);
			if (this.Master == null) {
				this.Bind (new DockMaster ());
			}
			if (this.floating) {
				//Need code here to handle floating shit.
			}
			this.DockObjectFlags |= DockObjectFlags.Attached;
		}
		
		public Dock (Dock original, bool _floating) : this ()
		{
			this.Master = original.Master;
			this.floating = _floating;
		}
		
		private DockObject root = null;
		private bool floating;
		private Widget window;
		private bool auto_title;
		private int float_x;
		private int float_y;
		private int width = -1;
		private int height = -1;
		private Gdk.GC xor_gc;

		public DockObject Root {
			get { return root; }
			set { root = value; }
		}
		
		public bool Floating {
			get { return floating; }
			set { floating = value; }
		}
		
		public string DefaultTitle {
			get {
				if (this.Master != null)
					return this.Master.DefaultTitle;
				return null;
			}
			set {
				if (this.Master != null)
					this.Master.DefaultTitle = value;
			}
		}
		
		public int Width {
			get { return width; }
			set {
				width = value;
				if (this.floating && this.window != null && this.window is Gtk.Window)
					((Gtk.Window)this.window).Resize (width, height);
			}
		}
		
		public int Height {
			get { return height; }
			set {
				height = value;
				if (this.floating && this.window != null && this.window is Gtk.Window)
					((Gtk.Window)this.window).Resize (width, height);
			}
		}
		
		public int FloatX {
			get { return float_x; }
			set {
				float_x = value;
				if (this.floating && this.window != null && this.window is Gtk.Window)
					((Gtk.Window)this.window).Resize (width, height);
			}
		}
		
		public int FloatY {
			get { return float_y; }
			set {
				float_y = value;
				if (this.floating && this.window != null && this.window is Gtk.Window)
					((Gtk.Window)this.window).Resize (width, height);
			}
		}
		
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			int border_width = (int)this.BorderWidth;
			if (this.root != null && this.root.Visible)
				requisition = this.root.SizeRequest ();
			else {
				requisition.Width = 0;
				requisition.Height = 0;
			}
			
			requisition.Width += 2 * border_width;
			requisition.Height += 2 * border_width;
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			int border_width = (int)this.BorderWidth;
			allocation.X += border_width;
			allocation.Y += border_width;
			allocation.Width = Math.Max (1, allocation.Width - 2 * border_width);
			allocation.Height = Math.Max (1, allocation.Height - 2 * border_width);
			
			if (this.root != null && this.root.Visible)
				this.root.SizeAllocate (allocation);
		}
		
		protected override void OnMapped ()
		{
			base.OnMapped ();
			Console.WriteLine ("Mapping");
			if (this.root != null) {
				Console.WriteLine ("root.Visible = " + this.root.Visible);
				if (this.root.Visible && !this.root.IsMapped) {
					Console.WriteLine ("Mapping root");
					this.root.Map ();
				}
			}
		}
		
		protected override void OnUnmapped ()
		{
			base.OnUnmapped ();
			if (this.root != null) {
				if (this.root.Visible && this.root.IsMapped)
					this.root.Unmap ();
			}
			if (this.window != null)
				window.Unmap ();
		}
		
		protected override void OnShown ()
		{
			base.OnShown ();
			if (this.floating && this.window != null)
				this.window.Show ();
			if (this.IsController) {
				foreach (DockObject item in this.Master.TopLevelDocks) {
					if (item == this)
						continue;
					if (item.IsAutomatic)
						item.Show ();
				}
			}
		}
		
		protected override void OnHidden ()
		{
			base.OnHidden ();
			if (this.floating && this.window != null)
				this.window.Hide ();
			/*PORT:
			    if (GDL_DOCK_IS_CONTROLLER (dock)) {
        gdl_dock_master_foreach_toplevel (GDL_DOCK_OBJECT_GET_MASTER (dock),
                                          FALSE, (GFunc) gdl_dock_foreach_automatic,
                                          gtk_widget_hide);
    		}*/
		}
		
		protected override void OnAdded (Gtk.Widget widget)
		{
			DockItem child = widget as DockItem;
			if (child == null)
				return;
			
			AddItem (child, DockPlacement.Top);
		}
		
		protected override void OnRemoved (Gtk.Widget widget)
		{
			bool was_visible = widget.Visible;
			if (this.root == widget) {
				this.root = null;
				((DockObject)widget).DockObjectFlags &= ~(DockObjectFlags.Attached);
				widget.Unparent ();
				if (was_visible && this.Visible)
					this.QueueResize ();
			}
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (this.root != null)
				invoker.Invoke (this.root);
		}
		
		/*PORT THIS CODE: its an override of Container.ChildType
		static GtkType
gdl_dock_child_type (GtkContainer *container)
{
    return GDL_TYPE_DOCK_ITEM;
}*/
		
		public override void Detach (bool recursive)
		{
			if (recursive && this.root != null)
				this.root.Detach (recursive);
			this.DockObjectFlags &= ~(DockObjectFlags.Attached);
		}
		
		public override void Reduce ()
		{
			if (this.root != null)
				return;
			
			if (this.IsAutomatic)
				this.Destroy ();
			else if (!(this.IsAttached)) {
				if (this.floating)
					this.Hide ();
				else {
					if (this.Parent != null && this.Parent is Gtk.Container)
						((Gtk.Container)this.Parent).Remove (this);
				}
			}
		}
		
		public override bool DockRequest (int x, int y, DockRequest request)
		{
			Gdk.Rectangle alloc = this.Allocation;
			int bw = (int)this.BorderWidth;
			int rel_x = x - alloc.X;
			int rel_y = y - alloc.Y;
			DockRequest my_request = null;
			bool may_dock = false;
			
			if (request != null)
				my_request = request;
				
			if (rel_x > 0 && rel_x < alloc.Width && rel_y > 0 && rel_y < alloc.Height) {
				may_dock = true;
				Gdk.Rectangle req_rect = new Gdk.Rectangle ();
				req_rect.X = alloc.X + bw;
				req_rect.Y = alloc.Y + bw;
				req_rect.Width = alloc.Width - 2 * bw;
				req_rect.Height = alloc.Height - 2 * bw;
				my_request.Rect = req_rect;
				
				if (this.root == null) {
					my_request.Position = DockPlacement.Top;
					my_request.Target = this;
				} else {
					my_request.Target = this.root;
					
					if (rel_x < bw) {
						my_request.Position = DockPlacement.Left;
						req_rect.Width = (int)(req_rect.Width * 0.3);
						my_request.Rect = req_rect;
					} else if (rel_x > alloc.Width - bw) {
						my_request.Position = DockPlacement.Right;
						req_rect.X += (int)(req_rect.Width * (1 - 0.3));
						req_rect.Width = (int)(req_rect.Width * 0.3);
						my_request.Rect = req_rect;
					} else if (rel_y < bw) {
						my_request.Position = DockPlacement.Top;
						req_rect.Height = (int)(req_rect.Height * 0.3);
						my_request.Rect = req_rect;
					} else if (rel_y > alloc.Height - bw) {
						my_request.Position = DockPlacement.Bottom;
						req_rect.Y += (int)(req_rect.Height * (1 - 0.3));
						req_rect.Height = (int)(req_rect.Height * 0.3);
						my_request.Rect = req_rect;
					} else {
						may_dock = this.root.DockRequest (x, y, my_request);
					}
				}
			}
			
			if (may_dock && request != null)
				request = my_request;
			return may_dock;
		}
		
		public override void Docking (DockObject requestor, DockPlacement position, object user_data)
		{
			if (!(requestor is DockItem))
				return;
			if (position == DockPlacement.Floating) {
				Console.WriteLine ("Adding a floating dockitem");
				DockItem item = requestor as DockItem;
				int x = 0, y = 0, width = -1, height = 01;
				if (user_data != null && user_data is Gdk.Rectangle) {
					Gdk.Rectangle rect = (Gdk.Rectangle)user_data;
					x = rect.X;
					y = rect.Y;
					width = rect.Width;
					height = rect.Height;
				}
				AddFloatingItem (item, x, y, width, height);
			} else if (this.root != null) {
				Console.WriteLine ("root was not null, docking to root");
				this.root.Docking (requestor, position, null);
				//gdl_dock_set_title (dock /*this*/);
			} else {
				Console.WriteLine ("root is null, setting requestor to root");
				this.root = requestor;
				this.root.DockObjectFlags &= DockObjectFlags.Attached;
				this.root.Parent = this;
				((DockItem)this.root).ShowGrip ();
				if (this.IsRealized) {
					Console.WriteLine ("realizing new root");
					this.root.Realize ();
				}
				if (this.Visible && this.root.Visible) {
					Console.WriteLine ("root is visible");
					if (this.IsMapped) {
						Console.WriteLine ("mapping new root");
						this.root.Map ();
					}
					this.root.QueueResize ();
				}
				//gdl_dock_set_title (dock /*this*/);
			}
		}
		
		public override bool Reorder (DockObject requestor, DockPlacement new_position, object other_data)
		{
			bool handled = false;
			if (this.floating && new_position == DockPlacement.Floating && this.root == requestor) {
				if (other_data != null && other_data is Gdk.Rectangle) {
					Gdk.Rectangle rect = (Gdk.Rectangle)other_data;
					if (this.window != null && this.window is Gtk.Window) {
						((Gtk.Window)this.window).Move (rect.X, rect.Y);
						handled = true;
					}
				}
			}
			return handled;
		}
		
		public override bool ChildPlacement (DockObject child, ref DockPlacement placement)
		{
			bool retval = true;
			if (this.root == child) {
				if (placement == DockPlacement.None || placement == DockPlacement.Floating)
					placement = DockPlacement.Top;
			} else
				retval = false;
				
			return retval;
		}
		
		public override void Present (DockObject child)
		{
			if (this.floating && this.window != null && this.window is Gtk.Window)
				((Gtk.Window)this.window).Present ();
		}
		
		public void AddItem (DockItem item, DockPlacement placement)
		{
			if (item == null)
				return;
			if (placement == DockPlacement.Floating)
				AddFloatingItem (item, 0, 0, -1, -1);
			else {
				Console.WriteLine ("about to dock");
				this.Docking (item, placement, null);
			}
		}
		
		public void AddFloatingItem (DockItem item, int x, int y, int width, int height)
		{
			Gdl.Dock new_dock = new Dock (this, true);
			new_dock.Width = width;
			new_dock.Height = height;
			new_dock.FloatX = x;
			new_dock.FloatY = y;
			if (this.Visible) {
				new_dock.Show ();
				if (this.IsMapped)
					new_dock.Map ();
				new_dock.QueueResize ();
			}
			new_dock.AddItem (item, DockPlacement.Top);
		}
		
		public DockItem GetItemByName (string name)
		{
			if (name == null)
				return null;
			DockObject found = this.Master.GetObject (name);
			if (found != null && found is DockItem)
				return (DockItem)found;
			return null;
		}
		
		public DockPlaceholder GetPlaceholderByName (string name)
		{
			if (name == null)
				return null;
			DockObject found = this.Master.GetObject (name);
			if (found != null && found is DockPlaceholder)
				return (DockPlaceholder)found;
			return null;
		}
		
		public ArrayList NamedItems {
			get {
				/*PORT THIS:
				
    gdl_dock_master_foreach (GDL_DOCK_OBJECT_GET_MASTER (dock),
                             (GFunc) _gdl_dock_foreach_build_list, &list);
                             */
                             return null;
			}
		}
		
		public static Dock GetTopLevel (DockObject obj)
		{
			DockObject parent = obj;
			while (parent != null && !(parent is Gdl.Dock))
				parent = parent.ParentObject;
			return (Dock)parent;
		}
		
		public void XorRect (Gdk.Rectangle rect)
		{
			if (this.xor_gc == null) {
				if (this.IsRealized) {
					Gdk.GCValues values = new Gdk.GCValues ();
					values.Function = Gdk.Function.Invert;
					values.SubwindowMode = Gdk.SubwindowMode.IncludeInferiors;
					this.xor_gc = new Gdk.GC (this.GdkWindow);
					this.xor_gc.SetValues (values, Gdk.GCValuesMask.Function | Gdk.GCValuesMask.Subwindow);
				} else
					return;
			}
			xor_gc.SetLineAttributes (1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.NotLast, Gdk.JoinStyle.Bevel);
			xor_gc.SetDashes (1, new sbyte[] { 1, 1}, 2);
			this.GdkWindow.DrawRectangle (xor_gc, false, rect.X, rect.Y, rect.Width, rect.Height);
			xor_gc.SetDashes (0, new sbyte[] { 1, 1}, 2);
			this.GdkWindow.DrawRectangle (xor_gc, false, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
		}
		
		private bool IsController {
			get {
				if (this.Master == null) {
					Console.WriteLine ("Master is null");
					return false;
				}
				if (this.Master.Controller == null)
					Console.WriteLine ("Master.Controller is null");
				return (this.Master.Controller == this); 
			}
		}
	}
}
