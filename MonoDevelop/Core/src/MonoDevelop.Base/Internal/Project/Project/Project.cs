// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.CodeDom.Compiler;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Components;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Internal.Serialization;

namespace MonoDevelop.Internal.Project
{
	public enum NewFileSearch {
		None,
		OnLoad,
		OnLoadAutoInsert
	}
	
	/// <summary>
	/// External language bindings must extend this class
	/// </summary>
	[DataItemAttribute ("Project")]
	[DataInclude (typeof(ProjectFile))]
	public abstract class Project : CombineEntry
	{
		readonly static string currentProjectFileVersion = "1.1";
		readonly static string configurationNodeName     = "Configuration";
		
		[ItemProperty ("Description", DefaultValue="")]
		protected string description     = "";

		[ItemProperty ("newfilesearch", DefaultValue = NewFileSearch.None)]
		protected NewFileSearch newFileSearch  = NewFileSearch.None;

		[ItemProperty ("enableviewstate", DefaultValue = true)]
		protected bool enableViewState = true;
		
		ProjectFileCollection projectFiles;

		[ItemProperty ("References")]
		protected ProjectReferenceCollection projectReferences = new ProjectReferenceCollection();
		
		[ItemProperty ("DeploymentInformation")]
		protected DeployInformation deployInformation = new DeployInformation();
		
		bool isDirty = true;
		
