using System;
using Gtk;
using Gdl;

class T
{
	static void Main (string[] args)
	{
		new T (args);
	}
	
	T (string[] args)
	{
		Application.Init ();
		Window app = new Window ("test");
		app.SetDefaultSize (400, 400);
		app.DeleteEvent += new DeleteEventHandler (OnAppDelete);
		
		Dock dock = new Dock ();
		//DockLayout layout = new DockLayout (dock);
		
		DockItem di = new DockItem ("item1", "Item #1", DockItemBehavior.Locked);
		di.Add (CreateTextView ());
		dock.AddItem (di, DockPlacement.Top);
		
		DockItem di2 = new DockItem ("item2", "Item #2 has some large title",
					     Gtk.Stock.Execute, DockItemBehavior.Normal);
		di2.Add (new Button ("Button 2"));
		dock.AddItem (di2, DockPlacement.Right);
		
		DockItem di3 = new DockItem ("item3", "Item #3 has accented characters",/* (áéíóúñ)",*/
					     Gtk.Stock.Convert, DockItemBehavior.Normal |
					     DockItemBehavior.CantClose);
		di3.Add (CreateTextView ());
		dock.AddItem (di3, DockPlacement.Bottom);
		
		DockItem[] items = new DockItem[4];
		items[0] = new DockItem ("item4", "Item #4", Gtk.Stock.JustifyFill,
					 DockItemBehavior.Normal | DockItemBehavior.CantIconify);
		items[0].Add (CreateTextView ());
		dock.AddItem (items[0], DockPlacement.Bottom);
		
		for (int i = 1; i < 3; i++) {
			string name = "Item #" + (i + 4);
			items[i] = new DockItem (name, name, Gtk.Stock.New,
						 DockItemBehavior.Normal);
			items[i].Add (CreateTextView ());
			items[i].Show ();

	    		items[0].Dock (items[i], DockPlacement.Center, null);	    
		}

		di3.DockTo (di, DockPlacement.Top);
		di2.DockTo (di3, DockPlacement.Right);
		di2.DockTo (di3, DockPlacement.Left);

		app.Add (dock);
		app.ShowAll ();
		Application.Run ();
	}
	
	private Widget CreateTextView ()
	{
		ScrolledWindow sw = new ScrolledWindow (null, null);
		sw.ShadowType = ShadowType.In;
		sw.HscrollbarPolicy = PolicyType.Automatic;
		sw.VscrollbarPolicy = PolicyType.Automatic;
		TextView tv = new TextView ();
		sw.Add (tv);
		sw.ShowAll ();

		return sw;
	}
	
	private void OnAppDelete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}
}
