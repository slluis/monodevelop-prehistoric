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

using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;

namespace JavaBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class JavaBindingCompilerServices
	{	
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName) == ".java";
		}
		
		public ICompilerResult CompileFile(string filename)
		{
			string output = "";
			string error  = "";
			string options = "";
			
			JavaCompilerParameters cparam = new JavaCompilerParameters();
			
			if (cparam.Debugmode) {
				options += " -g ";
			} else {
				options += " -g:none ";
			}
			
			if (cparam.Optimize) {
				options += " -O ";
			}
			
			if (cparam.GenWarnings) {
				options += " -nowarn ";
			}
			options += " -encoding utf8 ";
			
			TempFileCollection  tf = new TempFileCollection();					
			
			string outstr = "javac \"" + filename + "\" -classpath " + cparam.ClassPath + options;
			Executor.ExecWaitWithCapture(outstr, tf, ref error , ref output);
			
			ICompilerResult cr = ParseOutput(tf, output);
			
			File.Delete(output);
			File.Delete(error);
			
			return cr;	
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".class");
		}

		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

		public string GetCompiledOutputName(IProject project)
		{
			JavaProject p = (JavaProject)project;
			JavaCompilerParameters compilerparameters = (JavaCompilerParameters)p.ActiveConfiguration;
			
			string exe         = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".class";
			return exe;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			JavaProject p = (JavaProject)project;
			JavaCompilerParameters compilerparameters = (JavaCompilerParameters)p.ActiveConfiguration;
			
			string exe         = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".class";
			string options   = "";
			
			if (compilerparameters.Debugmode) 
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
			
			string output = "";
			string error  = "";
			string files  = "";
			
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							files = files + " \"" + finfo.Name + "\"";
						break;
					}
				}
			}

			TempFileCollection  tf = new TempFileCollection ();			
			
			//string CurrentDir = Directory.GetCurrentDirectory();
			//Directory.SetCurrentDirectory(compilerparameters.OutputDirectory);
			
			//string outstr = compilerparameters.CompilerPath + "" + files + " -classpath " + compilerparameters.ClassPath + options;
			//string outstr = compilerparameters.CompilerPath + "" + files + " -classpath " + compilerparameters.ClassPath + options;

			string outstr = compilerparameters.CompilerPath + " " + files + options;			
			DoCompilation (outstr, tf, ref output, ref error);
			//Executor.ExecWaitWithCapture(outstr, tf, ref error , ref output);			
			ICompilerResult cr = ParseOutput (tf, output);			
			File.Delete(output);
			File.Delete(error);
			
			//Directory.SetCurrentDirectory(CurrentDir);			
			
//			TempFileCollection  tf = new TempFileCollection ();
//			string output = "";
//			string error  = "";
//			//string outstr = compilerparameters.CompilerPath + " " + compilerparameters.OutputDirectory+"untitled.java -classpath " + compilerparameters.ClassPath + options;
//			string outstr = compilerparameters.CompilerPath + " " + compilerparameters.OutputDirectory + finfo.Name;					
//			Executor.ExecWaitWithCapture(outstr, tf, ref error , ref output);												
//			CompilerResults cr = ParseOutput(tf, output);
//			File.Delete(output);
//			File.Delete(error);
			
			return cr;
		}

		private void DoCompilation (string outstr, TempFileCollection tf, ref string output, ref string error)
		{
			output = Path.GetTempFileName ();
            error = Path.GetTempFileName ();

            string arguments = outstr + " > " + output + " 2> " + error;
            string command = arguments;
            ProcessStartInfo si = new ProcessStartInfo("/bin/sh -c \"" + command + "\"");
			si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
			si.UseShellExecute = false;
			Process p = new Process ();
            p.StartInfo = si;
            p.Start ();
            p.WaitForExit ();
        }
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			string compilerOutput = "";		
			StreamReader sr = new StreamReader(file, System.Text.Encoding.Default);
			CompilerResults cr = new CompilerResults(tf);
			
			while (true) 
				{
				string next = sr.ReadLine();									
				
				if (next == null)
					break;
				
				compilerOutput += next + "\n";
					
				CompilerError error = new CompilerError();
				
				int index1 = next.IndexOf(".java:");
				if (index1 < 0)
					continue;				
				
				string s1 = next.Substring(0, index1);
				string s2 = next.Substring(index1 + 6);									
				int index2  = s2.IndexOf(":");				
				int line = Int32.Parse(next.Substring(index1 + 6,index2));
					
//				error.Column = Int32.Parse(pos2[1]);
//				error.IsWarning   = what[0] == "warning";
//				error.ErrorNumber = what[what.Length - 1];				
								
				error.Column = 25;
				error.Line   	= line;
				error.ErrorText = next.Substring(index1 + index2 + 7);
				error.FileName  = Path.GetFullPath(next.Substring(0, index1) + ".java"); //Path.GetFileName(filename);
				cr.Errors.Add(error);
				
			}
			sr.Close();			
			return new DefaultCompilerResult(cr, compilerOutput);

		}
	}
}