		public Project ()
		{
			Name = "New Project";
			projectReferences.SetProject (this);
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectClass.Description}",
		                   Description = "${res:MonoDevelop.Internal.Project.ProjectClass.Description.Description}")]
		[DefaultValue("")]
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		[Browsable(false)]
		[ItemProperty ("Contents")]
		[ItemProperty ("File", Scope=1)]
		public ProjectFileCollection ProjectFiles {
			get {
				if (projectFiles != null) return projectFiles;
				return projectFiles = new ProjectFileCollection (this);
			}
		}
		
		[Browsable(false)]
		public ProjectReferenceCollection ProjectReferences {
			get {
				return projectReferences;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Project.NewFileSearch}",
		                   Description = "${res:MonoDevelop.Internal.Project.Project.NewFileSearch.Description}")]
		[DefaultValue(NewFileSearch.None)]
		public NewFileSearch NewFileSearch {
			get {
				return newFileSearch;
			}

			set {
				newFileSearch = value;
			}
		}
		
		[Browsable(false)]
		public bool EnableViewState {
			get {
				return enableViewState;
			}
			set {
				enableViewState = value;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Project.ProjectType}",
		                   Description = "${res:MonoDevelop.Internal.Project.Project.ProjectType.Description}")]
		public abstract string ProjectType {
			get;
		}
		
		[Browsable(false)]
		public DeployInformation DeployInformation {
			get {
				return deployInformation;
			}
		}

		public bool IsFileInProject(string filename)
		{
			if (filename == null) return false;
			foreach (ProjectFile file in ProjectFiles) {
				if (file.Name == filename) {
					return true;
				}
			}
			return false;
		}

		public virtual bool IsCompileable (string fileName)
		{
			return false;
		}
		
		public void SearchNewFiles()
		{
			if (newFileSearch == NewFileSearch.None) {
				return;
			}

			StringCollection newFiles   = new StringCollection();
			StringCollection collection = Runtime.FileUtilityService.SearchDirectory (BaseDirectory, "*");

			foreach (string sfile in collection) {
				string extension = Path.GetExtension(sfile).ToUpper();
				string file = Path.GetFileName (sfile);

				if (!IsFileInProject(sfile) &&
					extension != ".SCC" &&  // source safe control files -- Svante Lidmans
					extension != ".DLL" &&
					extension != ".PDB" &&
					extension != ".EXE" &&
					extension != ".CMBX" &&
					extension != ".PRJX" &&
					extension != ".SWP" &&
					extension != ".MDSX" &&
					extension != ".PIDB" &&
					!file.EndsWith ("make.sh") &&
					!file.EndsWith ("~") &&
					!file.StartsWith (".") &&
					!(Path.GetDirectoryName(sfile).IndexOf("CVS") != -1) &&
					!(Path.GetDirectoryName(sfile).IndexOf(".svn") != -1) &&
					!file.StartsWith ("Makefile") &&
					!Path.GetDirectoryName(file).EndsWith("ProjectDocumentation")) {

					newFiles.Add(sfile);
				}
			}
			
			if (newFiles.Count > 0) {
				if (newFileSearch == NewFileSearch.OnLoadAutoInsert) {
					foreach (string file in newFiles) {
						ProjectFile newFile = new ProjectFile(file);
						newFile.BuildAction = IsCompileable(file) ? BuildAction.Compile : BuildAction.Nothing;
						ProjectFiles.Add(newFile);
					}
				} else {
					new IncludeFilesDialog(this, newFiles).ShowDialog();
				}
			}
		}
		
		public static Project LoadProject (string filename)
		{
			Project prj = Runtime.ProjectService.ReadFile (filename) as Project;
			if (prj == null)
				throw new InvalidOperationException ("Invalid project file: " + filename);
			
			return prj;
		}
		
		public override void Deserialize (ITypeSerializer handler, DataCollection data)
		{
			base.Deserialize (handler, data);
			SearchNewFiles();
			projectReferences.SetProject (this);
		}

		public virtual string GetParseableFileContent(string fileName)
		{
			fileName = fileName.Replace('\\', '/'); // FIXME PEDRO
			StreamReader sr = File.OpenText(fileName);
			string content = sr.ReadToEnd();
			sr.Close();
			return content;
		}
		
		public void SaveProjectAs()
		{
			using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Save Project As..."))) {
				//fdiag.Filename = System.Environment.GetEnvironmentVariable ("HOME");

				if (fdiag.Run() == (int)Gtk.ResponseType.Ok) {
					string filename = fdiag.Filename;
					Save (filename);
					Runtime.MessageService.ShowMessage(filename, GettextCatalog.GetString ("Project saved"));
				}
				
				fdiag.Hide ();
			}
		}

		public void CopyReferencesToOutputPath(bool force)
		{
			AbstractProjectConfiguration config = ActiveConfiguration as AbstractProjectConfiguration;
			if (config == null) {
				return;
			}
			foreach (ProjectReference projectReference in ProjectReferences) {
				if ((projectReference.LocalCopy || force) && projectReference.ReferenceType != ReferenceType.Gac) {
					string referenceFileName   = projectReference.GetReferencedFileName();
					string destinationFileName = Runtime.FileUtilityService.GetDirectoryNameWithSeparator(config.OutputDirectory) + Path.GetFileName(referenceFileName);
					try {
						if (destinationFileName != referenceFileName) {
							File.Copy(referenceFileName, destinationFileName, true);
							if (File.Exists (referenceFileName + ".mdb"))
								File.Copy (referenceFileName + ".mdb", destinationFileName + ".mdb", true);
						}
					} catch (Exception e) {
						Console.WriteLine("Can't copy reference file from {0} to {1} reason {2}", referenceFileName, destinationFileName, e);
					}
				}
			}
		}
		
		public override void Dispose()
		{
			base.Dispose ();
			foreach (ProjectFile file in ProjectFiles) {
				file.Dispose ();
			}
		}
		
		public ProjectReference AddReference (string filename)
		{
			foreach (ProjectReference rInfo in ProjectReferences) {
				if (rInfo.Reference == filename) {
					return rInfo;
				}
			}
			ProjectReference newReferenceInformation = new ProjectReference (ReferenceType.Assembly, filename);
			ProjectReferences.Add (newReferenceInformation);
			return newReferenceInformation;
		}
		
		public ProjectFile AddFile (string filename, BuildAction action)
		{
			foreach (ProjectFile fInfo in ProjectFiles) {
				if (fInfo.Name == filename) {
					return fInfo;
				}
			}
			ProjectFile newFileInformation = new ProjectFile (filename, action);
			ProjectFiles.Add (newFileInformation);
			return newFileInformation;
		}
		
		public void AddFile (ProjectFile projectFile) {
			ProjectFiles.Add (projectFile);
		}

		public override void Clean ()
		{
			isDirty = true;
		}
		
		public override void Build ()
		{
			Compile ();
		}
		
		public ICompilerResult Compile ()
		{
			if (isDirty) {
				Runtime.StringParserService.Properties["Project"] = Name;
				TaskService taskService = Runtime.TaskService;
				
				Runtime.Gui.StatusBar.SetMessage(String.Format (GettextCatalog.GetString ("Compiling: {0}"), Name));
				
				DoPreBuild ();
				
				taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("------ Build started: Project: {0} Configuration: {1} ------\n\nPerforming main compilation...\n"), Name, ActiveConfiguration.Name);
				
				ICompilerResult res = DoBuild ();
				
				DoPostBuild ();
				
				isDirty = false;
				foreach (System.CodeDom.Compiler.CompilerError err in res.CompilerResults.Errors) {
					isDirty = true;
					taskService.AddTask(new Task(this, err));
				}
				
				if (taskService.Errors > 0) {
					++CombineEntry.BuildErrors;
				} else {
					++CombineEntry.BuildProjects;
				}
				
				taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("Build complete -- {0} errors, {1} warnings\n\n"), taskService.Errors.ToString (), taskService.Warnings.ToString ());
				return res;
			}
			return new DefaultCompilerResult (new CompilerResults (null), "");
		}
		
		protected virtual void DoPreBuild ()
		{
			AbstractProjectConfiguration conf = ActiveConfiguration as AbstractProjectConfiguration;
				
			// create output directory, if not exists
			string outputDir = conf.OutputDirectory;
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}
			} catch (Exception e) {
				throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
			}
			
			if (conf != null && File.Exists(conf.ExecuteBeforeBuild)) {
				Runtime.TaskService.CompilerOutput += String.Format (GettextCatalog.GetString ("Execute : {0}"), conf.ExecuteBeforeBuild);
				ProcessStartInfo ps = new ProcessStartInfo(conf.ExecuteBeforeBuild);
				ps.UseShellExecute = false;
				ps.RedirectStandardOutput = true;
				ps.WorkingDirectory = Path.GetDirectoryName(conf.ExecuteBeforeBuild);
				Process process = new Process();
				process.StartInfo = ps;
				process.Start();
				Runtime.TaskService.CompilerOutput += process.StandardOutput.ReadToEnd();
			}
		}
		
		protected virtual ICompilerResult DoBuild ()
		{
			return new DefaultCompilerResult (new CompilerResults (null), "");
		}
		
		protected virtual void DoPostBuild ()
		{
			AbstractProjectConfiguration conf = ActiveConfiguration as AbstractProjectConfiguration;

			if (conf != null && File.Exists(conf.ExecuteAfterBuild)) {
				Runtime.TaskService.CompilerOutput += String.Format (GettextCatalog.GetString ("Execute : {0}"), conf.ExecuteAfterBuild);
				ProcessStartInfo ps = new ProcessStartInfo(conf.ExecuteAfterBuild);
				ps.UseShellExecute = false;
				ps.RedirectStandardOutput = true;
				ps.WorkingDirectory = Path.GetDirectoryName(conf.ExecuteAfterBuild);
				Process process = new Process();
				process.StartInfo = ps;
				process.Start();
				Runtime.TaskService.CompilerOutput += process.StandardOutput.ReadToEnd();
			}
		}
		
		public override void Execute()
		{
			if (Runtime.TaskService.Errors != 0) return;
			
			AbstractProjectConfiguration configuration = (AbstractProjectConfiguration) ActiveConfiguration;
			if (Runtime.TaskService.Warnings != 0 && configuration != null && !configuration.RunWithWarnings)
				return;
				
			string args = configuration.CommandLineParameters;
			
			ProcessStartInfo psi;
			if (configuration.ExecuteScript != null && configuration.ExecuteScript.Length > 0) {
				string additionalCommands = "";
				if (configuration.PauseConsoleOutput)
					additionalCommands = @"echo; read -p 'press any key to continue...' -n1;";
				psi = new ProcessStartInfo("xterm",
					String.Format (@"-e ""cd {3} ; '{0}' {1} ; {2}""", configuration.ExecuteScript, args, additionalCommands, BaseDirectory));
				psi.UseShellExecute = false;
				Process p = new Process ();
				p.StartInfo = psi;
				p.Start ();
			} else {
				DoExecute ();
			}
		}
		
		
		protected virtual void DoExecute ()
		{
		}
		
		public override bool NeedsBuilding {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
			}
		}

 		internal void NotifyFileChangedInProject (ProjectFile file)
		{
			OnFileChangedInProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void NotifyFileRemovedFromProject (ProjectFile file)
		{
			OnFileRemovedFromProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void NotifyFileAddedToProject (ProjectFile file)
		{
			OnFileAddedToProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void NotifyReferenceRemovedFromProject (ProjectReference reference)
		{
			OnReferenceRemovedFromProject (new ProjectReferenceEventArgs (this, reference));
		}
		
		internal void NotifyReferenceAddedToProject (ProjectReference reference)
		{
			OnReferenceAddedToProject (new ProjectReferenceEventArgs (this, reference));
		}
		
		protected virtual void OnFileRemovedFromProject (ProjectFileEventArgs e)
		{
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject (this, e);
			}
		}
		
		protected virtual void OnFileAddedToProject (ProjectFileEventArgs e)
		{
			if (FileAddedToProject != null) {
				FileAddedToProject (this, e);
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

 		protected virtual void OnFileChangedInProject (ProjectFileEventArgs e)
		{
			if (FileChangedInProject != null) {
				FileChangedInProject (this, e);
			}
		}
		
				
		public event ProjectFileEventHandler FileRemovedFromProject;
		public event ProjectFileEventHandler FileAddedToProject;
		public event ProjectFileEventHandler FileChangedInProject;
		public event ProjectReferenceEventHandler ReferenceRemovedFromProject;
		public event ProjectReferenceEventHandler ReferenceAddedToProject;
	}
	
	public class ProjectActiveConfigurationTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context,Type sourceType)
		{
			return true;
		}
		
		public override  bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,  object value)
		{
			Project project = (Project)context.Instance;
			foreach (IConfiguration configuration in project.Configurations) {
				if (configuration.Name == value.ToString()) {
					return configuration;
				}
			}
			return null;
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			IConfiguration config = value as IConfiguration;
			Debug.Assert(config != null, String.Format("Tried to convert {0} to IConfiguration", config));
			if (config != null) {
				return config.Name;
			}
			return String.Empty;
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return new TypeConverter.StandardValuesCollection(((Project)context.Instance).Configurations);
		}
	}
}
