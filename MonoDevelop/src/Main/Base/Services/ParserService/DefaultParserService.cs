// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Utility;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui;
using MonoDevelop.Internal.Parser;

namespace MonoDevelop.Services
{
	public class DefaultParserService : AbstractService, IParserService
	{
		CodeCompletionDatabase coreDatabase;
		
		const int MAX_CACHE_SIZE = 10;
		const string CoreDB = "Assembly:mscorlib";

		class ParsingCacheEntry
		{
			   public ParseInformation ParseInformation;
			   public string FileName;
			   public DateTime AccessTime;
		}
		
		class ParsingJob
		{
			public object Data;
			public WaitCallback ParseCallback;
		}

		Hashtable lastUpdateSize = new Hashtable();
		Hashtable parsings = new Hashtable ();
		
		ParseInformation addedParseInformation = new ParseInformation();
		ParseInformation removedParseInformation = new ParseInformation();
		CombineEntryEventHandler combineEntryAddedHandler;
		CombineEntryEventHandler combineEntryRemovedHandler;

		public static Queue parseQueue = new Queue();
		
		string codeCompletionPath;

		Hashtable databases = new Hashtable();
		
		IParser[] parser;
		
		readonly static string[] assemblyList = {
			"Microsoft.VisualBasic",
			"mscorlib",
			"System.Data",
			"System.Design",
			"System.Drawing.Design",
			"System.Drawing",
			"System.Runtime.Remoting",
			"System.Security",
			"System.ServiceProcess",
			"System.Web.Services",
			"System.Web",
			"System",
			"System.Xml",
			"glib-sharp",
			"atk-sharp",
			"pango-sharp",
			"gdk-sharp",
			"gtk-sharp",
			"gnome-sharp",
			"gconf-sharp",
			"gtkhtml-sharp",
			//"System.Windows.Forms",
			//"Microsoft.JScript",
		};
		
		StringNameTable nameTable;
		
		string[] sharedNameTable = new string[] {
			"System.String", "System.Boolean", "System.Int32", "System.Attribute",
			"System.Delegate", "System.Enum", "System.Exception", "System.MarshalByRefObject",
			"System.Object", "SerializableAttribtue", "System.Type", "System.ValueType",
			"System.ICloneable", "System.IDisposable", "System.IConvertible", "System.Byte",
			"System.Char", "System.DateTime", "System.Decimal", "System.Double", "System.Int16",
			"System.Int64", "System.IntPtr", "System.SByte", "System.Single", "System.TimeSpan",
			"System.UInt16", "System.UInt32", "System.UInt64", "System.Void"
		};
		
		public DefaultParserService()
		{
			addedParseInformation.DirtyCompilationUnit = new DummyCompilationUnit();
			removedParseInformation.DirtyCompilationUnit = new DummyCompilationUnit();
			combineEntryAddedHandler = new CombineEntryEventHandler (OnCombineEntryAdded);
			combineEntryRemovedHandler = new CombineEntryEventHandler (OnCombineEntryRemoved);
			nameTable = new StringNameTable (sharedNameTable);
		}
		
		public string LoadAssemblyFromGac (string name) {
			MethodInfo gac_get = typeof (System.Environment).GetMethod ("internalGetGacPath", BindingFlags.Static|BindingFlags.NonPublic);
			
			if (name == "mscorlib")
				return typeof(object).Assembly.Location;
				
			Assembly asm;
			try {
				asm = Assembly.Load (name);
			}
			catch {
				asm = Assembly.LoadWithPartialName (name);
			}
			if (asm == null) {
				Console.WriteLine ("Could not find: " + name);
				return string.Empty;
			}
			
			return asm.Location;
		}
		
		string sys_version;
		string GetSysVersion () {
			if (sys_version != null)
				return sys_version;
			sys_version = typeof (object).Assembly.GetName ().Version.ToString ();
			return sys_version;
		}
		

		private bool ContinueWithProcess(IProgressMonitor progressMonitor)
		{
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();

			if (progressMonitor.Canceled)
				return false;
			else
				return true;
		}
	
