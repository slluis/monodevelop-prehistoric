// created on 06/06/2004 at 10:09 P
using System;
using Gtk;

namespace Gdl
{
	public class DockItem : DockObject
	{		
		private Gtk.Widget child = null;
		private DockItemBehavior behavior = DockItemBehavior.Normal;
		private Gtk.Orientation orientation = Gtk.Orientation.Horizontal;
		private uint resize = 1;
		private int dragoff_x = 0;
		private int dragoff_y = 0;
		private Gtk.Menu menu = null;
		private bool grip_shown;
		private DockItemGrip grip;
		private uint grip_size;
		private Gtk.Widget tab_label = null;
		private int preferred_width = -1;
		private int preferred_height = -1;
		private DockPlaceholder ph = null;
		private int start_x;
		private int start_y;
		
		static DockItem ()
		{
			Gtk.Rc.ParseString ("style \"gdl-dock-item-default\" {\n" +
			                    "xthickness = 0\n" +
			                    "ythickness = 0\n" + 
			                    "}\n" + 
			                    "class \"Gdl_DockItem\" " +
			                    "style : gtk \"gdl-dock-item-default\"\n");
		}
		
		public DockItem ()
		{
			if (this.HasGrip) {
				this.grip_shown = true;
				this.grip = new DockItemGrip (this);
				this.grip.Parent = this;
				this.grip.Show ();
			} else {
				this.grip_shown = false;
			}
			this.DockObjectFlags &= ~(DockObjectFlags.Automatic);
		}
		
		public DockItem (string name, string long_name, DockItemBehavior behavior) : this ()
		{
			this.Name = name;
			this.LongName = long_name;
			this.Behavior = behavior;
			//FIXME: Set the tab label here, should it just be an hbox or that
			//strange DockTabLabel with what looks like a lot of dead code
			//from gdl-dock
		}
		
		public DockItem (string name, string long_name, string stock_id, DockItemBehavior behavior) : this (name, long_name, behavior)
		{
			this.StockId = stock_id;
		}
		
		public virtual bool HasGrip {
			get { return true; }
		}
		
		public int DragOffX {
			get { return this.dragoff_x; }
			set { this.dragoff_x = value; }
		}
		
		public int DragOffY {
			get { return this.dragoff_y; }
			set { this.dragoff_y = value; }
		}
		
		public override bool IsCompound {
			get { return false; }
		}
		
		public Gtk.Orientation Orientation {
			get { return orientation; }
			set { SetOrientation (value); }
		}
		
		public bool Resize {
			get { return this.resize; }
			set {
				this.resize = value;
				this.QueueResize ();
			}
		}
		
		public DockItemBehavior Behavior {
			get { return behavior; }
			set {
				DockItemBehavior old_beh = this.behavior;
				this.behavior = value;
				if (((old_beh ^ this.behavior) & DockItemBehavior.Locked) != 0) {
					/* PORT THIS:
					                if (GDL_DOCK_OBJECT_GET_MASTER (item))
                    g_signal_emit_by_name (GDL_DOCK_OBJECT_GET_MASTER (item),
                                           "layout_changed");
                g_object_notify (g_object, "locked");
                gdl_dock_item_showhide_grip (item);
                */
                }
			}
		}
		
		public bool Locked {
			get { return !((this.behavior & DockItemBehavior.Locked) != 0); }
			set {
				DockItemBehavior old_beh = this.behavior;
				if (value)
					this.behavior |= DockItemBehavior.Locked;
				else
					this.behavior &= ~(DockItemBehavior.Locked);
				if ((old_beh ^ this.behavior) != 0) {
					//PORT THIS:
					//gdl_dock_item_showhide_grip (item /*this*/);
					//g_object_notify (g_object, "behavior");
					//if (GDL_DOCK_OBJECT_GET_MASTER (item))
					//    g_signal_emit_by_name (GDL_DOCK_OBJECT_GET_MASTER (item)), "layout_changed");
				}
			}
		}
		
