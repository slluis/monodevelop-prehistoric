using System;
using Gtk;

using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Gui.Widgets
{
	// basically just to remember the last directory
	// we could do some if GTK2.4 then use new FileChooser
	// but that is probably to be hacky at best
	public class FileSelector : FileSelection
	{
		const string LastPathProperty = "MonoDevelop.FileSelector.LastPath";
		string lastPath;
		PropertyService propertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));

		public FileSelector () : base (GettextCatalog.GetString ("Open file ..."))
		{
			CommonSetup ();
		}

		public FileSelector (string title) : base (title)
		{
			CommonSetup ();
		}

		void CommonSetup ()
		{
			// Restore the last active directory
			string tmp = (string) propertyService.GetProperty (LastPathProperty);
			if (tmp != null && tmp.Length > 0)
			{
				int start = tmp.IndexOf (':') + 1;
				lastPath = tmp.Substring (start).Trim ();
			}
			else
			{
				// FIXME: use ~/DefaultPath
				lastPath = Environment.GetEnvironmentVariable ("HOME");
			}

			// FIXME: surely there is a better way to set the dir
			this.Complete (lastPath);

			// Basically need to track if the directory has
			// been changed in the simplest way possible
			this.DirList.RowActivated += OnDirectoryChanged;
			this.HistoryPulldown.Changed += OnOptionChanged;
		}

		void OnDirectoryChanged (object o, RowActivatedArgs args)
		{
			UpdateLastDir ();
		}

		void OnOptionChanged (object o, EventArgs args)
		{
			UpdateLastDir ();
		}

		void UpdateLastDir ()
		{
			lastPath = this.SelectionText.Text;
			propertyService.SetProperty (LastPathProperty, lastPath);
		}
	}
}

