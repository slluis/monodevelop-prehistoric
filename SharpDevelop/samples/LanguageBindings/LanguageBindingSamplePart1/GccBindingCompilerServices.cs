// GccBindingCompilerServices.cs
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
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.CodeDom.Compiler;

using Core.Util;
using Core.Services;

using ICSharpCode.SharpDevelop.CombineProjectLayer.Project;
using SharpDevelop.Gui;
using SharpDevelop.Tool.Data;
using SharpDevelop.WorkspaceServices;

namespace GccBinding {
	
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class GccBindingCompilerServices
	{	
		string[] gccSourceFileExtensions = { ".c", ".C", ".cpp", ".cc" };
		
		// Returns the compiled output name for a single file compile, this
		// may depend on the used operating system
		public string GetCompiledOutputName(string fileName)
		{
			// Caution !!! this only works under Windows !!!
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		// Returns the compiled output name for a project compilation, this may
		// depend on the used operating system.
		public string GetCompiledOutputName(IProject project)
		{
			GccProject p = (GccProject)project;
			GccCompilerParameters compilerparameters = (GccCompilerParameters)p.ActiveConfiguration;
			string exe  = FileUtility.GetDirectoryNameWithSeparator(compilerparameters.OutputDirectory) + compilerparameters.OutputAssembly + ".exe";
			return exe;
		}
		
		// This function should return true, if the file under the location 
		// fileName could be compiled with this language binding. Usually this is determined
		// by the file extension.
		public bool CanCompile(string fileName)
		{
			bool canCompile = false;
			
			foreach (string extension in gccSourceFileExtensions) {
				if (Path.GetExtension(fileName) == extension) {
					canCompile = true;
					break;
				}
			}
			return canCompile;
		}
		
		// TODO : actual compilation of a file or project.
		public ICompilerResult CompileFile(string fileName)
		{
			string compiler = "g++"; // default : C++
			if (Path.GetExtension(fileName).Length == 2) { // assume C for extensions .c + .C
				compiler = "gcc";
			}
			
			ProcessStartInfo processStartInfo = new ProcessStartInfo(compiler + " \"" + GetCompiledOutputName(fileName) + "\"");
			
			Process p =	new Process();
			p.StartInfo = processStartInfo;
			p.Start();
			
			return null;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			return null;
		}
	}
}
