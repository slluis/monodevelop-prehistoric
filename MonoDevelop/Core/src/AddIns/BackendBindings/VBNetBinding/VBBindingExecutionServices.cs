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

using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;

//using CSharpBinding;

namespace VBBinding
{
	/// <summary>
	/// This class controls the compilation of C Sharp files and C Sharp projects
	/// </summary>
	public class VBBindingExecutionServices //: CSharpBindingExecutionManager
	{	
		public void Debug (IProject project)
		{
			FileUtilityService fileUtilityService = (FileUtilityService) ServiceManager.GetService (typeof (FileUtilityService));
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((VBCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			string exe = ((VBCompilerParameters)project.ActiveConfiguration).OutputAssembly + ".exe";

			IDebuggingService dbgr = (IDebuggingService) ServiceManager.GetService (typeof (IDebuggingService));
			if (dbgr != null)
				dbgr.Run (new string[] { Path.Combine (directory, exe) } );
		}

		public void Execute(string filename)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			
			ProcessStartInfo psi = new ProcessStartInfo("/usr/bin/mono " + exe);
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = false;
			try {
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can not execute " + "\"" + exe + "\"\n(Try restarting MonoDevelop or start your app manually)");
			}
		}
		
		public void Execute(IProject project)
		{
			VBCompilerParameters parameters = (VBCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
			
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((VBCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			string exe = ((VBCompilerParameters)project.ActiveConfiguration).OutputAssembly + ".exe";
			string args = ((VBCompilerParameters)project.ActiveConfiguration).CommandLineParameters;
			
			ProcessStartInfo psi;
			if (parameters.ExecuteScript != null && parameters.ExecuteScript.Length > 0) {
				//Console.WriteLine("EXECUTE SCRIPT!!!!!!");
				psi = new ProcessStartInfo("\"" + parameters.ExecuteScript + "\"");
				psi.UseShellExecute = false;
			} else {
				string runtimeStarter = "mono --debug ";
				
				switch (parameters.NetRuntime) {
					case NetRuntime.Mono:
						runtimeStarter = "mono --debug ";
						break;
					case NetRuntime.MonoInterpreter:
						runtimeStarter = "mint ";
						break;
				}
				
				string additionalCommands = "";
				if (parameters.PauseConsoleOutput)
					additionalCommands = @"echo; read -p 'press any key to continue...' -n1;";

				psi = new ProcessStartInfo("xterm",
					string.Format (
					@"-e ""{0} '{1}{2}' {3} ; {4}""",
					runtimeStarter, directory, exe, args, additionalCommands));
				psi.UseShellExecute = false;
			}
			
			try {
				psi.WorkingDirectory = Path.GetDirectoryName(directory);
				psi.UseShellExecute  =  false;
				
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can not execute " + "\"" + directory + exe + "\"\n(Try restarting MonoDevelop or start your app manually)");
			}
		}

	}
}
