// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Lluis Sanchez Gual" email="lluis@ximian.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Internal.Parser;
using System.Reflection;

namespace MonoDevelop.Services
{
	internal class CodeCompletionDatabase
	{
		static readonly int MAX_ACTIVE_COUNT = 100;
		static readonly int MIN_ACTIVE_COUNT = 50;
		static readonly int FORMAT_VERSION = 2;
		
		NamespaceEntry rootNamespace;
		protected ArrayList references;
		protected Hashtable files;
		protected DefaultParserService parserService;
		protected Hashtable headers;
		
		BinaryReader datareader;
		FileStream datafile;
		int currentGetTime = 0;
		bool modified;
		
		string basePath;
		string dataFile;
		
		protected Object rwlock = new Object ();
		
		public CodeCompletionDatabase (DefaultParserService parserService)
		{
			this.parserService = parserService;
			rootNamespace = new NamespaceEntry ();
			files = new Hashtable ();
			references = new ArrayList ();
			headers = new Hashtable ();
		}
		
		protected void SetLocation (string basePath, string name)
		{
			dataFile = Path.Combine (basePath, name + ".pidb");
			this.basePath = basePath;
		}
		
		public void Rename (string name)
		{
			lock (rwlock)
			{
				Flush ();
				string oldDataFile = dataFile;
				dataFile = Path.Combine (basePath, name + ".pidb");

				CloseReader ();
				
				if (File.Exists (oldDataFile))
					File.Move (oldDataFile, dataFile);
			}
		}
		
