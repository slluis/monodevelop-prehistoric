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
			ProcessStartInfo psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec"), "/c java " + "\"" + exe + "\"" + " & pause");
			psi.WorkingDirectory = Path.GetDirectoryName(filename);
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
			JavaCompilerParameters parameters = (JavaCompilerParameters)project.ActiveConfiguration;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string directory = fileUtilityService.GetDirectoryNameWithSeparator(((JavaCompilerParameters)project.ActiveConfiguration).OutputDirectory);
			
			string CurrentDir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(parameters.OutputDirectory);
			if(((JavaCompilerParameters)project.ActiveConfiguration).MainClass==null)				
				Process.Start("java.exe",((JavaCompilerParameters)project.ActiveConfiguration).OutputAssembly);				
			else
				Process.Start("java.exe",((JavaCompilerParameters)project.ActiveConfiguration).MainClass);				
			
			Directory.SetCurrentDirectory(CurrentDir);		
		}
				
	}
}
