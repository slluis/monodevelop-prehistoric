using System;
using System.IO;
using Gtk;
using GtkSharp;

namespace MonoDevelop.Gui.Widgets
{
	public class FileBrowser : ScrolledWindow
	{
		private static GLib.GType gtype;
		private TreeView tv;
		private ListStore store;
		private string currentDir;
		private bool ignoreHidden;
		private string[] files;

		public FileBrowser () : base (GType)
		{
			this.VscrollbarPolicy = PolicyType.Automatic;
			this.HscrollbarPolicy = PolicyType.Automatic;
			ignoreHidden = true;

			tv = new TreeView ();
			tv.AppendColumn ("Name", new CellRendererText (), "text", 0);
			store = new ListStore (typeof (string));
			currentDir = Environment.GetEnvironmentVariable ("HOME");
			Populate ();
			tv.Model = store;

			tv.RowActivated += new RowActivatedHandler (OnRowActivated);
			tv.Selection.Changed += new EventHandler (OnSelectionChanged);

			this.Add (tv);
			this.ShowAll ();
		}

		public bool IgnoreHidden
		{
			get { return ignoreHidden; }
			set { ignoreHidden = value; }
		}

		public static new GLib.GType GType
		{
			get
			{
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (FileBrowser));

				return gtype;
			}
		}

		void Populate ()
		{
			store.Clear ();
			// seems unnecessary
			// store.AppendValues (".");
			if (currentDir != "/")
				store.AppendValues ("..");

			DirectoryInfo di = new DirectoryInfo (currentDir);
			DirectoryInfo[] dirs = di.GetDirectories ();
			
			foreach (DirectoryInfo d in dirs)
			{
				if (ignoreHidden)
				{
					if (!d.Name.StartsWith ("."))
						store.AppendValues (d.Name);
				}
				else
				{
					store.AppendValues (d.Name);
				}
			}
		}

		private void OnSelectionChanged (object o, EventArgs args)
		{
			TreeIter iter;
			TreeModel model;
			tv.Selection.GetSelected (out model, out iter);
			string selection = (string) model.GetValue (iter, 0);
			files = Directory.GetFiles (System.IO.Path.Combine (currentDir, selection));
		}

		private void OnRowActivated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			store.GetIter (out iter, args.Path);
			string file = (string) store.GetValue (iter, 0);
			currentDir = System.IO.Path.Combine (currentDir, file);
			Populate ();
		}
	}
}

