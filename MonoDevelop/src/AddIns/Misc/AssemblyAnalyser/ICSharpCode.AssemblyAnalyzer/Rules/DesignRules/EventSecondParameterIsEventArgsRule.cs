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
	/// Description of EventFirstParameterIsObject.	
	/// </summary>
	public class EventSecondParameterIsEventArgsRule : AbstractReflectionRule, IEventRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventSecondParameterIsEventArgs.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventSecondParameterIsEventArgs.Details}";
			}
		}
		
		public EventSecondParameterIsEventArgsRule()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(EventInfo evnt)
		{
			MethodInfo invokeMethod = evnt.EventHandlerType.GetMethod("Invoke");
			ParameterInfo[] parameters = invokeMethod.GetParameters();

			if (parameters.Length > 1 && !typeof(System.EventArgs).IsAssignableFrom(parameters[1].ParameterType)) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventSecondParameterIsEventArgs.Resolution}", evnt.EventHandlerType.FullName, new string[,] { { "EventType", evnt.EventHandlerType.FullName }, { "OldParameterType", parameters[1].ParameterType.FullName }});
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
	public class EventSecondParameterIsEventArgsRuleTest
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
			EventSecondParameterIsEventArgsRule rule = new EventSecondParameterIsEventArgsRule();
			Assertion.AssertNull(rule.Check(this.GetType().GetEvent("CorrectEvent")));
		}
		
		public delegate void IncorrectEventHandler(object sender, int e);
		public event IncorrectEventHandler IncorrectEvent;
		protected virtual void OnIncorrectEvent(int e)
		{
			if (IncorrectEvent != null) {
				IncorrectEvent(this, e);
			}
		}
		
		[Test]
		public void TestIncorrectEventHandler()
		{
			EventSecondParameterIsEventArgsRule rule = new EventSecondParameterIsEventArgsRule();
			Assertion.AssertNotNull(rule.Check(this.GetType().GetEvent("IncorrectEvent")));
		}
	}
}
#endif
#endregion