		public void GenerateCodeCompletionDatabase(string createPath, IProgressMonitor progressMonitor)
		{
			if (progressMonitor != null)
				progressMonitor.BeginTask(GettextCatalog.GetString ("Generate code completion database"), assemblyList.Length);

			for (int i = 0; i < assemblyList.Length; ++i)
			{
				try {
					AssemblyCodeCompletionDatabase db = new AssemblyCodeCompletionDatabase (codeCompletionPath, assemblyList[i], this);
					db.ParseAll ();
					db.Write ();
					
					if (progressMonitor != null)
						progressMonitor.Worked(i, GettextCatalog.GetString ("Writing class"));
						
					if (!ContinueWithProcess (progressMonitor))
						return;
				}
				catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}

			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}
		
		public void GenerateAssemblyDatabase (string baseDir, string name)
		{
			AssemblyCodeCompletionDatabase db = GetDatabase (baseDir, "Assembly:" + name) as AssemblyCodeCompletionDatabase;
			db.ParseInExternalProcess = false;
			db.ParseAll ();
			db.Write ();
		}
		
		void SetDefaultCompletionFileLocation()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string path = (propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty).ToString());
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			codeCompletionPath = fileUtilityService.GetDirectoryNameWithSeparator(path);
			if (!Directory.Exists (codeCompletionPath)) {
				Directory.CreateDirectory (codeCompletionPath);
			}
		}

		public override void InitializeService()
		{
			parser = (IParser[])(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Parser").BuildChildItems(this)).ToArray(typeof(IParser));
			
			SetDefaultCompletionFileLocation();
			DeleteObsoleteDatabases ();

			coreDatabase = new AssemblyCodeCompletionDatabase (codeCompletionPath, "mscorlib", this);
			databases [CoreDB] = coreDatabase;
			
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpened);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
			projectService.FileRemovedFromProject += new ProjectFileEventHandler (OnProjectFilesChanged);
			projectService.FileAddedToProject += new ProjectFileEventHandler (OnProjectFilesChanged);
			projectService.ReferenceAddedToProject += new ProjectReferenceEventHandler (OnProjectReferencesChanged);
			projectService.ReferenceRemovedFromProject += new ProjectReferenceEventHandler (OnProjectReferencesChanged);
			projectService.ProjectRenamed += new ProjectRenameEventHandler(OnProjectRenamed);
		}
		
		internal CodeCompletionDatabase GetDatabase (string uri)
		{
			return GetDatabase (null, uri);
		}
		
		internal ProjectCodeCompletionDatabase GetProjectDatabase (IProject project)
		{
			if (project == null) return null;
			return (ProjectCodeCompletionDatabase) GetDatabase (null, "Project:" + project.Name);
		}
		
		internal CodeCompletionDatabase GetDatabase (string baseDir, string uri)
		{
			lock (databases)
			{
				if (baseDir == null) baseDir = codeCompletionPath;
				CodeCompletionDatabase db = (CodeCompletionDatabase) databases [uri];
				if (db == null) 
				{
					// Create/load the database
						
					if (uri.StartsWith ("Assembly:"))
					{
						string file = uri.Substring (9);
						db = new AssemblyCodeCompletionDatabase (baseDir, file, this);
					}
					else if (uri.StartsWith ("Gac:"))
					{
						string file = uri.Substring (4);
						db = new AssemblyCodeCompletionDatabase (baseDir, file, this);
					}
					if (db != null)
						databases [uri] = db;
				}
				return db;
			}
		}
		
		void LoadProjectDatabase (IProject project)
		{
			lock (databases)
			{
				string uri = "Project:" + project.Name;
				if (databases.Contains (uri)) return;
				
				ProjectCodeCompletionDatabase db = new ProjectCodeCompletionDatabase (project, this);
				databases [uri] = db;
				
				foreach (ReferenceEntry re in db.References)
				{
					GetDatabase (re.Uri);
				}
			}
		}
		
		void UnloadDatabase (string uri)
		{
			if (uri == CoreDB) return;
			lock (databases)
			{
				CodeCompletionDatabase db = databases [uri] as CodeCompletionDatabase;
				if (db != null) {
					db.Write ();
					databases.Remove (uri);
				}
			}
		}
		
		void UnloadProjectDatabase (IProject project)
		{
			string uri = "Project:" + project.Name;
			UnloadDatabase (uri);
		}
		
		void CleanUnusedDatabases ()
		{
			lock (databases)
			{
				Hashtable references = new Hashtable ();
				foreach (CodeCompletionDatabase db in databases.Values)
				{
					if (db is ProjectCodeCompletionDatabase) {
						foreach (ReferenceEntry re in ((ProjectCodeCompletionDatabase)db).References)
							references [re.Uri] = null;
					}
				}
				
				ArrayList todel = new ArrayList ();
				foreach (DictionaryEntry en in databases)
				{
					if (!(en.Value is ProjectCodeCompletionDatabase) && !references.Contains (en.Key))
						todel.Add (en.Key);
				}
				
				foreach (string uri in todel)
					UnloadDatabase (uri);
			}
		}
		
		public void LoadCombineDatabases (Combine combine)
		{
			ArrayList projects = Combine.GetAllProjects(combine);
			foreach (ProjectCombineEntry entry in projects) {
				LoadProjectDatabase (entry.Project);
			}
		}
		
		public void UnloadCombineDatabases (Combine combine)
		{
			ArrayList projects = Combine.GetAllProjects(combine);
			foreach (ProjectCombineEntry entry in projects) {
				UnloadProjectDatabase (entry.Project);
			}
		}
		
		public void OnCombineOpened(object sender, CombineEventArgs e)
		{
			LoadCombineDatabases (e.Combine);
			e.Combine.EntryAdded += combineEntryAddedHandler;
			e.Combine.EntryRemoved += combineEntryRemovedHandler;
		}
		
		public void OnCombineClosed (object sender, CombineEventArgs e)
		{
			UnloadCombineDatabases (e.Combine);
			CleanUnusedDatabases ();
			e.Combine.EntryAdded -= combineEntryAddedHandler;
			e.Combine.EntryRemoved -= combineEntryRemovedHandler;
		}
		
		void OnProjectRenamed (object sender, ProjectRenameEventArgs args)
		{
			ProjectCodeCompletionDatabase db = GetProjectDatabase (args.Project);
			if (db == null) return;
			
			db.Rename (args.NewName);
			databases.Remove ("Project:" + args.OldName);
			databases ["Project:" + args.NewName] = db;
			RefreshProjectDatabases ();
			CleanUnusedDatabases ();
		}
		
		void OnCombineEntryAdded (object sender, CombineEntryEventArgs args)
		{
			if (args.CombineEntry is ProjectCombineEntry)
				LoadProjectDatabase (((ProjectCombineEntry)args.CombineEntry).Project);
			else if (args.CombineEntry is CombineCombineEntry)
				LoadCombineDatabases (((CombineCombineEntry)args.CombineEntry).Combine);
		}
		
		void OnCombineEntryRemoved (object sender, CombineEntryEventArgs args)
		{
			if (args.CombineEntry is ProjectCombineEntry)
				UnloadProjectDatabase (((ProjectCombineEntry)args.CombineEntry).Project);
			else if (args.CombineEntry is CombineCombineEntry)
				UnloadCombineDatabases (((CombineCombineEntry)args.CombineEntry).Combine);
			CleanUnusedDatabases ();
		}
		
		void OnProjectFilesChanged (object sender, ProjectFileEventArgs args)
		{
			ProjectCodeCompletionDatabase db = GetProjectDatabase (args.Project);
			if (db != null) db.UpdateFromProject ();
		}
		
		void OnProjectReferencesChanged (object sender, ProjectReferenceEventArgs args)
		{
			ProjectCodeCompletionDatabase db = GetProjectDatabase (args.Project);
			if (db != null) {
				db.UpdateFromProject ();
				foreach (ReferenceEntry re in db.References)
				{
					// Make sure the db is loaded
					GetDatabase (re.Uri);
				}
			}
		}
		
		void RefreshProjectDatabases ()
		{
			foreach (CodeCompletionDatabase db in databases.Values)
			{
				ProjectCodeCompletionDatabase pdb = db as ProjectCodeCompletionDatabase;
				if (pdb != null)
					pdb.UpdateFromProject ();
			}
		}
		
		internal void QueueParseJob (WaitCallback callback, object data)
		{
			ParsingJob job = new ParsingJob ();
			job.ParseCallback = callback;
			job.Data = data;
			lock (parseQueue)
			{
				parseQueue.Enqueue (job);
			}
		}
		
		void DeleteObsoleteDatabases ()
		{
			string[] files = Directory.GetFiles (codeCompletionPath, "*.pidb");
			foreach (string file in files)
			{
				string name = Path.GetFileNameWithoutExtension (file);
				string baseDir = Path.GetDirectoryName (file);
				AssemblyCodeCompletionDatabase.CleanDatabase (baseDir, name);
			}
		}
		
		public void StartParserThread()
		{
			Thread t = new Thread(new ThreadStart(ParserUpdateThread));
			t.IsBackground  = true;
			t.Start();
		}
		
		
		void ParserUpdateThread()
		{
			int loop = 0;
			while (true)
			{
				Thread.Sleep(500);
				
				ParseCurrentFile ();
				
				ConsumeParsingQueue ();
				
				if (loop % 10 == 0)
					CheckModifiedFiles ();
				
				loop++;
			}
		}
		
		void CheckModifiedFiles ()
		{
			ArrayList list = new ArrayList ();
			lock (databases) {
				list.AddRange (databases.Values);
			}
			
			foreach (CodeCompletionDatabase db in list) 
				db.CheckModifiedFiles ();
		}
		
		void ConsumeParsingQueue ()
		{
			int pending;
			do {
				ParsingJob job = null;
				lock (parseQueue)
				{
					if (parseQueue.Count > 0)
						job = (ParsingJob) parseQueue.Dequeue ();
				}
				
				if (job != null)
					job.ParseCallback (job.Data);
				
				lock (parseQueue)
					pending = parseQueue.Count;
				
			}
			while (pending > 0);
		}
		
		
		void ParseCurrentFile()
		{
			try {
				IWorkbenchWindow win = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (win == null || win.ActiveViewContent == null) return;
				
				IEditable editable = win.ActiveViewContent as IEditable;
				if (editable == null) return;
				
				string fileName = null;
				
				IViewContent viewContent = win.ViewContent;
				IParseableContent parseableContent = win.ActiveViewContent as IParseableContent;
				
				if (parseableContent != null) {
					fileName = parseableContent.ParseableContentName;
				} else {
					fileName = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.ContentName;
				}
				
				if (fileName == null || fileName.Length == 0) return;
				
				string text = editable.Text;
				if (text == null) return;
					
				IParseInformation parseInformation = null;
				bool updated = false;
				lock (parsings) {
				
					if (lastUpdateSize[fileName] == null || (int)lastUpdateSize[fileName] != text.GetHashCode()) {
						parseInformation = DoParseFile(fileName, text);
						if (parseInformation == null) return;
						
						ProjectCodeCompletionDatabase db = GetProjectDatabase (viewContent.Project);
						if (db != null) {
							ICompilationUnit cu = (ICompilationUnit)parseInformation.BestCompilationUnit;
							ClassUpdateInformation res = db.UpdateClassInformation (cu.Classes, fileName);
							NotifyParseInfoChange (fileName, res);
						}
						lastUpdateSize[fileName] = text.GetHashCode();
						updated = true;
					}
				}
				if (updated && parseInformation != null && editable is IParseInformationListener) {
					((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
				}
			} catch (Exception e) {
				try {
					Console.WriteLine(e.ToString());
				} catch {}
			}
		}
		
		
#region Default Parser Layer dependent functions

		public IClass GetClass (IProject project, string typeName)
		{
			return GetClass(project, typeName, true);
		}
		
		public IClass GetClass (IProject project, string typeName, bool caseSensitive)
		{
			CodeCompletionDatabase db = GetProjectDatabase (project);
			IClass c;
			if (db != null) {
				c = db.GetClass (typeName, caseSensitive);
				if (c != null) return c;
				foreach (ReferenceEntry re in db.References)
				{
					CodeCompletionDatabase cdb = GetDatabase (re.Uri);
					if (cdb == null) continue;
					c = cdb.GetClass (typeName, caseSensitive);
					if (c != null) return c;
				}
			}
			db = GetDatabase (CoreDB);
			return db.GetClass (typeName, caseSensitive);
		}
		
		public string[] GetNamespaceList (IProject project, string subNameSpace)
		{
			return GetNamespaceList (project, subNameSpace, true);
		}
		
		public string[] GetNamespaceList (IProject project, string subNameSpace, bool caseSensitive)
		{
			ArrayList contents = new ArrayList ();
			
			CodeCompletionDatabase db = GetProjectDatabase (project);
			if (db != null) {
				db.GetNamespaceList (contents, subNameSpace, caseSensitive);
				foreach (ReferenceEntry re in db.References)
				{
					CodeCompletionDatabase cdb = GetDatabase (re.Uri);
					if (cdb == null) continue;
					cdb.GetNamespaceList (contents, subNameSpace, caseSensitive);
				}
			}
			
			db = GetDatabase (CoreDB);
			db.GetNamespaceList (contents, subNameSpace, caseSensitive);
			
			return (string[]) contents.ToArray (typeof(string));
		}
		
		public ArrayList GetNamespaceContents (IProject project, string namspace, bool includeReferences)
		{
			return GetNamespaceContents (project, namspace, includeReferences, true);
		}
		
		public ArrayList GetNamespaceContents (IProject project, string namspace, bool includeReferences, bool caseSensitive)
		{
			ArrayList contents = new ArrayList ();
			
			CodeCompletionDatabase db = GetProjectDatabase (project);
			if (db != null) {
				db.GetNamespaceContents (contents, namspace, caseSensitive);
				if (includeReferences) {
					foreach (ReferenceEntry re in db.References)
					{
						CodeCompletionDatabase cdb = GetDatabase (re.Uri);
						if (cdb == null) continue;
						cdb.GetNamespaceContents (contents, namspace, caseSensitive);
					}
				}
			}
			
			if (includeReferences) {
				db = GetDatabase (CoreDB);
				db.GetNamespaceContents (contents, namspace, caseSensitive);
			}
			
			return contents;
		}
		
		public bool NamespaceExists(IProject project, string name)
		{
			return NamespaceExists(project, name, true);
		}
		
		public bool NamespaceExists(IProject project, string name, bool caseSensitive)
		{
			CodeCompletionDatabase db = GetProjectDatabase (project);
			if (db != null) {
				if (db.NamespaceExists (name, caseSensitive)) return true;
				foreach (ReferenceEntry re in db.References)
				{
					CodeCompletionDatabase cdb = GetDatabase (re.Uri);
					if (cdb == null) continue;
					if (cdb.NamespaceExists (name, caseSensitive)) return true;
				}
			}
			
			db = GetDatabase (CoreDB);
			return db.NamespaceExists (name, caseSensitive);
			}

		public string SearchNamespace(IProject project, IUsing usin, string partitialNamespaceName)
		{
			return SearchNamespace(project, usin, partitialNamespaceName, true);
		}
		
		public string SearchNamespace(IProject project, IUsing usin, string partitialNamespaceName, bool caseSensitive)
		{
//			Console.WriteLine("SearchNamespace : >{0}<", partitialNamespaceName);
			if (NamespaceExists(project, partitialNamespaceName, caseSensitive)) {
				return partitialNamespaceName;
			}
			
			// search for partitial namespaces
			string declaringNamespace = (string)usin.Aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					if ((caseSensitive ? declaringNamespace.EndsWith(partitialNamespaceName) : declaringNamespace.ToLower().EndsWith(partitialNamespaceName.ToLower()) ) && NamespaceExists(project, declaringNamespace, caseSensitive)) {
						return declaringNamespace;
					}
					int index = declaringNamespace.IndexOf('.');
					if (index > 0) {
						declaringNamespace = declaringNamespace.Substring(0, index);
					} else {
						break;
					}
				}
			}
			
			// Remember:
			//     Each namespace has an own using object
			//     The namespace name is an alias which has the key ""
			foreach (DictionaryEntry entry in usin.Aliases) {
				string aliasString = entry.Key.ToString();
				if (caseSensitive ? partitialNamespaceName.StartsWith(aliasString) : partitialNamespaceName.ToLower().StartsWith(aliasString.ToLower())) {
					if (aliasString.Length >= 0) {
						string nsName = nsName = String.Concat(entry.Value.ToString(), partitialNamespaceName.Remove(0, aliasString.Length));
						if (NamespaceExists (project, nsName, caseSensitive)) {
							return nsName;
						}
					}
				}
			}
			return null;
		}

		public IClass SearchType(IProject project, IUsing iusing, string partitialTypeName)
		{
			return SearchType(project, iusing, partitialTypeName, true);
		}
		
		public IClass SearchType(IProject project, IUsing iusing, string partitialTypeName, bool caseSensitive)
		{
//			Console.WriteLine("Search type : >{0}<", partitialTypeName);
			IClass c = GetClass(project, partitialTypeName, caseSensitive);
			if (c != null) {
				return c;
			}
			
			foreach (string str in iusing.Usings) {
				string possibleType = String.Concat(str, ".", partitialTypeName);
//				Console.WriteLine("looking for " + possibleType);
				c = GetClass(project, possibleType, caseSensitive);
				if (c != null) {
//					Console.WriteLine("Found!");
					return c;
				}
			}
			
			// search class in partitial namespaces
			string declaringNamespace = (string)iusing.Aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					string className = String.Concat(declaringNamespace, ".", partitialTypeName);
//					Console.WriteLine("looking for " + className);
					c = GetClass(project, className, caseSensitive);
					if (c != null) {
//						Console.WriteLine("Found!");
						return c;
					}
					int index = declaringNamespace.IndexOf('.');
					if (index > 0) {
						declaringNamespace = declaringNamespace.Substring(0, index);
					} else {
						break;
					}
				}
			}
			
			foreach (DictionaryEntry entry in iusing.Aliases) {
				string aliasString = entry.Key.ToString();
				if (caseSensitive ? partitialTypeName.StartsWith(aliasString) : partitialTypeName.ToLower().StartsWith(aliasString.ToLower())) {
					string className = null;
					if (aliasString.Length > 0) {
						className = String.Concat(entry.Value.ToString(), partitialTypeName.Remove(0, aliasString.Length));
//						Console.WriteLine("looking for " + className);
						c = GetClass(project, className, caseSensitive);
						if (c != null) {
//							Console.WriteLine("Found!");
							return c;
						}
					}
				}
			}
			
			return null;
		}
		
		public IEnumerable GetClassInheritanceTree (IProject project, IClass cls)
		{
			return new ClassInheritanceEnumerator (this, project, cls);
		}
		
