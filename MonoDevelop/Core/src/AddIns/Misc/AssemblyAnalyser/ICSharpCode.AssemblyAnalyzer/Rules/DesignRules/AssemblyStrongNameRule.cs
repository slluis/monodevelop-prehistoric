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
	public class AssemblyStrongNameRule : AbstractReflectionRule, IAssemblyRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyStrongName.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyStrongName.Details}";
			}
		}
		
		public AssemblyStrongNameRule()
		{
			certainty = 95;
		}
		
		public Resolution Check(Assembly assembly)
		{
			byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
			if (publicKeyToken == null || publicKeyToken.Length == 0) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.AssemblyStrongName.Resolution}", assembly.Location, new string[,] { {"AssemblyName", Path.GetFileName(assembly.Location)} });
			}
			return null;
		}
	}
}
