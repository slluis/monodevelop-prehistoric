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
	/// Description of AttributeSuffixIsAttribute.	
	/// </summary>
	public class AttributeSuffixIsAttributeRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AttributeSuffixIsAttribute.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.AttributeSuffixIsAttribute.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (type.IsSubclassOf(typeof(System.Attribute)) && !type.Name.EndsWith("Attribute")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.AttributeSuffixIsAttribute.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class AttributeSuffixIsAttributeRuleTest
	{
		class MyAttribute : System.Attribute
		{
		}
		[Test]
		public void TestCorrectAttribute()
		{
			AttributeSuffixIsAttributeRule rule = new AttributeSuffixIsAttributeRule();
			Assertion.AssertNull(rule.Check(typeof(MyAttribute)));
		}
		
		class MyAttr : System.Attribute
		{
		}
		[Test]
		public void TestIncorrectAttribute()
		{
			AttributeSuffixIsAttributeRule rule = new AttributeSuffixIsAttributeRule();
			Assertion.AssertNotNull(rule.Check(typeof(MyAttr)));
		}
	}
}
*/
#endif
#endregion
