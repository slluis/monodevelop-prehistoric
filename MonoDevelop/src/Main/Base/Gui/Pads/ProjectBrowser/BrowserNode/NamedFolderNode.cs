// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;

using MonoDevelop.Core.Properties;

using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents a named folder. A named folder gets a localized name
    /// out of the resourceService instead directly.
	/// </summary>
	public class NamedFolderNode : FolderNode 
	{
		string resourceReference;
		int    sortPriority;
		
		public int SortPriority {
			get {
				return sortPriority;
			}
		}
		
		
		public NamedFolderNode(string resourceReference, int sortPriority) : base(((ResourceService)ServiceManager.Services.GetService(typeof(ResourceService))).GetString(resourceReference))
		{
			this.resourceReference = resourceReference;
			this.sortPriority      = sortPriority;
		}
		
		public override void UpdateNaming()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			Text = resourceService.GetString(resourceReference);
			base.UpdateNaming();
		}
	}
}
