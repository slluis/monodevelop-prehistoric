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

		bool visible = true;
		
		public PropertyGridGroup (string header) : base (2, 2, false)
		{
			PropertyGridItems = new ArrayList ();
			this.expandButton = new Button (".");
			expandButton.Clicked += new EventHandler (OnExpandClicked);
			this.header = new ELabel (header);
			internalTable = new Gtk.Table (1, 2, true);
			Setup ();
		}

		void Setup ()
		{
			foreach (Gtk.Widget child in Children) {
				Remove (child);
			}
			Attach (this.expandButton, 0, 1, 0, 1, Gtk.AttachOptions.Shrink, Gtk.AttachOptions.Shrink, 0, 0);
			if (visible) {
				Attach (this.header, 1, 2, 0, 1, Gtk.AttachOptions.Fill, Gtk.AttachOptions.Shrink, 0, 0);
				Attach (internalTable, 1, 2, 1, 2, Gtk.AttachOptions.Expand, Gtk.AttachOptions.Shrink, 0, 0);
			} else {
				Attach (this.header, 1, 2, 0, 1);
			}
		}

		void OnExpandClicked (object o, EventArgs e)
		{
			if (visible) {
				visible = false;
				Setup ();
			} else {
				visible = true;
				Setup ();
			}
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
