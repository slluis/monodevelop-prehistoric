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
	public class FlagEnumerationsArePlural : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.FlagEnumerationsArePlural.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.FlagEnumerationsArePlural.Details}";
			}
		}
		
		public FlagEnumerationsArePlural()
		{
			base.certainty = 75;
		}
		
		public Resolution Check(Type type)
		{
			if (type.IsSubclassOf(typeof(System.Enum)) && type.IsDefined(typeof(System.FlagsAttribute), true)) {
				if (!type.Name.EndsWith("s") && !type.Name.EndsWith("ae") && !type.Name.EndsWith("i")) {
 					return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.FlagEnumerationsArePlural.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
				}
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
	public class FlagEnumerationsArePluralTest
	{
		[Flags()]
		enum CorrectFlags { a, b, c}
		[Flags()]
		enum CorrectNovae { Type1, Type2}
		[Flags()]
		enum CorrectSpaghetti { Bolognese, Napoli}
		
		[Test]
		public void TestCorrectPluralEnums()
		{
			FlagEnumerationsArePlural elagEnumerationsArePlural = new FlagEnumerationsArePlural();
			Assertion.AssertNull(elagEnumerationsArePlural.Check(typeof(CorrectFlags)));
			Assertion.AssertNull(elagEnumerationsArePlural.Check(typeof(CorrectNovae)));
			Assertion.AssertNull(elagEnumerationsArePlural.Check(typeof(CorrectSpaghetti)));
		}
		
		[Flags()]
		enum SomeFlag { Bolognese, Napoli}
		
		[Test]
		public void TestIncorrectPluralEnums()
		{
			FlagEnumerationsArePlural elagEnumerationsArePlural = new FlagEnumerationsArePlural();
			Assertion.AssertNotNull(elagEnumerationsArePlural.Check(typeof(SomeFlag)));
		}
	}
}
*/
#endif
#endregion
