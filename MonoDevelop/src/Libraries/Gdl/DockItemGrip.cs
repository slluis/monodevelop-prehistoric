// created on 07/06/2004 at 2:52 P

using System;
using Gtk;

//FIXME: Hook up the event notifyin stuff here.

namespace Gdl
{
	public class DockItemGrip : Gtk.Container
	{
		private DockItem item;
		private Gdk.Window title_window;
		private Gtk.Button close_button;
		private Gtk.Button iconify_button;
		private Gtk.Tooltips tooltips;
		private bool icon_pixbuf_valid = false;
		private Gdk.Pixbuf icon_pixbuf = null;
		private string title;
		private Pango.Layout title_layout = null;
		
		public DockItemGrip ()
		{
			this.Flags |= (int)Gtk.WidgetFlags.NoWindow;
		
			Widget.PushCompositeChild ();
			this.close_button = new Gtk.Button ();
			Widget.PopCompositeChild ();
			
			this.close_button.Flags |= (int)Gtk.WidgetFlags.CanFocus;
			this.close_button.Parent = this;
			this.close_button.Relief = Gtk.ReliefStyle.None;
			this.close_button.Show ();
			
			Gtk.Image image = new Gtk.Image (Gdl.Stock.Close, Gtk.IconSize.Menu);
			this.close_button.Add (image);
			image.Show ();
			
			this.close_button.Clicked += new EventHandler (CloseClicked);
			
			Widget.PushCompositeChild ();
			this.iconify_button = new Gtk.Button ();
			Widget.PopCompositeChild ();
			
			this.iconify_button.Flags |= (int)Gtk.WidgetFlags.CanFocus;
			this.iconify_button.Parent = this;
			this.iconify_button.Relief = Gtk.ReliefStyle.None;
			this.iconify_button.Show ();
			
			image = new Gtk.Image (Gdl.Stock.MenuLeft, Gtk.IconSize.Menu);
			this.iconify_button.Add (image);
			image.Show ();
			
			this.iconify_button.Clicked += new EventHandler (IconifyClicked);
			
			this.tooltips = new Gtk.Tooltips ();
			this.tooltips.SetTip (this.iconify_button, "Iconify", "Iconify this dock");
			this.tooltips.SetTip (this.close_button, "Close", "Close this dock");
		}
		
		public DockItemGrip (DockItem item) : this ()
		{
			this.Item = item;
		}
		
		public DockItem Item {
			get { return item; }
			set {
				//hookup notify stuff here
				item = value;
				if (!(item.CantClose) && this.close_button != null)
					this.close_button.Show ();
				if (!(item.CantIconify) && this.iconify_button != null)
					this.iconify_button.Show ();
			}
		}
		
		public Gdk.Window TitleWindow {
			get { return title_window; }
			set { title_window = value; }
		}
		
		public Gdk.Rectangle GetTitleArea ()
		{
			Gdk.Rectangle area;
			int border = (int)this.BorderWidth;
			int alloc_height, alloc_width;
			
			area.Width = (this.Allocation.Width - 2 * border);
			
			title_layout.GetPixelSize (out alloc_width, out alloc_height);
			
			if (this.close_button.Visible) {
				alloc_height = Math.Max (alloc_height, this.close_button.Allocation.Height);
				area.Width -= this.close_button.Allocation.Width;
			}
			if (this.iconify_button.Visible) {
				alloc_height = Math.Max (alloc_height, this.iconify_button.Allocation.Height);
				area.Width -= this.close_button.Allocation.Width;
			}
			
			area.X = this.Allocation.X + border;
			area.Y = this.Allocation.Y + border;
			area.Height = alloc_height;
			
			if (this.Direction == Gtk.TextDirection.Rtl)
				area.X += (this.Allocation.Width - 2 * border) - area.Width;
				
			return area;
		}
		
