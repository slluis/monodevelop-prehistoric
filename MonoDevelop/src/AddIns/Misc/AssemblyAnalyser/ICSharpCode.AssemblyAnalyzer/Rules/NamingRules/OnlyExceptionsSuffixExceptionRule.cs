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
	/// Description of OnlyExceptionsSuffixExceptionRule.	
	/// </summary>
	public class OnlyExceptionsSuffixExceptionRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyExceptionsSuffixException.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyExceptionsSuffixException.Details}";
			}
		}
		
		public OnlyExceptionsSuffixExceptionRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		
		public Resolution Check(Type type)
		{
			if (!typeof(System.Exception).IsAssignableFrom(type) && type.Name.EndsWith("Exception")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyExceptionsSuffixException.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class OnlyExceptionsSuffixExceptionRuleTest
	{
		class MyException : System.Exception
		{
		}
		class OtherClass
		{}
		[Test]
		public void TestCorrectException()
		{
			OnlyExceptionsSuffixExceptionRule rule = new OnlyExceptionsSuffixExceptionRule();
			Assertion.AssertNull(rule.Check(typeof(MyException)));
			Assertion.AssertNull(rule.Check(typeof(OtherClass)));
		}
		
		class NotAnException
		{
		}
		[Test]
		public void TestIncorrectException()
		{
			OnlyExceptionsSuffixExceptionRule rule = new OnlyExceptionsSuffixExceptionRule();
			Assertion.AssertNotNull(rule.Check(typeof(NotAnException)));
		}
	}
}
#endif
#endregion
