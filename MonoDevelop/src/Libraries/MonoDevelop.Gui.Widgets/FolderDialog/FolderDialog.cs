//
// Author: John Luke  <jluke@cfl.rr.com>
// License: LGPL
//

using System;
using Gtk;

namespace MonoDevelop.Gui.Widgets
{
	public class FolderDialog : FileSelector
	{
		public FolderDialog (string title) : base (title)
		{
			this.SelectMultiple = false;
			this.ShowFileops = false;
			this.FileList.Sensitive = false;
		}
	}
}
