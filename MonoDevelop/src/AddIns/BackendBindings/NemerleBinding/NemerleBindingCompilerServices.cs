using System;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;
using System.Threading;

using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Components;
using MonoDevelop.Services;

namespace NemerleBinding
{
	public class NemerleBindingCompilerServices
	{
		class CompilerResultsParser : CompilerResults
		{
			public CompilerResultsParser() : base (new TempFileCollection ())
			{
			}

			public void Parse(string l)
			{
				if ((l.IndexOf(".n:") < 0) &&
					(l.IndexOf(":0:0:") < 0))
					return;				

				CompilerError error = new CompilerError();

				int s1 = l.IndexOf(':')+1;
				int s2 = l.IndexOf(':',s1)+1;
				int s3 = l.IndexOf(':',s2)+1;
				int s4 = l.IndexOf(':',s3)+1;

				error.FileName  = l.Substring(0, s1-1);
				error.Line   	= Int32.Parse(l.Substring(s1, s2-s1-1));
				error.Column    = Int32.Parse(l.Substring(s2, s3-s2-1));
				error.ErrorNumber = String.Empty;
				error.ErrorText = "";
				switch(l.Substring(s3+1, s4-s3-2))
				{
					case "error":
						error.IsWarning = false;
						break;
						case "warning":
						error.IsWarning = true;
						break;
					case "hint":
						error.IsWarning = true;
						error.ErrorText = "hint: ";
						break;
					default:
						error.IsWarning = false;
						error.ErrorText = "unknown: ";
						break;
				}
				error.ErrorText += l.Substring(s4+1);
				Errors.Add(error);
			}

			public ICompilerResult GetResult()
			{
				return new DefaultCompilerResult(this, "");
			} 
		}
	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		static string ncc = "ncc";

		private string GetOptionsString(NemerleParameters cp)
		{
			string options = " -q -no-color";
			if (cp.Nostdmacros)
				options += " -no-stdmacros";
			if (cp.Nostdlib)
				options += " -no-stdlib";
			if (cp.Ot)
				options += " -Ot";
			if (cp.Obcm)
				options += " -Obcm";
			if (cp.Oocm)
				options += " -Oocm";
			if (cp.Oscm)
				options += " -Oscm";
			if ((int)cp.Target == 1)
				options += " -tdll";
				
			return options;			
		}
		
		public bool CanCompile(string fileName)
		{
			return Path.GetExtension(fileName) == ".n";
		} 
		
		public ICompilerResult CompileFile(string fileName)
		{
			throw new ApplicationException("No CompileFile");
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			throw new ApplicationException("No CompileFile");
		}

		public string GetCompiledOutputName(IProject project)
		{
			NemerleParameters cp = (NemerleParameters)project.ActiveConfiguration;
			
			return fileUtilityService.GetDirectoryNameWithSeparator(cp.OutputPath)
					+ cp.AssemblyName + ((int)cp.Target == 0?".exe":".dll");
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			NemerleParameters cp = (NemerleParameters)project.ActiveConfiguration;
			
			string references = "";
			string files   = "";
			
			foreach (ProjectReference lib in project.ProjectReferences)
				references += " -r \"" + lib.GetReferencedFileName(project) + "\"";
			
			foreach (ProjectFile f in project.ProjectFiles)
				if (f.Subtype != Subtype.Directory)
					switch (f.BuildAction)
					{
						case BuildAction.Compile:
							files += " \"" + f.Name + "\"";
						break;
					}

			if (!Directory.Exists(cp.OutputPath))
				Directory.CreateDirectory(cp.OutputPath);
			
			string args = GetOptionsString(cp) + references + files  + " -o " + GetCompiledOutputName(project);
			return DoCompilation (args);
		}
		
		// This enables check if we have output without blocking 
		class VProcess : Process
		{
			Thread t = null;
			public void thr()
			{
				while (StandardOutput.Peek() == -1){};
			}
			public void OutWatch()
			{
				t = new Thread(new ThreadStart(thr));
				t.Start();
			}
			public bool HasNoOut()
			{
				return t.IsAlive;
			} 
		}
		
		private ICompilerResult DoCompilation(string arguments)
		{
			string l;
			ProcessStartInfo si = new ProcessStartInfo("/bin/sh -c \"" + ncc + arguments + "\"");
			si.RedirectStandardOutput = true;
			si.RedirectStandardError = true;
			si.UseShellExecute = false;
			VProcess p = new VProcess();
			p.StartInfo = si;
			p.Start();

			IStatusBarService sbs = (IStatusBarService)ServiceManager.Services.GetService (typeof (IStatusBarService));
			sbs.SetMessage ("Compiling...");
			
			p.OutWatch();
			while ((!p.HasExited) && p.HasNoOut())
//			while ((!p.HasExited) && (p.StandardOutput.Peek() == -1)) // this could eliminate VProcess outgrowth
			{
				((SdStatusBar)sbs.ProgressMonitor).Pulse();
				while (Gtk.Application.EventsPending ())
					Gtk.Application.RunIteration ();
				System.Threading.Thread.Sleep (100);
			}
			
			CompilerResultsParser cr = new CompilerResultsParser();			
			while ((l = p.StandardOutput.ReadLine()) != null)
			{
				((SdStatusBar)sbs.ProgressMonitor).Pulse();
				while (Gtk.Application.EventsPending ())
					Gtk.Application.RunIteration ();
				cr.Parse(l);
			}
			((SdStatusBar)sbs.ProgressMonitor).Done();
			
			if  ((l = p.StandardError.ReadLine()) != null)
			{
				cr.Parse(":0:0: error: " + ncc + " execution problem");
			}
			
			return cr.GetResult();
		}
	}
}
