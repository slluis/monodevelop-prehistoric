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
using MonoDevelop.Gui;
using MonoDevelop.Core.Services;

namespace JavaBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class JavaBindingExecutionServices
	{	
		
		public void Execute(string filename)
		{
			string exe = Path.GetFileNameWithoutExtension(filename);
			ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c java\"" + " & pause");
			psi.WorkingDirectory = Path.GetDirectoryName(filename);
			psi.UseShellExecute = false;
			try {
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can not execute " + "\"" + exe + "\"\n(Try restarting MonoDevelop or manual start)");
			}
		}
		
		public void Execute(IProject project)
		{
			JavaCompilerParameters parameters = (JavaCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((JavaCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			
			string CurrentDir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(parameters.OutputDirectory);
			ProcessStartInfo psi;
			if(((JavaCompilerParameters)project.ActiveConfiguration).MainClass==null) {
					//FIXME:
				psi = new ProcessStartInfo("xterm -e \"java " + ((JavaCompilerParameters)project.ActiveConfiguration).OutputAssembly + ";read -p 'press any key to continue...' -n1\"");
			} else {
				if (parameters.PauseConsoleOutput) {
					//FIXME:
					psi = new ProcessStartInfo("xterm -e \"java " + ((JavaCompilerParameters)project.ActiveConfiguration).MainClass + ";read -p 'press any key to continue...' -n1\"");
				} else {
					//FIXME:
					psi = new ProcessStartInfo("xterm -e \"java " + ((JavaCompilerParameters)project.ActiveConfiguration).MainClass + ";read -p 'press any key to continue...' -n1\"");
				}
			}
			
			try {
				psi.WorkingDirectory = parameters.OutputDirectory;
				psi.UseShellExecute = false;
			
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can not execute");
			}
			
			Directory.SetCurrentDirectory(CurrentDir);		
		}
				
	}
}
