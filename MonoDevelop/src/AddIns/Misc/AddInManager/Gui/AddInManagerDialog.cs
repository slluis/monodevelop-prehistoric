using System;
using Gtk;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Gui;
using MonoDevelop.Services;

namespace AddInManager
{
	public class AddInManagerDialog : Dialog
	{
		public AddInManagerDialog ()
		{
			this.BorderWidth = 12;
			this.Title = GettextCatalog.GetString ("AddInManager");
			this.TransientFor = (Window) WorkbenchSingleton.Workbench;
			this.SetDefaultSize (300, 250);

			ScrolledWindow sw = new ScrolledWindow ();
			TreeView tv = new TreeView ();
			tv.AppendColumn (GettextCatalog.GetString ("Enabled"), new CellRendererToggle (), "active", 0);
			tv.AppendColumn (GettextCatalog.GetString ("Title"), new CellRendererText (), "text", 1);
			tv.AppendColumn (GettextCatalog.GetString ("Version"), new CellRendererText (), "text", 2);
			sw.Add (tv);

			this.AddButton (Gtk.Stock.Close, ResponseType.Close);
	
			tv.Model = LoadAddIns ();
			this.VBox.Add (sw);
			this.ShowAll ();
		}

		TreeStore LoadAddIns ()
		{
			TreeStore store = new TreeStore (typeof (bool), typeof (string), typeof (string));
			AddInCollection addins = AddInTreeSingleton.AddInTree.AddIns;

			foreach (AddIn a in addins)
			{
				store.AppendValues (true, a.Name, a.Version);
			}
		
			return store;
		}
	}
}

