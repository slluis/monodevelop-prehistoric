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
			this.SetDefaultSize (300, 250);

			ScrolledWindow sw = new ScrolledWindow ();
			TreeView tv = new TreeView ();

			CellRendererToggle toggle = new CellRendererToggle ();
			toggle.Toggled += OnCellToggled;
			tv.AppendColumn (GettextCatalog.GetString ("Enabled"), toggle, "active", 0);
			tv.AppendColumn (GettextCatalog.GetString ("Title"), new CellRendererText (), "text", 1);
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
			{
				store.AppendValues (true, a.Name, a.Version);
			}
		}

		void OnCellToggled (object o, ToggledArgs args)
		{
			CellRendererToggle toggle = (CellRendererToggle) o;
                                                                                
                        TreeIter iter;
                        if (store.GetIterFromString(out iter, args.Path))
                        {
                                bool val = (bool) store.GetValue(iter, 0);
                                store.SetValue (iter, 0, !val);
                        }

		}
	}
}

