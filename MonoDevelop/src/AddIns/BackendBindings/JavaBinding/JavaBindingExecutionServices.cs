// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Pads;
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
			string mainClass = ((JavaCompilerParameters) project.ActiveConfiguration).MainClass;
			
			string CurrentDir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory (parameters.OutputDirectory);
		
			string javaExec;
			switch (parameters.Runtime) {
				case JavaRuntime.Ikvm:
					javaExec = "-e \"ikvm -classpath " + parameters.ClassPath + " " + mainClass + ";read -p 'press any key to continue...' -n1\"";
				break;
				// FIXME: need to both compile with ikvmc
				// and then run with mono
				case JavaRuntime.Mono:
					javaExec = "-e \"ikvm -classpath " + parameters.ClassPath + " " + mainClass + ";read -p 'press any key to continue...' -n1\"";
					break;
				case JavaRuntime.Java:
					javaExec = "-e \"java -classpath " + parameters.ClassPath + " " + mainClass + ";read -p 'press any key to continue...' -n1\"";
					break;
				case JavaRuntime.Gij:
					javaExec = "-e \"gij -classpath " + parameters.ClassPath + " " + mainClass + ";read -p 'press any key to continue...' -n1\"";
					break;
				default:
					javaExec = "-e \"ikvm -classpath " + parameters.ClassPath + " " + mainClass + ";read -p 'press any key to continue...' -n1\"";
					break;
			}

			ProcessStartInfo psi = new ProcessStartInfo("xterm", javaExec);

            try {
                psi.WorkingDirectory = Path.GetDirectoryName (directory);
                psi.UseShellExecute = false;

                Process p = new Process ();
                p.StartInfo = psi;
                p.Start ();
            } catch (Exception) {
                throw new ApplicationException ("Can not execute " + "\"" + directory + mainClass + "\"\n(Try restarting MonoDevelop or start your app manually)");
            }

/*
			//FIXME: find out how to set the working dir better
			TerminalPad outputPad = (TerminalPad) WorkbenchSingleton.Workbench.GetPad (typeof (TerminalPad));
			outputPad.RunCommand ("cd " + parameters.OutputDirectory);

			string runtime = "ikvm"; // make it project.RuntimeOptions or so
			switch (runtime) {
				// is this even supposed to work with CLI binaries?
				//case "java": // use an installed jre
				//	outputPad.RunCommand ("java -classpath " + parameters.ClassPath + " "  + ((JavaCompilerParameters) project.ActiveConfiguration).MainClass);
				//	break;
				case "ikvm": // JIT to Java then JIT to mono
					outputPad.RunCommand ("ikvm -classpath " + parameters.ClassPath + " "  + ((JavaCompilerParameters) project.ActiveConfiguration).MainClass);
					break;
				default: // run compiled to exe with mono
					string command = "ikvmc -reference:/usr/lib/classpath.dll " + ((JavaCompilerParameters) project.ActiveConfiguration).MainClass + ".class ";
					string[] allJars = parameters.ClassPath.Split (':');
					foreach (string jar in allJars)
					{
						if (jar != ".")
							command += jar + " ";
					}
					outputPad.RunCommand (command);
					outputPad.RunCommand ("mono " + ((JavaCompilerParameters) project.ActiveConfiguration).MainClass + ".exe");
					break;
			}
			outputPad.RunCommand ("cd -");
*/				
		}
	}
}
