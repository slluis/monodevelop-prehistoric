// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace VBBinding
{
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters = null;
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null) {
					return true;
				}
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				
				if (ControlDictionary["win32IconTextBox"].Text.Length > 0) {
					if (!fileUtilityService.IsValidFileName(ControlDictionary["win32IconTextBox"].Text)) {
						MessageBox.Show("Invalid Win32Icon specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
					if (!File.Exists(ControlDictionary["win32IconTextBox"].Text)) {
						MessageBox.Show("Win32Icon doesn't exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
				}
											
				compilerParameters.CompileTarget = (CompileTarget)((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex;
				compilerParameters.DefineSymbols = ControlDictionary["symbolsTextBox"].Text;
				compilerParameters.MainClass     = ControlDictionary["mainClassTextBox"].Text;
				compilerParameters.Win32Icon     = ControlDictionary["win32IconTextBox"].Text;
				compilerParameters.Imports       = ControlDictionary["importsTextBox"].Text;
				compilerParameters.RootNamespace = ControlDictionary["RootNamespaceTextBox"].Text;
				
				compilerParameters.Debugmode = ((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked;
				compilerParameters.Optimize = ((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked;
				compilerParameters.GenerateOverflowChecks = ((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked;
				compilerParameters.RunWithWarnings  = !((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex = (int)compilerParameters.CompileTarget;
			ControlDictionary["symbolsTextBox"].Text   = compilerParameters.DefineSymbols;
			ControlDictionary["mainClassTextBox"].Text = compilerParameters.MainClass;
			ControlDictionary["win32IconTextBox"].Text = compilerParameters.Win32Icon;
			ControlDictionary["importsTextBox"].Text = compilerParameters.Imports;
			ControlDictionary["RootNamespaceTextBox"].Text = compilerParameters.RootNamespace;
			
			
			((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked = compilerParameters.Debugmode;
			((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked       = compilerParameters.Optimize;
			((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked       = compilerParameters.GenerateOverflowChecks;
			((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked       = !compilerParameters.RunWithWarnings;
			
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public CodeGenerationPanel() : base(propertyService.DataDirectory + @"\resources\panels\ProjectOptions\VBNetCodeGenerationPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Library"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Module"));
			
			ControlDictionary["browseWin32IconButton"].Click += new EventHandler(SelectWin32Icon);
		}
		
		void SelectWin32Icon(object sender, EventArgs e) 
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter          = "Icon|*.ico|All files (*.*)|*.*";
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["win32IconTextBox"].Text = fdiag.FileName;
				}
			}
		}
	}
}
