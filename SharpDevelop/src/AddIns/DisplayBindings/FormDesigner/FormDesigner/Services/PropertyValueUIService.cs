//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class PropertyValueUIService : IPropertyValueUIService
	{
		PropertyValueUIHandler propertyValueUIHandler;

		public void AddPropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			propertyValueUIHandler += newHandler;
		}

		public PropertyValueUIItem[] GetPropertyUIValueItems(ITypeDescriptorContext context, PropertyDescriptor propDesc)
		{
			// Let registered handlers have a chance to add their UIItems
			ArrayList propUIValues = new ArrayList();
			if (propertyValueUIHandler != null) {
				propertyValueUIHandler(context,propDesc,propUIValues);
			}
			PropertyValueUIItem[] values = new PropertyValueUIItem[propUIValues.Count];
			if (propUIValues.Count > 0) {
				propUIValues.CopyTo(values);
			}
			return values;
		}

		public void NotifyPropertyValueUIItemsChanged()
		{
			OnPropertyUIValueItemsChanged(EventArgs.Empty);
		}

		public void RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			propertyValueUIHandler -= newHandler;
		}

		protected virtual void OnPropertyUIValueItemsChanged(EventArgs e)
		{
			if (PropertyUIValueItemsChanged != null) {
				PropertyUIValueItemsChanged(this, e);
			}
		}

		public event EventHandler PropertyUIValueItemsChanged;
	}
}
