// created on 6/24/2004 at 7:39 PM

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	// stub so I can compile in MD
	public class DockLayout
	{
		public DockLayout (Dock dock)
		{
		}
	
		public string[] GetLayouts (bool something)
		{
			return new string[] {""};
		}
		
		public void LoadLayout (string newLayout)
		{
		}
		
		public void LoadFromFile (string configFile)
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