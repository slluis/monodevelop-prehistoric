using System;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;

using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;

namespace NemerleBinding
{
	public class NemerleBindingCompilerServices
	{	
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		static string ncc = "ncc -q -no-color";
		
		private string GetOptionsString(NemerleParameters cp)
		{
			string options = "";
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
			
			string outstr = ncc + GetOptionsString(cp) + references + files  + " -o " + GetCompiledOutputName(project);
			string output = "";
			string error  = "";
			TempFileCollection  tf = new TempFileCollection ();		
//			Executor.ExecWaitWithCapture(outstr, tf, ref error , ref output);
			DoCompilation (outstr, tf, ref output, ref error);			
			ICompilerResult cr = ParseOutput (tf, output);			
			File.Delete(output);
			File.Delete(error);
			return cr;
		}
		
		private void DoCompilation(string outstr, TempFileCollection tf, ref string output, ref string error)
		{
			output = Path.GetTempFileName();
			error = Path.GetTempFileName();
			
			string arguments = outstr + " > " + output + " 2> " + error;
			string command = arguments;
			ProcessStartInfo si = new ProcessStartInfo("/bin/sh -c \"" + command + "\"");
			si.RedirectStandardOutput = true;
			si.RedirectStandardError = true;
			si.UseShellExecute = false;
			Process p = new Process();
			p.StartInfo = si;
			p.Start();
			p.WaitForExit();
		}
		
		ICompilerResult ParseOutput(TempFileCollection tf, string file)
		{
			string compilerOutput = "";
			string l;		
			StreamReader sr = new StreamReader(file, System.Text.Encoding.Default);
			CompilerResults cr = new CompilerResults(tf);
			
			while ((l = sr.ReadLine())!=null) 
			{
				compilerOutput += l + "\n";

				if ((l.IndexOf(".n:") < 0) &&
					(l.IndexOf(":0:0:") < 0))
					continue;				

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
				
				cr.Errors.Add(error);
			}
			sr.Close();			
			return new DefaultCompilerResult(cr, compilerOutput);
		}
	}
}
