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
	public abstract class CombineEntry : ICustomDataItem, IDisposable
	{
		[ItemProperty ("Configurations")]
		[ItemProperty ("Configuration", ValueType=typeof(IConfiguration), Scope=1)]
		ArrayList configurations = new ArrayList();

		Combine parentCombine;
		IConfiguration activeConfiguration;
		string name;
		string path;
		
		IFileFormat fileFormat;
		
		public event CombineEntryRenamedEventHandler NameChanged;
		
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
		
		public ArrayList Configurations {
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
				activeConfiguration = value;
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
			if (NameChanged != null) {
				NameChanged (this, e);
			}
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
	
	public interface ICombineEntryCollection: IEnumerable
	{
		int Count { get; }
		CombineEntry this [int n] { get; }
	}
	
	public class CombineEntryCollection: ICombineEntryCollection
	{
		ArrayList list = new ArrayList ();
		Combine parentCombine;
		
		internal CombineEntryCollection ()
		{
		}
		
		internal CombineEntryCollection (Combine combine)
		{
			parentCombine = combine;
		}
		
		public int Count
		{
			get { return list.Count; }
		}
		
		public CombineEntry this [int n]
		{
			get { return (CombineEntry) list[n]; }
		}
		
		public CombineEntry this [string name]
		{
			get {
			for (int n=0; n<list.Count; n++)
				if (((CombineEntry)list[n]).Name == name)
					return (CombineEntry)list[n];
			return null;
			}
		}
		
		public IEnumerator GetEnumerator ()
		{
			return list.GetEnumerator ();
		}
		
		public void Add (CombineEntry entry)
		{
			list.Add (entry);
			if (parentCombine != null) {
				entry.SetParentCombine (parentCombine);
				parentCombine.NotifyEntryAdded (entry);
			}
		}
		
		public void Remove (CombineEntry entry)
		{
			list.Remove (entry);
			if (parentCombine != null) {
				entry.SetParentCombine (null);
				parentCombine.NotifyEntryRemoved (entry);
			}
		}
		
		public int IndexOf (CombineEntry entry)
		{
			return list.IndexOf (entry);
		}
		
		public int IndexOf (string name)
		{
			for (int n=0; n<list.Count; n++)
				if (((CombineEntry)list[n]).Name == name)
					return n;
			return -1;
		}
		
		public void Clear ()
		{
			list.Clear ();
		}
	}
}
