// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class TypeDescriptorFilterService : ITypeDescriptorFilterService
	{
		IDesignerFilter GetDesignerFilter(IComponent component)
		{
			ISite site = component.Site;
			
			if (site == null) {
				return null;
			}
			
			IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
			return host.GetDesigner(component) as IDesignerFilter;
		}
		

#region System.ComponentModel.Design.ITypeDescriptorFilterService interface implementation
		public bool FilterProperties(System.ComponentModel.IComponent component, System.Collections.IDictionary properties)
		{
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if (designerFilter != null) {
				designerFilter.PreFilterProperties(properties);
				designerFilter.PostFilterProperties(properties);
			}
			return false;
		}
		
		public bool FilterEvents(System.ComponentModel.IComponent component, System.Collections.IDictionary events)
		{
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if (designerFilter != null) {
				designerFilter.PreFilterEvents(events);
				designerFilter.PostFilterEvents(events);
			}
			return false;
		}
		
		public bool FilterAttributes(System.ComponentModel.IComponent component, System.Collections.IDictionary attributes)
		{
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if (designerFilter != null) {
				designerFilter.PreFilterAttributes(attributes);
				designerFilter.PostFilterAttributes(attributes);
			}
			return false;
		}
#endregion
	}
}
