// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Dialogs;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.Internal.Templates
{
	public class OpenFileAction
	{
		string fileName;
		
		public OpenFileAction(string fileName)
		{
			this.fileName = fileName;
		}
		
		public void Run(ProjectCreateInformation projectCreateInformation)
		{
			Runtime.FileService.OpenFile (projectCreateInformation.ProjectBasePath + Path.DirectorySeparatorChar + fileName);
		}
	}
	
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	public class ProjectTemplate
	{
		public static ArrayList ProjectTemplates = new ArrayList();
		
		string    originator   = null;
		string    created      = null;
		string    lastmodified = null;
		string    name         = null;
		string    category     = null;
		string    languagename = null;
		string    description  = null;
		string    icon         = null;
		string    wizardpath   = null;
		ArrayList actions      = new ArrayList();

		
		CombineDescriptor combineDescriptor = null;
		
#region Template Properties
		public string WizardPath {
			get {
				return wizardpath;
			}
		}
		
		public string Originator {
			get {
				return originator;
			}
		}
		
		public string Created {
			get {
				return created;
			}
		}
		
		public string LastModified {
			get {
				return lastmodified;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string LanguageName {
			get {
				return languagename;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
		}

		[Browsable(false)]
		public CombineDescriptor CombineDescriptor
		{
			get 
			{
				return combineDescriptor;
			}
		}
#endregion
		
		protected ProjectTemplate(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			
			originator   = doc.DocumentElement.Attributes["originator"].InnerText;
			created      = doc.DocumentElement.Attributes["created"].InnerText;
			lastmodified = doc.DocumentElement.Attributes["lastModified"].InnerText;
			
			XmlElement config = doc.DocumentElement["TemplateConfiguration"];
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].InnerText;
			}
			
			name         = GettextCatalog.GetString (config["_Name"].InnerText);
			category     = config["Category"].InnerText;
			
			if (config["LanguageName"] != null)
				languagename = config["LanguageName"].InnerText;
			
			if (config["_Description"] != null) {
				description  = GettextCatalog.GetString (config["_Description"].InnerText);
			}
			
			if (config["Icon"] != null) {
				icon = config["Icon"].InnerText;
			}
			
			if (doc.DocumentElement["Combine"] != null) {
				combineDescriptor = CombineDescriptor.CreateCombineDescriptor(doc.DocumentElement["Combine"]);
			}
			
			// Read Actions;
			if (doc.DocumentElement["Actions"] != null) {
				foreach (XmlElement el in doc.DocumentElement["Actions"]) {
					actions.Add(new OpenFileAction(el.Attributes["filename"].InnerText));
				}
			}
		}
		
		string lastCombine    = null;
//		string startupProject = null;
		ProjectCreateInformation projectCreateInformation;
		
		public string CreateProject(ProjectCreateInformation projectCreateInformation)
		{
			this.projectCreateInformation = projectCreateInformation;
			
			if (wizardpath != null) {
//              TODO: WIZARD
				IProperties customizer = new DefaultProperties();
				customizer.SetProperty("ProjectCreateInformation", projectCreateInformation);
				customizer.SetProperty("ProjectTemplate", this);
				WizardDialog wizard = new WizardDialog("Project Wizard", customizer, wizardpath);
				//if (wizard.ShowDialog() == DialogResult.OK) {
				//	lastCombine = combineDescriptor.CreateCombine(projectCreateInformation, this.languagename);
				//} else {
				//	return null;
				//}
			} else {
				lastCombine = combineDescriptor.CreateCombine(projectCreateInformation, this.languagename);
			}
			
			return lastCombine;
		}
		
		public void OpenCreatedCombine()
		{
			Runtime.ProjectService.OpenCombine(lastCombine);
			
			foreach (OpenFileAction action in actions) {
				action.Run(projectCreateInformation);
			}
		}

		static void LoadProjectTemplate(string fileName)
		{
			try {
				
			} catch (Exception e) {
				throw new ApplicationException("error while loading " + fileName + " original exception was : " + e.ToString());
			}
		}
		
		static ProjectTemplate()
		{
			LoadTemplates ((ProjectTemplateCodon[])(AddInTreeSingleton.AddInTree.GetTreeNode ("/MonoDevelop/ProjectTemplates").BuildChildItems (new object ()).ToArray (typeof (ProjectTemplateCodon))));
		}

		static void LoadTemplates (ProjectTemplateCodon[] codons)
		{
			foreach (ProjectTemplateCodon codon in codons) {
				try {
					ProjectTemplates.Add (new ProjectTemplate (codon.Location));
				} catch (Exception e) {
					Runtime.MessageService.ShowError (e, String.Format (GettextCatalog.GetString ("Error loading template file {0}"), codon.Location));
				}
			}
		}
	}
}
