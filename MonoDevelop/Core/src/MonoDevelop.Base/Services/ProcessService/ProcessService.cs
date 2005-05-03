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
		ProcessHostController externalProcess;
		
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
		
		public ProcessWrapper StartConsoleProcess (string command, string arguments, string workingDirectory, bool externalConsole, bool pauseBeforeExit, EventHandler exited) 
		{
			if (externalConsole) {
				string additionalCommands = "";
				if (pauseBeforeExit)
					additionalCommands = @"echo; read -p 'Press any key to continue...' -n1;";
				ProcessStartInfo psi = new ProcessStartInfo("xterm",
					String.Format (@"-e ""cd {3} ; '{0}' {1} ; {2}""", command, arguments, additionalCommands, workingDirectory));
				psi.UseShellExecute = false;
				
				if (workingDirectory != null)
					psi.WorkingDirectory = workingDirectory;

				psi.UseShellExecute  =  false;
				
				ProcessWrapper p = new ProcessWrapper();
				
				if (exited != null)
					p.Exited += exited;
					
				p.StartInfo = psi;
				p.Start();
				return p;
			} else {
				// This should create an vte pad instead, but an output panel will be enough until we can do it
				IProgressMonitor monitor = Runtime.TaskService.GetOutputProgressMonitor ("Application Output", MonoDevelop.Gui.Stock.RunProgramIcon, true, true);

				ProcessMonitor pm = new ProcessMonitor ();
				pm.Exited = exited;
				pm.Monitor = monitor;
				pm.CancelHandler = new MonitorHandler(pm.OnCancelRequest);

				monitor.CancelRequested += pm.CancelHandler;

				ProcessWrapper pw = StartProcess (command, arguments, workingDirectory, monitor.Log, monitor.Log, new EventHandler (pm.OnExited));
				pm.ProcessWrapper = pw;

				return pw;
			}
		}
		
		ProcessHostController GetHost (bool shared)
		{
			if (!shared)
				return new ProcessHostController (0);
			
			lock (this) {
				if (externalProcess == null)
					externalProcess = new ProcessHostController (10000);
	
				return externalProcess;
			}
		}
		
		public RemoteProcessObject CreateExternalProcessObject (Type type)
		{
			return CreateExternalProcessObject (type, true);
		}
		
		public RemoteProcessObject CreateExternalProcessObject (Type type, bool shared)
		{
			return GetHost (shared).CreateInstance (type);
		}
		
		public RemoteProcessObject CreateExternalProcessObject (string assemblyPath, string typeName, bool shared)
		{
			return GetHost (shared).CreateInstance (assemblyPath, typeName);
		}
	}
	
	class ProcessMonitor
	{
		public IProgressMonitor Monitor;
		public EventHandler Exited;

		public ProcessWrapper ProcessWrapper;
		public MonitorHandler CancelHandler;
		
		public void OnExited (object sender, EventArgs args)
		{
			ProcessWrapper p = (ProcessWrapper) sender;
			try {
				if (Exited != null)
					Exited (sender, args);
				p.WaitForOutput ();
			} finally {
				Monitor.Dispose ();
			}
		}

		public void OnCancelRequest (IProgressMonitor monitor) {
			if (ProcessWrapper != null) {
				if (!ProcessWrapper.HasExited) {
					ProcessWrapper.Kill();
					monitor.Log.WriteLine("Application stopped by user.");
				}
			}
			//remove the cancel handler, it will be attached again when StartConsoleProcess is called
			monitor.CancelRequested -= CancelHandler;
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
