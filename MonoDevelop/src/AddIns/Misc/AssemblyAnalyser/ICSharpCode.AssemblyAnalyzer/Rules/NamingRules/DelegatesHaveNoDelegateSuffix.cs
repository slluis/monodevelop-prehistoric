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
	public class DelegatesHaveNoDelegateSuffix : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.DelegatesHaveNoDelegateSuffix.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.DelegatesHaveNoDelegateSuffix.Details}";
			}
		}
		
		public DelegatesHaveNoDelegateSuffix()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Type type)
		{
			if (type.IsSubclassOf(typeof(System.Delegate)) && type.Name.EndsWith("Delegate")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.DelegatesHaveNoDelegateSuffix.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class DelegatesHaveNoDelegateSuffixTest
	{
		delegate void MyDelegateWithoutDelegateSuffix();
		[Test]
		public void TestCorrectDelegate()
		{
			DelegatesHaveNoDelegateSuffix delegatesHaveNoDelegateSuffix = new DelegatesHaveNoDelegateSuffix();
			Assertion.AssertNull(delegatesHaveNoDelegateSuffix.Check(typeof(MyDelegateWithoutDelegateSuffix)));
		}
		
		delegate void MyDelegateWithDelegateSuffixDelegate();
		[Test]
		public void TestIncorrectDelegate()
		{
			DelegatesHaveNoDelegateSuffix delegatesHaveNoDelegateSuffix = new DelegatesHaveNoDelegateSuffix();
			Assertion.AssertNotNull(delegatesHaveNoDelegateSuffix.Check(typeof(MyDelegateWithDelegateSuffixDelegate)));
		}
	}
}
#endif
#endregion
