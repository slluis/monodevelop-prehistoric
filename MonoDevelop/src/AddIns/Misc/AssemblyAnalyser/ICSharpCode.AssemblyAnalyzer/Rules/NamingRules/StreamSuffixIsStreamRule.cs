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
	/// Description of StreamSuffixIsStreamRule.	
	/// </summary>
	public class StreamSuffixIsStreamRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.StreamSuffixIsStream.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.StreamSuffixIsStream.Details}";
			}
		}
		
		public StreamSuffixIsStreamRule()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (typeof(System.IO.Stream).IsAssignableFrom(type) && !type.Name.EndsWith("Stream")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.StreamSuffixIsStream.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class StreamSuffixIsStreamRuleTest
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
			StreamSuffixIsStreamRule rule = new StreamSuffixIsStreamRule();
			Assertion.AssertNull(rule.Check(typeof(MyOtherClass)));
			Assertion.AssertNull(rule.Check(typeof(RealStream)));
		}
		
		class WrongStrm : System.IO.FileStream  
		{
			public WrongStrm(string path,FileMode mode) : base(path, mode)
			{}
		}
		[Test]
		public void TestIncorrectStream()
		{
			StreamSuffixIsStreamRule rule = new StreamSuffixIsStreamRule();
			Assertion.AssertNotNull(rule.Check(typeof(WrongStrm)));
		}
	}
}
#endif
#endregion
