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
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.Core.Services;

namespace JScriptBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class JScriptBindingExecutionServices
	{	
		public void Execute(string filename)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			Process.Start("\"" + exe + "\"" );
		}
		
		public void Execute(IProject project)
		{
			JScriptCompilerParameters parameters = (JScriptCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));			
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((JScriptCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			

			string exe = Path.ChangeExtension(((JScriptCompilerParameters)project.ActiveConfiguration).OutputAssembly, ".exe");
			
			//if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput)
			//	Process.Start("cmd", "/c " + "\"" + directory + exe + "\"" + " & pause");
			//else
			//	Process.Start("cmd", "/c \"" + directory + exe + "\"");
		}
		
	}
}