		private void EnsureTitleAndIconPixbuf ()
		{
			if (this.title == null) {
				this.title = this.item.LongName;
				if (this.title == null)
					this.title = "";
			}
			
			if (!this.icon_pixbuf_valid) {
				if (this.item.StockId != null) {
					this.icon_pixbuf = this.RenderIcon (this.item.StockId, Gtk.IconSize.Menu, "");
				}
				this.icon_pixbuf_valid = true;
			}
			
			if (this.title_layout == null) {
				this.title_layout = this.CreatePangoLayout (this.title);
				this.title_layout.SingleParagraphMode = true;
			}
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Gdk.Rectangle title_area = this.GetTitleArea ();
			Gdk.Rectangle expose_area;
			
			if (this.icon_pixbuf != null) {
				Gdk.Rectangle pixbuf_rect;
				pixbuf_rect.Width = this.icon_pixbuf.Width;
				pixbuf_rect.Height = this.icon_pixbuf.Height;
				
				if (this.Direction == Gtk.TextDirection.Rtl) {
					pixbuf_rect.X = title_area.X + title_area.Width - pixbuf_rect.Width;
				} else {
					pixbuf_rect.X = title_area.X;
					title_area.X += pixbuf_rect.Width + 1;
				}
				
				title_area.Width -= pixbuf_rect.Width - 1;
				pixbuf_rect.Y = title_area.Y + (title_area.Height - pixbuf_rect.Height) / 2;
				if (evnt.Area.Intersect (pixbuf_rect, out expose_area)) {
					Gdk.GC gc = this.Style.BackgroundGC (this.State);
					this.GdkWindow.DrawPixbuf (gc, this.icon_pixbuf, 0, 0, pixbuf_rect.X, pixbuf_rect.Y, pixbuf_rect.Width, pixbuf_rect.Height, Gdk.RgbDither.None, 0, 0);
				}
			}

			if (title_area.Intersect (evnt.Area, out expose_area)) {
				int layout_width, layout_height, text_x, text_y;
				this.title_layout.GetPixelSize (out layout_width, out layout_height);
				if (this.Direction == Gtk.TextDirection.Rtl)
					text_x = title_area.X + title_area.Width - layout_width;
				else
					text_x = title_area.X;
				text_y = title_area.Y + (title_area.Height - layout_height) / 2;
				Gtk.Style.PaintLayout (this.Style, this.GdkWindow, this.State, true, expose_area, this, null, text_x, text_y, this.title_layout);
			}
			
			return base.OnExposeEvent (evnt);
		}
		
		private void CloseClicked (object o, EventArgs e)
		{
			this.item.HideItem ();
		}
		
		private void IconifyClicked (object o, EventArgs e)
		{
			this.item.IconifyItem ();
			this.iconify_button.InButton = false;
			this.iconify_button.Leave ();
		}
		
		protected override void OnRealized ()
		{
			base.OnRealized ();
			if (this.title_window == null) {
				Gdk.WindowAttr attributes = new Gdk.WindowAttr ();
				
				this.EnsureTitleAndIconPixbuf ();
				
				Gdk.Rectangle area = this.GetTitleArea ();
				
				attributes.X = area.X;
				attributes.Y = area.Y;
				attributes.Width = area.Width;
				attributes.Height = area.Height;
				attributes.WindowType = Gdk.WindowType.Temp;
				attributes.Wclass = Gdk.WindowClass.InputOnly;
				attributes.OverrideRedirect = true;
				attributes.EventMask = (int) (this.Events | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonMotionMask);
				this.title_window = new Gdk.Window (this.ParentWindow, attributes, (int) (Gdk.WindowAttributesType.X | Gdk.WindowAttributesType.Y | Gdk.WindowAttributesType.Noredir));
				this.title_window.UserData = this.Handle;
				this.title_window.Cursor = new Gdk.Cursor (this.Display, Gdk.CursorType.Hand2);
			}
		}
		
		protected override void OnUnrealized ()
		{
			if (this.title_window != null) {
				this.title_window.UserData = IntPtr.Zero;
				this.title_window.Destroy ();
				this.title_window = null;
			}
			base.OnUnrealized ();
		}
		
		protected override void OnMapped ()
		{
			base.OnMapped ();
			Console.WriteLine ("Mapping the grip");
			if (this.title_window != null) {
				this.title_window.Show ();
			}
		}
		
		protected override void OnUnmapped ()
		{
			if (this.title_window != null) {
				this.title_window.Hide ();
			}
			base.OnUnmapped ();
		}
		
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			requisition.Width = (int)this.BorderWidth * 2;
			requisition.Height = (int)this.BorderWidth * 2;
			
