using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using Gtk;

namespace Gdl
{
	public class DockLayout
	{
		XmlDocument doc;
		XmlNode rootNode;

		ListStore itemsModel;
		ListStore layoutsModel;

		bool dirty = false;
		bool idleSavePending = false;

		Widget itemsUI, layoutsUI;
		DockMaster master = null;
		ArrayList layouts;
		Hashtable placeholders;

		CheckButton locked_check;

		public DockLayout (Dock dock)
		{
			layouts = new ArrayList ();
			this.Attach (dock.Master);
			BuildModels ();
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

		private XmlNode RootNode {
			get {
				if (rootNode == null && doc != null)
					rootNode = doc.SelectSingleNode ("/dock-layout");
				return rootNode;
			}
		}

		private Hashtable Placeholders {
			get {
				if (placeholders == null)
					placeholders = new Hashtable ();
				return placeholders;
			}
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
				this.RootNode.RemoveChild (node);
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
				Stream s = File.OpenRead (file);
				doc.Load (s);
				s.Close ();
				// minimum validation: test root element
				if (this.RootNode != null) {
					// FIXME: I cheated here
					foreach (XmlNode n in this.RootNode.ChildNodes)
					{
						if (n.Name == "layout")
							layouts.Add (n.Attributes["name"].Value);
					}
					UpdateLayoutsModel ();
					// FIXME: for testing load the default
					return LoadLayout (null);
					// return true;
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
				this.RootNode.RemoveChild (node);

			// create the new node
			XmlElement element = doc.CreateElement ("layout");
			element.SetAttribute ("name", name);
			this.RootNode.AppendChild (element);

			// save the layout
			Save (element);
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
			writer.Flush ();
			writer.Close ();
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
			doc.AppendChild (doc.CreateElement ("dock-layout"));
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
			foreach (object o in master.DockObjects) {
				if (o is DockItem)
					items.Add (o);
			}

			TreeIter iter;
			// update items model data after a layout load
    		if (itemsModel.GetIterFirst (out iter)) {
				bool valid = true;
			walk_start:
				while (valid) {
					DockItem item = itemsModel.GetValue (iter, 3) as DockItem;
					if (item != null) {
                		// look for the object in the items list
						foreach (DockItem di in items)
						{
                    		// found, update data
							if (item == di) {
								UpdateItemData (iter, item);
								items.Remove (di);
								valid = itemsModel.IterNext (ref iter);
								goto walk_start;
							}
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

		void UpdateItemData (TreeIter iter, DockItem item)
		{
			itemsModel.SetValue (iter, 0, item.Name);
			itemsModel.SetValue (iter, 1, item.IsAttached);
			itemsModel.SetValue (iter, 2, item.Locked);
		}

		void UpdateLayoutsModel ()
		{
			if (master == null || layoutsModel == null)
				return;

			// build layouts list
			layoutsModel.Clear ();
			foreach (string s in this.Layouts) {
				if (s != "__default__")
					layoutsModel.AppendValues (s, true);
			}
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
			locked_check = gui.GetWidget ("locked_check") as CheckButton;
			Gtk.TreeView items_list = gui.GetWidget ("items_list") as TreeView;

			locked_check.Toggled += AllLockedToggledCb;
			if (master != null) {
				master.NotifyLocked += MasterLockedNotifyCb;
				// force update now
				MasterLockedNotifyCb (master, EventArgs.Empty);
			}

			// set models
			items_list.Model = itemsModel;

			// construct list views
			CellRendererToggle renderer = new CellRendererToggle ();
			renderer.Toggled += ShowToggledCb;
			TreeViewColumn column = new TreeViewColumn ("Visible", renderer, "active", 1);
			items_list.AppendColumn (column);

			items_list.AppendColumn ("Item", new CellRendererText (), "text", 0);

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

			return container;
		}

		Glade.XML LoadInterface (string topWidget)
		{
			return new Glade.XML (null, "layout.glade", topWidget, null);
		}

		DockObject SetupObject (XmlNode node)
		{
			DockObject obj = null;
			// FIXME: notebooks don't get names ...
			if (node.Name == "notebook") {
				DockNotebook dn = new DockNotebook ();
				dn.Bind (master);
				dn.FromXml (node);
				return dn;
			}
			// FIXME: paned don't get names ...
			if (node.Name == "paned") {
				DockPaned dp = new DockPaned ();
				dp.Bind (master);
				dp.FromXml (node);
				return dp;
			}

			string name = node.Attributes["name"].Value;

			if (name != null && name.Length > 0) {
				obj = master.GetObject (name);
			}
			else {
				Console.WriteLine ("While loading layout: don't know how to create a dock object whose nick is '{0}'", name);
			}

			// FIXME: all sorts of unserialization stuff
			if (obj != null)
				obj.FromXml (node);

			return obj;
		}

		// this appears to create objects from the xml
		void RecursiveBuild (XmlNode parentNode, DockObject parent)
		{
			//Console.WriteLine ("RecursiveBuild: {0}, {1}", parentNode.Name, parent);
			if (master == null || parentNode == null)
				return;

			DockObject obj;

			// if parent is null, we should build toplevels
			//if (parent == null)
			//	parent = master.TopLevelDocks[0] as DockObject;

			foreach (XmlNode node in parentNode.ChildNodes)
			{
				obj = SetupObject (node);
				if (obj != null) {
					obj.Freeze ();

					// recurse here to catch placeholders
					RecursiveBuild (node, obj);

					// placeholders are later attached to the parent
					if (obj is DockPlaceholder)
						obj.Detach (false);

					// apply "after" parameters
					// FIXME:

					// add the object to the parent
					if (parent != null) {
						if (obj is DockPlaceholder) {
							((DockPlaceholder) obj).Attach (parent);
						}
						else if (parent.IsCompound) {
							parent.Add (obj);
							if (parent.Visible)
								obj.Show ();
						}
					}
					else {
						if (master.Controller != obj && master.Controller.Visible)
							obj.Show ();
					}
					
					// call reduce just in case child is missing
					if (obj.IsCompound)
						obj.Reduce ();

					obj.Thaw ();
				}
			}
		}

		void ForeachDetach (DockObject obj)
		{
			obj.Detach (true);
		}

		void ForeachToplevelDetach (DockObject obj)
		{
			DockObject child;
			foreach (Widget w in obj.Children) {
				child = w as DockObject;
				if (w != null)
					ForeachDetach (child);
			}
		}

		void Load (XmlNode node)
		{
			if (node == null)
				return;

			// start by detaching all items from the toplevels
			foreach (DockObject o in master.TopLevelDocks)
				ForeachToplevelDetach (o);

			RecursiveBuild (node, null);
		}

		string GetXmlName (Type t)
		{
			switch (t.ToString ()) {
				case "Gdl.Dock":
					return "dock";
				case "Gdl.DockItem":
					return "item";
				case "Gdl.DockNotebook":
					return "notebook";
				case "Gdl.DockPaned":
					return "paned";
				default:
					return "object";
			}
		}

		void ForeachObjectSave (DockObject obj, XmlNode parent)
		{
			if (obj == null)
				return;

			XmlElement element = doc.CreateElement (GetXmlName (obj.GetType ()));

			// get object exported attributes
			ArrayList exported = new ArrayList ();
			PropertyInfo[] props = obj.GetType ().GetProperties (BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo p in props) {
				if (p.IsDefined (typeof (ExportLayoutAttribute), true))
					exported.Add (p);
			}

			foreach (PropertyInfo p in exported)
				element.SetAttribute (p.Name.ToLower (), p.GetValue (obj, null).ToString ());

			parent.AppendChild (element);

			// save placeholders for the object
			if (!(obj is DockPlaceholder)) {
				//object list = this.Placeholders[obj];
				//foreach (DockObject child in list)
				//	ForeachObjectSave (child);
			}

			// recurse the object if appropriate
			if (obj.IsCompound) {
				DockObject child;
				foreach (Widget w in obj.Children)
				{
					child = w as DockObject;
					if (child != null)
						ForeachObjectSave (child, element);
				}
			}
		}

		void AddPlaceholder (DockObject obj, Hashtable placeholders)
		{
			if (obj is DockPlaceholder) {
				// FIXME:
				// add the current placeholder to the list of placeholders for that host
			}
		}

		void Save (XmlNode node)
		{
			// FIXME: implement this?
			// build the placeholder's hash: the hash keeps lists of
			// placeholders associated to each object, so that we can save the
			// placeholders when we are saving the object (since placeholders
			// don't show up in the normal widget hierarchy)

			// save the layout recursively
			foreach (DockObject o in master.TopLevelDocks)
				ForeachObjectSave (o, node);
		}

		bool IdleSave ()
		{
			SaveLayout (null);
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
			TreeModel model;
			TreeIter iter;

			if (((TreeView) sender).Selection.GetSelected (out model, out iter))
				LoadLayout ((string) model.GetValue (iter, 0));
		}

		void DeleteLayoutCb (object sender, EventArgs a)
		{
			TreeModel model;
			TreeIter iter;

			if (((TreeView) sender).Selection.GetSelected (out model, out iter)) {
				DeleteLayout ((string) model.GetValue (iter, 0));
				((ListStore)model).Remove (ref iter);
			}
		}

		void ShowToggledCb (object sender, ToggledArgs a)
		{
			TreeIter iter;
			if (itemsModel.GetIterFromString (out iter, a.Path)) {
				bool show = (bool) itemsModel.GetValue (iter, 1);
				DockItem item = itemsModel.GetValue (iter, 3) as DockItem;
				if (show)
					item.ShowItem ();
				else
					item.HideItem ();
			}
		}

		void AllLockedToggledCb (object sender, EventArgs a)
		{
			if (master != null)
				master.Locked = ((CheckButton) sender).Active ? 1 : 0;
		}

		void MasterLockedNotifyCb (object sender, EventArgs a)
		{
			if (master.Locked == -1) {
				locked_check.Inconsistent = true;
			}
			else {
				locked_check.Inconsistent = false;
				locked_check.Active = (master.Locked == 1);
			}
		}

		void CellEditedCb (object sender, EditedArgs a)
		{
			TreeIter iter;
			layoutsModel.GetIterFromString (out iter, a.Path);
			string name = (string) layoutsModel.GetValue (iter, 0);

			XmlNode node = FindLayout (name);
			if (node == null)
				return;
			node.Attributes["name"].Value = a.NewText;

			layoutsModel.SetValue (iter, 0, a.NewText);
			layoutsModel.SetValue (iter, 1, true);

			SaveLayout (a.NewText);
		}
	}
}

