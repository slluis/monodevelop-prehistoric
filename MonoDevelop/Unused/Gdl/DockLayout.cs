using System;
using System.Collections;
using System.IO;
using System.Xml;
using Gtk;

namespace Gdl
{
	public class DockLayout
	{
		XmlDocument doc;
		ListStore itemsModel;
		ListStore layoutsModel;
		bool dirty = false;
		bool idleSavePending = false;

		Widget itemsUI, layoutsUI;
		DockMaster master = null;
		ArrayList layouts;

		public DockLayout (Dock dock)
		{
			layouts = new ArrayList ();
			this.Attach (dock.Master);
		}

		public Widget ItemsUI {
			get {
				if (itemsUI == null)
					itemsUI = ConstructItemsUI ();
				return itemsUI;
			}
		}

		public ArrayList Layouts {
			get { return layouts; }
		}

		public Widget LayoutsUI {
			get {
				if (layoutsUI == null)
					layoutsUI = ConstructLayoutsUI ();
				return layoutsUI;
			}
		}

		public DockMaster Master {
			get { return master; }
			set { master = value; }
		}

		public Widget UI {
			get { return ConstructUI ();}
		}

        // true if the layouts have changed and need to be saved to a file
		public bool IsDirty {
			get { return dirty; }
		}

		public void Attach (DockMaster master)
		{
			if (master == null)
				return;

			master.LayoutChanged -= OnLayoutChanged;

			if (itemsModel != null)
				itemsModel.Clear ();

			this.master = master;
			master.LayoutChanged += OnLayoutChanged;
			UpdateItemsModel ();
		}

		public void DeleteLayout (string name)
		{
			// dont allow deletion of default layout
			if (name == null || name == "__default__")
				return;

			XmlNode node = FindLayout (name);
			if (node != null) {
				doc.RemoveChild (node);
				dirty = true;
				// notify dirty
			}
		}

		public void Dump ()
		{
			XmlTextWriter writer = new XmlTextWriter (Console.Out);
			writer.Formatting = Formatting.Indented;
			doc.WriteTo (writer);
		}

		public bool LoadFromFile (string file)
		{
			if (doc != null) {
				doc = null;
				dirty = false;
				// notify dirty
			}

			if (File.Exists (file))
			{
				doc = new XmlDocument ();
				doc.Load (file);
				// minimum validation: test root element
				if (doc.SelectSingleNode ("/dock-layout") != null) {
					UpdateLayoutsModel ();
					return true;
				}
				else {
					doc = null;	
				}
			}

			return false;
		}

		public bool LoadLayout (string name)
		{
			if (doc == null || master == null)
				return false;

			if (name == null || name.Length < 1)
				name = "__default__";

			XmlNode node = FindLayout (name);
			if (node == null)
				node = FindLayout (null);
			
			if (node == null)
				return false;

			Load (node);
			return true;
		}

		public void RunManager ()
		{
			if (master == null)
				return;

			Widget container = ConstructUI ();
			if (container == null)
				return;

			Widget parent = master.Controller;
			if (parent != null)
				parent = parent.Toplevel;

			Dialog dialog = new Dialog ();
			dialog.Title = "Layout management";
			dialog.TransientFor = parent as Window;
			dialog.AddButton (Gtk.Stock.Close, Gtk.ResponseType.Close);
			dialog.SetDefaultSize (-1, 300);
			dialog.VBox.Add (container);
			dialog.Run ();
			dialog.Destroy ();
		}

		public void SaveLayout (string name)
		{
			if (master == null)
				return;

			if (doc == null)
				BuildDoc ();

			if (name == null || name.Length < 1)
				name = "__default__";

			// delete any previous node with the same name
			XmlNode node = FindLayout (name);
			if (node != null)
				doc.RemoveChild (node);

			// create the new node
			doc.CreateNode (XmlNodeType.Element, "layout", null);
			// FIXME:set name attribute to name

			// save the layout
			Save (node);
			dirty = true;
			// notify dirty
		}

