// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Resources;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.ComponentModel.Design;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class DesignerResources : IResourceReader, IResourceWriter 
	{
		Hashtable resources = new Hashtable();
		string    fileName;
		
		public Hashtable Resources {
			get {
				return resources;
			}
			set {
				resources = value;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public void Load()
		{
			if (File.Exists(fileName)) {
				ResourceReader rr = new ResourceReader(fileName);
				foreach (DictionaryEntry entry in rr) {
					if (!resources.ContainsKey(entry.Key))
						resources.Add(entry.Key, entry.Value);
				}
				rr.Close();
			}
		}
		
		public void Save()
		{
			foreach (DictionaryEntry entry2 in Resources) {
				if (entry2.Value is Image) {
					ResourceWriter rw = new ResourceWriter(fileName);
					foreach (DictionaryEntry entry in resources) {
						rw.AddResource(entry.Key.ToString(), entry.Value);
					}
					rw.Generate();
					rw.Close();
					return;
				}
			}
		}
		
		public void Dispose()
		{
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public IDictionaryEnumerator GetEnumerator()
		{
			return resources.GetEnumerator();
		}
		
		public void Close()
		{
			// nothing
		}
		
		public void Generate()
		{
			Save();
		}
		
		public void AddResource(string name, byte[] value)
		{
			resources[name] = value;
			OnResourceAdded(EventArgs.Empty);
		}
		
		public void AddResource(string name, object value)
		{
			resources[name] = value;
			OnResourceAdded(EventArgs.Empty);
		}
		
		public void AddResource(string name, string value)
		{
			resources[name] = value;
			OnResourceAdded(EventArgs.Empty);
		}
		
		protected virtual void OnResourceAdded(EventArgs e)
		{
			if (ResourceAdded != null) {
				ResourceAdded(this, e);
			}
		}
		
		public event EventHandler ResourceAdded;
	}
	
	public class DesignerResourceService : IResourceService
	{
		IDesignerHost host;
		
		DesignerResources designerResources = new DesignerResources();
		
		public string FileName  = String.Empty;
		public string NameSpace = String.Empty;
		public string RootType  = String.Empty;
		
		string ResourceFileName {
			get {
				string r = Path.GetDirectoryName(FileName) + Path.DirectorySeparatorChar + NameSpace + "." + (host.RootComponent == null ? RootType : host.RootComponent.Site.Name) + ".resources";
				return r;
			}
		}
		
		public DesignerResourceService(IDesignerHost host)
		{
			this.host = host;
			designerResources.ResourceAdded += new EventHandler(AddResourceToProject);
		}
		
		public void LoadResources()
		{
			if (FileName.Length > 0) {
				designerResources.FileName = ResourceFileName;
				designerResources.Load();
			}
		}
		
		public void SaveResources()
		{
			if (FileName.Length > 0) {
				designerResources.FileName = ResourceFileName;
				designerResources.Save();
			}
		}
		
		public void AddResourceToProject(object sender, EventArgs e)
		{
			DesignerResources res = (DesignerResources)sender;
			
			foreach (DictionaryEntry entry in res.Resources) {
				if (entry.Value is Image) {
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					IProject project = projectService.CurrentSelectedProject;
					if (FileName.Length > 0 && project != null && !project.IsFileInProject(ResourceFileName)) {
						ProjectFile fileInformation = projectService.AddFileToProject(project, ResourceFileName, BuildAction.EmbedAsResource);
						ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView));
						pbv.UpdateCombineTree();
						((IResourceWriter)sender).Generate();
					}
				}
			}
		}
		
#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info)
		{
			return designerResources;
		}
		
		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
		{
			return designerResources;
		}
#endregion
	}
}
