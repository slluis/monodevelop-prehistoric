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
	public class EventSecondParameterNameIsE : AbstractReflectionRule, IEventRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventSecondParameterNameIsE.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.EventSecondParameterNameIsE.Details}";
			}
		}
		
		public EventSecondParameterNameIsE()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(EventInfo evnt)
		{
			MethodInfo invokeMethod = evnt.EventHandlerType.GetMethod("Invoke");
			ParameterInfo[] parameters = invokeMethod.GetParameters();

			if (parameters.Length > 1 && parameters[1].Name != "e") {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.EventSecondParameterNameIsE.Resolution}", evnt.EventHandlerType.FullName, new string[,] { { "EventType", evnt.EventHandlerType.FullName }, { "OldParameterName", parameters[1].Name }});
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
	public class EventSecondParameterNameIsETest
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
			EventSecondParameterNameIsE eventSecondParameterNameIsE = new EventSecondParameterNameIsE();
			Assertion.AssertNull(eventSecondParameterNameIsE.Check(this.GetType().GetEvent("CorrectEvent")));
		}
		
		public delegate void IncorrectEventHandler(object sender, EventArgs notE);
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
			EventSecondParameterNameIsE eventSecondParameterNameIsE = new EventSecondParameterNameIsE();
			Assertion.AssertNotNull(eventSecondParameterNameIsE.Check(this.GetType().GetEvent("IncorrectEvent")));
		}
	}
}
#endif
#endregion
