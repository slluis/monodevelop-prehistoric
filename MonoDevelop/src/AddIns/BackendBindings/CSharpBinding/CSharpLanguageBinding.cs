// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Gui;

namespace CSharpBinding
{
	public class CSharpLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C#";
		
		CSharpBindingCompilerManager   compilerManager  = new CSharpBindingCompilerManager();
		CSharpBindingExecutionManager  executionManager = new CSharpBindingExecutionManager();
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public void Execute(string filename)
		{
			Debug.Assert(executionManager != null);
			executionManager.Execute(filename);
		}
		
		public void Execute(IProject project)
		{
			Debug.Assert(executionManager != null);
			executionManager.Execute(project);
		}

		public void DebugProject (IProject project)
		{
			executionManager.Debug (project);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.GetCompiledOutputName(fileName);
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.GetCompiledOutputName(project);
		}
		
		public bool CanCompile(string fileName)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.CanCompile(fileName);
		}
		
		public ICompilerResult CompileFile(string fileName)
		{
			Debug.Assert(compilerManager != null);
			CSharpCompilerParameters param = new CSharpCompilerParameters();
			param.OutputAssembly = Path.ChangeExtension(fileName, ".exe");
			return compilerManager.CompileFile(fileName, param);
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			Debug.Assert(compilerManager != null);
			return compilerManager.CompileProject(project);
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			return CompileProject(project);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new CSharpProject(info, projectOptions);
		}

		public void GenerateMakefile (IProject project)
		{
			compilerManager.GenerateMakefile (project);
		}
	}
}
