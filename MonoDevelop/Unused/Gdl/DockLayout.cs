using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Gtk;

namespace Gdl
{
	public class DockLayout
	{
		DockMaster master;
		Widget itemsui;
		Widget layoutsui;
		ArrayList layouts;
		bool dirty;
		XmlDocument doc;

		public DockLayout (Dock dock)
		{
			layouts = new ArrayList ();
			this.Attach (dock.Master);
		}
		
		public DockMaster Master {
			get { return master; }
			set { master = value; }
		}
		
		public bool IsDirty {
			get {
				return dirty;
			}
		}
		
		public Widget ItemsUi { 
			get {
				return itemsui;
			}
		}
		
		public Widget LayoutsUi { 
			get {
				return layoutsui;
			}
		}

		public string[] Layouts {
			get {
				return layouts.ToArray (typeof (string)) as string[];
			}
		}
		
		public void Attach (DockMaster master)
		{
			if (this.master != null)
				master.LayoutChanged -= OnLayoutChanged;

			this.master = master;
			master.LayoutChanged += OnLayoutChanged;
		}
		
		public void DeleteLayout (string name)
		{
		}
	
		public void LoadLayout (string newLayout)
		{
		}
		
		public void LoadFromFile (string configFile)
		{
			doc = new XmlDocument ();
			doc.Load (configFile);
			XmlNodeList nodes = doc.SelectNodes ("/dock-layout/layout");
			foreach (XmlNode n in nodes)
				LoadLayout (n);
		}

		void LoadLayout (XmlNode node)
		{
			layouts.Add (node.Attributes["name"].Value);
			LoadDock (node["dock"]);
		}

		void LoadDock (XmlNode node)
		{
			Dock dock = new Dock ();
			foreach (XmlNode child in node.ChildNodes)
			{
				switch (child.Name) {
					case "notebook":
						LoadNotebook (child);
						break;
					default:
						Console.WriteLine (child.Name);
						break;
				}
			}	
		}

		void LoadNotebook (XmlNode node)
		{
			DockNotebook notebook = new DockNotebook ();
			notebook.Orientation = node.Attributes ["orientation"].Value == "vertical" ? Orientation.Vertical : Orientation.Horizontal;
			notebook.Page = int.Parse (node.Attributes ["page"].Value);

			foreach (XmlNode child in node.ChildNodes)
			{
				switch (child.Name) {
					case "item":
						LoadItem (child);
						break;
					default:
						Console.WriteLine (child.Name);
						break;
				}
			}	
		}

		void LoadItem (XmlNode node)
		{
			string name = node.Attributes ["name"].Value;
			string locked = node.Attributes ["locked"].Value;
			DockItem item = new DockItem (name, name, DockItemBehavior.Normal);
			item.Orientation = node.Attributes ["orientation"].Value == "vertical" ? Orientation.Vertical : Orientation.Horizontal;
		}
		
		public void RunManager ()
		{
		}
		
		public void SaveLayout (string currentLayout)
		{
		}
		
		public void SaveToFile (string file)
		{
			XmlTextWriter writer = new XmlTextWriter (file, System.Text.Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			doc.WriteTo (writer);
		}

		public void Dump ()
		{
			XmlTextWriter writer = new XmlTextWriter (Console.Out);
			writer.Formatting = Formatting.Indented;
			doc.WriteTo (writer);
		}

		void OnLayoutChanged (object sender, EventArgs a)
		{
			Console.WriteLine ("layout changed");
		}
	}
}

