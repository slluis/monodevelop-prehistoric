// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Reflection;
using MonoDevelop.AssemblyAnalyser.Rules;

namespace MonoDevelop.AssemblyAnalyser
{
	/// <summary>
	/// Description of AssemblyVersionNumber.	
	/// </summary>
	public class AssemblyVersionNumberRule : AbstractReflectionRule, IAssemblyRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyVersionNumber.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyVersionNumber.Details}";
			}
		}
		
		public AssemblyVersionNumberRule()
		{
			certainty = 95;
		}
		
		public Resolution Check(Assembly assembly)
		{
			if (assembly.GetName().Version == new Version(0, 0, 0, 0)) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyVersionNumber.Resolution}", assembly.Location, new string[,] { {"AssemblyName", Path.GetFileName(assembly.Location)} });
			}
			return null;
		}
	}
}
