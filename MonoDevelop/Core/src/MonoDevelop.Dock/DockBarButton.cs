using System;
using Gtk;

namespace Gdl
{
	public class DockBarButton : Button
	{
		DockItem item;

		public DockBarButton (DockItem item)
		{
			this.item = item;
			this.Relief = ReliefStyle.None;
			this.Add (new Image (item.StockId, IconSize.SmallToolbar));
		}

		public DockItem DockItem {
			get { return item; }
		}
	}
}

