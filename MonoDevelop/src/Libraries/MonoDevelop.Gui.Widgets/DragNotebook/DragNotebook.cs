// created on 03/19/2004 at 20:45

using System;
using Gtk;
using GtkSharp;
using Gdk;

/*public class prueba
{
	public static void Main()
	{
		Application.Init ();
		new Interfaz ();
		Application.Run ();
	}
}*/
namespace MonoDevelop.Gui.Widgets 
{

	public delegate void TabsReorderedHandler (Widget widget, int oldPlacement, int newPlacement);

	public class DragNotebook : Notebook {
		protected bool draginprogress = false;
		protected int srcpage;
		protected double xstart;
		protected double ystart;
		protected Cursor cursor;

		public DragNotebook ()
		{
			this.ButtonPressEvent += new ButtonPressEventHandler (ButtonPressCallback);
			this.ButtonReleaseEvent += new ButtonReleaseEventHandler (ButtonReleaseCallback);
			this.AddEvents ((Int32) (EventMask.AllEventsMask));
		}
		
		protected int FindTabAtNumPos (double absx, double absy)
		{
			PositionType tabpos;
			int pagenum, xroot, yroot, maxx, maxy;
			pagenum = 0;
			Widget page, tab;
			
			tabpos = this.TabPos;
			if (this.NPages == 0) {
				return -1;
			}
			
			page = this.GetNthPage (pagenum);
			
			while (page != null) {
				tab = this.GetTabLabel (page);
				
				if (tab == null) {
					return -1;
				}
				
				// if (!tab.Mapped)
				// {
				//	pagenum++;
				//	continue;
				// }
				
				tab.ParentWindow.GetOrigin (out xroot, out yroot);
				
				maxx = xroot + tab.Allocation.X + tab.Allocation.Width;
				maxy = yroot + tab.Allocation.Y + tab.Allocation.Height;
				
				if ((tabpos == PositionType.Top || tabpos == PositionType.Bottom) && absx <= maxx) {
					return pagenum;
				}
				else if ((tabpos == PositionType.Right || tabpos == PositionType.Left) && absx <= maxy) {
					return pagenum;
				}
				
				pagenum++;
				page = this.GetNthPage (pagenum);
			}
			
			return -1;
		}
		
		public event TabsReorderedHandler OnTabsReordered;
		
		[GLib.ConnectBefore]
		protected void MotionNotifyCallback (object obj, MotionNotifyEventArgs args)
		{
			int curpage, pagenum;
			
			if (!draginprogress) {
				//if (Gtk.Drag.CheckThreshold (this, (Int32) xstart, (Int32) ystart, (Int32) args.Event.XRoot, (Int32) args.Event.YRoot))
				//{
				curpage = this.CurrentPage;
				DragStart (curpage, args.Event.Time);
				//}
				//else
				//{
				//	return;
				//}
			}
			
			pagenum = FindTabAtNumPos (args.Event.XRoot, args.Event.YRoot);
			
			MoveTab (pagenum);
		}
		
		protected void MoveTab (int destpagenum)
		{
			int curpagenum;
			Widget curpage, tab;
			
			curpagenum = this.CurrentPage;
			
			if (destpagenum != curpagenum) {
				curpage = this.GetNthPage (curpagenum);
				tab = this.GetTabLabel (curpage);
				this.ReorderChild (CurrentPageWidget, destpagenum);
				if (OnTabsReordered != null)
					OnTabsReordered (CurrentPageWidget, curpagenum, destpagenum);
			}
		}
		
		protected void DragStart (int srcpage, uint time)
		{
			draginprogress = true;
			
			this.srcpage = srcpage;
			
			if (cursor == null) {
				cursor = new Cursor (CursorType.Fleur);
			}
			
			Grab.Add (this);
			
			if (!Pointer.IsGrabbed) {
				Pointer.Grab (this.ParentWindow, false, EventMask.Button1MotionMask | EventMask.ButtonReleaseMask, null, cursor, time);						
			}
		}
		
		protected void DragStop ()
		{
			if (draginprogress) {
				//OnTabsReordered();
			}
			
			draginprogress = false;
			srcpage = -1;
			this.MotionNotifyEvent -= new MotionNotifyEventHandler (MotionNotifyCallback);
		}
		
		protected void ButtonReleaseCallback (object obj, ButtonReleaseEventArgs args)
		{
			if (Pointer.IsGrabbed) {
				Pointer.Ungrab (args.Event.Time);
				Gtk.Grab.Remove (this);
			}
			
			DragStop ();
		}
		
		[GLib.ConnectBefore]
		protected void ButtonPressCallback (object obj, ButtonPressEventArgs args) {
			int tabpos;
			
			tabpos = FindTabAtNumPos (args.Event.XRoot, args.Event.YRoot);
			
			if (draginprogress) {
				return;
			}
			else {
				srcpage = this.CurrentPage;
			}
			
			xstart = args.Event.XRoot;
			ystart = args.Event.YRoot;
			
			if (args.Event.Button == 1 && args.Event.Type == EventType.ButtonPress && tabpos >= 0) {
					this.MotionNotifyEvent += new MotionNotifyEventHandler (MotionNotifyCallback);
			}
		}
	}
}

