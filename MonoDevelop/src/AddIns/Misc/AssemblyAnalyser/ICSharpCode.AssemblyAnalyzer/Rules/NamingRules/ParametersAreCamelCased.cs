// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections;

namespace MonoDevelop.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of NamespacesArePascalCased.	
	/// </summary>
	public class ParametersAreCamelCased : AbstractReflectionRule, IParameterRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.ParametersAreCamelCased.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.ParametersAreCamelCased.Details}";
			}
		}
		
		public ParametersAreCamelCased()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Module module, ParameterInfo param)
		{
			if (!NamingUtilities.IsCamelCase(param.Name)) {
				string memberName = NamingUtilities.Combine(param.Member.DeclaringType.FullName, param.Member.Name);
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.ParametersAreCamelCased.Resolution}", memberName, new string[,] {{"ParameterName", param.Name}, {"MemberName", memberName}, {"AlternateName", NamingUtilities.CamelCase(param.Name)}});
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
	public class ParametersAreCamelCasedTest
	{
		public class A {
			public void TestMethod1(int right)
			{
			}
			public void TestMethod2(int a, int b, int c, int d)
			{
			}
			public void TestMethod3(int Wrong)
			{
			}
			public void TestMethod4(int A, int B, int C, int D)
			{
			}
			public static void TestMethod(MethodInfo methodInfo, bool isNull)
			{
				ParametersAreCamelCased parametersAreCamelCased = new ParametersAreCamelCased();
				foreach (ParameterInfo parameter in methodInfo.GetParameters()) {
					if (isNull) {
						Assertion.AssertNull(parametersAreCamelCased.Check(null, parameter));
					} else {
						Assertion.AssertNotNull(parametersAreCamelCased.Check(null, parameter));
					}
				}
			}
		}
		
		
		[Test]
		public void TestCorrectParameters()
		{
			A.TestMethod(typeof(A).GetMethod("TestMethod1"), true);
			A.TestMethod(typeof(A).GetMethod("TestMethod2"), true);
		}
		
		[Test]
		public void TestIncorrectParameters()
		{
			A.TestMethod(typeof(A).GetMethod("TestMethod3"), false);
			A.TestMethod(typeof(A).GetMethod("TestMethod4"), false);
		}
	}
}
*/
#endif
#endregion
