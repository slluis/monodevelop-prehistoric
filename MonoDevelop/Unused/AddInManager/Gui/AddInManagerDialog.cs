using System;
using Gtk;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Gui;
using MonoDevelop.Services;

namespace AddInManager
{
	public class AddInManagerDialog : Dialog
	{
		TreeStore store;

		public AddInManagerDialog ()
		{
			this.BorderWidth = 12;
			this.Title = GettextCatalog.GetString ("AddInManager");
			this.TransientFor = (Window) WorkbenchSingleton.Workbench;
			this.SetDefaultSize (400, 350);

			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.In;
			TreeView tv = new TreeView ();
			tv.RowActivated += new RowActivatedHandler (OnRowActivated);

			CellRendererToggle toggle = new CellRendererToggle ();
			toggle.Toggled += OnCellToggled;
			tv.AppendColumn (GettextCatalog.GetString ("Enabled"), toggle, "active", 0);
			tv.AppendColumn (GettextCatalog.GetString ("AddIn Name"), new CellRendererText (), "text", 1);
			tv.AppendColumn (GettextCatalog.GetString ("Version"), new CellRendererText (), "text", 2);
			sw.Add (tv);

			this.AddButton (Gtk.Stock.Close, ResponseType.Close);
	
			LoadAddIns ();
			tv.Model = store;
			this.VBox.Add (sw);
			this.ShowAll ();
		}

		void LoadAddIns ()
		{
			store = new TreeStore (typeof (bool), typeof (string), typeof (string));
			AddInCollection addins = AddInTreeSingleton.AddInTree.AddIns;

			foreach (AddIn a in addins)
				store.AppendValues (true, a.Name, a.Version);
		}

		void OnCellToggled (object sender, ToggledArgs a)
		{
			TreeIter iter;
			if (store.GetIterFromString (out iter, a.Path))
				Toggle (iter);
		}

		void OnRowActivated (object sender, RowActivatedArgs a)
		{
			TreeIter iter;
			if (store.GetIter (out iter, a.Path))
				Toggle (iter);
		}

		void Toggle (TreeIter iter)
		{
			bool val = (bool) store.GetValue (iter, 0);
			store.SetValue (iter, 0, !val);
		}
	}
}

