// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;
//using CSharpBinding;

namespace VBBinding
{
	public class VBLanguageBinding : ILanguageBinding
	//public class VBLanguageBinding : CSharpLanguageBinding
	{
		public const string LanguageName = "VBNet";
		
		VBBindingCompilerServices   compilerServices  = new VBBindingCompilerServices();
		VBBindingExecutionServices  executionServices = new VBBindingExecutionServices();
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public void Execute(string filename, bool debug)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(filename);
		}
		
		public void Execute(IProject project, bool debug)
		{
			Debug.Assert(executionServices != null);
			if(debug){
				executionServices.Debug(project);
			}else{
				executionServices.Execute(project);
			}//if
			
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
			return new VBProject(info, projectOptions);
		}
		
		public virtual void Execute(string filename)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(filename);
		}
		
		public virtual void Execute(IProject project)
		{
			Debug.Assert(executionServices != null);
			executionServices.Execute(project);
		}

		
		public void DebugProject (IProject project)
		{
			executionServices.Debug (project);
		}
		
		public void GenerateMakefile (IProject project, Combine parentCombine)
		{
			compilerServices.GenerateMakefile (project, parentCombine);
		}

		public string CommentTag
		{
			get { return "'"; }
		}
	}
}