		public virtual void Read ()
		{
			if (basePath == null)
				throw new InvalidOperationException ("Location not set");
				
			if (!File.Exists (dataFile)) return;
			
			lock (rwlock)
			{
				FileStream ifile = null;
				try 
				{
					modified = false;
					currentGetTime = 0;
					CloseReader ();
					
					Console.WriteLine ("Reading " + dataFile);
					ifile = new FileStream (dataFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					BinaryFormatter bf = new BinaryFormatter ();
					
					// Read the headers
					headers = (Hashtable) bf.Deserialize (ifile);
					int ver = (int) headers["Version"];
					if (ver != FORMAT_VERSION)
						throw new Exception ("Expected version " + FORMAT_VERSION + ", found version " + ver);
					
					// Move to the index offset and read the index
					BinaryReader br = new BinaryReader (ifile);
					long indexOffset = br.ReadInt64 ();
					ifile.Position = indexOffset;
					
					object[] data = (object[]) bf.Deserialize (ifile);
					ifile.Close ();
					
					references = (ArrayList) data[0];
					rootNamespace = (NamespaceEntry) data[1];
					files = (Hashtable) data[2];
				}
				catch (Exception ex)
				{
					if (ifile != null) ifile.Close ();
					Console.WriteLine ("PIDB file '" + dataFile + "' couldn not be loaded: '" + ex.Message + "'. The file will be recreated");
					rootNamespace = new NamespaceEntry ();
					files = new Hashtable ();
					references = new ArrayList ();
					headers = new Hashtable ();
				}
			}
		}
		
		public static Hashtable ReadHeaders (string baseDir, string name)
		{
			string file = Path.Combine (baseDir, name + ".pidb");
			FileStream ifile = new FileStream (file, FileMode.Open, FileAccess.Read, FileShare.Read);
			BinaryFormatter bf = new BinaryFormatter ();
			Hashtable headers = (Hashtable) bf.Deserialize (ifile);
			ifile.Close ();
			return headers;
		}
		
		public void Write ()
		{
			lock (rwlock)
			{
				if (!modified) return;
				modified = false;
				headers["Version"] = FORMAT_VERSION;
							
				Console.WriteLine ("Writing " + dataFile);
				
				string tmpDataFile = dataFile + ".tmp";
				FileStream dfile = new FileStream (tmpDataFile, FileMode.Create, FileAccess.Write, FileShare.Write);
				
				BinaryFormatter bf = new BinaryFormatter ();
				BinaryWriter bw = new BinaryWriter (dfile);
				
				// The headers are the first thing to write, so they can be read
				// without deserializing the whole file.
				bf.Serialize (dfile, headers);
				
				// The position of the index will be written here
				long indexOffsetPos = dfile.Position;
				bw.Write ((long)0);
				
				// Write all class data
				foreach (FileEntry fe in files.Values) 
				{
					ClassEntry ce = fe.FirstClass;
					while (ce != null)
					{
						IClass c = ce.Class;
						if (c == null)
							c = ReadClass (ce);
							
						ce.Position = dfile.Position;
						PersistentClass.WriteTo (c, bw, parserService.DefaultNameEncoder);
						ce = ce.NextInFile;
					}
				}
				
				// Write the index
				long indexOffset = dfile.Position;
				object[] data = new object[] { references, rootNamespace, files };
				bf.Serialize (dfile, data);
				
				dfile.Position = indexOffsetPos;
				bw.Write (indexOffset);
				
				bw.Close ();
				dfile.Close ();
				
				CloseReader ();
				
				if (File.Exists (dataFile))
					File.Delete (dataFile);
					
				File.Move (tmpDataFile, dataFile);
				
				Console.WriteLine ("Done Writing " + tmpDataFile);
			}
		}
		
		void Flush ()
		{
			int activeCount = 0;
			
			foreach (FileEntry fe in files.Values) {
				ClassEntry ce = fe.FirstClass;
				while (ce != null) { 
					if (ce.Class != null) activeCount++;
					ce = ce.NextInFile;
				}
			}
			
			if (activeCount <= MAX_ACTIVE_COUNT) return;
			
			Write ();
			
			foreach (FileEntry fe in files.Values) {
				ClassEntry ce = fe.FirstClass;
				while (ce != null) { 
					if (ce.LastGetTime < currentGetTime - MIN_ACTIVE_COUNT)
						ce.Class = null;
					ce = ce.NextInFile;
				}
			}
		}
		
		IClass ReadClass (ClassEntry ce)
		{
			if (datareader == null) {
				datafile = new FileStream (dataFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				datareader = new BinaryReader (datafile);
			}
			datafile.Position = ce.Position;
			return PersistentClass.Read (datareader, parserService.DefaultNameDecoder);
		}
		
		void CloseReader ()
		{
			if (datareader != null) {
				datareader.Close ();
				datareader = null;
			}
		}
		
		public IClass GetClass (string typeName, bool caseSensitive)
		{
			lock (rwlock)
			{
				string[] path = typeName.Split ('.');
				int len = path.Length - 1;
				NamespaceEntry nst = GetNamespaceEntry (path, len, false, caseSensitive);
				if (nst == null) return null;
	
				ClassEntry ce = nst.GetClass (path[len], caseSensitive);
				if (ce == null) return null;
				return GetClass (ce);
			}
		}
		
		IClass GetClass (ClassEntry ce)
		{
			ce.LastGetTime = currentGetTime++;
			if (ce.Class != null) return ce.Class;
			
			// Read the class from the file
			
			ce.Class = ReadClass (ce);
			return ce.Class;
		}		
		
		public void CheckModifiedFiles ()
		{
			lock (rwlock)
			{
				foreach (FileEntry file in files.Values)
				{
					if (!File.Exists (file.FileName)) continue;
					FileInfo fi = new FileInfo (file.FileName);
					if (fi.LastWriteTime > file.LastParseTime) 
					{
						// Change date now, to avoid reparsing if CheckModifiedFiles is called again
						// before the parse job is executed
						
						file.LastParseTime = fi.LastWriteTime;
						parserService.QueueParseJob (new WaitCallback (ParseCallback), file.FileName);
					}
				}
			}
		}
		
		void ParseCallback (object ob)
		{
			lock (rwlock)
			{
				ParseFile ((string)ob);
			}
		}
		
		protected virtual void ParseFile (string fileName)
		{
		}
		
		public void ParseAll ()
		{
			lock (rwlock)
			{
				foreach (FileEntry fe in files.Values) 
					ParseFile (fe.FileName);
			}
		}
		
		protected void AddReference (string uri)
		{
			Console.WriteLine ("AddReference " + uri);
			lock (rwlock)
			{
				ReferenceEntry re = new ReferenceEntry (uri);
				references.Add (re);
				modified = true;
			}
		}
		
		protected void RemoveReference (string uri)
		{
			Console.WriteLine ("RemoveReference " + uri);
			lock (rwlock)
			{
				for (int n=0; n<references.Count; n++)
				{
					if (((ReferenceEntry)references[n]).Uri == uri) {
						references.RemoveAt (n);
						modified = true;
						return;
					}
				}
			}
		}
		
		public void AddFile (string fileName)
		{
			lock (rwlock)
			{
				FileEntry fe = new FileEntry (fileName);
				files [fileName] = fe;
				modified = true;
			}
		}
		
		public void RemoveFile (string fileName)
		{
			lock (rwlock)
			{
				FileEntry fe = files [fileName] as FileEntry;
				if (fe == null) return;
				
				ClassEntry ce = fe.FirstClass;
				while (ce != null) {
					ce.NamespaceRef.Remove (ce.Name);
					ce = ce.NextInFile;
				}
				
				files.Remove (fileName);
				modified = true;
			}
		}
		
		protected void AddClass (IClass c, FileEntry fe)
		{
			lock (rwlock)
			{
				string[] path = c.Namespace.Split ('.');
				NamespaceEntry nst = GetNamespaceEntry (path, path.Length, true, true);
				ClassEntry ce = nst.GetClass (c.Name, true);
				if (ce == null) {
					ce = new ClassEntry (c, fe, nst);
					nst.Add (c.Name, ce);
				}
				else {
					ce.Class = CopyClass (c);
				}
				
				if (ce.FileEntry != fe)
					fe.AddClass (ce);
	
				ce.LastGetTime = currentGetTime++;
				modified = true;
			}
		}
			
		public ClassUpdateInformation UpdateClassInformation (ClassCollection newClasses, string fileName)
		{
			lock (rwlock)
			{
				ClassUpdateInformation res = new ClassUpdateInformation ();
				
				FileEntry fe = files [fileName] as FileEntry;
				if (fe == null) return null;
				
				bool[] added = new bool [newClasses.Count];
				NamespaceEntry[] newNss = new NamespaceEntry [newClasses.Count];
				for (int n=0; n<newClasses.Count; n++) {
					string[] path = newClasses[n].Namespace.Split ('.');
					newNss[n] = GetNamespaceEntry (path, path.Length, true, true);
				}
				
				ArrayList newFileClasses = new ArrayList ();
				
				if (fe != null)
				{
					ClassEntry ce = fe.FirstClass;
					while (ce != null)
					{
						IClass newClass = null;
						for (int n=0; n<newClasses.Count && newClass == null; n++) {
							IClass uc = newClasses [n];
							if (uc.Name == ce.Name && newNss[n] == ce.NamespaceRef) {
								newClass = uc;
								added[n] = true;
							}
						}
						
						if (newClass != null) {
							// Class found, replace it
							ce.Class = CopyClass (newClass);
							ce.LastGetTime = currentGetTime++;
							newFileClasses.Add (ce);
							res.Modified.Add (ce.Class);
						}
						else {
							// Class not found, it has to be deleted, unless it has
							// been added in another file
							if (ce.FileEntry == fe) {
								IClass c = ce.Class;
								if (c == null) c = ReadClass (ce);
								res.Removed.Add (c);
								ce.NamespaceRef.Remove (ce.Name);
							}
						}
						ce = ce.NextInFile;
					}
				}
				
				if (fe == null) {
					fe = new FileEntry (fileName);
					files [fileName] = fe;
				}
				
				for (int n=0; n<newClasses.Count; n++) {
					if (!added[n]) {
						IClass c = CopyClass (newClasses[n]);
						ClassEntry ce = new ClassEntry (c, fe, newNss[n]);
						ce.LastGetTime = currentGetTime++;
						newNss[n].Add (c.Name, ce);
						newFileClasses.Add (ce);
						res.Added.Add (c);
					}
				}
				
				fe.SetClasses (newFileClasses);
				rootNamespace.Clean ();
				fe.LastParseTime = DateTime.Now;
				modified = true;
				Flush ();
				
				return res;
			}
		}
		
		public void GetNamespaceContents (ArrayList list, string subNameSpace, bool caseSensitive)
		{
			lock (rwlock)
			{
				string[] path = subNameSpace.Split ('.');
				NamespaceEntry tns = GetNamespaceEntry (path, path.Length, false, caseSensitive);
				if (tns == null) return;
				
				foreach (DictionaryEntry en in tns.Contents) {
					if (en.Value is NamespaceEntry)
						list.Add (en.Key);
					else
						list.Add (GetClass ((ClassEntry)en.Value));
				}
			}
		}
		
		public void GetNamespaceList (ArrayList list, string subNameSpace, bool caseSensitive)
		{
			lock (rwlock)
			{
				string[] path = subNameSpace.Split ('.');
				NamespaceEntry tns = GetNamespaceEntry (path, path.Length, false, caseSensitive);
				if (tns == null) return;
				
				foreach (DictionaryEntry en in tns.Contents) {
					if (en.Value is NamespaceEntry)
						list.Add (en.Key);
				}
			}
		}
		
		public bool NamespaceExists (string name, bool caseSensitive)
		{
			lock (rwlock)
			{
				string[] path = name.Split ('.');
				NamespaceEntry tns = GetNamespaceEntry (path, path.Length, false, caseSensitive);
				return tns != null;
			}
		}
		
		public ICollection References
		{
			get { return references; }
		}
		
		IClass CopyClass (IClass cls)
		{
			MemoryStream ms = new MemoryStream ();
			BinaryWriter bw = new BinaryWriter (ms);
			PersistentClass.WriteTo (cls, bw, parserService.DefaultNameEncoder);
			bw.Flush ();
			ms.Position = 0;
			BinaryReader br = new BinaryReader (ms);
			return PersistentClass.Read (br, parserService.DefaultNameDecoder);
		}
		
		NamespaceEntry GetNamespaceEntry (string[] path, int length, bool createPath, bool caseSensitive)
		{
			NamespaceEntry nst = rootNamespace;

			if (length == 0 || (length == 1 && path[0] == "")) {
				return nst;
			}
			else
			{
				for (int n=0; n<length; n++) {
					NamespaceEntry nh = nst.GetNamespace (path[n], caseSensitive);
					if (nh == null) {
						if (!createPath) return null;
						nh = new NamespaceEntry ();
						nst.Add (path[n], nh);
					}
					nst = nh;
				}
				return nst;
			}
		}
	}
	
	internal class ProjectCodeCompletionDatabase: CodeCompletionDatabase
	{
		IProject project;
		
		public ProjectCodeCompletionDatabase (IProject project, DefaultParserService parserService)
		: base (parserService)
		{
			SetLocation (project.BaseDirectory, project.Name);
			
			this.project = project;
			Read ();
			
			UpdateFromProject ();
		}
		
		public void UpdateFromProject ()
		{
			Hashtable fs = new Hashtable ();
			foreach (ProjectFile file in project.ProjectFiles)
			{
				if (file.BuildAction != BuildAction.Compile) continue;
				FileEntry fe = files[file.Name] as FileEntry;
				if (fe == null) AddFile (file.Name);
				fs [file.Name] = null;
			}
			
			ArrayList keys = new ArrayList ();
			keys.AddRange (files.Keys);
			foreach (string file in keys)
			{
				if (!fs.Contains (file))
					RemoveFile (file);
			}
			
			fs.Clear ();
			foreach (ProjectReference pr in project.ProjectReferences)
			{
				string refId = pr.ReferenceType + ":" + pr.Reference;
				if (pr.ReferenceType == ReferenceType.Gac && refId.ToLower().EndsWith (".dll"))
					refId = refId.Substring (0, refId.Length - 4);

				fs[refId] = null;
				bool found = false;
				for (int n=0; n<references.Count && !found; n++) {
					ReferenceEntry re = (ReferenceEntry) references[n];
					found = ((ReferenceEntry) references[n]).Uri == refId;
				}
				if (!found)
					AddReference (refId);
			}
			
			keys.Clear();
			keys.AddRange (references);
			foreach (ReferenceEntry re in keys)
			{
				if (!fs.Contains (re.Uri))
					RemoveReference (re.Uri);
			}
		}
		
		protected override void ParseFile (string fileName)
		{
			IParseInformation parserInfo = parserService.DoParseFile ((string)fileName, null);
			ICompilationUnit cu = (ICompilationUnit)parserInfo.BestCompilationUnit;
			ClassUpdateInformation res = UpdateClassInformation (cu.Classes, fileName);
			parserService.NotifyParseInfoChange (fileName, res);
		}
	}
	
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
			
			this.assemblyName = assemblyName;
			
			if (assemblyName == "mscorlib") {
				name = assemblyName;
				assemblyFile = typeof(object).Assembly.Location;
			}
			else if (assemblyName.ToLower().EndsWith (".dll")) {
				name = assemblyName.Substring (0, assemblyName.Length - 4);
				name = name.Replace(',','_').Replace(" ","").Replace('/','_');
				assemblyFile = assemblyName;
			}
			else {
				assemblyName = GetAssemblyFullName (assemblyName);
				name = EncodeGacAssemblyName (assemblyName);
				assemblyFile = GetGacAssemblyLocation (assemblyName);
			}
			
			this.baseDir = baseDir;
			SetLocation (baseDir, name);

			Read ();
			
			if (files [assemblyFile] == null) {
				AddFile (assemblyFile);
				headers ["CheckFile"] = assemblyFile;
			}
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
		
		string GetGacAssemblyLocation (string assemblyName)
		{
			Assembly asm;
			try {
				asm = Assembly.Load (assemblyName);
			}
			catch {
				asm = Assembly.LoadWithPartialName (assemblyName);
			}
			
			if (asm == null)
				throw new InvalidOperationException ("Could not find: " + assemblyName);

			return asm.Location;
		}
		
		public string GetAssemblyFullName (string assemblyName)
		{
			if (assemblyName.IndexOf (',') == -1) return assemblyName;
			Assembly asm = Assembly.LoadWithPartialName (assemblyName);
			return asm.GetName().FullName;
		}
		
		protected override void ParseFile (string fileName)
		{
			if (useExternalProcess)
			{
				string dbgen = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "dbgen.exe");
				Console.WriteLine ("Starting " + dbgen);
				Process proc = Process.Start ("mono " + dbgen, "\"" + baseDir + "\" \"" + assemblyName + "\"");
				proc.WaitForExit ();
				Console.WriteLine ("Done " + proc.ExitCode);
				Read ();
			}
			else
			{
				Console.WriteLine ("Parsing assembly: " + fileName);
				AssemblyInformation ainfo = new AssemblyInformation();
				ainfo.Load (fileName, false);
				UpdateClassInformation (ainfo.Classes, fileName);
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
			if (!File.Exists (checkFile)) {
				string dataFile = Path.Combine (baseDir, name + ".pidb");
				File.Delete (dataFile);
				Console.WriteLine ("File not exists: " + checkFile);
				Console.WriteLine ("Deleted " + dataFile);
			}
		}
	}
	

