// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;

using MonoDevelop.Gui.Components;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;

namespace JavaBinding
{
	public class JavaBindingCompilerServices
	{
		public bool CanCompile (string fileName)
		{
			return Path.GetExtension(fileName) == ".java";
		}
		
		FileUtilityService fileUtilityService = (FileUtilityService) ServiceManager.GetService(typeof(FileUtilityService));

		string GetCompilerName (JavaCompilerParameters cp)
		{
			if (cp.Compiler == JavaCompiler.Gcj)
			{
				return "gcj";
			}

			return "javac";
		}
		
		public ICompilerResult Compile (ProjectFileCollection projectFiles, ProjectReferenceCollection references, DotNetProjectConfiguration configuration, IProgressMonitor monitor)
		{
			JavaCompilerParameters compilerparameters = (JavaCompilerParameters) configuration.CompilationParameters;
			if (compilerparameters == null) compilerparameters = new JavaCompilerParameters ();
			
			string outdir = configuration.OutputDirectory;
			string options = "";

			string compiler = GetCompilerName (compilerparameters);
			
			if (configuration.DebugMode) 
				options += " -g ";
			else
				options += " -g:none ";
			
			if (compilerparameters.Optimize)
				options += " -O ";
			
			if (compilerparameters.Deprecation)
				options += " -deprecation ";
			
			if (compilerparameters.GenWarnings)
				options += " -nowarn ";
			
			if (compilerparameters.ClassPath == null)
				options += " -classpath " + compilerparameters.ClassPath;
			
			options += " -encoding utf8 ";
			
			string files  = "";
			
			foreach (ProjectFile finfo in projectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							files = files + " \"" + finfo.Name + "\"";
						break;
					}
				}
			}

			TempFileCollection  tf = new TempFileCollection ();			
			string args = "";
			
			if (compilerparameters.Compiler == JavaCompiler.Gcj)
				args = "-C ";
			
			//FIXME re-enable options
			//FIXME re-enable compilerPath
			if (compilerparameters.ClassPath == "") {
				args += files + " -d " + outdir;			
			} else {
				args += " -classpath " + compilerparameters.ClassPath + files + " -d " + outdir;
			}
			//Console.WriteLine (args);

			StreamReader output;
			StreamReader error;
			DoCompilation (monitor, compiler, args, tf, configuration, compilerparameters, out output, out error);
			ICompilerResult cr = ParseOutput (tf, error);			
			
			return cr;
		}

		private void DoCompilation (IProgressMonitor monitor, string compiler, string args, TempFileCollection tf, DotNetProjectConfiguration configuration, JavaCompilerParameters compilerparameters, out StreamReader output, out StreamReader error)
		{
			try {
				monitor.BeginTask (null, 2);
				monitor.Log.WriteLine ("Compiling Java source code ...");
				ProcessStartInfo si = new ProcessStartInfo (compiler, args);
				si.RedirectStandardOutput = true;
				si.RedirectStandardError = true;
				si.UseShellExecute = false;
				Process p = new Process ();
				p.StartInfo = si;
				p.Start ();
				p.WaitForExit ();
				
				monitor.Step (1);
				monitor.Log.WriteLine ("Generating assembly ...");
				CompileToAssembly (configuration, compilerparameters);
			
				// FIXME: avoid having a full buffer
				// perhaps read one line and append parsed output
				// and then return cr at end 
				output = p.StandardOutput;
				error = p.StandardError;
			} finally {
				monitor.EndTask ();
			}
        }

		void CompileToAssembly (DotNetProjectConfiguration configuration, JavaCompilerParameters compilerparameters)
		{
			string outdir = configuration.OutputDirectory;
			string outclass = Path.Combine (outdir, configuration.OutputAssembly + ".class");
			string asm = Path.GetFileNameWithoutExtension (outclass);
		
			// sadly I dont think we can specify the output .class name
			string args = String.Format ("{0} -assembly:{1}", "*.class", asm);
            ProcessStartInfo si = new ProcessStartInfo ("ikvmc", args);
            si.WorkingDirectory = outdir;
			si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
			si.UseShellExecute = false;
			Process p = new Process ();
           	p.StartInfo = si;
            p.Start ();
			p.WaitForExit ();
		}
		
		ICompilerResult ParseOutput (TempFileCollection tf, StreamReader errorStream)
		{
			string compilerOutput = "";		
			StreamReader sr = errorStream;
			CompilerResults cr = new CompilerResults (tf);
			
			while (true) 
			{
				string next = sr.ReadLine ();
				
				if (next == null)
					break;

				CompilerError error = new CompilerError ();

				int errorCol = 0;
				string col = next.Trim ();
				if (col.Length == 1 && col == "^")
					errorCol = next.IndexOf ("^");

				compilerOutput += next + "\n";

				int index1 = next.IndexOf (".java:");
				if (index1 < 0)
					continue;				
				
				//string s1 = next.Substring (0, index1);
				string s2 = next.Substring (index1 + 6);									
				int index2  = s2.IndexOf (":");				
				int line = Int32.Parse (next.Substring (index1 + 6, index2));
				//error.IsWarning   = what[0] == "warning";
				//error.ErrorNumber = what[what.Length - 1];
								
				error.Column = errorCol;
				error.Line = line;
				error.ErrorText = next.Substring (index1 + index2 + 7);
				error.FileName = Path.GetFullPath (next.Substring (0, index1) + ".java"); //Path.GetFileName(filename);
				cr.Errors.Add (error);
			}

			sr.Close ();			
			return new DefaultCompilerResult (cr, compilerOutput);
		}
	}
}
