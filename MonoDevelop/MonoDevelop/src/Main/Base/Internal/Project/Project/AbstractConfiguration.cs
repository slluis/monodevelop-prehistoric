// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Xml;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Internal.Project
{
	public abstract class AbstractConfiguration : IConfiguration
	{
		[XmlAttribute("name")]
		protected string name = null;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override string ToString()
		{
			return name;
		}
	}
}
