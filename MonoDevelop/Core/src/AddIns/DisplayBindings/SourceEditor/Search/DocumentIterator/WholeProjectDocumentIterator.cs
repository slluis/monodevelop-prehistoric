// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.TextEditor;

using MonoDevelop.SourceEditor.Gui;
using SourceEditor = MonoDevelop.SourceEditor.Gui.SourceEditor;

namespace MonoDevelop.TextEditor.Document
{
	public class WholeProjectDocumentIterator : IDocumentIterator
	{
		ArrayList files    = new ArrayList();
		int       curIndex = -1;
		
		public WholeProjectDocumentIterator()
		{
			Reset();
		}
		
		public string CurrentFileName {
			get {
				if (curIndex < 0 || curIndex >= files.Count) {
					return null;
				}
				
				return files[curIndex].ToString();
			}
		}
				
		public IDocumentInformation Current {
			get {
				if (curIndex < 0 || curIndex >= files.Count) {
					return null;
				}
				if (!File.Exists(files[curIndex].ToString())) {
					++curIndex;
					return Current;
				}
				SourceEditor document;
				string fileName = files[curIndex].ToString();
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					// WINDOWS DEPENDENCY : ToUpper
					if (content.ContentName != null &&
						content.ContentName.ToUpper() == fileName.ToUpper()) {
						document = (SourceEditor) (((SourceEditorDisplayBindingWrapper)content).Control);
						return new EditorDocumentInformation(document, fileName);
					}
				}
				return new FileDocumentInformation (fileName, 0);
			}
		}
		
		public bool MoveForward() 
		{
			return ++curIndex < files.Count;
		}
		
		public bool MoveBackward()
		{
			if (curIndex == -1) {
				curIndex = files.Count - 1;
				return true;
			}
			return --curIndex >= -1;
		}
		
		
		void AddFiles(IProject project)
		{
			foreach (ProjectFile file in project.ProjectFiles) {
				if (file.BuildAction == BuildAction.Compile &&
				    file.Subtype     == Subtype.Code) {
					files.Add(file.Name);
				}
			}
		}
		
		void AddFiles(Combine combine)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					AddFiles(((ProjectCombineEntry)entry).Project);
				} else {
					AddFiles(((CombineCombineEntry)entry).Combine);
				}
			}
		}
		
		public void Reset() 
		{
			files.Clear();
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			if (projectService.CurrentOpenCombine != null) {
				AddFiles(projectService.CurrentOpenCombine);
			}
			
			curIndex = -1;
		}
	}
}