			this.EnsureTitleAndIconPixbuf ();
			
			int layout_height, layout_width;
			this.title_layout.GetPixelSize (out layout_height, out layout_width);
			
			if (this.close_button.Visible) {
				Gtk.Requisition child_req = this.close_button.SizeRequest ();
				requisition.Width += child_req.Width;
				layout_height = Math.Max (layout_height, child_req.Height);
			}
			if (this.iconify_button.Visible) {
				Gtk.Requisition child_req = this.iconify_button.SizeRequest ();
				requisition.Width += child_req.Width;
				layout_height = Math.Max (layout_height, child_req.Height);
			}
			requisition.Height += layout_height;
			if (this.icon_pixbuf != null) {
				requisition.Width += this.icon_pixbuf.Width + 1;
			}
		}
		
		private void EllipsizeLayout (int width)
		{
			if (width <= 0) {
				this.title_layout.SetText ("");
				return;
			}
			
			int w, h, ell_w, ell_h, x, empty;
			this.title_layout.GetPixelSize (out w, out h);
			if (w <= width) return;
			
			Pango.Layout ell = this.title_layout.Copy ();
			ell.SetText ("...");
			ell.GetPixelSize (out ell_w, out ell_h);
			if (width < ell_w) {
				this.title_layout.SetText ("");
				return;
			}
			
			width -= ell_w;
			Pango.LayoutLine line = this.title_layout.GetLine (0);
			string text = this.title_layout.Text;
			if (line.XToIndex (width * 1024, out x, out empty)) {
				this.title_layout.SetText (text.Substring (0, x) + "...");
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			Gdk.Rectangle child_allocation;
			
			if (this.Direction == Gtk.TextDirection.Rtl)
				child_allocation.X = allocation.X + (int)this.BorderWidth;
			else
				child_allocation.X = allocation.X + allocation.Width - (int)this.BorderWidth;
			child_allocation.Y = allocation.Y + (int)this.BorderWidth;
			
			if (this.close_button.Visible) {
				Gtk.Requisition button_requisition = this.close_button.SizeRequest ();
				if (this.Direction != Gtk.TextDirection.Rtl) 
					child_allocation.X -= button_requisition.Width;
				
				child_allocation.Width = button_requisition.Width;
				child_allocation.Height = button_requisition.Height;
				
				this.close_button.SizeAllocate (child_allocation);
				
				if (this.Direction == Gtk.TextDirection.Rtl)
					child_allocation.X += button_requisition.Width;
			}
			
			if (this.iconify_button.Visible) {
				Gtk.Requisition button_requisition = this.iconify_button.SizeRequest ();
				if (this.Direction != Gtk.TextDirection.Rtl)
					child_allocation.X -= button_requisition.Width;
				
				child_allocation.Width = button_requisition.Width;
				child_allocation.Height = button_requisition.Height;
				
				this.iconify_button.SizeAllocate (child_allocation);
				
				if (this.Direction == Gtk.TextDirection.Rtl)
					child_allocation.X += button_requisition.Width;
			}
			
			if (this.title_window != null) {
				this.EnsureTitleAndIconPixbuf ();
				this.title_layout.SetText (this.title);
				Gdk.Rectangle area = this.GetTitleArea ();
				this.title_window.MoveResize (area.X, area.Y, area.Width, area.Height);
				if (this.icon_pixbuf != null) {
					area.Width -= this.icon_pixbuf.Width + 1;
				}
				this.EllipsizeLayout (area.Width);
				Console.WriteLine ("Text: |" + this.title_layout.Text + "|");
			} else {
				Console.WriteLine ("title_window was null");
			}
		}
		
		protected override void OnAdded (Gtk.Widget widget)
		{
			Console.WriteLine ("You can't add a widget to DockItemGrip directly");
		}
		
		protected override void OnRemoved (Gtk.Widget widget)
		{
			Console.WriteLine ("You can't remove a widget from DockItemGrip directly");
		}
		
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals) {
				invoker.Invoke (this.close_button);
				invoker.Invoke (this.iconify_button);
			}
		}
	}
}