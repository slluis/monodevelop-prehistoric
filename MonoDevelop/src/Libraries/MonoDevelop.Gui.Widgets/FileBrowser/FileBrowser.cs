//
// Author: John Luke  <jluke@cfl.rr.com>
// License: LGPL
//

using System;
using System.Diagnostics;
using System.IO;
using Gtk;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using MonoDevelop.Gui.Utils;

namespace MonoDevelop.Gui.Widgets
{
	public delegate void DirectoryChangedEventHandler (string path);

	public class FileBrowser : VBox
	{
		public DirectoryChangedEventHandler DirectoryChangedEvent;
		private static GLib.GType gtype;
		private Gtk.TreeView tv;
		private Button upbutton;
		private Entry entry;
		private ListStore store;
		private string currentDir;
		private bool ignoreHidden = true;
		private string[] files;
		private bool init = false;
		PropertyService PropertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));

		public FileBrowser () : base (GType)
		{
			if (!Vfs.Initialized) {
				Vfs.Init();
			}

			ScrolledWindow scrolledwindow = new ScrolledWindow ();
			scrolledwindow.VscrollbarPolicy = PolicyType.Automatic;
			scrolledwindow.HscrollbarPolicy = PolicyType.Automatic;

			Button homebutton = new Button ();
			homebutton.Add (new Image (Stock.Home, IconSize.SmallToolbar));
			homebutton.Relief = ReliefStyle.None;
			homebutton.Clicked += new EventHandler (OnHomeClicked);

			upbutton = new Button ();
			upbutton.Add (new Image (Stock.GoUp, IconSize.SmallToolbar));
			upbutton.Relief = ReliefStyle.None;
			upbutton.Clicked += new EventHandler (OnUpClicked);

			entry = new Entry ();
			entry.Activated += new EventHandler (OnEntryActivated);

			HBox buttonbox = new HBox (false, 0);
			buttonbox.PackStart (upbutton, false, false, 0);
			buttonbox.PackStart (homebutton, false, false, 0);
			buttonbox.PackStart (entry, true, true, 0);

			IProperties p = (IProperties) PropertyService.GetProperty ("SharpDevelop.UI.SelectStyleOptions", new DefaultProperties ());
			ignoreHidden = !p.GetProperty ("ICSharpCode.SharpDevelop.Gui.FileScout.ShowHidden", false);

			tv = new Gtk.TreeView ();
			tv.RulesHint = true;

			TreeViewColumn directorycolumn = new TreeViewColumn ();
			directorycolumn.Title = "Directories";
			
			CellRendererPixbuf pix_render = new CellRendererPixbuf ();
			directorycolumn.PackStart (pix_render, false);
			directorycolumn.AddAttribute (pix_render, "pixbuf", 0);

			CellRendererText text_render = new CellRendererText ();
			directorycolumn.PackStart (text_render, false);
			directorycolumn.AddAttribute (text_render, "text", 1);
			
			tv.AppendColumn (directorycolumn);

			store = new ListStore (typeof (Gdk.Pixbuf), typeof (string));
			CurrentDir = Environment.GetEnvironmentVariable ("HOME");
			tv.Model = store;

			tv.RowActivated += new RowActivatedHandler (OnRowActivated);
			tv.ButtonReleaseEvent += new ButtonReleaseEventHandler (OnButtonRelease);			
			tv.PopupMenu += new PopupMenuHandler (OnPopupMenu);			

			scrolledwindow.Add (tv);
			this.Homogeneous = false;
			this.PackStart (buttonbox, false, false, 0);
			this.PackStart (scrolledwindow);
			this.ShowAll ();
			init = true;
		}

		// FIXME: we should watch the PropertyChanged event instead
		// of exposing the set part
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

		public string CurrentDir
		{
			get { return System.IO.Path.GetFullPath (currentDir); }
			set { 
					currentDir = System.IO.Path.GetFullPath (value);
					Populate ();

					if (DirectoryChangedEvent != null) {
						DirectoryChangedEvent(CurrentDir);
					}
				}
		}

		public string[] Files
		{
			get
			{
				if (files == null)
					return new string [0];
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

		public void SelectFirst ()
		{
			tv.Selection.SelectPath (new TreePath ("0"));
		}

		void Populate ()
		{
			store.Clear ();

			// FIXME: never turns back on
			if (System.IO.Path.GetPathRoot (CurrentDir) == CurrentDir)
				upbutton.Sensitive = false;
			else if (upbutton.Sensitive == false)
				upbutton.Sensitive = true;

			DirectoryInfo di = new DirectoryInfo (CurrentDir);
			DirectoryInfo[] dirs = di.GetDirectories ();
			
			foreach (DirectoryInfo d in dirs)
			{
				if (ignoreHidden)
				{
					if (!d.Name.StartsWith ("."))
						store.AppendValues (FileIconLoader.GetPixbufForFile (System.IO.Path.Combine (CurrentDir, d.Name), 24, 24), d.Name);
				}
				else
				{
					store.AppendValues (FileIconLoader.GetPixbufForFile (System.IO.Path.Combine (CurrentDir, d.Name), 24, 24), d.Name);
				}
			}

			if (init == true)
				tv.Selection.SelectPath (new Gtk.TreePath ("0"));

			entry.Text = CurrentDir;
			files = Directory.GetFiles (CurrentDir);
		}

		private void OnSelectionChanged (object o, EventArgs args)
		{
			TreeIter iter;
			TreeModel model;
			if (tv.Selection.GetSelected (out model, out iter))
			{
				string selection = (string) store.GetValue (iter, 1);
				files = Directory.GetFiles (System.IO.Path.Combine (currentDir, selection));
			}
		}

		private void OnRowActivated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			store.GetIter (out iter, args.Path);
			string file = (string) store.GetValue (iter, 1);
			string newDir = System.IO.Path.Combine (currentDir, file);

			if (Directory.Exists (newDir))
			{
				CurrentDir = newDir;
			}
		}
		
		private void OnButtonRelease (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 3)
			{
				ShowPopup ();
			}	
		}

		private void OnPopupMenu (object o, PopupMenuArgs args)
		{
			ShowPopup ();
		}

		private void ShowPopup ()
		{
			 Menu menu = new Menu ();
			 MenuItem openfilebrowser = new MenuItem ("Open with file browser");
			 openfilebrowser.Activated += new EventHandler (OpenFileBrowser);

			 MenuItem openterminal = new MenuItem ("Open with terminal");
			 openterminal.Activated += new EventHandler (OpenTerminal);

			 menu.Append (openterminal);
			 menu.Append (openfilebrowser);
			 menu.Popup (null, null, null, IntPtr.Zero, 3, Global.CurrentEventTime);
			 menu.ShowAll ();
		}
		
		private void OpenFileBrowser (object o, EventArgs args)
		{
			TreeIter iter;
			TreeModel model;
			// FIXME: look in GConf for the settings
			// but strangely there is not one
			string commandline = "nautilus \"";

			if (tv.Selection.GetSelected (out model, out iter))
			{
				string selection = (string) store.GetValue (iter, 1);
				commandline += System.IO.Path.Combine (currentDir, selection) + "\"";
				Process.Start (commandline);
			}
		}
		
		private void OpenTerminal(object o, EventArgs args)
		{
			TreeIter iter;
			TreeModel model;
			// FIXME: look in GConf for the settings
			// but the args will be terminal dependent
			// leaving as hardcoded for now
			string commandline = "gnome-terminal --working-directory=\"";
			if (tv.Selection.GetSelected (out model, out iter))
			{
				string selection = (string) store.GetValue (iter, 1);
				commandline += System.IO.Path.Combine (currentDir, selection) + "\"";
				Process.Start (commandline);
			}
		}
		
		private void OnUpClicked (object o, EventArgs args)
		{
			if (System.IO.Path.GetPathRoot (CurrentDir) != CurrentDir)
				CurrentDir = System.IO.Path.Combine (CurrentDir, "..");
		}
		
		private void OnHomeClicked (object o, EventArgs args)
		{
			CurrentDir = Environment.GetEnvironmentVariable ("HOME");
		}

		private void OnEntryActivated (object sender, EventArgs args)
		{
			if (Directory.Exists (entry.Text.Trim ()))
				CurrentDir = entry.Text;
		}
	}
}

