using System;
using Gtk;
using GtkSharp;

namespace MonoDevelop.Gui.Widgets
{
	public class FolderDialog : FileSelection
	{
		static GLib.GType type;

		static FolderDialog ()
		{
			type = RegisterGType (typeof (FolderDialog));
		}

		public FolderDialog (string title) : base (type)
		{
			this.Title = title;
			this.SelectMultiple = false;
			this.ShowFileops = false;
			this.FileList.Sensitive = false;
		}
	}
}
