// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Xml;

using MonoDevelop.Services;

using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Templates;

namespace MonoDevelop.Gui.Components
{
	public class SharpDevelopSideTabItem : AxSideTabItem
	{
		public SharpDevelopSideTabItem(string name) : base(name)
		{
			//Icon = Runtime.Gui.Resources.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag) : base(name, tag)
		{
			//Icon = Runtime.Gui.Resources.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag, Bitmap icon) : base(name, tag, icon)
		{
		}
	}
}
