// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.ComponentModel;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Internal.Project
{
	public class Combine : LocalizedObject, IDisposable
	{
		string name        = null;
		string description = null;
		
		/// <summary>
		/// name of the project to startup in singlestartup mode.
		/// </summary>
		string startProject  = null;
		bool   singleStartup = true;
		string path          = null;
		
		ArrayList entries       = new ArrayList();
		
		CombineConfiguration activeConfiguration;
		
		Hashtable configurations            = new Hashtable();
		ArrayList combineExecuteDefinitions = new ArrayList();
		IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Combine.ActiveConfiguration}",
		                   Description = "${res:MonoDevelop.Internal.Project.Combine.ActiveConfiguration.Description}")]
		[TypeConverter(typeof(CombineActiveConfigurationTypeConverter))]
		public CombineConfiguration ActiveConfiguration {
			get {
				return activeConfiguration;
			}
			set {
				activeConfiguration = value;
			}
		}
		
		[Browsable(false)]
		public Hashtable Configurations {
			get {
				return configurations;
			}
		}
		
		[Browsable(false)]
		public ArrayList CombineExecuteDefinitions {
			get {
				return combineExecuteDefinitions;
			}
		}
		[Browsable(false)]
		public ArrayList Entries {
			get {
				return entries;
			}
		}
		
		[Browsable(false)]
		public string SingleStartProjectName {
			get {
				return startProject;
			}
			set {
				startProject = value;
				OnStartupPropertyChanged(null);
			}
		}
		
		[Browsable(false)]
		public bool SingleStartupProject {
			get {
				return singleStartup;
			}
			set {
				singleStartup = value;
				OnStartupPropertyChanged(null);
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Combine.Name}",
		                   Description ="${res:MonoDevelop.Internal.Project.Combine.Name.Description}")]
		public string Name {
			get {
				return name;
			}
			set {
				if (name != value && value != null && value.Length > 0) {
					name = value;
					OnNameChanged(EventArgs.Empty);
				}
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Combine.Description}",
		                   Description ="${res:MonoDevelop.Internal.Project.Combine.Description.Description}")]
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Combine.NeedsBuilding}",
		                   Description ="${res:MonoDevelop.Internal.Project.Combine.NeedsBuilding.Description}")]
		public bool NeedsBuilding {
			get {
				ArrayList projects = new ArrayList();
				GetAllProjects(projects, this);
				foreach (ProjectCombineEntry projectEntry in projects) {
					if (projectEntry.IsDirty) {
						return true;
					}
				}
				return false;
			}
		}
		
		public void Dispose()
		{
			if (entries != null) {
				foreach (object o in entries) {
					if (o is IDisposable) {
						((IDisposable)o).Dispose();
					}
				}
			}
		}
		
		public Combine()
		{
		}
		
		public Combine(string filename)
		{
			LoadCombine(filename);
		}
		
		public IProject LoadProject(string filename)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			ILanguageBinding binding = languageBindingService.GetBindingPerProjectFile(filename);
			if (binding == null) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(String.Format (GettextCatalog.GetString ("Can't find language binding for {0}"), filename));
				return null;
			}
			
			IProject project = binding.CreateProject(null, null);
			project.LoadProject(filename);
			return project;
		}
		
		public void LoadCombine(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			path = Path.GetDirectoryName(filename);
			
			
			XmlElement root = doc.DocumentElement;
			
			name          = root.Attributes["name"].InnerText;
			description   = root.Attributes["description"].InnerText;
			
			startProject   = root["StartMode"].Attributes["startupentry"].InnerText;
			singleStartup  = Boolean.Parse(root["StartMode"].Attributes["single"].InnerText);
			
			XmlNodeList nodes = root["Entries"].ChildNodes;
			entries.Clear();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (XmlElement el in nodes) {
				string abs_path = fileUtilityService.RelativeToAbsolutePath(path, el.Attributes["filename"].InnerText);
				AddEntry(abs_path);
			}
			
			nodes = root["StartMode"].ChildNodes;
			combineExecuteDefinitions.Clear();
			foreach (XmlElement el in nodes)  {
				if (el.Name == "Execute") {
					CombineExecuteDefinition ced = new CombineExecuteDefinition();
					ced.Entry = GetEntry(el.Attributes["entry"].InnerText);
					ced.Type = (EntryExecuteType)Enum.Parse(typeof(EntryExecuteType), el.Attributes["type"].InnerText);
					combineExecuteDefinitions.Add(ced);
				}
			}
			
			nodes = root["Configurations"].ChildNodes;
			configurations.Clear();
			foreach (XmlElement el in nodes) {
				CombineConfiguration cconf = new CombineConfiguration(el, this);
				configurations[cconf.Name] = cconf;
				
				// set the active configuration, either to the first (if the active attribute is not set)
				// or to the active configuration specified by the active attribute.
				if ((doc.DocumentElement["Configurations"].Attributes["active"] == null) || cconf.Name == doc.DocumentElement["Configurations"].Attributes["active"].InnerText) { // ok, I know that && has a higher priority than ||, but many programmers think that a bracket is easier to read ... one thing I don't find easy to read are long lines :)
					ActiveConfiguration = cconf;
				}
			}
		}
		
		public void SaveCombine(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<Combine fileversion=\"1.0\"/>");
			
			XmlAttribute combineNameAttribute = doc.CreateAttribute("name");
			combineNameAttribute.InnerText = name;
			doc.DocumentElement.Attributes.Append(combineNameAttribute);
			
			XmlAttribute combineDescriptionAttribute = doc.CreateAttribute("description");
			combineDescriptionAttribute.InnerText = description;
			doc.DocumentElement.Attributes.Append(combineDescriptionAttribute);
			
			string path = Path.GetDirectoryName(filename);
			
			XmlElement startupnode  = doc.CreateElement("StartMode");
			
			XmlAttribute single = doc.CreateAttribute("startupentry");
			single.InnerText  = startProject;
			startupnode.Attributes.Append(single);
			
			XmlAttribute activeconf = doc.CreateAttribute("single");
			activeconf.InnerText = singleStartup.ToString();
			startupnode.Attributes.Append(activeconf);
			
			foreach (CombineExecuteDefinition ced in combineExecuteDefinitions) {
				XmlElement el = doc.CreateElement("Execute");
				
				XmlAttribute a1 = doc.CreateAttribute("entry");
				CombineEntry centry = ced.Entry;
				if (centry == null || centry.Entry == null) {
					continue;
				}
				if (centry.Entry is IProject) {
					a1.InnerText  = ((IProject)centry.Entry).Name;
				} else {
					a1.InnerText  = ((Combine)centry.Entry).Name;
				}
				el.Attributes.Append(a1);
				
				XmlAttribute a2 = doc.CreateAttribute("type");
				a2.InnerText  = ced.Type.ToString();
				el.Attributes.Append(a2);
				
				startupnode.AppendChild(el);
			}
			
			doc.DocumentElement.AppendChild(startupnode);
						
			XmlElement projectsnode = doc.CreateElement("Entries");
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (CombineEntry entry in entries) {
				XmlElement el = doc.CreateElement("Entry");
	
				XmlAttribute entrynameattr = doc.CreateAttribute("filename");
				entrynameattr.InnerText = fileUtilityService.AbsoluteToRelativePath(path, entry.Filename);
				el.Attributes.Append(entrynameattr);
				
				projectsnode.AppendChild(el);
			}
			doc.DocumentElement.AppendChild(projectsnode);
			
			XmlElement confnode = doc.CreateElement("Configurations");
			
			if (ActiveConfiguration != null) {
				XmlAttribute activeconfattr = doc.CreateAttribute("active");
				activeconfattr.InnerText = ActiveConfiguration.Name;
				confnode.Attributes.Append(activeconfattr);
			}
			foreach (DictionaryEntry dentry in configurations) {
				confnode.AppendChild(((CombineConfiguration)dentry.Value).ToXmlElement(doc));
			}
			doc.DocumentElement.AppendChild(confnode);
			
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save),
			                                filename,
			                                GettextCatalog.GetString ("Can't save solution\nPlease check your file and directory permissions."),
							FileErrorPolicy.ProvideAlternative);
		}
		
		public void SaveCombineAs()
		{
			using (Gtk.FileSelection fdiag = new Gtk.FileSelection (GettextCatalog.GetString ("Save Combine As..."))) {
				fdiag.Filename = System.Environment.GetEnvironmentVariable ("HOME");
				if (fdiag.Run() == (int)Gtk.ResponseType.Ok) {
					string filename = fdiag.Filename;
					SaveCombine(filename);
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowMessage(filename, GettextCatalog.GetString ("Combine saved"));
				}
				
				fdiag.Hide ();
			}
		}

		public object AddEntry(string filename)
		{
			if (Path.GetExtension(filename).ToUpper() == ".PRJX") {
				IProject project = LoadProject(filename);
				ProjectCombineEntry newEntry = new ProjectCombineEntry(project, filename);
				entries.Add(newEntry);
				combineExecuteDefinitions.Add(new CombineExecuteDefinition(newEntry, EntryExecuteType.None));
				if (startProject == null)
					startProject = project.Name;
				
				if (configurations.Count == 0) {
					foreach (IConfiguration pconf in project.Configurations) {
						if (pconf.Name == null)
							continue;
						CombineConfiguration cconf = new CombineConfiguration(pconf.Name, this);
						configurations[pconf.Name] = cconf;
						if (ActiveConfiguration == null)
							ActiveConfiguration = cconf;
					}
				}
				
				foreach (DictionaryEntry entry in configurations) {
					CombineConfiguration conf = (CombineConfiguration)entry.Value;
					conf.AddEntry(project);
				}				
				return project;
			} else {
				Combine combine = new Combine(filename);
				CombineCombineEntry newEntry = new CombineCombineEntry(combine, filename);
				entries.Add(newEntry);
				combineExecuteDefinitions.Add(new CombineExecuteDefinition(newEntry, EntryExecuteType.None));
				if (startProject == null)
					startProject = combine.Name;
				
				if (configurations.Count == 0) {
					foreach (DictionaryEntry dentry in combine.Configurations) {
						CombineConfiguration cconf = ((CombineConfiguration)dentry.Value);
						configurations[cconf.Name] = new CombineConfiguration(cconf.Name, this);
						if (ActiveConfiguration == null)
							ActiveConfiguration = cconf;
					}
				}
				
				foreach (DictionaryEntry entry in configurations) {
					CombineConfiguration conf = (CombineConfiguration)entry.Value;
					conf.AddEntry(combine);
				}
				return combine;
			}
		}
		
		public void SaveAllProjects()
		{
			foreach (CombineEntry entry in entries) {
				entry.Save();
			}
		}
		
		public int GetEntryNumber(string name)
		{
			for (int i = 0; i < entries.Count; ++i) {
				if (((CombineEntry)entries[i]).Name == name) {
					return i;		
				}
			}
			return -1;
		}
		
		public CombineEntry GetEntry(string name)
		{
			for (int i = 0; i < entries.Count; ++i) {
				if (((CombineEntry)entries[i]).Name == name) {
					return (CombineEntry)entries[i];
				}
			}
			return null;
		}
		
		void StartProject(int  nr) 
		{
			CombineEntry entry = (CombineEntry)entries[nr];
			entry.Execute();
		}
		
		void StartProject(string name) 
		{
			int entrynum = GetEntryNumber(name);
			if (entrynum == -1) {
				throw new NoStartupCombineDefinedException();
			}
			StartProject(entrynum);
		}

		public void Debug ()
		{
			CombineEntry entry = (CombineEntry)entries[GetEntryNumber (startProject)];
			entry.Debug ();
		}

		public void Execute()
		{
			if (singleStartup) {
				StartProject(startProject);
			} else {
				foreach (CombineExecuteDefinition ced in combineExecuteDefinitions) {
					if (ced.Type == EntryExecuteType.Execute) {
						StartProject(Entries.IndexOf(ced.Entry));
					}
				}
			}
		}
		
		/// <remarks>
		/// Returns an ArrayList containing all ProjectEntries in this combine and 
		/// undercombines
		/// </remarks>
		public static ArrayList GetAllProjects(Combine combine)
		{
			ArrayList list = new ArrayList();
			GetAllProjects(list, combine);
			return list;
		}
		
		static void GetAllProjects(ArrayList list, Combine combine)
		{
			if (combine != null && combine.Entries != null) {
				foreach (CombineEntry entry in combine.Entries) {
					if (entry is ProjectCombineEntry) {
						list.Add((ProjectCombineEntry)entry);
					} else {
						GetAllProjects(list, ((CombineCombineEntry)entry).Combine);
					}
				}
			} else {
				Console.WriteLine("combine or combine.Entries == null");
			}
		}
		
		public ProjectCombineEntry GetProjectEntryContaining(string fileName) 
		{
			ArrayList projects = new ArrayList();
			GetAllProjects(projects, this);
			foreach (ProjectCombineEntry projectEntry in projects) {
				if (projectEntry.Project.IsFileInProject(fileName)) {
					return projectEntry;
				}
			}
			return null;
		}
		
		ArrayList TopologicalSort(ArrayList allProjects)
		{
			ArrayList sortedEntries = new ArrayList(allProjects.Count);
			bool[]    inserted      = new bool[allProjects.Count];
			bool[]    triedToInsert = new bool[allProjects.Count];
			for (int i = 0; i < allProjects.Count; ++i) {
				inserted[i] = triedToInsert[i] = false;
			}
			for (int i = 0; i < allProjects.Count; ++i) {
				if (!inserted[i]) {
					Insert(i, allProjects, sortedEntries, inserted, triedToInsert);
				}
			}
			return sortedEntries;
		}
		
		void Insert(int index, ArrayList allProjects, ArrayList sortedEntries, bool[] inserted, bool[] triedToInsert)
		{
			if (triedToInsert[index]) {
				throw new CyclicBuildOrderException();
			}
			triedToInsert[index] = true;
			foreach (ProjectReference reference in ((ProjectCombineEntry)allProjects[index]).Project.ProjectReferences) {
				if (reference.ReferenceType == ReferenceType.Project) {
					int j = 0;
					for (; j < allProjects.Count; ++j) {
						if (reference.Reference == ((ProjectCombineEntry)allProjects[j]).Name) {
							if (!inserted[j]) {
								Insert(j, allProjects, sortedEntries, inserted, triedToInsert);
							}
							break;
						}
					}
				}
			}
			sortedEntries.Add(allProjects[index]);
			inserted[index] = true;
		}
		
		public void Build(bool doBuildAll)
		{
			ArrayList allProjects = GetAllProjects(this);
			try {
				allProjects = TopologicalSort(allProjects);
			} catch (CyclicBuildOrderException) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(GettextCatalog.GetString ("Cyclic dependencies can not be built with this version.\nBut we are working on it."));
				return;
			}
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			foreach (ProjectCombineEntry entry in allProjects) {
				entry.Build(doBuildAll);
				if (taskService.Errors > 0) {
					break;
				}
			}
		}

		public void GenerateMakefiles ()
		{
			ArrayList projects = new ArrayList ();
			foreach (CombineEntry entry in entries) {
				if (entry is ProjectCombineEntry) {
					entry.GenerateMakefiles ();
					projects.Add (((ProjectCombineEntry)entry).Project);
				}
				else
					Console.WriteLine ("Dont know how to generate makefiles for " + entry);
			}

			StreamWriter stream = new StreamWriter (Path.Combine (path, "Makefile"));

			stream.Write ("all: ");
			foreach (IProject proj in projects) {
				stream.Write ("Makefile.{0}.all", proj.Name);
			}
			stream.WriteLine ();
			stream.WriteLine ();

			stream.Write ("clean: ");
			foreach (IProject proj in projects) {
				stream.Write ("Makefile.{0}.clean", proj.Name);
			}
			stream.WriteLine ();
			stream.WriteLine ();

			stream.Write ("depcheck: ");
			foreach (IProject proj in projects) {
				stream.Write ("Makefile.{0}.depcheck", proj.Name);
			}
			stream.WriteLine ();
			stream.WriteLine ();

			foreach (IProject proj in projects) {
				stream.WriteLine ("Makefile.{0}.%:", proj.Name);
				stream.WriteLine ("\t@$(MAKE) -f $(subst .$*,,$@) $*");
			}

			stream.Flush ();
			stream.Close ();
		}
		
		protected virtual void OnStartupPropertyChanged(EventArgs e)
		{
			if (StartupPropertyChanged != null) {
				StartupPropertyChanged(this, e);
			}
		}
			
		
		protected virtual void OnNameChanged(EventArgs e)
		{
			if (NameChanged != null) {
				NameChanged(this, e);
			}
		}
		
		public event EventHandler NameChanged;
		public event EventHandler StartupPropertyChanged;
	}
	
	public class CombineActiveConfigurationTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context,Type sourceType)
		{
			return true;
		}
		
		public override  bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,  object value)
		{
			Combine combine = (Combine)context.Instance;
			foreach (IConfiguration configuration in combine.Configurations.Values) {
				if (configuration.Name == value.ToString()) {
					return configuration;
				}
			}
			return null;
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			IConfiguration config = value as IConfiguration;
			Debug.Assert(config != null, String.Format("Tried to convert {0} to IConfiguration", config));
			if (config != null) {
				return config.Name;
			}
			return String.Empty;
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return new TypeConverter.StandardValuesCollection(((Combine)context.Instance).Configurations.Values);
		}
	}
}
