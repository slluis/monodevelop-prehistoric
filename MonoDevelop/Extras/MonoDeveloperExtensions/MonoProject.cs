//
// MonoProject.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.IO;
using MonoDevelop.Services;
using System.Text.RegularExpressions;
using MonoDevelop.Internal.Project;
using System.CodeDom.Compiler;

namespace MonoDeveloper
{
	public class MonoProject: Project
	{
		string outFile;
		ArrayList refNames = new ArrayList ();
		bool loading;
		
		public override string ProjectType {
			get { return "MonoMakefile"; }
		}
		
		internal MonoProject (MonoMakefile mkfile)
		{
			Read (mkfile);
		}
		
		void Read (MonoMakefile mkfile)
		{
			loading = true;
			
			string basePath = Path.GetDirectoryName (mkfile.FileName);
			string aname;
			
			string targetAssembly = mkfile.GetVariable ("LIBRARY");
			if (targetAssembly == null) {
				targetAssembly = mkfile.GetVariable ("PROGRAM");
				if (Path.GetDirectoryName (targetAssembly) == "")
					targetAssembly = Path.Combine (basePath, targetAssembly);
				aname = Path.GetFileName (targetAssembly);
			} else {
				aname = Path.GetFileName (targetAssembly);
				string targetName = mkfile.GetVariable ("LIBRARY_NAME");
				if (targetName != null) targetAssembly = targetName;
				targetAssembly = "$(topdir)/class/lib/$(PROFILE)/" + targetAssembly;
			}
			
			Name = Path.GetFileNameWithoutExtension (aname);
			outFile = Path.Combine (basePath, aname);
			FileName = mkfile.FileName;
			
			// Parse projects
			string sources = outFile + ".sources";
			StreamReader sr = new StreamReader (sources);
			string line;
			while ((line = sr.ReadLine ()) != null) {
				line = line.Trim (' ','\t');
				if (line != "") ProjectFiles.Add (new ProjectFile (Path.Combine (basePath, line)));
			}
			
			sr.Close ();
			
			ArrayList files = new ArrayList ();
			FindFiles (basePath, "ChangeLog", files);
			foreach (string file in files)
				ProjectFiles.Add (new ProjectFile (file, BuildAction.Exclude));
			
			// Project references
			string refs = mkfile.GetVariable ("LIB_MCS_FLAGS");
			if (refs == null || refs == "") refs = mkfile.GetVariable ("LOCAL_MCS_FLAGS");
			
			if (refs != null && refs != "") {
				Regex var = new Regex(@"(.*?/r:(?<ref>.*?)(( |\t)|$).*?)*");
				Match match = var.Match (refs);
				if (match.Success) {
					foreach (Capture c in match.Groups["ref"].Captures)
						refNames.Add (Path.GetFileNameWithoutExtension (c.Value));
				}
			}
			
			int i = basePath.LastIndexOf ("/mcs/", basePath.Length - 2);
			string topdir = basePath.Substring (0, i + 4);
			targetAssembly = targetAssembly.Replace ("$(topdir)", topdir);
			
			MonoProjectConfiguration conf = new MonoProjectConfiguration (".NET 1.1", "default");
			conf.OutputDirectory = basePath;
			conf.AssemblyPathTemplate = targetAssembly;
			Configurations.Add (conf);
			
			conf = new MonoProjectConfiguration (".NET 2.0", "net_2_0");
			conf.OutputDirectory = basePath;
			conf.AssemblyPathTemplate = targetAssembly;
			Configurations.Add (conf);
			
			Console.WriteLine ("{0} {1}", aname, GetOutputFileName ());
			loading = false;
			Runtime.ProjectService.CombineOpened += new CombineEventHandler (CombineOpened);
		}
		
		void FindFiles (string path, string name, ArrayList files)
		{
			files.AddRange (Directory.GetFiles (path, name));
			foreach (string sd in Directory.GetDirectories (path))
				FindFiles (sd, name, files);
		}
		
		static Regex regexError = new Regex (@"^(\s*(?<file>.*)\((?<line>\d*)(,(?<column>\d*))?\)\s+)*(?<level>\w+)\s*(?<number>.*):\s(?<message>.*)",
				RegexOptions.Compiled | RegexOptions.ExplicitCapture);
				
		protected override ICompilerResult DoBuild (IProgressMonitor monitor)
		{
			MonoProjectConfiguration conf = (MonoProjectConfiguration) ActiveConfiguration;
			
			StringWriter output = new StringWriter ();
			LogTextWriter tw = new LogTextWriter ();
			tw.ChainWriter (output);
			tw.ChainWriter (monitor.Log);
			
			ProcessWrapper proc = Runtime.ProcessService.StartProcess ("make", "PROFILE=" + conf.Profile, conf.OutputDirectory, tw, tw, null);
			proc.WaitForOutput ();
			
			CompilerResults cr = new CompilerResults (null);			
			string[] lines = output.ToString().Split ('\n');
			foreach (string line in lines) {
				CompilerError err = CreateErrorFromString (line);
				if (err != null) cr.Errors.Add (err);
			}
			
			return new DefaultCompilerResult (cr, output.ToString());
		}
		
