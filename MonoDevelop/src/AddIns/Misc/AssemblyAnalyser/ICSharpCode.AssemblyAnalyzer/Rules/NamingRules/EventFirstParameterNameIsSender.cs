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
	public class EventFirstParameterNameIsSender : AbstractReflectionRule, IEventRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventFirstParameterNameIsSender.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventFirstParameterNameIsSender.Details}";
			}
		}
		
		public EventFirstParameterNameIsSender()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(EventInfo evnt)
		{
			MethodInfo invokeMethod = evnt.EventHandlerType.GetMethod("Invoke");
			ParameterInfo[] parameters = invokeMethod.GetParameters();

			if (parameters.Length > 0 && parameters[0].Name != "sender") {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventFirstParameterNameIsSender.Resolution}", evnt.EventHandlerType.FullName, new string[,] { { "EventType", evnt.EventHandlerType.FullName }, { "OldParameterName", parameters[0].Name }});
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
	public class EventFirstParameterNameIsSenderTest
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
			EventFirstParameterNameIsSender eventFirstParameterNameIsSender = new EventFirstParameterNameIsSender();
			Assertion.AssertNull(eventFirstParameterNameIsSender.Check(this.GetType().GetEvent("CorrectEvent")));
		}
		
		public delegate void IncorrectEventHandler(object s, EventArgs e);
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
			EventFirstParameterNameIsSender eventFirstParameterNameIsSender = new EventFirstParameterNameIsSender();
			Assertion.AssertNotNull(eventFirstParameterNameIsSender.Check(this.GetType().GetEvent("IncorrectEvent")));
		}
	}
}
*/
#endif
#endregion