	public interface INameEncoder
	{
		int GetStringId (string text);
	}
	
	public interface INameDecoder
	{
		string GetStringValue (int id);
	}
	
	
	public class StringNameTable: INameEncoder, INameDecoder
	{
		string[] table;
		
		public StringNameTable (string[] names)
		{
			table = names;
			Array.Sort (table);
		}
		
		public string GetStringValue (int id)
		{
			return table [id];
		}
		
		public int GetStringId (string text)
		{
			int i = Array.BinarySearch (table, text);
			if (i >= 0) return i;
			else return -1;
		}
	}
	
	[Serializable]
	class NamespaceEntry
	{
		Hashtable contents = new Hashtable ();
		
		// This is the case insensitive version of the hashtable.
		// It is constructed only when needed.
		[NonSerialized] Hashtable contents_ci;
		
		// All methods with the caseSensitive parameter, first check for an
		// exact match, and if not found, they try with the case insensitive table.
		
		public NamespaceEntry GetNamespace (string ns, bool caseSensitive)
		{
			NamespaceEntry ne = contents[ns] as NamespaceEntry;
			if (ne != null || caseSensitive) return ne;
			
			if (contents_ci == null) BuildCaseInsensitiveTable ();
			return contents_ci[ns] as NamespaceEntry;
		}
		