		public int PreferredWidth {
			get { return this.preferred_width; }
			set { this.preferred_width = value; }
		}
		
		public int PreferredHeight {
			get { return this.preferred_height; }
			set { this.preferred_height = value; }
		}
		
		public bool InDrag {
			get { return ((this.DockObjectFlags & DockObjectFlags.InDrag) != 0); }
		}
		
		public bool InPreDrag {
			get { return ((this.DockObjectFlags & DockObjectFlags.InPreDrag) != 0); }
		}
		
		public bool Iconified {
			get { return ((this.DockObjectFlags & DockObjectFlags.Iconified) != 0); }
		}
		
		public bool UserAction {
			get { return ((this.DockObjectFlags & DockObjectFlags.UserAction) != 0); }
		}
		
		public bool GripShown {
			get { return (this.HasGrip && !this.Locked && this.grip_shown); }
		}
		
		public bool CantClose {
			get { return ((this.Behavior & DockItemBehavior.CantClose) != 0); }
		}
		
		public bool CantIconify {
			get { return ((this.Behavior & DockItemBehavior.CantIconify) != 0); }
		}
		
		protected override OnAdded (Gtk.Widget widget)
		{
			if (widget is DockObject) {
				Console.WriteLine ("You can't add a DockObject to a DockItem");
				return;
			}
			if (this.Child != null) {
				Console.WriteLine ("This DockItem already has a child");
				return;
			}
			widget.Parent = this;
			this.Child = widget;
		}
		
		protected override OnRemoved (Gtk.Widget widget)
		{
			if (this.grip == widget) {
				bool grip_was_visible = widget.Visible;
				widget.Unparent ();
				this.grip = null;
				if (grip_was_visible)
					this.QueueResize ();
				return;
			}
			if (this.InDrag) {
				this.DragEnd (true);
			}
			
			if (widget != this.Child)
				return;
			
			bool was_visible = widget.Visible;
			widget.Unparent ();
			this.Child = null;
			
			if (was_visible)
				this.QueueResize ();
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals && this.grip != null)
				invoker.Invoke (this.grip);
			if (this.Child != null)
				invoker.Invoke (this.Child);
		}
		
		protected void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			Gtk.Requisition child_requisition = new Gtk.Requisition ();
			Gtk.Requisition grip_requisition = new Gtk.Requisition ();
			
