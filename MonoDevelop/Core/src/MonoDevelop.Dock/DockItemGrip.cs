// created on 07/06/2004 at 2:52 P

using System;
using Gtk;

namespace Gdl
{
	public class DockItemGrip : Container
	{
		private DockItem item;
		private Gdk.Window titleWindow;
		private Button closeButton;
		private Button iconifyButton;
		private Tooltips tooltips;
		private Gdk.Pixbuf icon = null;
		private string title;
		private Pango.Layout layout = null;

		protected DockItemGrip (IntPtr raw) : base (raw) { }
		
		protected DockItemGrip ()
		{
			WidgetFlags |= WidgetFlags.NoWindow;
			
			Widget.PushCompositeChild ();
			closeButton = new Button ();
			Widget.PopCompositeChild ();
			
			closeButton.WidgetFlags &= ~WidgetFlags.CanFocus;
			closeButton.Parent = this;
			closeButton.Relief = ReliefStyle.None;
			closeButton.Show ();
			
			Image image = new Image (Gdl.Stock.Close, IconSize.Menu);
			closeButton.Add (image);
			image.Show ();
			
			closeButton.Clicked += new EventHandler (CloseClicked);
			
			Widget.PushCompositeChild ();
			iconifyButton = new Button ();
			Widget.PopCompositeChild ();
			
			iconifyButton.WidgetFlags &= ~WidgetFlags.CanFocus;
			iconifyButton.Parent = this;
			iconifyButton.Relief = ReliefStyle.None;
			iconifyButton.Show ();
			
			image = new Image (Gdl.Stock.MenuLeft, IconSize.Menu);
			iconifyButton.Add (image);
			image.Show ();
			
			iconifyButton.Clicked += new EventHandler (IconifyClicked);
			
			tooltips = new Tooltips ();
			tooltips.SetTip (iconifyButton, "Iconify", "Iconify this dock");
			tooltips.SetTip (closeButton, "Close", "Close this dock");
		}
		
		public DockItemGrip (DockItem item) : this ()
		{
			if (item == null)
				throw new ArgumentNullException ("A valid DockItem must be given");
			Item = item;
		}
		
		private Gdk.Pixbuf Icon {
			get {
				if (icon == null && item.StockId != null)
					icon = RenderIcon (item.StockId, IconSize.Menu, "");
				return icon;
			}
		}
		
		public new DockItem Item {
			get {
				return item;
			}
			set {
				if (item != null)
					item.PropertyChanged -= OnPropertyChanged;
				
				item = value;
				item.PropertyChanged += OnPropertyChanged;
				
				if (!item.CantClose)
					closeButton.Show ();
				else
					closeButton.Hide ();

				if (!item.CantIconify)
					iconifyButton.Show ();
				else
					iconifyButton.Hide ();

				icon = null;
				layout = null;
				title = null;
			}
		}
		
		private Pango.Layout Layout {
			get {
				if (layout == null) {
					layout = CreatePangoLayout (Title);
					layout.SingleParagraphMode = true;
				}
				return layout;
			}
		}
		
		private string Title {
			get {
				if (title == null) {
					if (item.LongName != null)
						title = item.LongName;
					else
						title = "";
				}
				return title;
			}
			set {
				title = value;
				if (layout != null)
					layout.SetMarkup (Title);
			}
		}
		
		private Gdk.Rectangle TitleArea {
			get {
				Gdk.Rectangle area;
				int bw = (int)BorderWidth;
				int height, width;
				
				area.Width = Allocation.Width - 2 * bw;
				
				Layout.GetPixelSize (out width, out height);
				
				if (closeButton.Visible) {
					height = Math.Max (height, closeButton.Allocation.Height);
					area.Width -= closeButton.Allocation.Width;
				}
				
				if (iconifyButton.Visible) {
					height = Math.Max (height, iconifyButton.Allocation.Height);
					area.Width -= iconifyButton.Allocation.Width;
				}
				
				area.X = Allocation.X + bw;
				area.Y = Allocation.Y + bw;
				area.Height = height;
				
				if (Direction == TextDirection.Rtl)
					area.X += (Allocation.Width - 2 * bw) - area.Width;
					
				return area;
			}
		}
		
