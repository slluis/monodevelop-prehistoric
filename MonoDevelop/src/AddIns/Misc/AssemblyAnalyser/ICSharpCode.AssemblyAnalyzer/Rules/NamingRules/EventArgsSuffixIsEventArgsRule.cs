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
	public class EventArgsSuffixIsEventArgsRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventArgsSuffixIsEventArgs.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventArgsSuffixIsEventArgs.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (typeof(System.EventArgs).IsAssignableFrom(type) && !type.Name.EndsWith("EventArgs")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventArgsSuffixIsEventArgs.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class EventArgsSuffixIsEventArgsRuleTest
	{
		class CorrectEventArgs : System.EventArgs
		{
		}
		[Test]
		public void TestCorrectEventArgs()
		{
			EventArgsSuffixIsEventArgsRule rule = new EventArgsSuffixIsEventArgsRule();
			Assertion.AssertNull(rule.Check(typeof(CorrectEventArgs)));
		}
		
		class IncorrectEventArgsWithWrongSuffix : System.EventArgs
		{
		}
		[Test]
		public void TestIncorrectEventArgs()
		{
			EventArgsSuffixIsEventArgsRule rule = new EventArgsSuffixIsEventArgsRule();
			Assertion.AssertNotNull(rule.Check(typeof(IncorrectEventArgsWithWrongSuffix)));
		}
		
	}
}
#endif
#endregion
