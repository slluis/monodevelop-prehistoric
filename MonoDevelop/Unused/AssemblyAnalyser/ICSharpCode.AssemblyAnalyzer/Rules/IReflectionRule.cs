// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;

namespace ICSharpCode.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of IAssemblyRule.	
	/// </summary>
	public interface IReflectionRule : IRule
	{
		ProtectionLevels MemberProtectionLevel {
			get;
		}
		
		ProtectionLevels NestedTypeProtectionLevel {
			get;
		}
		
		ProtectionLevels TypeProtectionLevel {
			get;
		}
		
	}
}
