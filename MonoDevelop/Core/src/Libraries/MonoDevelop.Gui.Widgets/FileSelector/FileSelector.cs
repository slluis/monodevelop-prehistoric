using System;
using System.IO;
using Gtk;

using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Gui.Widgets
{
	// basically just to remember the last directory
	// we could do some if GTK2.4 then use new FileChooser
	// but that is probably to be hacky at best
	public class FileSelector : FileChooserDialog
	{
		const string LastPathProperty = "MonoDevelop.FileSelector.LastPath";
		string lastPath;
		PropertyService propertyService = (PropertyService) ServiceManager.GetService (typeof (PropertyService));

		public FileSelector () : base (GettextCatalog.GetString ("Open file ..."), null, FileChooserAction.Open)
		{
			AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
			AddButton (Gtk.Stock.Open, ResponseType.Ok);
			CommonSetup ();
		}

		public FileSelector (string title) : base (title, null, FileChooserAction.Open)
		{
			AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
			AddButton (Gtk.Stock.Open, ResponseType.Ok);
			CommonSetup ();
		}

		public FileSelector (string title, FileChooserAction action) : base (title, null, action)
		{
			if (action == FileChooserAction.SelectFolder) {
				AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
				AddButton ("Select Folder", ResponseType.Ok);
			} else if (action == FileChooserAction.Save) {
				AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
				AddButton (Gtk.Stock.Save, ResponseType.Ok);
			}
			CommonSetup ();
		}

		void CommonSetup ()
		{
			// Restore the last active directory
			string tmp = (string) propertyService.GetProperty (LastPathProperty);
			if (tmp != null && tmp.Length > 0)
			{
				if (tmp.EndsWith ("/"))
					lastPath = String.Format ("{0}", tmp.Trim ());
				else
					lastPath = String.Format ("{0}/", tmp.Trim ());
			}
			else
			{
				// FIXME: use ~/DefaultPath?
				lastPath = Environment.GetEnvironmentVariable ("HOME");
			}

			// Set the dir here, must end in "/" to work right
			this.SetFilename (lastPath);

			// Basically need to track if the directory has
			// been changed in the simplest way possible
			// I think that this always changes when the dir does
			this.CurrentFolderChanged += OnOptionListChanged;
		}

		void OnOptionListChanged (object o, EventArgs args)
		{
			UpdateLastDir ();
		}

		void UpdateLastDir ()
		{
			if (this.Filename == null || this.Filename == "")
				return;
			
			if (this.Filename.EndsWith ("/") || Directory.Exists (this.Filename))
				lastPath = this.Filename;
			else
				lastPath = System.IO.Path.GetDirectoryName (this.Filename);
		
			// Console.WriteLine ("storing: {0}", lastPath);
			// FIXME: find a way to only set this once per-dialog
			propertyService.SetProperty (LastPathProperty, lastPath);
		}
	}
}

