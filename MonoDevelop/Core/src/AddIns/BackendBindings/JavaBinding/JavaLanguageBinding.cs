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

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Gui;
using MonoDevelop.Services;

namespace JavaBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language binding
	/// </summary>
	public class JavaLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Java";
		
		JavaBindingCompilerServices   compilerServices  = new JavaBindingCompilerServices();
		
		public JavaLanguageBinding ()
		{
			Runtime.ProjectService.DataContext.IncludeType (typeof(JavaCompilerParameters));
		}
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public bool CanCompile(string fileName)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.CanCompile(fileName);
		}
		
		public ICompilerResult Compile (ProjectFileCollection projectFiles, ProjectReferenceCollection references, DotNetProjectConfiguration configuration)
		{
			Debug.Assert(compilerServices != null);
			return compilerServices.Compile (projectFiles, references, configuration);
		}
		
		public void GenerateMakefile (Project project, Combine parentCombine)
		{
			throw new NotImplementedException ();
		}
		
		public object CreateCompilationParameters (XmlElement projectOptions)
		{
			JavaCompilerParameters parameters = new JavaCompilerParameters ();
			if (projectOptions != null) {
				if (projectOptions.Attributes["MainClass"] != null) {
					parameters.MainClass = projectOptions.GetAttribute ("MainClass");
				}
				if (projectOptions.Attributes["ClassPath"] != null) {
					parameters.ClassPath = projectOptions.GetAttribute ("ClassPath");
				}
			}
			return parameters;
		}
		
		// http://www.nbirn.net/Resources/Developers/Conventions/Commenting/Java_Comments.htm#CommentBlock
		public string CommentTag
		{
			get { return "//"; }
		}
	}
}
