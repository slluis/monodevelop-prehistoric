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
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
	
		private class FileInformation
		{
			public FileOpeningFinished OnFileOpened;
			public string FileName;
		}
		
		public RecentOpen RecentOpen {
			get {
				if (recentOpen == null) {
					PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
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
				DisplayBindingService displayBindingService = (DisplayBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(DisplayBindingService));
				displayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
			}
		}
		
		public void OpenFile (string fileName)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			FileInformation openFileInfo=new FileInformation();
			openFileInfo.OnFileOpened=null;
			openFileInfo.FileName=fileName;
			dispatcher.GuiDispatch (new StatefulMessageHandler (realOpenFile), openFileInfo);
		}

		public void OpenFile (string fileName, FileOpeningFinished OnFileOpened){
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			FileInformation openFileInfo=new FileInformation();
			openFileInfo.OnFileOpened=OnFileOpened;
			openFileInfo.FileName=fileName;
			dispatcher.GuiDispatch (new StatefulMessageHandler (realOpenFile), openFileInfo);
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
			if (!fileName.StartsWith ("http://"))
				fileName = System.IO.Path.GetFullPath (fileName);
			
			//Debug.Assert(fileUtilityService.IsValidFileName(fileName));
			if (fileUtilityService.IsDirectory (fileName)) {
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
				if (!fileUtilityService.TestFileExists(fileName)) {
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
			
			DisplayBindingService displayBindingService = (DisplayBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(DisplayBindingService));
			
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
			IDisplayBinding binding = displayBindingService.GetBindingPerFileName(fileName);
			
			if (binding != null) {
				IProject project = null;
				Combine combine = null;
				GetProjectAndCombineFromFile (fileName, out project, out combine);
				
				if (combine != null && project != null)
				{
					if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, project).Invoke), fileName) == FileOperationResult.OK) {
						fileService.RecentOpen.AddLastFile(fileName);
					}
				}
				else
				{
					if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, null).Invoke), fileName) == FileOperationResult.OK) {
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
					if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate (new LoadFileWrapper (displayBindingService.LastBinding, null).Invoke), fileName) == FileOperationResult.OK) {
						fileService.RecentOpen.AddLastFile (fileName);
					}
				}
			}
			if(oFileInfo.OnFileOpened!=null) oFileInfo.OnFileOpened();
		}
		
		protected void GetProjectAndCombineFromFile (string fileName, out IProject project, out Combine combine)
		{
			IProjectService projectService = (IProjectService) ServiceManager.GetService(typeof(IProjectService));
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
		
		public void NewFile(string defaultName, string language, string content)
		{
			DisplayBindingService displayBindingService = (DisplayBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(DisplayBindingService));
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
					IMessageService messageService = (IMessageService)ServiceManager.GetService(typeof(IMessageService));
					messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't remove directory {0}"), fileName));
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, true));
			} else {
				try {
					File.Delete(fileName);
				} catch (Exception e) {
					IMessageService messageService = (IMessageService)ServiceManager.GetService(typeof(IMessageService));
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
						IMessageService messageService = (IMessageService)ServiceManager.GetService(typeof(IMessageService));
						messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Can't rename directory {0}"), oldName));
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, true));
				} else {
					try {
						File.Move(oldName, newName);
					} catch (Exception e) {
						IMessageService messageService = (IMessageService)ServiceManager.GetService(typeof(IMessageService));
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