		public bool SaveToFile (string file)
		{
			if (file == null)
				return false;

			// if there is still no xml doc, create an empty one
			if (doc == null)
				BuildDoc ();

			XmlTextWriter writer = new XmlTextWriter (file, System.Text.Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			doc.WriteTo (writer);
			dirty = false;
			// notify dirty
			return true;
		}

		void BuildModels ()
		{
			// NAME, SHOW, LOCKED, ITEM
			itemsModel = new ListStore (typeof (string), typeof (bool), typeof (bool), typeof (DockItem));
			itemsModel.SetSortColumnId (0, SortType.Ascending);
			layoutsModel = new ListStore (typeof (string), typeof (bool));
			layoutsModel.SetSortColumnId (0, SortType.Ascending);
		}

		void BuildDoc ()
		{
			doc = new XmlDocument ();
			doc.CreateXmlDeclaration ("1.0", null, null);
			doc.CreateNode (XmlNodeType.Element, "dock-layout", null);
		}

		XmlNode FindLayout (string name)
		{
			if (doc == null)
				return null;

			foreach (XmlNode n in doc.SelectNodes ("/dock-layout/layout"))
			{
				if (n.Attributes["name"].Value == name)
					return n;
			}

			return null;
		}

		void UpdateItemsModel ()
		{
			if (itemsModel == null || master == null)
				return;

			// build items list
			ArrayList items = new ArrayList ();
    		//gdl_dock_master_foreach (master, BuildList, out items);
			foreach (object o in master.DockObjects) {
				if (o is DockItem)
					items.Add (o);
			}

			TreeIter iter;
			// update items model data after a layout load
    		if (itemsModel.GetIterFirst (out iter)) {
				bool valid = true;
				while (valid) {
					DockItem item = itemsModel.GetValue (iter, 3) as DockItem;
					if (item != null) {
                		// look for the object in the items list
						foreach (DockItem di in items)
						{
                    		// found, update data
							if (item == di) {
								itemsModel.SetValue (iter, 0, item.Name);
								itemsModel.SetValue (iter, 1, item.IsAttached);
								itemsModel.SetValue (iter, 2, item.Locked);
							}

                    		// remove the item from the linked list and keep on walking the model
							items.Remove (di);
                    		valid = itemsModel.IterNext (ref iter);
						}
					}
					else {
                		// not a valid row
                		valid = itemsModel.Remove (ref iter);
					}
				}
			}

			// add any remaining objects
			foreach (DockItem ditem in items)
				itemsModel.AppendValues (ditem.Name, ditem.IsAttached, ditem.Locked, ditem);
		}

		void UpdateLayoutsModel ()
		{
			if (master == null || layoutsModel == null)
				return;

			// build layouts list
			layoutsModel.Clear ();
    		ArrayList items = this.Layouts;
			foreach (string s in items)
				layoutsModel.AppendValues (s, true);
		}

		Notebook ConstructUI ()
		{
			Notebook notebook = new Notebook ();
			notebook.Show ();

			Widget child;

			child = ConstructItemsUI ();
			if (child != null)
				notebook.AppendPage (child, new Label ("Items"));

			child = ConstructLayoutsUI ();
			if (child != null)
				notebook.AppendPage (child, new Label ("Layouts"));

			notebook.CurrentPage = 0;
			return notebook;
		}

		Widget ConstructItemsUI ()
		{
			Glade.XML gui = LoadInterface ("items_vbox");
			if (gui == null)
				return null;

			Gtk.VBox container = gui.GetWidget ("items_vbox") as VBox;
			Gtk.CheckButton locked_check = gui.GetWidget ("locked_check") as CheckButton;
			Gtk.TreeView items_list = gui.GetWidget ("items_list") as TreeView;

			locked_check.Toggled += AllLockedToggledCb;
			if (master != null) {
				//g_signal_connect (layout->master, "notify::locked", MasterLockedNotifyCb
				// force update now
				MasterLockedNotifyCb (master, null);
			}

			// set models
			items_list.Model = itemsModel;

			// construct list views
			CellRendererToggle renderer = new CellRendererToggle ();
			renderer.Toggled += ShowToggledCb;
			TreeViewColumn column = new TreeViewColumn ("Visible", renderer, "active", 1);
			items_list.AppendColumn (column);

			// connect signals
			container.Destroyed += LayoutUIDestroyed;

			return container;
		}

		Widget ConstructLayoutsUI ()
		{
			Glade.XML gui = LoadInterface ("layouts_vbox");

			if (gui == null)
				return null;

			Gtk.VBox container = gui.GetWidget ("layouts_vbox") as VBox;
			Gtk.TreeView layouts_list = gui.GetWidget ("layouts_list") as TreeView;
			layouts_list.Model = layoutsModel;
			CellRendererText renderer = new CellRendererText ();
			renderer.Edited += CellEditedCb;
			TreeViewColumn column = new TreeViewColumn ("Name", renderer, "text", 0, "editable", 1);
			layouts_list.AppendColumn (column);

			container.Destroyed += LayoutUIDestroyed;
			return container;
		}

		Glade.XML LoadInterface (string topWidget)
		{
			return new Glade.XML (null, "layout.glade", topWidget, null);
		}

		DockObject SetupObject (DockMaster master, XmlNode node)
		{
			return null;
		}

		void RecursiveBuild (XmlNode parentNode, DockObject parent)
		{
		}

		void ForeachDetach (DockObject obj)
		{
			obj.Detach (true);
		}

		void ForeachToplevelDetach (DockObject obj)
		{
			//((Container)obj).Foreach (ForeachDetach);
		}

		void Load (XmlNode node)
		{
			if (node == null)
				return;

			// start by detaching all items from the toplevels
			//gdl_dock_master_foreach_toplevel (master, TRUE, (GFunc) gdl_dock_layout_foreach_toplevel_detach, NULL);

			RecursiveBuild (node, null);
		}

		void ForeachObjectSave (DockObject obj)
		{
		}

		void AddPlaceholder (DockObject obj, Hashtable placeholders)
		{
			if (obj is DockPlaceholder) {
			}
		}

		void Save (XmlNode node)
		{
		}

		bool IdleSave ()
		{
			//SaveLayout (this);
			idleSavePending = false;
			return false;
		}

		void OnLayoutChanged (object sender, EventArgs a)
		{
			UpdateItemsModel ();

			if (!idleSavePending) {
				GLib.Idle.Add (IdleSave);
				idleSavePending = true;
			}
		}

		void LoadLayoutCb (object sender, EventArgs a)
		{
		}

		void DeleteLayoutCb (object sender, EventArgs a)
		{
		}

		void ShowToggledCb (object sender, EventArgs a)
		{
		}

		void AllLockedToggledCb (object sender, EventArgs a)
		{
		}

		void LayoutUIDestroyed (object sender, EventArgs a)
		{
		}

		void MasterLockedNotifyCb (object sender, EventArgs a)
		{
		}

		void CellEditedCb (object sender, EventArgs a)
		{
		}
	}
}

