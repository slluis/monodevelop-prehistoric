// created on 05/06/2004 at 11:21 A

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class Dock : DockObject
	{
		private DockObject root = null;
		private bool floating;
		private Widget window;
		private bool auto_title;
		private int float_x;
		private int float_y;
		private int width = -1;
		private int height = -1;
		private Gdk.GC xor_gc;

		public Dock ()
		{
			Flags |= (int)WidgetFlags.NoWindow;
			DockObjectFlags &= ~(DockObjectFlags.Automatic);
			if (Master == null) {
				Bind (new DockMaster ());
			}
			if (floating) {
				//Need code here to handle floating shit.
			}
			DockObjectFlags |= DockObjectFlags.Attached;
		}
		
		public Dock (Dock original, bool _floating) : this ()
		{
			Master = original.Master;
			floating = _floating;
		}
		
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
				if (Master != null)
					return Master.DefaultTitle;
				return null;
			}
			set {
				if (Master != null)
					Master.DefaultTitle = value;
			}
		}
		
		public int Width {
			get { return width; }
			set {
				width = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		public int Height {
			get { return height; }
			set {
				height = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		public int FloatX {
			get { return float_x; }
			set {
				float_x = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		public int FloatY {
			get { return float_y; }
			set {
				float_y = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			requisition.Width = 2 * (int)BorderWidth;
			requisition.Height = 2 * (int)BorderWidth;

			if (root != null && root.Visible) {
				Requisition rootReq = root.SizeRequest ();
				requisition.Width += rootReq.Width;
				requisition.Height += rootReq.Height;
			}			
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
		
			if (root != null && root.Visible) {
				int bw = (int)BorderWidth;
				Gdk.Rectangle childAlloc;
			
				childAlloc.X = allocation.X + bw;
				childAlloc.Y = allocation.Y + bw;
				childAlloc.Width = Math.Max (1, allocation.Width - 2 * bw);
				childAlloc.Height = Math.Max (1, allocation.Height - 2 * bw);
			
				root.SizeAllocate (childAlloc);
			}
		}
		
		protected override void OnMapped ()
		{
			base.OnMapped ();
			Console.WriteLine ("Mapping");
			if (root != null) {
				Console.WriteLine ("root.Visible = " + root.Visible);
				if (root.Visible && !root.IsMapped) {
					Console.WriteLine ("Mapping root");
					root.Map ();
				}
			}
		}
		
		protected override void OnUnmapped ()
		{
			base.OnUnmapped ();
			if (root != null) {
				if (root.Visible && root.IsMapped)
					root.Unmap ();
			}
			if (window != null)
				window.Unmap ();
		}
		
		protected override void OnShown ()
		{
			base.OnShown ();
			if (floating && window != null)
				window.Show ();
			if (IsController) {
				foreach (DockObject item in Master.TopLevelDocks) {
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
			if (floating && window != null)
				window.Hide ();
			/*PORT:
			    if (GDL_DOCK_IS_CONTROLLER (dock)) {
        gdl_dock_master_foreach_toplevel (GDL_DOCK_OBJECT_GET_MASTER (dock),
                                          FALSE, (GFunc) gdl_dock_foreach_automatic,
                                          gtk_widget_hide);
    		}*/
		}
		
		protected override void OnAdded (Widget widget)
		{
			Console.WriteLine ("OnAdded {0}", widget);
			DockItem child = widget as DockItem;
			if (child == null)
				return;
			
			AddItem (child, DockPlacement.Top);
		}
		
		protected override void OnRemoved (Widget widget)
		{
			bool was_visible = widget.Visible;
			if (root == widget) {
				root = null;
				((DockObject)widget).DockObjectFlags &= ~(DockObjectFlags.Attached);
				widget.Unparent ();
				if (was_visible && Visible)
					QueueResize ();
			}
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (root != null)
				invoker.Invoke (root);
		}
		
		/*PORT THIS CODE: its an override of Container.ChildType
		static GtkType
gdl_dock_child_type (GtkContainer *container)
{
    return GDL_TYPE_DOCK_ITEM;
}*/
		
		public override void Detach (bool recursive)
		{
			if (recursive && root != null)
				root.Detach (recursive);
			DockObjectFlags &= ~(DockObjectFlags.Attached);
		}
		
		public override void Reduce ()
		{
			if (root != null)
				return;
			
			if (IsAutomatic)
				Destroy ();
			else if (!(IsAttached)) {
				if (floating)
					Hide ();
				else {
					if (Parent != null && Parent is Container)
						((Container)Parent).Remove (this);
				}
			}
		}
		
		public override bool DockRequest (int x, int y, DockRequest request)
		{
			Gdk.Rectangle alloc = Allocation;
			int bw = (int)BorderWidth;
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
				
				if (root == null) {
					my_request.Position = DockPlacement.Top;
					my_request.Target = this;
				} else {
					my_request.Target = root;
					
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
						may_dock = root.DockRequest (x, y, my_request);
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
			} else if (root != null) {
				Console.WriteLine ("root was not null, docking to root");
				root.Docking (requestor, position, null);
				//gdl_dock_set_title (dock /*this*/);
			} else {
				Console.WriteLine ("root is null, setting requestor to root");
				root = requestor;
				root.DockObjectFlags &= DockObjectFlags.Attached;
				root.Parent = this;
				((DockItem)root).ShowGrip ();
				if (IsRealized) {
					Console.WriteLine ("realizing new root");
					root.Realize ();
				}
				if (Visible && root.Visible) {
					Console.WriteLine ("root is visible");
					if (IsMapped) {
						Console.WriteLine ("mapping new root");
						root.Map ();
					}
					root.QueueResize ();
				}
				//gdl_dock_set_title (dock /*this*/);
			}
		}
		
		public override bool Reorder (DockObject requestor, DockPlacement new_position, object other_data)
		{
			bool handled = false;
			if (floating && new_position == DockPlacement.Floating && root == requestor) {
				if (other_data != null && other_data is Gdk.Rectangle) {
					Gdk.Rectangle rect = (Gdk.Rectangle)other_data;
					if (window != null && window is Window) {
						((Window)window).Move (rect.X, rect.Y);
						handled = true;
					}
				}
			}
			return handled;
		}
		
		public override bool ChildPlacement (DockObject child, ref DockPlacement placement)
		{
			bool retval = true;
			if (root == child) {
				if (placement == DockPlacement.None || placement == DockPlacement.Floating)
					placement = DockPlacement.Top;
			} else
				retval = false;
				
			return retval;
		}
		
		public override void Present (DockObject child)
		{
			if (floating && window != null && window is Window)
				((Window)window).Present ();
		}
		
		public void AddItem (DockItem item, DockPlacement placement)
		{
			if (item == null)
				return;
			if (placement == DockPlacement.Floating)
				AddFloatingItem (item, 0, 0, -1, -1);
			else {
				Console.WriteLine ("about to dock");
				Docking (item, placement, null);
			}
		}
		
		public void AddFloatingItem (DockItem item, int x, int y, int width, int height)
		{
			Gdl.Dock new_dock = new Dock (this, true);
			new_dock.Width = width;
			new_dock.Height = height;
			new_dock.FloatX = x;
			new_dock.FloatY = y;
			if (Visible) {
				new_dock.Show ();
				if (IsMapped)
					new_dock.Map ();
				new_dock.QueueResize ();
			}
			new_dock.AddItem (item, DockPlacement.Top);
		}
		
		public DockItem GetItemByName (string name)
		{
			if (name == null)
				return null;
			DockObject found = Master.GetObject (name);
			if (found != null && found is DockItem)
				return (DockItem)found;
			return null;
		}
		
		public DockPlaceholder GetPlaceholderByName (string name)
		{
			if (name == null)
				return null;
			DockObject found = Master.GetObject (name);
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
			if (xor_gc == null) {
				if (IsRealized) {
					Gdk.GCValues values = new Gdk.GCValues ();
					values.Function = Gdk.Function.Invert;
					values.SubwindowMode = Gdk.SubwindowMode.IncludeInferiors;
					xor_gc = new Gdk.GC (GdkWindow);
					xor_gc.SetValues (values, Gdk.GCValuesMask.Function | Gdk.GCValuesMask.Subwindow);
				} else
					return;
			}
			xor_gc.SetLineAttributes (1, Gdk.LineStyle.OnOffDash, Gdk.CapStyle.NotLast, Gdk.JoinStyle.Bevel);
			xor_gc.SetDashes (1, new sbyte[] { 1, 1}, 2);
			GdkWindow.DrawRectangle (xor_gc, false, rect.X, rect.Y, rect.Width, rect.Height);
			xor_gc.SetDashes (0, new sbyte[] { 1, 1}, 2);
			GdkWindow.DrawRectangle (xor_gc, false, rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
		}
		
		private bool IsController {
			get {
				if (Master == null) {
					Console.WriteLine ("Master is null");
					return false;
				}
				if (Master.Controller == null)
					Console.WriteLine ("Master.Controller is null");
				return (Master.Controller == this); 
			}
		}
	}
}
