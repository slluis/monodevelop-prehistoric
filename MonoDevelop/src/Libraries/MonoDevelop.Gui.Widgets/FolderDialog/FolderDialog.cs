//
// Author: John Luke  <jluke@cfl.rr.com>
// License: LGPL
//

using System;
using Gtk;

namespace MonoDevelop.Gui.Widgets
{
	public class FolderDialog : FileSelection
	{
		static GLib.GType gtype;

		public static new GLib.GType GType
		{
			get
			{
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (FolderDialog));
				return gtype;
			}
		}

		public FolderDialog (string title) : base (GType)
		{
			this.Title = title;
			this.SelectMultiple = false;
			this.ShowFileops = false;
			this.FileList.Sensitive = false;
		}
	}
}
