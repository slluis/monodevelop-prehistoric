// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using ICSharpCode.SharpDevelop.Gui.Pads;

using ICSharpCode.SharpDevelop.Gui.ErrorDialogs;
using ICSharpCode.SharpDevelop.FormDesigner.Gui;

using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;

namespace ICSharpCode.SharpDevelop.FormDesigner {
	
	public class ComponentRemovedUndoAction : ICSharpCode.SharpDevelop.Internal.Undo.IUndoableOperation
	{
		IDesignerHost host;
		
		Type   componentType;
		string componentName;
		string parent;
		IComponent oldComponent;
		
		public ComponentRemovedUndoAction(IDesignerHost host, ComponentEventArgs cea, string parent)
		{
			this.host     = host;
			oldComponent  = cea.Component;
			componentName = cea.Component.Site.Name;
			componentType = cea.Component.GetType();
			this.parent = parent;
		}
		
		public void Undo()
		{
			IComponent component = host.CreateComponent(componentType, componentName);
			if (parent != null && parent.Length > 0) {
				IComponent parentCtrl = host.Container.Components[parent];
				((Control)parentCtrl).Controls.Add((Control)component);
			}
			
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(oldComponent.GetType());
			foreach (PropertyDescriptor pd in properties) {
				try {
					if (pd.ShouldSerializeValue(oldComponent) && pd.IsBrowsable) {
						if (pd.Name == "Visible") {
							continue;
						}
						pd.SetValue(component, pd.GetValue(oldComponent));
					}
				} catch (Exception) {
				}
			}
		}
		
		public void Redo()
		{
			host.DestroyComponent(host.Container.Components[componentName]);
		}
	}
}
