// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
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
	public class OutputOptionsPanel : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null) {
					return true;
				}
				
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (!fileUtilityService.IsValidFileName(ControlDictionary["assemblyNameTextBox"].Text)) {
					MessageBox.Show("Invalid assembly name specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				if (!fileUtilityService.IsValidFileName(ControlDictionary["outputDirectoryTextBox"].Text)) {
					MessageBox.Show("Invalid output directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				
				compilerParameters.OutputAssembly  = ControlDictionary["assemblyNameTextBox"].Text;
				compilerParameters.OutputDirectory = ControlDictionary["outputDirectoryTextBox"].Text;
				compilerParameters.CommandLineParameters = ControlDictionary["parametersTextBox"].Text;
				compilerParameters.ExecuteBeforeBuild    = ControlDictionary["executeBeforeTextBox"].Text;
				compilerParameters.ExecuteAfterBuild     = ControlDictionary["executeAfterTextBox"].Text;
				compilerParameters.ExecuteScript         = ControlDictionary["executeScriptTextBox"].Text;
				
				compilerParameters.PauseConsoleOutput = ((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked;
			}
			return true;
		}
	
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			ControlDictionary["assemblyNameTextBox"].Text = compilerParameters.OutputAssembly;
			ControlDictionary["outputDirectoryTextBox"].Text = compilerParameters.OutputDirectory;
			ControlDictionary["parametersTextBox"].Text = compilerParameters.CommandLineParameters;
			ControlDictionary["executeBeforeTextBox"].Text   = compilerParameters.ExecuteBeforeBuild;
			ControlDictionary["executeAfterTextBox"].Text    = compilerParameters.ExecuteAfterBuild;
			ControlDictionary["executeScriptTextBox"].Text   = compilerParameters.ExecuteScript;
			
			((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked = compilerParameters.PauseConsoleOutput;
		}
		
		void SelectFolder(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			
			if (fdiag.DisplayDialog("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}") == DialogResult.OK) {
				ControlDictionary["outputDirectoryTextBox"].Text = fdiag.Path;
			}
		}
		
		void SelectFile2(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = stringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog() == DialogResult.OK) {
				ControlDictionary["executeBeforeTextBox"].Text = fdiag.FileName;
			}
		}
		
		void SelectFile3(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = stringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog() == DialogResult.OK) {
				ControlDictionary["executeAfterTextBox"].Text = fdiag.FileName;
			}
		}
		void SelectFile4(object sender, EventArgs e)
		{
			OpenFileDialog fdiag = new OpenFileDialog();
			fdiag.Filter      = stringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.Multiselect = false;
			
			if(fdiag.ShowDialog() == DialogResult.OK) {
				ControlDictionary["executeScriptTextBox"].Text = fdiag.FileName;
			}
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public OutputOptionsPanel() : base(propertyService.DataDirectory + @"\resources\panels\ProjectOptions\OutputPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
			ControlDictionary["browseButton"].Click += new EventHandler(SelectFolder);
			ControlDictionary["browseButton2"].Click += new EventHandler(SelectFile2);
			ControlDictionary["browseButton3"].Click += new EventHandler(SelectFile3);
			ControlDictionary["browseButton4"].Click += new EventHandler(SelectFile4);
		}
	}

}
