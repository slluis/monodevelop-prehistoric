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
	/// Description of OnlyAttributesSuffixAttribute.	
	/// </summary>
	public class OnlyAttributesSuffixAttributeRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyAttributesSuffixAttribute.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyAttributesSuffixAttribute.Details}";
			}
		}
		
		public OnlyAttributesSuffixAttributeRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		public Resolution Check(Type type)
		{
			if (!type.IsSubclassOf(typeof(System.Attribute)) && type.Name.EndsWith("Attribute")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyAttributesSuffixAttribute.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
			}
			return null;
		}
	}
}
#region Unit Test
#if TEST
namespace MonoDevelop.AssemblyAnalyser.Rules
{
	using NUnit.Framework;

	[TestFixture]
	public class OnlyAttributesSuffixAttributeTest
	{
		class MyOtherClass
		{
		}
		class RealAttribute : System.Attribute
		{
		}
		[Test]
		public void TestCorrectAttribute()
		{
			OnlyAttributesSuffixAttributeRule rule = new OnlyAttributesSuffixAttributeRule();
			Assertion.AssertNull(rule.Check(typeof(MyOtherClass)));
			Assertion.AssertNull(rule.Check(typeof(RealAttribute)));
		}
		
		class MyAttribute
		{
		}
		[Test]
		public void TestIncorrectAttribute()
		{
			OnlyAttributesSuffixAttributeRule rule = new OnlyAttributesSuffixAttributeRule();
			Assertion.AssertNotNull(rule.Check(typeof(MyAttribute)));
		}
	}
}
#endif
#endregion
