
// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using MonoDevelop.Internal.Serialization;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Internal.Project
{
	public enum Subtype {
		Code,
		Directory,
		WinForm,
		WebForm,
		XmlForm,
		WebService,		
		WebReferences,
		Dataset
	}
	
	public enum BuildAction {
		Nothing,
		Compile,
		EmbedAsResource,
		Exclude		
	}
	
	/// <summary>
	/// This class represent a file information in an IProject object.
	/// </summary>
	public class ProjectFile : LocalizedObject, ICloneable
	{
		[ProjectPathItemProperty("name")]
		string filename;
		
		[ItemProperty("subtype")]	
		Subtype subtype;
		
		[ItemProperty("buildaction")]
		BuildAction buildaction;
		
		[ItemProperty("dependson", DefaultValue="")]		
		string dependsOn;
		
		[ItemProperty("data", DefaultValue="")]
		string data;
		
		Project project;
		
		private FileSystemWatcher ProjectFileWatcher;
		
		public ProjectFile()
		{
			AddFileWatch();
		}
		
		public ProjectFile(string filename)
		{
			this.filename = filename;
			subtype       = Subtype.Code;
			buildaction   = BuildAction.Compile;
			AddFileWatch();
		}
		
		public ProjectFile(string filename, BuildAction buildAction)
		{
			this.filename = filename;
			subtype       = Subtype.Code;
			buildaction   = buildAction;
			AddFileWatch();
		}
		
		private void AddFileWatch()
		{
			ProjectFileWatcher = new FileSystemWatcher();

			ProjectFileWatcher.Changed += new FileSystemEventHandler(OnChanged);
		
			if (this.filename != null) 
				UpdateFileWatch();
				
		}
		
		private void UpdateFileWatch()
		{
		
			if ((this.filename == null) || (this.filename.Length == 0))
				return;				

			try {
				ProjectFileWatcher.EnableRaisingEvents = false;
				ProjectFileWatcher.Path = Path.GetDirectoryName(filename);
				ProjectFileWatcher.Filter = Path.GetFileName(filename);
				ProjectFileWatcher.EnableRaisingEvents = true;
			} catch {
				Console.WriteLine ("NOT WATCHING " + filename);
			}

		}
		
		private void OnChanged(object source, FileSystemEventArgs e)
		{
			if (project != null)
				project.NotifyFileChangedInProject(this);
		}

		internal void SetProject (Project prj)
		{
			project = prj;
			UpdateFileWatch();
		}
						
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectFile.Name}",
		                   Description ="${res:MonoDevelop.Internal.Project.ProjectFile.Name.Description}")]
		[ReadOnly(true)]
		public string Name {
			get {
				return filename;
			}
			set {
				Debug.Assert (value != null && value.Length > 0, "name == null || name.Length == 0");
				if (project != null) project.NotifyFileRemovedFromProject (this);
				filename = value;
				UpdateFileWatch();
				if (project != null) project.NotifyFileAddedToProject (this);
			}
		}
		
		public string FilePath {
			get {
				return filename;
			}
		}
		
		public string RelativePath {
			get { return filename; }
		}
		
		[Browsable(false)]
		public Subtype Subtype {
			get {
				return subtype;
			}
			set {
				subtype = value;
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectFile.BuildAction}",
		                   Description ="${res:MonoDevelop.Internal.Project.ProjectFile.BuildAction.Description}")]
		public BuildAction BuildAction {
			get {
				return buildaction;
			}
			set {
				buildaction = value;
			}
		}
		
		[Browsable(false)]
		public string DependsOn {
			get {
				return dependsOn;
			}
			set {
				dependsOn = value;
			}
		}
		
		[Browsable(false)]
		public string Data {
			get {
				return data;
			}
			set {
				data = value;
			}
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override string ToString()
		{
			return "[ProjectFile: FileName=" + filename + ", Subtype=" + subtype + ", BuildAction=" + BuildAction + "]";
		}
										
		public virtual void Dispose ()
		{
			ProjectFileWatcher.Dispose ();
		}
	}
}
