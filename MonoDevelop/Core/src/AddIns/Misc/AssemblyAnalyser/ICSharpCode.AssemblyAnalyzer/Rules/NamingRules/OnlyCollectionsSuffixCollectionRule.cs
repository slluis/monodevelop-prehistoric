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
	public class OnlyCollectionsSuffixCollectionRule : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyCollectionsSuffixCollection.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyCollectionsSuffixCollection.Details}";
			}
		}
		
		public OnlyCollectionsSuffixCollectionRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		public Resolution Check(Type type)
		{
			if (!typeof(ICollection).IsAssignableFrom(type) && !typeof(IEnumerable).IsAssignableFrom(type)) {
				if (!typeof(Queue).IsAssignableFrom(type) && type.Name.EndsWith("Queue")) {
					return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyCollectionsSuffixCollection.Resolution1}", type.FullName, new string[,] { { "TypeName", type.FullName }});
				} else if (!typeof(Stack).IsAssignableFrom(type) && type.Name.EndsWith("Stack")) {
					return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyCollectionsSuffixCollection.Resolution2}", type.FullName, new string[,] { { "TypeName", type.FullName }});
				} else if (type.Name.EndsWith("Collection")) {
					return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyCollectionsSuffixCollection.Resolution3}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class OnlyCollectionsSuffixCollectionRuleTest
	{
		#region Collection suffix tests
		class MyCollection : System.Collections.ArrayList
		{
		}
		class OtherClass 
		{
		}
		[Test]
		public void TestCorrectCollection()
		{
			OnlyCollectionsSuffixCollectionRule rule = new OnlyCollectionsSuffixCollectionRule();
			Assertion.AssertNull(rule.Check(typeof(MyCollection)));
			Assertion.AssertNull(rule.Check(typeof(OtherClass)));
		}
		
		class My2Collection
		{
		}
		[Test]
		public void TestIncorrectCollection()
		{
			OnlyCollectionsSuffixCollectionRule rule = new OnlyCollectionsSuffixCollectionRule();
			Assertion.AssertNotNull(rule.Check(typeof(My2Collection)));
		}
		#endregion
		
		#region Queue suffix tests
		class MyQueue : System.Collections.Queue
		{
		}
		[Test]
		public void TestCorrectQueue()
		{
			OnlyCollectionsSuffixCollectionRule rule = new OnlyCollectionsSuffixCollectionRule();
			Assertion.AssertNull(rule.Check(typeof(MyQueue)));
		}
		
		class My2Queue
		{
		}
		[Test]
		public void TestIncorrectQueue()
		{
			OnlyCollectionsSuffixCollectionRule rule = new OnlyCollectionsSuffixCollectionRule();
			Assertion.AssertNotNull(rule.Check(typeof(My2Queue)));
		}
		#endregion 
		
		#region Stack suffix tests
		class MyStack : System.Collections.Stack
		{
		}
		[Test]
		public void TestCorrectStack()
		{
			OnlyCollectionsSuffixCollectionRule rule = new OnlyCollectionsSuffixCollectionRule();
			Assertion.AssertNull(rule.Check(typeof(MyStack)));
		}
		
		class My2Stack
		{
		}
		[Test]
		public void TestIncorrectStack()
		{
			OnlyCollectionsSuffixCollectionRule rule = new OnlyCollectionsSuffixCollectionRule();
			Assertion.AssertNotNull(rule.Check(typeof(My2Stack)));
		}
		#endregion
	}
}
*/
#endif
#endregion
