// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Reflection;

namespace MonoDevelop.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of AssemblyStrongName.	
	/// </summary>
	public class AssemblyClsCompliantRule : AbstractReflectionRule, IAssemblyRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyClsCompliantRule.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyClsCompliantRule.Details}";
			}
		}
		
		public AssemblyClsCompliantRule()
		{
			certainty = 99;
		}
		
		public Resolution Check(Assembly assembly)
		{
			object[] attributes = assembly.GetCustomAttributes(typeof(System.CLSCompliantAttribute), true);
			if (attributes == null || attributes.Length == 0) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyClsCompliantRule.Resolution1}", assembly.Location, new string[,] { {"AssemblyName", Path.GetFileName(assembly.Location)} });
			} else {
				foreach (CLSCompliantAttribute attr in attributes) {
					if (!attr.IsCompliant) {
						return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyClsCompliantRule.Resolution2}", assembly.Location, new string[,] { {"AssemblyName", Path.GetFileName(assembly.Location)} });
					}
				}
			}
			return null;
		}
	}
}