		private CompilerError CreateErrorFromString (string error_string)
		{
			// When IncludeDebugInformation is true, prevents the debug symbols stats from braeking this.
			if (error_string.StartsWith ("WROTE SYMFILE") ||
			    error_string.StartsWith ("make[") ||
			    error_string.StartsWith ("OffsetTable") ||
			    error_string.StartsWith ("Compilation succeeded") ||
			    error_string.StartsWith ("Compilation failed"))
				return null;

			CompilerError error = new CompilerError();

			Match match=regexError.Match(error_string);
			if (!match.Success) return null;
			if (String.Empty != match.Result("${file}"))
				error.FileName = Path.Combine (BaseDirectory, match.Result("${file}"));
			if (String.Empty != match.Result("${line}"))
				error.Line=Int32.Parse(match.Result("${line}"));
			if (String.Empty != match.Result("${column}"))
				error.Column = Int32.Parse(match.Result("${column}"));
			if (match.Result("${level}") == "warning")
				error.IsWarning = true;
			error.ErrorNumber = match.Result ("${number}");
			error.ErrorText = match.Result ("${message}");
			return error;
		}
		
		public void Install (IProgressMonitor monitor)
		{
			MonoProjectConfiguration conf = (MonoProjectConfiguration) ActiveConfiguration;
			monitor.BeginTask ("Installing: " + Name + " - " + conf.Name, 1);
			ProcessWrapper proc = Runtime.ProcessService.StartProcess ("make", "install PROFILE=" + conf.Profile, conf.OutputDirectory, monitor.Log, monitor.Log, null);
			proc.WaitForOutput ();
			monitor.EndTask ();
		}
		
		public override void Debug (IProgressMonitor monitor)
		{
		}
		
		public override string GetOutputFileName ()
		{
			MonoProjectConfiguration conf = (MonoProjectConfiguration) ActiveConfiguration;
			return conf.GetAssemblyPath ();
		}
		
		public void CombineOpened (object sender, CombineEventArgs args)
		{
			foreach (string pref in refNames) {
				Project p = Runtime.ProjectService.GetProject (pref);
				if (p != null) ProjectReferences.Add (new ProjectReference (p));
			}
		}
		
		protected override void OnFileAddedToProject (ProjectFileEventArgs e)
		{
			base.OnFileAddedToProject (e);
			if (loading) return;
			
			if (e.ProjectFile.BuildAction != BuildAction.Compile)
				return;

			StreamReader sr = null;
			StreamWriter sw = null;
			
			try {
				sr = new StreamReader (outFile + ".sources");
				sw = new StreamWriter (outFile + ".sources.new");

				string newFile = GetRelativeChildPath (e.ProjectFile.Name);
				if (newFile.StartsWith ("./")) newFile = newFile.Substring (2);
				
				string line;
				while ((line = sr.ReadLine ()) != null) {
					string file = line.Trim (' ','\t');
					if (newFile != null && (file == "" || string.Compare (file, newFile) > 0)) {
						sw.WriteLine (newFile);
						newFile = null;
					}
					sw.WriteLine (line);
				}
				if (newFile != null)
					sw.WriteLine (newFile);
			} finally {
				if (sr != null) sr.Close ();
				if (sw != null) sw.Close ();
			}
			File.Delete (outFile + ".sources");
			File.Move (outFile + ".sources.new", outFile + ".sources");
		}
		
		protected override void OnFileRemovedFromProject (ProjectFileEventArgs e)
		{
			base.OnFileRemovedFromProject (e);
			if (loading) return;
			
			if (e.ProjectFile.BuildAction != BuildAction.Compile)
				return;

			StreamReader sr = null;
			StreamWriter sw = null;
			
			try {
				sr = new StreamReader (outFile + ".sources");
				sw = new StreamWriter (outFile + ".sources.new");

				string oldFile = GetRelativeChildPath (e.ProjectFile.Name);
				if (oldFile.StartsWith ("./")) oldFile = oldFile.Substring (2);
				
				string line;
				while ((line = sr.ReadLine ()) != null) {
					string file = line.Trim (' ','\t');
					if (oldFile != file)
						sw.WriteLine (line);
				}
			} finally {
				if (sr != null) sr.Close ();
				if (sw != null) sw.Close ();
			}
			File.Delete (outFile + ".sources");
			File.Move (outFile + ".sources.new", outFile + ".sources");
		}
		
		public override void GenerateMakefiles (Combine parentCombine)
		{
		}
		
		public override void Dispose ()
		{
			base.Dispose ();
			Runtime.ProjectService.CombineOpened -= new CombineEventHandler (CombineOpened);
		}
	}
}
