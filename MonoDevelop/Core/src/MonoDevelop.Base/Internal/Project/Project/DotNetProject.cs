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
		
		protected override ICompilerResult DoBuild ()
		{
			DotNetProjectConfiguration conf = (DotNetProjectConfiguration) ActiveConfiguration;
			foreach (ProjectFile finfo in ProjectFiles) {
				// Treat app.config in the project root directory as the application config
				if (Path.GetFileName (finfo.Name).ToUpper () == "app.config".ToUpper() &&
					Path.GetDirectoryName (finfo.Name) == BaseDirectory)
				{
					File.Copy (finfo.Name, conf.CompiledOutputName + ".config",true);
				}
			}

			ICompilerResult res = languageBinding.Compile (ProjectFiles, ProjectReferences, conf);
			CopyReferencesToOutputPath (false);
			return res;
		}
		
		public override string GetOutputFileName ()
		{
			DotNetProjectConfiguration conf = (DotNetProjectConfiguration) ActiveConfiguration;
			return conf.CompiledOutputName;
		}
		
		public override void Debug ()
		{
			if (Runtime.TaskService.Errors != 0) return;

			DotNetProjectConfiguration configuration = (DotNetProjectConfiguration) ActiveConfiguration;
			if (Runtime.DebuggingService != null)
				Runtime.DebuggingService.Run (new string[] { configuration.CompiledOutputName } );
		}

		protected override void DoExecute ()
		{
			CopyReferencesToOutputPath (true);
			
			DotNetProjectConfiguration configuration = (DotNetProjectConfiguration) ActiveConfiguration;
			string args = configuration.CommandLineParameters;
			
			ProcessStartInfo psi;
			string runtimeStarter = "mono --debug ";
			
			switch (configuration.NetRuntime) {
				case NetRuntime.Mono:
					runtimeStarter = "mono --debug ";
					break;
				case NetRuntime.MonoInterpreter:
					runtimeStarter = "mint ";
					break;
			}
			
			string additionalCommands = "";
			if (configuration.PauseConsoleOutput)
				additionalCommands = @"echo; read -p 'press any key to continue...' -n1;";

			psi = new ProcessStartInfo("xterm",
				string.Format (
				@"-e ""{0} '{1}' {2} ; {3}""",
				runtimeStarter, configuration.CompiledOutputName, args, additionalCommands));
			psi.UseShellExecute = false;
			
			try {
				psi.WorkingDirectory = Path.GetDirectoryName (configuration.CompiledOutputName);
				psi.UseShellExecute  =  false;
				
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can not execute " + "\"" + configuration.CompiledOutputName + "\"\n(Try restarting MonoDevelop or start your app manually)");
			}
		}

		public override void GenerateMakefiles (Combine parentCombine)
		{
			Console.WriteLine ("Generating makefiles for " + Name);
			languageBinding.GenerateMakefile (this, parentCombine);
		}
		
		public override bool IsCompileable(string fileName)
		{
			return languageBinding.CanCompile(LanguageName);
		}
	}
}
