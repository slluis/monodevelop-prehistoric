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
using System.Text;

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
		
		string GetCompilerName (JavaCompilerParameters cp)
		{
			if (cp.Compiler == JavaCompiler.Gcj)
				return "gcj";

			return "javac";
		}
		
		public ICompilerResult Compile (ProjectFileCollection projectFiles, ProjectReferenceCollection references, DotNetProjectConfiguration configuration, IProgressMonitor monitor)
		{
			JavaCompilerParameters compilerparameters = (JavaCompilerParameters) configuration.CompilationParameters;
			if (compilerparameters == null)
				compilerparameters = new JavaCompilerParameters ();
			
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

			string output = String.Empty;
			string error = String.Empty;
			TempFileCollection  tf = new TempFileCollection ();			
			DoCompilation (monitor, compiler, args, tf, configuration, compilerparameters, ref output, ref error);
			ICompilerResult cr = ParseOutput (tf, output, error);			
			File.Delete (output);
			File.Delete (error);
			return cr;
		}

		private void DoCompilation (IProgressMonitor monitor, string compiler, string args, TempFileCollection tf, DotNetProjectConfiguration configuration, JavaCompilerParameters compilerparameters, ref string output, ref string error)
		{
			output = Path.GetTempFileName ();
			error = Path.GetTempFileName ();

			try {
				monitor.BeginTask (null, 2);
				monitor.Log.WriteLine ("Compiling Java source code ...");
				string arguments = String.Format ("-c \"{0} {1} > {2} 2> {3}\"", compiler, args, output, error);
				ProcessStartInfo si = new ProcessStartInfo ("/bin/sh", arguments);
				//Console.WriteLine ("{0} {1}", si.FileName, si.Arguments);
				si.RedirectStandardOutput = true;
				si.RedirectStandardError = true;
				si.UseShellExecute = false;
				Process p = new Process ();
				p.StartInfo = si;
				p.Start ();
				p.WaitForExit ();
				
				monitor.Step (1);
				monitor.Log.WriteLine ("Generating assembly ...");
				CompileToAssembly (configuration, compilerparameters, output, error);
			} finally {
				monitor.EndTask ();
			}
        }

		void CompileToAssembly (DotNetProjectConfiguration configuration, JavaCompilerParameters compilerparameters, string output, string error)
		{
			string outdir = configuration.OutputDirectory;
			string outclass = Path.Combine (outdir, configuration.OutputAssembly + ".class");
			string asm = Path.GetFileNameWithoutExtension (outclass);
		
			// sadly I dont think we can specify the output .class name
			string args = String.Format ("-c \"ikvmc {0} -assembly:{1} > {2} 2> {3}\"", "*.class", asm, output, error);
            ProcessStartInfo si = new ProcessStartInfo ("/bin/sh", args);
			//Console.WriteLine ("{0} {1}", si.FileName, si.Arguments);
            si.WorkingDirectory = outdir;
			si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
			si.UseShellExecute = false;
			Process p = new Process ();
           	p.StartInfo = si;
            p.Start ();
			p.WaitForExit ();
		}
		
		ICompilerResult ParseOutput (TempFileCollection tf, string stdout, string stderr)
		{
			StringBuilder compilerOutput = new StringBuilder ();
			CompilerResults cr = new CompilerResults (tf);
			
			foreach (string s in new string[] { stdout, stderr })
			{
				StreamReader sr = File.OpenText (s);
				while (true) 
				{
					string next = sr.ReadLine ();
				
					if (next == null)
						break;

					CompilerError error = CreateErrorFromString (next);

					if (error != null)
						cr.Errors.Add (error);
				}
				sr.Close ();
			}
			return new DefaultCompilerResult (cr, compilerOutput.ToString ());
		}

		// FIXME: the various java compilers will probably need to be parse on
		// their own and then ikvmc would need one as well
		private static CompilerError CreateErrorFromString (string error)
		{
			if (error.StartsWith ("Note") || error.StartsWith ("Warning"))
				return null;
			string trimmed = error.Trim ();
			if (trimmed.StartsWith ("(to avoid this warning add"))
				return null;
			//Console.WriteLine ("error: {0}", error);

			CompilerError cerror = new CompilerError ();
			cerror.ErrorText = error;
			return cerror;
		}
/* old javac parser
					CompilerError error = new CompilerError ();

					int errorCol = 0;
					string col = next.Trim ();
					if (col.Length == 1 && col == "^")
						errorCol = next.IndexOf ("^");

					compilerOutput.Append (next);
					compilerOutput.Append ("\n");

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
*/
	}
}
