// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Services;

namespace MonoDevelop.Internal.Project
{
	public abstract class CombineEntry : IDisposable
	{
		public static int BuildProjects = 0;
		public static int BuildErrors   = 0;
		
		object    entry;
		
		ArrayList dependencies = new ArrayList();
		
		string    filename;
		public string Filename {
			get {
				return filename;
			}
			set {
				filename = value;
			}
		}
		
		public abstract string Name {
			get;
		}
		
		public object Entry {
			get {
				return entry;
			}
		}
		
		public CombineEntry(object entry, string filename)
		{
			this.entry = entry;
			this.filename = filename;
		}
		
		public void Dispose()
		{
			if (entry is IDisposable) {
				((IDisposable)entry).Dispose();
			}
		}
		
		public abstract void Build(bool doBuildAll);
		public abstract void Execute();
		public abstract void Save();
		public abstract void Debug ();
		public abstract void GenerateMakefiles (Combine parentCombine);
		public abstract string GetOutputName ();
	}
	
	public class ProjectCombineEntry : CombineEntry
	{
		IProject project;
		bool     isDirty = true;
		
		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public override string Name {
			get {
				return project.Name;
			}
		}
		
		public ProjectCombineEntry(IProject project, string filename) : base(project, filename)
		{
			this.project = project;
		}
		
				
		public override void Build(bool doBuildAll)
		{ // if you change something here look at the DefaultProjectService BeforeCompile method
			if (doBuildAll || isDirty) {
				StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				stringParserService.Properties["Project"] = Name;
				IProjectService   projectService   = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
				TaskService       taskService      = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				
				statusBarService.SetMessage(String.Format (GettextCatalog.GetString ("Compiling: {0}"), Project.Name));
				LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
				
				// create output directory, if not exists
				string outputDir = ((AbstractProjectConfiguration)project.ActiveConfiguration).OutputDirectory;
				try {
					DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
					if (!directoryInfo.Exists) {
						directoryInfo.Create();
					}
				} catch (Exception e) {
					throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
				}
				
				ILanguageBinding csc = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
				
				AbstractProjectConfiguration conf = project.ActiveConfiguration as AbstractProjectConfiguration;

				taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("------ Build started: Project: {0} Configuration: {1} ------\n\nPerforming main compilation...\n"), Project.Name, Project.ActiveConfiguration.Name);
				
				if (conf != null && File.Exists(conf.ExecuteBeforeBuild)) {
					taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("Execute : {0}"), conf.ExecuteBeforeBuild);
					ProcessStartInfo ps = new ProcessStartInfo(conf.ExecuteBeforeBuild);
					ps.UseShellExecute = false;
					ps.RedirectStandardOutput = true;
					ps.WorkingDirectory = Path.GetDirectoryName(conf.ExecuteBeforeBuild);
					Process process = new Process();
					process.StartInfo = ps;
					process.Start();
					taskService.CompilerOutput += process.StandardOutput.ReadToEnd();
				}
				
				ICompilerResult res = csc.CompileProject(project);
				
				if (conf != null && File.Exists(conf.ExecuteAfterBuild)) {
					taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("Execute : {0}"), conf.ExecuteAfterBuild);
					ProcessStartInfo ps = new ProcessStartInfo(conf.ExecuteAfterBuild);
					ps.UseShellExecute = false;
					ps.RedirectStandardOutput = true;
					ps.WorkingDirectory = Path.GetDirectoryName(conf.ExecuteAfterBuild);
					Process process = new Process();
					process.StartInfo = ps;
					process.Start();
					taskService.CompilerOutput += process.StandardOutput.ReadToEnd();
				}
				
				isDirty = false;
				foreach (CompilerError err in res.CompilerResults.Errors) {
					isDirty = true;
					taskService.AddTask(new Task(project, err));
				}
				
				if (taskService.Errors > 0) {
					++BuildErrors;
				} else {
					++BuildProjects;
				}
				
				taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("Build complete -- {0} errors, {1} warnings\n\n"), taskService.Errors.ToString (), taskService.Warnings.ToString ());
			}
		}
		
		public override void Execute()
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			if (binding == null) {
				throw new ApplicationException("can't find language binding for project ");
			}
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			
			if (taskService.Errors == 0) {
				if (taskService.Warnings == 0 || project.ActiveConfiguration != null && ((AbstractProjectConfiguration)project.ActiveConfiguration).RunWithWarnings) {
					project.CopyReferencesToOutputPath (true);
					binding.Execute(project);
				}
			}

		}

		public override void Debug ()
		{
			LanguageBindingService langBindingServ = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService (typeof (LanguageBindingService));
			ILanguageBinding binding = langBindingServ.GetBindingPerLanguageName (project.ProjectType);
			if (binding == null) {
				Console.WriteLine ("Language binding unknown");
				return;
			}
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService (typeof (TaskService));

			if (taskService.Errors == 0)
				binding.DebugProject (project);
		}
		
		public override void Save()
		{
			project.SaveProject(Filename);
		}

		public override void GenerateMakefiles (Combine parentCombine)
		{
			Console.WriteLine ("Generating makefiles for " + Name);
			LanguageBindingService languageBindingService = (LanguageBindingService)ServiceManager.GetService(typeof(LanguageBindingService));
			ILanguageBinding langBinding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			langBinding.GenerateMakefile (project, parentCombine);
		}

		public override string GetOutputName ()
		{
			LanguageBindingService lbs = (LanguageBindingService)ServiceManager.GetService (typeof (LanguageBindingService));
			ILanguageBinding langBinding = lbs.GetBindingPerLanguageName (project.ProjectType);
			return System.IO.Path.GetFileName (langBinding.GetCompiledOutputName (project));
		}
	}
	
	public class CombineCombineEntry : CombineEntry
	{
		Combine combine;
		
		public Combine Combine {
			get {
				return combine;
			}
		}
		public override string Name {
			get {
				return combine.Name;
			}
		}
		
		public CombineCombineEntry(Combine combine, string filename) : base(combine, filename)
		{
			this.combine = combine;
		}
		
		public override void Build(bool doBuildAll)
		{
			combine.Build(doBuildAll);
		}
		
		public override void Execute()
		{
			combine.Execute();
		}
		
		public override void Save()
		{
			combine.SaveCombine(System.IO.Path.GetFullPath (Filename));
			combine.SaveAllProjects();
		}

		public override void Debug ()
		{
			combine.Debug ();
		}

		public override void GenerateMakefiles (Combine parentCombine)
		{
		}

		public override string GetOutputName ()
		{
			return String.Empty;
		}
	}

	
	public interface ICombineEntryCollection: IEnumerable
	{
		int Count { get; }
		CombineEntry this [int n] { get; }
		IEnumerator GetEnumerator ();
	}
	
	public class CombineEntryCollection: ICombineEntryCollection
	{
		ArrayList list = new ArrayList ();
		
		public int Count
		{
			get { return list.Count; }
		}
		
		public CombineEntry this [int n]
		{
			get { return (CombineEntry) list[n]; }
		}
		
		public IEnumerator GetEnumerator ()
		{
			return list.GetEnumerator ();
		}
		
		public void Add (CombineEntry entry)
		{
			list.Add (entry);
		}
		
		public void Remove (CombineEntry entry)
		{
			list.Remove (entry);
		}
		
		public int IndexOf (CombineEntry entry)
		{
			return list.IndexOf (entry);
		}
		
		public void Clear ()
		{
			list.Clear ();
		}
	}
}
