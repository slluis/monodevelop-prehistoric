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
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public RecentOpen RecentOpen {
			get {
				if (recentOpen == null) {
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					recentOpen = (RecentOpen)propertyService.GetProperty("MonoDevelop.Gui.MainWindow.RecentOpen", new RecentOpen());
				}
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
			string projectname, pathrelativetoproject;
			
			public LoadFileWrapper(IDisplayBinding binding)
			{
				this.binding = binding;
			}
			
			public LoadFileWrapper(IDisplayBinding binding, string projectname, string pathrelativetoproject)
			{
				this.binding = binding;
				this.projectname = projectname;
				this.pathrelativetoproject = pathrelativetoproject;
			}
			
			public void Invoke(string fileName)
			{
				IViewContent newContent = binding.CreateContentForFile(fileName);
				if (projectname != null && projectname != "" &&  pathrelativetoproject != null && pathrelativetoproject != "")
				{ 
					newContent.HasProject = true;
					newContent.ProjectName = projectname;
					newContent.PathRelativeToProject = pathrelativetoproject;
				}
				WorkbenchSingleton.Workbench.ShowView(newContent);
				DisplayBindingService displayBindingService = (DisplayBindingService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
				displayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
			}
		}
		
		public void OpenFile (string fileName)
		{
			if (fileName == null)
				return;
			string origName = fileName;
			if (!fileName.StartsWith ("http://"))
				fileName = System.IO.Path.GetFullPath (fileName);
			
			//Debug.Assert(fileUtilityService.IsValidFileName(fileName));
			// test, if file fileName exists
			if (!fileName.StartsWith("http://")) {
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(origName)) { 
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == origName) {
							content.WorkbenchWindow.SelectWindow();
							return;
						}
					}
				} else 
				if (!fileUtilityService.TestFileExists(fileName)) {
					return;
				}
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.ContentName != null && 
				    content.ContentName == fileName) {
					content.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			
			DisplayBindingService displayBindingService = (DisplayBindingService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
			
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			IDisplayBinding binding = displayBindingService.GetBindingPerFileName(fileName);
			
			if (binding != null) {
				IProject project = null;
				Combine combine = null;
				GetProjectAndCombineFromFile (fileName, out project, out combine);
				string pathrelativetoproject = GetRelativePath (project, fileName);
				
				if (combine != null && project != null)
				{
					if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, project.Name, pathrelativetoproject).Invoke), fileName) == FileOperationResult.OK) {
						fileService.RecentOpen.AddLastFile(fileName);
					}
				}
				else
				{
					if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, null, null).Invoke), fileName) == FileOperationResult.OK) {
						fileService.RecentOpen.AddLastFile(fileName);
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
					if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate (new LoadFileWrapper (displayBindingService.LastBinding, null, null).Invoke), fileName) == FileOperationResult.OK) {
						fileService.RecentOpen.AddLastFile (fileName);
					}
				}
			}
		}
		
		protected void GetProjectAndCombineFromFile (string fileName, out IProject project, out Combine combine)
		{
			IProjectService projectService = (IProjectService) ServiceManager.Services.GetService(typeof(IProjectService));
			combine = projectService.CurrentOpenCombine;
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
		
		protected string GetRelativePath (IProject project, string fileName)
		{
			string relativepath;
	
			if (project != null && fileName.IndexOf (project.BaseDirectory) == 0)
			{
				relativepath = fileName.Substring (project.BaseDirectory.Length);
			
				if (relativepath.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
				{
					relativepath = relativepath.Substring (1);
				}
			}
			else
			{
				relativepath = System.IO.Path.GetFileName (fileName);
			}
			
			return relativepath;
		}
		
		public void NewFile(string defaultName, string language, string content)
		{
			DisplayBindingService displayBindingService = (DisplayBindingService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
			IDisplayBinding binding = displayBindingService.GetBindingPerLanguageName(language);
			
			if (binding != null) {
				IViewContent newContent = binding.CreateContentForLanguage(language, content, defaultName);
				if (newContent == null) {
					throw new ApplicationException(String.Format("Created view content was null{3}DefaultName:{0}{3}Language:{1}{3}Content:{2}", defaultName, language, content, Environment.NewLine));
				}
				newContent.UntitledName = defaultName;
				newContent.IsDirty      = false;
				WorkbenchSingleton.Workbench.ShowView(newContent);
				
				displayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
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
					IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't remove directory {0}"), fileName));
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, true));
			} else {
				try {
					File.Delete(fileName);
				} catch (Exception e) {
					IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't remove file {0}"), fileName));
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
						IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't rename directory {0}"), oldName));
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, true));
				} else {
					try {
						File.Move(oldName, newName);
					} catch (Exception e) {
						IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't rename file {0}"), oldName));
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
