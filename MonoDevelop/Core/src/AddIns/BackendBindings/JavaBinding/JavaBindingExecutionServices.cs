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
using MonoDevelop.Services;

namespace JavaBinding
{
	public class JavaBindingExecutionServices
	{	
		public void Execute (string filename)
		{
			throw new ApplicationException ("Cannot execute a file.");
		}
		
		public void Execute (IProject project)
		{
			JavaCompilerParameters parameters = (JavaCompilerParameters) project.ActiveConfiguration;
			string exe = ((JavaCompilerParameters) project.ActiveConfiguration).OutputAssembly;
			exe = Path.ChangeExtension (exe, ".exe");
			exe = Path.Combine (parameters.OutputDirectory, exe);
	
			if (!File.Exists (exe))
			{
				IMessageService messageService = (IMessageService) ServiceManager.GetService (typeof (IMessageService));
				messageService.ShowError (String.Format (GettextCatalog.GetString ("Error running {0}"), exe));
				return;
			}
			
			string javaExec = String.Format ("-e \"mono {0}; echo; read -p 'press any key to continue...' -n1\"", exe);
			ProcessStartInfo psi = new ProcessStartInfo ("xterm", javaExec);

            try
            {
                psi.UseShellExecute = false;

                Process p = new Process ();
                p.StartInfo = psi;
                p.Start ();
                p.WaitForExit ();
            }
            catch
            {
                throw new ApplicationException (String.Format ("Cannot execute: {0}", exe));
            }
		}
	}
}
