// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

namespace MonoDevelop.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of NamespacesArePascalCased.	
	/// </summary>
	public class NamespacesArePascalCased : AbstractReflectionRule, INamespaceRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.NamespacesArePascalCased.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.NamespacesArePascalCased.Details}";
			}
		}
		
		public NamespacesArePascalCased()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(string namespaceName, ICollection types)
		{
			string[] namespaces = namespaceName.Split('.');
			foreach (string name in namespaces) {
				if (!NamingUtilities.IsPascalCase(name)) {
					for (int i = 0; i < namespaces.Length; ++i) {
						namespaces[i] = NamingUtilities.PascalCase(namespaces[i]);
					}
					return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.NamespacesArePascalCased.Resolution}", namespaceName, new string[,]{{"NamespaceName", namespaceName}, {"AlternateName", String.Join(".", namespaces) }});
				}
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
	public class NamespacesArePascalCasedTest
	{
		[Test]
		public void TestCorrectNamespaces()
		{
			NamespacesArePascalCased namespacesArePascalCased = new NamespacesArePascalCased();
			Assertion.AssertNull("Empty Namespace", namespacesArePascalCased.Check("", null));
			Assertion.AssertNull("Single Namespace", namespacesArePascalCased.Check("MyNamespace", null));
			Assertion.AssertNull("Complex Namespace", namespacesArePascalCased.Check("System.Windows.Form", null));
		}
		
		[Test]
		public void TestIncorrectAttribute()
		{
			NamespacesArePascalCased namespacesArePascalCased = new NamespacesArePascalCased();
			Assertion.AssertNotNull(namespacesArePascalCased.Check("a", null));
			Assertion.AssertNotNull(namespacesArePascalCased.Check("A.Namespace.isWrong", null));
			Assertion.AssertNotNull(namespacesArePascalCased.Check("System.windows.Form", null));
		}
	}
}
#endif
#endregion
