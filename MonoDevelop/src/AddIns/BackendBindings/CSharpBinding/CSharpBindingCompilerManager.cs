// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;

using MonoDevelop.Core.Services;

using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Services;

namespace CSharpBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class CSharpBindingCompilerManager
	{	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			CSharpProject p = (CSharpProject)project;
			CSharpCompilerParameters compilerparameters = (CSharpCompilerParameters)p.ActiveConfiguration;
			string exe  = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			return exe;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".CS";
		}
		
		public ICompilerResult CompileFile(string filename, CSharpCompilerParameters compilerparameters)
		{
			string output = "";
			string error  = "";
			string exe = Path.ChangeExtension(filename, ".exe");
			if (compilerparameters.OutputAssembly != null && compilerparameters.OutputAssembly.Length > 0) {
				exe = compilerparameters.OutputAssembly;
			}
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);

			writer.WriteLine("\"/out:" + exe + '"');
			
			writer.WriteLine("/nologo");
			writer.WriteLine("/utf8output");
			writer.WriteLine("/w:" + compilerparameters.WarningLevel);
			
			if (compilerparameters.Debugmode) {
				writer.WriteLine("/debug:+");
				writer.WriteLine("/debug:full");
				writer.WriteLine("/d:DEBUG");
			}
			
			if (!compilerparameters.RunWithWarnings) {
				writer.WriteLine("/warnaserror+");
			}
			
			if (compilerparameters.Optimize) {
				writer.WriteLine("/o");
			}
			
			if (compilerparameters.UnsafeCode) {
				writer.WriteLine("/unsafe");
			}
			
			if (compilerparameters.DefineSymbols.Length > 0) {
				writer.WriteLine("/define:" + '"' + compilerparameters.DefineSymbols + '"');
			}
			
			switch (compilerparameters.CompileTarget) {
				case CompileTarget.Exe:
					writer.WriteLine("/target:exe");
					break;
				case CompileTarget.WinExe:
					writer.WriteLine("/target:winexe");
					break;
				case CompileTarget.Library:
					writer.WriteLine("/target:library");
					break;
				case CompileTarget.Module:
					writer.WriteLine("/target:module");
					break;
			}
			
			writer.WriteLine('"' + filename + '"');
			
			TempFileCollection  tf = new TempFileCollection ();
			
			if (compilerparameters.GenerateXmlDocumentation) {
				writer.WriteLine("\"/doc:" + Path.ChangeExtension(exe, ".xml") + '"');
			}	
			
			writer.Close();
			
			// add " to the responseFileName when they aren't there
			if (!responseFileName.StartsWith("\"") && !responseFileName.EndsWith("\"")) {
				responseFileName = String.Concat("\"", responseFileName, "\"");
			}
			
			string compilerName = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? GetCompilerName() : "mcs";
			string outstr =  compilerName + " @" +responseFileName;
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			CSharpProject p = (CSharpProject)project;
			CSharpCompilerParameters compilerparameters = (CSharpCompilerParameters)p.ActiveConfiguration;
			
			string exe              = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			string responseFileName = Path.GetTempFileName();
			StreamWriter writer = new StreamWriter(responseFileName);
			
			string optionString = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? "/" : "-";
			
			if (compilerparameters.CsharpCompiler == CsharpCompiler.Csc) {
				writer.WriteLine("\"/out:" + exe + '"');
				
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				
				foreach (ProjectReference lib in p.ProjectReferences) {
					string fileName = lib.GetReferencedFileName(p);
					writer.WriteLine("\"/r:" + fileName + "\"");
				}
				
				writer.WriteLine("/nologo");
				writer.WriteLine("/codepage:utf8");
//				writer.WriteLine("/utf8output");
//				writer.WriteLine("/w:" + compilerparameters.WarningLevel);;
				
				if (compilerparameters.Debugmode) {
					writer.WriteLine("/debug:+");
					writer.WriteLine("/debug:full");
					writer.WriteLine("/d:DEBUG");
				}
				
//				if (compilerparameters.Optimize) {
//					writer.WriteLine("/o");
//				}
				
				if (compilerparameters.Win32Icon != null && compilerparameters.Win32Icon.Length > 0 && File.Exists(compilerparameters.Win32Icon)) {
					writer.WriteLine("\"/win32icon:" + compilerparameters.Win32Icon + "\"");
				}
				
				if (compilerparameters.UnsafeCode) {
					writer.WriteLine("/unsafe");
				}
				
				if (compilerparameters.DefineSymbols.Length > 0) {
					writer.WriteLine("/define:" + '"' + compilerparameters.DefineSymbols + '"');
				}
				
				if (compilerparameters.MainClass != null && compilerparameters.MainClass.Length > 0) {
					writer.WriteLine("/main:" + compilerparameters.MainClass);
				}
				
				switch (compilerparameters.CompileTarget) {
					case CompileTarget.Exe:
						writer.WriteLine("/t:exe");
						break;
					case CompileTarget.WinExe:
						writer.WriteLine("/t:winexe");
						break;
					case CompileTarget.Library:
						writer.WriteLine("/t:library");
						break;
				}
				
				foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.Subtype != Subtype.Directory) {
						switch (finfo.BuildAction) {
							case BuildAction.Compile:
								Console.Error.WriteLine(finfo.Name);
								writer.WriteLine('"' + finfo.Name + '"');
								break;
							case BuildAction.EmbedAsResource:
								// Workaround 50752
								writer.WriteLine(@"""/res:{0},{1}""", finfo.Name, Path.GetFileName (finfo.Name));
								break;
						}
					}
				}
				
				if (compilerparameters.GenerateXmlDocumentation) {
					writer.WriteLine("\"/doc:" + Path.ChangeExtension(exe, ".xml") + '"');
				}
			} 
			else {
				writer.WriteLine("-o " + exe);
				
				if (compilerparameters.UnsafeCode) {
					writer.WriteLine("--unsafe");
				}
				
				writer.WriteLine("--wlevel " + compilerparameters.WarningLevel);
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				
				foreach (ProjectReference lib in p.ProjectReferences) {
					string fileName = lib.GetReferencedFileName(p);
					writer.WriteLine("-r:" + fileName );
				}
				
				switch (compilerparameters.CompileTarget) {
					case CompileTarget.Exe:
						writer.WriteLine("--target exe");
						break;
					case CompileTarget.WinExe:
						writer.WriteLine("--target winexe");
						break;
					case CompileTarget.Library:
						writer.WriteLine("--target library");
						break;
				}
				foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.Subtype != Subtype.Directory) {
						switch (finfo.BuildAction) {
							case BuildAction.Compile:
								writer.WriteLine('"' + finfo.Name + '"');
								break;
							
							case BuildAction.EmbedAsResource:
								writer.WriteLine("--linkres " + finfo.Name);
								break;
						}
					}
				}			
			}
			writer.Close();
			
			string output = String.Empty;
			string error  = String.Empty; 
			
			string compilerName = compilerparameters.CsharpCompiler == CsharpCompiler.Csc ? GetCompilerName() : System.Environment.GetEnvironmentVariable("ComSpec") + " /c mcs";
			string outstr = compilerName + " @" + responseFileName;
			TempFileCollection tf = new TempFileCollection();
			
			
			StreamReader t = File.OpenText(responseFileName);
			
			//Executor.ExecWaitWithCapture(outstr,  tf, ref output, ref error);
			DoCompilation(outstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			project.CopyReferencesToOutputPath(false);
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		string GetCompilerName()
		{
			//return fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()) + 
			//       "csc.exe";
			string ret = fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
			ret = ret.Substring(0, ret.Length - 4);
			ret = ret + "bin/mcs";
			return ret;
			
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			StringBuilder compilerOutput = new StringBuilder();
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			//sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
			// we have 2 formats for the error output the csc gives :
			Regex normalError  = new Regex(@"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
			Regex generalError = new Regex(@"(?<error>.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)", RegexOptions.Compiled);
			
			while (true) {
				string curLine = sr.ReadLine();
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
				curLine = curLine.Trim();
				if (curLine.Length == 0) {
					continue;
				}
				
				CompilerError error = CreateErrorFromString (curLine);
				
				if (error != null)
					cr.Errors.Add (error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
		}
		
		private void DoCompilation(string outstr, TempFileCollection tf, ref string output, ref string error) {
			output = Path.GetTempFileName();
			error = Path.GetTempFileName();
			
			string arguments = outstr + " > " + output + " 2> " + error;
			string command = arguments;
			ProcessStartInfo si = new ProcessStartInfo("/bin/sh -c \"" + command + "\"");
			si.RedirectStandardOutput = true;
			si.RedirectStandardError = true;
			si.UseShellExecute = false;
			Process p = new Process();
			p.StartInfo = si;
			p.Start();
			//FIXME: The glib.idle stuff is here because this *SHOULD* be
			//a background thread calling back to the main thread.
			//GLib.Idle.Add (new GLib.IdleHandler (setmsg));
			setmsg ();
			while (!p.HasExited) {
				//GLib.Idle.Add (new GLib.IdleHandler (pulse));
				pulse ();
				System.Threading.Thread.Sleep (100);
			}
			//GLib.Idle.Add (new GLib.IdleHandler (done));
			done ();
		}

		bool setmsg ()
		{
			((IStatusBarService)ServiceManager.Services.GetService (typeof (IStatusBarService))).SetMessage ("Compiling...");
			return false;
		}

		bool done ()
		{
			((SdStatusBar)((IStatusBarService)ServiceManager.Services.GetService (typeof (IStatusBarService))).ProgressMonitor).Done ();
			return false;
		}

		bool pulse () 
		{
			((SdStatusBar)((IStatusBarService)ServiceManager.Services.GetService (typeof (IStatusBarService))).ProgressMonitor).Pulse ();
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();
			return false;
		}
		
		// Snatched from our codedom code :-).
		static Regex regexError = new Regex (@"^(\s*(?<file>.*)\((?<line>\d*)(,(?<column>\d*))?\)\s+)*(?<level>\w+)\s*(?<number>.*):\s(?<message>.*)",
			RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static CompilerError CreateErrorFromString(string error_string)
		{
			// When IncludeDebugInformation is true, prevents the debug symbols stats from braeking this.
			if (error_string.StartsWith ("WROTE SYMFILE") ||
			    error_string.StartsWith ("OffsetTable") ||
			    error_string.StartsWith ("Compilation succeeded") ||
			    error_string.StartsWith ("Compilation failed"))
				return null;

			CompilerError error = new CompilerError();

			Match match=regexError.Match(error_string);
			if (!match.Success) return null;
			if (String.Empty != match.Result("${file}"))
				error.FileName=match.Result("${file}");
			if (String.Empty != match.Result("${line}"))
				error.Line=Int32.Parse(match.Result("${line}"));
			if (String.Empty != match.Result("${column}"))
				error.Column=Int32.Parse(match.Result("${column}"));
			if (match.Result("${level}")=="warning")
				error.IsWarning=true;
			error.ErrorNumber=match.Result("${number}");
			error.ErrorText=match.Result("${message}");
			return error;
		}
	}
}