		public ClassEntry GetClass (string name, bool caseSensitive)
		{
			ClassEntry ne = contents[name] as ClassEntry;
			if (ne != null || caseSensitive) return ne;
			
			if (contents_ci == null) BuildCaseInsensitiveTable ();
			return contents_ci[name] as ClassEntry;
		}
		
		public void Add (string name, object value)
		{
			contents [name] = value;
			if (contents_ci != null)
				contents_ci [name] = value;
		}
		
		public void Remove (string name)
		{
			contents.Remove (name);
			contents_ci = null;
		}
		
		public ICollection Contents
		{
			get { return contents; }
		}
		
		public int ContentCount
		{
			get { return contents.Count; }
		}
		
		public void Clean ()
		{
			ArrayList todel = new ArrayList ();
			foreach (DictionaryEntry en in contents)
			{
				NamespaceEntry h = en.Value as NamespaceEntry;
				if (h != null) {
					h.Clean ();
					if (h.ContentCount == 0) todel.Add (en.Key);
				}
			}
			
			if (todel.Count > 0)
			{
				contents_ci = null;
				foreach (string key in todel)
					contents.Remove (key);
			}
		}
		
		void BuildCaseInsensitiveTable ()
		{
			contents_ci = new Hashtable (CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			foreach (DictionaryEntry en in contents)
				contents_ci.Add (en.Key, en.Value);
		}
	}

