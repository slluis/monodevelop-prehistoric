// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;

namespace MonoDevelop.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of CheckCollectionSuffix.	
	/// </summary>
	public class CheckCollectionSuffix : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.CheckCollectionSuffix.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.CheckCollectionSuffix.Details}";
			}
		}
		
		public CheckCollectionSuffix()
		{
			base.certainty = 90;
		}
		
		public Resolution Check(Type type)
		{
			if ((typeof(ICollection).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type)) && !typeof(System.Collections.IDictionary).IsAssignableFrom(type)) {
				if (typeof(Queue).IsAssignableFrom(type)) {
					if (!type.Name.EndsWith("Queue")) {
						return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.CheckCollectionSuffix.Resolution1}", type.FullName, new string[,] { { "TypeName", type.FullName }} );
					}
				} else if (typeof(Stack).IsAssignableFrom(type)) {
					if (!type.Name.EndsWith("Stack")) {
						return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.CheckCollectionSuffix.Resolution2}", type.FullName, new string[,] { { "TypeName", type.FullName }});
					}
				} else {
					if (!type.Name.EndsWith("Collection")) {
						return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.CheckCollectionSuffix.Resolution3}", type.FullName, new string[,] { { "TypeName", type.FullName }});
					}
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
	public class CheckCollectionSuffixTest
	{
		#region Collection suffix tests
		class MyCollection : System.Collections.ArrayList
		{
		}
		class MyDictionary : System.Collections.Hashtable
		{
		}
		[Test]
		public void TestCorrectCollection()
		{
			CheckCollectionSuffix checkCollectionSuffix = new CheckCollectionSuffix();
			Assertion.AssertNull(checkCollectionSuffix.Check(typeof(MyCollection)));
			Assertion.AssertNull(checkCollectionSuffix.Check(typeof(MyDictionary)));
			Assertion.AssertNull(checkCollectionSuffix.Check(typeof(CheckCollectionSuffixTest)));
		}
		
		class MyColl : System.Collections.ArrayList
		{
		}
		[Test]
		public void TestIncorrectCollection()
		{
			CheckCollectionSuffix checkCollectionSuffix = new CheckCollectionSuffix();
			Assertion.AssertNotNull(checkCollectionSuffix.Check(typeof(MyColl)));
		}
		#endregion
		
		#region Queue suffix tests
		class MyQueue : System.Collections.Queue
		{
		}
		[Test]
		public void TestCorrectQueue()
		{
			CheckCollectionSuffix checkCollectionSuffix = new CheckCollectionSuffix();
			Assertion.AssertNull(checkCollectionSuffix.Check(typeof(MyQueue)));
		}
		
		class MyQWEQWEQ : System.Collections.Queue
		{
		}
		[Test]
		public void TestIncorrectQueue()
		{
			CheckCollectionSuffix checkCollectionSuffix = new CheckCollectionSuffix();
			Assertion.AssertNotNull(checkCollectionSuffix.Check(typeof(MyQWEQWEQ)));
		}
		#endregion 
		
		#region Stack suffix tests
		class MyStack : System.Collections.Stack
		{
		}
		[Test]
		public void TestCorrectStack()
		{
			CheckCollectionSuffix checkCollectionSuffix = new CheckCollectionSuffix();
			Assertion.AssertNull(checkCollectionSuffix.Check(typeof(MyStack)));
		}
		class MySfwefew : System.Collections.Stack
		{
		}
		[Test]
		public void TestIncorrectStack()
		{
			CheckCollectionSuffix checkCollectionSuffix = new CheckCollectionSuffix();
			Assertion.AssertNotNull(checkCollectionSuffix.Check(typeof(MySfwefew)));
		}
		#endregion
	}
}
#endif
#endregion