#endregion
		
		public IParseInformation ParseFile(string fileName)
		{
			return ParseFile(fileName, null);
		}
		
		public IParseInformation ParseFile (string fileName, string fileContent)
		{
			return DoParseFile (fileName, fileContent);
		}
		
		public IParseInformation DoParseFile (string fileName, string fileContent)
		{
			Console.WriteLine ("PARSING " + fileName);
			IParser parser = GetParser(fileName);
			
			if (parser == null) {
				return null;
			}
			
			parser.LexerTags = new string[] { "HACK", "TODO", "UNDONE", "FIXME" };
			
			ICompilationUnitBase parserOutput = null;
			
			if (fileContent == null) {
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				if (projectService.CurrentOpenCombine != null) {
					ArrayList projects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
					foreach (ProjectCombineEntry entry in projects) {
						if (entry.Project.IsFileInProject(fileName)) {
							fileContent = entry.Project.GetParseableFileContent(fileName);
						}
					}
				}
			}
			
			if (fileContent != null) {
				parserOutput = parser.Parse(fileName, fileContent);
			} else {
				parserOutput = parser.Parse(fileName);
			}
			
			ParseInformation parseInformation = GetCachedParseInformation (fileName);
			bool newInfo = false;
			
			if (parseInformation == null) {
				parseInformation = new ParseInformation();
				newInfo = true;
			}
			
			if (parserOutput.ErrorsDuringCompile) {
				parseInformation.DirtyCompilationUnit = parserOutput;
			} else {
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			
			if (newInfo) {
				AddToCache (parseInformation, fileName);
			}
			
			OnParseInformationChanged (new ParseInformationEventArgs (fileName, parseInformation));
			return parseInformation;
		}
		
		ParseInformation GetCachedParseInformation (string fileName)
		{
			lock (parsings) 
			{
				ParsingCacheEntry en = parsings [fileName] as ParsingCacheEntry;
				if (en != null) {
					en.AccessTime = DateTime.Now;
					return en.ParseInformation;
				}
				else
					return null;
			}
		}
		
		void AddToCache (ParseInformation info, string fileName)
		{
			lock (parsings) 
			{
				if (parsings.Count >= MAX_CACHE_SIZE)
				{
					DateTime tim = DateTime.MaxValue;
					string toDelete = null;
					foreach (DictionaryEntry pce in parsings)
					{
						DateTime ptim = ((ParsingCacheEntry)pce.Value).AccessTime;
						if (ptim < tim) {
							tim = ptim;
							toDelete = pce.Key.ToString();
						}
					}
					parsings.Remove (toDelete);
				}
				
				ParsingCacheEntry en = new ParsingCacheEntry();
				en.ParseInformation = info;
				en.AccessTime = DateTime.Now;
				parsings [fileName] = en;
			}
		}

		public IParseInformation GetParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			
			IParseInformation info = GetCachedParseInformation (fileName);
			if (info != null) return info;
			else return ParseFile(fileName);
		}
		
		public IExpressionFinder GetExpressionFinder(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.ExpressionFinder;
			}
			return null;
		}
		public virtual IParser GetParser(string fileName)
		{
			// HACK: I'm too lazy to do it 'right'
			if (fileName != null) {
				if (Path.GetExtension(fileName).ToUpper() == ".CS") {
					return parser[0];
				}
				if (Path.GetExtension(fileName).ToUpper() == ".VB") {
					return parser[1];
				}
			}
			return null;
		}
		
		////////////////////////////////////
		
		public ArrayList CtrlSpace(IParserService parserService, IProject project, int caretLine, int caretColumn, string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CtrlSpace(parserService, project, caretLine, caretColumn, fileName);
			}
			return null;
		}

		public ArrayList IsAsResolve (string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			try {
				IParser parser = GetParser (fileName);
				if (parser != null) {
					return parser.IsAsResolve (this, expression, caretLineNumber, caretColumn, fileName, fileContent);
				}
				return null;
			} catch {
				return null;
			}
		}
		
		public ResolveResult Resolve(IProject project,
									 string expression, 
		                             int caretLineNumber,
		                             int caretColumn,
		                             string fileName,
		                             string fileContent)
		{
			// added exception handling here to prevent silly parser exceptions from
			// being thrown and corrupting the textarea control
			try {
				IParser parser = GetParser(fileName);
				//Console.WriteLine("Parse info : " + GetParseInformation(fileName).MostRecentCompilationUnit.Tag);
				if (parser != null) {
					return parser.Resolve(this, project, expression, caretLineNumber, caretColumn, fileName, fileContent);
				}
				return null;
			} catch {
				return null;
			}
		}
		
		internal INameEncoder DefaultNameEncoder {
			get { return nameTable; }
		}

		internal INameDecoder DefaultNameDecoder {
			get { return nameTable; }
		}
		
		public string MonodocResolver (string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			try {
				IParser parser = GetParser (fileName);
				if (parser != null) {
					return parser.MonodocResolver (this, expression, caretLineNumber, caretColumn, fileName, fileContent);
				}
				return null;
			} catch {
				return null;
			}
		}
		
		public void NotifyParseInfoChange (string file, ClassUpdateInformation res)
		{
			ClassInformationEventArgs args = new ClassInformationEventArgs (file, res);
			OnClassInformationChanged (args);
		}

		protected virtual void OnParseInformationChanged(ParseInformationEventArgs e)
		{
			if (ParseInformationChanged != null) {
				ParseInformationChanged(this, e);
			}
		}
		
		protected virtual void OnClassInformationChanged(ClassInformationEventArgs e)
		{
			if (ClassInformationChanged != null) {
				ClassInformationChanged(this, e);
			}
		}
		
		public event ParseInformationEventHandler ParseInformationChanged;
		public event ClassInformationEventHandler ClassInformationChanged;
	}
	
	[Serializable]
	public class DummyCompilationUnit : AbstractCompilationUnit
	{
		CommentCollection miscComments = new CommentCollection();
		CommentCollection dokuComments = new CommentCollection();
		TagCollection     tagComments  = new TagCollection();
		
		public override CommentCollection MiscComments {
			get {
				return miscComments;
			}
		}
		
		public override CommentCollection DokuComments {
			get {
				return dokuComments;
			}
		}
		
		public override TagCollection TagComments {
			get {
				return tagComments;
			}
		}
	}
	
	public class ClassInheritanceEnumerator : IEnumerator, IEnumerable
	{
		IParserService parserService;
		IClass topLevelClass;
		IClass currentClass  = null;
		Queue  baseTypeQueue = new Queue();
		IProject project;

		public ClassInheritanceEnumerator(IParserService parserService, IProject project, IClass topLevelClass)
		{
			this.parserService = parserService;
			this.project = project;
			this.topLevelClass = topLevelClass;
			baseTypeQueue.Enqueue(topLevelClass.FullyQualifiedName);
			PutBaseClassesOnStack(topLevelClass);
			baseTypeQueue.Enqueue("System.Object");
		}
		public IEnumerator GetEnumerator()
		{
			return this;
		}

		void PutBaseClassesOnStack(IClass c)
		{
			foreach (string baseTypeName in c.BaseTypes) {
				baseTypeQueue.Enqueue(baseTypeName);
			}
		}

		public IClass Current {
			get {
				return currentClass;
			}
		}

		object IEnumerator.Current {
			get {
				return currentClass;
			}
		}

		public bool MoveNext()
		{
			if (baseTypeQueue.Count == 0) {
				return false;
			}
			string baseTypeName = baseTypeQueue.Dequeue().ToString();

			IClass baseType = parserService.GetClass(project, baseTypeName);
			if (baseType == null) {
				ICompilationUnit unit = currentClass == null ? null : currentClass.CompilationUnit;
				if (unit != null) {
					foreach (IUsing u in unit.Usings) {
						baseType = parserService.SearchType(project, u, baseTypeName);
						if (baseType != null) {
							break;
						}
					}
				}
			}

			if (baseType != null) {
				currentClass = baseType;
				PutBaseClassesOnStack(currentClass);
			}

			return baseType != null;
		}

		public void Reset()
		{
			baseTypeQueue.Clear();
			baseTypeQueue.Enqueue(topLevelClass.FullyQualifiedName);
			PutBaseClassesOnStack(topLevelClass);
			baseTypeQueue.Enqueue("System.Object");
		}
	}	
	
	public class ClassUpdateInformation
	{
		ClassCollection added = new ClassCollection ();
		ClassCollection removed = new ClassCollection ();
		ClassCollection modified = new ClassCollection ();
		
		public ClassCollection Added
		{
			get { return added; }
		}
		
		public ClassCollection Removed
		{
			get { return removed; }
		}
		
		public ClassCollection Modified
		{
			get { return modified; }
		}
	}
}
