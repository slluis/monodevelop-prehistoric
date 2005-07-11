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
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Services;
using MonoDevelop.Internal.Serialization;

namespace MonoDevelop.Internal.Project
{
	public abstract class CombineEntry : ICustomDataItem, IDisposable, IExtendedDataItem
	{
		[ItemProperty ("Configurations")]
		[ItemProperty ("Configuration", ValueType=typeof(IConfiguration), Scope=1)]
		ConfigurationCollection configurations = new ConfigurationCollection ();
		Hashtable extendedProperties;

		Combine parentCombine;
		IConfiguration activeConfiguration;
		string name;
		string path;
		
		IFileFormat fileFormat;
		
		public event CombineEntryRenamedEventHandler NameChanged;
		public event ConfigurationEventHandler ActiveConfigurationChanged;
		
		IDictionary IExtendedDataItem.ExtendedProperties {
			get {
				if (extendedProperties == null)
					extendedProperties = new Hashtable ();
				return extendedProperties;
			}
		}
		
		[ItemProperty ("name")]
		public virtual string Name {
			get {
				return name;
			}
			set {
				if (name != value && value != null && value.Length > 0) {
					string oldName = name;
					name = value;
					OnNameChanged (new CombineEntryRenamedEventArgs (this, oldName, name));
				}
			}
		}
		
		public virtual string FileName {
			get {
				if (parentCombine != null && path != null)
					return parentCombine.GetAbsoluteChildPath (path);
				else
					return path;
			}
			set {
				if (parentCombine != null && path != null)
					path = parentCombine.GetRelativeChildPath (value);
				else
					path = value;
				if (fileFormat != null)
					path = fileFormat.GetValidFormatName (FileName);
			}
		}
		
		public virtual IFileFormat FileFormat {
			get { return fileFormat; }
			set {
				fileFormat = value;
				FileName = fileFormat.GetValidFormatName (FileName);
			}
		}
		
		public virtual string RelativeFileName {
			get {
				if (path != null && parentCombine != null)
					return parentCombine.GetRelativeChildPath (path);
				else
					return path;
			}
		}
		
		public string BaseDirectory {
			get { return Path.GetDirectoryName (FileName); }
		}
		
		[ItemProperty ("fileversion")]
		protected virtual string CurrentFileVersion {
			get { return "2.0"; }
			set {}
		}
		
		public Combine ParentCombine {
			get { return parentCombine; }
		}
		
		public virtual void Save (string fileName, IProgressMonitor monitor)
		{
			FileName = fileName;
			Save (monitor);
		}
		
		public virtual void Save (IProgressMonitor monitor)
		{
			Runtime.ProjectService.WriteFile (FileName, this, monitor);
		}
		
		internal void SetParentCombine (Combine combine)
		{
			parentCombine = combine;
		}
		
		public ConfigurationCollection Configurations {
			get {
				return configurations;
			}
		}
		
		public IConfiguration ActiveConfiguration {
			get {
				if (activeConfiguration == null && configurations.Count > 0) {
					return (IConfiguration)configurations[0];
				}
				return activeConfiguration;
			}
			set {
				if (activeConfiguration != value) {
					activeConfiguration = value;
					OnActiveConfigurationChanged (new ConfigurationEventArgs (this, value));
				}
			}
		}
		
		public virtual DataCollection Serialize (ITypeSerializer handler)
		{
			DataCollection data = handler.Serialize (this);
			if (activeConfiguration != null) {
				DataItem confItem = data ["Configurations"] as DataItem;
				confItem.UniqueNames = true;
				if (confItem != null)
					confItem.ItemData.Add (new DataValue ("active", activeConfiguration.Name));
			}
			return data;
		}
		
		public virtual void Deserialize (ITypeSerializer handler, DataCollection data)
		{
			DataValue ac = null;
			DataItem confItem = data ["Configurations"] as DataItem;
			if (confItem != null)
				ac = (DataValue) confItem.ItemData.Extract ("active");
				
			handler.Deserialize (this, data);
			if (ac != null)
				activeConfiguration = GetConfiguration (ac.Value);
		}
		
		public abstract IConfiguration CreateConfiguration (string name);
		
		public IConfiguration GetConfiguration (string name)
		{
			if (configurations != null) {
				foreach (IConfiguration conf in configurations)
					if (conf.Name == name) return conf;
			}
			return null;
		}

		public string GetAbsoluteChildPath (string relPath)
		{
			if (Path.IsPathRooted (relPath))
				return relPath;
			else
				return Runtime.FileUtilityService.RelativeToAbsolutePath (BaseDirectory, relPath);
		}
		
		public string GetRelativeChildPath (string absPath)
		{
			return Runtime.FileUtilityService.AbsoluteToRelativePath (BaseDirectory, absPath);
		}
		
		public virtual void Dispose()
		{
		}
		
		protected virtual void OnNameChanged (CombineEntryRenamedEventArgs e)
		{
			Combine topMostParentCombine = this.parentCombine;

			if (topMostParentCombine != null) {
				while (topMostParentCombine.ParentCombine != null) {
					topMostParentCombine = topMostParentCombine.ParentCombine;
				}
				
				foreach (Project project in topMostParentCombine.GetAllProjects()) {
					if (project == this) {
						continue;
					}
					
					project.RenameReferences(e.OldName, e.NewName);
				}
			}
			
			if (NameChanged != null) {
				NameChanged (this, e);
			}
		}
		
		protected virtual void OnActiveConfigurationChanged (ConfigurationEventArgs args)
		{
			if (ActiveConfigurationChanged != null)
				ActiveConfigurationChanged (this, args);
		}
		
		public abstract void Clean ();
		public abstract ICompilerResult Build (IProgressMonitor monitor);
		public abstract void Execute (IProgressMonitor monitor);
		public abstract void Debug (IProgressMonitor monitor);
		public abstract bool NeedsBuilding { get; set; }
		
		public virtual void GenerateMakefiles (Combine parentCombine)
		{
		}
	}
}
