// created on 06/06/2004 at 10:09 P
using System;
using Gtk;

namespace Gdl
{
	public delegate void DockItemMotionHandler (DockItem o, int x, int y);
	public delegate void DockItemDragBeginHandler (DockItem o);
	public delegate void DockItemDragEndHandler (DockItem o, bool cancel);
	
	public class DockItem : DockObject
	{		
		private Widget child = null;
		private DockItemBehavior behavior = DockItemBehavior.Normal;
		private Orientation orientation = Orientation.Vertical;
		private bool resize = false;
		private int dragoffX = 0;
		private int dragoffY = 0;
		private Menu menu = null;
		private DockItemGrip grip;
		private uint gripSize;
		private Widget tabLabel = null;
		private int preferredWidth = -1;
		private int preferredHeight = -1;
		private DockPlaceholder ph = null;
		private int startX;
		private int startY;
		
		public event DockItemMotionHandler DockItemMotion;
		public event DockItemDragBeginHandler DockItemDragBegin;
		public event DockItemDragEndHandler DockItemDragEnd;		
				
		static DockItem ()
		{
			Rc.ParseString ("style \"gdl-dock-item-default\" {\n" +
			                    "xthickness = 0\n" +
			                    "ythickness = 0\n" + 
			                    "}\n" + 
			                    "class \"Gdl_DockItem\" " +
			                    "style : gtk \"gdl-dock-item-default\"\n");
		}
		
		protected DockItem ()
		{
			Flags |= (int)WidgetFlags.NoWindow;
		
			if (HasGrip) {
				grip = new DockItemGrip (this);
				grip.Parent = this;
				grip.Show ();
			}
		}
		
		public DockItem (string name, string longName, DockItemBehavior behavior) : this ()
		{
			Name = name;
			LongName = longName;
			Behavior = behavior;			
		}
		
		public DockItem (string name, string longName, string stockid,
				 DockItemBehavior behavior) : this (name, longName, behavior)
		{
			StockId = stockid;
		}
		
