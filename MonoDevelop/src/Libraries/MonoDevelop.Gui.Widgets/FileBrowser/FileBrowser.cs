//
// Author: John Luke  <jluke@cfl.rr.com>
// License: LGPL
//

using System;
using System.IO;
using Gtk;
using GtkSharp;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace MonoDevelop.Gui.Widgets
{
	public class FileBrowser : ScrolledWindow
	{
		private static GLib.GType gtype;
		private Gtk.TreeView tv;
		private ListStore store;
		private string currentDir;
		private bool ignoreHidden = true;
		private string[] files;
		private bool init = false;
		PropertyService PropertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));

		public FileBrowser () : base (GType)
		{
			this.VscrollbarPolicy = PolicyType.Automatic;
			this.HscrollbarPolicy = PolicyType.Automatic;

			IProperties p = (IProperties) PropertyService.GetProperty ("SharpDevelop.UI.SelectStyleOptions", new DefaultProperties ());
			ignoreHidden = !p.GetProperty ("ICSharpCode.SharpDevelop.Gui.FileScout.ShowHidden", false);

			tv = new Gtk.TreeView ();
			tv.RulesHint = true;
			tv.AppendColumn ("Directories", new CellRendererText (), "text", 0);
			store = new ListStore (typeof (string));
			currentDir = Environment.GetEnvironmentVariable ("HOME");
			Populate ();
			tv.Model = store;

			tv.RowActivated += new RowActivatedHandler (OnRowActivated);
			tv.Selection.Changed += new EventHandler (OnSelectionChanged);

			this.Add (tv);
			this.ShowAll ();
			init = true;
		}

		// jba 23 feb 04 - reneabled this property to make the file browser obey the
		// ignoreHidden property from the visual style option panel
		public bool IgnoreHidden
		{
			get { return ignoreHidden; }
			set {
				/* for some reasont his code crashes (NullReferenceException on the Populate() call
				if (ignoreHidden != value) {
					ignoreHidden = value; 
					// redraw folder list
					System.Console.WriteLine("before poplate call");
					Populate ();
				}
				*/
				
				ignoreHidden = value;
			}
		}

		public Gtk.TreeView TreeView
		{
			get { return tv; }
		}

		public string CurrentDir
		{
			get { return System.IO.Path.GetFullPath (currentDir); }
			set { currentDir = value;
				  Populate (); }
		}

		public string[] Files
		{
			get {
				if (files == null) {
					return new string [0];
				}
				return files; 
			}
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
			store.AppendValues (".");

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
			if (init == true)
				tv.Selection.SelectPath (new Gtk.TreePath ("0"));
		}

		private void OnSelectionChanged (object o, EventArgs args)
		{
			TreeIter iter;
			TreeModel model;
			if (tv.Selection.GetSelected (out model, out iter))
			{
				string selection = (string) store.GetValue (iter, 0);
				files = Directory.GetFiles (System.IO.Path.Combine (currentDir, selection));
			}
		}

		private void OnRowActivated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			store.GetIter (out iter, args.Path);
			string file = (string) store.GetValue (iter, 0);
			string newDir = System.IO.Path.Combine (currentDir, file);
			if (Directory.Exists (newDir))
			{
				currentDir = newDir;
				Populate ();
			}
		}
	}
}

