// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Gui.Utils;

namespace MonoDevelop.Services
{
	public class DefaultFileService : AbstractService, IFileService
	{
		string currentFile;
		RecentOpen       recentOpen = null;
	
		private class FileInformation
		{
			public FileOpeningFinished OnFileOpened;
			public string FileName;
		}
		
		public RecentOpen RecentOpen {
			get {
				if (recentOpen == null)
					recentOpen = new RecentOpen ();
				return recentOpen;
			}
		}
		
		public string CurrentFile {
			get {
				return currentFile;
			}
			set {
				currentFile = value;
			}
		}
		
		class LoadFileWrapper
		{
			IDisplayBinding binding;
			IProject project;
			
			public LoadFileWrapper(IDisplayBinding binding)
			{
				this.binding = binding;
			}
			
			public LoadFileWrapper(IDisplayBinding binding, IProject project)
			{
				this.binding = binding;
				this.project = project;
			}
			
			public void Invoke(string fileName)
			{
				IViewContent newContent = binding.CreateContentForFile(fileName);
				if (project != null)
				{
					newContent.HasProject = true;
					newContent.Project = project;
				}
				WorkbenchSingleton.Workbench.ShowView(newContent);
				Runtime.Gui.DisplayBindings.AttachSubWindows(newContent.WorkbenchWindow);
			}
		}
		
		public void OpenFile (string fileName)
		{
			FileInformation openFileInfo=new FileInformation();
			openFileInfo.OnFileOpened=null;
			openFileInfo.FileName=fileName;
			Runtime.DispatchService.GuiDispatch (new StatefulMessageHandler (realOpenFile), openFileInfo);
		}

		public void OpenFile (string fileName, FileOpeningFinished OnFileOpened){
			FileInformation openFileInfo=new FileInformation();
			openFileInfo.OnFileOpened=OnFileOpened;
			openFileInfo.FileName=fileName;
			Runtime.DispatchService.GuiDispatch (new StatefulMessageHandler (realOpenFile), openFileInfo);
		}
		
