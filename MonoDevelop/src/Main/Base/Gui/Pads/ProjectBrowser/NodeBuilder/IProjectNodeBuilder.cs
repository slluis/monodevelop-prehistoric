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

using MonoDevelop.Internal.Project;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	public interface IProjectNodeBuilder
	{
		bool                CanBuildProjectTree(IProject project);
		AbstractBrowserNode BuildProjectTreeNode(IProject project);
	}
}
