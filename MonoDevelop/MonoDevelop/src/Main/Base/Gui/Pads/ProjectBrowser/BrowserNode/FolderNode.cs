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
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default folder in the project browser.
	/// NOTE: For a 'directory' folder use the DirectoryNode class
	/// </summary>
	public class FolderNode : AbstractBrowserNode 
	{
		public FolderNode(string nodeName)
		{
			Text           = nodeName;
			canLabelEdited = false;
		}
	}
}
