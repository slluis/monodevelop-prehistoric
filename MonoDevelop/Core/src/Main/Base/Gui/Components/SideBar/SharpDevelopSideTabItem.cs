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

using MonoDevelop.Core.Services;

using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Templates;

namespace MonoDevelop.Gui.Components
{
	public class SharpDevelopSideTabItem : AxSideTabItem
	{
		ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(ResourceService));
		
		public SharpDevelopSideTabItem(string name) : base(name)
		{
			//Icon = resourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag) : base(name, tag)
		{
			//Icon = resourceService.GetBitmap("Icons.16x16.SideBarDocument");
		}
		
		public SharpDevelopSideTabItem(string name, object tag, Bitmap icon) : base(name, tag, icon)
		{
		}
	}
}