		public DockItemBehavior Behavior {
			get {
				return behavior;
			}
			set {
				DockItemBehavior oldBehavior = behavior;
				behavior = value;
				if (((oldBehavior ^ behavior) & DockItemBehavior.Locked) != 0) {
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
		
		public bool CantClose {
			get {
				return ((Behavior & DockItemBehavior.CantClose) != 0);
			}
		}
		
		public bool CantIconify {
			get {
				return ((Behavior & DockItemBehavior.CantIconify) != 0);
			}
		}
		
		public new Widget Child {
			get {
				return child;
			}
			set {
				child = value;
			}
		}
		
		public int DragOffX {
			get {
				return dragoffX;
			}
			set {
				dragoffX = value; 
			}
		}
		
		public int DragOffY {
			get {
				return dragoffY;
			}
			set {
				dragoffY = value;
			}
		}
		
		public bool GripShown {
			get {
				return (HasGrip && !Locked && grip.Visible);
			}
		}
		
		public virtual bool HasGrip {
			get {
				return true;
			}
		}
		
		public bool Iconified {
			get {
				return ((DockObjectFlags & DockObjectFlags.Iconified) != 0);
			}
		}
		
		public bool InDrag {
			get {
				return ((DockObjectFlags & DockObjectFlags.InDrag) != 0);
			}
		}
		
		public bool InPreDrag {
			get {
				return ((DockObjectFlags & DockObjectFlags.InPreDrag) != 0);
			}
		}
		
		public override bool IsCompound {
			get {
				return false;
			}
		}
		
		public bool Locked {
			get {
				return ((behavior & DockItemBehavior.Locked) != 0);
			}
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
		
		public Orientation Orientation {
			get {
				return orientation;
			}
			set {
				SetOrientation (value);
			}
		}
		
		public int PreferredHeight {
			get {
				return preferredHeight;
			}
			set {
				preferredHeight = value;
			}
		}
		
		public int PreferredWidth {
			get {
				return preferredWidth;
			}
			set {
				preferredWidth = value;
			}
		}
		
		public bool Resize {
			get {
				return resize;
			}
			set {
				resize = value;
				QueueResize ();
			}
		}
		
		public Widget TabLabel {
			get {
				return tabLabel;
			}
			set {
				tabLabel = value;
			}
		}
		
		public bool UserAction {
			get {
				return ((DockObjectFlags & DockObjectFlags.UserAction) != 0);
			}
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
			bool wasVisible = widget.Visible;

			if (grip == widget) {
				widget.Unparent ();
				grip = null;
				if (wasVisible)
					QueueResize ();
				return;
			} else if (widget != Child) {
				return;
			}

			if (InDrag)
				DockDragEnd (true);
			
			widget.Unparent ();
			Child = null;
			
			if (wasVisible)
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
		
		protected override void OnStyleSet (Style style)
		{
			if (IsRealized && !IsNoWindow) {
				Style.SetBackground (GdkWindow, State);
				if (IsDrawable)
					GdkWindow.Clear ();
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			if (IsDrawable && evnt.Window == GdkWindow) {
				Style.PaintBox (Style, GdkWindow, State,
						ShadowType.None, evnt.Area, this,
						"dockitem", 0, 0, -1, -1);
				base.OnExposeEvent (evnt);
			}

			return false;
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			if (!EventInGripWindow (evnt) || Locked)
				return false;
			
			bool eventHandled = false;
			bool inHandle;
			Gdk.Cursor cursor = null;
			
			/* Check if user clicked on the drag handle. */      
			switch (Orientation) {
			case Orientation.Horizontal:
				inHandle = evnt.X < grip.Allocation.Width;
				break;
			case Orientation.Vertical:
				inHandle = evnt.Y < grip.Allocation.Height;
				break;
			default:
				inHandle = false;
				break;
			}
			
			/* Left mousebutton click on dockitem. */
			if (evnt.Button == 1 && evnt.Type == Gdk.EventType.ButtonPress) {
				/* Set in_drag flag, grab pointer and call begin drag operation. */
				if (inHandle) {
					startX = (int)evnt.X;
					startY = (int)evnt.Y;
					DockObjectFlags |= DockObjectFlags.InPreDrag;
					cursor = new Gdk.Cursor (Display, Gdk.CursorType.Fleur);
					grip.TitleWindow.Cursor = cursor;
					eventHandled = true;
				}
			} else if (evnt.Type == Gdk.EventType.ButtonRelease && evnt.Button == 1) {
				if (InDrag) {
					/* User dropped widget somewhere. */
					DockDragEnd (false);
					eventHandled = true;
				} else if (InPreDrag) {
					DockObjectFlags &= ~(DockObjectFlags.InPreDrag);
					eventHandled = true;
				}
				
				/* we check the window since if the item was redocked it's
				   been unrealized and maybe it's not realized again yet */
				if (grip.TitleWindow != null) {
					cursor = new Gdk.Cursor (Display, Gdk.CursorType.Hand2);
					grip.TitleWindow.Cursor = cursor;
				}
			} else if (evnt.Button == 3 && evnt.Type == Gdk.EventType.ButtonPress && inHandle) {
				DockPopupMenu (evnt.Button, evnt.Time);
				eventHandled = true;
			}

			return eventHandled;
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
				if (Drag.CheckThreshold (this, startX, startY,
							 (int)evnt.X, (int)evnt.Y)) {
					DockObjectFlags &= ~(DockObjectFlags.InPreDrag);
					dragoffX = startX;
					dragoffY = startY;
					DockDragStart ();
				}
			}
			
			if (!InDrag)
				return false;
			
			int newX = (int)evnt.XRoot;
			int newY = (int)evnt.YRoot;
			OnMotion (newX, newY);
			
			return true;
		}
		
		protected void OnMotion (int x, int y)
		{
			if (DockItemMotion != null)
				DockItemMotion (this, x, y);
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if (InDrag && evnt.Key == Gdk.Key.Escape) {
				DockDragEnd (false);
				return true;
			}
			
			return base.OnKeyPressEvent (evnt);
		}
		
		public override bool OnDockRequest (int x, int y, DockRequest request)
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
		
		public override void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
			DockObject parent = ParentObject;
			DockObject newParent = null;
			bool addOurselvesFirst;
			
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
			
			if (parent != null)
				parent.Add (newParent);
			
			if (Visible)
				newParent.Show ();
			
			if (position != DockPlacement.Center && data != null &&
			    data is System.Int32) {
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
			
			OnDragBegin ();
		}
		
		protected void OnDragBegin ()
		{
			if (DockItemDragBegin != null)
				DockItemDragBegin (this);
		}
		
		public void DockDragEnd (bool cancel)
		{
			Grab.Remove (Grab.Current);
			
			OnDragEnd (cancel);
			
			DockObjectFlags &= ~(DockObjectFlags.InDrag);
		}
		
		
		protected void OnDragEnd (bool cancel)
		{
			if (DockItemDragEnd != null)
				DockItemDragEnd (this, cancel);
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
		
		public void DockTo (DockItem target, DockPlacement position)
		{
			if (target == null && position != DockPlacement.Floating)
				return;

			if (position == DockPlacement.Floating || target != null) {
				if (!IsBound) {
					Console.WriteLine ("Attempting to bind an unbound item");
					return;
				}
				
				dragoffX = dragoffY = 0;
				((Dock)Master.Controller).AddFloatingItem (this, 0, 0, -1, -1);
			} else {
				target.Dock (this, position, null);
			}
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
			if (GripShown)
				ShowHideGrip ();
		}
		
		public void ShowGrip ()
		{
			if (!GripShown)
				ShowHideGrip ();
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
				/* already hidden/detached */
				return;
			
			/* if the object is manual, create a new placeholder to be
			   able to restore the position later */
			if (!IsAutomatic)
				ph = new DockPlaceholder (this, false);
			
			Freeze ();

			if (IsCompound)
				Foreach (new Callback (HideItem));
			
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
					Master.Controller.Dock (this, DockPlacement.Floating, null);
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

		public static Requisition PreferredSize (DockItem item)
		{
			Requisition req;
			req.Width = Math.Max (item.preferredWidth, item.Allocation.Width);
			req.Height = Math.Max (item.preferredHeight, item.Allocation.Height);
			return req;
		}
		
		private bool EventInGripWindow (Gdk.Event evnt)
		{
			if (grip != null && grip.TitleWindow == evnt.Window)
				return true;
			else
				return false;
		}
	}
}
