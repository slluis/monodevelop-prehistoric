//
// ProjectNodeBuilder.cs
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
	public class ProjectNodeBuilder: FolderNodeBuilder
	{
		ProjectFileEventHandler fileAddedHandler;
		ProjectFileEventHandler fileRemovedHandler;
		ProjectFileRenamedEventHandler fileRenamedHandler;
		CombineEntryRenamedEventHandler projectNameChanged;
		
		public override Type NodeDataType {
			get { return typeof(Project); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof(ProjectNodeCommandHandler); }
		}
		
		protected override void Initialize ()
		{
			fileAddedHandler = (ProjectFileEventHandler) Runtime.DispatchService.GuiDispatch (new ProjectFileEventHandler (OnAddFile));
			fileRemovedHandler = (ProjectFileEventHandler) Runtime.DispatchService.GuiDispatch (new ProjectFileEventHandler (OnRemoveFile));
			fileRenamedHandler = (ProjectFileRenamedEventHandler) Runtime.DispatchService.GuiDispatch (new ProjectFileRenamedEventHandler (OnRenameFile));
			projectNameChanged = (CombineEntryRenamedEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEntryRenamedEventHandler (OnProjectRenamed));
			
			Runtime.ProjectService.FileAddedToProject += fileAddedHandler;
			Runtime.ProjectService.FileRemovedFromProject += fileRemovedHandler;
			Runtime.ProjectService.FileRenamedInProject += fileRenamedHandler;
		}
		
		public override void Dispose ()
		{
			Runtime.ProjectService.FileAddedToProject -= fileAddedHandler;
			Runtime.ProjectService.FileRemovedFromProject -= fileRemovedHandler;
			Runtime.ProjectService.FileRenamedInProject -= fileRenamedHandler;
		}

		public override void OnNodeAdded (object dataObject)
		{
			Project project = (Project) dataObject;
			project.NameChanged += projectNameChanged;
		}
		
		public override void OnNodeRemoved (object dataObject)
		{
			Project project = (Project) dataObject;
			project.NameChanged -= projectNameChanged;
		}
		
		public override string GetNodeName (object dataObject)
		{
			return ((Project)dataObject).Name;
		}
		
		public override string GetFolderPath (object dataObject)
		{
			return ((Project)dataObject).BaseDirectory;
		}
		
		public override string ContextMenuAddinPath {
			get { return "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ProjectBrowserNode"; }
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, ref string label, ref Gdk.Pixbuf icon, ref Gdk.Pixbuf closedIcon)
		{
			Project p = dataObject as Project;
			label = p.Name;
			string iconName = Runtime.Gui.Icons.GetImageForProjectType (p.ProjectType);
			icon = Context.GetIcon (iconName);
		}

		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			Project project = (Project) dataObject;
			builder.AddChild (project.ProjectReferences);
			builder.AddChild (new ResourceFolder (project));
			
			base.BuildChildNodes (builder, dataObject);
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
		
		void OnAddFile (object sender, ProjectFileEventArgs e)
		{
			ITreeBuilder tb = Context.GetTreeBuilder (e.Project);
			if (tb != null) {
				if (e.ProjectFile.BuildAction != BuildAction.EmbedAsResource)
					AddFile (tb, e.Project, e.ProjectFile);
				else {
					tb.MoveToChild ("Resources", typeof(ResourceFolder));
					tb.AddChild (e.ProjectFile);
				}
			}
		}
		
		void OnRemoveFile (object sender, ProjectFileEventArgs e)
		{
			ITreeBuilder tb = Context.GetTreeBuilder (e.ProjectFile);
			if (tb != null) tb.Remove ();
		}
		
		void OnRenameFile (object sender, ProjectFileRenamedEventArgs e)
		{
			ITreeBuilder tb = Context.GetTreeBuilder (e.ProjectFile);
			if (tb != null) tb.Update ();
		}
		
		void OnProjectRenamed (object sender, CombineEntryRenamedEventArgs e)
		{
			ITreeBuilder tb = Context.GetTreeBuilder (e.CombineEntry);
			if (tb != null) tb.Update ();
		}
	}
	
	public class ProjectNodeCommandHandler: NodeCommandHandler
	{
		public override void RenameItem (string newName)
		{
			if (newName.IndexOfAny (new char [] { '\'', '(', ')', '"', '{', '}', '|' } ) != -1) {
				Runtime.MessageService.ShowError (String.Format (GettextCatalog.GetString ("Project name may not contain any of the following characters: {0}"), "', (, ), \", {, }, |"));
				return;
			}
			
			Project project = (Project) CurrentNode.DataItem;
			project.Name = newName;
			Runtime.ProjectService.SaveCombine();
		}
		
		public override void ActivateItem ()
		{
		}
		
		public override void RemoveItem ()
		{
			Combine cmb = CurrentNode.GetParentDataItem (typeof(Combine), false) as Combine;;
			Project prj = CurrentNode.DataItem as Project;
			
			bool yes = Runtime.MessageService.AskQuestion (String.Format (GettextCatalog.GetString ("Do you really want to remove project {0} from solution {1}"), prj.Name, cmb.Name));
			if (yes) {
				cmb.RemoveEntry (prj);
				Runtime.ProjectService.SaveCombine();
			}
		}
		
		public override DragOperation CanDragNode ()
		{
			return DragOperation.Copy | DragOperation.Move;
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
