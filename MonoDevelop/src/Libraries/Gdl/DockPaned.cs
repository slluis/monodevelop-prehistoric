// created on 07/06/2004 at 5:43 P

using System;
using Gtk;

namespace Gdl
{
	public class DockPaned : DockItem
	{
		private bool positionChanged = false;

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
	}
}
