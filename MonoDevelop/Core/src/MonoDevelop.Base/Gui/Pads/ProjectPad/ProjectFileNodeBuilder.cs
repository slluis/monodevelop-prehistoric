//
// ProjectFileNodeBuilder.cs
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
	public class ProjectFileNodeBuilder: TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ProjectFile); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof(ProjectFileNodeCommandHandler); }
		}
		
		public override string GetNodeName (object dataObject)
		{
			return Path.GetFileName (((ProjectFile)dataObject).Name);
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ProjectFileNode"; }
		}
		
		public override void GetNodeAttributes (ITreeNavigator treeNavigator, object dataObject, ref NodeAttributes attributes)
		{
			attributes |= NodeAttributes.AllowRename;
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ProjectFile file = (ProjectFile) dataObject;
			label = Path.GetFileName (file.FilePath);
			icon = Context.GetIcon (Runtime.Gui.Icons.GetImageForFile (file.FilePath));
			if (file.Project == null) {
				Gdk.Pixbuf gicon = Context.GetComposedIcon (icon, "fade");
				if (gicon == null) {
					gicon = Runtime.Gui.Icons.MakeTransparent (icon, 0.5);
					Context.CacheComposedIcon (icon, "fade", gicon);
				}
				icon = gicon;
			}
		}
	}
	
	public class ProjectFileNodeCommandHandler: NodeCommandHandler
	{
		public override void RenameItem (string newName)
		{
			ProjectFile file = CurrentNode.DataItem as ProjectFile;
			string oldname = file.Name;

			string newname = Path.Combine (Path.GetDirectoryName (oldname), newName);
			if (oldname != newname) {
				try {
					if (Runtime.FileUtilityService.IsValidFileName (newname)) {
						Runtime.FileService.RenameFile (oldname, newname);
						Runtime.ProjectService.SaveCombine();
					}
				} catch (System.IO.IOException) {   // assume duplicate file
					Runtime.MessageService.ShowError (GettextCatalog.GetString ("File or directory name is already in use, choose a different one."));
				} catch (System.ArgumentException) { // new file name with wildcard (*, ?) characters in it
					Runtime.MessageService.ShowError (GettextCatalog.GetString ("The file name you have chosen contains illegal characters. Please choose a different file name."));
				}
			}
		}
		
		public override void ActivateItem ()
		{
			ProjectFile file = CurrentNode.DataItem as ProjectFile;
			Runtime.FileService.OpenFile (file.FilePath);
		}
		
		public override void RemoveItem ()
		{
			ProjectFile file = CurrentNode.DataItem as ProjectFile;
			Project project = CurrentNode.GetParentDataItem (typeof(Project), false) as Project;
			
			bool yes = Runtime.MessageService.AskQuestion (String.Format (GettextCatalog.GetString ("Are you sure you want to remove file {0} from project {1}?"), Path.GetFileName (file.Name), project.Name));
			if (!yes) return;

			Runtime.ProjectService.RemoveFileFromProject (file.Name);
			Runtime.ProjectService.SaveCombine();
		}
		
		public override DragOperation CanDragNode ()
		{
			return DragOperation.Copy | DragOperation.Move;
		}
		
		public override bool CanDropNode (object dataObject, DragOperation operation)
		{
			return dataObject is CombineEntry;
		}
		
		public override void OnNodeDrop (object dataObject, DragOperation operation)
		{
		}
	}
}
