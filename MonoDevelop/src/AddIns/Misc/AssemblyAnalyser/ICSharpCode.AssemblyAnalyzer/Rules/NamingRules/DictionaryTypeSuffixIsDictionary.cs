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
	public class DictionaryTypeSuffixIsDictionary : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.DictionaryTypeSuffixIsDictionary.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.DictionaryTypeSuffixIsDictionary.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (typeof(System.Collections.IDictionary).IsAssignableFrom(type) && !type.Name.EndsWith("Dictionary")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.DictionaryTypeSuffixIsDictionary.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class DictionaryTypeSuffixIsDictionaryTest
	{
		class CorrectDictionary : System.Collections.Hashtable
		{
		}
		[Test]
		public void TestCorrectDictionary()
		{
			DictionaryTypeSuffixIsDictionary dictionaryTypeSuffixIsDictionary = new DictionaryTypeSuffixIsDictionary();
			Assertion.AssertNull(dictionaryTypeSuffixIsDictionary.Check(typeof(CorrectDictionary)));
		}
		
		class IncorrectDictionaryWrongSuffix : System.Collections.Hashtable
		{
		}
		[Test]
		public void TestIncorrectDictionary()
		{
			DictionaryTypeSuffixIsDictionary dictionaryTypeSuffixIsDictionary = new DictionaryTypeSuffixIsDictionary();
			Assertion.AssertNotNull(dictionaryTypeSuffixIsDictionary.Check(typeof(IncorrectDictionaryWrongSuffix)));
		}
		
	}
}
*/
#endif
#endregion
