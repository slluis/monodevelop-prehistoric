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
using MonoDevelop.Services;

using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Pads
{
	public interface IClassScoutNodeBuilder
	{
		bool     CanBuildClassTree(IProject project);
		TreeNode BuildClassTreeNode(IProject project);

		void     AddToClassTree(TreeNode projectNode, ParseInformationEventArgs e);
		void     RemoveFromClassTree(TreeNode parentNode, ParseInformationEventArgs e);
	}
}
