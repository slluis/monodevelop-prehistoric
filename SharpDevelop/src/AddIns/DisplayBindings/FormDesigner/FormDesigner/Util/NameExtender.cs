// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.SharpDevelop.FormDesigner.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Util
{
	[ProvideProperty("Name", typeof(IComponent))]
	public class NameExtender : IExtenderProvider
	{
		public NameExtender()
		{
		}
		
#region System.ComponentModel.IExtenderProvider interface implementation
		public bool CanExtend(object extendee)
		{
			return extendee is IComponent;
		}
#endregion
		
		[DesignOnly(true)]
		[Category("Design")]
		[Browsable(true)]
		[ParenthesizePropertyName(true)]
		[Description("The name of the component.")]
		public string GetName(IComponent component)
		{
			ISite site = component.Site;
			if (site != null) {
				return site.Name;
			}
			return null;
		}
		
		public void SetName(IComponent component, string name)
		{
			ISite site     = component.Site;
			if (site == null) {
				return;
			}
			INameCreationService nameCreationService = (INameCreationService)site.GetService(typeof(INameCreationService));
			if (!nameCreationService.IsValidName(name)) {
				return;
			}
			
			ComponentChangeService componentChangeService = (ComponentChangeService)site.GetService(typeof(IComponentChangeService));
			
			if (componentChangeService != null) {
				MemberDescriptor md = TypeDescriptor.CreateProperty(component.GetType(), "Name", typeof(string), new Attribute[0]);
				
				componentChangeService.OnComponentChanging(component, md);
				string oldName = site.Name;
				site.Name = name;
				componentChangeService.OnComponentChanged(component, md, oldName, name);
				componentChangeService.OnComponentRename(component, oldName, name);
			}
		}
	}
}
