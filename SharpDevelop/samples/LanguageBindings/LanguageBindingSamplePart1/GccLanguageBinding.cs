// GccLanguageBinding.cs
// Copyright (C) 2001 Mike Krueger
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.CombineProjectLayer.Project;
using SharpDevelop.Gui;

namespace GccBinding {
	
	/// <summary>
	/// This class is the class which is used by SharpDevelop, it provides :
	///    A way to compile and execute files
	///    A name for the project type, which is used to identify projects
	///    A factory method that create projects out of a ProjectCreateInformation which
	///    contains project name, directory and other stuff
	/// </summary>
	public class GccLanguageBinding : ILanguageBinding
	{
		// I need this string in the projecttype too, therefore I use public const ...
		public const string LanguageName = "GCC/C++";
		
		// to make this class a bit shorter I've split the functionalaty in two helper classes:
		GccBindingCompilerServices   compilerServices  = new GccBindingCompilerServices();
		GccBindingExecutionServices  executionServices = new GccBindingExecutionServices();
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		// Execution functions
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
		
		// Compiler functions
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
		
		// Project create functions
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new GccProject(info);
		}
	}
}
