// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using System.ComponentModel;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Internal.Project
{
	public enum ReferenceType {
		Assembly,
		Project,
		Gac,
		Typelib
	}
	
	/// <summary>
	/// This class represent a reference information in an IProject object.
	/// </summary>
	[XmlNodeName("Reference")]
	public class ProjectReference : LocalizedObject, ICloneable
	{
		[XmlAttribute("type")]
		ReferenceType referenceType;
		
		[XmlAttribute("refto")]
		[ConvertToRelativePath("IsAssembly")]
		string        reference = String.Empty;
		
		[XmlAttribute("localcopy")]
		bool          localCopy = true;
		
		bool IsAssembly {
			get {
				return referenceType == ReferenceType.Assembly;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectReference.ReferenceType}",
		                   Description ="${res:MonoDevelop.Internal.Project.ProjectReference.ReferenceType.Description})")]
		public ReferenceType ReferenceType {
			get {
				return referenceType;
			}
			set {
				referenceType = value;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectReference.Reference}",
		                   Description = "${res:MonoDevelop.Internal.Project.ProjectReference.Reference.Description}")]
		public string Reference {
			get {
				return reference;
			}
			set {
				reference = value;
				OnReferenceChanged(EventArgs.Empty);
			}
		}
		
		[LocalizedProperty("${res:MonoDevelop.Internal.Project.ProjectReference.LocalCopy}",
		                   Description = "${res:MonoDevelop.Internal.Project.ProjectReference.LocalCopy.Description}")]
		[DefaultValue(true)]
		public bool LocalCopy {
			get {
				return localCopy;
			}
			set {
				localCopy = value;
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				projectService.SaveCombine();
			}
		}
		
		/// <summary>
		/// Returns the file name to an assembly, regardless of what 
		/// type the assembly is.
		/// </summary>
		public string GetReferencedFileName(IProject project)
		{
			switch (ReferenceType) {
				case ReferenceType.Typelib:
					return String.Empty;
				case ReferenceType.Assembly:
					return reference;
				
				case ReferenceType.Gac:
					string file = ((IParserService)ServiceManager.GetService (typeof (IParserService))).LoadAssemblyFromGac (GetPathToGACAssembly (this));
					return file == String.Empty ? reference : file;
				case ReferenceType.Project:
					IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
					string projectOutputLocation   = projectService.GetOutputAssemblyName(reference);
					return projectOutputLocation;
				
				default:
					throw new NotImplementedException("unknown reference type : " + ReferenceType);
			}
		}
		
		public ProjectReference()
		{
		}
		
		public ProjectReference(ReferenceType referenceType, string reference)
		{
			this.referenceType = referenceType;
			this.reference     = reference;
		}
		
		
		/// <summary>
		/// This method returns the absolute path to an GAC assembly.
		/// </summary>
		/// <param name ="refInfo">
		/// The reference information containing a GAC reference information.
		/// </param>
		/// <returns>
		/// the absolute path to the GAC assembly which refInfo points to.
		/// </returns>
		static string GetPathToGACAssembly(ProjectReference refInfo)
		{ // HACK : Only works on windows.
			Debug.Assert(refInfo.ReferenceType == ReferenceType.Gac);
			string[] info = refInfo.Reference.Split(',');
			
			//if (info.Length < 4) {
			return info[0];
			//	}
			
			/*string aName      = info[0];
			string aVersion   = info[1].Substring(info[1].LastIndexOf('=') + 1);
			string aPublicKey = info[3].Substring(info[3].LastIndexOf('=') + 1);
			
			return System.Environment.GetFolderPath(Environment.SpecialFolder.System) + 
			       Path.DirectorySeparatorChar + ".." +
			       Path.DirectorySeparatorChar + "assembly" +
			       Path.DirectorySeparatorChar + "GAC" +
			       Path.DirectorySeparatorChar + aName +
			       Path.DirectorySeparatorChar + aVersion + "__" + aPublicKey +
			       Path.DirectorySeparatorChar + aName + ".dll";*/
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		protected virtual void OnReferenceChanged(EventArgs e) 
		{
			if (ReferenceChanged != null) {
				ReferenceChanged(this, e);
			}
		}
		
		public event EventHandler ReferenceChanged;
	}
}
