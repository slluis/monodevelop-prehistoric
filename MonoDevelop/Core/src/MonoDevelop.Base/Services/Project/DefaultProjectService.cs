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
using MonoDevelop.Internal.Serialization;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Core.AddIns;

namespace MonoDevelop.Services
{
	
	public enum BeforeCompileAction {
		Nothing,
		SaveAllFiles,
		PromptForSave,
	}
	
	public class DefaultProjectService : AbstractService, IProjectService
	{
		Project currentProject = null;
		Combine  currentCombine = null;
		Combine  openCombine    = null;
		DataContext dataContext = new DataContext ();
		ProjectBindingCodon[] projectBindings;
		
		FileFormatManager formatManager = new FileFormatManager ();
		IFileFormat defaultProjectFormat = new PrjxFileFormat ();
		IFileFormat defaultCombineFormat = new CmbxFileFormat ();
			
		public Project CurrentSelectedProject {
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
				CombineEntryCollection projects = openCombine.GetAllProjects();
				
				foreach (Project projectEntry in projects) {
					foreach (ProjectFile fInfo in projectEntry.ProjectFiles) {
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
		
		public DataContext DataContext {
			get { return dataContext; }
		}
		
		public FileFormatManager FileFormats {
			get { return formatManager; }
		}
		
		public void SaveCombinePreferences()
		{
			if (CurrentOpenCombine != null)
				SaveCombinePreferences(CurrentOpenCombine);
		}
		
		public CombineEntry ReadFile (string file)
		{
			IFileFormat format = formatManager.GetFileFormat (file);

			if (format == null)
				throw new InvalidOperationException ("Unknown file format: " + file);
			
			CombineEntry obj = format.ReadFile (file) as CombineEntry;
			if (obj == null)
				throw new InvalidOperationException ("Invalid file format: " + file);
				
			obj.FileFormat = format;
			return obj;
		}
		
		public void WriteFile (string file, CombineEntry entry)
		{
			IFileFormat format = entry.FileFormat;
			if (format == null) {
				if (entry is Project) format = defaultProjectFormat;
				else if (entry is Combine) format = defaultCombineFormat;
				else format = formatManager.GetFileFormatForObject (entry);
				
				if (format == null)
					throw new InvalidOperationException ("FileFormat not provided for combine entry '" + entry.Name + "'");
			}

			format.WriteFile (file, entry);
		}
		
		public Project CreateSingleFileProject (string file)
		{
			foreach (ProjectBindingCodon projectBinding in projectBindings) {
				Project project = projectBinding.ProjectBinding.CreateSingleFileProject (file);
				if (project != null)
					return project;
			}
			return null;
		}
		
		public Project CreateProject (string type, ProjectCreateInformation info, XmlElement projectOptions)
		{
			foreach (ProjectBindingCodon projectBinding in projectBindings) {
				if (projectBinding.ProjectBinding.Name == type) {
					Project project = projectBinding.ProjectBinding.CreateProject (info, projectOptions);
					return project;
				}
			}
			return null;
		}
		
		public void CloseCombine()
		{
			CloseCombine(true);
		}

		public void CloseCombine (bool saveCombinePreferencies)
		{
			if (CurrentOpenCombine != null) {
				if (saveCombinePreferencies)
					SaveCombinePreferences (CurrentOpenCombine);
				Combine closedCombine = CurrentOpenCombine;
				CurrentSelectedProject = null;
				CurrentOpenCombine = CurrentSelectedCombine = null;
				WorkbenchSingleton.Workbench.CloseAllViews();
				OnCombineClosed(new CombineEventArgs(closedCombine));
				closedCombine.Dispose();
			}
		}
		
		FileUtilityService fileUtilityService = Runtime.FileUtilityService;
		
		public bool IsCombineEntryFile (string filename)
		{
			if (filename.StartsWith ("file://"))
				filename = filename.Substring (7);
				
			IFileFormat format = formatManager.GetFileFormat (filename);
			return format != null;
		}
		
		public void OpenCombine(string filename)
		{
			if (openCombine != null) {
				SaveCombine();
				CloseCombine();
			}

			if (filename.StartsWith ("file://"))
				filename = filename.Substring (7);
				
			Runtime.Gui.StatusBar.SetMessage(GettextCatalog.GetString ("Opening Combine..."));

			LoadCombine (filename);
			
			Runtime.Gui.StatusBar.SetMessage(GettextCatalog.GetString ("Ready"));
		}
		
		void LoadCombine(string filename)
		{
			Runtime.DispatchService.BackgroundDispatch (new StatefulMessageHandler (backgroundLoadCombine), filename);
		}

		void backgroundLoadCombine (object arg)
		{
			string filename = arg as string;
			if (!fileUtilityService.TestFileExists(filename)) {
				return;
			}
			
			string validcombine = Path.ChangeExtension (filename, ".cmbx");
			
			if (Path.GetExtension (filename).ToLower() != ".cmbx") {
				if (File.Exists (validcombine))
					filename = validcombine;
			}
			
			CombineEntry entry = ReadFile (filename);
			if (!(entry is Combine)) {
				Combine loadingCombine = new Combine();
				loadingCombine.Entries.Add (entry);
				loadingCombine.Name = entry.Name;
				loadingCombine.Save (validcombine);
				entry = loadingCombine;
			}
		
			openCombine = (Combine) entry;
			
			Runtime.FileService.RecentOpen.AddLastProject (filename, openCombine.Name);
			
			OnCombineOpened(new CombineEventArgs(openCombine));
			openCombine.FileAddedToProject += new ProjectFileEventHandler (NotifyFileAddedToProject);
			openCombine.FileRemovedFromProject += new ProjectFileEventHandler (NotifyFileRemovedFromProject);
			openCombine.FileChangedInProject += new ProjectFileEventHandler (NotifyFileChangedInProject);
			openCombine.ReferenceAddedToProject += new ProjectReferenceEventHandler (NotifyReferenceAddedToProject);
			openCombine.ReferenceRemovedFromProject += new ProjectReferenceEventHandler (NotifyReferenceRemovedFromProject);
	
			RestoreCombinePreferences (CurrentOpenCombine);
		}
		
		public void SaveCombine()
		{
			openCombine.Save ();
		}
		
		public void MarkFileDirty (string filename)
		{
			if (openCombine != null) {
				Project entry = openCombine.GetProjectEntryContaining (filename);
				if (entry != null) {
					entry.NeedsBuilding = true;
				}
			}
		}

		public void CompileCombine()
		{
			if (openCombine != null) {
				DoBeforeCompileAction();
				Runtime.TaskService.ClearTasks();
				
				openCombine.Build ();
			}
		}
		
		public void RecompileAll()
		{
			if (openCombine != null) {
				DoBeforeCompileAction();
				Runtime.TaskService.ClearTasks();
				
				openCombine.Clean ();
				openCombine.Build ();
			}
		}
		
		void BeforeCompile (Project project)
		{
			DoBeforeCompileAction();
			
			Runtime.TaskService.NotifyTaskChange();
			
			// cut&pasted from CombineEntry.cs
			Runtime.StringParserService.Properties["Project"] = project.Name;
			Runtime.Gui.StatusBar.SetMessage(String.Format (GettextCatalog.GetString ("Compiling {0}"), project.Name));
			
			string outputDir = ((AbstractProjectConfiguration)project.ActiveConfiguration).OutputDirectory;
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}
			} catch (Exception e) {
				throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
			}
		}
		
		void AfterCompile (Project project, ICompilerResult res)
		{
			// cut&pasted from CombineEntry.cs
			foreach (CompilerError err in res.CompilerResults.Errors) {
				Runtime.TaskService.AddTask(new Task(project, err));
			}
			
			if (Runtime.TaskService.Errors > 0) {
				++CombineEntry.BuildErrors;
			} else {
				++CombineEntry.BuildProjects;
			}
			
			Runtime.TaskService.CompilerOutput = res.CompilerOutput;
			Runtime.TaskService.NotifyTaskChange();
		}
		
		public void RecompileProject(Project project)
		{
			project.Clean ();
			BeforeCompile (project);
			AfterCompile(project, project.Compile ());
		}
		
		public void CompileProject(Project project)
		{
			BeforeCompile (project);
			AfterCompile(project, project.Compile ());
		}
		
		void DoBeforeCompileAction()
		{
			BeforeCompileAction action = (BeforeCompileAction)Runtime.Properties.GetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			
			switch (action) {
				case BeforeCompileAction.Nothing:
					break;
				case BeforeCompileAction.PromptForSave:
					bool save = false;
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.ContentName != null && content.IsDirty) {
							if (!save) {
								if (Runtime.MessageService.AskQuestion(GettextCatalog.GetString ("Save changed files?"))) {
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
		
		void RemoveFileFromAllProjects(string fileName)
		{
			CombineEntryCollection projects = openCombine.GetAllProjects();
			
			restart:
			foreach (Project projectEntry in projects) {
				foreach (ProjectReference rInfo in projectEntry.ProjectReferences) {
					if (rInfo.ReferenceType == ReferenceType.Assembly && rInfo.Reference == fileName) {
						projectEntry.ProjectReferences.Remove(rInfo);
						goto restart;
					}
				}
				foreach (ProjectFile fInfo in projectEntry.ProjectFiles) {
					if (fInfo.Name == fileName) {
						projectEntry.ProjectFiles.Remove(fInfo);
						goto restart;
					}
				}
			}
		}
		
		void RemoveAllInDirectory(string dirName)
		{
			CombineEntryCollection projects = openCombine.GetAllProjects();
			
			restart:
			foreach (Project projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.ProjectFiles) {
					if (fInfo.Name.StartsWith(dirName)) {
						projectEntry.ProjectFiles.Remove(fInfo);
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
			CombineEntryCollection projects = openCombine.GetAllProjects();
			
			foreach (Project projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.ProjectFiles) {
					if (fInfo.Name == oldName) {
						fInfo.Name = newName;
					}
				}
			}
		}

		void RenameDirectoryInAllProjects(string oldName, string newName)
		{
			CombineEntryCollection projects = openCombine.GetAllProjects();
			
			foreach (Project projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.ProjectFiles) {
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

			formatManager.RegisterFileFormat (defaultProjectFormat);
			formatManager.RegisterFileFormat (defaultCombineFormat);
			
			DataContext.IncludeType (typeof(Combine));
			DataContext.IncludeType (typeof(Project));
			DataContext.IncludeType (typeof(DotNetProject));
			
			Runtime.FileService.FileRemoved += new FileEventHandler(CheckFileRemove);
			Runtime.FileService.FileRenamed += new FileEventHandler(CheckFileRename);
			
			projectBindings = (ProjectBindingCodon[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/ProjectBindings").BuildChildItems(null)).ToArray(typeof(ProjectBindingCodon));
		}
		
		string MakeValidName(string str)
		{
			string tmp = "";
			foreach (char ch in str) {
				tmp += ((byte)ch).ToString();
			}
			return tmp;
		}
		
		void RestoreCombinePreferences (Combine combine)
		{
			string combinefilename = combine.FileName;
			string directory = Runtime.Properties.ConfigDirectory + "CombinePreferences";

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
					foreach (XmlElement el in root["Files"].ChildNodes) {
						string fileName = fileUtilityService.RelativeToAbsolutePath(combinepath, el.Attributes["filename"].InnerText);
						if (File.Exists(fileName)) {
							Runtime.FileService.OpenFile (fileName);
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
							Runtime.DispatchService.GuiDispatch (new MessageHandler (content.WorkbenchWindow.SelectWindow));
							break;
						}
					}
				}
			} 
		}
		
		void SaveCombinePreferences (Combine combine)
		{
			string combinefilename = combine.FileName;
			string directory = Runtime.Properties.ConfigDirectory + "CombinePreferences";

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
			if (CombineOpened != null) {
				CombineOpened(this, e);
			}
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
				Runtime.StringParserService.Properties["PROJECTNAME"] = CurrentSelectedProject.Name;
			}
			if (CurrentProjectChanged != null) {
				CurrentProjectChanged(this, e);
			}
		}
		
		public Project GetProject (string projectName)
		{
			if (CurrentOpenCombine == null) return null;
			CombineEntryCollection allProjects = CurrentOpenCombine.GetAllProjects();
			foreach (Project project in allProjects) {
				if (project.Name == projectName)
					return project;
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
			}
		}
	
		public void OnStartBuild()
		{
			if (StartBuild != null) {
				StartBuild(this, null);
			}
		}
		
		public void OnEndBuild(bool success)
		{
			if (EndBuild != null) {
				EndBuild(success);
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
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject(this, e);
			}
		}

		protected virtual void OnFileAddedToProject (ProjectFileEventArgs e)
		{
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
			if (ReferenceRemovedFromProject != null) {
				ReferenceRemovedFromProject (this, e);
			}
		}
		
		protected virtual void OnReferenceAddedToProject (ProjectReferenceEventArgs e)
		{
			if (ReferenceAddedToProject != null) {
				ReferenceAddedToProject (this, e);
			}
		}

		public event ProjectFileEventHandler FileRemovedFromProject;
		public event ProjectFileEventHandler FileAddedToProject;
		public event ProjectFileEventHandler FileChangedInProject;
		
		public event EventHandler     StartBuild;
		public event ProjectCompileEventHandler EndBuild;
		public event EventHandler     BeforeStartProject;
		
		
		public event CombineEventHandler CombineOpened;
		public event CombineEventHandler CombineClosed;
		public event CombineEventHandler CurrentSelectedCombineChanged;
		
		public event ProjectEventHandler       CurrentProjectChanged;
		
		public event ProjectReferenceEventHandler ReferenceAddedToProject;
		public event ProjectReferenceEventHandler ReferenceRemovedFromProject;
	}
}
