// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;


namespace ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels
{
	public class GridOptionsPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\WindowsFormsGridOptions.xfrm"));
			ControlDictionary["widthTextBox"].Text                      = PropertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeWidth", 8).ToString();
			ControlDictionary["heightTextBox"].Text                     = PropertyService.GetProperty("FormsDesigner.DesignerOptions.GridSizeHeight", 8).ToString();
			((CheckBox)ControlDictionary["showGridCheckBox"]).Checked   = PropertyService.GetProperty("FormsDesigner.DesignerOptions.ShowGrid", true);
			((CheckBox)ControlDictionary["snapToGridCheckBox"]).Checked = PropertyService.GetProperty("FormsDesigner.DesignerOptions.SnapToGrid", true);
			((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked = PropertyService.GetProperty("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
		}
		
		public override bool StorePanelContents()
		{
			int width = 0;
			try {
				width = Int32.Parse(ControlDictionary["widthTextBox"].Text);
			} catch (Exception) {
				MessageService.ShowError("Forms Designer grid with is invalid");
				return false;
			}
			
			int height = 0;
			try {
				height = Int32.Parse(ControlDictionary["heightTextBox"].Text);
			} catch (Exception) {
				MessageService.ShowError("Forms Designer height with is invalid");
				return false;
			}
			
			PropertyService.SetProperty("FormsDesigner.DesignerOptions.GridSizeWidth", width);
			PropertyService.SetProperty("FormsDesigner.DesignerOptions.GridSizeHeight", height);
			PropertyService.SetProperty("FormsDesigner.DesignerOptions.ShowGrid", ((CheckBox)ControlDictionary["showGridCheckBox"]).Checked);
			PropertyService.SetProperty("FormsDesigner.DesignerOptions.SnapToGrid", ((CheckBox)ControlDictionary["snapToGridCheckBox"]).Checked);
			PropertyService.SetProperty("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", ((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked);
			
			return true;
		}
	}
}
