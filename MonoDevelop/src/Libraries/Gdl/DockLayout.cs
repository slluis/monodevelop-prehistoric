// created on 6/24/2004 at 7:39 PM

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	// stub so I can compile in MD
	public class DockLayout
	{
		DockMaster master;
		Widget itemsui;
		Widget layoutsui;
		string[] layouts;
		bool dirty;
	
		public DockLayout (Dock dock)
		{
		}
		
		public DockMaster Master {
			get {
				return master;
			}
			set {
				master = value;
			}
		}
		
		// generated had Dirty and IsDirty
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
		
		public void Attach (DockMaster master)
		{
			this.master = master;
		}
		
		public void DeleteLayout (string name)
		{
		}
	
		public string[] GetLayouts (bool includeDefault)
		{
			return layouts;
		}
		
		public void LoadLayout (string newLayout)
		{
		}
		
		public void LoadFromFile (string configFile)
		{
		}
		
		public void RunManager ()
		{
		}
		
		public void SaveLayout (string currentLayout)
		{
		}
		
		public void SaveToFile (string file)
		{
		}
	}
}