using System;
using System.IO;
using Gtk;
using Gdk;
using GtkSharp;
using GdkSharp;

namespace MonoDevelop.Gui.Widgets {
	public class FolderEntry : BaseFileEntry {

		public FolderEntry (string name) : base (name)
		{
		}
		
		protected override string ShowBrowseDialog (string name, string start_in)
		{
			FolderDialog fd = new FolderDialog (name);
			if (start_in != null)
				fd.Filename = start_in;
			
			int response = fd.Run ();
			fd.Hide ();
			
			if (response == (int) ResponseType.Ok)
				return fd.Filename;
			
			return null;
		}
	}
}
