// created on 12/17/2004 at 22:07
using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;

using MonoDevelop.Core.Services;

namespace MonoDevelop.Services
{
	public class ProcessService : AbstractService
	{
		public void StartProcess (string command, string arguments, string workingDirectory, EventHandler exited) 
		{
			StartProcess (command, arguments, workingDirectory, null, null, exited);	
		}
		
		public void StartProcess (string command, string arguments, string workingDirectory, ProcessEventHandler outputStreamChanged, ProcessEventHandler errorStreamChanged)
		{	
			StartProcess (command, arguments, workingDirectory, outputStreamChanged, errorStreamChanged, null);
		}
		
		public void StartProcess (string command, string arguments, string workingDirectory, ProcessEventHandler outputStreamChanged, ProcessEventHandler errorStreamChanged, EventHandler exited)
		{
			if (command == null)
				throw new ArgumentNullException("command");
			
			if (command.Length == 0)
				throw new ArgumentException("command");
		
			ProcessWrapper p = new ProcessWrapper();

			if (outputStreamChanged != null) {
				p.OutputStreamChanged += outputStreamChanged;
			}
				
			if (errorStreamChanged != null)
				p.ErrorStreamChanged += errorStreamChanged;
			
			if (exited != null)
				p.Exited += exited;
				
			if(arguments == null || arguments.Length == 0)
				p.StartInfo = new ProcessStartInfo (command);
			else
				p.StartInfo = new ProcessStartInfo (command, arguments);
			
			if(workingDirectory != null && workingDirectory.Length > 0)
				p.StartInfo.WorkingDirectory = workingDirectory;


			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.UseShellExecute = false;
			p.EnableRaisingEvents = true;
			
			p.Start ();
		}
	}
}
