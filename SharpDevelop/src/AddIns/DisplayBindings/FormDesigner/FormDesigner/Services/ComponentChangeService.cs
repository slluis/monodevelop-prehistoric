// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class ComponentChangeService : IComponentChangeService
	{
		public void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
		{
			if (ComponentChanged != null) {
				try {
					ComponentChanged(this, new ComponentChangedEventArgs(component, member, oldValue, newValue));
				} catch (Exception) {}
			}
		}
		
		public void OnComponentChanging(object component, MemberDescriptor member)
		{
			if (ComponentChanging != null) {
				ComponentChanging(this, new ComponentChangingEventArgs(component, member));
			}
		}
		
		public void OnComponentAdded(IComponent component)
		{
			if (ComponentAdded != null) {
				ComponentAdded(this, new ComponentEventArgs(component));
			}
		}
		
		public void OnComponentAdding(IComponent component)
		{
			if (ComponentAdding != null) {
				ComponentAdding(this, new ComponentEventArgs(component));
			}
		}
		
		public void OnComponentRemoved(IComponent component)
		{
			if (ComponentRemoved != null) {
				ComponentRemoved(this, new ComponentEventArgs(component));
			}
		}
		
		public void OnComponentRemoving(IComponent component)
		{
			if (ComponentRemoving != null) {
				ComponentRemoving(this, new ComponentEventArgs(component));
			}
		}
		
		public void OnComponentRename(object component, string oldName, string newName)
		{
			if (ComponentRename != null) {
				ComponentRename(this, new ComponentRenameEventArgs(component, oldName, newName));
			}
		}
#region IComponentChangeService implementation		
		public event ComponentEventHandler         ComponentAdded;
		public event ComponentEventHandler         ComponentAdding;
		public event ComponentChangedEventHandler  ComponentChanged;
		public event ComponentChangingEventHandler ComponentChanging;
		public event ComponentEventHandler         ComponentRemoved;
		public event ComponentEventHandler         ComponentRemoving;
		public event ComponentRenameEventHandler   ComponentRename;
#endregion
	}
}
