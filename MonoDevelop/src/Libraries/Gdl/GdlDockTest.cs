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
		
		Box table = new VBox (false, 5);
		table.BorderWidth = 10;
		app.Add (table);
		
		Dock dock = new Dock ();		
		DockLayout layout = new DockLayout (dock);
		DockBar dockbar = new DockBar (dock);
		
		Box box = new HBox (false, 5);
		box.PackStart (dockbar, false, false, 0);
		box.PackEnd (dock, true, true, 0);
		table.PackStart (box, true, true, 0);

		DockItem di = new DockItem ("item1", "Item #1", DockItemBehavior.Locked);
		di.Add (CreateTextView ());
		dock.AddItem (di, DockPlacement.Top);
		
		DockItem di2 = new DockItem ("item2", "Item #2 has some large title",
					     Gtk.Stock.Execute, DockItemBehavior.Normal);
		di2.Add (new Button ("Button 2"));
		dock.AddItem (di2, DockPlacement.Right);

#if false
		DockItem di3 = new DockItem ("item3", "Item #3 has accented characters",/* (áéíóúñ)",*/
					     Gtk.Stock.Convert, DockItemBehavior.Normal |
					     DockItemBehavior.CantClose);
		di3.Add (new Button ("Button 3"));
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
#endif
		di2.DockTo (null, DockPlacement.Floating);

		box = new HBox (true, 5);
		table.PackEnd (box, false, false, 0);
		
		Button button = new Button (Gtk.Stock.Save);
		button.Clicked += OnSaveLayout;
		box.PackEnd (button, false, true, 0);
		
		button = new Button ("Layout Manager");
		button.Clicked += OnRunLayoutManager;
		box.PackEnd (button, false, true, 0);

		button = new Button ("Dump XML");
		button.Clicked += OnDumpXML;
		box.PackEnd (button, false, true, 0);

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
	
	private void OnSaveLayout (object o, EventArgs args)
	{
	}
	
	private void OnRunLayoutManager (object o, EventArgs args)
	{
	}
	
	private void OnDumpXML (object o, EventArgs args)
	{
	}
	
	private void OnAppDelete (object o, DeleteEventArgs args)
	{
		Application.Quit ();
	}
}