	[Serializable]
	class ClassEntry
	{
		long position;
		NamespaceEntry namespaceRef;
		string name;
		FileEntry fileEntry;
		ClassEntry nextInFile;
		
		[NonSerialized]
		int lastGetTime;
		
		[NonSerialized]
		public IClass cls;
		
		public ClassEntry (IClass cls, FileEntry fileEntry, NamespaceEntry namespaceRef)
		{
			this.cls = cls;
			this.fileEntry = fileEntry;
			this.namespaceRef = namespaceRef;
			this.name = cls.Name;
			position = -1;
		}
		
		public long Position
		{
			get { return position; }
			set { position = value; }
		}
		
		public IClass Class
		{
			get { 
				return cls; 
			}
			set {
				cls = value; 
				if (cls != null) {
					name = cls.Name; 
					position = -1; 
				}
			}
		}
		
		public string Name
		{
			get { return name; }
		}
		
		public NamespaceEntry NamespaceRef
		{
			get { return namespaceRef; }
		}
		
		public FileEntry FileEntry
		{
			get { return fileEntry; }
			set { fileEntry = value; }
		}
		
		public int LastGetTime
		{
			get { return lastGetTime; }
			set { lastGetTime = value; }
		}
		
		public ClassEntry NextInFile
		{
			get { return nextInFile; }
			set { nextInFile = value; }
		}
	}
	
