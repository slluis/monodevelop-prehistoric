//
// ProjectFolderNodeBuilder.cs
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
	public class ProjectFolderNodeBuilder: FolderNodeBuilder
	{
		Gdk.Pixbuf folderOpenIcon;
		Gdk.Pixbuf folderClosedIcon;
		
		FileEventHandler fileRenamedHandler;
		FileEventHandler fileRemovedHandler;
		
		public override Type NodeDataType {
			get { return typeof(ProjectFolder); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof(ProjectFolderCommandHandler); }
		}
		
		public override string GetNodeName (object dataObject)
		{
			return ((ProjectFolder)dataObject).Name;
		}
		
		public override string GetFolderPath (object dataObject)
		{
			return ((ProjectFolder)dataObject).Path;
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ProjectBrowser/ContextMenu/DefaultDirectoryNode"; }
		}
		
		protected override void Initialize ()
		{
			base.Initialize ();

			folderOpenIcon = Context.GetIcon (Stock.OpenFolderBitmap);
			folderClosedIcon = Context.GetIcon (Stock.ClosedFolderBitmap);
			
			fileRenamedHandler = (FileEventHandler) Runtime.DispatchService.GuiDispatch (new FileEventHandler (OnFolderRenamed));
			fileRemovedHandler = (FileEventHandler) Runtime.DispatchService.GuiDispatch (new FileEventHandler (OnFolderRemoved));
		}
		
		public override void OnNodeAdded (object dataObject)
		{
			ProjectFolder folder = (ProjectFolder) dataObject;
			folder.FolderRenamed += fileRenamedHandler;
			folder.FolderRemoved += fileRemovedHandler;
		}
		
		public override void OnNodeRemoved (object dataObject)
		{
			ProjectFolder folder = (ProjectFolder) dataObject;
			folder.FolderRenamed -= fileRenamedHandler;
			folder.FolderRemoved -= fileRemovedHandler;
			folder.Dispose ();
		}
		
		void OnFolderRenamed (object sender, FileEventArgs e)
		{
			ITreeBuilder tb = Context.GetTreeBuilder (sender);
			if (tb != null) tb.Update ();
		}
		
		void OnFolderRemoved (object sender, FileEventArgs e)
		{
			ITreeBuilder tb = Context.GetTreeBuilder (sender);
			if (tb != null) tb.Remove ();
		}
	
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			ProjectFolder folder = (ProjectFolder) dataObject;
			label = folder.Name;
			icon = folderOpenIcon;
			closedIcon = folderClosedIcon;
			
			if (folder.Project == null)
			{
				Gdk.Pixbuf gicon = Context.GetComposedIcon (icon, "fade");
				if (gicon == null) {
					gicon = Runtime.Gui.Icons.MakeTransparent (icon, 0.5);
					Context.CacheComposedIcon (icon, "fade", gicon);
				}
				icon = gicon;
				
				gicon = Context.GetComposedIcon (closedIcon, "fade");
				if (gicon == null) {
					gicon = Runtime.Gui.Icons.MakeTransparent (closedIcon, 0.5);
					Context.CacheComposedIcon (closedIcon, "fade", gicon);
				}
				closedIcon = gicon;
			}
		}
		
		public override int CompareObjects (object thisDataObject, object otherDataObject)
		{
			if (otherDataObject is ProjectFolder)
				return DefaultSort;
			else if (otherDataObject is ProjectFile)
				return -1;
			else
				return 1;
		}
	}
	
	public class ProjectFolderCommandHandler: NodeCommandHandler
	{
		public override void RenameItem (string newName)
		{
			ProjectFolder folder = (ProjectFolder) CurrentNode.DataItem as ProjectFolder;
			string oldFoldername = folder.Path;
			string newFoldername = Path.Combine (Path.GetDirectoryName(oldFoldername), newName);
			
			if (oldFoldername != newFoldername) {
				try {
					
					if (Runtime.FileUtilityService.IsValidFileName (newFoldername)) {
						Runtime.FileService.RenameFile (oldFoldername, newFoldername);
						Runtime.ProjectService.SaveCombine();
					}
				} catch (System.IO.IOException) {   // assume duplicate file
					Runtime.MessageService.ShowError(GettextCatalog.GetString ("File or directory name is already in use, choose a different one."));
				} catch (System.ArgumentException) { // new file name with wildcard (*, ?) characters in it
					Runtime.MessageService.ShowError(GettextCatalog.GetString ("The file name you have chosen contains illegal characters. Please choose a different file name."));
				}
			}
		}
		
		public override void RemoveItem ()
		{
			ProjectFolder folder = (ProjectFolder) CurrentNode.DataItem as ProjectFolder;
			
			bool yes = Runtime.MessageService.AskQuestion (String.Format (GettextCatalog.GetString ("Do you want to remove folder {0}?"), folder.Name));
			if (!yes) return;
			
			Runtime.ProjectService.RemoveFileFromProject (folder.Path);
		}
		
		public override DragOperation CanDragNode ()
		{
			return DragOperation.Move | DragOperation.Copy;
		}
		
		public override bool CanDropNode (object dataObject, DragOperation operation)
		{
			return (dataObject is ProjectFile) || (dataObject is ProjectFolder);
		}
		
		public override void OnNodeDrop (object dataObject, DragOperation operation)
		{
		}
	}
}
