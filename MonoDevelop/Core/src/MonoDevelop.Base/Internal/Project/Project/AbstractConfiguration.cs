// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Xml;
using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.Serialization;
using System.Collections;

namespace MonoDevelop.Internal.Project
{
	public abstract class AbstractConfiguration : IConfiguration, IExtendedDataItem
	{
		[ItemProperty("name")]
		string name = null;

		Hashtable properties;
		
		public string Name {
			get { return name; }
			set { name = value; }
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override string ToString()
		{
			return name;
		}
		
		public IDictionary ExtendedProperties {
			get {
				if (properties == null) properties = new Hashtable ();
				return properties;
			}
		}
	}
}
