using System;
using Gtk;
using GtkSharp;
using Gnome;
using GnomeSharp;
using Gdl;
using GdlSharp;

class T
{
	Program program;
	
	static void Main (string[] args)
	{
		new T (args);
	}
	
	T (string[] args)
	{
		program = new Program ("test", "0.0", Modules.UI, args);
		App app = new App ("test", "Test for Gdl.Dock widget");
		app.SetDefaultSize (600, 450);
		app.DeleteEvent += new DeleteEventHandler (OnAppDelete);
		
		Dock dock = new Dock ();
		DockLayout layout = new DockLayout (dock);
		
		DockItem di = new DockItem ("item1", "Item #1", DockItemBehavior.Locked);
		di.Add (new Label ("test"));
		dock.AddItem (di, DockPlacement.Right);
		
		DockItem di2 = new DockItem ("item2", "Item #2", DockItemBehavior.Locked);
		di2.Add (new Label ("test2"));
		dock.AddItem (di2, DockPlacement.Bottom);
		
		DockItem di3 = new DockItem ("item3", "Item #3", DockItemBehavior.Locked);
		di3.Add (new Label ("test3"));
		dock.AddItem (di3, DockPlacement.Left);
		
		DockItem di4 = new DockItem ("item4", "Item #4", DockItemBehavior.Locked);
		di4.Add (new Label ("test4"));
		dock.AddItem (di4, DockPlacement.Top);
		
		app.Contents = dock;
		app.ShowAll ();
		program.Run ();
	}
	
	private void OnAppDelete (object o, DeleteEventArgs args)
	{
		program.Quit ();
	}
}
