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
	[TestSuiteAttribute("Test the ReflectionClass class")]
	public class ReflectionFieldTests
	{
		public             int publicField;
		private            int privateField;
		protected          int protectedField;
		internal           int internalField;
		internal protected int internalProtectedField;
		static             int staticPrivateField;
		
		[TestMethodAttribute()]
		public void ReflectionFieldTest1()
		{
			IField field = new ReflectionField(this.GetType().GetField("publicField"), null);
			Assertion.AssertEquals(ModifierEnum.Public, field.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionFieldTest2()
		{
			IField field = new ReflectionField(this.GetType().GetField("privateField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public), null);
			Assertion.AssertEquals(ModifierEnum.Private, field.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionFieldTest3()
		{
			IField field = new ReflectionField(this.GetType().GetField("protectedField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public), null);
			Assertion.AssertEquals(ModifierEnum.Protected, field.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionFieldTest4()
		{
			IField field = new ReflectionField(this.GetType().GetField("internalField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public), null);
			Assertion.AssertEquals(ModifierEnum.Internal, field.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionFieldTest5()
		{
			IField field = new ReflectionField(this.GetType().GetField("internalProtectedField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public), null);
			Assertion.AssertEquals(ModifierEnum.ProtectedOrInternal, field.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionFieldTest6()
		{
			IField field = new ReflectionField(this.GetType().GetField("staticPrivateField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public), null);
			Assertion.AssertEquals(ModifierEnum.Static | ModifierEnum.Private, field.Modifiers);
		}
	}
}
