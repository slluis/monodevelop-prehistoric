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
		private bool autoTitle;
		private int float_x;
		private int float_y;
		private int width = -1;
		private int height = -1;
		private Gdk.GC xorGC;
		private string title;

		protected Dock (IntPtr raw) : base (raw) { }

		public Dock () : this (null, false)
		{
		}

		public Dock (Dock original, bool _float) : this (original, _float, 0, 0, -1, -1)
		{
		}

		public Dock (Dock original, bool _float, int x, int y, int _width, int _height)
		{
			float_x = x;
			float_y = y;
			width = _width;
			height = _height;
			SetFlag (WidgetFlags.NoWindow);
			if (original != null) {
				Bind (original.Master);
			}
			this.floating = _float;
			if (Master == null) {
				DockObjectFlags &= ~(DockObjectFlags.Automatic);
				Bind (new DockMaster ());
			}
			if (floating) {
				window = new Window (WindowType.Toplevel);
				((Gtk.Window)window).WindowPosition = WindowPosition.Mouse;
				((Gtk.Window)window).SetDefaultSize (width, height);
				((Gtk.Window)window).TypeHint = Gdk.WindowTypeHint.Normal;
				((Gtk.Window)window).Move (float_x, float_y);
				window.ConfigureEvent += new ConfigureEventHandler (floatingConfigure);
				SetWindowTitle ();
				//TODO: connect to long name notify
				DockObject controller = Master.Controller;
				if (controller != null && controller is Dock) {
					if (!((Dock)controller).Floating) {
						if (controller.Toplevel != null && controller.Toplevel is Gtk.Window) {
							((Gtk.Window)window).TransientFor = (Gtk.Window)controller.Toplevel;
						}
					}
				}
				((Gtk.Window)window).Add (this);
			}
			DockObjectFlags |= DockObjectFlags.Attached;
		}
		
		public bool Floating {
			get {
				return floating;
			}
			set {
				floating = value;
			}
		}
		
		public int FloatX {
			get {
				return float_x;
			}
			set {
				float_x = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		public int FloatY {
			get {
				return float_y;
			}
			set {
				float_y = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		public int Height {
			get {
				return height;
			}
			set {
				height = value;
				if (floating && window != null && window is Window)
					((Window)window).Resize (width, height);
			}
		}
		
		private bool IsController {
			get {
				if (Master == null)
					return false;
				return Master.Controller == this;
			}
		}

		public ICollection NamedItems {
			get {
				return Master.DockObjects;
			}
		}
		
		public DockObject Root {
			get {
				return root;
			}
			set {
				root = value;
			}
		}
		
		public string Title {
			get {
				return title;
			}
			set {
				title = value;
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

			if (root != null && root.Visible && !root.IsMapped)
				root.Map ();
		}
		
		protected override void OnUnmapped ()
		{
			base.OnUnmapped ();

			if (root != null && root.Visible && root.IsMapped)
				root.Unmap ();

			if (window != null)
				window.Unmap ();
		}
		
		protected override void OnShown ()
		{
			base.OnShown ();

			if (floating && window != null)
				window.ShowAll ();

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

			if (IsController) {
				foreach (DockObject item in Master.TopLevelDocks) {
					if (item == this)
						continue;
					if (item.IsAutomatic)
						item.Hide ();
				}
			}
		}
		
		protected override void OnAdded (Widget widget)
		{
			DockItem child = widget as DockItem;
			AddItem (child, DockPlacement.Top);
		}
		
		protected override void OnRemoved (Widget widget)
		{
			bool wasVisible = widget.Visible;

			if (root == widget) {
				root.DockObjectFlags &= ~(DockObjectFlags.Attached);
				root = null;
				widget.Unparent ();

				if (wasVisible && Visible)
					QueueResize ();
			}
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (root != null)
				invoker.Invoke (root);
		}
		
		public override void OnDetached (bool recursive)
		{
			if (recursive && root != null)
				root.Detach (recursive);

			DockObjectFlags &= ~(DockObjectFlags.Attached);
		}
		
		public override void OnReduce ()
		{
			if (root != null)
				return;
			
			if (IsAutomatic) {
				Destroy ();
			} else if (!IsAttached) {
				if (floating)
					Hide ();
				else if (Parent != null && Parent is Container)
					((Container)Parent).Remove (this);
			}
		}
		
		public override bool OnDockRequest (int x, int y, DockRequest request)
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
						may_dock = root.OnDockRequest (x, y, my_request);
					}
				}
			}
			
			if (may_dock && request != null)
				request = my_request;
			return may_dock;
		}
		
		public override void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
			/* only dock items allowed at this time */
			if (!(requestor is DockItem))
				return;

			if (position == DockPlacement.Floating) {
				Console.WriteLine ("Adding a floating dockitem");
				DockItem item = requestor as DockItem;
				int x = 0, y = 0, width = -1, height = -1;
				if (data != null && data is Gdk.Rectangle) {
					Gdk.Rectangle rect = (Gdk.Rectangle)data;
					x = rect.X;
					y = rect.Y;
					width = rect.Width;
					height = rect.Height;
				}
				AddFloatingItem (item, x, y, width, height);
			} else if (root != null) {
				/* This is somewhat a special case since we know
				   which item to pass the request on because we only
				   have one child */
				root.Dock (requestor, position, null);
				SetWindowTitle ();
			} else { /* Item about to be added is root item. */
				root = requestor;
				root.DockObjectFlags |= DockObjectFlags.Attached;
				root.Parent = this;
				((DockItem)root).ShowGrip ();
				
				/* Realize the item (create its corresponding GdkWindow)
			           when the Dock has been realized. */
				if (IsRealized)
					root.Realize ();
				
				/* Map the widget if it's visible and the parent is
			           visible and has been mapped. This is done to make
			           sure that the GdkWindow is visible. */
				if (Visible && root.Visible) {
					if (IsMapped)
						root.Map ();
					
					/* Make the widget resize. */
					root.QueueResize ();
				}
				
				SetWindowTitle ();
			}
		}
		
		public override bool OnReorder (DockObject requestor, DockPlacement position, object data)
		{
			if (Floating && position == DockPlacement.Floating && root == requestor) {
				if (window != null && window is Window &&
				    data != null && data is Gdk.Rectangle) {
					Gdk.Rectangle rect = (Gdk.Rectangle)data;
					((Window)window).Move (rect.X, rect.Y);
					return true;
				}
			}

			return false;
		}
		
		public override bool OnChildPlacement (DockObject child, ref DockPlacement placement)
		{
			if (root == child) {
				if (placement == DockPlacement.None ||
				    placement == DockPlacement.Floating)
					placement = DockPlacement.Top;
				return true;
			}
				
			return false;
		}
		
		public override void OnPresent (DockObject child)
		{
			if (Floating && window != null && window is Window)
				((Window)window).Present ();
		}
		
		public void AddItem (DockItem item, DockPlacement placement)
		{
			if (item == null)
				return;

			if (placement == DockPlacement.Floating)
				AddFloatingItem (item, 0, 0, -1, -1);
			else
				Dock (item, placement, null);
		}
		
		public void AddFloatingItem (DockItem item, int x, int y, int width, int height)
		{
			if (this.Master == null) {
				Console.WriteLine ("something is seriously fucked here");
			}
			Gdl.Dock dock = new Dock (this, true, x, y, width, height);
			if (Visible) {
				dock.Show ();
				if (IsMapped)
					dock.Map ();
				dock.QueueResize ();
			}
			
			dock.AddItem (item, DockPlacement.Top);
		}
		
		public DockItem GetItemByName (string name)
		{
			if (name == null)
				return null;

			DockObject found = Master.GetObject (name);
			if (found != null && found is DockItem)
				return (DockItem)found;
			else
				return null;
		}
		
		public DockPlaceholder GetPlaceholderByName (string name)
		{
			if (name == null)
				return null;

			DockObject found = Master.GetObject (name);
			if (found != null && found is DockPlaceholder)
				return (DockPlaceholder)found;
			else
				return null;
		}
		
		public static Dock GetTopLevel (DockObject obj)
		{
			DockObject parent = obj;
			while (parent != null && !(parent is Gdl.Dock))
				parent = parent.ParentObject;

			return parent != null ? (Dock)parent : null;
		}
		
		public void XorRect (Gdk.Rectangle rect)
		{
			if (xorGC == null) {
				if (IsRealized) {
					Gdk.GCValues values = new Gdk.GCValues ();
					values.Function = Gdk.Function.Invert;
					values.SubwindowMode = Gdk.SubwindowMode.IncludeInferiors;
					xorGC = new Gdk.GC (GdkWindow);
					xorGC.SetValues (values, Gdk.GCValuesMask.Function |
							 Gdk.GCValuesMask.Subwindow);
				} else {
					return;
				}
			}

			xorGC.SetLineAttributes (1, Gdk.LineStyle.OnOffDash,
						 Gdk.CapStyle.NotLast,
						 Gdk.JoinStyle.Bevel);
			xorGC.SetDashes (1, new sbyte[] { 1, 1}, 2);
			
			GdkWindow.DrawRectangle (xorGC, false, rect.X, rect.Y,
						 rect.Width, rect.Height);

			xorGC.SetDashes (0, new sbyte[] { 1, 1}, 2);

			GdkWindow.DrawRectangle (xorGC, false, rect.X + 1,
						 rect.Y + 1, rect.Width - 2,
						 rect.Height - 2);
		}
		
		private void SetWindowTitle ()
		{
			if (window == null)
				return;
		
			if (!autoTitle && LongName != null)
				title = LongName;
			else if (Master != null)
				title = Master.DefaultTitle;
			
			if (title == null && root != null)
				title = root.LongName;
			
			if (title == null) {
				autoTitle = true;
				title = "Dock " + Master.DockNumber++;
				LongName = title;
			}
			
			((Window)window).Title = title;
		}

		[GLib.ConnectBefore]
		private void floatingConfigure (object o, ConfigureEventArgs e)
		{
			float_x = e.Event.X;
			float_y = e.Event.Y;
			width = e.Event.Width;
			height = e.Event.Height;
			e.RetVal = false;
		}
	}
}
