using System;

using Gtk;

namespace MonoDevelop.Gui.Widgets
{

	public class PropertyGrid : Frame
	{

		public PropertyGridWidget internalTable;

		object gridding;

		public PropertyGrid () : base ()
		{
			internalTable = new PropertyGridWidget ();
			internalTable.HscrollbarPolicy = Gtk.PolicyType.Never;
			this.Add (internalTable);
		}

		public PropertyGrid (object togrid) : this ()
		{
			gridding = togrid;
		}
	}
}
