// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using MonoDevelop.Internal.Project;
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
	[XmlNodeName("File")]	
	public class ProjectFile : LocalizedObject, ICloneable
	{
		[XmlAttribute("name"),
		 ConvertToRelativePath()]
		string      filename;
		
		[XmlAttribute("subtype")]	
		Subtype     subtype;
		
		[XmlAttribute("buildaction")]
		BuildAction buildaction;
		
		[XmlAttribute("dependson"),
		 ConvertToRelativePath()]		
		string		dependsOn;
		
		[XmlAttribute("data")]
		string		data;
		
		[XmlAttribute(null)]
		AbstractProject project;
		
		internal void SetProject (AbstractProject prj)
		{
			project = prj;
		}
						
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectFile.Name}",
		                   Description ="${res:MonoDevelop.Internal.Project.ProjectFile.Name.Description}")]
		[ReadOnly(true)]
		public string Name {
			get {
				return filename;
			}
			set {
				project.NotifyFileRemovedFromProject (this);
				filename = value;
				Debug.Assert(filename != null && filename.Length > 0, "name == null || name.Length == 0");
				project.NotifyFileAddedToProject (this);
			}
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
		
		public ProjectFile()
		{
		}
		
		public ProjectFile(string filename)
		{
			this.filename = filename;
			subtype       = Subtype.Code;
			buildaction   = BuildAction.Compile;
		}
		
		public ProjectFile(string filename, BuildAction buildAction)
		{
			this.filename = filename;
			subtype       = Subtype.Code;
			buildaction   = buildAction;
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override string ToString()
		{
			return "[ProjectFile: FileName=" + filename + ", Subtype=" + subtype + ", BuildAction=" + BuildAction + "]";
		}
										
		
	}
}
