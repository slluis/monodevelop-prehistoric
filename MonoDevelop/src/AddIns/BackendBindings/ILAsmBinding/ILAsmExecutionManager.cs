// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
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
using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.ILAsmBinding
{
	/// <summary>
	/// This class describes the main functionalaty of a language codon
	/// </summary>
	public class ILAsmExecutionManager
	{
		public void Execute(string filename, bool debug)
		{
			string exe = Path.ChangeExtension(filename, ".exe");
			ProcessStartInfo psi = new ProcessStartInfo("\"" + exe + "\"");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute = true;
			
			//DebuggerService debuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			//debuggerService.StartWithoutDebugging(psi);
		}
		
		public void Execute(IProject project, bool debug)
		{
			ILAsmCompilerParameters parameters = (ILAsmCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			string exe = Path.GetFullPath(Path.Combine(parameters.OutputDirectory, parameters.OutputAssembly) + ".exe");
			ProcessStartInfo psi = new ProcessStartInfo("\"" + exe  + "\"");
			psi.WorkingDirectory = Path.GetDirectoryName(exe);
			psi.UseShellExecute  = true;
			
			//DebuggerService debuggerService  = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			//debuggerService.StartWithoutDebugging(psi);
		}
	}
}
