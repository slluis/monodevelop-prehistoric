// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Internal.Project.Collections;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Components;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Internal.Project
{
	/// <summary>
	/// External language bindings must extend this class
	/// </summary>
	[XmlNodeName("Project")]
	public abstract class AbstractProject : LocalizedObject, IProject
	{
		readonly static string currentProjectFileVersion = "1.1";
		readonly static string configurationNodeName     = "Configuration";
		
		protected string basedirectory   = String.Empty;

		[XmlAttribute("name")]
		protected string projectname     = "New Project";

		[XmlAttribute("description")]
		protected string description     = "";

		[XmlAttribute("newfilesearch")]
		protected NewFileSearch newFileSearch  = NewFileSearch.None;

		[XmlAttribute("enableviewstate")]
		protected bool          enableViewState = true;

		[XmlSetAttribute(typeof(ProjectFile),      "Contents")]
		protected ProjectFileCollection      projectFiles       = new ProjectFileCollection();

		[XmlSetAttribute(typeof(ProjectReference), "References")]
		protected ProjectReferenceCollection projectReferences = new ProjectReferenceCollection();
		
		protected DeployInformation deployInformation = new DeployInformation();
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
		
		[Browsable(false)]
		public string BaseDirectory {
			get {
				return basedirectory;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Project.Name}",
		                   Description ="${res:MonoDevelop.Internal.Project.Project.Description}")]
		public string Name {
			get {
				return projectname;
			}
			set {
				if (projectname != value && value != null && value.Length > 0) {
					IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
					if (!projectService.ExistsEntryWithName(value)) {
						string oldName = projectname;
						projectname = value;
						projectService.OnRenameProject(new ProjectRenameEventArgs(this, oldName, value));
						OnNameChanged(EventArgs.Empty);
					}
				}
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectClass.Description}",
		                   Description = "${res:MonoDevelop.Internal.Project.ProjectClass.Description.Description}")]
		[DefaultValue("")]
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		[Browsable(false)]
		public ProjectFileCollection ProjectFiles {
			get {
				return projectFiles;
			}
		}
		
		[Browsable(false)]
		public ProjectReferenceCollection ProjectReferences {
			get {
				return projectReferences;
			}
		}
		
		protected ArrayList configurations = new ArrayList();
		protected IConfiguration activeConfiguration = null;
		
		[Browsable(false)]
		public ArrayList Configurations {
			get {
				return configurations;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Project.ActiveConfiguration}",
		                   Description = "${res:MonoDevelop.Internal.Project.Project.ActiveConfiguration.Description}")]
		[TypeConverter(typeof(ProjectActiveConfigurationTypeConverter))]
		public IConfiguration ActiveConfiguration {
			get {
				if (activeConfiguration == null && configurations.Count > 0) {
					return (IConfiguration)configurations[0];
				}
				return activeConfiguration;
			}
			set {
				activeConfiguration = value;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Project.NewFileSearch}",
		                   Description = "${res:MonoDevelop.Internal.Project.Project.NewFileSearch.Description}")]
		[DefaultValue(NewFileSearch.None)]
		public NewFileSearch NewFileSearch {
			get {
				return newFileSearch;
			}

			set {
				newFileSearch = value;
			}
		}
		
		[Browsable(false)]
		public bool EnableViewState {
			get {
				return enableViewState;
			}
			set {
				enableViewState = value;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.Project.ProjectType}",
		                   Description = "${res:MonoDevelop.Internal.Project.Project.ProjectType.Description}")]
		public abstract string ProjectType {
			get;
		}
		
		[Browsable(false)]
		public DeployInformation DeployInformation {
			get {
				return deployInformation;
			}
		}

		public AbstractProject()
		{
			projectFiles.SetProject (this);
			projectReferences.SetProject (this);
		}

		public bool IsFileInProject(string filename)
		{
			if (filename == null) return false;
			foreach (ProjectFile file in projectFiles) {
				if (file.Name == filename) {
					return true;
				}
			}
			return false;
		}

		public bool IsCompileable(string fileName)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
			return languageBindingService.GetBindingPerLanguageName(ProjectType).CanCompile(fileName);
		}
		
		public void SearchNewFiles()
		{
			if (newFileSearch == NewFileSearch.None) {
				return;
			}

			StringCollection newFiles   = new StringCollection();
			StringCollection collection = fileUtilityService.SearchDirectory(basedirectory, "*");

			foreach (string sfile in collection) {
				string extension = Path.GetExtension(sfile).ToUpper();
				string file = Path.GetFileName (sfile);

				if (!IsFileInProject(sfile) &&
					extension != ".SCC" &&  // source safe control files -- Svante Lidmans
					extension != ".DLL" &&
					extension != ".PDB" &&
					extension != ".EXE" &&
					extension != ".CMBX" &&
					extension != ".PRJX" &&
					extension != ".SWP" &&
					extension != ".MDSX" &&
					extension != ".PIDB" &&
					!file.EndsWith ("make.sh") &&
					!file.EndsWith ("~") &&
					!file.StartsWith (".") &&
					!(Path.GetDirectoryName(sfile).IndexOf("CVS") != -1) &&
					!(Path.GetDirectoryName(sfile).IndexOf(".svn") != -1) &&
					!file.StartsWith ("Makefile") &&
					!Path.GetDirectoryName(file).EndsWith("ProjectDocumentation")) {

					newFiles.Add(sfile);
				}
			}
			
			if (newFiles.Count > 0) {
				if (newFileSearch == NewFileSearch.OnLoadAutoInsert) {
					foreach (string file in newFiles) {
						ProjectFile newFile = new ProjectFile(file);
						newFile.BuildAction = IsCompileable(file) ? BuildAction.Compile : BuildAction.Nothing;
						projectFiles.Add(newFile);
					}
				} else {
					new IncludeFilesDialog(this, newFiles).ShowDialog();
				}
			}
		}
		
		public virtual void LoadProject(string fileName)
		{
			basedirectory = Path.GetDirectoryName(fileName);
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			
			string version = null;
			if (doc.DocumentElement.Attributes["version"] == null) {
				if (doc.DocumentElement.Attributes["fileversion"] != null) {
					version = doc.DocumentElement.Attributes["fileversion"].InnerText;
				}
			} else {
				version = doc.DocumentElement.Attributes["version"].InnerText;
			}
			
			if (version != "1.0" && version != currentProjectFileVersion) {
				throw new UnknownProjectVersionException(version);
			}
			if (version == "1.0") {
				string tempFile = Path.GetTempFileName();
				IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
				messageService.ShowMessage(String.Format (GettextCatalog.GetString ("Old project file format found.\n It will be automatically converted to {0} information"), currentProjectFileVersion));
				PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
				
				ConvertXml.Convert(fileName,
				                   propertyService.DataDirectory + Path.DirectorySeparatorChar +
				                   "ConversionStyleSheets" + Path.DirectorySeparatorChar +
				                   "ConvertPrjx10to11.xsl",
				                   tempFile);
				try {
					File.Delete(fileName);
					File.Copy(tempFile, fileName);
					LoadProject(fileName);
					File.Delete(tempFile);
					return;
				} catch (Exception) {
					messageService.ShowError(GettextCatalog.GetString ("Error writing the old project file.\nCheck if you have write permission on the project file (.prjx).\n A non persistent proxy project will be created but no changes will be saved.\nIt is better if you close MonoDevelop and correct the problem."));
					if (File.Exists(tempFile)) {
						doc.Load(tempFile);
						File.Delete(tempFile);
					} else {
						messageService.ShowError("damn! (should never happen)");
					}
				}
			}
			
			GetXmlAttributes(doc, doc.DocumentElement, this);
			
			// add the configurations
			XmlNode configurationElement = doc.DocumentElement.SelectSingleNode("Configurations");
			
			string activeConfigurationName = configurationElement.Attributes["active"].InnerText;
			
			foreach (XmlNode configuration in configurationElement.ChildNodes) {
				if (configuration.Name == configurationNodeName) {
					IConfiguration newConfiguration = CreateConfiguration();
					GetXmlAttributes(doc, (XmlElement)configuration, newConfiguration);
					if (newConfiguration.Name == activeConfigurationName) {
						activeConfiguration = newConfiguration;
					}
					Configurations.Add(newConfiguration);
				}
			}
			
			SearchNewFiles();

			projectFiles.SetProject (this);
			projectReferences.SetProject (this);
		}

		void GetXmlAttributes(XmlDocument doc, XmlElement element, object o)
		{
			FieldInfo[] fieldInfos = o.GetType().GetFields(BindingFlags.FlattenHierarchy |
			                                               BindingFlags.Public           |
			                                               BindingFlags.Instance         |
			                                               BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fieldInfos) {
				// set the xml attributes for this object
				XmlAttributeAttribute[]           xmlAttributes = (XmlAttributeAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), true);
				ConvertToRelativePathAttribute[]  convertToRelPath = (ConvertToRelativePathAttribute[])fieldInfo.GetCustomAttributes(typeof(ConvertToRelativePathAttribute), true);
				bool convertRel = convertToRelPath != null && convertToRelPath.Length > 0;
				
				if (xmlAttributes != null && xmlAttributes.Length > 0) {
					if (xmlAttributes[0].Name == null) continue;
					XmlAttribute xmlAttribute = element.Attributes[xmlAttributes[0].Name];
					if (xmlAttribute != null) {
						if (convertRel && convertToRelPath[0].PredicatePropertyName != null && convertToRelPath[0].PredicatePropertyName.Length > 0) {
							PropertyInfo myPropInfo = o.GetType().GetProperty(convertToRelPath[0].PredicatePropertyName, 
							                                                          BindingFlags.Public |
							                                                          BindingFlags.NonPublic |
							                                                          BindingFlags.Instance);
							if (myPropInfo != null) {
								convertRel = (bool)myPropInfo.GetValue(o, null);
							}
						}
						
						string val = null;
						if (convertRel) {
							if (xmlAttribute.InnerText.Length == 0) {
								val = String.Empty;
							} else {
								val = fileUtilityService.RelativeToAbsolutePath(basedirectory, xmlAttribute.InnerText);
							}
						} else {
							val = xmlAttribute.InnerText;
						}
						
						if (fieldInfo.FieldType.IsEnum) {
							fieldInfo.SetValue(o, Enum.Parse(fieldInfo.FieldType,val));
						} else {
							fieldInfo.SetValue(o, Convert.ChangeType(val, fieldInfo.FieldType));
						}
					}
				} else { // add sets to the xmlElement
					XmlSetAttribute[] xmlSetAttributes = (XmlSetAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlSetAttribute), true);
					if (xmlSetAttributes != null && xmlSetAttributes.Length > 0) {
						XmlElement setElement;
						if (xmlSetAttributes[0].Name == null) {
							setElement = element;
						} else {
							setElement = (XmlElement)element.SelectSingleNode("descendant::" + xmlSetAttributes[0].Name);
						}
						
						if (setElement != null) {
							IList collection = (IList)fieldInfo.GetValue(o);
							foreach (XmlNode childNode in setElement.ChildNodes) {
								object instance = xmlSetAttributes[0].Type.Assembly.CreateInstance(xmlSetAttributes[0].Type.FullName);
								GetXmlAttributes(doc, (XmlElement)childNode, instance);
								collection.Add(instance);
							}
						}
					} else { // finally try, if the field is from a type which has a XmlNodeName attribute attached
						
						XmlNodeNameAttribute[] xmlNodeNames = (XmlNodeNameAttribute[])fieldInfo.FieldType.GetCustomAttributes(typeof(XmlNodeNameAttribute), true);
						
						if (xmlNodeNames != null && xmlNodeNames.Length == 1) {
							XmlElement el = (XmlElement)element.SelectSingleNode("descendant::" + xmlNodeNames[0].Name);
							object instance = fieldInfo.FieldType.Assembly.CreateInstance(fieldInfo.FieldType.FullName);
							if (el != null) {
								GetXmlAttributes(doc, el, instance);
							}
							fieldInfo.SetValue(o, instance);
						}
					}
				}
			}
		}
		
		void SetXmlAttributes(XmlDocument doc, XmlElement element, object o)
		{
			FieldInfo[] fieldInfos = o.GetType().GetFields(BindingFlags.FlattenHierarchy |
			                                               BindingFlags.Public           |
			                                               BindingFlags.Instance         |
			                                               BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fieldInfos) {
				// set the xml attributes for this object
				XmlAttributeAttribute[] xmlAttributes = (XmlAttributeAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), true);
				
				ConvertToRelativePathAttribute[]  convertToRelPath = (ConvertToRelativePathAttribute[])fieldInfo.GetCustomAttributes(typeof(ConvertToRelativePathAttribute), true);
				bool convertRel = convertToRelPath != null && convertToRelPath.Length > 0;
								
				if (xmlAttributes != null && xmlAttributes.Length > 0) {
					if (xmlAttributes[0].Name == null) continue;
					XmlAttribute xmlAttribute = doc.CreateAttribute(xmlAttributes[0].Name);
					object fieldValue = fieldInfo.GetValue(o);
					
					if (convertRel && convertToRelPath[0].PredicatePropertyName != null && convertToRelPath[0].PredicatePropertyName.Length > 0) {
						PropertyInfo myPropInfo = o.GetType().GetProperty(convertToRelPath[0].PredicatePropertyName,
						                                                          BindingFlags.Public |
						                                                          BindingFlags.NonPublic |
						                                                          BindingFlags.Instance);
						if (myPropInfo != null) {
							convertRel = (bool)myPropInfo.GetValue(o, null);
						}
					}
					
					if (convertRel) {
						string val = fieldValue == null ? String.Empty : fieldValue.ToString();
						if (val.Length == 0) {
							fieldValue = String.Empty;
						} else {
							fieldValue = fileUtilityService.AbsoluteToRelativePath(basedirectory, val);
						}
					}
					xmlAttribute.InnerText = fieldValue == null ? String.Empty : fieldValue.ToString();
					element.Attributes.Append(xmlAttribute);
				} else { // add sets to the xmlElement
					XmlSetAttribute[] xmlSetAttributes = (XmlSetAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlSetAttribute), true);
					if (xmlSetAttributes != null && xmlSetAttributes.Length > 0) {
						XmlElement setElement;
						if (xmlSetAttributes[0].Name == null) {
							setElement = element;
						} else {
							setElement = doc.CreateElement(xmlSetAttributes[0].Name);
						}

						// A set must always be a collection
						ICollection collection = (ICollection)fieldInfo.GetValue(o);
						foreach (object collectionObject in collection) {
							XmlNodeNameAttribute[] xmlNodeNames = (XmlNodeNameAttribute[])collectionObject.GetType().GetCustomAttributes(typeof(XmlNodeNameAttribute), true);
							if (xmlNodeNames == null || xmlNodeNames.Length != 1) {
								throw new Exception("XmlNodeNames mismatch");
							}
							XmlElement collectionElement = doc.CreateElement(xmlNodeNames[0].Name);
							SetXmlAttributes(doc, collectionElement, collectionObject);
							setElement.AppendChild(collectionElement);
						}
						if (element != setElement) {
							element.AppendChild(setElement);
						}
					} else { // finally try, if the field is from a type which has a XmlNodeName attribute attached
						object fieldValue = fieldInfo.GetValue(o);
						if (fieldValue != null) {
							XmlNodeNameAttribute[] xmlNodeNames = (XmlNodeNameAttribute[])fieldValue.GetType().GetCustomAttributes(typeof(XmlNodeNameAttribute), true);

							if (xmlNodeNames != null && xmlNodeNames.Length == 1) {
								XmlElement setElement = doc.CreateElement(xmlNodeNames[0].Name);
								SetXmlAttributes(doc, setElement, fieldValue);
								element.AppendChild(setElement);
							}
						}
					}
				}
			}
		}
		
		public virtual void SaveProject(string fileName)
		{
			fileName = System.IO.Path.GetFullPath (fileName);
			basedirectory = Path.GetDirectoryName(fileName);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<Project/>");

			SetXmlAttributes(doc, doc.DocumentElement, this);
			
			// set version attribute to the root node
			XmlAttribute versionAttribute = doc.CreateAttribute("version");
			versionAttribute.InnerText    = currentProjectFileVersion;
			doc.DocumentElement.Attributes.Append(versionAttribute);
			
			
			// set projecttype attribute to the root node
			XmlAttribute projecttypeAttribute = doc.CreateAttribute("projecttype");
			projecttypeAttribute.InnerText    = ProjectType;
			doc.DocumentElement.Attributes.Append(projecttypeAttribute);
			
			// create the configuration nodes
			// I choosed to add the configuration nodes 'per hand' instead of using the automated
			// version, because it is more cleaner for the language binding implementors to just 
			// creating a factory method for their configurations.
			XmlElement configurationElement = doc.CreateElement("Configurations");
			XmlAttribute activeConfigAttribute = doc.CreateAttribute("active");
			activeConfigAttribute.InnerText = ActiveConfiguration == null ? String.Empty : ActiveConfiguration.Name;
			configurationElement.Attributes.Append(activeConfigAttribute);
			
			foreach (IConfiguration configuration in Configurations) {
				XmlElement newConfig = doc.CreateElement(configurationNodeName);
				SetXmlAttributes(doc, newConfig, configuration);
				configurationElement.AppendChild(newConfig);
			}
			
			doc.DocumentElement.AppendChild(configurationElement);
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), 
			                                fileName, 
                                                        GettextCatalog.GetString ("Can't save solution\nPlease check your file and directory permissions."), 
							FileErrorPolicy.ProvideAlternative);
		}
		
		public virtual string GetParseableFileContent(string fileName)
		{
			fileName = fileName.Replace('\\', '/'); // FIXME PEDRO
			StreamReader sr = File.OpenText(fileName);
			string content = sr.ReadToEnd();
			sr.Close();
			return content;
		}
		
		public void SaveProjectAs()
		{
			using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Save Project As..."))) {
				//fdiag.Filename = System.Environment.GetEnvironmentVariable ("HOME");

				if (fdiag.Run() == (int)Gtk.ResponseType.Ok) {
					string filename = fdiag.Filename;
					SaveProject(filename);
					IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
					messageService.ShowMessage(filename, GettextCatalog.GetString ("Project saved"));
				}
				
				fdiag.Hide ();
			}
		}

		public void CopyReferencesToOutputPath(bool force)
		{
			AbstractProjectConfiguration config = ActiveConfiguration as AbstractProjectConfiguration;
			if (config == null) {
				return;
			}
			foreach (ProjectReference projectReference in ProjectReferences) {
				if (projectReference.ReferenceType == ReferenceType.Project) {
					IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
					ArrayList allProjects = Combine.GetAllProjects (projectService.CurrentOpenCombine);
					foreach (ProjectCombineEntry entry in allProjects)
					{
						IProject proj = entry.Project;
						if (proj.Name != projectReference.Reference)
							continue;
						foreach (ProjectReference refrnc in proj.ProjectReferences)
						{
							if (refrnc.ReferenceType != ReferenceType.Gac && (refrnc.LocalCopy || force)) {
								string referenceFileName = refrnc.GetReferencedFileName (proj);
								string destinationFileName = fileUtilityService.GetDirectoryNameWithSeparator (config.OutputDirectory) + Path.GetFileName (referenceFileName);
								try {
									if (destinationFileName != referenceFileName) {
										File.Copy (referenceFileName, destinationFileName, true);
										if (File.Exists (referenceFileName + ".mdb"))
											File.Copy (referenceFileName + ".mdb", destinationFileName + ".mdb", true);
									}
								} catch { }
							}
						}
					}
				}
				if ((projectReference.LocalCopy || force) && projectReference.ReferenceType != ReferenceType.Gac) {
					string referenceFileName   = projectReference.GetReferencedFileName(this);
					string destinationFileName = fileUtilityService.GetDirectoryNameWithSeparator(config.OutputDirectory) + Path.GetFileName(referenceFileName);
					try {
						if (destinationFileName != referenceFileName) {
							File.Copy(referenceFileName, destinationFileName, true);
							if (File.Exists (referenceFileName + ".mdb"))
								File.Copy (referenceFileName + ".mdb", destinationFileName + ".mdb", true);
						}
					} catch (Exception e) {
						Console.WriteLine("Can't copy reference file from {0} to {1} reason {2}", referenceFileName, destinationFileName, e);
					}
				}
			}
		}
		
		public virtual void Dispose()
		{
			foreach (ProjectFile file in ProjectFiles) {
				file.Dispose ();
			}
		}
		
		public abstract IConfiguration CreateConfiguration();
		
		public virtual  IConfiguration CreateConfiguration(string name)
		{
			IConfiguration config = CreateConfiguration();
			config.Name = name;
			
			return config;
		}
		
 		internal void NotifyFileChangedInProject (ProjectFile file)
		{
			OnFileChangedInProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void NotifyFileRemovedFromProject (ProjectFile file)
		{
			OnFileRemovedFromProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void NotifyFileAddedToProject (ProjectFile file)
		{
			OnFileAddedToProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void NotifyReferenceRemovedFromProject (ProjectReference reference)
		{
			OnReferenceRemovedFromProject (new ProjectReferenceEventArgs (this, reference));
		}
		
		internal void NotifyReferenceAddedToProject (ProjectReference reference)
		{
			OnReferenceAddedToProject (new ProjectReferenceEventArgs (this, reference));
		}
		
		protected virtual void OnNameChanged(EventArgs e)
		{
			if (NameChanged != null) {
				NameChanged(this, e);
			}
		}
		
		protected virtual void OnFileRemovedFromProject (ProjectFileEventArgs e)
		{
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject (this, e);
			}
		}
		
		protected virtual void OnFileAddedToProject (ProjectFileEventArgs e)
		{
			if (FileAddedToProject != null) {
				FileAddedToProject (this, e);
			}
		}
		
		protected virtual void OnReferenceRemovedFromProject (ProjectReferenceEventArgs e)
		{
			if (ReferenceRemovedFromProject != null) {
				ReferenceRemovedFromProject (this, e);
			}
		}
		
		protected virtual void OnReferenceAddedToProject (ProjectReferenceEventArgs e)
		{
			if (ReferenceAddedToProject != null) {
				ReferenceAddedToProject (this, e);
			}
		}

 		protected virtual void OnFileChangedInProject (ProjectFileEventArgs e)
		{
			if (FileChangedInProject != null) {
				FileChangedInProject (this, e);
			}
		}
		
				
		public event EventHandler NameChanged;
		public event ProjectFileEventHandler FileRemovedFromProject;
		public event ProjectFileEventHandler FileAddedToProject;
		public event ProjectFileEventHandler FileChangedInProject;
		public event ProjectReferenceEventHandler ReferenceRemovedFromProject;
		public event ProjectReferenceEventHandler ReferenceAddedToProject;
	}
	
	public class ProjectActiveConfigurationTypeConverter : TypeConverter
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
			IProject project = (IProject)context.Instance;
			foreach (IConfiguration configuration in project.Configurations) {
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
			return new TypeConverter.StandardValuesCollection(((IProject)context.Instance).Configurations);
		}
	}
}
