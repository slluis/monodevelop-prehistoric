using System;
using System.IO;
using Gtk;
using Gdk;

namespace MonoDevelop.Gui.Widgets {
	public class FileEntry : BaseFileEntry {

		public FileEntry (string name) : base (name)
		{
		}
		
		protected override string ShowBrowseDialog (string name, string start_in)
		{
			FileSelector fd = new FileSelector (name);
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
