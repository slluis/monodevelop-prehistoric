// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.CodeDom.Compiler;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Services
{
	
	public enum BeforeCompileAction {
		Nothing,
		SaveAllFiles,
		PromptForSave,
	}
	
	public class DefaultProjectService : AbstractService, IProjectService
	{
		IProject currentProject = null;
		Combine  currentCombine = null;
		Combine  openCombine    = null;
		
		string   openCombineFileName = null;
		
		public IProject CurrentSelectedProject {
			get {
				return currentProject;
			}
			set {
				Debug.Assert(openCombine != null);
				currentProject = value;
				OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
			}
		}
		
		public Combine CurrentSelectedCombine {
			get {
				return currentCombine;
			}
			set {
				Debug.Assert(openCombine != null);
				currentCombine = value;
				OnCurrentSelectedCombineChanged(new CombineEventArgs(currentCombine));
			}
		}
		
		public Combine CurrentOpenCombine {
			get {
				return openCombine;
			}
			set {
				openCombine = value;
			}
		}
		
		bool IsDirtyFileInCombine {
			get {
				ArrayList projects = Combine.GetAllProjects(openCombine);
				
				foreach (ProjectCombineEntry projectEntry in projects) {
					foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
						foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
							if (content.IsDirty && content.ContentName == fInfo.Name) {
								return true;
							}
						}
					}
				}
				return false;
			}
		}
		
		public bool NeedsCompiling {
			get {
				if (openCombine == null) {
					return false;
				}
				return openCombine.NeedsBuilding || IsDirtyFileInCombine;
			}
		}
		
		public void SaveCombinePreferences()
		{
			if (CurrentOpenCombine != null) {
				SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);
			}
		}
		
		public void CloseCombine()
		{
			CloseCombine(true);
		}

		public void CloseCombine(bool saveCombinePreferencies)
		{
			if (CurrentOpenCombine != null) {
				if (saveCombinePreferencies)
					SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);
				GenerateMakefiles ();
				Combine closedCombine = CurrentOpenCombine;
				CurrentSelectedProject = null;
				CurrentOpenCombine = CurrentSelectedCombine = null;
				openCombineFileName = null;
				WorkbenchSingleton.Workbench.CloseAllViews();
				OnCombineClosed(new CombineEventArgs(closedCombine));
				closedCombine.Dispose();
			}
		}
		
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
		
		public void OpenCombine(string filename)
		{
			if (openCombine != null) {
				SaveCombine();
				CloseCombine();
			}

			if (filename.StartsWith ("file://"))
				filename = filename.Substring (7);
				
			if (!fileUtilityService.TestFileExists(filename)) {
				return;
			}
			IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage(GettextCatalog.GetString ("Opening Combine..."));
				
			if (Path.GetExtension(filename).ToUpper() == ".PRJX") {
				string validcombine = Path.ChangeExtension(filename, ".cmbx");
				if (File.Exists(validcombine)) {
					LoadCombine(validcombine);
				} else {
					Combine loadingCombine = new Combine();
					IProject project = (IProject)loadingCombine.AddEntry(filename);
					if (project == null) {
						return;
					}
					loadingCombine.Name = project.Name;
					loadingCombine.SaveCombine(validcombine);
					LoadCombine(validcombine);
				}
			} else {
				LoadCombine(filename);
			}
			statusBarService.SetMessage(GettextCatalog.GetString ("Ready"));
		}
		
		void LoadCombine(string filename)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.BackgroundDispatch (new StatefulMessageHandler (backgroundLoadCombine), filename);
		}

		void backgroundLoadCombine (object arg)
		{
			string filename = arg as string;
			if (!fileUtilityService.TestFileExists(filename)) {
				return;
			}
			
			Combine loadingCombine = new Combine();
			loadingCombine.LoadCombine(filename);
			openCombine         = loadingCombine;
			openCombineFileName = filename;
			
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
			fileService.RecentOpen.AddLastProject(filename);
			
			OnCombineOpened(new CombineEventArgs(openCombine));
			openCombine.FileAddedToProject += new ProjectFileEventHandler (NotifyFileAddedToProject);
			openCombine.FileRemovedFromProject += new ProjectFileEventHandler (NotifyFileRemovedFromProject);
			openCombine.FileChangedInProject += new ProjectFileEventHandler (NotifyFileChangedInProject);
			openCombine.ReferenceAddedToProject += new ProjectReferenceEventHandler (NotifyReferenceAddedToProject);
			openCombine.ReferenceRemovedFromProject += new ProjectReferenceEventHandler (NotifyReferenceRemovedFromProject);
	
			RestoreCombinePreferences(CurrentOpenCombine, openCombineFileName);
		}
		
		void Save(string fileName)
		{
			openCombineFileName = System.IO.Path.GetFullPath (fileName);
			openCombine.SaveCombine(fileName);
			openCombine.SaveAllProjects();
		}
		
		public ProjectReference AddReferenceToProject(IProject prj, string filename)
		{
			foreach (ProjectReference rInfo in prj.ProjectReferences) {
				if (rInfo.Reference == filename) {
					return rInfo;
				}
			}
			ProjectReference newReferenceInformation = new ProjectReference(ReferenceType.Assembly, filename);
			prj.ProjectReferences.Add(newReferenceInformation);
			return newReferenceInformation;
		}
		
		public ProjectFile AddFileToProject(IProject prj, string filename, BuildAction action)
		{
			foreach (ProjectFile fInfo in prj.ProjectFiles) {
				if (fInfo.Name == filename) {
					return fInfo;
				}
			}
			ProjectFile newFileInformation = new ProjectFile(filename, action);
			prj.ProjectFiles.Add(newFileInformation);
			return newFileInformation;
		}
		
		public void AddFileToProject(IProject prj, ProjectFile projectFile) {
			prj.ProjectFiles.Add(projectFile);
		}

		
		public void SaveCombine()
		{
			Save(openCombineFileName);
		}
		
		public void MarkFileDirty(string filename)
		{
			if (openCombine != null) {
				ProjectCombineEntry entry = openCombine.GetProjectEntryContaining(filename);
				if (entry != null) {
					entry.IsDirty = true;
				}
			}
		}
		
		public void MarkProjectDirty(IProject project)
		{
			if (openCombine != null) {
				ArrayList projectEntries = Combine.GetAllProjects(openCombine);
				foreach (ProjectCombineEntry entry in projectEntries) {
					if (entry.Project == project) {
						entry.IsDirty = true;
						break;
					}
				}
			}
		}
		
		public void CompileCombine()
		{
			if (openCombine != null) {
				DoBeforeCompileAction();
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				taskService.ClearTasks();
				
				openCombine.Build(false);
			}
		}
		
		public void RecompileAll()
		{
			if (openCombine != null) {
				DoBeforeCompileAction();
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				taskService.ClearTasks();
				
				openCombine.Build(true);
			}
		}
		
		ILanguageBinding BeforeCompile(IProject project)
		{
			DoBeforeCompileAction();
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			
			taskService.NotifyTaskChange();
			
			// cut&pasted from CombineEntry.cs
			stringParserService.Properties["Project"] = project.Name;
			IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage(String.Format (GettextCatalog.GetString ("Compiling {0}"), project.Name));
			
			string outputDir = ((AbstractProjectConfiguration)project.ActiveConfiguration).OutputDirectory;
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}
			} catch (Exception e) {
				throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
			}
			// cut&paste EDND
			LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
			return languageBindingService.GetBindingPerLanguageName(project.ProjectType);
		}
		
		void AfterCompile(IProject project, ICompilerResult res)
		{
			// cut&pasted from CombineEntry.cs
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			foreach (CompilerError err in res.CompilerResults.Errors) {
				taskService.AddTask(new Task(project, err));
			}
			
			if (taskService.Errors > 0) {
				++CombineEntry.BuildErrors;
			} else {
				++CombineEntry.BuildProjects;
			}
			
			taskService.CompilerOutput = res.CompilerOutput;
			taskService.NotifyTaskChange();
		}
		
		public void RecompileProject(IProject project)
		{
			AfterCompile(project, BeforeCompile(project).RecompileProject(project));
		}
		
		public void CompileProject(IProject project)
		{
			AfterCompile(project, BeforeCompile(project).CompileProject(project));
		}
		
		void DoBeforeCompileAction()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
			BeforeCompileAction action = (BeforeCompileAction)propertyService.GetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			
			switch (action) {
				case BeforeCompileAction.Nothing:
					break;
				case BeforeCompileAction.PromptForSave:
					bool save = false;
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.ContentName != null && content.IsDirty) {
							if (!save) {
								IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
								if (messageService.AskQuestion(GettextCatalog.GetString ("Save changed files?"))) {
									save = true;
								} else {
									break;
								}
							}
							MarkFileDirty(content.ContentName);
							content.Save();
						}
					}
					break;
				case BeforeCompileAction.SaveAllFiles:
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.ContentName != null && content.IsDirty) {
							MarkFileDirty(content.ContentName);
							content.Save();
						}
					}
					break;
				default:
					Debug.Assert(false);
					break;
			}
		}
		
		public ProjectFile RetrieveFileInformationForFile(string fileName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name == fileName) {
						return fInfo;
					}
				}
			}
			return null;
		}
		
		void RemoveFileFromAllProjects(string fileName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			restart:
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectReference rInfo in projectEntry.Project.ProjectReferences) {
					if (rInfo.ReferenceType == ReferenceType.Assembly && rInfo.Reference == fileName) {
						projectEntry.Project.ProjectReferences.Remove(rInfo);
						goto restart;
					}
				}
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name == fileName) {
						projectEntry.Project.ProjectFiles.Remove(fInfo);
						goto restart;
					}
				}
			}
		}
		
		void RemoveAllInDirectory(string dirName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			restart:
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name.StartsWith(dirName)) {
						projectEntry.Project.ProjectFiles.Remove(fInfo);
						goto restart;
					}
				}
			}
		}
		
		void CheckFileRemove(object sender, FileEventArgs e)
		{
			if (openCombine != null) {
				if (e.IsDirectory) {
					RemoveAllInDirectory(e.FileName);
				} else {
					RemoveFileFromAllProjects(e.FileName);
				}
			}
		}
		
		void RenameFileInAllProjects(string oldName, string newName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name == oldName) {
						fInfo.Name = newName;
					}
				}
			}
		}

		void RenameDirectoryInAllProjects(string oldName, string newName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name.StartsWith(oldName)) {
						fInfo.Name = newName + fInfo.Name.Substring(oldName.Length);
					}
				}
			}
		}

		void CheckFileRename(object sender, FileEventArgs e)
		{
			Debug.Assert(e.SourceFile != e.TargetFile);
			if (openCombine != null) {
				if (e.IsDirectory) {
					RenameDirectoryInAllProjects(e.SourceFile, e.TargetFile);
				} else {
					RenameFileInAllProjects(e.SourceFile, e.TargetFile);
				}
			}
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
			
			fileService.FileRemoved += new FileEventHandler(CheckFileRemove);
			fileService.FileRenamed += new FileEventHandler(CheckFileRename);
		}
		
		string MakeValidName(string str)
		{
			string tmp = "";
			foreach (char ch in str) {
				tmp += ((byte)ch).ToString();
			}
			return tmp;
		}
		
		void RestoreCombinePreferences(Combine combine, string combinefilename)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "CombinePreferences";
			if (!Directory.Exists(directory)) {
				return;
			}
			
			string[] files = Directory.GetFiles(directory, combine.Name + "*.xml");
			
			if (files.Length > 0) {
				XmlDocument doc = new XmlDocument();
				try {
					doc.Load(files[0]);
				} catch (Exception) {
					return;
				}
				XmlElement root = doc.DocumentElement;
				string combinepath = Path.GetDirectoryName(combinefilename);
				if (root["Files"] != null) {
					IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IFileService));
					foreach (XmlElement el in root["Files"].ChildNodes) {
						string fileName = fileUtilityService.RelativeToAbsolutePath(combinepath, el.Attributes["filename"].InnerText);
						if (File.Exists(fileName)) {
							fileService.OpenFile (fileName);
						}
					}
				}
				
				if (root["Views"] != null) {
					foreach (XmlElement el in root["Views"].ChildNodes) {
						foreach (IPadContent view in WorkbenchSingleton.Workbench.PadContentCollection) {
							if (el.Attributes["class"].InnerText == view.GetType().ToString() && view is IMementoCapable && el.ChildNodes.Count > 0) {
								IMementoCapable m = (IMementoCapable)view; 
								m.SetMemento((IXmlConvertable)m.CreateMemento().FromXmlElement((XmlElement)el.ChildNodes[0]));
							}
						}
					}
				}
				
				if (root["Properties"] != null) {
					IProperties properties = (IProperties)new DefaultProperties().FromXmlElement((XmlElement)root["Properties"].ChildNodes[0]);
					string name = properties.GetProperty("ActiveWindow", "");
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.ContentName != null &&
							content.ContentName == name) {
							dispatcher.GuiDispatch (new MessageHandler (content.WorkbenchWindow.SelectWindow));
							break;
						}
					}
				}
			} 
		}
		
		void SaveCombinePreferences(Combine combine, string combinefilename)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "CombinePreferences";
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			string combinepath = Path.GetDirectoryName(combinefilename);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<?xml version=\"1.0\"?>\n<UserCombinePreferences/>");
			
			XmlAttribute fileNameAttribute = doc.CreateAttribute("filename");
			fileNameAttribute.InnerText = combinefilename;
			doc.DocumentElement.Attributes.Append(fileNameAttribute);
			
			XmlElement filesnode = doc.CreateElement("Files");
			doc.DocumentElement.AppendChild(filesnode);
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.ContentName != null) {
					XmlElement el = doc.CreateElement("File");
					
					XmlAttribute attr = doc.CreateAttribute("filename");
					attr.InnerText = fileUtilityService.AbsoluteToRelativePath(combinepath, content.ContentName);
					el.Attributes.Append(attr);
					
					filesnode.AppendChild(el);
				}
			}
			
			XmlElement viewsnode = doc.CreateElement("Views");
			doc.DocumentElement.AppendChild(viewsnode);
			
			foreach (IPadContent view in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (view is IMementoCapable) {
					XmlElement el = doc.CreateElement("ViewMemento");
					
					XmlAttribute attr = doc.CreateAttribute("class");
					attr.InnerText = view.GetType().ToString();
					el.Attributes.Append(attr);
					
					el.AppendChild(((IMementoCapable)view).CreateMemento().ToXmlElement(doc));
					
					viewsnode.AppendChild(el);
				}
			}
			
			IProperties properties = new DefaultProperties();
			string name = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null ? String.Empty : WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
			properties.SetProperty("ActiveWindow", name == null ? String.Empty : name);
			
			XmlElement propertynode = doc.CreateElement("Properties");
			doc.DocumentElement.AppendChild(propertynode);
			
			propertynode.AppendChild(properties.ToXmlElement(doc));
			
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), directory + Path.DirectorySeparatorChar + combine.Name + ".xml", FileErrorPolicy.ProvideAlternative);
		}
		
		//********* own events
		protected virtual void OnCombineOpened(CombineEventArgs e)
		{
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			GenerateMakefiles ();
			if (CombineOpened != null) {
				dispatcher.GuiDispatch (new StatefulMessageHandler (dispatchOpened), e);
				//CombineOpened(this, e);
			}
		}

		void dispatchOpened (object args)
		{
			CombineOpened (this, (CombineEventArgs)args);
		}
		
		protected virtual void OnCombineClosed(CombineEventArgs e)
		{
			if (CombineClosed != null) {
				CombineClosed(this, e);
			}
		}
		
		protected virtual void OnCurrentSelectedCombineChanged(CombineEventArgs e)
		{
			if (CurrentSelectedCombineChanged != null) {
				CurrentSelectedCombineChanged(this, e);
			}
		}
		
		protected virtual void OnCurrentProjectChanged(ProjectEventArgs e)
		{
			if (CurrentSelectedProject != null) {
				StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				stringParserService.Properties["PROJECTNAME"] = CurrentSelectedProject.Name;
			}
			if (CurrentProjectChanged != null) {
				CurrentProjectChanged(this, e);
			}
		}
		
		public virtual void OnRenameProject(ProjectRenameEventArgs e)
		{
			GenerateMakefiles ();
			if (ProjectRenamed != null) {
				ProjectRenamed(this, e);
			}
		}
		
		public bool ExistsEntryWithName(string name)
		{
			ArrayList allProjects = Combine.GetAllProjects(openCombine);
			foreach (ProjectCombineEntry projectEntry in allProjects) {
				if (projectEntry.Project.Name == name) {
					return true;
				}
			}
			return false;
		}
		
		public string GetOutputAssemblyName(IProject project)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			return binding.GetCompiledOutputName(project);
		}
		
		public string GetOutputAssemblyName(string projectName)
		{
			ArrayList allProjects = Combine.GetAllProjects(CurrentOpenCombine);
			foreach (ProjectCombineEntry projectEntry in allProjects) {
				if (projectEntry.Project.Name == projectName) {
					return GetOutputAssemblyName(projectEntry.Project);
				}
			}
			return null;
		}
		
		public void RemoveFileFromProject(string fileName)
		{
			if (openCombine != null) {
				if (Directory.Exists (fileName)) {
					RemoveAllInDirectory(fileName);
				} else {
					RemoveFileFromAllProjects(fileName);
				}
				GenerateMakefiles ();
			}
		}
	
		public void OnStartBuild()
		{
			if (StartBuild != null) {
				StartBuild(this, null);
			}
		}
		
		public void OnEndBuild()
		{
			if (EndBuild != null) {
				EndBuild(this, null);
			}
		}
		public void OnBeforeStartProject()
		{
			if (BeforeStartProject != null) {
				BeforeStartProject(this, null);
			}
		}
		
		void NotifyFileRemovedFromProject (object sender, ProjectFileEventArgs e)
		{
			OnFileRemovedFromProject (e);
		}
		
		void NotifyFileAddedToProject (object sender, ProjectFileEventArgs e)
		{
			OnFileAddedToProject (e);
		}

		internal void NotifyFileChangedInProject (object sender, ProjectFileEventArgs e)
		{
				OnFileChangedInProject (e);
		}		
		
		internal void NotifyReferenceAddedToProject (object sender, ProjectReferenceEventArgs e)
		{
			OnReferenceRemovedFromProject (e);
		}
		
		internal void NotifyReferenceRemovedFromProject (object sender, ProjectReferenceEventArgs e)
		{
			OnReferenceAddedToProject (e);
		}
		
		protected virtual void OnFileRemovedFromProject (ProjectFileEventArgs e)
		{
			GenerateMakefiles ();
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject(this, e);
			}
		}

		protected virtual void OnFileAddedToProject (ProjectFileEventArgs e)
		{
			GenerateMakefiles ();
			if (FileAddedToProject != null) {
				FileAddedToProject (this, e);
			}
		}

		protected virtual void OnFileChangedInProject (ProjectFileEventArgs e)
		{
			if (FileChangedInProject != null) {
				FileChangedInProject (this, e);
			}
		}
		
		protected virtual void OnReferenceRemovedFromProject (ProjectReferenceEventArgs e)
		{
			GenerateMakefiles ();
			if (ReferenceRemovedFromProject != null) {
				ReferenceRemovedFromProject (this, e);
			}
		}
		
		protected virtual void OnReferenceAddedToProject (ProjectReferenceEventArgs e)
		{
			GenerateMakefiles ();
			if (ReferenceAddedToProject != null) {
				ReferenceAddedToProject (this, e);
			}
		}

		public string GetFileName(IProject project)
		{
			if (openCombine != null) {
				ArrayList projects = Combine.GetAllProjects(openCombine);
				foreach (ProjectCombineEntry projectCombineEntry in projects) {
					if (projectCombineEntry.Project == project) {
						return projectCombineEntry.Filename;
					}
				}
			}
			return String.Empty;
		}
		
		public string GetFileName(Combine combine)
		{
			if (combine == openCombine) {
				return openCombineFileName;
			}
			Stack combines = new Stack();
			combines.Push(openCombine);
			while (combines.Count > 0) {
				Combine curCombine = (Combine)combines.Pop();
				foreach (CombineEntry entry in curCombine.Entries) {
					CombineCombineEntry combineEntry = (CombineCombineEntry)entry;
					if (combineEntry != null) {
						if (combineEntry.Combine == combine) {
							return entry.Filename;
						}
						combines.Push(combineEntry.Combine);
					}
				}
			}
			
			return String.Empty;
		}

		public void GenerateMakefiles ()
		{
			if (openCombine != null)
				try {
					openCombine.GenerateMakefiles ();
				}
				catch { }
		}
		
		public event ProjectFileEventHandler FileRemovedFromProject;
		public event ProjectFileEventHandler FileAddedToProject;
		public event ProjectFileEventHandler FileChangedInProject;
		
		public event EventHandler     StartBuild;
		public event EventHandler     EndBuild;
		public event EventHandler     BeforeStartProject;
		
		
		public event CombineEventHandler CombineOpened;
		public event CombineEventHandler CombineClosed;
		public event CombineEventHandler CurrentSelectedCombineChanged;
		
		public event ProjectRenameEventHandler ProjectRenamed;
		public event ProjectEventHandler       CurrentProjectChanged;
		
		public event ProjectReferenceEventHandler ReferenceAddedToProject;
		public event ProjectReferenceEventHandler ReferenceRemovedFromProject;
	}
}
