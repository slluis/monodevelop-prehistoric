// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;


using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;

namespace MonoDevelop.Core.AddIns
{
	[ConditionAttribute()]
	public class CombineOpenCondition : AbstractCondition
	{
		[XmlMemberAttribute("iscombineopen", IsRequired = true)]
		bool isCombineOpen;
		
		public bool IsCombineOpen {
			get {
				return isCombineOpen;
			}
			set {
				isCombineOpen = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			return projectService.CurrentOpenCombine != null || !isCombineOpen;
		}
	}
}
