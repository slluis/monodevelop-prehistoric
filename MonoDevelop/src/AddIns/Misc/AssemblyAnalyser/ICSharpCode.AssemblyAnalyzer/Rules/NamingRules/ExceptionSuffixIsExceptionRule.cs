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
	/// Description of ExceptionSuffixIsException.	
	/// </summary>
	public class ExceptionSuffixIsExceptionRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.ExceptionSuffixIsException.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.ExceptionSuffixIsException.Details}";
			}
		}
		
		public ExceptionSuffixIsExceptionRule()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (typeof(System.Exception).IsAssignableFrom(type) && !type.Name.EndsWith("Exception")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.ExceptionSuffixIsException.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class ExceptionSuffixIsExceptionRuleTest
	{
		class MyException : System.Exception
		{
		}
		[Test]
		public void TestCorrectException()
		{
			ExceptionSuffixIsExceptionRule rule = new ExceptionSuffixIsExceptionRule();
			Assertion.AssertNull(rule.Check(typeof(MyException)));
		}
		
		class MyExcpt : System.Exception
		{
		}
		[Test]
		public void TestIncorrectException()
		{
			ExceptionSuffixIsExceptionRule rule = new ExceptionSuffixIsExceptionRule();
			Assertion.AssertNotNull(rule.Check(typeof(MyExcpt)));
		}
	}
}
*/
#endif
#endregion
