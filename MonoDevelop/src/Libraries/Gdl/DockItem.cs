// created on 06/06/2004 at 10:09 P
using System;
using Gtk;

namespace Gdl
{
	public class DockItem : DockObject
	{		
		private Widget child = null;
		private DockItemBehavior behavior = DockItemBehavior.Normal;
		private Orientation orientation = Orientation.Vertical;
		private bool resize = false;
		private int dragoff_x = 0;
		private int dragoff_y = 0;
		private Menu menu = null;
		private bool grip_shown;
		private DockItemGrip grip;
		private uint grip_size;
		private Widget tab_label = null;
		private int preferred_width = -1;
		private int preferred_height = -1;
		private DockPlaceholder ph = null;
		private int start_x;
		private int start_y;
		
		static DockItem ()
		{
			Rc.ParseString ("style \"gdl-dock-item-default\" {\n" +
			                    "xthickness = 0\n" +
			                    "ythickness = 0\n" + 
			                    "}\n" + 
			                    "class \"Gdl_DockItem\" " +
			                    "style : gtk \"gdl-dock-item-default\"\n");
		}
		
		public DockItem ()
		{
			if (HasGrip) {
				grip_shown = true;
				grip = new DockItemGrip (this);
				grip.Parent = this;
				grip.Show ();
			} else {
				grip_shown = false;
			}
			DockObjectFlags &= ~(DockObjectFlags.Automatic);
		}
		
		public DockItem (string name, string long_name, DockItemBehavior behavior) : this ()
		{
			Name = name;
			LongName = long_name;
			Behavior = behavior;
			//FIXME: Set the tab label here, should it just be an hbox or that
			//strange DockTabLabel with what looks like a lot of dead code
			//from gdl-dock
		}
		
		public DockItem (string name, string long_name, string stock_id, DockItemBehavior behavior) : this (name, long_name, behavior)
		{
			StockId = stock_id;
		}
		
		public Widget TabLabel {
			get { return tab_label; }
			set { tab_label = value; }
		}
		
		public new Widget Child {
			get { return child; }
			set { child = value; }
		}
		
		public virtual bool HasGrip {
			get { return true; }
		}
		
		public int DragOffX {
			get { return dragoff_x; }
			set { dragoff_x = value; }
		}
		
		public int DragOffY {
			get { return dragoff_y; }
			set { dragoff_y = value; }
		}
		
		public override bool IsCompound {
			get { return false; }
		}
		
		public Orientation Orientation {
			get { return orientation; }
			set { SetOrientation (value); }
		}
		
		public bool Resize {
			get { return resize; }
			set {
				resize = value;
				QueueResize ();
			}
		}
		
