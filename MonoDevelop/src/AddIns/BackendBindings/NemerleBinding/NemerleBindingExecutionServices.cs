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
			string exe;
			
			if (p.ExecuteScript == String.Empty)
			{
				exe	= "mono --debug";
			} else
			{
				exe = p.ExecuteScript;
			}
			
			exe += " " + p.OutputAssembly + ".exe " + p.Parameters;
			
			try {
				ProcessStartInfo psi = new ProcessStartInfo("xterm",
					string.Format (
					@"-e ""{0} ;echo;read -p 'press any key to continue...' -n1""",
					exe));
				psi.WorkingDirectory = fus.GetDirectoryNameWithSeparator(p.OutputDirectory);
				psi.UseShellExecute = false;
				
				Process pr = new Process();
				pr.StartInfo = psi;
				pr.Start();
			} catch (Exception) {
				throw new ApplicationException("Can not execute");
			}
		}
				
	}
}
