// created on 07/06/2004 at 5:43 P

using System;
using Gtk;

namespace Gdl
{
	public class DockPaned : DockItem
	{
		private readonly float SplitRatio = 0.3f;
		private bool positionChanged = false;

		protected DockPaned (IntPtr raw) : base (raw) { }

		public DockPaned (Orientation orientation)
		{
			CreateChild (orientation);
		}
		
		public override bool HasGrip {
			get {
				return false;
			}
		}
		
		public override bool IsCompound {
			get {
				return true;
			}
		}
		
		public int Position {
			get {
				if (Child != null && Child is Paned) {
					return ((Paned)Child).Position;
				}
				return 0;
			}
			set {
				if (Child != null && Child is Paned) {
					((Paned)Child).Position = value;
				}
			}
		}
		
		private void CreateChild (Orientation orientation)
		{
			if (Child != null)
				Child.Unparent ();
				
			/* create the container paned */
			if (orientation == Orientation.Horizontal)
				Child = new HPaned ();
			else
				Child = new VPaned ();
			
			// FIXME: Register signal handlers.						
												
			Child.Parent = this;
			Child.Show ();
		}
		
		protected override void OnAdded (Widget widget)
		{
			if (Child == null)
				return;
		
			Paned paned = Child as Paned;
			if (paned.Child1 != null && paned.Child2 != null)
				return;
			
			DockItem item = widget as DockItem;
			DockPlacement pos = DockPlacement.None;			
			if (paned.Child1 == null)
				pos = (Orientation == Orientation.Horizontal ?
				       DockPlacement.Left : DockPlacement.Top);
			else
				pos = (Orientation == Orientation.Horizontal ?
				       DockPlacement.Right : DockPlacement.Bottom);
			
			if (pos != DockPlacement.None)
				Dock (item, pos, null);
		}
		
		private void childForAll (Widget widget)
		{
			stored_invoker.Invoke (widget);
		}

		/*protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
			if (Child != null) {
				Child.Unparent ();
				Child = null;
			}
		}*/	
	
		private CallbackInvoker stored_invoker;
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals) {
				base.ForAll (include_internals, invoker);
			} else {
				if (Child != null) {
					stored_invoker = invoker;
					((Paned)Child).Foreach (new Callback (childForAll));
				}
			}
		}
		
		public override void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
			if (Child == null)
				return;
		
			Paned paned = (Paned)Child;
			bool done = false;
			
			/* see if we can dock the item in our paned */
			switch (Orientation) {
			case Orientation.Horizontal:
				if (paned.Child1 == null && position == DockPlacement.Left) {
					paned.Pack1 (requestor, false, false);
					done = true;
				} else if (paned.Child2 == null && position == DockPlacement.Right) {
					paned.Pack2 (requestor, true, false);
					done = true;
				}
				break;
			case Orientation.Vertical:
				if (paned.Child1 == null && position == DockPlacement.Top) {
					paned.Pack1 (requestor, false, false);
					done = true;
				} else if (paned.Child2 == null && position == DockPlacement.Bottom) {
					paned.Pack2 (requestor, true, false);
					done = true;
				}
				break;
			}
			
			if (!done) {
				/* this will create another paned and reparent us there */
				base.OnDocked (requestor, position, data);
			} else {
				((DockItem)requestor).ShowGrip ();
				requestor.DockObjectFlags |= DockObjectFlags.Attached;
			}
		}
		
		public override bool OnDockRequest (int x, int y, ref DockRequest request)
		{
			bool mayDock = false;

			/* we get (x,y) in our allocation coordinates system */
			
			/* Get item's allocation. */
			Gdk.Rectangle alloc = Allocation;
			int bw = (int)BorderWidth;

			/* Get coordinates relative to our window. */
			int relX = x - alloc.X;
			int relY = y - alloc.Y;
			
			DockRequest myRequest = new DockRequest (request);
			
			/* Location is inside. */
			if (relX > 0 && relX < alloc.Width &&
			    relY > 0 && relY < alloc.Width) {
			    	int divider = -1;
			    
				/* It's inside our area. */
				mayDock = true;

				/* these are for calculating the extra docking parameter */
				Requisition other = DockItem.PreferredSize ((DockItem)request.Applicant);
				Requisition my = DockItem.PreferredSize (this);
				
				/* Set docking indicator rectangle to the Dock size. */
				Gdk.Rectangle reqRect = new Gdk.Rectangle ();
				reqRect.X = alloc.X + bw;
				reqRect.Y = alloc.Y + bw;
				reqRect.Width = alloc.Width - 2 * bw;
				reqRect.Height = alloc.Height - 2 * bw;
				myRequest.Rect = reqRect;
				
				myRequest.Target = this;

				/* See if it's in the border_width band. */
				/*if (relX < bw) {
					myRequest.Position = DockPlacement.Left;
					myRequest.Rect.Width = myRequest.Rect.Width * SplitRatio;
					divider = other.Width;
				} else if (relX > alloc->Width - bw) {
					myRequest.Position = DockPlacement.Right;
					myRequest.Rect.X += myRequest.Rect.Width * (1 - SplitRatio);
					myRequest.Rect.Width *= SplitRatio;
					divider = Math.Max (0, my.Width - other.Width);
				} else if (relY < bw) {
					myRequest.Position = DockPlacement.Top;
					myRequest.Rect.Height *= SplitRatio;
					divider = other.Height;
				} else if (relY > alloc->Height - bw) {
					myRequest.Position = DockPlacement.Bottom;
					myRequest.Rect.Y += myRequest.Rect.Height * (1 - SplitRatio);
					myRequest.Rect.Height *= SplitRatio;
					divider = Math.Max (0, my.Height - other.Height);
				} else { /* Otherwise try our children. */
				//}
			}
			return true;
		}
	}
}
