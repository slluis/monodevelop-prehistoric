// created on 07/06/2004 at 2:52 P

using System;
using Gtk;

//FIXME: Hook up the event notifyin stuff here.

namespace Gdl
{
	public class DockItemGrip : Container
	{
		private DockItem item;
		private Gdk.Window title_window;
		private Button close_button;
		private Button iconify_button;
		private Tooltips tooltips;
		private bool icon_pixbuf_valid = false;
		private Gdk.Pixbuf icon_pixbuf = null;
		private string title;
		private Pango.Layout title_layout = null;

		protected DockItemGrip (IntPtr raw) : base (raw) { }
		
		public DockItemGrip ()
		{
			Flags |= (int)WidgetFlags.NoWindow;
			
			Widget.PushCompositeChild ();
			close_button = new Button ();
			Widget.PopCompositeChild ();
			
			close_button.Flags &= ~(int)WidgetFlags.CanFocus;
			close_button.Parent = this;
			close_button.Relief = ReliefStyle.None;
			close_button.Show ();
			
			Image image = new Image (Gdl.Stock.Close, IconSize.Menu);
			close_button.Add (image);
			image.Show ();
			
			close_button.Clicked += new EventHandler (CloseClicked);
			
			Widget.PushCompositeChild ();
			iconify_button = new Button ();
			Widget.PopCompositeChild ();
			
			iconify_button.Flags &= ~(int)(WidgetFlags.CanFocus);
			iconify_button.Parent = this;
			iconify_button.Relief = ReliefStyle.None;
			iconify_button.Show ();
			
			image = new Image (Gdl.Stock.MenuLeft, IconSize.Menu);
			iconify_button.Add (image);
			image.Show ();
			
			iconify_button.Clicked += new EventHandler (IconifyClicked);
			
			tooltips = new Tooltips ();
			tooltips.SetTip (iconify_button, "Iconify", "Iconify this dock");
			tooltips.SetTip (close_button, "Close", "Close this dock");
		}
		
		public DockItemGrip (DockItem item) : this ()
		{
			Item = item;
		}
		
		public DockItem Item {
			get { return item; }
			set {
				//hookup notify stuff here
				item = value;
				if (!(item.CantClose) && close_button != null)
					close_button.Show ();
				if (!(item.CantIconify) && iconify_button != null)
					iconify_button.Show ();
			}
		}
		
		public Gdk.Window TitleWindow {
			get { return title_window; }
			set { title_window = value; }
		}
		
		public Gdk.Rectangle GetTitleArea ()
		{
			Gdk.Rectangle area;
			int border = (int)BorderWidth;
			int alloc_height, alloc_width;
			
			area.Width = (Allocation.Width - 2 * border);
			
			title_layout.GetPixelSize (out alloc_width, out alloc_height);
			
			if (close_button.Visible) {
				alloc_height = Math.Max (alloc_height, close_button.Allocation.Height);
				area.Width -= close_button.Allocation.Width;
			}
			if (iconify_button.Visible) {
				alloc_height = Math.Max (alloc_height, iconify_button.Allocation.Height);
				area.Width -= close_button.Allocation.Width;
			}
			
			area.X = Allocation.X + border;
			area.Y = Allocation.Y + border;
			area.Height = alloc_height;
			
			if (Direction == TextDirection.Rtl)
				area.X += (Allocation.Width - 2 * border) - area.Width;
				
			return area;
		}
		
