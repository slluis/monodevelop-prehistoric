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
	/// Description of EventsDoNotHaveBeforeOrAfterPrefix.	
	/// </summary>
	public class EventsDoNotHaveBeforeOrAfterPrefix : AbstractReflectionRule, IEventRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventsDoNotHaveBeforeOrAfterPrefix.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventsDoNotHaveBeforeOrAfterPrefix.Details}";
			}
		}
		
		public EventsDoNotHaveBeforeOrAfterPrefix()
		{
			base.certainty = 90;
		}
		
		public Resolution Check(EventInfo evnt)
		{
			if (evnt.Name.StartsWith("Before")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventsDoNotHaveBeforeOrAfterPrefix.Resolution1}", NamingUtilities.Combine(evnt.ReflectedType.FullName, evnt.Name), new string[,] { { "EventName", evnt.Name }, { "ReflectedType", evnt.ReflectedType.FullName }});
			} else if (evnt.Name.StartsWith("After")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventsDoNotHaveBeforeOrAfterPrefix.Resolution2}", NamingUtilities.Combine(evnt.ReflectedType.FullName, evnt.Name), new string[,] { { "EventName", evnt.Name }, { "ReflectedType", evnt.ReflectedType.FullName }});	
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
	public class EventHandlerSuffixIsEventHandlerTest
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
			EventHandlerSuffixIsEventHandler eventHandlerSuffixIsEventHandler = new EventHandlerSuffixIsEventHandler();
			Assertion.AssertNull(eventHandlerSuffixIsEventHandler.Check(this.GetType().GetEvent("CorrectEvent")));
		}
		
		public delegate void IncorrectEventHandlerWithWrongSuffix(object sender, EventArgs e);
		public event IncorrectEventHandlerWithWrongSuffix IncorrectEvent;
		protected virtual void OnIncorrectEvent(EventArgs e)
		{
			if (IncorrectEvent != null) {
				IncorrectEvent(this, e);
			}
		}
		
		[Test]
		public void TestIncorrectEventHandler()
		{
			EventHandlerSuffixIsEventHandler eventHandlerSuffixIsEventHandler = new EventHandlerSuffixIsEventHandler();
			EventInfo evnt = this.GetType().GetEvent("IncorrectEvent");
			Assertion.AssertNotNull("Type name is >" + evnt.EventHandlerType.FullName + "<", eventHandlerSuffixIsEventHandler.Check(evnt));
		}
	}
}
#endif
#endregion
