// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
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

namespace VBBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class VBBindingExecutionServices
	{	

		public void Execute(string filename)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c " + "\"" + exe + "\"" + " & pause");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = false;
			
			try {
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + exe + "\"\n(.NET bug? Try restaring SD or manual start)");
			}
		}
		
		public void Execute(IProject project)
		{
			VBCompilerParameters parameters = (VBCompilerParameters)project.ActiveConfiguration;
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(parameters.OutputDirectory);
			string exe = parameters.OutputAssembly + ".exe";
			string args = parameters.CommandLineParameters;

			ProcessStartInfo psi;
			
			if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput) {
				psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c \"" + directory + exe + "\" " + args +  " & pause");
			} else {
				psi = new ProcessStartInfo(directory + exe);
				psi.Arguments = args;
			}
			
			try {
				psi.WorkingDirectory = Path.GetDirectoryName(directory);
				psi.UseShellExecute = false;
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + exe + "\"\n(.NET bug? Try restaring SD or manual start)");
			}
		}
	}
}