		private void EnsureTitleAndIconPixbuf ()
		{
			if (title == null) {
				title = item.LongName;
				if (title == null)
					title = "";
			}
			
			if (!icon_pixbuf_valid) {
				if (item.StockId != null) {
					icon_pixbuf = RenderIcon (item.StockId, IconSize.Menu, "");
				}
				icon_pixbuf_valid = true;
			}
			
			if (title_layout == null) {
				title_layout = CreatePangoLayout (title);
				title_layout.SingleParagraphMode = true;
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Gdk.Rectangle title_area = GetTitleArea ();
			Gdk.Rectangle expose_area;
			
			if (icon_pixbuf != null) {
				Gdk.Rectangle pixbuf_rect;
				pixbuf_rect.Width = icon_pixbuf.Width;
				pixbuf_rect.Height = icon_pixbuf.Height;
				
				if (Direction == TextDirection.Rtl) {
					pixbuf_rect.X = title_area.X + title_area.Width - pixbuf_rect.Width;
				} else {
					pixbuf_rect.X = title_area.X;
					title_area.X += pixbuf_rect.Width + 1;
				}
				
				title_area.Width -= pixbuf_rect.Width - 1;
				pixbuf_rect.Y = title_area.Y + (title_area.Height - pixbuf_rect.Height) / 2;
				if (evnt.Area.Intersect (pixbuf_rect, out expose_area)) {
					Gdk.GC gc = Style.BackgroundGC (State);
					GdkWindow.DrawPixbuf (gc, icon_pixbuf, 0, 0, pixbuf_rect.X, pixbuf_rect.Y, pixbuf_rect.Width, pixbuf_rect.Height, Gdk.RgbDither.None, 0, 0);
				}
			}

			if (title_area.Intersect (evnt.Area, out expose_area)) {
				int layout_width, layout_height, text_x, text_y;
				title_layout.GetPixelSize (out layout_width, out layout_height);
				if (Direction == TextDirection.Rtl)
					text_x = title_area.X + title_area.Width - layout_width;
				else
					text_x = title_area.X;
				text_y = title_area.Y + (title_area.Height - layout_height) / 2;
				Style.PaintLayout (Style, GdkWindow, State, true, expose_area, this, null, text_x, text_y, title_layout);
			}
			
			return base.OnExposeEvent (evnt);
		}
		
		private void CloseClicked (object o, EventArgs e)
		{
			item.HideItem ();
		}
		
		private void IconifyClicked (object o, EventArgs e)
		{
			item.IconifyItem ();
			iconify_button.InButton = false;
			iconify_button.Leave ();
		}
		
		protected override void OnRealized ()
		{
			base.OnRealized ();
			if (title_window == null) {
				Gdk.WindowAttr attributes = new Gdk.WindowAttr ();
				
				EnsureTitleAndIconPixbuf ();
				
				Gdk.Rectangle area = GetTitleArea ();
				
				attributes.X = area.X;
				attributes.Y = area.Y;
				attributes.Width = area.Width;
				attributes.Height = area.Height;
				attributes.WindowType = Gdk.WindowType.Temp;
				attributes.Wclass = Gdk.WindowClass.InputOnly;
				attributes.OverrideRedirect = true;
				attributes.EventMask = (int) (Events | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonMotionMask);
				title_window = new Gdk.Window (ParentWindow, attributes, (int) (Gdk.WindowAttributesType.X | Gdk.WindowAttributesType.Y | Gdk.WindowAttributesType.Noredir));
				title_window.UserData = Handle;
				title_window.Cursor = new Gdk.Cursor (Display, Gdk.CursorType.Hand2);
			}
		}
		
		protected override void OnUnrealized ()
		{
			if (title_window != null) {
				title_window.UserData = IntPtr.Zero;
				title_window.Destroy ();
				title_window = null;
			}
			base.OnUnrealized ();
		}
		
		protected override void OnMapped ()
		{
			base.OnMapped ();
			if (title_window != null) {
				title_window.Show ();
			}
		}
		
		protected override void OnUnmapped ()
		{
			if (title_window != null) {
				title_window.Hide ();
			}
			base.OnUnmapped ();
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			requisition.Width = (int)BorderWidth * 2;
			requisition.Height = (int)BorderWidth * 2;

			EnsureTitleAndIconPixbuf ();
			
			if (close_button.Visible) {
				Requisition childReq = close_button.SizeRequest ();
				requisition.Width += childReq.Width;
				requisition.Height = Math.Max (requisition.Height,
							       childReq.Height);
			}
			
			if (iconify_button.Visible) {
				Requisition childReq = iconify_button.SizeRequest ();
				requisition.Width += childReq.Width;
				requisition.Height = Math.Max (requisition.Height,
							       childReq.Height);
			}
			
			if (icon_pixbuf != null) {
				requisition.Width += icon_pixbuf.Width + 1;
				requisition.Height = Math.Max (requisition.Height,
							       icon_pixbuf.Height);
			}
		}
		
		private void EllipsizeLayout (int width)
		{
			if (width <= 0) {
				title_layout.SetText ("");
				return;
			}
			
			int w, h, ell_w, ell_h, x, empty;
			title_layout.GetPixelSize (out w, out h);
			if (w <= width) return;
			
			Pango.Layout ell = title_layout.Copy ();
			ell.SetText ("...");
			ell.GetPixelSize (out ell_w, out ell_h);
			if (width < ell_w) {
				title_layout.SetText ("");
				return;
			}
			
			width -= ell_w;
			Pango.LayoutLine line = title_layout.GetLine (0);
			string text = title_layout.Text;
			if (line.XToIndex (width * 1024, out x, out empty)) {
				title_layout.SetText (text.Substring (0, x) + "...");
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			Gdk.Rectangle child_allocation;
			
			if (Direction == TextDirection.Rtl)
				child_allocation.X = allocation.X + (int)BorderWidth;
			else
				child_allocation.X = allocation.X + allocation.Width - (int)BorderWidth;
			child_allocation.Y = allocation.Y + (int)BorderWidth;
			
			if (close_button.Visible) {
				Requisition button_requisition = close_button.SizeRequest ();
				if (Direction != TextDirection.Rtl) 
					child_allocation.X -= button_requisition.Width;
				
				child_allocation.Width = button_requisition.Width;
				child_allocation.Height = button_requisition.Height;
				
				close_button.SizeAllocate (child_allocation);
				
				if (Direction == TextDirection.Rtl)
					child_allocation.X += button_requisition.Width;
			}
			
			if (iconify_button.Visible) {
				Requisition button_requisition = iconify_button.SizeRequest ();
				if (Direction != TextDirection.Rtl)
					child_allocation.X -= button_requisition.Width;
				
				child_allocation.Width = button_requisition.Width;
				child_allocation.Height = button_requisition.Height;
				
				iconify_button.SizeAllocate (child_allocation);
				
				if (Direction == TextDirection.Rtl)
					child_allocation.X += button_requisition.Width;
			}
			
			if (title_window != null) {
				EnsureTitleAndIconPixbuf ();
				title_layout.SetText (title);
				Gdk.Rectangle area = GetTitleArea ();
				title_window.MoveResize (area.X, area.Y, area.Width, area.Height);
				if (icon_pixbuf != null) {
					area.Width -= icon_pixbuf.Width + 1;
				}
				EllipsizeLayout (area.Width);
			}
		}
		
		protected override void OnAdded (Widget widget)
		{
			Console.WriteLine ("You can't add a widget to DockItemGrip directly");
		}
		
		protected override void OnRemoved (Widget widget)
		{
			Console.WriteLine ("You can't remove a widget from DockItemGrip directly");
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals) {
				invoker.Invoke (close_button);
				invoker.Invoke (iconify_button);
			}
		}
	}
}
