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
using System.Threading;

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
	
	public class ProjectService : AbstractService, IProjectService
	{
		Project currentProject = null;
		Combine  currentCombine = null;
		Combine  openCombine    = null;
		DataContext dataContext = new DataContext ();
		ProjectBindingCodon[] projectBindings;
		
		IAsyncOperation currentBuildOperation;
		IAsyncOperation currentRunOperation;
		
		FileFormatManager formatManager = new FileFormatManager ();
		IFileFormat defaultProjectFormat = new MdpFileFormat ();
		IFileFormat defaultCombineFormat = new MdsFileFormat ();
		
		ICompilerResult lastResult = new DefaultCompilerResult ();
			
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
		
		public ICompilerResult LastCompilerResult {
			get { return lastResult; }
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
		
		public CombineEntry ReadFile (string file, IProgressMonitor monitor)
		{
			IFileFormat format = formatManager.GetFileFormat (file);

			if (format == null)
				throw new InvalidOperationException ("Unknown file format: " + file);
			
			CombineEntry obj = format.ReadFile (file, monitor) as CombineEntry;
			if (obj == null)
				throw new InvalidOperationException ("Invalid file format: " + file);
			
			if (obj.FileFormat == null)	
				obj.FileFormat = format;

			return obj;
		}
		
		public void WriteFile (string file, CombineEntry entry, IProgressMonitor monitor)
		{
			IFileFormat format = entry.FileFormat;
			if (format == null) {
				if (entry is Project) format = defaultProjectFormat;
				else if (entry is Combine) format = defaultCombineFormat;
				else format = formatManager.GetFileFormatForObject (entry);
				
				if (format == null)
					throw new InvalidOperationException ("FileFormat not provided for combine entry '" + entry.Name + "'");
				entry.FileName = format.GetValidFormatName (file);
			}
			entry.FileName = file;
			format.WriteFile (entry.FileName, entry, monitor);
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
		
		public IAsyncOperation OpenCombine(string filename)
		{
			if (openCombine != null) {
				SaveCombine();
				CloseCombine();
			}

			if (filename.StartsWith ("file://"))
				filename = filename.Substring (7);

			IProgressMonitor monitor = Runtime.TaskService.GetLoadProgressMonitor ();
			
			object[] data = new object[] { filename, monitor };
			Runtime.DispatchService.BackgroundDispatch (new StatefulMessageHandler (backgroundLoadCombine), data);
			return monitor.AsyncOperation;
		}
		
		void backgroundLoadCombine (object arg)
		{
			object[] data = (object[]) arg;
			string filename = data[0] as string;
			IProgressMonitor monitor = data [1] as IProgressMonitor;
			
			try {
				if (!fileUtilityService.TestFileExists(filename)) {
					monitor.ReportError (string.Format (GettextCatalog.GetString ("File not found: {0}"), filename), null);
					return;
				}
				
				string validcombine = Path.ChangeExtension (filename, ".mds");
				
				if (Path.GetExtension (filename).ToLower() != ".mds") {
					if (File.Exists (validcombine))
						filename = validcombine;
				} else if (Path.GetExtension (filename).ToLower () != ".cmbx") {
					if (File.Exists (Path.ChangeExtension (filename, ".cmbx")))
						filename = Path.ChangeExtension (filename, ".cmbx");
				}
			
				CombineEntry entry = ReadFile (filename, monitor);
				if (!(entry is Combine)) {
					Combine loadingCombine = new Combine();
					loadingCombine.Entries.Add (entry);
					loadingCombine.Name = entry.Name;
					loadingCombine.Save (validcombine, monitor);
					entry = loadingCombine;
				}
			
				openCombine = (Combine) entry;
				
				Runtime.FileService.RecentOpen.AddLastProject (filename, openCombine.Name);
				
				openCombine.FileAddedToProject += new ProjectFileEventHandler (NotifyFileAddedToProject);
				openCombine.FileRemovedFromProject += new ProjectFileEventHandler (NotifyFileRemovedFromProject);
				openCombine.FileRenamedInProject += new ProjectFileRenamedEventHandler (NotifyFileRenamedInProject);
				openCombine.FileChangedInProject += new ProjectFileEventHandler (NotifyFileChangedInProject);
				openCombine.ReferenceAddedToProject += new ProjectReferenceEventHandler (NotifyReferenceAddedToProject);
				openCombine.ReferenceRemovedFromProject += new ProjectReferenceEventHandler (NotifyReferenceRemovedFromProject);
				
				SearchForNewFiles ();
		
				OnCombineOpened(new CombineEventArgs(openCombine));

				Runtime.DispatchService.GuiDispatch (new StatefulMessageHandler (RestoreCombinePreferences), CurrentOpenCombine);
				
				SaveCombine ();
				monitor.ReportSuccess (GettextCatalog.GetString ("Combine loaded."));
			} catch (Exception ex) {
				monitor.ReportError ("Load operation failed.", ex);
			} finally {
				monitor.Dispose ();
			}
		}
		
		void SearchForNewFiles ()
		{
			foreach (Project p in openCombine.GetAllProjects()) {
				if (p.NewFileSearch != NewFileSearch.None)
					p.SearchNewFiles ();
			}
		}
		
		public void SaveCombine()
		{
			IProgressMonitor monitor = Runtime.TaskService.GetSaveProgressMonitor ();
			try {
				openCombine.Save (monitor);
				monitor.ReportSuccess (GettextCatalog.GetString ("Combine saved."));
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Save failed."), ex);
			} finally {
				monitor.Dispose ();
			}
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

		public IAsyncOperation ExecuteActiveCombine ()
		{
			if (openCombine == null) return NullAsyncOperation.Success;
			if (currentRunOperation != null && !currentRunOperation.IsCompleted) return currentRunOperation;
			IProgressMonitor monitor = new NullProgressMonitor ();
			Runtime.DispatchService.ThreadDispatch (new StatefulMessageHandler (ExecuteActiveCombineAsync), monitor);
			currentRunOperation = monitor.AsyncOperation;
			return currentRunOperation;
		}
		
		void ExecuteActiveCombineAsync (object ob)
		{
			IProgressMonitor monitor = (IProgressMonitor) ob;

			OnBeforeStartProject ();
			try {
				openCombine.Execute (monitor);
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Execution failed."), ex);
			} finally {
				monitor.Dispose ();
			}
		}

		public IAsyncOperation ExecuteProject (Project project)
		{
			IProgressMonitor monitor = new NullProgressMonitor ();
			Runtime.DispatchService.ThreadDispatch (new StatefulMessageHandler (ExecuteProjectAsync), new object[] {project, monitor});
			return monitor.AsyncOperation;
		}
		
		void ExecuteProjectAsync (object ob)
		{
			object[] data = (object[]) ob;
			Project project = (Project) data[0];
			IProgressMonitor monitor = (IProgressMonitor) data[1];
			OnBeforeStartProject ();
			try {
				project.Execute (monitor);
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Execution failed."), ex);
			} finally {
				monitor.Dispose ();
			}
		}
		
		class ProjectOperationHandler {
			public Project Project;
			public void Run (IAsyncOperation op) { Project.Dispose (); }
		}
		
		public IAsyncOperation BuildFile (string file)
		{
			Project tempProject = CreateSingleFileProject (file);
			if (tempProject != null) {
				IAsyncOperation aop = BuildProject (tempProject);
				ProjectOperationHandler h = new ProjectOperationHandler ();
				h.Project = tempProject;
				aop.Completed += new OperationHandler (h.Run);
				return aop;
			} else {
				Runtime.MessageService.ShowError (string.Format (GettextCatalog.GetString ("The current file {0} can't be compiled."), file));
				return NullAsyncOperation.Failure;
			}
		}
		
		public IAsyncOperation ExecuteFile (string file)
		{
			Project tempProject = CreateSingleFileProject (file);
			if (tempProject != null) {
				IAsyncOperation aop = ExecuteProject (tempProject);
				ProjectOperationHandler h = new ProjectOperationHandler ();
				h.Project = tempProject;
				aop.Completed += new OperationHandler (h.Run);
				return aop;
			} else {
				Runtime.MessageService.ShowError(GettextCatalog.GetString ("No runnable executable found."));
				return NullAsyncOperation.Failure;
			}
		}
	
		public IAsyncOperation BuildActiveCombine ()
		{
			if (openCombine == null) return NullAsyncOperation.Success;
			if (currentBuildOperation != null && !currentBuildOperation.IsCompleted) return currentBuildOperation;
			
			DoBeforeCompileAction();
			
			IProgressMonitor monitor = Runtime.TaskService.GetBuildProgressMonitor ();			
			Runtime.DispatchService.ThreadDispatch (new StatefulMessageHandler (BuildActiveCombineAsync), monitor);
			currentBuildOperation = monitor.AsyncOperation;
			return currentBuildOperation;
		}
		
		void BuildActiveCombineAsync (object ob)
		{
			IProgressMonitor monitor = (IProgressMonitor) ob;
			try {
				BeginBuild ();
				ICompilerResult result = openCombine.Build (monitor);
				BuildDone (monitor, result);
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Build failed."), ex);
			} finally {
				monitor.Dispose ();
			}
		}
		
		public IAsyncOperation RebuildActiveCombine()
		{
			if (openCombine == null) return NullAsyncOperation.Success;
			openCombine.Clean ();
			return BuildActiveCombine ();
		}
		
		public IAsyncOperation BuildActiveProject ()
		{
			if (CurrentSelectedProject == null) {
				Runtime.MessageService.ShowError (GettextCatalog.GetString ("Active project not set."));
				return NullAsyncOperation.Failure;
			}
				
			return BuildProject (CurrentSelectedProject);
		}
		
		public IAsyncOperation RebuildActiveProject ()
		{
			return RebuildProject (CurrentSelectedProject);
		}
		
		public IAsyncOperation BuildProject (Project project)
		{
			BeforeCompile (project);
			IProgressMonitor monitor = Runtime.TaskService.GetBuildProgressMonitor ();
			Runtime.DispatchService.ThreadDispatch (new StatefulMessageHandler (BuildProjectAsync), new object[] {project, monitor});
			return monitor.AsyncOperation;
		}
		
		public void BuildProjectAsync (object ob)
		{
			object[] data = (object[]) ob;
			Project project = (Project) data [0];
			IProgressMonitor monitor = (IProgressMonitor) data [1];
			ICompilerResult result = null;
			try {
				BeginBuild ();
				result = project.Build (monitor);
				BuildDone (monitor, result);
			} catch (Exception ex) {
				monitor.ReportError (GettextCatalog.GetString ("Build failed."), ex);
			} finally {
				monitor.Dispose ();
			}
		}
		
		public IAsyncOperation RebuildProject (Project project)
		{
			project.Clean ();
			return BuildProject (project);
		}
		
		void BeginBuild ()
		{
			Runtime.TaskService.ClearTasks();
			OnStartBuild ();
		}
		
		void BuildDone (IProgressMonitor monitor, ICompilerResult result)
		{
			lastResult = result;
			monitor.Log.WriteLine ();
			monitor.Log.WriteLine (String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------")));
			
			foreach (CompilerError err in result.CompilerResults.Errors) {
				Runtime.TaskService.AddTask (new Task(null, err));
			}
			
			if (result.ErrorCount == 0 && result.WarningCount == 0 && lastResult.FailedBuildCount == 0) {
				monitor.ReportSuccess (GettextCatalog.GetString ("Build successful."));
			} else if (result.ErrorCount == 0 && result.WarningCount > 0) {
				monitor.ReportWarning (String.Format (GettextCatalog.GetString ("Build: {0} errors, {1} warnings."), result.ErrorCount, result.WarningCount));
			} else if (result.ErrorCount > 0) {
				monitor.ReportError (String.Format (GettextCatalog.GetString ("Build: {0} errors, {1} warnings."), result.ErrorCount, result.WarningCount), null);
			} else {
				monitor.ReportError (String.Format (GettextCatalog.GetString ("Build failed.")), null);
			}
			
			OnEndBuild (lastResult.FailedBuildCount == 0);
		}
		
		void BeforeCompile (Project project)
		{
			DoBeforeCompileAction();
			
			// cut&pasted from CombineEntry.cs
			Runtime.StringParserService.Properties["Project"] = project.Name;
			
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
			
			FileFormatCodon[] formatCodons = (FileFormatCodon[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/ProjectFileFormats").BuildChildItems(null)).ToArray(typeof(FileFormatCodon));
			foreach (FileFormatCodon codon in formatCodons)
				formatManager.RegisterFileFormat (codon.FileFormat);
			
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
		
		void RestoreCombinePreferences (object data)
		{
			Combine combine = (Combine) data;
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
							Runtime.FileService.OpenFile (fileName, false);
						}
					}
				}
				
				if (root["Views"] != null) {
					foreach (XmlElement el in root["Views"].ChildNodes) {
						foreach (IPadContent view in WorkbenchSingleton.Workbench.PadContentCollection) {
							if (el.GetAttribute ("Id") == view.Id && view is IMementoCapable && el.ChildNodes.Count > 0) {
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
					el.SetAttribute ("Id", view.Id);
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
		
		
		void OnStartBuild()
		{
			if (StartBuild != null) {
				StartBuild(this, null);
			}
		}
		
		void OnEndBuild (bool success)
		{
			if (EndBuild != null) {
				EndBuild(success);
			}
		}
		
		void OnBeforeStartProject()
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

		internal void NotifyFileRenamedInProject (object sender, ProjectFileRenamedEventArgs e)
		{
			OnFileRenamedInProject (e);
		}		
		
		internal void NotifyFileChangedInProject (object sender, ProjectFileEventArgs e)
		{
			OnFileChangedInProject (e);
		}		
		
		internal void NotifyReferenceAddedToProject (object sender, ProjectReferenceEventArgs e)
		{
			OnReferenceAddedToProject (e);
		}
		
		internal void NotifyReferenceRemovedFromProject (object sender, ProjectReferenceEventArgs e)
		{
			OnReferenceRemovedFromProject (e);
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

		protected virtual void OnFileRenamedInProject (ProjectFileRenamedEventArgs e)
		{
			if (FileRenamedInProject != null) {
				FileRenamedInProject (this, e);
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
		public event ProjectFileRenamedEventHandler FileRenamedInProject;
		
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
