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
	/// Description of OnlyExceptionsSuffixException.	
	/// </summary>
	public class OnlyPermissionsSuffixPermissionRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyPermissionsSuffixPermission.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyPermissionsSuffixPermission.Details}";
			}
		}
		
		public OnlyPermissionsSuffixPermissionRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		public Resolution Check(Type type)
		{
			if (!typeof(System.Security.IPermission).IsAssignableFrom(type) && type.Name.EndsWith("Permission")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyPermissionsSuffixPermission.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class OnlyPermissionsSuffixPermissionRuleTest
	{
		class OtherClass
		{}
		[Test]
		public void TestCorrectPermission()
		{
			OnlyPermissionsSuffixPermissionRule rule = new OnlyPermissionsSuffixPermissionRule();
			Assertion.AssertNull(rule.Check(typeof(System.Security.Permissions.EnvironmentPermission)));
			Assertion.AssertNull(rule.Check(typeof(System.Security.Permissions.FileIOPermission)));
			Assertion.AssertNull(rule.Check(typeof(OtherClass)));
		}
		
		class NotAnPermission
		{
		}
		[Test]
		public void TestIncorrectPermission()
		{
			OnlyPermissionsSuffixPermissionRule rule = new OnlyPermissionsSuffixPermissionRule();
			Assertion.AssertNotNull(rule.Check(typeof(NotAnPermission)));
		}
	}
}
*/
#endif
#endregion
