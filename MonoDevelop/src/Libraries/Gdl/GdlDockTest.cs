using System;
using Gtk;
using GtkSharp;
using Gnome;
using GnomeSharp;
using Gdl;

class T
{
	Program program;
	
	static void Main (string[] args)
	{
		new T (args);
	}
	
	T (string[] args)
	{
		//program = new Program ("test", "0.0", Modules.UI, args);
		//App app = new App ("test", "Test for Gdl.Dock widget");
		Gtk.Application.Init ();
		Gtk.Window app = new Gtk.Window ("test");
		app.SetDefaultSize (600, 450);
		app.DeleteEvent += new DeleteEventHandler (OnAppDelete);
		
		Dock dock = new Dock ();
		//DockLayout layout = new DockLayout (dock);
		
		DockItem di = new DockItem ("item1", "Item #1", Gtk.Stock.Execute, DockItemBehavior.Normal);
		di.Add (new Label ("test"));
		dock.AddItem (di, DockPlacement.Center);
		
		DockItem di2 = new DockItem ("item2", "Item #2", DockItemBehavior.Normal);
		di2.Add (new Label ("test2"));
		dock.AddItem (di2, DockPlacement.Center);
		
		DockItem di3 = new DockItem ("item3", "Item #3", DockItemBehavior.Normal);
		di3.Add (new Label ("test3"));
		dock.AddItem (di3, DockPlacement.Top);
		
		/*DockItem di4 = new DockItem ("item4", "Item #4", DockItemBehavior.Normal);
		di4.Add (new Label ("test4"));
		dock.AddItem (di4, DockPlacement.Center);*/
		
		app.Add (dock);
		app.ShowAll ();
		//if (dock.Root == null) {
		//	Console.WriteLine ("Crap, dock.root is null");
		//}
		Gtk.Application.Run ();
	}
	
	private void OnAppDelete (object o, DeleteEventArgs args)
	{
		Gtk.Application.Quit ();
	}
}