		public Gdk.Window TitleWindow {
			get {
				return titleWindow;
			}
			set {
				titleWindow = value;
			}
		}
		
		private void OnPropertyChanged (object o, string name)
		{
			switch (name) {
			case "StockId":
				icon = RenderIcon (item.StockId, IconSize.Menu, "");
				break;
			case "LongName":
				Title = item.LongName;
				break;
			case "Behavior":
				if (!item.CantClose)
					closeButton.Show ();
				else
					closeButton.Hide ();

				if (!item.CantIconify)
					iconifyButton.Show ();
				else
					iconifyButton.Hide ();
				break;
			default:
				break;
			}
		}

		protected override void OnDestroyed ()
		{
			if (layout != null)
				layout = null;
			if (icon != null)
				icon = null;
			if (tooltips != null)
				tooltips = null;
			if (item != null) {
				// FIXME: Disconnect future signal handlers for notify.
			}
			item = null;
			base.OnDestroyed ();
		}
	
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			Gdk.Rectangle titleArea = TitleArea;
			Gdk.Rectangle exposeArea;
			
			if (Icon != null) {
				Gdk.Rectangle pixbufRect;
				pixbufRect.Width = icon.Width;
				pixbufRect.Height = icon.Height;
				
				if (Direction == TextDirection.Rtl) {
					pixbufRect.X = titleArea.X + titleArea.Width - pixbufRect.Width;
				} else {
					pixbufRect.X = titleArea.X;
					titleArea.X += pixbufRect.Width + 1;
				}
				
				titleArea.Width -= pixbufRect.Width - 1;
				pixbufRect.Y = titleArea.Y + (titleArea.Height - pixbufRect.Height) / 2;

				if (evnt.Area.Intersect (pixbufRect, out exposeArea)) {
					Gdk.GC gc = Style.BackgroundGC (State);
					GdkWindow.DrawPixbuf (gc, icon, 0, 0, pixbufRect.X,
							      pixbufRect.Y, pixbufRect.Width,
							      pixbufRect.Height, Gdk.RgbDither.None,
							      0, 0);
				}
			}

			if (titleArea.Intersect (evnt.Area, out exposeArea)) {
				int width, height, textX, textY;
				Layout.GetPixelSize (out width, out height);
				
				if (Direction == TextDirection.Rtl)
					textX = titleArea.X + titleArea.Width - width;
				else
					textX = titleArea.X;

				textY = titleArea.Y + (titleArea.Height - height) / 2;
	
				Style.PaintLayout (Style, GdkWindow, State, true,
						   exposeArea, this, null, textX,
						   textY, layout);
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
			iconifyButton.InButton = false;
			iconifyButton.Leave ();
		}
		
		protected override void OnRealized ()
		{
			base.OnRealized ();

			if (titleWindow == null) {
				Gdk.WindowAttr attributes = new Gdk.WindowAttr ();
				Gdk.Rectangle area = TitleArea;
				
				attributes.X = area.X;
				attributes.Y = area.Y;
				attributes.Width = area.Width;
				attributes.Height = area.Height;
				attributes.WindowType = Gdk.WindowType.Temp;
				attributes.Wclass = Gdk.WindowClass.InputOnly;
				attributes.OverrideRedirect = true;
				attributes.EventMask = (int) (Events |
					Gdk.EventMask.ButtonPressMask |
					Gdk.EventMask.ButtonReleaseMask |
					Gdk.EventMask.ButtonMotionMask);
	
				titleWindow = new Gdk.Window (ParentWindow, attributes,
					(int) (Gdk.WindowAttributesType.X |
					Gdk.WindowAttributesType.Y |
					Gdk.WindowAttributesType.Noredir));

				titleWindow.UserData = Handle;
				titleWindow.Cursor = new Gdk.Cursor (Display, Gdk.CursorType.Hand2);
			}
		}
		
