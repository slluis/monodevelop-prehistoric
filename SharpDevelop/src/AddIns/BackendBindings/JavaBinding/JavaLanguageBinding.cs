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
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace JavaBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language binding
	/// </summary>
	public class JavaLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Java";
		
		JavaBindingCompilerServices   compilerServices  = new JavaBindingCompilerServices();
		JavaBindingExecutionServices  executionServices = new JavaBindingExecutionServices();
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public void Execute(string filename)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(filename);
		}
		
		public void Execute(IProject project)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(project);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.GetCompiledOutputName(fileName);
		}
		
		public string GetCompiledOutputName(IProject project)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.GetCompiledOutputName(project);
		}
		
		public bool CanCompile(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CanCompile(fileName);
		}
		
		public ICompilerResult CompileFile(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CompileFile(fileName);
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CompileProject(project);
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			return CompileProject(project);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new JavaProject(info, projectOptions);
		}
	}
}
