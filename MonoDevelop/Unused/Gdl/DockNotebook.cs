// created on 07/06/2004 at 5:44 P

using System;
using System.Xml;
using Gtk;

namespace Gdl
{
	public class DockNotebook : DockItem
	{
		private struct DockInfo
		{
			public DockPlacement position;
			public object data;
			
			public DockInfo (DockPlacement position, object data)
			{
				this.position = position;
				this.data = data;
			}
		}
		
		private DockInfo dockInfo;
		private CallbackInvoker storedInvoker;

		protected DockNotebook (IntPtr raw) : base (raw) { }

		static DockNotebook ()
		{
			Rc.ParseString ("style \"gdl-dock-notebook-default\" {\n" +
			                    "xthickness = 2\n" +
			                    "ythickness = 2\n" +
			                    "}\n" +
			                    "widget_class \"*.GtkNotebook.Gdl_DockItem\" " +
			                    "style : gtk \"gdl-dock-notebook-default\"\n");
		}
		
		public DockNotebook ()
		{
			Child = new Notebook ();
			Child.Parent = this;
			((Notebook)Child).TabPos = PositionType.Bottom;
			//((Notebook)Child).SwitchPage += new SwitchPageHandler (SwitchPageCb);
			//((Notebook)Child).ButtonPressEvent += new ButtonPressEvent (ButtonPressCb);
			//((Notebook)Child).ButtonReleaseEvent += new ButtonReleaseEvent (ButtonReleaseCb);
			((Notebook)Child).Scrollable = true;
			Child.Show ();
			DockObjectFlags &= DockObjectFlags.Automatic;
		}
		
		protected void SwitchPageHandler (object o, SwitchPageArgs e)
		{
			//Does this code need to be ported at all?
		}

		/*protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
			if (Child != null) {
				Child.Unparent ();
				Child = null;
			}
		}*/
		
		protected override void OnAdded (Widget widget)
		{
			if (widget == null || !(widget is DockItem))
				return;

			Dock ((DockObject)widget, DockPlacement.Center, null);
		}
		
		protected override void ForAll (bool includeInternals, CallbackInvoker invoker)
		{
			if (includeInternals) {
				base.ForAll (includeInternals, invoker);
			} else {
				if (Child != null) {
					storedInvoker = invoker;
					((Notebook)Child).Foreach (new Callback (ChildForAll));
				}
			}
		}
		
		private void ChildForAll (Widget widget)
		{
			storedInvoker.Invoke (widget);
		}
		
		private void DockChild (Widget w)
		{
			if (w is DockObject)
				Dock ((DockObject)w, dockInfo.position, dockInfo.data);
		}

		public override void FromXml (XmlNode node)
		{
			string orientation = node.Attributes["orientation"].Value;
			this.Orientation = orientation == "vertical" ? Orientation.Vertical : Orientation.Horizontal;
			string locked = node.Attributes["locked"].Value;
			this.Locked = locked == "no" ? false : true;
			string page = node.Attributes["page"].Value;
			// FIXME: after property?
			this.Page = int.Parse (page);
		}
		
		public override void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
			/* we only add support for DockPlacement.Center docking
			   strategy here... for the rest use our parent class' method */
			if (position == DockPlacement.Center) {
				/* we can only dock simple (not compound) items */
				if (requestor.IsCompound) {
					requestor.Freeze ();
					dockInfo = new DockInfo (position, data);
					requestor.Foreach (new Callback (DockChild));
					requestor.Thaw ();
				} else {
					DockItem requestorItem = requestor as DockItem;
					Widget label = requestorItem.TabLabel;
					if (label == null) {
						label = new Label (requestorItem.LongName);
						requestorItem.TabLabel = label;
					}
					
					int tabPosition = -1;
					if (data is Int32)
						tabPosition = Convert.ToInt32 (data);
					((Notebook)Child).InsertPage (requestor, label, tabPosition);
					requestor.DockObjectFlags |= DockObjectFlags.Attached;
				}
			} else {
				base.OnDocked (requestor, position, data);
			}
		}
		
		public override void SetOrientation (Orientation orientation)
		{
			if (Child != null && Child is Notebook) {
				if (orientation == Orientation.Horizontal)
					((Notebook)Child).TabPos = PositionType.Top;
				else
					((Notebook)Child).TabPos = PositionType.Left;
			}
			base.SetOrientation (orientation);
		}
		
		public override bool OnChildPlacement (DockObject child, ref DockPlacement position)
		{
			DockPlacement pos = DockPlacement.None;
			if (Child != null) {
				foreach (Widget widget in ((Notebook)Child).Children) {
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
		
		public override void OnPresent (DockObject child)
		{
			Notebook nb = Child as Notebook;
			
			int i = nb.PageNum (child);
			if (i >= 0)
				nb.CurrentPage = i;

			base.OnPresent (child);
		}
		
		public override bool OnReorder (DockObject requestor, DockPlacement position, object other_data)
		{
			bool handled = false;
			int current_position, new_pos = -1;
			
			if (Child != null && position == DockPlacement.Center) {
				current_position = ((Notebook)Child).PageNum (requestor);
				if (current_position >= 0) {
					handled = true;
					if (other_data is Int32)
						new_pos = Convert.ToInt32 (other_data);
					((Notebook)Child).ReorderChild (requestor, new_pos);
				}
			}
			return handled;
		}
		
		public override bool IsCompound {
			get { return true; }
		}
		
		public int Page {
			get { return ((Notebook)Child).CurrentPage; }
			set { ((Notebook)Child).CurrentPage = value; }
		}
		
		public override bool HasGrip {
			get { return false; }
		}
	}
}