		protected override void OnUnrealized ()
		{
			if (titleWindow != null) {
				titleWindow.UserData = IntPtr.Zero;
				titleWindow.Destroy ();
				titleWindow = null;
			}

			base.OnUnrealized ();
		}
		
		protected override void OnMapped ()
		{
			base.OnMapped ();

			if (titleWindow != null)
				titleWindow.Show ();
		}
		
		protected override void OnUnmapped ()
		{
			if (titleWindow != null)
				titleWindow.Hide ();

			base.OnUnmapped ();
		}
		
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			requisition.Width = (int)BorderWidth * 2;
			requisition.Height = (int)BorderWidth * 2;

			if (closeButton.Visible) {
				Requisition childReq = closeButton.SizeRequest ();
				requisition.Width += childReq.Width;
				requisition.Height = Math.Max (requisition.Height,
							       childReq.Height);
			}
			
			if (iconifyButton.Visible) {
				Requisition childReq = iconifyButton.SizeRequest ();
				requisition.Width += childReq.Width;
				requisition.Height = Math.Max (requisition.Height,
							       childReq.Height);
			}
			
			if (Icon != null) {
				requisition.Width += icon.Width + 1;
				requisition.Height = Math.Max (requisition.Height,
							       icon.Height);
			}
		}
		
		private void EllipsizeLayout (int width)
		{
			// no room to draw anything
			if (width < 1) {
				layout.SetMarkup ("");
				return;
			}
			
			// plenty of room
			int lw, lh;
			layout.GetPixelSize (out lw, out lh);
			if (width > lw)
				return;
			
			// not enough room for ...
			int ell_w, ell_h;
			Pango.Layout ell = layout.Copy ();
			ell.SetMarkup ("...");
			ell.GetPixelSize (out ell_w, out ell_h);
			if (width < ell_w) {
				layout.SetMarkup ("");
				return;
			}
			
			// subtract ellipses width
			width -= ell_w;

			int index, trailing;
			Pango.LayoutLine line = layout.GetLine (0);
			if (line.XToIndex (width * 1024, out index, out trailing)) {
				// Console.WriteLine ("length: {0} index: {1} trailing: {2}", layout.Text.Length, index, trailing);
				// FIXME: breaks on accented chars
				if (index < layout.Text.Length)
					layout.SetMarkup (layout.Text.Substring (0, index) + "...");
			}
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);

			Gdk.Rectangle childAlloc;
			int bw = (int)BorderWidth;
			
			if (Direction == TextDirection.Rtl)
				childAlloc.X = allocation.X + bw;
			else
				childAlloc.X = allocation.X + allocation.Width - bw;
			childAlloc.Y = allocation.Y + bw;
			
			if (closeButton.Visible) {
				Requisition buttonReq = closeButton.SizeRequest ();

				if (Direction != TextDirection.Rtl) 
					childAlloc.X -= buttonReq.Width;
				childAlloc.Width = buttonReq.Width;
				childAlloc.Height = buttonReq.Height;
				
				closeButton.SizeAllocate (childAlloc);
				
				if (Direction == TextDirection.Rtl)
					childAlloc.X += buttonReq.Width;
			}
			
			if (iconifyButton.Visible) {
				Requisition buttonReq = iconifyButton.SizeRequest ();
				
				if (Direction != TextDirection.Rtl)
					childAlloc.X -= buttonReq.Width;
				childAlloc.Width = buttonReq.Width;
				childAlloc.Height = buttonReq.Height;
				
				iconifyButton.SizeAllocate (childAlloc);
				
				if (Direction == TextDirection.Rtl)
					childAlloc.X += buttonReq.Width;
			}
			
			if (TitleWindow != null) {
				layout.SetMarkup (title);

				Gdk.Rectangle area = TitleArea;
				titleWindow.MoveResize (area.X, area.Y,area.Width, area.Height);
				
				if (Icon != null)
					area.Width -= icon.Width + 1;
				
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
				invoker.Invoke (closeButton);
				invoker.Invoke (iconifyButton);
			}
		}
	}
}
