// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.CodeDom.Compiler;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace VBBinding {
	
	/// <summary>
	/// This class controls the compilation of VB.net files and VB.net projects
	/// </summary>
	public class VBBindingCompilerServices
	{	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		PropertyService propertyService       = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			VBProject p = (VBProject)project;
			VBCompilerParameters compilerparameters = (VBCompilerParameters)p.ActiveConfiguration;
			return fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName) == ".vb";
		}
		
		public ICompilerResult CompileFile(string filename)
		{
			string output = "";
			string error  = "";
			string exe = Path.ChangeExtension(filename, ".exe");
			VBCompilerParameters compilerparameters = new VBCompilerParameters();
			string stdResponseFileName = propertyService.DataDirectory + Path.DirectorySeparatorChar + "vb.rsp";
			
			string responseFileName = Path.GetTempFileName();
			
			StreamWriter writer = new StreamWriter(responseFileName);

			writer.WriteLine("\"/out:" + exe + '"');
			
			writer.WriteLine("/nologo");
			writer.WriteLine("/utf8output");
			
			if (compilerparameters.Debugmode) {
				writer.WriteLine("/debug+");
				writer.WriteLine("/debug:full");
			}
			
			if (compilerparameters.Optimize) {
				writer.WriteLine("/optimize");
			}
			
			if (compilerparameters.DefineSymbols.Length > 0) {
				writer.WriteLine("/define:" + '"' + compilerparameters.DefineSymbols + '"');
			}
			
			if(compilerparameters.Imports.Length > 0) {
				writer.WriteLine("/imports:" + compilerparameters.Imports);
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
			
			writer.WriteLine('"' + filename + '"');
			
			TempFileCollection  tf = new TempFileCollection ();
			writer.Close();
			
			// add " to the responseFileName when they aren't there
			if (!responseFileName.StartsWith("\"") && !responseFileName.EndsWith("\"")) {
				responseFileName = String.Concat("\"", responseFileName, "\"");
			}
			
			string outstr = GetCompilerName() + " \"@" +responseFileName + "\" \"@" + stdResponseFileName + "\"";
			
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			
			ICompilerResult result = ParseOutput(tf, output);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			VBProject p = (VBProject)project;
			VBCompilerParameters compilerparameters = (VBCompilerParameters)p.ActiveConfiguration;
			string exe       = fileUtilityService.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + (compilerparameters.CompileTarget == CompileTarget.Library ? ".dll" : ".exe");
			string responseFileName = Path.GetTempFileName();
			string stdResponseFileName = propertyService.DataDirectory + Path.DirectorySeparatorChar + "vb.rsp";
			StreamWriter writer = new StreamWriter(responseFileName);

			writer.WriteLine("\"/out:" + exe + '"');
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ArrayList allProjects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
			
			foreach (ProjectReference lib in p.ProjectReferences) {
				string fileName = lib.GetReferencedFileName(p);
				writer.WriteLine("\"/r:" + fileName + "\"");
			}
			
			writer.WriteLine("/nologo");
			writer.WriteLine("/utf8output");
			
			if (compilerparameters.Debugmode) {
				writer.WriteLine("/debug+");
				writer.WriteLine("/debug:full");
			}
			
			if (compilerparameters.Optimize) {
				writer.WriteLine("/optimize");
			}
			
			if (compilerparameters.Win32Icon != null && compilerparameters.Win32Icon.Length > 0 && File.Exists(compilerparameters.Win32Icon)) {
				writer.WriteLine("/win32icon:" + '"' + compilerparameters.Win32Icon + '"');
			}
			
			if (compilerparameters.RootNamespace!= null && compilerparameters.RootNamespace.Length > 0) {
				writer.WriteLine("/rootnamespace:" + '"' + compilerparameters.RootNamespace + '"');
			}
			
			if (compilerparameters.DefineSymbols.Length > 0) {
				writer.WriteLine("/define:" + '"' + compilerparameters.DefineSymbols + '"');
			}
			
			if (compilerparameters.MainClass != null && compilerparameters.MainClass.Length > 0) {
				writer.WriteLine("/main:" + compilerparameters.MainClass);
			}
			
			if(compilerparameters.Imports.Length > 0) {
				writer.WriteLine("/imports:" + compilerparameters.Imports);
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
			
			foreach (ProjectFile finfo in p.ProjectFiles) {
				if (finfo.Subtype != Subtype.Directory) {
					switch (finfo.BuildAction) {
						case BuildAction.Compile:
							writer.WriteLine('"' + finfo.Name + '"');
						break;
						
						case BuildAction.EmbedAsResource:
							writer.WriteLine("\"/res:" + finfo.Name + "\"");
						break;
					}
				}
			}
			
			TempFileCollection tf = new TempFileCollection ();
			writer.Close();
			
			string output = "";
			string error  = "";
			string outstr = GetCompilerName() + " \"@" + responseFileName + "\" \"@" + stdResponseFileName + "\"";
			
			Executor.ExecWaitWithCapture(outstr, tf, ref output, ref error);
			VBDOCServices.RunVBDOC(project, exe);
			ICompilerResult result = ParseOutput(tf, output);
			project.CopyReferencesToOutputPath(false);
			
			File.Delete(responseFileName);
			File.Delete(output);
			File.Delete(error);
			return result;
		}
		
		string GetCompilerName()
		{
			return fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()) + 
			       "vbc.exe";
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			string compilerOutput = "";
			
			StreamReader sr = File.OpenText(file);
			
			// skip fist whitespace line
			sr.ReadLine();
			
			CompilerResults cr = new CompilerResults(tf);
			
			while (true) {
				string next = sr.ReadLine();
				compilerOutput += next + "\n";
				if (next == null) {
					break;
				}
				CompilerError error = new CompilerError();
				
				int index           = next.IndexOf(": ");
				if (index < 0) {
					continue;
				}
				
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
						error.Line = Int32.Parse(location.Substring(idx1 + 1, idx2 - idx1 - 1));
						error.FileName = Path.GetFullPath(filename.Trim()); // + "\\" + Path.GetFileName(filename);
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
