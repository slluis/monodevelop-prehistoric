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

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public interface IClassScoutNodeBuilder
	{
		bool     CanBuildClassTree(IProject project);
		TreeNode BuildClassTreeNode(IProject project);

		void     AddToClassTree(TreeNode projectNode, ParseInformationEventArgs e);
		void     RemoveFromClassTree(TreeNode parentNode, ParseInformationEventArgs e);
	}
}
