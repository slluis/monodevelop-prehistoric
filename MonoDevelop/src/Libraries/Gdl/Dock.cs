// created on 05/06/2004 at 11:21 A

using System;
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
		
		protected void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			int border_width = this.BorderWidth;
			if (this.root != null && this.root.Visible)
				this.root.SizeRequest (requisition);
			else {
				requisition.Width = 0;
				requisition.Height = 0;
			}
			
			requisition.Width += 2 * border_width;
			requisition.Height += 2 * border_width;
		}
		
		protected void OnSizeAllocated (ref Gdk.Rectangle allocation)
		{
			int border_width = this.BorderWidth;
			allocation.X += border_width;
			allocation.Y += border_width;
			allocation.Width = Math.Max (1, allocation.Width - 2 * border_width);
			allocation.Height = Math.Max (1, allocation.Height - 2 * border_width);
			
			if (this.root != null && this.root.Visible)
				this.root.SizeAllocate (allocation);
		}
		
		protected void OnMapped ()
		{
			base.OnMapped ();
			if (this.root != null) {
				if (this.root.Visible && !this.root.IsMapped)
					this.root.Map ();
			}
		}
		
		protected void OnUnmapped ()
		{
			base.OnUnmapped ();
			if (this.root != null) {
				if (this.root.Visible && this.root.IsMapped)
					this.root.Unmap ();
			}
			if (this.window != null)
				window.Unmap ();
		}
		
		public override void Show ()
		{
			base.Show ();
			if (this.floating && this.window != null)
				this.window.Show ();
			/*PORT:
			    if (GDL_DOCK_IS_CONTROLLER (dock)) {
        gdl_dock_master_foreach_toplevel (GDL_DOCK_OBJECT_GET_MASTER (dock),
                                          FALSE, (GFunc) gdl_dock_foreach_automatic,
                                          gtk_widget_show);
    			}
			*/
		}
		
		public override void Hide ()
		{
			base.Hide ();
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
			if (this.root)
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
					if (this.Parent && this.Parent is Gtk.Container)
						((Gtk.Container)this.Parent).Remove (this);
				}
			}
		}
		
		public override bool DockRequest (int x, int y, DockRequest request)
		{
			Gdk.Rectangle alloc = this.Allocation;
			uint bw = this.BorderWidth;
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
						req_rect.Width *= 0.3;
						my_request.Rect = req_rect;
					} else if (rel_x > alloc.Width - bw) {
						my_request.Position = DockPlacement.Right;
						req_rect.X += req_rect.Width * (1 - 0.3);
						req_rect.Width *= 0.3;
						my_request.Rect = req_rect;
					} else if (rel_y < bw) {
						my_request.Position = DockPlacement.Top;
						req_rect.Height *= 0.3;
						my_request.Rect = req_rect;
					} else if (rel_y > alloc.Height - bw) {
						my_request.Position = DockPlacement.Bottom;
						req_rect.y += req_rect.Height * (1 - 0.3;
						req_rect.Height *= 0.3;
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
		
		private void AddItem (DockItem item, DockPlacement placement)
		{
		}
		
		private bool IsController {
			get {
				return (this.Master.Controller == this); 
			}
		}
	}
}