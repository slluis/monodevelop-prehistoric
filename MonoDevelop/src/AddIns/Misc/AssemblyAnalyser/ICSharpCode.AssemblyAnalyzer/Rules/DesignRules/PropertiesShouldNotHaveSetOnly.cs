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
	public class PropertiesShouldNotHaveSetOnly : AbstractReflectionRule, IPropertyRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.PropertiesShouldNotHaveSetOnly.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.PropertiesShouldNotHaveSetOnly.Details}";
			}
		}
		
		public Resolution Check(PropertyInfo property)
		{
			if (!property.CanRead && property.CanWrite) {
				return new Resolution(this, 
				                      "${res:MonoDevelop.AssemblyAnalyser.Rules.PropertiesShouldNotHaveSetOnly.Resolution}",
				                      NamingUtilities.Combine(property.DeclaringType.FullName, property.Name),
				                      new string[,] { 
				                      	{"PropertyName", property.Name}, 
				                      	{"DeclaringType", property.DeclaringType.FullName}, 
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
	public class PropertiesShouldNotHaveSetOnlyTest
	{
		class A {
			public int Inta {
				get {
					return 5;
				}
				set {
					
				}
			}
			public string StrB {
				get {
					return "";
				}
			}
		}
		[Test]
		public void TestCorrectProperties()
		{
			PropertiesShouldNotHaveSetOnly rule = new PropertiesShouldNotHaveSetOnly();
			Assertion.AssertNull(rule.Check(typeof(A).GetProperty("Inta")));
			Assertion.AssertNull(rule.Check(typeof(A).GetProperty("StrB")));
		}
		
		class B {
			public int Inta {
				set {
				}
			}
			public string StrB {
				set {
				}
			}
		}
		
		[Test]
		public void TestIncorrectProperties()
		{
			PropertiesShouldNotHaveSetOnly rule = new PropertiesShouldNotHaveSetOnly();
			Assertion.AssertNotNull(rule.Check(typeof(B).GetProperty("Inta")));
			Assertion.AssertNotNull(rule.Check(typeof(B).GetProperty("StrB")));
		}
	}
}
#endif
#endregion