		void realOpenFile (object openFileInfo)
		{
			string fileName;
			FileInformation oFileInfo;
			if(openFileInfo is FileInformation){
				oFileInfo=openFileInfo as FileInformation;
				fileName=oFileInfo.FileName;
			}else{
				return;
			}
			
			if (fileName == null)
				return;

			string origName = fileName;

			if (fileName.StartsWith ("file://"))
				fileName = fileName.Substring (7);

			if (!fileName.StartsWith ("http://"))
				fileName = System.IO.Path.GetFullPath (fileName);
			
			//Debug.Assert(Runtime.FileUtilityService.IsValidFileName(fileName));
			if (Runtime.FileUtilityService.IsDirectory (fileName)) {
				return;
			}
			// test, if file fileName exists
			if (!fileName.StartsWith("http://")) {
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(origName)) { 
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == origName) {
							content.WorkbenchWindow.SelectWindow();
							if(oFileInfo.OnFileOpened!=null) oFileInfo.OnFileOpened();
							return;
						}
					}
				} else 
				if (!Runtime.FileUtilityService.TestFileExists(fileName)) {
					if(oFileInfo.OnFileOpened!=null) oFileInfo.OnFileOpened();
					return;
				}
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.ContentName != null && 
				    content.ContentName == fileName) {
					content.WorkbenchWindow.SelectWindow();
					if(oFileInfo.OnFileOpened!=null) oFileInfo.OnFileOpened();
					return;
				}
			}
			
			IDisplayBinding binding = Runtime.Gui.DisplayBindings.GetBindingPerFileName(fileName);
			
			if (binding != null) {
				IProject project = null;
				Combine combine = null;
				GetProjectAndCombineFromFile (fileName, out project, out combine);
				
				if (combine != null && project != null)
				{
					if (Runtime.FileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, project).Invoke), fileName) == FileOperationResult.OK) {
						Runtime.FileService.RecentOpen.AddLastFile (fileName, project.Name);
					}
				}
				else
				{
					if (Runtime.FileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, null).Invoke), fileName) == FileOperationResult.OK) {
						Runtime.FileService.RecentOpen.AddLastFile (fileName, null);
					}
				}
			} else {
				try {
					string mimetype = Vfs.GetMimeType (fileName);
					Console.WriteLine ("About to MimeApp.Exec on mimetype: " + mimetype);
					if (mimetype != null) {
						MimeApplication.Exec (mimetype, fileName);
					} else {
						Gnome.Url.Show ("file://" + fileName);
					}
				} catch {
					if (Runtime.FileUtilityService.ObservedLoad(new NamedFileOperationDelegate (new LoadFileWrapper (Runtime.Gui.DisplayBindings.LastBinding, null).Invoke), fileName) == FileOperationResult.OK) {
						Runtime.FileService.RecentOpen.AddLastFile (fileName, null);
					}
				}
			}
			if(oFileInfo.OnFileOpened!=null) oFileInfo.OnFileOpened();
		}
		
		protected void GetProjectAndCombineFromFile (string fileName, out IProject project, out Combine combine)
		{
			combine = Runtime.ProjectService.CurrentOpenCombine;
			project = null;
			
			if (combine != null)
			{
				ArrayList projectslist = Combine.GetAllProjects(combine);

				foreach (ProjectCombineEntry projectaux in projectslist)
				{
					if (projectaux.Project.IsFileInProject (fileName))
					{
						project = projectaux.Project;
					}
				}
			}
		}
		
		public void NewFile(string defaultName, string language, string content)
		{
			IDisplayBinding binding = Runtime.Gui.DisplayBindings.GetBindingPerLanguageName(language);
			
			if (binding != null) {
				IViewContent newContent = binding.CreateContentForLanguage(language, content, defaultName);
				if (newContent == null) {
					throw new ApplicationException(String.Format("Created view content was null{3}DefaultName:{0}{3}Language:{1}{3}Content:{2}", defaultName, language, content, Environment.NewLine));
				}
				newContent.UntitledName = defaultName;
				newContent.IsDirty      = false;
				WorkbenchSingleton.Workbench.ShowView(newContent);
				
				Runtime.Gui.DisplayBindings.AttachSubWindows(newContent.WorkbenchWindow);
			} else {
				throw new ApplicationException("Can't create display binding for language " + language);				
			}
		}
		
		public IWorkbenchWindow GetOpenFile(string fileName)
		{
			fileName = System.IO.Path.GetFullPath (fileName);
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.ContentName != null &&
				    content.ContentName.ToUpper() == fileName.ToUpper()) {
					return content.WorkbenchWindow;
				}
			}
			return null;
		}
		
		public void RemoveFileFromProject(string fileName)
		{
			if (Directory.Exists(fileName)) {
				OnFileRemovedFromProject(new FileEventArgs(fileName, true));
			} else {
				OnFileRemovedFromProject(new FileEventArgs(fileName, false));
			}
		}
		
		public void RemoveFile(string fileName)
		{
			if (Directory.Exists(fileName)) {
				try {
					Directory.Delete(fileName);
				} catch (Exception e) {
					Runtime.MessageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't remove directory {0}"), fileName));
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, true));
			} else {
				try {
					File.Delete(fileName);
				} catch (Exception e) {
					Runtime.MessageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't remove file {0}"), fileName));
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, false));
			}
		}
		
		public void RenameFile(string oldName, string newName)
		{
			if (oldName != newName) {
				if (Directory.Exists(oldName)) {
					try {
						Directory.Move(oldName, newName);
					} catch (Exception e) {
						Runtime.MessageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't rename directory {0}"), oldName));
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, true));
				} else {
					try {
						File.Move(oldName, newName);
					} catch (Exception e) {
						Runtime.MessageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't rename file {0}"), oldName));
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, false));
				}
			}
		}
		
		protected virtual void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null) {
				FileRemoved(this, e);
			}
		}

		protected virtual void OnFileRenamed(FileEventArgs e)
		{
			if (FileRenamed != null) {
				FileRenamed(this, e);
			}
		}

		protected virtual void OnFileRemovedFromProject(FileEventArgs e)
		{
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject(this, e);
			}
		}
		
		public event FileEventHandler FileRemovedFromProject;
		public event FileEventHandler FileRenamed;
		public event FileEventHandler FileRemoved;
	}
}
