//
// AssemblyCodeCompletionDatabase.cs
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
using System.Diagnostics;
using System.IO;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Parser;
using System.Reflection;

namespace MonoDevelop.Services
{	
	internal class AssemblyCodeCompletionDatabase: CodeCompletionDatabase
	{
		bool useExternalProcess = true;
		string baseDir;
		string assemblyName;
		
		public AssemblyCodeCompletionDatabase (string baseDir, string assemblyName, DefaultParserService parserService)
		: base (parserService)
		{
			string assemblyFile;
			string name;
			Assembly asm = null;
			
			if (assemblyName.ToLower().EndsWith (".dll")) 
			{
				name = assemblyName.Substring (0, assemblyName.Length - 4);
				name = name.Replace(',','_').Replace(" ","").Replace('/','_');
				assemblyFile = assemblyName;
				try {
					asm = Assembly.LoadFrom (assemblyFile);
				}
				catch {}
				
				if (asm == null) {
					Runtime.LoggingService.Info ("Could not load assembly: " + assemblyFile);
					return;
				}
			}
			else 
			{
				asm = FindAssembly (assemblyName);
				
				if (asm == null) {
					Runtime.LoggingService.Info ("Could not load assembly: " + assemblyName);
					return;
				}
				
				assemblyName = asm.GetName().FullName;
				name = EncodeGacAssemblyName (assemblyName);
				assemblyFile = asm.Location;
			}
			
			this.assemblyName = assemblyName;
			this.baseDir = baseDir;
			
			SetLocation (baseDir, name);

			Read ();
			
			if (files [assemblyFile] == null) {
				AddFile (assemblyFile);
				headers ["CheckFile"] = assemblyFile;
			}
			
			// Update references to other assemblies
			
			Hashtable rs = new Hashtable ();
			foreach (AssemblyName aname in asm.GetReferencedAssemblies ()) {
				string uri = "Assembly:" + aname.ToString();
				rs[uri] = null;
				if (!HasReference (uri))
					AddReference (uri);
			}
			
			ArrayList keys = new ArrayList ();
			keys.AddRange (references);
			foreach (ReferenceEntry re in keys)
			{
				if (!rs.Contains (re.Uri))
					RemoveReference (re.Uri);
			}
		}
		
		public static string GetFullAssemblyName (string s)
		{
			if (s.ToLower().EndsWith (".dll")) 
				return s;
				
			Assembly asm = FindAssembly (s);
			
			if (asm != null)
				return asm.GetName().FullName;
			else
				return s;
		}
		
		public static Assembly FindAssembly (string name)
		{
			Assembly asm = null;
			try {
				asm = Assembly.Load (name);
			}
			catch {}
			
			if (asm == null) {
				try {
					asm = Assembly.LoadWithPartialName (name);
				}
				catch {}
			}
			return asm;
		}
		
		string EncodeGacAssemblyName (string assemblyName)
		{
			string[] assemblyPieces = assemblyName.Split(',');
			string res = "";
			foreach (string item in assemblyPieces) {
				string[] pieces = item.Trim ().Split (new char[] { '=' }, 2);
				if(pieces.Length == 1)
					res += pieces[0];
				else if (!(pieces[0] == "Culture" && pieces[1] != "Neutral"))
					res += "_" + pieces[1];
			}
			return res;
		}
		
		public string AssemblyName
		{
			get { return assemblyName; }
		}
		
		protected override void ParseFile (string fileName, IProgressMonitor parentMonitor)
		{
			IProgressMonitor monitor = parentMonitor;
			if (parentMonitor == null) monitor = parserService.GetParseProgressMonitor ();
			
			try {
				monitor.BeginTask ("Parsing assembly: " + Path.GetFileName (fileName), 1);
				if (useExternalProcess)
				{
					string dbgen = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "dbgen.exe");
					Process proc = Process.Start ("mono " + dbgen, "\"" + baseDir + "\" \"" + assemblyName + "\"");
					proc.WaitForExit ();
					Read ();
				}
				else
				{
					AssemblyInformation ainfo = new AssemblyInformation();
					ainfo.Load (fileName, false);
					UpdateClassInformation (ainfo.Classes, fileName);
				}
			} finally {
				monitor.EndTask ();
				if (parentMonitor == null) monitor.Dispose ();
			}
		}
		
		public bool ParseInExternalProcess
		{
			get { return useExternalProcess; }
			set { useExternalProcess = value; }
		}
		
		public static void CleanDatabase (string baseDir, string name)
		{
			// Read the headers of the file without fully loading the database
			Hashtable headers = ReadHeaders (baseDir, name);
			string checkFile = (string) headers ["CheckFile"];
			int version = (int) headers ["Version"];
			if (!File.Exists (checkFile) || version != FORMAT_VERSION) {
				string dataFile = Path.Combine (baseDir, name + ".pidb");
				File.Delete (dataFile);
				Runtime.LoggingService.Info ("Deleted " + dataFile);
			}
		}
	}
}
