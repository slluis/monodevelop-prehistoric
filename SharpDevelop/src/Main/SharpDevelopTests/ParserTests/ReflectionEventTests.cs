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
	public class ReflectionEventTests
	{
		public             event EventHandler publicEvent;
		private            event EventHandler privateEvent;
		protected          event EventHandler protectedEvent;
		internal           event EventHandler internalEvent;
		internal protected event EventHandler internalProtectedEvent;
		static             event EventHandler staticPrivateEvent;
		
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
		
		[TestMethodAttribute()]
		public void ReflectionEventTest1()
		{
			IEvent evt = new ReflectionEvent(this.GetType().GetEvent("publicEvent", flags), null);
			Assertion.AssertEquals(ModifierEnum.Public, evt.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionEventTest2()
		{
			IEvent evt = new ReflectionEvent(this.GetType().GetEvent("privateEvent", flags), null);
			Assertion.AssertEquals(ModifierEnum.Private, evt.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionEventTest3()
		{
			IEvent evt = new ReflectionEvent(this.GetType().GetEvent("protectedEvent", flags), null);
			Assertion.AssertEquals(ModifierEnum.Protected, evt.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionEventTest4()
		{
			IEvent evt = new ReflectionEvent(this.GetType().GetEvent("internalEvent", flags), null);
			Assertion.AssertEquals(ModifierEnum.Internal, evt.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionEventTest5()
		{
			IEvent evt = new ReflectionEvent(this.GetType().GetEvent("internalProtectedEvent", flags), null);
			Assertion.AssertEquals(ModifierEnum.ProtectedOrInternal, evt.Modifiers);
		}
		
		[TestMethodAttribute()]
		public void ReflectionEventTest6()
		{
			IEvent evt = new ReflectionEvent(this.GetType().GetEvent("staticPrivateEvent", flags), null);
			Assertion.AssertEquals(ModifierEnum.Static | ModifierEnum.Private, evt.Modifiers);
		}	
	}
}
