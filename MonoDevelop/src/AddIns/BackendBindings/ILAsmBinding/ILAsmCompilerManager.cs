// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using Gtk;

using MonoDevelop.Gui.Components;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;

namespace ILAsmBinding
{
	/// <summary>
	/// Description of ILAsmCompilerManager.	
	/// </summary>
	public class ILAsmCompilerManager
	{
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			ILAsmProject p = (ILAsmProject)project;
			ILAsmCompilerParameters compilerparameters = (ILAsmCompilerParameters)p.ActiveConfiguration;
			string exe  = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".exe";
			return exe;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".IL";
		}
		
		ICompilerResult Compile(ILAsmCompilerParameters compilerparameters, string[] fileNames)
		{
			// TODO: No response file possible ? @FILENAME seems not to work.
			StringBuilder parameters = new StringBuilder();
			foreach (string fileName in fileNames) {
				parameters.Append('"');
				parameters.Append(Path.GetFullPath(fileName));
				parameters.Append("\" ");
			}
			
			string outputFile = Path.GetFullPath(fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".exe");
			Console.WriteLine (outputFile);
			parameters.Append("/out:" + outputFile);
			parameters.Append(" ");
			parameters.Append(compilerparameters.CurrentCompilerOptions.GenerateOptions());
			string outstr = parameters.ToString();
			
			TempFileCollection tf = new TempFileCollection();
			StreamReader output;
			StreamReader error;
			DoCompilation (outstr, tf, out output, out error);
			ICompilerResult result = ParseOutput(tf, output);
			
			return result;
		}

		private void DoCompilation (string outstr, TempFileCollection tf, out StreamReader output, out StreamReader error)
		{
            		ProcessStartInfo si = new ProcessStartInfo (GetCompilerName (), outstr);
			si.RedirectStandardOutput = true;
            		si.RedirectStandardError = true;
			si.UseShellExecute = false;
			Process p = new Process ();
            		p.StartInfo = si;
            		p.Start ();

			IStatusBarService sbs = (IStatusBarService)ServiceManager.Services.GetService (typeof (IStatusBarService));
			sbs.SetMessage ("Compiling...");

			while (!p.HasExited) {
				((SdStatusBar)sbs.ProgressMonitor).Pulse();
				while (Gtk.Application.EventsPending ())
					Gtk.Application.RunIteration ();
				System.Threading.Thread.Sleep (100);
			}

			((SdStatusBar) sbs.ProgressMonitor).Done ();

			// FIXME: avoid having a full buffer
			// perhaps read one line and append parsed output
			// and then return cr at end 
			output = p.StandardOutput;
			error = p.StandardError;
            		p.WaitForExit ();
        }
		
		public ICompilerResult CompileFile(string fileName, ILAsmCompilerParameters compilerparameters)
		{
			compilerparameters.OutputDirectory = Path.GetDirectoryName(fileName);
			compilerparameters.OutputAssembly  = Path.GetFileNameWithoutExtension(fileName);
			
			return Compile(compilerparameters, new string[] { fileName });
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			ILAsmProject            p                  = (ILAsmProject)project;
			ILAsmCompilerParameters compilerparameters = (ILAsmCompilerParameters)p.ActiveConfiguration;
			
			ArrayList fileNames = new ArrayList();
			
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							fileNames.Add(finfo.Name);
							break;
//				TODO : Embedded resources ?		
//						case BuildAction.EmbedAsResource:
//							writer.WriteLine("\"/res:" + finfo.Name + "\"");
//							break;
					}
				}
			}
			
			return Compile(compilerparameters, (string[])fileNames.ToArray(typeof(string)));
		}
		
		string GetCompilerName()
		{
			return "ilasm";
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, StreamReader sr)
		{
			StringBuilder compilerOutput = new StringBuilder();
			
			//StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
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
				// TODO : PARSE ERROR OUTPUT.
				
//				curLine = curLine.Trim();
//				if (curLine.Length == 0) {
//					continue;
//				}
//				
//				CompilerError error = new CompilerError();
//				
//				// try to match standard errors
//				Match match = normalError.Match(curLine);
//				if (match.Success) {
//					error.Column      = Int32.Parse(match.Result("${column}"));
//					error.Line        = Int32.Parse(match.Result("${line}"));
//					error.FileName    = Path.GetFullPath(match.Result("${file}"));
//					error.IsWarning   = match.Result("${error}") == "warning"; 
//					error.ErrorNumber = match.Result("${number}");
//					error.ErrorText   = match.Result("${message}");
//				} else {
//					match = generalError.Match(curLine); // try to match general csc errors
//					if (match.Success) {
//						error.IsWarning   = match.Result("${error}") == "warning"; 
//						error.ErrorNumber = match.Result("${number}");
//						error.ErrorText   = match.Result("${message}");
//					} else { // give up and skip the line
//						continue;
////						error.IsWarning = false;
////						error.ErrorText = curLine;
//					}
//				}
//				
//				cr.Errors.Add(error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput.ToString());
		}
	}
}
