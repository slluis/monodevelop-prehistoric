// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class SelectionService : ISelectionService
	{
		IDesignerHost host;
		ArrayList     selectedComponents = new ArrayList();
		
		public object PrimarySelection {
			get {
				if (selectedComponents.Count > 0) {
					return selectedComponents[0];
				}
				return null;
			}
		}
		
		public int SelectionCount {
			get {
				return selectedComponents.Count;
			}
		}
		
		public SelectionService(IDesignerHost host)
		{
			Debug.Assert(host != null);
			this.host = host;
			((IComponentChangeService)host.GetService(typeof(IComponentChangeService))).ComponentRemoved += new ComponentEventHandler(ComponentRemovedHandler);
		}
		
		public bool GetComponentSelected(object component) 
		{
			return selectedComponents.Contains(component);
		}
		
		public ICollection GetSelectedComponents() 
		{
			return selectedComponents.ToArray();
		}
		
		public void SetSelectedComponents(ICollection components, SelectionTypes selectionType) 
		{
			OnSelectionChanging(EventArgs.Empty);
			if (components == null || components.Count == 0) {
				selectedComponents.Clear();
				FireSelectionChange();
				return;
			}
			
			bool controlPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;
			bool shiftPressed   = (Control.ModifierKeys & Keys.Shift)   == Keys.Shift;
			switch (selectionType) {
				case SelectionTypes.Replace:
					ReplaceSelection(components);
					break;
				default:
					if (components.Count == 1 && (controlPressed || shiftPressed)) {
						ToggleSelection(components);
					} else if (controlPressed) {
						AddSelection(components);
					} else if (shiftPressed) {
						ReplaceSelection(components);
					} else {
						NormalSelection(components);
					}
					break;
			}
			selectedComponents.TrimToSize();
			FireSelectionChange();
		}
		
		public void SetSelectedComponents(ICollection components) 
		{
			SetSelectedComponents(components, SelectionTypes.Replace);
		}
		
#region SetSelection helper methods
		void ToggleSelection(ICollection components)
		{
			foreach (object component in components) {
				if (component == null) {
					continue;
				}
				if (GetComponentSelected(component)) {
					selectedComponents.Remove(component);
				} else {
					selectedComponents.Insert(0, component);
				}
			}
		}
		
		void AddSelection(ICollection components)
		{
			foreach (object component in components) {
				if (component == null) {
					continue;
				}
				if (!GetComponentSelected(component)) {
					selectedComponents.Insert(0, component);
				}
			}
		}
		
		void ReplaceSelection(ICollection components)
		{
			selectedComponents.Clear();
			AddSelection(components);
		}
		
		void NormalSelection(ICollection components)
		{
			if (components.Count == 1) {
				// just getting the first == last element in the components, I admit that foreach is ugly but it is one element.
				// If you can make it more elegant please replace this. (I find the GetEnumerator, Movenext, .Current version more uglier ... Mike
				object componentToAdd = null;
				foreach (object component in components) {
					if (component == null) {
						continue;
					}
					componentToAdd = component;
				}
				
				if (componentToAdd != null && GetComponentSelected(componentToAdd)) {
					selectedComponents.Remove(componentToAdd);
					selectedComponents.Insert(0, componentToAdd);
					return;
				} 
			} 
			ReplaceSelection(components);
		}
#endregion
		
#region Event methods
		void ComponentRemovedHandler(object sender, ComponentEventArgs e)
		{
			if (selectedComponents.Contains(e.Component)) {
				OnSelectionChanging(EventArgs.Empty);
				selectedComponents.Remove(e.Component);
				if (selectedComponents.Count == 0 && host.RootComponent != null) {
					selectedComponents.Add(host.RootComponent);
				}
				FireSelectionChange();
			}
		}
		
		void FireSelectionChange()
		{
			OnSelectionChanged(EventArgs.Empty);
			if (host.RootComponent != null) {
				DesignerEventService designerEventService = ((DesignerEventService)host.RootComponent.Site.GetService(typeof(IDesignerEventService)));
				if (designerEventService != null) {
					designerEventService.FileSelectionChanged();
				}
			}
			
		}
		
		protected virtual void OnSelectionChanging(EventArgs e)
		{
			if (SelectionChanging != null) {
				SelectionChanging(this, e);
			}
		}
		
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (SelectionChanged != null) {
				SelectionChanged(this, e);
			}
		}
#endregion
		public event EventHandler SelectionChanging;
		public event EventHandler SelectionChanged;
	}
}
