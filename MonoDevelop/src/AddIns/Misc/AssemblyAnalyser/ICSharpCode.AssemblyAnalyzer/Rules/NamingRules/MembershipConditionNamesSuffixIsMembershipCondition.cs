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
	public class MembershipConditionNamesSuffixIsMembershipCondition : AbstractReflectionRule, ITypeRule
	{
		public override string Description {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.MembershipConditionNamesSuffixIsMembershipCondition.Description}";
			}
		}
		
		// System.Attribute
		public override string Details {
			get {
				return "${res:MonoDevelop.AssemblyAnalyser.Rules.MembershipConditionNamesSuffixIsMembershipCondition.Details}";
			}
		}
		
		public Resolution Check(Type type)
		{
			if (typeof(System.Security.Policy.IMembershipCondition).IsAssignableFrom(type) && !type.Name.EndsWith("MembershipCondition")) {
				return new Resolution(this, "${res:MonoDevelop.AssemblyAnalyser.Rules.MembershipConditionNamesSuffixIsMembershipCondition.Resolution}", type.FullName, new string[,] { { "TypeName", type.FullName }});
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
	public class MembershipConditionNamesSuffixIsMembershipConditionTest
	{
		class MyClass 
		{
		}
		[Test]
		public void TestCorrectMembershipCondition()
		{
			MembershipConditionNamesSuffixIsMembershipCondition membershipConditionNamesSuffixIsMembershipCondition = new MembershipConditionNamesSuffixIsMembershipCondition();
			Assertion.AssertNull(membershipConditionNamesSuffixIsMembershipCondition.Check(typeof(System.Security.Policy.AllMembershipCondition)));
			Assertion.AssertNull(membershipConditionNamesSuffixIsMembershipCondition.Check(typeof(System.Security.Policy.ZoneMembershipCondition)));
			Assertion.AssertNull(membershipConditionNamesSuffixIsMembershipCondition.Check(typeof(MyClass)));
		}
		
		class MyClass2 : System.Security.Policy.IMembershipCondition
		{
			#region System.Security.ISecurityEncodable interface implementation
			public void FromXml(System.Security.SecurityElement e)
			{
				
			}
			
			public System.Security.SecurityElement ToXml()
			{
				return null;
			}
			#endregion
			
			#region System.Security.ISecurityPolicyEncodable interface implementation
			public void FromXml(System.Security.SecurityElement e, System.Security.Policy.PolicyLevel level)
			{
				
			}
			
			public System.Security.SecurityElement ToXml(System.Security.Policy.PolicyLevel level)
			{
				return null;
			}
			#endregion
			
			#region System.Security.Policy.IMembershipCondition interface implementation
			public System.Security.Policy.IMembershipCondition Copy()
			{
				return null;
			}
			
			public bool Check(System.Security.Policy.Evidence evidence)
			{
				return false;
			}
			#endregion
		}
		
		[Test]
		public void TestIncorrectAttribute()
		{
			MembershipConditionNamesSuffixIsMembershipCondition membershipConditionNamesSuffixIsMembershipCondition = new MembershipConditionNamesSuffixIsMembershipCondition();
			Assertion.AssertNotNull(membershipConditionNamesSuffixIsMembershipCondition.Check(typeof(MyClass2)));
		}
	}
}
#endif
#endregion
