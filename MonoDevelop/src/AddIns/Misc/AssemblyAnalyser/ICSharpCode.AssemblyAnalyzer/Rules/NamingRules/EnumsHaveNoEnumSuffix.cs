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
	/// Description of DelegatesHaveNoDelegateSuffix.	
	/// </summary>
	public class EnumsHaveNoEnumSuffix : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EnumsHaveNoEnumSuffix.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EnumsHaveNoEnumSuffix.Details}";
			}
		}
		
		public EnumsHaveNoEnumSuffix()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (type.IsSubclassOf(typeof(System.Enum)) && type.Name.EndsWith("Enum")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EnumsHaveNoEnumSuffix.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class EnumsHaveNoEnumSuffixTest
	{
		enum CorrectEnumWithAnotherSuffix
		{
			A, B, C
		}
		[Test]
		public void TestCorrectEnum()
		{
			EnumsHaveNoEnumSuffix enumsHaveNoEnumSuffix = new EnumsHaveNoEnumSuffix();
			Assertion.AssertNull(enumsHaveNoEnumSuffix.Check(typeof(CorrectEnumWithAnotherSuffix)));
		}
		
		enum IncorrectEnum
		{
			A, B, C
		}
		[Test]
		public void TestIncorrectDictionary()
		{
			EnumsHaveNoEnumSuffix enumsHaveNoEnumSuffix = new EnumsHaveNoEnumSuffix();
			Assertion.AssertNotNull(enumsHaveNoEnumSuffix.Check(typeof(IncorrectEnum)));
		}
		
	}
}
*/
#endif
#endregion
