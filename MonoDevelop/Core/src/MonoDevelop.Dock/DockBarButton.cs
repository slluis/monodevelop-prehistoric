using System;
using Gtk;

namespace Gdl
{
	public class DockBarButton : Button
	{
		DockItem item;

		public event EventHandler DockButtonClicked;

		public DockBarButton (DockItem item)
		{
			this.item = item;
			this.Relief = ReliefStyle.None;
			Image image = new Image (item.StockId, IconSize.SmallToolbar);
			this.Add (image);
			this.Clicked += OnClicked;
		}

		public DockItem DockItem {
			get {
				return item;
			}
		}

		private void OnClicked (object sender, EventArgs args)
		{
			if (DockButtonClicked != null)
				DockButtonClicked (this, EventArgs.Empty);
		}
	}
}

