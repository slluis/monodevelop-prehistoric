// created on 12/17/2004 at 22:07
using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Diagnostics;

using MonoDevelop.Core.Services;

namespace MonoDevelop.Services
{
	public class ProcessService : AbstractService
	{
		public ProcessWrapper StartProcess (string command, string arguments, string workingDirectory, EventHandler exited) 
		{
			return StartProcess (command, arguments, workingDirectory, (ProcessEventHandler)null, (ProcessEventHandler)null, exited);	
		}
		
		public ProcessWrapper StartProcess (string command, string arguments, string workingDirectory, ProcessEventHandler outputStreamChanged, ProcessEventHandler errorStreamChanged)
		{	
			return StartProcess (command, arguments, workingDirectory, outputStreamChanged, errorStreamChanged, null);
		}
		
		public ProcessWrapper StartProcess (string command, string arguments, string workingDirectory, TextWriter outWriter, TextWriter errorWriter, EventHandler exited) 
		{
			ProcessEventHandler wout = OutWriter.GetWriteHandler (outWriter);
			ProcessEventHandler werr = OutWriter.GetWriteHandler (errorWriter);
			return StartProcess (command, arguments, workingDirectory, wout, werr, exited);	
		}
		
		public ProcessWrapper StartProcess (string command, string arguments, string workingDirectory, ProcessEventHandler outputStreamChanged, ProcessEventHandler errorStreamChanged, EventHandler exited)
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
			return p;
		}
		
		public ProcessWrapper StartConsoleProcess (string command, string arguments, string workingDirectory, bool pauseBeforeExit, EventHandler exited) 
		{
			string additionalCommands = "";
			if (pauseBeforeExit)
				additionalCommands = @"echo; read -p 'Press any key to continue...' -n1;";
			ProcessStartInfo psi = new ProcessStartInfo("xterm",
				String.Format (@"-e ""cd {3} ; '{0}' {1} ; {2}""", command, arguments, additionalCommands, workingDirectory));
			psi.UseShellExecute = false;
			psi.WorkingDirectory = workingDirectory;
			psi.UseShellExecute  =  false;
			
			ProcessWrapper p = new ProcessWrapper();
			
			if (exited != null)
				p.Exited += exited;
				
			p.StartInfo = psi;
			p.Start();
			return p;
		}
		
	}
	
	class OutWriter
	{
		TextWriter writer;
		
		public OutWriter (TextWriter writer)
		{
			this.writer = writer;
		}
		
		public void WriteOut (object sender, string s)
		{
			writer.WriteLine (s);
		}
		
		public static ProcessEventHandler GetWriteHandler (TextWriter tw)
		{
			return tw != null ? new ProcessEventHandler(new OutWriter (tw).WriteOut) : null;
		}
	}
}
