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
	/// Description of EventFirstParameterNameIsSender.	
	/// </summary>
	public class EventHandlersReturnVoidRule : AbstractReflectionRule, IEventRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventHandlersReturnVoid.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventHandlersReturnVoid.Details}";
			}
		}
		
		public EventHandlersReturnVoidRule()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(EventInfo evnt)
		{
			MethodInfo invokeMethod = evnt.EventHandlerType.GetMethod("Invoke");
			if (invokeMethod.ReturnType != typeof(void)) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventHandlersReturnVoid.Resolution}", evnt.EventHandlerType.FullName, new string[,] { { "EventType", evnt.EventHandlerType.FullName }, {"OldReturnType", invokeMethod.ReturnType.FullName}});
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
	public class EventHandlersReturnVoidRuleTest
	{
		public delegate void CorrectEventHandler(object sender, EventArgs e);
		public event CorrectEventHandler CorrectEvent;
		protected virtual void OnCorrectEvent(EventArgs e)
		{
			if (CorrectEvent != null) {
				CorrectEvent(this, e);
			}
		}
		[Test]
		public void TestCorrectEventHandler()
		{
			EventHandlersReturnVoidRule rule = new EventHandlersReturnVoidRule();
			Assertion.AssertNull(rule.Check(this.GetType().GetEvent("CorrectEvent")));
		}
		
		public delegate int IncorrectEventHandler(object sender, EventArgs e);
		public event IncorrectEventHandler IncorrectEvent;
		protected virtual void OnIncorrectEvent(EventArgs e)
		{
			if (IncorrectEvent != null) {
				IncorrectEvent(this, e);
			}
		}
		
		[Test]
		public void TestIncorrectEventHandler()
		{
			EventHandlersReturnVoidRule rule = new EventHandlersReturnVoidRule();
			Assertion.AssertNotNull(rule.Check(this.GetType().GetEvent("IncorrectEvent")));
		}
	}
}
#endif
#endregion
