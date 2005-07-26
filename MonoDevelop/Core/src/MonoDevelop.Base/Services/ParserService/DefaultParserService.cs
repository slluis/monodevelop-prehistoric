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
		
		const int MAX_PARSING_CACHE_SIZE = 10;
		const int MAX_SINGLEDB_CACHE_SIZE = 10;
		string CoreDB;

		class ParsingCacheEntry
		{
			   public ParseInformation ParseInformation;
			   public string FileName;
			   public DateTime AccessTime;
		}
		
		class SingleFileCacheEntry
		{
			   public SimpleCodeCompletionDatabase Database;
			   public DateTime AccessTime;
		}
		
		class ParsingJob
		{
			public object Data;
			public JobCallback ParseCallback;
		}

		class CompilationUnitTypeResolver: ITypeResolver
		{
			public IClass CallingClass;
			Project project;
			ICompilationUnit unit;
			DefaultParserService parserService;
			bool allResolved;
			
			public CompilationUnitTypeResolver (Project project, ICompilationUnit unit, DefaultParserService parserService)
			{
				this.project = project;
				this.unit = unit;
				this.parserService = parserService;
			}
			
			public string Resolve (string typeName)
			{
				IClass c = parserService.SearchType (project, typeName, CallingClass, unit);
				if (c != null)
					return c.FullyQualifiedName;
				else {
					allResolved = false;
					return typeName;
				}
			}
			
			public bool AllResolved
			{
				get { return allResolved; }
				set { allResolved = value; }
			}
		}
		
		Hashtable lastUpdateSize = new Hashtable();
		Hashtable parsings = new Hashtable ();
		
		CombineEntryEventHandler combineEntryAddedHandler;
		CombineEntryEventHandler combineEntryRemovedHandler;

		public static Queue parseQueue = new Queue();
		
		string codeCompletionPath;

		Hashtable databases = new Hashtable();
		Hashtable singleDatabases = new Hashtable ();
		
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
			combineEntryAddedHandler = new CombineEntryEventHandler (OnCombineEntryAdded);
			combineEntryRemovedHandler = new CombineEntryEventHandler (OnCombineEntryRemoved);
			nameTable = new StringNameTable (sharedNameTable);
		}
		
		public string LoadAssemblyFromGac (string name) {
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
				Runtime.LoggingService.Info ("Could not find: " + name);
				return string.Empty;
			}
			
			return asm.Location;
		}
		
		private bool ContinueWithProcess(IProgressMonitor progressMonitor)
		{
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();

			if (progressMonitor.IsCancelRequested)
				return false;
			else
				return true;
		}
	
		public void GenerateCodeCompletionDatabase(string createPath, IProgressMonitor progressMonitor)
		{
			if (progressMonitor != null)
				progressMonitor.BeginTask(GettextCatalog.GetString ("Generating database"), assemblyList.Length);

			for (int i = 0; i < assemblyList.Length; ++i)
			{
				try {
					AssemblyCodeCompletionDatabase db = new AssemblyCodeCompletionDatabase (codeCompletionPath, assemblyList[i], this);
					db.ParseAll ();
					db.Write ();
					
					if (progressMonitor != null)
						progressMonitor.Step (1);
						
					if (!ContinueWithProcess (progressMonitor))
						return;
				}
				catch (Exception ex) {
					Runtime.LoggingService.Info (ex);
				}
			}

			if (progressMonitor != null) {
				progressMonitor.Dispose ();
			}
		}
		
		public void GenerateAssemblyDatabase (string baseDir, string name)
		{
			AssemblyCodeCompletionDatabase db = new AssemblyCodeCompletionDatabase (baseDir, name, this);
			db.ParseInExternalProcess = false;
			db.ParseAll ();
			db.Write ();
		}
		
		void SetDefaultCompletionFileLocation()
		{
			PropertyService propertyService = Runtime.Properties;
			string path = propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty).ToString();
			if (path == String.Empty) {
				path = Path.Combine (Runtime.FileUtilityService.GetDirectoryNameWithSeparator(propertyService.ConfigDirectory), "CodeCompletionData");
				propertyService.SetProperty ("SharpDevelop.CodeCompletion.DataDirectory", path);
				propertyService.SaveProperties ();
			}
			if (!Directory.Exists (path))
				Directory.CreateDirectory (path);

			codeCompletionPath = Runtime.FileUtilityService.GetDirectoryNameWithSeparator(path);
		}

		public override void InitializeService()
		{
			parser = (IParser[])(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Parser").BuildChildItems(this)).ToArray(typeof(IParser));
			
			SetDefaultCompletionFileLocation();
			DeleteObsoleteDatabases ();

			string coreName = typeof(object).Assembly.GetName().ToString ();
			CoreDB = "Assembly:" + coreName;
			coreDatabase = new AssemblyCodeCompletionDatabase (codeCompletionPath, coreName, this);
			databases [CoreDB] = coreDatabase;
			
			IProjectService projectService = Runtime.ProjectService;
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpened);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
		}
		
		internal IProgressMonitor GetParseProgressMonitor ()
		{
			if (Runtime.TaskService == null) {
				return new ConsoleProgressMonitor ();
			}
			return Runtime.TaskService.GetBackgroundProgressMonitor ("Code Completion Database Generation", "Icons.16x16.RunProgramIcon");
		}
			
		internal CodeCompletionDatabase GetDatabase (string uri)
		{
			return GetDatabase (null, uri);
		}
		
		internal ProjectCodeCompletionDatabase GetProjectDatabase (Project project)
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
						
						// We may be trying to load an assembly db using a partial name.
						// In this case we get the full name to avoid database conflicts
						file = AssemblyCodeCompletionDatabase.GetFullAssemblyName (file);
						string realUri = "Assembly:" + file;
						db = (CodeCompletionDatabase) databases [realUri];
						if (db != null) {
							databases [uri] = db;
							return db;
						}
						
						AssemblyCodeCompletionDatabase adb;
						db = adb = new AssemblyCodeCompletionDatabase (baseDir, file, this);
						databases [realUri] = adb;
						if (uri != realUri)
							databases [uri] = adb;
						
						// Load referenced databases
						foreach (ReferenceEntry re in db.References)
							GetDatabase (baseDir, re.Uri);
					}
				}
				return db;
			}
		}
		
		internal SimpleCodeCompletionDatabase GetSingleFileDatabase (string file)
		{
			lock (singleDatabases)
			{
				SingleFileCacheEntry entry = singleDatabases [file] as SingleFileCacheEntry;
				if (entry != null) {
					entry.AccessTime = DateTime.Now;
					return entry.Database;
				}
				else 
				{
					if (singleDatabases.Count >= MAX_SINGLEDB_CACHE_SIZE)
					{
						DateTime tim = DateTime.MaxValue;
						string toDelete = null;
						foreach (DictionaryEntry pce in singleDatabases)
						{
							DateTime ptim = ((SingleFileCacheEntry)pce.Value).AccessTime;
							if (ptim < tim) {
								tim = ptim;
								toDelete = pce.Key.ToString();
							}
						}
						singleDatabases.Remove (toDelete);
					}
				
					SimpleCodeCompletionDatabase db = new SimpleCodeCompletionDatabase (file, this);
					entry = new SingleFileCacheEntry ();
					entry.Database = db;
					entry.AccessTime = DateTime.Now;
					singleDatabases [file] = entry;
					return db;
				}
			}
		}
		
		void LoadProjectDatabase (Project project)
		{
			lock (databases)
			{
				string uri = "Project:" + project.Name;
				if (databases.Contains (uri)) return;
				
				ProjectCodeCompletionDatabase db = new ProjectCodeCompletionDatabase (project, this);
				databases [uri] = db;
				
				foreach (ReferenceEntry re in db.References)
					GetDatabase (re.Uri);

				project.NameChanged += new CombineEntryRenamedEventHandler (OnProjectRenamed);
				project.ReferenceAddedToProject += new ProjectReferenceEventHandler (OnProjectReferencesChanged);
				project.ReferenceRemovedFromProject += new ProjectReferenceEventHandler (OnProjectReferencesChanged);
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
					db.Dispose ();
				}
			}
		}
		
		void UnloadProjectDatabase (Project project)
		{
			string uri = "Project:" + project.Name;
			UnloadDatabase (uri);
			project.NameChanged -= new CombineEntryRenamedEventHandler (OnProjectRenamed);
			project.ReferenceAddedToProject -= new ProjectReferenceEventHandler (OnProjectReferencesChanged);
			project.ReferenceRemovedFromProject -= new ProjectReferenceEventHandler (OnProjectReferencesChanged);
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
			CombineEntryCollection projects = combine.GetAllProjects();
			foreach (Project entry in projects) {
				LoadProjectDatabase (entry);
			}
		}
		
		public void UnloadCombineDatabases (Combine combine)
		{
			CombineEntryCollection projects = combine.GetAllProjects();
			foreach (Project entry in projects) {
				UnloadProjectDatabase (entry);
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
		
		void OnProjectRenamed (object sender, CombineEntryRenamedEventArgs args)
		{
			ProjectCodeCompletionDatabase db = GetProjectDatabase ((Project) args.CombineEntry);
			if (db == null) return;
			
			db.Rename (args.NewName);
			databases.Remove ("Project:" + args.OldName);
			databases ["Project:" + args.NewName] = db;
			RefreshProjectDatabases ();
			CleanUnusedDatabases ();
		}
		
		void OnCombineEntryAdded (object sender, CombineEntryEventArgs args)
		{
			if (args.CombineEntry is Project)
				LoadProjectDatabase ((Project)args.CombineEntry);
			else if (args.CombineEntry is Combine)
				LoadCombineDatabases ((Combine)args.CombineEntry);
		}
		
		void OnCombineEntryRemoved (object sender, CombineEntryEventArgs args)
		{
			if (args.CombineEntry is Project)
				UnloadProjectDatabase ((Project) args.CombineEntry);
			else if (args.CombineEntry is Combine)
				UnloadCombineDatabases ((Combine) args.CombineEntry);
			CleanUnusedDatabases ();
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
		
		internal void QueueParseJob (JobCallback callback, object data)
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
			// Check databases following a bottom-up strategy in the dependency
			// tree. This will help resolving parsed classes.
			
			ArrayList list = new ArrayList ();
			lock (databases) 
			{
				// There may be several uris for the same db
				foreach (object ob in databases.Values)
					if (!list.Contains (ob))
						list.Add (ob);
			}
			
			ArrayList done = new ArrayList ();
			while (list.Count > 0) 
			{
				CodeCompletionDatabase readydb = null;
				CodeCompletionDatabase bestdb = null;
				int bestRefCount = int.MaxValue;
				
				// Look for a db with all references resolved
				for (int n=0; n<list.Count && readydb==null; n++)
				{
					CodeCompletionDatabase db = (CodeCompletionDatabase)list[n];

					bool allDone = true;
					foreach (ReferenceEntry re in db.References) {
						CodeCompletionDatabase refdb = GetDatabase (re.Uri);
						if (!done.Contains (refdb)) {
							allDone = false;
							break;
						}
					}
					
					if (allDone)
						readydb = db;
					else if (db.References.Count < bestRefCount) {
						bestdb = db;
						bestRefCount = db.References.Count;
					}
				}

				// It may not find any db without resolved references if there
				// are circular dependencies. In this case, take the one with
				// less references
				
				if (readydb == null)
					readydb = bestdb;
					
				readydb.CheckModifiedFiles ();
				list.Remove (readydb);
				done.Add (readydb);
			}
		}
		
		void ConsumeParsingQueue ()
		{
			int pending = 0;
			IProgressMonitor monitor = null;
			
			try {
				do {
					if (pending > 5 && monitor == null) {
						monitor = GetParseProgressMonitor ();
						monitor.BeginTask ("Generating database", 0);
					}
					
					ParsingJob job = null;
					lock (parseQueue)
					{
						if (parseQueue.Count > 0)
							job = (ParsingJob) parseQueue.Dequeue ();
					}
					
					if (job != null)
						job.ParseCallback (job.Data, monitor);
					
					lock (parseQueue)
						pending = parseQueue.Count;
					
				}
				while (pending > 0);
			} finally {
				if (monitor != null) monitor.Dispose ();
			}
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
				
				if (GetParser (fileName) == null)
					return;
				
				string text = editable.Text;
				if (text == null) return;
					
				IParseInformation parseInformation = null;
				bool updated = false;
			
				if (lastUpdateSize[fileName] == null || (int)lastUpdateSize[fileName] != text.GetHashCode()) {
					parseInformation = DoParseFile(fileName, text);
					if (parseInformation == null) return;
					
					if (viewContent.Project != null) {
						ProjectCodeCompletionDatabase db = GetProjectDatabase (viewContent.Project);
						ClassUpdateInformation res = db.UpdateFromParseInfo (parseInformation, fileName);
						if (res != null) NotifyParseInfoChange (fileName, res, viewContent.Project);
					}
					else {
						SimpleCodeCompletionDatabase db = GetSingleFileDatabase (fileName);
						db.UpdateFromParseInfo (parseInformation);
					}

					lastUpdateSize[fileName] = text.GetHashCode();
					updated = true;
				}
				if (updated && parseInformation != null && editable is IParseInformationListener) {
					((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
				}
			} catch (Exception e) {
				try {
					Runtime.LoggingService.Info(e.ToString());
				} catch {}
			}
		}
		
		CodeCompletionDatabase GetActiveFileDatabase()
		{
			IWorkbenchWindow win = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (win == null || win.ActiveViewContent == null) return null;
			
			IEditable editable = win.ActiveViewContent as IEditable;
			if (editable == null) return null;
			
			string fileName = null;
			
			IViewContent viewContent = win.ViewContent;
			IParseableContent parseableContent = win.ActiveViewContent as IParseableContent;
			
			if (parseableContent != null) {
				fileName = parseableContent.ParseableContentName;
			} else {
				fileName = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.ContentName;
			}
			
			if (fileName == null || fileName.Length == 0) return null;
			return GetSingleFileDatabase (fileName);
		}
		
#region Default Parser Layer dependent functions

		public IClass GetClass (Project project, string typeName)
		{
			return GetClass(project, typeName, false, true);
		}
		
		public IClass GetClass (Project project, string typeName, bool deepSearchReferences, bool caseSensitive)
		{
			if (deepSearchReferences)
				return DeepGetClass (project, typeName, caseSensitive);
			else
				return GetClass (project, typeName, caseSensitive);
		}
		
		public IClass GetClass (Project project, string typeName, bool caseSensitive)
		{
			CodeCompletionDatabase db = project != null ? GetProjectDatabase (project) : GetActiveFileDatabase ();
			if (db != null) {
				IClass c = db.GetClass (typeName, caseSensitive);
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
		
		public IClass DeepGetClass (Project project, string typeName, bool caseSensitive)
		{
			CodeCompletionDatabase db = (project != null) ? GetProjectDatabase (project) : GetActiveFileDatabase ();
			
			ArrayList visited = new ArrayList ();
			IClass c = DeepGetClassRec (visited, db, typeName, caseSensitive);
			if (c != null) return c;

			db = GetDatabase (CoreDB);
			return db.GetClass (typeName, caseSensitive);
		}
		
		internal IClass DeepGetClassRec (ArrayList visitedDbs, CodeCompletionDatabase db, string typeName, bool caseSensitive)
		{
			if (db == null) return null;
			if (visitedDbs.Contains (db)) return null;
			
			visitedDbs.Add (db);
			
			IClass c = db.GetClass (typeName, caseSensitive);
			if (c != null) return c;
			
			foreach (ReferenceEntry re in db.References)
			{
				CodeCompletionDatabase cdb = GetDatabase (re.Uri);
				if (cdb == null) continue;
				c = DeepGetClassRec (visitedDbs, cdb, typeName, caseSensitive);
				if (c != null) return c;
			}
			return null;
		}
		
		public IClass[] GetProjectContents (Project project)
		{
			CodeCompletionDatabase db = GetProjectDatabase (project);
			if (db != null)
				return db.GetClassList ();
			else
				return new IClass[0];
		}
		
		public string[] GetClassList (Project project, string subNameSpace, bool includeReferences)
		{
			return GetClassList (project, subNameSpace, includeReferences, true);
		}
		
		public string[] GetClassList (Project project, string subNameSpace, bool includeReferences, bool caseSensitive)
		{
			ArrayList contents = new ArrayList ();
			
			CodeCompletionDatabase db = (project != null) ? GetProjectDatabase (project) : GetActiveFileDatabase ();
			if (db != null) {
				db.GetClassList (contents, subNameSpace, caseSensitive);
				if (includeReferences) {
					foreach (ReferenceEntry re in db.References) {
						CodeCompletionDatabase cdb = GetDatabase (re.Uri);
						if (cdb == null) continue;
						cdb.GetClassList (contents, subNameSpace, caseSensitive);
					}
				}
			}
			
			if (includeReferences) {
				db = GetDatabase (CoreDB);
				db.GetClassList (contents, subNameSpace, caseSensitive);
			}
			
			return (string[]) contents.ToArray (typeof(string));
		}

		public string[] GetNamespaceList (Project project, string subNameSpace)
		{
			return GetNamespaceList (project, subNameSpace, true, true);
		}
		
		public string[] GetNamespaceList (Project project, string subNameSpace, bool includeReferences, bool caseSensitive)
		{
			ArrayList contents = new ArrayList ();
			
			CodeCompletionDatabase db = (project != null) ? GetProjectDatabase (project) : GetActiveFileDatabase ();
			if (db != null) {
				db.GetNamespaceList (contents, subNameSpace, caseSensitive);
				if (includeReferences) {
					foreach (ReferenceEntry re in db.References) {
						CodeCompletionDatabase cdb = GetDatabase (re.Uri);
						if (cdb == null) continue;
						cdb.GetNamespaceList (contents, subNameSpace, caseSensitive);
					}
				}
			}
			
			if (includeReferences) {
				db = GetDatabase (CoreDB);
				db.GetNamespaceList (contents, subNameSpace, caseSensitive);
			}
			
			return (string[]) contents.ToArray (typeof(string));
		}
		
		public ArrayList GetNamespaceContents (Project project, string namspace, bool includeReferences)
		{
			return GetNamespaceContents (project, namspace, includeReferences, true);
		}
		
		public ArrayList GetNamespaceContents (Project project, string namspace, bool includeReferences, bool caseSensitive)
		{
			ArrayList contents = new ArrayList ();
			
			CodeCompletionDatabase db = (project != null) ? GetProjectDatabase (project) : GetActiveFileDatabase ();
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
		
		public bool NamespaceExists(Project project, string name)
		{
			return NamespaceExists(project, name, true);
		}
		
		public bool NamespaceExists(Project project, string name, bool caseSensitive)
		{
			CodeCompletionDatabase db = (project != null) ? GetProjectDatabase (project) : GetActiveFileDatabase ();
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

		public string SearchNamespace(Project project, IUsing usin, string partitialNamespaceName)
		{
			return SearchNamespace(project, usin, partitialNamespaceName, true);
		}
		
		public string SearchNamespace(Project project, IUsing usin, string partitialNamespaceName, bool caseSensitive)
		{
//			Runtime.LoggingService.Info("SearchNamespace : >{0}<", partitialNamespaceName);
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

		/// <remarks>
		/// use the usings and the name of the namespace to find a class
		/// </remarks>
		public IClass SearchType (Project project, string name, IClass callingClass, ICompilationUnit unit)
		{
			if (name == null || name == String.Empty)
				return null;
				
			IClass c;
			c = GetClass(project, name);
			if (c != null)
				return c;

			if (unit != null) {
				foreach (IUsing u in unit.Usings) {
					if (u != null) {
						c = SearchType(project, u, name);
						if (c != null) {
							return c;
						}
					}
				}
			}
			if (callingClass == null) {
				return null;
			}
			string fullname = callingClass.FullyQualifiedName;
			string[] namespaces = fullname.Split(new char[] {'.'});
			string curnamespace = "";
			int i = 0;
			
			do {
				curnamespace += namespaces[i] + '.';
				c = GetClass(project, curnamespace + name);
				if (c != null) {
					return c;
				}
				i++;
			}
			while (i < namespaces.Length);
			
			return null;
		}
		
		public IClass SearchType(Project project, IUsing iusing, string partitialTypeName)
		{
			return SearchType(project, iusing, partitialTypeName, true);
		}
		
		public IClass SearchType(Project project, IUsing iusing, string partitialTypeName, bool caseSensitive)
		{
//			Runtime.LoggingService.Info("Search type : >{0}<", partitialTypeName);
			IClass c = GetClass(project, partitialTypeName, caseSensitive);
			if (c != null) {
				return c;
			}
			
			foreach (string str in iusing.Usings) {
				string possibleType = String.Concat(str, ".", partitialTypeName);
//				Runtime.LoggingService.Info("looking for " + possibleType);
				c = GetClass(project, possibleType, caseSensitive);
				if (c != null) {
//					Runtime.LoggingService.Info("Found!");
					return c;
				}
			}
			
			// search class in partitial namespaces
			string declaringNamespace = (string)iusing.Aliases[""];
			if (declaringNamespace != null) {
				while (declaringNamespace.Length > 0) {
					string className = String.Concat(declaringNamespace, ".", partitialTypeName);
//					Runtime.LoggingService.Info("looking for " + className);
					c = GetClass(project, className, caseSensitive);
					if (c != null) {
//						Runtime.LoggingService.Info("Found!");
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
//						Runtime.LoggingService.Info("looking for " + className);
						c = GetClass(project, className, caseSensitive);
						if (c != null) {
//							Runtime.LoggingService.Info("Found!");
							return c;
						}
					}
				}
			}
			
			return null;
		}
		
		public bool ResolveTypes (Project project, ICompilationUnit unit, ClassCollection types, out ClassCollection result)
		{
			CompilationUnitTypeResolver tr = new CompilationUnitTypeResolver (project, unit, this);
			
			bool allResolved = true;
			result = new ClassCollection ();
			foreach (IClass c in types) {
				tr.CallingClass = c;
				tr.AllResolved = true;
				result.Add (PersistentClass.Resolve (c, tr));
				allResolved = allResolved && tr.AllResolved;
			}
				
			return allResolved;
		}
		
		public IEnumerable GetClassInheritanceTree (Project project, IClass cls)
		{
			return new ClassInheritanceEnumerator (this, project, cls);
		}
		
		public IClass[] GetFileContents (Project project, string fileName)
		{
			CodeCompletionDatabase db = (project != null) ? GetProjectDatabase (project) : GetActiveFileDatabase ();
			return db.GetFileContents (fileName);
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
			IParser parser = GetParser(fileName);
			
			if (parser == null) {
				return null;
			}
			
			parser.LexerTags = new string[] { "HACK", "TODO", "UNDONE", "FIXME" };
			
			ICompilationUnitBase parserOutput = null;
			
			if (fileContent == null) {
				if (Runtime.ProjectService.CurrentOpenCombine != null) {
					CombineEntryCollection projects = Runtime.ProjectService.CurrentOpenCombine.GetAllProjects ();
					foreach (Project entry in projects) {
						if (entry.IsFileInProject(fileName)) {
							fileContent = entry.GetParseableFileContent(fileName);
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
				if (parsings.Count >= MAX_PARSING_CACHE_SIZE)
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
			// HACK: Still a hack, but extensible
			if (fileName != null) {
				foreach(IParser p in parser){
					if(p.CanParse(fileName)){
						return p;
					}
				}
			}
			return null;
		}
		
		////////////////////////////////////
		
		public ArrayList CtrlSpace(IParserService parserService, Project project, int caretLine, int caretColumn, string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CtrlSpace(parserService, project, caretLine, caretColumn, fileName);
			}
			return null;
		}

		public ArrayList IsAsResolve (Project project, string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			try {
				IParser parser = GetParser (fileName);
				if (parser != null) {
					return parser.IsAsResolve (this, project, expression, caretLineNumber, caretColumn, fileName, fileContent);
				}
				return null;
			} catch {
				return null;
			}
		}
		
		public ResolveResult Resolve(Project project,
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
				//Runtime.LoggingService.Info("Parse info : " + GetParseInformation(fileName).MostRecentCompilationUnit.Tag);
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
		
		public string MonodocResolver (Project project, string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			try {
				IParser parser = GetParser (fileName);
				if (parser != null) {
					return parser.MonodocResolver (this, project, expression, caretLineNumber, caretColumn, fileName, fileContent);
				}
				return null;
			} catch {
				return null;
			}
		}
		
		public void NotifyParseInfoChange (string file, ClassUpdateInformation res, Project project)
		{
			ClassInformationEventArgs args = new ClassInformationEventArgs (file, res, project);
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
	internal class DummyCompilationUnit : AbstractCompilationUnit
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
	
	internal class ClassInheritanceEnumerator : IEnumerator, IEnumerable
	{
		DefaultParserService parserService;
		IClass topLevelClass;
		IClass currentClass  = null;
		Queue  baseTypeQueue = new Queue();
		Project project;

		internal ClassInheritanceEnumerator(DefaultParserService parserService, Project project, IClass topLevelClass)
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
			foreach (string baseTypeName in c.BaseTypes)
				baseTypeQueue.Enqueue(baseTypeName);
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

			IClass baseType = parserService.DeepGetClass (project, baseTypeName, true);
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
	
	public interface ITypeResolver
	{
		string Resolve (string typeName);
	}
	
	public delegate void JobCallback (object data, IProgressMonitor monitor);
}
