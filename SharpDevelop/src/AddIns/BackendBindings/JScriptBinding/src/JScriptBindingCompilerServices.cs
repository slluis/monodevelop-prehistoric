// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Services;


using ICSharpCode.SharpDevelop.Internal.Project;

namespace JScriptBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class JScriptBindingCompilerServices
	{
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			JScriptProject p = (JScriptProject)project;
			JScriptCompilerParameters compilerparameters = (JScriptCompilerParameters)p.ActiveConfiguration;
			string exe  = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			return exe;
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName) == ".js";
		}
		
		public ICompilerResult CompileFile(string filename)
		{	
			string output = "";
			string error  = "";
			string exe = filename.Substring(0, filename.LastIndexOf('.')) + ".exe";
			string options = "";
			
			JScriptCompilerParameters cparam = new JScriptCompilerParameters();
			
			options += " /w:" + cparam.WarningLevel;
			
			if (cparam.Debugmode) 
				options += " /debug ";
			
			options += " /utf8output";
			
			options += " /optimize" + (cparam.Optimize ? "+" : "-");
			
			if (cparam.DefineSymbols.Length > 0) {
				options += " /d:" + cparam.DefineSymbols;
			}
			
			options += " /checked" + (cparam.GenerateOverflowChecks ? "+" : "-");
			
			if (cparam.UnsafeCode)
				options += " /unsafe";
			
			TempFileCollection  tf = new TempFileCollection();
			Executor.ExecWaitWithCapture("jsc /t:exe "+ options + " \"/out:" + exe + "\" \"" + filename + "\"", tf, ref output, ref error);
			
			ICompilerResult cr = ParseOutput(tf, output);
			
			File.Delete(output);
			File.Delete(error);
			
			return cr;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			JScriptProject p = (JScriptProject)project;
			JScriptCompilerParameters compilerparameters = (JScriptCompilerParameters)p.ActiveConfiguration;
			string resources = "";
			string exe       = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			string libs      = "";
			string options   = "";
			string files     = "";
			
			
			foreach (ProjectReference lib in p.ProjectReferences) {
				libs += " \"/r:" + lib.GetReferencedFileName(p) + "\"";
			}
			
			options += " /w:" + compilerparameters.WarningLevel;
			
			if (compilerparameters.Debugmode) 
				options += " /d:DEBUG";
			
			//if (compilerparameters.Optimize)
			//	options += " /o";
			
			//if (compilerparameters.UnsafeCode)
			//	options += " /unsafe";
			
			switch (compilerparameters.CompileTarget) {
				case CompileTarget.Exe:
					options += " /t:exe";
				break;
				//case CompileTarget.WinExe:
				//	options += " /t:winexe";
				//break;
				case CompileTarget.Library:
					options += " /t:library";
				break;
			}
			
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							if (Path.GetExtension(finfo.Name).ToUpper() == ".JS")
								files += " \"" + finfo.Name + '"';
						break;
						
						case BuildAction.EmbedAsResource:
							resources += " \"/res:" + finfo.Name + "\""; 
						break;
					}
				}	
			}
			
			TempFileCollection  tf = new TempFileCollection ();
			
			if (compilerparameters.GenerateXmlDocumentation) {
				options += " \"/doc:" + Path.ChangeExtension(exe, ".xml") + '"';
			}	
			
			string output = "";
			string error  = "";
			string outstr = "jsc " + options + " \"/out:" + exe+ '"' + " " + files + libs + resources;
			
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			
			ICompilerResult cr = ParseOutput(tf, output);
			
			File.Delete(output);
			File.Delete(error);
			return cr;
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			string compilerOutput = "";
			
			StreamReader sr = File.OpenText(file);
			for (int i = 0;i < 6; ++i) {
				if (i > 0)
					compilerOutput += sr.ReadLine() + "\n";
				else
					sr.ReadLine();
			}
			
			CompilerResults cr = new CompilerResults(tf);
			
			
			while (true) {
				string next = sr.ReadLine();
				compilerOutput += next + "\n";
				if (next == null)
					break;
				CompilerError error = new CompilerError();
				
				int index           = next.IndexOf(": ");
				if (index < 0)
					continue;
				
				string description  = null;
				string errorwarning = null;
				string location     = null;
				
				string s1 = next.Substring(0, index);
				string s2 = next.Substring(index + 2);
				index  = s2.IndexOf(": ");
				
				if (index == -1) {
					errorwarning = s1;
					description = s2;
				} else {
					location = s1;
					s1 = s2.Substring(0, index);
					s2 = s2.Substring(index + 2);
					errorwarning = s1;
					description = s2;
				}
				
				if (location != null) {
					int idx1 = location.LastIndexOf('(');
					int idx2 = location.LastIndexOf(')');
					if (idx1 >= 0 &&  idx2 >= 0) {
						string filename = location.Substring(0, idx1);
						string pos      = location.Substring(idx1 + 1, idx2 - idx1 - 1);
						string[] pos2   = pos.Split(new char[] {','});
						error.Column = Int32.Parse(pos2[1]);
						error.Line   = Int32.Parse(pos2[0]);
					
						error.FileName = Path.GetFullPath(filename); // + "\\" + Path.GetFileName(filename);
					}
				}
				
				string[] what = errorwarning.Split(new char[] { ' ' } );
				error.IsWarning   = what[0] == "warning";
				error.ErrorNumber = what[what.Length - 1];
				
				error.ErrorText = description;
				
				cr.Errors.Add(error);
			}
			sr.Close();
			return new DefaultCompilerResult(cr, compilerOutput);
		}
	}
}
