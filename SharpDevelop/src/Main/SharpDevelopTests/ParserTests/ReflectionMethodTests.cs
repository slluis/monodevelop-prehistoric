// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Reflection;

using ICSharpCode.SharpUnit;
using SharpDevelop.Internal.Parser;

namespace SharpDevelopTests
{ 
	[TestSuiteAttribute()]
	public class ReflectionMethodTests
	{
		public             void publicMethod() {}
		private            void privateMethod() {}
		protected          void protectedMethod() {}
		internal           void internalMethod() {}
		internal protected void internalProtectedMethod() {}
		static             void staticPrivateMethod() {}
		
		
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
		
		[TestMethodAttribute()]
		public void ReflectionMethodTest1()
		{
			IMethod method = new ReflectionMethod(this.GetType().GetMethod("publicMethod", flags), null);
			Assertion.AssertEquals(ModifierEnum.Public, method.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionMethodTest2()
		{
			IMethod method = new ReflectionMethod(this.GetType().GetMethod("privateMethod", flags), null);
			Assertion.AssertEquals(ModifierEnum.Private, method.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionMethodTest3()
		{
			IMethod method = new ReflectionMethod(this.GetType().GetMethod("protectedMethod", flags), null);
			Assertion.AssertEquals(ModifierEnum.Protected, method.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionMethodTest4()
		{
			IMethod method = new ReflectionMethod(this.GetType().GetMethod("internalMethod", flags), null);
			Assertion.AssertEquals(ModifierEnum.Internal, method.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionMethodTest5()
		{
			IMethod method = new ReflectionMethod(this.GetType().GetMethod("internalProtectedMethod", flags), null);
			Assertion.AssertEquals(ModifierEnum.ProtectedOrInternal, method.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionMethodTest6()
		{
			IMethod method = new ReflectionMethod(this.GetType().GetMethod("staticPrivateMethod", flags), null);
			Assertion.AssertEquals(ModifierEnum.Static | ModifierEnum.Private, method.Modifiers);
		}
	}
}
