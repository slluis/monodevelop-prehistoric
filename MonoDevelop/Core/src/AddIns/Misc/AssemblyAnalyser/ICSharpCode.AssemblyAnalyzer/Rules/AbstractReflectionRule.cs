// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace MonoDevelop.AssemblyAnalyser.Rules
{
	/// <summary>
	/// Description of AbstractReflectionRule.	
	/// </summary>
	public abstract class AbstractReflectionRule : AbstractRule, MonoDevelop.AssemblyAnalyser.Rules.IReflectionRule
	{
		protected ProtectionLevels memberProtectionLevel     = ProtectionLevels.All;
		protected ProtectionLevels nestedTypeProtectionLevel = ProtectionLevels.All;
		protected ProtectionLevels typeProtectionLevel       = ProtectionLevels.All;
		
		#region MonoDevelop.AssemblyAnalyser.Rules.IReflectionRule interface implementation
		public virtual ProtectionLevels MemberProtectionLevel {
			get {
				return memberProtectionLevel;
			}
		}
		
		public virtual ProtectionLevels NestedTypeProtectionLevel {
			get {
				return nestedTypeProtectionLevel;
			}
		}
		
		public virtual ProtectionLevels TypeProtectionLevel {
			get {
				return typeProtectionLevel;
			}
		}
		#endregion
	}
}
