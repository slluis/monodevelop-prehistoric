// created on 07/06/2004 at 5:44 P

using System;
using Gtk;

namespace Gdl
{
	public class DockNotebook : DockItem
	{
		
		static DockNotebook ()
		{
			Gtk.Rc.ParseString ("style \"gdl-dock-notebook-default\" {\n" +
			                    "xthickness = 2\n" +
			                    "ythickness = 2\n" +
			                    "}\n" +
			                    "widget_class \"*.GtkNotebook.Gdl_DockItem\" " +
			                    "style : gtk \"gdl-dock-notebook-default\"\n");
		}
		
		public DockNotebook ()
		{
			this.Child = new Gtk.Notebook ();
			this.Child.Parent = this;
			((Gtk.Notebook)this.Child).TabPos = Gtk.PositionType.Bottom;
			//((Gtk.Notebook)this.Child).SwitchPage += new Gtk.SwitchPageHandler (SwitchPageCb);
			//((Gtk.Notebook)this.Child).ButtonPressEvent += new Gtk.ButtonPressEvent (ButtonPressCb);
			//((Gtk.Notebook)this.Child).ButtonReleaseEvent += new Gtk.ButtonReleaseEvent (ButtonReleaseCb);
			((Gtk.Notebook)this.Child).Scrollable = true;
			this.Child.Show ();
			this.DockObjectFlags &= DockObjectFlags.Automatic;
		}
		
		protected void SwitchPageHandler (object o, SwitchPageArgs e)
		{
			//Does this code need to be ported at all?
		}
		
		protected override void OnAdded (Gtk.Widget widget)
		{
			if (widget == null || !(widget is DockItem))
				return;
			this.Docking ((DockObject)widget, DockPlacement.Center, null);
		}
		
		private CallbackInvoker stored_invoker;
		protected override void ForAll (bool include_internals, CallbackInvoker invoker)
		{
			if (include_internals) {
				base.ForAll (include_internals, invoker);
			} else {
				if (this.Child != null) {
					stored_invoker = invoker;
					((Gtk.Notebook)this.Child).Foreach (new Gtk.Callback (childForAll));
				}
			}
		}
		
		private void childForAll (Gtk.Widget widget)
		{
			stored_invoker.Invoke (widget);
		}
		
		protected struct DockInfo
		{
			public DockPlacement position;
			public object other_data;
			
			public DockInfo (DockPlacement pos, object data)
			{
				position = pos;
				other_data = data;
			}
		}
		
		private void dock_child (Widget w)
		{
			if (w is DockObject)
				this.Docking ((DockObject)w, stored_info.position, stored_info.other_data);
		}
		
		private DockInfo stored_info;
		public override void Docking (DockObject requestor, DockPlacement position, object extra_data)
		{
			if (position == DockPlacement.Center) {
				if (requestor.IsCompound) {
					requestor.Freeze ();
					stored_info = new DockInfo (position, extra_data);
					requestor.Foreach (new Gtk.Callback (dock_child));
					requestor.Thaw ();
				} else {
					DockItem requestor_item = requestor as DockItem;
					if (requestor_item == null)
						return;
					Gtk.Widget label = requestor_item.TabLabel;
					if (label == null) {
						label = new Gtk.Label (requestor_item.LongName);
						requestor_item.TabLabel = label;
					}
					int new_position = -1;
					if (extra_data is Int32)
						new_position = Convert.ToInt32 (extra_data);
					((Gtk.Notebook)this.Child).InsertPage (requestor, label, new_position);
					requestor.DockObjectFlags |= DockObjectFlags.Attached;
				}
			} else
				base.Docking (requestor, position, extra_data);
		}
		
		public override void SetOrientation (Gtk.Orientation orientation)
		{
			if (this.Child != null && this.Child is Gtk.Notebook) {
				if (orientation == Gtk.Orientation.Horizontal)
					((Gtk.Notebook)this.Child).TabPos = Gtk.PositionType.Top;
				else
					((Gtk.Notebook)this.Child).TabPos = Gtk.PositionType.Left;
			}
			base.SetOrientation (orientation);
		}
		
		public override bool ChildPlacement (DockObject child, ref DockPlacement position)
		{
			DockPlacement pos = DockPlacement.None;
			if (this.Child != null) {
				foreach (Gtk.Widget widget in ((Gtk.Notebook)this.Child).Children) {
					if (widget == child) {
						pos = DockPlacement.Center;
						break;
					}
				}
			}
			if (pos != DockPlacement.None) {
				position = pos;
				return true;
			}
			return false;
		}
		
		public override void Present (DockObject child)
		{
			int i = ((Gtk.Notebook)this.Child).PageNum (child);
			if (i >= 0) {
				((Gtk.Notebook)this.Child).CurrentPage = i;
			}
			base.Present (child);
		}
		
		public override bool Reorder (DockObject requestor, DockPlacement new_position, object other_data)
		{
			bool handled = false;
			int current_position, new_pos = -1;
			
			if (this.Child != null && new_position == DockPlacement.Center) {
				current_position = ((Gtk.Notebook)this.Child).PageNum (requestor);
				if (current_position >= 0) {
					handled = true;
					if (other_data is Int32)
						new_pos = Convert.ToInt32 (other_data);
					((Gtk.Notebook)this.Child).ReorderChild (requestor, new_pos);
				}
			}
			return handled;
		}
		
		public override bool IsCompound {
			get { return false; }
		}
		
		public int Page {
			get { return ((Gtk.Notebook)this.Child).CurrentPage; }
			set { ((Gtk.Notebook)this.Child).CurrentPage = value; }
		}
		
		public override bool HasGrip {
			get { return false; }
		}
	}
}