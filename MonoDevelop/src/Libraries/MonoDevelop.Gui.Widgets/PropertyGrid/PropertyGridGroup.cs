using System;
using System.Collections;

using Gtk;

namespace MonoDevelop.Gui.Widgets
{
	public class PropertyGridGroup : Gtk.Table
	{
		ELabel header;
		Button expandButton;

		Table internalTable;

		ArrayList PropertyGridItems;
		
		public PropertyGridGroup (string header) : base (2, 2, false)
		{
			PropertyGridItems = new ArrayList ();
			this.expandButton = new Button (".");
			Attach (this.expandButton, 0, 1, 0, 1, Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0);
			this.header = new ELabel (header);
			Attach (this.header, 1, 2, 0, 1, Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink, 0, 0);

			internalTable = new Gtk.Table (1, 2, true);
			Attach (internalTable, 1, 2, 1, 2, Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink, 0, 0);
		}

		public void AddGridItem (string name, Gtk.Widget editor)
		{
			PropertyGridItem newItem = new PropertyGridItem (name, editor);
			PropertyGridItems.Add (newItem);

			Refresh ();
		}

		void Refresh ()
		{
			internalTable.NRows = (uint)PropertyGridItems.Count;
			foreach (Gtk.Widget child in internalTable.Children) {
				internalTable.Remove (child);
			}
			for (uint i = 0; i < PropertyGridItems.Count; i++) {
				internalTable.Attach (((PropertyGridItem)PropertyGridItems[(int)i]).Label, 0, 1, i, i + 1);
				internalTable.Attach (((PropertyGridItem)PropertyGridItems[(int)i]).Editor, 1, 2, i, i + 1);
			}
		}
	}
}
