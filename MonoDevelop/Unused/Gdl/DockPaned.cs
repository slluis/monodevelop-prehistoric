// created on 07/06/2004 at 5:43 P

using System;
using System.Xml;
using Gtk;

namespace Gdl
{
	public class DockPaned : DockItem
	{
		private readonly float SplitRatio = 0.3f;
		private bool positionChanged = false;

		protected DockPaned (IntPtr raw) : base (raw) { }

		public DockPaned () : this (Orientation.Horizontal)
		{
		}

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
		
		[ExportLayout]
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
			
			Orientation = orientation;

			/* create the container paned */
			if (orientation == Orientation.Horizontal)
				Child = new HPaned ();
			else
				Child = new VPaned ();
			
			// FIXME: track position to emit layout changed
			Child.AddNotification ("position", OnNotifyPosition);
												
			Child.Parent = this;
			Child.Show ();
		}

		// <paned orientation="horizontal" locked="no" position="226">
		public override void FromXml (XmlNode node)
		{
			string orientation = node.Attributes["orientation"].Value;
			this.Orientation = orientation == "horizontal" ? Orientation.Horizontal : Orientation.Vertical;
			CreateChild (this.Orientation);
			string locked = node.Attributes["locked"].Value;
			this.Locked = locked == "no" ? false : true;
		}

		public override void FromXmlAfter (XmlNode node)
		{
			// FIXME: still dont work
			this.Position = int.Parse (node.Attributes["position"].Value);
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

		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
			if (Child != null) {
				Child.Unparent ();
				Child = null;
			}
		}	
	
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
			
			/* Location is inside. */
			if (relX > 0 && relX < alloc.Width &&
			    relY > 0 && relY < alloc.Height) {
			    	int divider = -1;
			    
				/* It's inside our area. */
				mayDock = true;

				/* these are for calculating the extra docking parameter */
				Requisition other = ((DockItem)request.Applicant).PreferredSize;
				Requisition my = PreferredSize;
				
				/* Set docking indicator rectangle to the Dock size. */
				request.X = bw;
				request.Y = bw;
				request.Width = alloc.Width - 2 * bw;
				request.Height = alloc.Height - 2 * bw;
				request.Target = this;

				/* See if it's in the BorderWidth band. */
				if (relX < bw) {
					request.Position = DockPlacement.Left;
					request.Width = (int)(request.Width * SplitRatio);
					divider = other.Width;
				} else if (relX > alloc.Width - bw) {
					request.Position = DockPlacement.Right;
					request.X += (int)(request.Width * (1 - SplitRatio));
					request.Width = (int)(request.Width * SplitRatio);
					divider = Math.Max (0, my.Width - other.Width);
				} else if (relY < bw) {
					request.Position = DockPlacement.Top;
					request.Height = (int)(request.Height * SplitRatio);
					divider = other.Height;
				} else if (relY > alloc.Height - bw) {
					request.Position = DockPlacement.Bottom;
					request.Y += (int)(request.Height * (1 - SplitRatio));
					request.Height = (int)(request.Height * SplitRatio);
					divider = Math.Max (0, my.Height - other.Height);
				} else { /* Otherwise try our children. */
					mayDock = false;
					DockRequest myRequest = new DockRequest (request);
					foreach (DockObject item in Children) {
						if (item.OnDockRequest (relX, relY, ref myRequest)) {
							mayDock = true;
							request = myRequest;
							break;
						}
					}
					
					if (!mayDock) {
						/* the pointer is on the handle, so snap
						   to top/bottom or left/right */
						mayDock = true;
						
						if (Orientation == Orientation.Horizontal) {
							if (relY < alloc.Height / 2) {
								request.Position = DockPlacement.Top;
								request.Height = (int)(request.Height * SplitRatio);
								divider = other.Height;
							} else {
								request.Position = DockPlacement.Bottom;
								request.Y += (int)(request.Height * (1 - SplitRatio));
								request.Height = (int)(request.Height * SplitRatio);
								divider = Math.Max (0, my.Height - other.Height);
							}
						} else {
							if (relX < alloc.Width / 2) {
								request.Position = DockPlacement.Left;
								request.Width = (int)(request.Width * SplitRatio);
								divider = other.Width;
							} else {
								request.Position = DockPlacement.Right;
								request.X += (int)(request.Width * (1 - SplitRatio));
								request.Width = (int)(request.Width * SplitRatio);
								divider = Math.Max (0, my.Width - other.Width);
							}
						}
					}
				}
				
				if (divider >= 0 && request.Position != DockPlacement.Center)
					request.Extra = divider;

				if (mayDock) {				
					/* adjust returned coordinates so they are
					   relative to our allocation */
					request.X += alloc.X;
					request.Y += alloc.Y;
				}
			}

			return mayDock;
		}

		void OnNotifyPosition (object sender, EventArgs a)
		{
			Master.EmitLayoutChangedEvent ();
		}
	}
}
