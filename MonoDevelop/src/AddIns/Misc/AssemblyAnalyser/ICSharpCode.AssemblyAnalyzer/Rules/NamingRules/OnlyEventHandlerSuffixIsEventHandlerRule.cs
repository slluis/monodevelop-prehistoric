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
	/// Description of OnlyEventHandlerSuffixIsEventHandlerRule.	
	/// </summary>
	public class OnlyEventHandlerSuffixIsEventHandlerRule : AbstractReflectionRule, IMemberRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyEventHandlerSuffixIsEventHandler.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyEventHandlerSuffixIsEventHandler.Details}";
			}
		}
		
		public OnlyEventHandlerSuffixIsEventHandlerRule()
		{
			base.certainty = 99;
			base.priorityLevel = PriorityLevel.CriticalError;
		}
		
		public Resolution Check(Module module, MemberInfo member)
		{
			if (member is MethodInfo) {
				MethodInfo mi = (MethodInfo)member;
				if (mi.IsSpecialName) {
					return null;
				}
			}
			if (member.Name.EndsWith("EventHandler")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.OnlyEventHandlerSuffixIsEventHandler.Resolution}", NamingUtilities.Combine(member.ReflectedType.FullName, member.Name), new string[,] { {"MemberName", member.Name}, {"DeclaringType", member.DeclaringType.FullName} });
			}
			return null;
		}
	}
}
