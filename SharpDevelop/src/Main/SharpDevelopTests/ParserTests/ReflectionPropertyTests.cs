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
	public class ReflectionPropertyTests
	{
		public             int publicProperty {get {return 0;} set {} }
		private            int privateProperty {set {} }
		protected          int protectedProperty {get {return 0;} }
		internal           int internalProperty {get {return 0;} set {} }
		internal protected int internalProtectedProperty {get {return 0;} set {} }
		static             int staticPrivateProperty { set {} }
		
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
		
		[TestMethodAttribute()]
		public void ReflectionPropertyTest1()
		{
			IProperty property = new ReflectionProperty(this.GetType().GetProperty("publicProperty", flags), null);
			Assertion.AssertEquals(ModifierEnum.Public, property.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionPropertyTest2()
		{
			IProperty property = new ReflectionProperty(this.GetType().GetProperty("privateProperty", flags), null);
			Assertion.AssertEquals(ModifierEnum.Private, property.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionPropertyTest3()
		{
			IProperty property = new ReflectionProperty(this.GetType().GetProperty("protectedProperty", flags), null);
			Assertion.AssertEquals(ModifierEnum.Protected, property.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionPropertyTest4()
		{
			IProperty property = new ReflectionProperty(this.GetType().GetProperty("internalProperty", flags), null);
			Assertion.AssertEquals(ModifierEnum.Internal, property.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionPropertyTest5()
		{
			IProperty property = new ReflectionProperty(this.GetType().GetProperty("internalProtectedProperty", flags), null);
			Assertion.AssertEquals(ModifierEnum.ProtectedOrInternal, property.Modifiers);
		}
	
		[TestMethodAttribute()]
		public void ReflectionPropertyTest6()
		{
			IProperty property = new ReflectionProperty(this.GetType().GetProperty("staticPrivateProperty", flags), null);
			Assertion.AssertEquals(ModifierEnum.Static | ModifierEnum.Private, property.Modifiers);
		}
		
	}
}
