//
// ResourceFolderNodeBuilder.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Collections;

using MonoDevelop.Internal.Project;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Pads.ProjectPad
{
	public class ResourceFolderNodeBuilder: TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ResourceFolder); }
		}
		
		public override string GetNodeName (object dataObject)
		{
			// Don't localize this string.
			return "Resources";
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ResourceFolderNode"; }
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			label = GettextCatalog.GetString ("Resources");
			icon = Context.GetIcon (Stock.OpenResourceFolder);
			closedIcon = Context.GetIcon (Stock.ClosedResourceFolder);
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			Project project = ((ResourceFolder)dataObject).Project;
			foreach (ProjectFile file in project.ProjectFiles)
				if (file.BuildAction == BuildAction.EmbedAsResource)
					return true;
			return false;
		}

		public override int CompareObjects (object thisDataObject, object otherDataObject)
		{
			if (otherDataObject is ProjectReferenceCollection)
				return 1;
			else
				return -1;
		}

		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			Project project = ((ResourceFolder)dataObject).Project;
			foreach (ProjectFile file in project.ProjectFiles)
				if (file.BuildAction == BuildAction.EmbedAsResource)
					builder.AddChild (file);
		}
	}
}
