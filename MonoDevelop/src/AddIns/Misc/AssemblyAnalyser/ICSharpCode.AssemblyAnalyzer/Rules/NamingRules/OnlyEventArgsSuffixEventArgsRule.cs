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
	/// Description of OnlyEventArgsSuffixEventArgsRule.	
	/// </summary>
	public class OnlyEventArgsSuffixEventArgsRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyEventArgsSuffixEventArgs.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyEventArgsSuffixEventArgs.Details}";
			}
		}
		
		public OnlyEventArgsSuffixEventArgsRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		public Resolution Check(Type type)
		{
			if (!typeof(System.EventArgs).IsAssignableFrom(type) && type.Name.EndsWith("EventArgs")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyEventArgsSuffixEventArgs.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class OnlyEventArgsSuffixEventArgsRuleTest
	{
		class CorrectEventArgs : System.EventArgs
		{
		}
		class OtherClass
		{
		}
		class MyEventArgs : CorrectEventArgs
		{
		}
		[Test]
		public void TestCorrectEventArgs()
		{
			OnlyEventArgsSuffixEventArgsRule rule = new OnlyEventArgsSuffixEventArgsRule();
			Assertion.AssertNull(rule.Check(typeof(CorrectEventArgs)));
			Assertion.AssertNull(rule.Check(typeof(OtherClass)));
			Assertion.AssertNull(rule.Check(typeof(MyEventArgs)));
		}
		
		class IncorrectEventArgs
		{
		}
		[Test]
		public void TestIncorrectEventArgs()
		{
			OnlyEventArgsSuffixEventArgsRule rule = new OnlyEventArgsSuffixEventArgsRule();
			Assertion.AssertNotNull(rule.Check(typeof(IncorrectEventArgs)));
		}
		
	}
}
*/
#endif
#endregion
