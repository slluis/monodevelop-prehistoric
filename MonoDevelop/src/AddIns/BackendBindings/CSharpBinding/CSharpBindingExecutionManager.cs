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
using MonoDevelop.Services;

namespace CSharpBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class CSharpBindingExecutionManager
	{
		public void Debug (IProject project)
		{
			FileUtilityService fileUtilityService = (FileUtilityService) ServiceManager.Services.GetService (typeof (FileUtilityService));
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((CSharpCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			string exe = ((CSharpCompilerParameters)project.ActiveConfiguration).OutputAssembly + ".exe";

			IDebuggingService dbgr = (IDebuggingService) ServiceManager.Services.GetService (typeof (IDebuggingService));
			if (dbgr != null)
				dbgr.Run (new string[] { Path.Combine (directory, exe) } );
		}

		public void Execute(string filename)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo("mono", "--debug " + exe);
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
			CSharpCompilerParameters parameters = (CSharpCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((CSharpCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			string exe = ((CSharpCompilerParameters)project.ActiveConfiguration).OutputAssembly + ".exe";
			string args = ((CSharpCompilerParameters)project.ActiveConfiguration).CommandLineParameters;
			
			ProcessStartInfo psi;
			if (parameters.ExecuteScript != null && parameters.ExecuteScript.Length > 0) {
				//Console.WriteLine("EXECUTE SCRIPT!!!!!!");
				psi = new ProcessStartInfo("\"" + parameters.ExecuteScript + "\"");
				psi.UseShellExecute = false;
			} else {
				string runtimeStarter = "mono ";
				
				switch (parameters.NetRuntime) {
					case NetRuntime.Mono:
						runtimeStarter = "mono ";
						break;
					case NetRuntime.MonoInterpreter:
						runtimeStarter = "mint ";
						break;
				}
				
				if (parameters.CompileTarget != CompileTarget.WinExe && parameters.PauseConsoleOutput) {
					psi = new ProcessStartInfo("gnome-terminal",
						string.Format (
						@"-x bash -c ""{0} '{1}{2}' {3} ; echo; read -p 'press any key to continue...' -n1""",
						runtimeStarter, directory, exe, args));
					psi.UseShellExecute = false;
				} else {
					psi = new ProcessStartInfo(runtimeStarter, "\"" + directory + exe + "\" " + args);
					psi.UseShellExecute = false;
				}
			}
			
			try {
				psi.WorkingDirectory = Path.GetDirectoryName(directory);
				psi.UseShellExecute  =  false;
				
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			} catch (Exception) {
				throw new ApplicationException("Can't execute " + "\"" + directory + exe + "\"\n(.NET bug? Try restaring SD or manual start)");
			}
		}
	}
}
