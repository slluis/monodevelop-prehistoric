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
	public class TypesArePascalCased : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesArePascalCased.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesArePascalCased.Details}";
			}
		}
		
		public TypesArePascalCased()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (!NamingUtilities.IsPascalCase(type.Name)) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesArePascalCased.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }, {"AlternateName", NamingUtilities.PascalCase(type.Name)}});
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
	public class TypesArePascalCasedTest
	{
		interface IInterface
		{
		}
		class AClassImplTest
		{
			
		}
		[Test]
		public void TestCorrectTypenames()
		{
			TypesArePascalCased typesArePascalCased = new TypesArePascalCased();
			Assertion.AssertNull(typesArePascalCased.Check(typeof(IInterface)));
			Assertion.AssertNull(typesArePascalCased.Check(typeof(AClassImplTest)));
		}
		
		class wrong
		{
			
		}
		[Test]
		public void TestIncorrectTypenames()
		{
			TypesArePascalCased typesArePascalCased = new TypesArePascalCased();
			Assertion.AssertNotNull(typesArePascalCased.Check(typeof(wrong)));
		}
	}
}
#endif
#endregion
