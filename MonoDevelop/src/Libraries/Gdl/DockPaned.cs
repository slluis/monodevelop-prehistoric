// created on 07/06/2004 at 5:43 P

using System;
using Gtk;

namespace Gdl
{
	public class DockPaned : DockItem
	{
		private bool position_changed = false;

		public DockPaned (Orientation orientation)
		{
			CreateChild (orientation);
		}
		
		public override bool HasGrip {
			get { return false; }
		}
		
		public override bool IsCompound {
			get { return true; }
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
			Console.WriteLine ("DockPaned.CreateChild");
			if (Child != null)
				Child.Unparent ();
				
			if (orientation == Orientation.Horizontal)
				Child = new HPaned ();
			else
				Child = new VPaned ();
			
			//Signal connects?
			Child.Parent = this;
			Child.Show ();
		}
		
		protected override void OnAdded (Widget widget)
		{
			if (widget == null)
				return;
			Gtk.Paned paned = (Gtk.Paned)this.Child;
			if (paned.Child1 != null && paned.Child2 != null) {
				return;
			}
			
			DockItem item = widget as DockItem;
			DockPlacement pos = DockPlacement.None;			
			if (paned.Child1 == null)
				pos = (this.Orientation == Gtk.Orientation.Horizontal ? DockPlacement.Left : DockPlacement.Top);
			else
				pos = (this.Orientation == Gtk.Orientation.Horizontal ? DockPlacement.Right : DockPlacement.Bottom);
			
			if (pos != DockPlacement.None)
				Docking (item, pos, null);
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
		
		public override void Docking (DockObject requestor, DockPlacement position, object other_data)
		{
			Console.WriteLine ("DockPaned.Docking");
		
			if (Child == null)
				return;
			Paned paned = (Paned)Child;
			bool hresize = false;
			bool wresize = false;
			bool done = false;
			
			if (requestor is DockItem) {
				hresize = ((DockItem)requestor).PreferredHeight == -2 ? true : false;
				wresize = ((DockItem)requestor).PreferredWidth == -2 ? true : false;
			}
			
			switch (Orientation) {
			case Orientation.Horizontal:
				if (paned.Child1 == null && position == DockPlacement.Left) {
					paned.Pack1 (requestor, wresize, false);
					done = true;
				} else if (paned.Child2 == null && position == DockPlacement.Right) {
					paned.Pack2 (requestor, wresize, false);
					done = true;
				}
				break;
			case Orientation.Vertical:
				if (paned.Child1 == null && position == DockPlacement.Top) {
					paned.Pack1 (requestor, hresize, false);
					done = true;
				} else if (paned.Child2 == null && position == DockPlacement.Bottom) {
					paned.Pack2 (requestor, hresize, false);
					done = true;
				}
				break;
			}
			Console.WriteLine ("DONE: {0}", done);
			if (!done) {
				base.Docking (requestor, position, other_data);
			} else {
				((DockItem)requestor).ShowGrip ();
				requestor.DockObjectFlags |= DockObjectFlags.Attached;
			}
		}
	}
}
