//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class ReferenceService : IReferenceService
	{
		IDesignerHost host;

		public ReferenceService(IDesignerHost host)
		{
			this.host = host;
		}

#region System.ComponentModel.Design.IReferenceService interface implementation
		public object[] GetReferences(System.Type baseType)
		{
			ArrayList components = new ArrayList();
			foreach (object o in host.Container.Components) {
				if (baseType.IsInstanceOfType(o)) {
					components.Add(o);
				}
			}
			return components.ToArray();
		}

		public object[] GetReferences()
		{
			IComponent[] references = new IComponent[host.Container.Components.Count];
			host.Container.Components.CopyTo(references, 0);
			return references;
		}

		public string GetName(object reference)
		{
			IComponent comp = reference as IComponent;
			if (comp != null && comp.Site != null) {
				return comp.Site.Name;
			}
			return null;
		}

		public object GetReference(string name)
		{
			foreach (IComponent component in host.Container.Components) {
				if (component.Site.Name == name) {
					return component;
				}
			}
			return null;
		}

		public System.ComponentModel.IComponent GetComponent(object reference)
		{
			IComponent comp = reference as IComponent;
			return comp;
		}
#endregion

	}
}
