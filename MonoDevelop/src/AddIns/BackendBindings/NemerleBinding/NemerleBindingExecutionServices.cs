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

namespace NemerleBinding
{
	public class NemerleBindingExecutionServices
	{	
		
		public void Execute(string filename)
		{
			throw new ApplicationException("No ExecuteFile");
		}
		
		public void Execute(IProject project)
		{
			
			NemerleParameters p = (NemerleParameters)project.ActiveConfiguration;
			FileUtilityService fus = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string dir = fus.GetDirectoryNameWithSeparator(p.OutputPath);
			string exe;
			
			if (p.ExecuteCommand == String.Empty)
			{
				exe	= "mono --debug";
			} else
			{
				exe = p.ExecuteCommand;
			}
			
			exe += " " + p.AssemblyName + ".exe " + p.Parameters;
			
			try {
				string currentDir = Directory.GetCurrentDirectory();
				Directory.SetCurrentDirectory(dir);				
				
				ProcessStartInfo psi = new ProcessStartInfo(exe);
				psi.WorkingDirectory = dir;
				psi.UseShellExecute = false;
				
				Process pr = new Process();
				pr.StartInfo = psi;
				pr.Start();
				Directory.SetCurrentDirectory(currentDir);	
			} catch (Exception) {
				throw new ApplicationException("Can not execute");
			}
		}
				
	}
}
