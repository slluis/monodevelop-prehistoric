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
	/// Description of TypesImplementingInterfacesHaveNoSuffixImplRule.	
	/// </summary>
	public class TypesImplementingInterfacesHaveNoSuffixImplRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesImplementingInterfacesHaveNoSuffixImpl.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesImplementingInterfacesHaveNoSuffixImpl.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (type.GetInterfaces().Length > 0 && type.Name.EndsWith("Impl")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesImplementingInterfacesHaveNoSuffixImpl.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class TypesImplementingInterfacesHaveNoSuffixImplRuleTest
	{
		interface IInterface
		{
		}
		class AClassImplTest : IInterface
		{
			
		}
		[Test]
		public void TestCorrectTypenames()
		{
			TypesImplementingInterfacesHaveNoSuffixImplRule rule = new TypesImplementingInterfacesHaveNoSuffixImplRule();
			Assertion.AssertNull(rule.Check(typeof(AClassImplTest)));
		}
		
		class BImpl : IInterface
		{
			
		}
		[Test]
		public void TestIncorrectTypenames()
		{
			TypesImplementingInterfacesHaveNoSuffixImplRule rule = new TypesImplementingInterfacesHaveNoSuffixImplRule();
			Assertion.AssertNotNull(rule.Check(typeof(BImpl)));
		}
	}
}
*/
#endif
#endregion
