// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Xml;

using MonoDevelop.Gui.Dialogs.OptionPanels;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui.XmlForms;

namespace MonoDevelop.Gui.Dialogs
{/*
	/// <summary>
	/// Basic "tabbed" options dialog
	/// </summary>
	public class TabbedOptions : BaseSharpDevelopForm
	{
		ArrayList OptionPanels = new ArrayList();
		IProperties properties = null;
		
		void AcceptEvent(object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void AddOptionPanels(ArrayList dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = properties;
					descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
					descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Activated);
					OptionPanels.Add(descriptor.DialogPanel);
					
					TabPage page = new TabPage(descriptor.Label);
					page.Controls.Add(descriptor.DialogPanel.Control);
					((TabControl)ControlDictionary["optionPanelTabControl"]).TabPages.Add(page);
				}
				
				if (descriptor.DialogPanelDescriptors != null) {
					AddOptionPanels(descriptor.DialogPanelDescriptors);
				}
			}
		}
		
		public TabbedOptions(string dialogName, IProperties properties, IAddInTreeNode node)
		{
			this.properties = properties;
			
			base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\TabbedOptionsDialog.xfrm"));
			this.Text       = dialogName;
			ControlDictionary["okButton"].Click += new EventHandler(AcceptEvent);
			Icon = null;
			Owner = (Form)WorkbenchSingleton.Workbench;
			AddOptionPanels(node.BuildChildItems(this));
		}
	}*/
}