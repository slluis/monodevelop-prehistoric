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
	/// Description of InterfaceNotEmpty.	
	/// </summary>
	public class InterfaceNotEmptyRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.InterfaceNotEmpty.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.InterfaceNotEmpty.Details}";
			}
		}
		public Resolution Check(Type type)
		{
			if (type.IsInterface && type.GetMembers().Length == 0) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.InterfaceNotEmpty.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class InterfaceNotEmptyRuleTests
	{
		interface NonEmptyInterface1
		{
			void A();
		}
		interface NonEmptyInterface2
		{
			event EventHandler TestEvent;
		}
		interface NonEmptyInterface3
		{
			int MyProperty {
				get;
			}
		}
		[Test]
		public void TestNonEmptyInterface()
		{
			InterfaceNotEmptyRule rule = new InterfaceNotEmptyRule();
			Assertion.AssertNull(rule.Check(typeof(NonEmptyInterface1)));
			Assertion.AssertNull(rule.Check(typeof(NonEmptyInterface2)));
			Assertion.AssertNull(rule.Check(typeof(NonEmptyInterface3)));
		}
		
		interface EmptyInterface
		{
		}
		
		[Test]
		public void TestEmptyInterface()
		{
			InterfaceNotEmptyRule rule = new InterfaceNotEmptyRule();
			Assertion.AssertNotNull(rule.Check(typeof(EmptyInterface)));
		}
	}
}
#endif
#endregion
