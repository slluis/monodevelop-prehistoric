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

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Serialization;

using MonoDevelop.Core.Properties;
using MonoDevelop.Gui;

namespace MonoDevelop.Internal.Project
{
	public class CombineConfiguration : AbstractConfiguration
	{
		[ExpandedCollection]
		[ItemProperty ("Entry", ValueType=typeof(Config))]
		ArrayList configurations = new ArrayList();
		
		[DataItem ("Entry")]
		public class Config 
		{
			[ItemProperty ("name")]
			string entryName;
			
			public CombineEntry entry;
			
			[ItemProperty ("configurationname")]
			public string ConfigurationName;
			
			[ItemProperty ("build")]
			public bool Build;
			
			public CombineEntry Entry {
				get { return entry; }
				set { entry = value; if (entry != null) entryName = entry.Name; }
			}
			
			internal void SetCombine (Combine combine)
			{
				if (entryName != null)
					Entry = combine.Entries [entryName];
			}
		}
		
		public CombineConfiguration ()
		{
		}
		
		public CombineConfiguration (string name)
		{
			this.Name = name;
		}
		
		internal void SetCombine (Combine combine)
		{
			foreach (Config conf in configurations)
				conf.SetCombine (combine);
		}
		
		public Config GetConfiguration(int number)
		{
			if (number < configurations.Count) {
				return (Config)configurations[number];
			} 
			Debug.Assert(false, "Configuration number " + number + " not found.\n" + configurations.Count + " configurations avaiable.");
			return null;
		}
		
		public void AddEntry (CombineEntry combine)
		{
			Config conf = new Config();
			conf.Entry = combine;
			conf.ConfigurationName = combine.ActiveConfiguration != null ? combine.ActiveConfiguration.Name : String.Empty;
			conf.Build = false;
			configurations.Add(conf);
		}
		
		public void RemoveEntry (CombineEntry entry)
		{
			Config removeConfig = null;
			
			foreach (Config config in configurations) {
				if (config.Entry == entry) {
					removeConfig = config;
					break;
				}
			}
			
			Debug.Assert(removeConfig != null);
			configurations.Remove(removeConfig);
		}
	}
}
