// created on 05/06/2004 at 11:14 A
using System;
using System.Collections;

using Gtk;

namespace Gdl
{
	public class DockMaster
	{
		private object obj;
		private Hashtable dock_objects;
		private ArrayList toplevel_docks;
		private DockObject controller;
		private int dock_number;
		private int number;
		private string default_title;
		private Gdk.GC root_xor_gc;
		private bool rect_drawn;
		private Dock rect_owner;
		private DockRequest drag_request;
		private uint idle_layout_changed_id;
		private Hashtable locked_items;
		private Hashtable unlocked_items;
		
	}
}