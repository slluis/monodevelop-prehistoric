using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Xml;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Gui;

namespace NemerleBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language binding
	/// </summary>
	public class NemerleLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Nemerle";
		
		NemerleBindingCompilerServices   compilerServices  = new NemerleBindingCompilerServices();
		NemerleBindingExecutionServices  executionServices = new NemerleBindingExecutionServices();
		
		public string Language {
			get { return LanguageName; }
		}
		
		public void Execute (string filename)
		{
			executionServices.Execute(filename);
		}
		
		public void Execute (IProject project)
		{
			executionServices.Execute (project);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			return compilerServices.GetCompiledOutputName(fileName);
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			return compilerServices.GetCompiledOutputName(project);
		}
		
		public bool CanCompile(string fileName)
		{
			return compilerServices.CanCompile(fileName);
		}
		
		public ICompilerResult CompileFile(string fileName)
		{
			return compilerServices.CompileFile(fileName);
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			return compilerServices.CompileProject(project);
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			return CompileProject(project);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new NemerleProject(info, projectOptions);
		}

		public void DebugProject (IProject project)
		{
			throw new ApplicationException("No Nemele debug");
		}

		public void GenerateMakefile (IProject project, Combine parentCombine)
		{
			compilerServices.GenerateMakefile(project, parentCombine);
		}
		
		// http://nemerle.org/csharp-diff.html
		public string CommentTag
		{
			get { return "//"; }
		}
	}
}