			if (this.Child)
				child_requistion = this.Child.SizeRequest ();
			else {
				child_requisition.Width = 0;
				child_requisition.Height = 0;
			}
			if (this.Orientation == Gtk.Orientation.Horizontal) {
				if (this.GripShown) {
					grip_requisition = this.grip.SizeRequest ();
					requisition.Width = grip_requisition.Width;
				} else {
					requisition.Width = 0;
				}
				
				if (this.Child != null) {
					requisition.Width += child_requisition.Width;
					requisition.Height = child_requisition.Height;
				} else {
					requisition.Height = 0;
				}
			} else {
				if (this.GripShown) {
					grip_requisition = this.grip.SizeRequest ();
					requisition.Height = grip_requisition.Height;
				} else {
					requisition.Height = 0;
				}
				
				if (this.Child != null) {
					requisition.Width = child_requisition.Width;
					requisition.Height += child_requisition.Height;
				} else {
					requisition.Width = 0;
				}
			}
			requisition.Width += (this.BorderWidth + this.Style.XThickness) * 2;
			requisition.Width += (this.BorderWidth + this.Style.YThickness) * 2;
			this.SetSizeRequest (requisition.Width, requisition.Height);
		}
		
		protected override OnSizeAllocated (ref Gdk.Rectangle allocation)
		{
			this.Allocation = allocation;
			if (this.IsRealized) {
				this.GdkWindow.MoveResize (allocation.X, allocation.Y, allocation.Width, allocation.Height);
			}
			if (this.Child != null && this.Child.Visible) {
				int border_width = this.BorderWidth;
				Gdk.Rectangle child_allocation = new Gdk.Rectangle ();
				child_allocation.X = border_width + this.Style.XThickness;
				child_allocation.Y = border_width + this.Style.YThickness;
				child_allocation.Width = allocation.Width - 2 * (border_width + this.Style.XThickness);
				child_allocation.Height = allocation.Height - 2 * (border_width + this.Style.YThickness);
				
				if (this.GripShown) {
					Gdk.Rectangle grip_alloc = child_allocation;
					Gtk.Requisition grip_req = this.grip.SizeRequest ();
					if (this.Orientation == Gtk.Orientation.Horizontal) {
						child_allocation.X += grip_req.Width;
						child_allocation.Width -= grip_req.Width;
						grip_alloc.Width = grip_req.Width;
					} else {
						child_allocation.Y += grip_req.Height;
						child_allocation.Height -= grip_req.Height;
						grip_alloc.Height = grip_req.Height;
					}
					if (this.grip != null) {
						this.grip.SizeAllocate (grip_alloc);
					}
				}
				this.Child.SizeAllocate (grip_alloc);
			}
		}
		
		protected override OnMapped ()
		{
			this.Flags |= (int)Gtk.WidgetFlags.Mapped;
			this.GdkWindow.Show ();
			if (this.Child != null && this.Child.Visible && !this.Child.IsMapped)
				this.Child.Map ();
			if (this.grip != null && this.grip.Visible && !this.grip.IsMapped)
				this.grip.Map ();
		}
		
		protected override OnUnmapped ()
		{
			this.Flags &= ~((int)Gtk.WidgetFlags.Mapped);
			this.GdkWindow.Hide ();
			if (this.grip != null)
				this.grip.Unmap ();
		}
		
		protected override OnRealized ()
		{
			this.Flags |= (int)Gtk.WidgetFlags.Realized;
			Gdk.WindowAttr attributes;
			attributes.X = this.Allocation.X;
			attributes.Y = this.Allocation.Y;
			attributes.Height = this.Allocation.Height;
			attributes.Width = this.Allocation.Width;
			attributes.WindowType = Gdk.WindowType.Child;
			attributes.Wclass = Gdk.WindowClass.InputOutput;
			attributes.visual = this.Visual;
			attributes.colormap = this.Colormap;
			attributes.EventMask = (this.Events | Gdk.EventMask.ExposureMask | Gdk.EventMask.Button1MotionMask | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask);
			Gdk.WindowAttributesType attributes_mask = Gdk.WindowAttributesType.X | Gdk.WindowAttributesType.Y | Gdk.WindowAttributesType.Colormap | Gdk.WindowAttributesType.Visual;
			this.GdkWindow = new Gdk.Window (this.ParentWindow, attributes, (int)attributes_mask);
			this.GdkWindow.UserData = this.Handle;
			this.Style = this.Style.Attach (this.GdkWindow);
			this.Style.SetBackground (this.GdkWindow, this.State);
			this.GdkWindow.SetBackPixmap (null, true);
			if (this.Child != null)
				this.Child.ParentWindow = this.GdkWindow;
			if (this.grip != null)
				this.grip.ParentWindow = this.GdkWindow;
		}
		
		protected override OnStyleSet (Gtk.Style previous_style)
		{
			if (this.IsRealized && !this.NoWindow) {
				this.Style.SetBackground (this.GdkWindow, this.State);
				if (this.Drawable) {
					this.GdkWindow.Clear ();
				}
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			if (this.Drawable && evnt.Window == this.GdkWindow) {
				Gtk.Style.PaintBox (this.GdkWindow, this.State, Gtk.ShadowType.None, evnt.Area, this, "dockitem", 0, 0, -1, -1);
				base.OnExposeEvent (evnt);
			}
			return false;
		}
		
		private bool EventInGripWindow (Gdk.Event evnt)
		{
			if (this.grip != null && this.grip.TitleWindow == evnt.Window)
				return true;
			return false;
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			if (!EventInGripWindow (evnt))
				return false;
			if (this.Locked)
				return false;
			
			bool event_handled = false;
			bool in_handle;
			Gdk.Cursor cursor = null;
			
			switch (this.Orientation) {
			case Gtk.Orientation.Horizontal:
				in_handle = evnt.X < this.grip.Allocation.Width;
				break;
			case Gtk.Orientation.Vertical:
				in_handle = evnt.Y < this.grip.Allocation.Height;
				break;
			default:
				in_handle = false;
				break;
			}
			
			if (evnt.Button == 1 && evnt.Type == Gdk.EventType.ButtonPress) {
				if (in_handle) {
					this.start_x = evnt.X;
					this.start_y = evnt.Y;
					this.DockObjectFlags |= DockObjectFlags.InPreDrag;
					cursor = new Gdk.Cursor (this.Display, (int)Gdk.CursorType.Fleur);
					this.grip.TitleWindow.Cursor = cursor;
					event_handled = true;
				}
			} else if (evnt.Type == Gdk.EventType.ButtonRelease && evnt.Button == 1) {
				if (this.InDrag) {
					this.DragEnd (false);
					event_handled = true;
				} else if (this.InPreDrag) {
					this.DockObjectFlags &= ~(DockObjectFlags.InPreDrag);
					event_handled = true;
				}
				
				if (this.grip.TitleWindow != null) {
					cursor = new Gdk.Cursor (this.Display, (int)Gdk.CursorType.Hand2);
					this.grip.TitleWindow.Cursor = cursor;
				}
			} else if (evnt.Button == 3 && evnt.Type == Gdk.EventType.ButtonPress && in_handle) {
				this.PopupMenu (evnt.Button, evnt.Time);
				event_handled = true;
			}
			return event_handled;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			return OnButtonPressEvent (evnt);
		}
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			if (!EventInGripWindow (evnt))
				return false;
			if (this.InPreDrag) {
				if (Gtk.Drag.CheckThreshold (this, this.start_x, this.start_y, evnt.X, evnt.Y)) {
					this.DockObjectFlags &= ~(DockObjectFlags.InPreDrag);
					this.dragoff_x = this.start_x;
					this.dragoff_y = this.start_y;
					this.DragStart ();
				}
			}
			
			if (!this.InDrag)
				return false;
			
			int new_x = evnt.XRoot;
			int new_y = evnt.YRoot;
			
			//PORT THIS:
			//    g_signal_emit (item, gdl_dock_item_signals [DOCK_DRAG_MOTION], 0, new_x, new_y);
			return true;
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if (this.InDrag && evnt.Key == Gdk.Key.Escape) {
				this.DragEnd (true);
				return true;
			}
			return base.OnKeyPressEvent (evnt);
		}
		
		protected static PreferredSize (DockItem item, ref Gdk.Rectangle req)
		{
			req.Width = Math.Max (item.preferred_width, item.Allocation.Width);
			req.Height = Math.Max (item.preferred_height, item.Allocation.Height);
		}
		
		public override bool DockRequest (int x, int y, DockRequest request)
		{
			Gdk.Rectangle alloc = this.Allocation;
			int rel_x = x - alloc.X;
			int rel_y = y - alloc.Y;
			
			if (rel_x > 0 && rel_x < alloc.Width && rel_y > 0 && rel_y < alloc.Width) {
				Gdk.Rectangle other, my;
				int divider;
				DockItem.PreferredSize (request.Applicant, ref other);
				DockItem.PreferredSize (this, ref my);
				float rx = (float) rel_x / alloc.Width;
				float ry = (float) rel_y / alloc.Height;
				
				if (rx < 0.4) {
					request.Position = DockPlacement.Left;
					divider = other.Width;
				} else if (rx > (1 - 0.4) {
					request.Position = DockPlacement.Right;
					rx = 1 - rx;
					divider = Math.Max (0, my.Width - other.Width);
				} else if (ry < 0.4 && ry < rx) {
					request.Position = DockPlacement.Top;
					divider = other.Height;
				} else if (ry > (1 - 0.4) && (1 - ry) < rx) {
					request.Position = DockPlacement.Bottom;
					divider = Math.Max (0, my.Height - other.Height);
				} else
					request.Position = DockPlacement.Center;
				
				Gdk.Rectangle req_rect = new Gdk.Rectangle ();
				req_rect.X = 0;
				req_rect.Y = 0;
				req_rect.Width = alloc.Width;
				req_rect.Height = alloc.Height;
				
				if (request.Applicant != this) {
					switch (request.Position) {
					case DockPlacement.Top:
						req_rect.Height *= 0.4;
						break;
					case DockPlacement.Bottom:
						req_rect.Y += req_rect.Height * (1 - 0.4);
						req_rect.Height *= 0.4;
						break;
					case DockPlacement.Left:
						req_rect.Width *= 0.4;
						break;
					case DockPlacement.Right:
						req_rect.X += req_rect.Width * (1 - 0.4);
						req_rect.Width *= 0.4;
						break;
					case DockPlacement.Center:
						req_rect.X = req_rect.Width * 0.2;
						req_rect.Y = req_rect.Height * 0.2;
						req_rect.Width = (req_rect.Width * (1 - 0.2)) - req_rect.X;
						req_rect.Height = (req_rect.Height * (1 - 0.2)) - req_rect.Y;
						break;
					default:
						break;
					}
				}
				
				req_rect.X += alloc.X;
				req_rect.Y += alloc.Y;
				request.Target = this;
				request.Rect = req_rect;
				if (request.Position != DockPlacement.Center && divider >= 0)
					request.Extra = divider;
				return true;
			} else
				return false;
		}
		
		public override void Dock (DockObject requestor, DockPlacement position, object other_data)
		{
			DockObject new_parent = null;
			bool add_ourselves_first;
			
			switch (position) {
			case DockPlacement.Top:
			case DockPlacement.Bottom:
				new_parent = new DockPaned (Gtk.Orientation.Vertical);
				add_ourselves_first = (position == DockPlacement.Bottom);
				break;
			case DockPlacement.Left:
			case DockPlacement.Right:
				new_parent = new DockPaned (Gtk.Orientation.Horizontal);
				add_ourselves_first = (position == DockPlacement.Right);
				break;
			case DockPlacement.Center:
				new_parent = new DockNotebook ();
				add_ourselves_first = true;
				break;
			default:
				Console.WriteLine ("Unsupported docking strategy");
				return;
			}
			
			if (this.ParentObject != null)
				this.ParentObject.Freeze ();
			this.DockObjectFlags |= DockObjectFlags.InReflow;
			this.Detach (false);
			new_parent.Freeze ();
			new_parent.Bind (this.Master);
			
			if (add_ourselves_first) {
				new_parent.Add (this);
				new_parent.Add (requestor);
			} else {
				new_parent.Add (requestor);
				new_parent.Add (this);
			}
			
			if (this.ParentObject != null)
				this.ParentObject.Add (new_parent);
			
			if (this.Visible)
				new_parent.Show ();
			
			if (position != DockPlacement.Center && other_data != null && other_data is System.Int32) {
				//PORT THIS:
				//g_object_set (G_OBJECT (new_parent), "position", g_value_get_uint (other_data), NULL);
			}
			
			this.DockObjectFlags &= ~(DockObjectFlags.InReflow);
			new_parent.Thaw ();
			if (this.ParentObject != null)
				this.ParentObject.Thaw ();
		}
		
		private void DetachMenu (Gtk.Widget item, Gtk.Menu menu)
		{
			if (item is DockItem)
				((DockItem)item).menu = null;
		}
		
		public void PopupMenu (int button, int time)
		{
			if (this.menu == null) {
				this.menu = new Gtk.Menu ();
				this.menu.AttachToWidget (this, new MenuDetachFunc (DetachMenu));
				
				Gtk.MenuItem mitem = new Gtk.MenuItem ("Hide");
				mitem.Activated += new EventHandler (ItemHideCb);
				this.menu.Append (mitem);
			}
			this.menu.ShowAll ();
			this.menu.Popup (null, null, null, null, button, time);
		}
		
		private void ItemHideCb (object o, EventArgs e)
		{
			this.HideItem ();
		}
		
		public void DragStart ()
		{
			Gdk.Cursor fleur = new Gdk.Cursor (Gdk.CursorType.Fleur);
			
			if (!this.IsRealized)
				this.Realize ();
			
			this.DockObjectFlags |= DockObjectFlags.InDrag;
			
			Gtk.Grab.Add (this);
			
			//PORT THIS:
			//g_signal_emit (item, gdl_dock_item_signals [DOCK_DRAG_BEGIN], 0);
		}
		
		public void DragEnd ()
		{
			Gtk.Grab.Remove (Gtk.Grab.Current);
			
			//PORT THIS:
			//g_signal_emit (item, gdl_dock_item_signals [DOCK_DRAG_END], 0);
			
			this.DockObjectFlags &= ~(DockObjectFlags.InDrag);
		}
		
		private void ShowHideGrip ()
		{
			if (this.grip != null) {
				if (this.GripShown)
					this.grip.Show ();
				else
					this.grip.Hide ();
			}
			this.QueueResize ();
		}
		
		public void DockTo (DockItem target, DockPlacement position, int docking_param)
		{
			if (target == null || position == DockPlacement.Floating)
				return;
			if (position == DockPlacement.Floating || target == null) {
				if (!this.IsBound) {
					Console.WriteLine ("Attempting to bind an unbound object");
					return;
				}
				this.dragoff_x = this.dragoff_y = 0;
				((Dock)this.Master.Controller).AddFloatingItem (this, 0, 0, -1, -1);
			} else
				target.Dock (item, position, null);
		}
		
		public virtual void SetOrientation (Gtk.Orientation orientation)
		{
			if (this.Orientation != orientation) {
				if (this.Child != null) {
					//FIXME: Port this, prolly w/ reflection
					            /*pspec = g_object_class_find_property (
                G_OBJECT_GET_CLASS (item->child), "orientation");
            if (pspec && pspec->value_type == GTK_TYPE_ORIENTATION)
                g_object_set (G_OBJECT (item->child),
                              "orientation", orientation,
                              NULL);*/
				}
				//PORT THIS:
				//        g_object_notify (G_OBJECT (item), "orientation");
			}
		}
		
		public void HideGrip ()
		{
			if (this.grip_shown) {
				this.grip_shown = false;
				this.ShowHideGrip ();
			}
		}
		
		public void ShowGrip ()
		{
			if (!this.grip_shown) {
				this.grip_shown = true;
				this.ShowHideGrip ();
			}
		}
		
		public void Bind (Dock dock)
		{
			if (dock == null)
				return;
			
			this.Bind (dock.Master);
		}
		
		public void HideItem ()
		{
			if (!this.IsAttached)
				return;
			
			if (!this.IsAutomatic) {
				this.ph = new DockPlaceholder (this, false);
			}
			
			this.Freeze ();
			if (this.IsCompound) {
				this.Foreach (new Gtk.Callback (HideItem));
			}
			
			this.Detach (true);
			this.Thaw ();
		}
		
		public void IconifyItem ()
		{
			this.DockObjectFlags |= DockObjectFlags.Iconified;
			this.HideItem ();
		}
		
		public void ShowItem ()
		{
			this.DockObjectFlags &= ~(DockObjectFlags.Iconified);
			
			if (this.ph != null) {
				this.ph.Add (this);
				this.ph = null;
			} else if (this.IsBound) {
				if (this.Master.Controller != null) {
					this.Master.Controller.Dock (this, DockPlacement.Floating, null);
				}
			}
		}
		
		public void SetDefaultPosition (DockObject reference)
		{
			this.ph = null;
			
			if (reference != null && reference.IsAttached) {
				if (reference is DockPlaceholder) {
					this.ph = reference;
				} else {
					this.ph = new DockPlaceholder (reference, true);
				}
			}
		}
	}
}