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
	/// Description of PropertiesShouldNotHaveSetOnly.	
	/// </summary>
	public class TypesHaveNoPublicInstanceFields : AbstractReflectionRule, IFieldRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesHaveNoPublicInstanceFields.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesHaveNoPublicInstanceFields.Details}";
			}
		}
		
		public TypesHaveNoPublicInstanceFields()
		{
			base.certainty = 90;
		}
		
		public Resolution Check(Module module, FieldInfo field)
		{
			if (!field.IsStatic && (field.IsPublic || field.IsAssembly)) {
				return new Resolution(this, 
				                      "${res:MonoDevelop.AssemblyAnalyser.Rules.TypesHaveNoPublicInstanceFields.Resolution}",
				                      NamingUtilities.Combine(field.DeclaringType.FullName, field.Name),
				                      new string[,] {
				                      	{"FieldName", field.Name}, 
				                      	{"DeclaringType", field.DeclaringType.FullName}, 
				                      });
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
	public class TypesHaveNoPublicInstanceFieldsTest
	{
		class A {
			int a;
			protected string b;
			public static int c = 12;
			public int AA {
				get {
					return a;
				}
				set {
					a = value;
				}
			}
			public string B {
				get {
					return b;
				}
				set {
					b = value;
				}
			}
		}
		[Test]
		public void TestCorrectFields()
		{
			TypesHaveNoPublicInstanceFields rule = new TypesHaveNoPublicInstanceFields();
			Assertion.AssertNull(rule.Check(null, typeof(A).GetField("a", BindingFlags.NonPublic | BindingFlags.Instance)));
			Assertion.AssertNull(rule.Check(null, typeof(A).GetField("b", BindingFlags.NonPublic | BindingFlags.Instance)));
			Assertion.AssertNull(rule.Check(null, typeof(A).GetField("c", BindingFlags.Public | BindingFlags.Static)));
		}
		
		class B {
			public int a = 5;
			internal string b ="";
		}
		
		[Test]
		public void TestIncorrectFields()
		{
			TypesHaveNoPublicInstanceFields rule = new TypesHaveNoPublicInstanceFields();
			Assertion.AssertNotNull(rule.Check(null, typeof(B).GetField("a", BindingFlags.Public | BindingFlags.Instance)));
			Assertion.AssertNotNull(rule.Check(null, typeof(B).GetField("b", BindingFlags.NonPublic | BindingFlags.Instance)));
		}
	}
}
#endif
#endregion
