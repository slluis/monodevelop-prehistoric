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
	/// Description of TypesShouldBeInNamespaces.	
	/// </summary>
	public class TypesShouldBeInNamespacesRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesShouldBeInNamespaces.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesShouldBeInNamespaces.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (type.Namespace == null || type.Namespace.Length == 0) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesShouldBeInNamespaces.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
			}
			return null;
		}
	}
}

#region Unit Test
#if TEST
class OutsideNamespace
{
	
}
namespace MonoDevelop.AssemblyAnalyser.Rules
{
	using NUnit.Framework;

	[TestFixture]
	public class TypesShouldBeInNamespacesRuleTest
	{
		interface ICorrectInterface
		{
		}
		[Test]
		public void TestCorrectAttribute()
		{
			TypesShouldBeInNamespacesRule rule = new TypesShouldBeInNamespacesRule();
			Assertion.AssertNull(rule.Check(typeof(System.ICloneable)));
			Assertion.AssertNull(rule.Check(typeof(TypesShouldBeInNamespacesRuleTest)));
			Assertion.AssertNull(rule.Check(typeof(ICorrectInterface)));
		}
		
		[Test]
		public void TestIncorrectAttribute()
		{
			TypesShouldBeInNamespacesRule rule = new TypesShouldBeInNamespacesRule();
			Assertion.AssertNotNull(rule.Check(typeof(OutsideNamespace)));
		}
	}
}
#endif
#endregion
