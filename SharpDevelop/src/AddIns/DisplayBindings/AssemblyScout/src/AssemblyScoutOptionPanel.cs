// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class AssemblyScoutOptionPanel : AbstractOptionPanel
	{
		Control[] combos = new Control[] {};
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\AssemblyScoutOptions.xfrm"));
			
			combos = new Control[] {ControlDictionary["privateTypesBox"],
											  ControlDictionary["internalTypesBox"],
											  ControlDictionary["privateMembersBox"],
											  ControlDictionary["internalMembersBox"]};
			
			foreach(ComboBox combo in combos) {
				combo.Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.AssemblyScout.Show}"));
				combo.Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.AssemblyScout.GreyOut}"));
				combo.Items.Add(StringParserService.Parse("${res:Dialog.Options.IDEOptions.AssemblyScout.Hide}"));
				
				combo.SelectedIndex = PropertyService.GetProperty("AddIns.AssemblyScout." + combo.Name, 1);
			}
			
			((CheckBox)ControlDictionary["showReturnTypesBox"]).Checked     
				= PropertyService.GetProperty("AddIns.AssemblyScout.ShowReturnTypes", true);
			((CheckBox)ControlDictionary["showResourcePreviewBox"]).Checked 
				= PropertyService.GetProperty("AddIns.AssemblyScout.ShowResPreview", true);
			((CheckBox)ControlDictionary["showSpecialMethodsBox"]).Checked 
				= PropertyService.GetProperty("AddIns.AssemblyScout.ShowSpecialMethods", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("AddIns.AssemblyScout.ShowReturnTypes", ((CheckBox)ControlDictionary["showReturnTypesBox"]).Checked);
			PropertyService.SetProperty("AddIns.AssemblyScout.ShowResPreview",  ((CheckBox)ControlDictionary["showResourcePreviewBox"]).Checked);
			PropertyService.SetProperty("AddIns.AssemblyScout.ShowSpecialMethods", ((CheckBox)ControlDictionary["showSpecialMethodsBox"]).Checked);
			
			foreach(ComboBox combo in combos) {
				PropertyService.SetProperty("AddIns.AssemblyScout." + combo.Name, combo.SelectedIndex);
			}
			
			return true;
		}
	}
}
