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
	public class TypeNamesDoNotContainUnderscores : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypeNamesDoNotContainUnderscores.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypeNamesDoNotContainUnderscores.Details}";
			}
		}
		
		public TypeNamesDoNotContainUnderscores()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (NamingUtilities.ContainsUnderscore(type.Name)) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.TypeNamesDoNotContainUnderscores.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class TypeNamesDoNotContainUnderscoresTest
	{
		interface ICorrectInterface
		{
		}
		[Test]
		public void TestCorrectTypenames()
		{
			TypeNamesDoNotContainUnderscores typeNamesDoNotContainUnderscores = new TypeNamesDoNotContainUnderscores();
			Assertion.AssertNull(typeNamesDoNotContainUnderscores.Check(typeof(ICorrectInterface)));
		}
		
		class Wrong_Class
		{
		}
		[Test]
		public void TestIncorrectTypenames()
		{
			TypeNamesDoNotContainUnderscores typeNamesDoNotContainUnderscores = new TypeNamesDoNotContainUnderscores();
			Assertion.AssertNotNull(typeNamesDoNotContainUnderscores.Check(typeof(Wrong_Class)));
		}
	}
}
*/
#endif
#endregion
