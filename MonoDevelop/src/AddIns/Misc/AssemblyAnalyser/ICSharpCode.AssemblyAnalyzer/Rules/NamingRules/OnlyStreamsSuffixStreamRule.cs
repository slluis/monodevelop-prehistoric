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
	/// Description of OnlyStreamsSuffixStreamRule.	
	/// </summary>
	public class OnlyStreamsSuffixStreamRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyStreamsSuffixStream.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyStreamsSuffixStream.Details}";
			}
		}
		
		public OnlyStreamsSuffixStreamRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		
		public Resolution Check(Type type)
		{
			if (!typeof(System.IO.Stream).IsAssignableFrom(type) && type.Name.EndsWith("Stream")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyStreamsSuffixStream.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
			}
			return null;
		}
	}
}
#region Unit Test
#if TEST
namespace MonoDevelop.AssemblyAnalyser.Rules
{
	using System.IO;
	using NUnit.Framework;

	[TestFixture]
	public class OnlyStreamsSuffixStreamRuleTest
	{
		class MyOtherClass
		{
		}
		class RealStream : System.IO.FileStream  
		{
			public RealStream(string path,FileMode mode) : base(path, mode)
			{}
		}
		[Test]
		public void TestCorrectStream()
		{
			OnlyStreamsSuffixStreamRule rule = new OnlyStreamsSuffixStreamRule();
			Assertion.AssertNull(rule.Check(typeof(MyOtherClass)));
			Assertion.AssertNull(rule.Check(typeof(RealStream)));
		}
		
		class MyStream
		{
		}
		[Test]
		public void TestIncorrectStream()
		{
			OnlyStreamsSuffixStreamRule rule = new OnlyStreamsSuffixStreamRule();
			Assertion.AssertNotNull(rule.Check(typeof(MyStream)));
		}
	}
}
#endif
#endregion
