// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Threading;
using MonoDevelop.Internal.Serialization;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Services;

namespace MonoDevelop.Internal.Project
{
	[DataInclude (typeof(DotNetProjectConfiguration))]
	public class DotNetProject : Project
	{
		[ItemProperty]
		string language;
		
		object debugStopEvent = new object ();
		
		ILanguageBinding languageBinding;
		
		public override string ProjectType {
			get { return "DotNet"; }
		}
		
		public string LanguageName {
			get { return language; }
		}
		
		internal DotNetProject ()
		{
		}
		
		internal DotNetProject (string languageName)
		{
			language = languageName;
			languageBinding = FindLanguage (language);
		}
		
		public DotNetProject (string languageName, ProjectCreateInformation info, XmlElement projectOptions)
		{
			string binPath;
			if (info != null) {
				Name = info.ProjectName;
				binPath = info.BinPath;
			} else {
				binPath = ".";
			}
			
			language = languageName;
			languageBinding = FindLanguage (language);
			
			DotNetProjectConfiguration configuration = CreateConfiguration ();
			configuration.Name = "Debug";
			configuration.CompilationParameters = languageBinding.CreateCompilationParameters (projectOptions);
			Configurations.Add (configuration);
			
			configuration = CreateConfiguration ();
			configuration.Name = "Release";
			configuration.DebugMode = false;
			configuration.CompilationParameters = languageBinding.CreateCompilationParameters (projectOptions);
			Configurations.Add (configuration);
			
			foreach (DotNetProjectConfiguration parameter in Configurations) {
				parameter.OutputDirectory = Path.Combine (binPath, parameter.Name);
				parameter.OutputAssembly  = Name;
				
				if (projectOptions != null) {
					if (projectOptions.Attributes["Target"] != null) {
						parameter.CompileTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), projectOptions.Attributes["Target"].InnerText);
					}
					if (projectOptions.Attributes["PauseConsoleOutput"] != null) {
						parameter.PauseConsoleOutput = Boolean.Parse(projectOptions.Attributes["PauseConsoleOutput"].InnerText);
					}
				}
			}
		}
		
		public override void Deserialize (ITypeSerializer handler, DataCollection data)
		{
			base.Deserialize (handler, data);
			languageBinding = FindLanguage (language);
		}
		
		ILanguageBinding FindLanguage (string name)
		{
			ILanguageBinding binding = Runtime.Languages.GetBindingPerLanguageName (language);
			if (binding == null)
				throw new InvalidOperationException ("Language not supported: " + language);
			return binding;
		}

		protected virtual DotNetProjectConfiguration CreateConfiguration ()
		{
			return new DotNetProjectConfiguration ();
		}
		
		protected override ICompilerResult DoBuild (IProgressMonitor monitor)
		{
			DotNetProjectConfiguration conf = (DotNetProjectConfiguration) ActiveConfiguration;
			conf.SourceDirectory = BaseDirectory;
			
			foreach (ProjectFile finfo in ProjectFiles) {
				// Treat app.config in the project root directory as the application config
				if (Path.GetFileName (finfo.Name).ToUpper () == "app.config".ToUpper() &&
					Path.GetDirectoryName (finfo.Name) == BaseDirectory)
				{
					File.Copy (finfo.Name, conf.CompiledOutputName + ".config",true);
				}
			}

			ICompilerResult res = languageBinding.Compile (ProjectFiles, ProjectReferences, conf, monitor);
			CopyReferencesToOutputPath (false);
			return res;
		}
		
		public override string GetOutputFileName ()
		{
			DotNetProjectConfiguration conf = (DotNetProjectConfiguration) ActiveConfiguration;
			return conf.CompiledOutputName;
		}
		
		public override void Debug (IProgressMonitor monitor)
		{
			DotNetProjectConfiguration configuration = (DotNetProjectConfiguration) ActiveConfiguration;
			if (Runtime.DebuggingService != null) {
				Runtime.DebuggingService.StoppedEvent += new EventHandler (OnStopDebug);
				lock (debugStopEvent) {
					try {
						Runtime.DebuggingService.Run (monitor, new string[] { configuration.CompiledOutputName } );
						Monitor.Wait (debugStopEvent);
					} catch (Exception ex) {
						monitor.ReportError (null, ex);
					}
				}
				Runtime.DebuggingService.StoppedEvent -= new EventHandler (OnStopDebug);
			}
		}
		
		void OnStopDebug (object sender, EventArgs e)
		{
			lock (debugStopEvent) {
				Monitor.PulseAll (debugStopEvent);
			}
		}

		protected override void DoExecute (IProgressMonitor monitor)
		{
			CopyReferencesToOutputPath (true);
			
			DotNetProjectConfiguration configuration = (DotNetProjectConfiguration) ActiveConfiguration;
			monitor.Log.WriteLine ("Running " + configuration.CompiledOutputName + " ...");
			
			string runtimeStarter = "mono";
			
			switch (configuration.NetRuntime) {
				case NetRuntime.Mono:
					runtimeStarter = "mono";
					break;
				case NetRuntime.MonoInterpreter:
					runtimeStarter = "mint";
					break;
			}
			
			string args = string.Format (@"--debug {0} {1}", configuration.CompiledOutputName, configuration.CommandLineParameters);
			
			try {
				ProcessWrapper p;
				
				if (configuration.ExternalConsole)
					p = Runtime.ProcessService.StartConsoleProcess (
							runtimeStarter, 
							args, 
							Path.GetDirectoryName (configuration.CompiledOutputName), 
							true, configuration.PauseConsoleOutput, null);
				else
					// The use of 'sh' is a workaround. Looks like there is a bug
					// in mono, Process can't start a "mono" process.
					p = Runtime.ProcessService.StartConsoleProcess (
							"sh", 
							string.Format ("-c \"{0} {1}\"", runtimeStarter, args),
							Path.GetDirectoryName (configuration.CompiledOutputName), 
							false, false, null);
				
				monitor.CancelRequested += new MonitorHandler (new ProcessStopper (p).OnStopExecution);
				p.WaitForOutput ();
				monitor.Log.WriteLine ("The application exited with code: {0}", p.ExitCode);
			} catch (Exception ex) {
				monitor.ReportError ("Can not execute " + "\"" + configuration.CompiledOutputName + "\"", ex);
			}
		}
		
		void OnStopExecution (IProgressMonitor monitor)
		{
		}

		public override void GenerateMakefiles (Combine parentCombine)
		{
			Runtime.LoggingService.Info ("Generating makefiles for " + Name);
			languageBinding.GenerateMakefile (this, parentCombine);
		}
		
		public override bool IsCompileable(string fileName)
		{
			return languageBinding.CanCompile(fileName);
		}
		
		class ProcessStopper
		{
			Process p;
			
			public ProcessStopper (Process p)
			{
				this.p = p;
			}
			
			public void OnStopExecution (IProgressMonitor monitor)
			{
				p.Kill ();
			}
		}
	}
}
