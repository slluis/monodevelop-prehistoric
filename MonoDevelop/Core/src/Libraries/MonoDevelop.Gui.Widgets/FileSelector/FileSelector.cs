using System;
using Gtk;

using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Gui.Widgets
{
	public class FileSelector : FileChooserDialog
	{
		const string LastPathProperty = "MonoDevelop.FileSelector.LastPath";
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
			switch (action) {
				case FileChooserAction.SelectFolder:
					AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
					AddButton ("Select Folder", ResponseType.Ok);
					break;
				case FileChooserAction.Save:
					AddButton (Gtk.Stock.Cancel, ResponseType.Cancel);
					AddButton (Gtk.Stock.Save, ResponseType.Ok);
					break;
				default:
					break;
			}

			CommonSetup ();
		}

		void CommonSetup ()
		{
			// Restore the last active directory
			string last = (string) propertyService.GetProperty (LastPathProperty);
			if (last != null && last.Length > 0)
				this.SetCurrentFolder (last);
			else
				this.SetCurrentFolder (Environment.GetFolderPath (Environment.SpecialFolder.Personal));

			// add ~/Projects as a MD bookmark
			this.AddShortcutFolder (System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Projects"));

			// FIXME: only set this once per-dialog
			// perhaps in Dispose ()? or only when a file or dir is selected
			this.CurrentFolderChanged += OnCurrentFolderChanged;
		}

		void OnCurrentFolderChanged (object o, EventArgs args)
		{
			propertyService.SetProperty (LastPathProperty, this.CurrentFolder);
		}
	}
}

