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
			
			bool SetErrorType(CompilerError error, string t)
			{
				switch(t)
				{
					case "error":
						error.IsWarning = false;
						return true;
					case "warning":
						error.IsWarning = true;
						return true;
					case "hint":
						error.IsWarning = true;
						error.ErrorNumber = "COMMENT";
						return true;
					default:
						return false;
				}
			}

			public void Parse(string l)
			{
				CompilerError error = new CompilerError();
				error.ErrorNumber = String.Empty;

				char [] delim = {':'};
				string [] s = l.Split(delim, 5);
				
				if (SetErrorType(error, s[0]))
				{
					error.ErrorText = l.Substring(l.IndexOf(s[0]+": ") + s[0].Length+2);
					error.FileName  = "";
					error.Line      = 0;
					error.Column    = 0;
				} else
				if ((s.Length >= 4)  && SetErrorType(error, s[3].Substring(1)))
				{
					error.ErrorText = l.Substring(l.IndexOf(s[3]+": ") + s[3].Length+2);
					error.FileName  = s[0];
					error.Line      = int.Parse(s[1]);
					error.Column    = int.Parse(s[2]);
				} else
				{
					error.ErrorText = l;
					error.FileName  = "";
					error.Line      = 0;
					error.Column    = 0;
					error.IsWarning = false;					
				}
				Errors.Add(error);
			}

			public ICompilerResult GetResult()
			{
				return new DefaultCompilerResult(this, "");
			} 
		}
	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
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
			
			return fileUtilityService.GetDirectoryNameWithSeparator(cp.OutputDirectory)
					+ cp.OutputAssembly + ((int)cp.Target == 0?".exe":".dll");
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

			if (!Directory.Exists(cp.OutputDirectory))
				Directory.CreateDirectory(cp.OutputDirectory);
			
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
			ProcessStartInfo si = new ProcessStartInfo(ncc, arguments);
			si.RedirectStandardOutput = true;
			si.RedirectStandardError = true;
			si.UseShellExecute = false;
			VProcess p = new VProcess();
			p.StartInfo = si;
			p.Start();

			IStatusBarService sbs = (IStatusBarService)ServiceManager.GetService (typeof (IStatusBarService));
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
				cr.Parse("error: " + ncc + " execution problem");
			}
			
			return cr.GetResult();
		}
	}
}
