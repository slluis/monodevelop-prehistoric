// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;

using ICSharpCode.SharpUnit;
using SharpDevelop.Internal.Parser;

namespace SharpDevelopTests
{ 
	class InternalTestClass
	{
		
	}
	
	[TestSuiteAttribute("Test the ReflectionClass class")]
	public class ReflectionClassTests
	{
		[TestMethodAttribute()]
		public void ReflectionClassTest0()
		{
			IClass c = new ReflectionClass(typeof(System.Console), null);
			Assertion.AssertEquals(ClassType.Class, c.ClassType);
			Assertion.AssertEquals(ModifierEnum.Public | ModifierEnum.Sealed, c.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionClassTest01()
		{
			IClass c = new ReflectionClass(typeof(InternalTestClass), null);
			Assertion.AssertEquals(ClassType.Class, c.ClassType);
			Assertion.AssertEquals(ModifierEnum.Internal, c.Modifiers);
		}
		
		
		[TestMethodAttribute()]
		public void ReflectionClassTest1()
		{
			IClass c = new ReflectionClass(typeof(PublicTestClass), null);
			Assertion.AssertEquals(ClassType.Class, c.ClassType);
			Assertion.AssertEquals(ModifierEnum.Public, c.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionClassTest2()
		{
			IClass c = new ReflectionClass(typeof(PrivateTestClass), null);
			Assertion.AssertEquals(ModifierEnum.Private, c.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionClassTest3()
		{
			IClass c = new ReflectionClass(typeof(ProtectedTestClass), null);
			Assertion.AssertEquals(ModifierEnum.Protected, c.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionClassTest4()
		{
			IClass c = new ReflectionClass(typeof(InternalTestClass), null);
			Assertion.AssertEquals(ModifierEnum.Internal, c.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionClassTest5()
		{
			IClass c = new ReflectionClass(typeof(ProtectedInternalTestClass), null);
			Assertion.AssertEquals(ModifierEnum.ProtectedOrInternal, c.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionClassDelegate()
		{
			IClass c = new ReflectionClass(typeof(System.EventHandler), null);
			Assertion.AssertEquals(c.Methods.Count, 1);
		}
		
		
		
		
		public class PublicTestClass
		{
			public int aField;
			
			public int AProperty {
				get {
					return aField;
				}
				set {
					aField = value;
				}
			}
				
			public void AMethod()
			{
			}
			
			public class ANestedClass
			{
			}
			
			public delegate void ADelegate();
			
			public event EventHandler aEvent;
		}
		
		private class PrivateTestClass
		{
			private int aField;
			
			private int AProperty {
				get {
					return aField;
				}
				set {
					aField = value;
				}
			}
				
			private void AMethod()
			{
			}
			
			private class ANestedClass
			{
			}
			
			private delegate void ADelegate();
			
			private event EventHandler aEvent;
		}
	
		
		protected class ProtectedTestClass
		{
			protected int aField;
			
			protected int AProperty {
				get {
					return aField;
				}
				set {
					aField = value;
				}
			}
				
			protected void AMethod()
			{
			}
			
			protected class ANestedClass
			{
			}
			
			protected delegate void ADelegate();
			
			protected event EventHandler aEvent;
		}
	
		
		internal class InternalTestClass
		{
			internal int aField;
			
			internal int AProperty {
				get {
					return aField;
				}
				set {
					aField = value;
				}
			}
				
			internal void AMethod()
			{
			}
			
			internal class ANestedClass
			{
			}
			
			internal delegate void ADelegate();
			
			internal event EventHandler aEvent;
		}
		
		protected internal class ProtectedInternalTestClass
		{
			protected internal int aField;
			
			protected internal int AProperty {
				get {
					return aField;
				}
				set {
					aField = value;
				}
			}
				
			protected internal void AMethod()
			{
			}
			
			protected internal class ANestedClass
			{
			}
			
			protected internal delegate void ADelegate();
			
			protected internal event EventHandler aEvent;
		}
	}
}
