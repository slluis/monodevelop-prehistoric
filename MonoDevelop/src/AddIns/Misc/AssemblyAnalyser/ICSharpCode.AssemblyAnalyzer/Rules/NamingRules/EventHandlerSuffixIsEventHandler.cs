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
	/// Description of EventHandlerSuffixIsEventHandler.	
	/// </summary>
	public class EventHandlerSuffixIsEventHandler : AbstractReflectionRule, IEventRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventHandlerSuffixIsEventHandler.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventHandlerSuffixIsEventHandler.Details}";
			}
		}
		
		public Resolution Check(EventInfo evnt)
		{
			if (!evnt.EventHandlerType.Name.EndsWith("EventHandler")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventHandlerSuffixIsEventHandler.Resolution}", evnt.EventHandlerType.FullName, new string[,] { { "EventType", evnt.EventHandlerType.FullName }});
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
	public class EventsDoNotHaveBeforeOrAfterPrefixTest
	{
		public event EventHandler CorrectEvent;
		protected virtual void OnCorrectEvent(EventArgs e)
		{
			if (CorrectEvent != null) {
				CorrectEvent(this, e);
			}
		}
		[Test]
		public void TestCorrectEventHandler()
		{
			EventsDoNotHaveBeforeOrAfterPrefix eventsDoNotHaveBeforeOrAfterPrefix = new EventsDoNotHaveBeforeOrAfterPrefix();
			Assertion.AssertNull(eventsDoNotHaveBeforeOrAfterPrefix.Check(this.GetType().GetEvent("CorrectEvent")));
		}

		public event EventHandler BeforeIncorrectEvent;
		protected virtual void OnBeforeIncorrectEvent(EventArgs e)
		{
			if (BeforeIncorrectEvent != null) {
				BeforeIncorrectEvent(this, e);
			}
		}
		[Test]
		public void TestIncorrectEventHandler1()
		{
			EventsDoNotHaveBeforeOrAfterPrefix eventsDoNotHaveBeforeOrAfterPrefix = new EventsDoNotHaveBeforeOrAfterPrefix();
			Assertion.AssertNotNull(eventsDoNotHaveBeforeOrAfterPrefix.Check(this.GetType().GetEvent("BeforeIncorrectEvent")));
		}
		
		public event EventHandler AfterIncorrectEvent;
		protected virtual void OnAfterIncorrectEvent(EventArgs e)
		{
			if (AfterIncorrectEvent != null) {
				AfterIncorrectEvent(this, e);
			}
		}
		[Test]
		public void TestIncorrectEventHandler2()
		{
			EventsDoNotHaveBeforeOrAfterPrefix eventsDoNotHaveBeforeOrAfterPrefix = new EventsDoNotHaveBeforeOrAfterPrefix();
			Assertion.AssertNotNull(eventsDoNotHaveBeforeOrAfterPrefix.Check(this.GetType().GetEvent("AfterIncorrectEvent")));
		}
		
	}
}
#endif
#endregion
