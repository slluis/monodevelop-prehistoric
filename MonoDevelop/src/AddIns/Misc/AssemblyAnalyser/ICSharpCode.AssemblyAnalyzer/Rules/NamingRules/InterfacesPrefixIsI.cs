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
	public class InterfacesPrefixIsI : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.InterfacesPrefixIsI.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.InterfacesPrefixIsI.Details}";
			}
		}
		
		public InterfacesPrefixIsI()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (type.IsInterface && !type.Name.StartsWith("I")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.InterfacesPrefixIsI.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class InterfacesPrefixIsITest
	{
		interface ICorrectInterface
		{
		}
		[Test]
		public void TestCorrectAttribute()
		{
			InterfacesPrefixIsI interfacesPrefixIsI = new InterfacesPrefixIsI();
			Assertion.AssertNull(interfacesPrefixIsI.Check(typeof(System.ICloneable)));
			Assertion.AssertNull(interfacesPrefixIsI.Check(typeof(System.IComparable)));
			Assertion.AssertNull(interfacesPrefixIsI.Check(typeof(ICorrectInterface)));
		}
		
		interface WrongInterface
		{
		}
		[Test]
		public void TestIncorrectAttribute()
		{
			InterfacesPrefixIsI interfacesPrefixIsI = new InterfacesPrefixIsI();
			Assertion.AssertNotNull(interfacesPrefixIsI.Check(typeof(WrongInterface)));
		}
	}
}
*/
#endif
#endregion
