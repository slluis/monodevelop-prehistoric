// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;

namespace MonoDevelop.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of PermissionSuffixIsPermissionRule.	
	/// </summary>
	public class PermissionSuffixIsPermissionRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.PermissionSuffixIsPermission.Description}";
			}
		}
		
		// System.Attribute
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.PermissionSuffixIsPermission.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (typeof(System.Security.IPermission).IsAssignableFrom(type) && !type.Name.EndsWith("Permission")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.PermissionSuffixIsPermission.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
			}
			return null;
		}
	}
}

#region Unit Test
#if TEST
/*
namespace MonoDevelop.AssemblyAnalyser.Rules
{
	using NUnit.Framework;

	[TestFixture]
	public class PermissionSuffixIsPermissionRuleTest
	{
		[Test]
		public void TestCorrectPermission()
		{
			PermissionSuffixIsPermissionRule rule = new PermissionSuffixIsPermissionRule();
			Assertion.AssertNull(rule.Check(typeof(System.Security.Permissions.EnvironmentPermission)));
			Assertion.AssertNull(rule.Check(typeof(System.Security.Permissions.FileIOPermission)));
			Assertion.AssertNull(rule.Check(typeof(PermissionSuffixIsPermissionRuleTest)));
		}
		
	}
}
*/
#endif
#endregion
