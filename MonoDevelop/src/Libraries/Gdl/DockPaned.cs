// created on 07/06/2004 at 5:43 P

using System;
using Gtk;

namespace Gdl
{
	public class DockPaned : Gdl.DockItem
	{
		private bool position_changed = false;

		public DockPaned (Gtk.Orientation orientation)
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
				if (this.Child != null && this.Child is Gtk.Paned) {
					return ((Gtk.Paned)this.Child).Position;
				}
				return 0;
			}
			set {
				if (this.Child != null && this.Child is Gtk.Paned) {
					((Gtk.Paned)this.Child).Position = value;
				}
			}
		}
		
		private void CreateChild (Gtk.Orientation orientation)
		{
			if (this.Child != null)
				this.Child.Unparent ();
				
			if (orientation == Gtk.Orientation.Horizontal)
				this.Child = new Gtk.HPaned ();
			else
				this.Child = new Gtk.VPaned ();
			
			//Signal connects?
			
			this.Child.Parent = this;
			this.Child.Show ();
		}
		
		protected override void OnAdded (Gtk.Widget widget)
		{
			DockItem item = widget as Gdl.DockItem;
			if (item == null)
				return;
			Gtk.Paned paned = (Gtk.Paned)this.Child;
			if (paned.Child1 != null && paned.Child2 != null)
				return;
			
			DockPlacement pos = DockPlacement.None;
			
			if (paned.Child1 == null)
				pos = item.Orientation == Gtk.Orientation.Horizontal ? DockPlacement.Left : DockPlacement.Top;
			else
				pos = item.Orientation == Gtk.Orientation.Vertical ? DockPlacement.Right : DockPlacement.Bottom;
			
			if (pos != DockPlacement.None)
				this.Docking (item, pos, null);
		}
		
		private void childForAll (Gtk.Widget widget)
		{
			stored_invoker.Invoke (widget);
		}
		
		private CallbackInvoker stored_invoker;
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals) {
				base.ForAll (include_internals, invoker);
			} else {
				if (this.Child != null) {
					stored_invoker = invoker;
					((Gtk.Paned)this.Child).Foreach (new Gtk.Callback (childForAll));
				}
			}
		}
		
		public override void Docking (DockObject requestor, DockPlacement position, object other_data)
		{
			if (this.Child == null)
				return;
			Gtk.Paned paned = (Gtk.Paned)this.Child;
			bool hresize = false;
			bool wresize = false;
			bool done = false;
			
			if (requestor is DockItem) {
				hresize = ((DockItem)requestor).PreferredHeight == -2 ? true : false;
				wresize = ((DockItem)requestor).PreferredWidth == -2 ? true : false;
			}
			
			switch (this.Orientation) {
			case Gtk.Orientation.Horizontal:
				if (paned.Child1 == null && position == DockPlacement.Left) {
					paned.Pack1 (requestor, wresize, false);
					done = true;
				} else if (paned.Child2 == null && position == DockPlacement.Right) {
					paned.Pack2 (requestor, wresize, false);
					done = true;
				}
				break;
			case Gtk.Orientation.Vertical:
				if (paned.Child1 == null && position == DockPlacement.Top) {
					paned.Pack1 (requestor, hresize, false);
					done = true;
				} else if (paned.Child2 == null && position == DockPlacement.Bottom) {
					paned.Pack2 (requestor, hresize, false);
					done = true;
				}
				break;
			}
			if (!done) {
				base.Docking (requestor, position, other_data);
			} else {
				((DockItem)requestor).ShowGrip ();
				requestor.DockObjectFlags |= DockObjectFlags.Attached;
			}
		}
	}
}