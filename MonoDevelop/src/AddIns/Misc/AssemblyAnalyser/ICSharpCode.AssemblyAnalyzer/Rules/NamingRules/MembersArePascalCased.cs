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
	/// Description of AttributeSuffixIsAttribute.	
	/// </summary>
	public class MembersArePascalCased : AbstractReflectionRule, IMemberRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.MembersArePascalCased.Description}";
			}
		}
		
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.MembersArePascalCased.Details}";
			}
		}
		
		public MembersArePascalCased()
		{
			base.certainty = 99;
		}
		
		public Resolution Check(Module module, MemberInfo member)
		{
			if (member is FieldInfo || member is ConstructorInfo) {
				return null;
			}
			if (member is MethodInfo) {
				MethodInfo mi = (MethodInfo)member;
				if (mi.IsSpecialName) {
					return null;
				}
			}
			
			if (!NamingUtilities.IsPascalCase(member.Name)) {
				return new Resolution(this, 
				                      "${res:MonoDevelop.AssemblyAnalyser.Rules.MembersArePascalCased.Resolution}",
				                      NamingUtilities.Combine(member.DeclaringType.FullName, member.Name),
				                      new string[,] { 
				                      	{"MemberName", member.Name}, 
				                      	{"DeclaringType", member.DeclaringType.FullName}, 
				                      	{"AlternateName", NamingUtilities.PascalCase(member.Name)}});
			}
			return null;
		}
	}
}