		public DockItemBehavior Behavior {
			get { return behavior; }
			set {
				DockItemBehavior old_beh = behavior;
				behavior = value;
				if (((old_beh ^ behavior) & DockItemBehavior.Locked) != 0) {
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
			get { return ((behavior & DockItemBehavior.Locked) != 0); }
			set {
				DockItemBehavior old_beh = behavior;
				if (value)
					behavior |= DockItemBehavior.Locked;
				else
					behavior &= ~(DockItemBehavior.Locked);
				if ((old_beh ^ behavior) != 0) {
					//PORT THIS:
					//gdl_dock_item_showhide_grip (item /*this*/);
					//g_object_notify (g_object, "behavior");
					//if (GDL_DOCK_OBJECT_GET_MASTER (item))
					//    g_signal_emit_by_name (GDL_DOCK_OBJECT_GET_MASTER (item)), "layout_changed");
				}
			}
		}
		
		public int PreferredWidth {
			get { return preferred_width; }
			set { preferred_width = value; }
		}
		
		public int PreferredHeight {
			get { return preferred_height; }
			set { preferred_height = value; }
		}
		
		public bool InDrag {
			get { return ((DockObjectFlags & DockObjectFlags.InDrag) != 0); }
		}
		
		public bool InPreDrag {
			get { return ((DockObjectFlags & DockObjectFlags.InPreDrag) != 0); }
		}
		
		public bool Iconified {
			get { return ((DockObjectFlags & DockObjectFlags.Iconified) != 0); }
		}
		
		public bool UserAction {
			get { return ((DockObjectFlags & DockObjectFlags.UserAction) != 0); }
		}
		
		public bool GripShown {
			get {
				return (HasGrip && !Locked && grip_shown);
			}
		}
		
		public bool CantClose {
			get { return ((Behavior & DockItemBehavior.CantClose) != 0); }
		}
		
		public bool CantIconify {
			get { return ((Behavior & DockItemBehavior.CantIconify) != 0); }
		}
		
		protected override void OnAdded (Widget widget)
		{
			if (widget is DockObject) {
				Console.WriteLine ("You can't add a DockObject to a DockItem");
				return;
			}
			if (Child != null) {
				Console.WriteLine ("This DockItem already has a child");
				return;
			}
			widget.Parent = this;
			Child = widget;
		}
		
		protected override void OnRemoved (Widget widget)
		{
			if (grip == widget) {
				bool grip_was_visible = widget.Visible;
				widget.Unparent ();
				grip = null;
				if (grip_was_visible)
					QueueResize ();
				return;
			}
			if (InDrag) {
				DockDragEnd ();
			}
			
			if (widget != Child)
				return;
			
			bool was_visible = widget.Visible;
			widget.Unparent ();
			Child = null;
			
			if (was_visible)
				QueueResize ();
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals && grip != null)
				invoker.Invoke (grip);
			if (Child != null)
				invoker.Invoke (Child);
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			requisition.Width = ((int)BorderWidth + Style.XThickness) * 2;
			requisition.Height = ((int)BorderWidth + Style.YThickness) * 2;
		
			Requisition childReq;
			if (Child != null && Child.Visible) {
				childReq = Child.SizeRequest ();
			} else {
				childReq.Width = 0;
				childReq.Height = 0;
			}

			Requisition gripReq;
			gripReq.Width = gripReq.Height = 0;

			if (Orientation == Orientation.Horizontal) {
				if (GripShown) {
					gripReq = grip.SizeRequest ();
					requisition.Width += gripReq.Width;
				}
				
				if (Child != null) {
					requisition.Width += childReq.Width;
					requisition.Height += Math.Max (childReq.Height,
									gripReq.Height);
				}
			} else {
				if (GripShown) {
					gripReq = grip.SizeRequest ();
					requisition.Height += gripReq.Height;
				}
				
				if (Child != null) {
					requisition.Width += Math.Max (childReq.Width,
								       gripReq.Width);
					requisition.Height += childReq.Height;
				}
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			if (IsRealized) {
				GdkWindow.MoveResize (allocation.X, allocation.Y,
						      allocation.Width, allocation.Height);
			}
			
			if (Child != null && Child.Visible) {
				int bw = (int)BorderWidth;
				Gdk.Rectangle childAlloc;
				
				childAlloc.X = bw + Style.XThickness;
				childAlloc.Y = bw + Style.YThickness;
				childAlloc.Width = allocation.Width - 2 * (bw + Style.XThickness);
				childAlloc.Height = allocation.Height - 2 * (bw + Style.YThickness);
				
				if (GripShown) {
					Gdk.Rectangle gripAlloc = childAlloc;
					Requisition gripReq = grip.SizeRequest ();
					
					if (Orientation == Orientation.Horizontal) {
						childAlloc.X += gripReq.Width;
						childAlloc.Width -= gripReq.Width;
						gripAlloc.Width = gripReq.Width;
					} else {
						childAlloc.Y += gripReq.Height;
						childAlloc.Height -= gripReq.Height;
						gripAlloc.Height = gripReq.Height;
					}
					
					grip.SizeAllocate (gripAlloc);
				}

				Child.SizeAllocate (childAlloc);
			}
		}
		
		protected override void OnMapped ()
		{
			Flags |= (int)WidgetFlags.Mapped;
			GdkWindow.Show ();
			if (Child != null && Child.Visible && !Child.IsMapped)
				Child.Map ();
			if (grip != null && grip.Visible && !grip.IsMapped)
				grip.Map ();
		}
		
		protected override void OnUnmapped ()
		{
			Flags &= ~((int)WidgetFlags.Mapped);
			GdkWindow.Hide ();
			if (grip != null)
				grip.Unmap ();
		}
		
		protected override void OnRealized ()
		{
			Flags |= (int)WidgetFlags.Realized;
			
			Gdk.WindowAttr attributes = new Gdk.WindowAttr ();
			attributes.X = Allocation.X;
			attributes.Y = Allocation.Y;
			attributes.Height = Allocation.Height;
			attributes.Width = Allocation.Width;
			attributes.WindowType = Gdk.WindowType.Child;
			attributes.Wclass = Gdk.WindowClass.InputOutput;
			attributes.visual = Visual;
			attributes.colormap = Colormap;
			attributes.EventMask = (int)(Events |
				Gdk.EventMask.ExposureMask |
				Gdk.EventMask.Button1MotionMask |
				Gdk.EventMask.ButtonPressMask |
				Gdk.EventMask.ButtonReleaseMask);
		
			Gdk.WindowAttributesType attributes_mask =
				Gdk.WindowAttributesType.X |
				Gdk.WindowAttributesType.Y |
				Gdk.WindowAttributesType.Colormap |
				Gdk.WindowAttributesType.Visual;
			GdkWindow = new Gdk.Window (ParentWindow, attributes, (int)attributes_mask);
			GdkWindow.UserData = Handle;
			
			Style = Style.Attach (GdkWindow);
			Style.SetBackground (GdkWindow, State);
			
			GdkWindow.SetBackPixmap (null, true);
			
			if (Child != null)
				Child.ParentWindow = GdkWindow;
			if (grip != null)
				grip.ParentWindow = GdkWindow;
		}
		
		protected override void OnStyleSet (Style previous_style)
		{
			if (IsRealized && !NoWindow) {
				Style.SetBackground (GdkWindow, State);
				if (Drawable) {
					GdkWindow.Clear ();
				}
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			if (Drawable && evnt.Window == GdkWindow) {
				Style.PaintBox (Style, GdkWindow, State, ShadowType.None, evnt.Area, this, "dockitem", 0, 0, -1, -1);
				base.OnExposeEvent (evnt);
			}
			return false;
		}
		
		private bool EventInGripWindow (Gdk.Event evnt)
		{
			if (grip != null && grip.TitleWindow == evnt.Window)
				return true;
			return false;
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			if (!EventInGripWindow (evnt))
				return false;
			if (Locked)
				return false;
			
			bool event_handled = false;
			bool in_handle;
			Gdk.Cursor cursor = null;
			
			switch (Orientation) {
			case Orientation.Horizontal:
				in_handle = evnt.X < grip.Allocation.Width;
				break;
			case Orientation.Vertical:
				in_handle = evnt.Y < grip.Allocation.Height;
				break;
			default:
				in_handle = false;
				break;
			}
			
			if (evnt.Button == 1 && evnt.Type == Gdk.EventType.ButtonPress) {
				if (in_handle) {
					start_x = (int)evnt.X;
					start_y = (int)evnt.Y;
					DockObjectFlags |= DockObjectFlags.InPreDrag;
					cursor = new Gdk.Cursor (Display, Gdk.CursorType.Fleur);
					grip.TitleWindow.Cursor = cursor;
					event_handled = true;
				}
			} else if (evnt.Type == Gdk.EventType.ButtonRelease && evnt.Button == 1) {
				if (InDrag) {
					DockDragEnd ();
					event_handled = true;
				} else if (InPreDrag) {
					DockObjectFlags &= ~(DockObjectFlags.InPreDrag);
					event_handled = true;
				}
				
				if (grip.TitleWindow != null) {
					cursor = new Gdk.Cursor (Display, Gdk.CursorType.Hand2);
					grip.TitleWindow.Cursor = cursor;
				}
			} else if (evnt.Button == 3 && evnt.Type == Gdk.EventType.ButtonPress && in_handle) {
				DockPopupMenu (evnt.Button, evnt.Time);
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
			if (InPreDrag) {
				if (Drag.CheckThreshold (this, start_x, start_y, (int)evnt.X, (int)evnt.Y)) {
					DockObjectFlags &= ~(DockObjectFlags.InPreDrag);
					dragoff_x = start_x;
					dragoff_y = start_y;
					DockDragStart ();
				}
			}
			
			if (!InDrag)
				return false;
			
			int new_x = (int)evnt.XRoot;
			int new_y = (int)evnt.YRoot;
			
			//PORT THIS:
			//    g_signal_emit (item, gdl_dock_item_signals [DOCK_DRAG_MOTION], 0, new_x, new_y);
			return true;
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if (InDrag && evnt.Key == Gdk.Key.Escape) {
				DockDragEnd ();
				return true;
			}
			return base.OnKeyPressEvent (evnt);
		}
		
		public static Requisition PreferredSize (DockItem item)
		{
			Requisition req;
			req.Width = Math.Max (item.preferred_width, item.Allocation.Width);
			req.Height = Math.Max (item.preferred_height, item.Allocation.Height);
			return req;
		}
		
		public override bool DockRequest (int x, int y, DockRequest request)
		{
			Gdk.Rectangle alloc = Allocation;
			int rel_x = x - alloc.X;
			int rel_y = y - alloc.Y;
			
			if (rel_x > 0 && rel_x < alloc.Width && rel_y > 0 && rel_y < alloc.Width) {
				Requisition other = DockItem.PreferredSize ((DockItem)request.Applicant);
				Requisition my = DockItem.PreferredSize (this);
				int divider = 0;
				float rx = (float) rel_x / alloc.Width;
				float ry = (float) rel_y / alloc.Height;
				
				if (rx < 0.4) {
					request.Position = DockPlacement.Left;
					divider = other.Width;
				} else if (rx > (1 - 0.4)) {
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
						req_rect.Height = (int)(req_rect.Height * 0.4);
						break;
					case DockPlacement.Bottom:
						req_rect.Y += (int)(req_rect.Height * (1 - 0.4));
						req_rect.Height = (int)(req_rect.Height * 0.4);
						break;
					case DockPlacement.Left:
						req_rect.Width = (int)(req_rect.Width * 0.4);
						break;
					case DockPlacement.Right:
						req_rect.X += (int)(req_rect.Width * (1 - 0.4));
						req_rect.Width = (int)(req_rect.Width * 0.4);
						break;
					case DockPlacement.Center:
						req_rect.X = (int)(req_rect.Width * 0.2);
						req_rect.Y = (int)(req_rect.Height * 0.2);
						req_rect.Width = (int)(req_rect.Width * (1 - 0.2)) - req_rect.X;
						req_rect.Height = (int)(req_rect.Height * (1 - 0.2)) - req_rect.Y;
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
			}
			return false;
		}
		
		public override void Docking (DockObject requestor, DockPlacement position, object other_data)
		{
			DockObject parent = ParentObject;
			DockObject newParent = null;
			bool addOurselvesFirst;
			DockObject parentObj = this.ParentObject;
			
			switch (position) {
			case DockPlacement.Top:
			case DockPlacement.Bottom:
				newParent = new DockPaned (Orientation.Vertical);
				addOurselvesFirst = (position == DockPlacement.Bottom);
				break;
			case DockPlacement.Left:
			case DockPlacement.Right:
				newParent = new DockPaned (Orientation.Horizontal);
				addOurselvesFirst = (position == DockPlacement.Right);
				break;
			case DockPlacement.Center:
				newParent = new DockNotebook ();
				addOurselvesFirst = true;
				break;
			default:
				Console.WriteLine ("Unsupported docking strategy");
				return;
			}
			
			if (parent != null)
				parent.Freeze ();

			DockObjectFlags |= DockObjectFlags.InReflow;
			Detach (false);
			newParent.Freeze ();
			newParent.Bind (Master);
			
			if (addOurselvesFirst) {
				newParent.Add (this);
				newParent.Add (requestor);
			} else {
				newParent.Add (requestor);
				newParent.Add (this);
			}
			Console.WriteLine ("Done Adding");
			
			if (parent != null)
				parent.Add (newParent);
			
			if (Visible)
				newParent.Show ();
			
			if (position != DockPlacement.Center && other_data != null && other_data is System.Int32) {
				//PORT THIS:
				//g_object_set (G_OBJECT (newParent), "position", g_value_get_uint (other_data), NULL);
			}
			
			DockObjectFlags &= ~(DockObjectFlags.InReflow);
			newParent.Thaw ();
			
			if (parent != null)
				parent.Thaw ();
		}
		
		private void DetachMenu (Widget item, Menu menu)
		{
			if (item is DockItem)
				((DockItem)item).menu = null;
		}
		
		public void DockPopupMenu (uint button, uint time)
		{
			if (menu == null) {
				menu = new Menu ();
				menu.AttachToWidget (this, new MenuDetachFunc (DetachMenu));
				
				MenuItem mitem = new MenuItem ("Hide");
				mitem.Activated += new EventHandler (ItemHideCb);
				menu.Append (mitem);
			}
			menu.ShowAll ();
			menu.Popup (null, null, null, IntPtr.Zero, button, time);
			
		}
		
		private void ItemHideCb (object o, EventArgs e)
		{
			HideItem ();
		}
		
		public void DockDragStart ()
		{
			Gdk.Cursor fleur = new Gdk.Cursor (Gdk.CursorType.Fleur);
			
			if (!IsRealized)
				Realize ();
			
			DockObjectFlags |= DockObjectFlags.InDrag;
			
			Grab.Add (this);
			
			//PORT THIS:
			//g_signal_emit (item, gdl_dock_item_signals [DOCK_DRAG_BEGIN], 0);
		}
		
		public void DockDragEnd ()
		{
			Grab.Remove (Grab.Current);
			
			//PORT THIS:
			//g_signal_emit (item, gdl_dock_item_signals [DOCK_DRAG_END], 0);
			
			DockObjectFlags &= ~(DockObjectFlags.InDrag);
		}
		
		private void ShowHideGrip ()
		{
			if (grip != null) {
				if (GripShown)
					grip.Show ();
				else
					grip.Hide ();
			}
			QueueResize ();
		}
		
		public void DockTo (DockItem target, DockPlacement position, int docking_param)
		{
			if (target == null || position == DockPlacement.Floating)
				return;
			if (position == DockPlacement.Floating || target == null) {
				if (!IsBound) {
					Console.WriteLine ("Attempting to bind an unbound object");
					return;
				}
				dragoff_x = dragoff_y = 0;
				((Dock)Master.Controller).AddFloatingItem (this, 0, 0, -1, -1);
			} else
				target.Docking (this, position, null);
		}
		
		public virtual void SetOrientation (Orientation orientation)
		{
			if (Orientation != orientation) {
				if (Child != null) {
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
			if (grip_shown) {
				grip_shown = false;
				ShowHideGrip ();
			}
		}
		
		public void ShowGrip ()
		{
			if (!grip_shown) {
				grip_shown = true;
				ShowHideGrip ();
			}
		}
		
		public void Bind (Dock dock)
		{
			if (dock == null)
				return;
			
			Bind (dock.Master);
		}
		
		public void HideItem ()
		{
			if (!IsAttached)
				return;
			
			if (!IsAutomatic) {
				ph = new DockPlaceholder (this, false);
			}
			
			Freeze ();
			if (IsCompound) {
				Foreach (new Callback (HideItem));
			}
			
			Detach (true);
			Thaw ();
		}
		
		public void HideItem (Widget widget)
		{
			if (!(widget is DockItem))
				return;
			DockItem item = widget as DockItem;
			if (!(item.IsAttached))
				return;
			if (!(item.IsAutomatic))
				item.ph = new DockPlaceholder (this, false);
			
			item.Freeze ();
			if (item.IsCompound) {
				item.Foreach (new Callback (HideItem));
			}
			
			item.Detach (true);
			item.Thaw ();
		}
		
		public void IconifyItem ()
		{
			DockObjectFlags |= DockObjectFlags.Iconified;
			HideItem ();
		}
		
		public void ShowItem ()
		{
			DockObjectFlags &= ~(DockObjectFlags.Iconified);
			
			if (ph != null) {
				ph.Add (this);
				ph = null;
			} else if (IsBound) {
				if (Master.Controller != null) {
					Master.Controller.Docking (this, DockPlacement.Floating, null);
				}
			}
		}
		
		public virtual void SetDefaultPosition (DockObject reference)
		{
			ph = null;
			
			if (reference != null && reference.IsAttached) {
				if (reference is DockPlaceholder) {
					ph = (DockPlaceholder)reference;
				} else {
					ph = new DockPlaceholder (reference, true);
				}
			}
		}
	}
}