	[Serializable]
	class FileEntry
	{
		string filePath;
		DateTime parseTime;
		ClassEntry firstClass;
		
		public FileEntry (string path)
		{
			filePath = path;
			parseTime = DateTime.MinValue;
		}
		
		public string FileName
		{
			get { return filePath; }
		}
		
		public DateTime LastParseTime
		{
			get { return parseTime; }
			set { parseTime = value; }
		}
		
		public ClassEntry FirstClass
		{
			get { return firstClass; }
		}
		
		public void SetClasses (ArrayList list)
		{
			firstClass = null;
			foreach (ClassEntry ce in list)
				AddClass (ce);
		}
		
		public void AddClass (ClassEntry ce)
		{
			if (ce.FileEntry != null)
				ce.FileEntry.RemoveClass (ce);
				
			ce.NextInFile = firstClass;
			firstClass = ce;
		}
		
		public void RemoveClass (ClassEntry ce)
		{
			ClassEntry oldent = null;
			ClassEntry curent = firstClass;
			
			while (curent != null && curent != ce) {
				oldent = curent;
				curent = curent.NextInFile;
			}
			
			if (curent == null) 
				return;
			else if (oldent == null)
				firstClass = curent.NextInFile;
			else
				oldent.NextInFile = curent.NextInFile;
				
			ce.FileEntry = null;
		}
		
		public bool IsAssembly
		{
			get { return filePath.ToLower().EndsWith (".dll"); }
		}
	}
	
	[Serializable]
	class ReferenceEntry
	{
		string databaseUri;
		
		public ReferenceEntry (string dbUri)
		{
			databaseUri = dbUri;
		}
		
		public string Uri
		{
			get { return databaseUri; }
		}
	}
}
