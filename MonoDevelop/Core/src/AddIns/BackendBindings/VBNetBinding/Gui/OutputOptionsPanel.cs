// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;

using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;

namespace VBBinding
{
	public class OutputOptionsPanel : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
		
		/* public override bool ReceiveDialogMessage(DialogMessage message)
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
				
				compilerParameters.CompileTarget               = (CompileTarget)((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex;
				compilerParameters.OutputAssembly              = ControlDictionary["assemblyNameTextBox"].Text;
				compilerParameters.OutputDirectory             = ControlDictionary["outputDirectoryTextBox"].Text;
				compilerParameters.CommandLineParameters       = ControlDictionary["parametersTextBox"].Text;
				compilerParameters.ExecuteBeforeBuild          = ControlDictionary["executeBeforeTextBox"].Text;
				compilerParameters.ExecuteAfterBuild           = ControlDictionary["executeAfterTextBox"].Text;
				compilerParameters.ExecuteScript               = ControlDictionary["executeScriptTextBox"].Text;
				compilerParameters.Win32Icon                   = ControlDictionary["win32IconTextBox"].Text;
				compilerParameters.ExecuteBeforeBuildArguments = ControlDictionary["executeBeforeArgumentsTextBox"].Text;
				compilerParameters.ExecuteAfterBuildArguments  = ControlDictionary["executeAfterArgumentsTextBox"].Text;
				
				compilerParameters.PauseConsoleOutput = ((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked;
			}
			return true;
		}
	
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			((ComboBox)ControlDictionary["compileTargetComboBox"]).SelectedIndex = (int)compilerParameters.CompileTarget;
			ControlDictionary["win32IconTextBox"].Text              = compilerParameters.Win32Icon;
			ControlDictionary["assemblyNameTextBox"].Text           = compilerParameters.OutputAssembly;
			ControlDictionary["outputDirectoryTextBox"].Text        = compilerParameters.OutputDirectory;
			ControlDictionary["parametersTextBox"].Text             = compilerParameters.CommandLineParameters;
			ControlDictionary["executeScriptTextBox"].Text          = compilerParameters.ExecuteScript;
			ControlDictionary["executeBeforeTextBox"].Text          = compilerParameters.ExecuteBeforeBuild;
			ControlDictionary["executeAfterTextBox"].Text           = compilerParameters.ExecuteAfterBuild;
			ControlDictionary["executeBeforeArgumentsTextBox"].Text = compilerParameters.ExecuteBeforeBuildArguments;
			ControlDictionary["executeAfterArgumentsTextBox"].Text  = compilerParameters.ExecuteAfterBuildArguments;
			
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
		void SelectWin32Icon(object sender, EventArgs e) 
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["win32IconTextBox"].Text = fdiag.FileName;
				}
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
			ControlDictionary["browseWin32IconButton"].Click += new EventHandler(SelectWin32Icon);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Library"));
			((ComboBox)ControlDictionary["compileTargetComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.PrjOptions.Configuration.CompileTarget.Module"));
			
		}
	} */
	
			static MessageService messageService = (MessageService) ServiceManager.GetService (typeof (MessageService));

		class OutputOptionsPanelWidget : GladeWidgetExtract 
		{
			//
			// Gtk Controls	
			//
			[Glade.Widget] Entry assemblyNameEntry;
			[Glade.Widget] Entry outputDirectoryEntry;
			[Glade.Widget] Entry parametersEntry;
			[Glade.Widget] Entry executeBeforeEntry;
			[Glade.Widget] Entry executeScriptEntry;
			[Glade.Widget] Entry executeAfterEntry;
			[Glade.Widget] CheckButton pauseConsoleOutputCheckButton;			
			[Glade.Widget] Button browseButton;
			[Glade.Widget] Button browseButton2;
			[Glade.Widget] Button browseButton3;
			[Glade.Widget] Button browseButton4;
			
			VBCompilerParameters compilerParameters;

			public  OutputOptionsPanelWidget(IProperties CustomizationObject) : base ("VB.glade", "OutputOptionsPanel")
 			{			
				this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
				browseButton.Clicked += new EventHandler (SelectFolder);
				browseButton2.Clicked += new EventHandler (SelectFile4);
				browseButton3.Clicked += new EventHandler (SelectFile3);
				browseButton4.Clicked += new EventHandler (SelectFile2);
				
				assemblyNameEntry.Text = compilerParameters.OutputAssembly;
				outputDirectoryEntry.Text = compilerParameters.OutputDirectory;
				parametersEntry.Text      = compilerParameters.CommandLineParameters;
				executeScriptEntry.Text   = compilerParameters.ExecuteScript;
 				executeBeforeEntry.Text   = compilerParameters.ExecuteBeforeBuild;
 				executeAfterEntry.Text    = compilerParameters.ExecuteAfterBuild;
				
 				pauseConsoleOutputCheckButton.Active = compilerParameters.PauseConsoleOutput;
			}

			public bool Store ()
			{	
				if (compilerParameters == null) {
					return true;
				}
				
				FileUtilityService fileUtilityService = (FileUtilityService) ServiceManager.GetService (typeof (FileUtilityService));

				if (!fileUtilityService.IsValidFileName(assemblyNameEntry.Text)) {
					messageService.ShowError (GettextCatalog.GetString ("Invalid assembly name specified"));
					return false;
				}

				if (!fileUtilityService.IsValidFileName (outputDirectoryEntry.Text)) {
					messageService.ShowError (GettextCatalog.GetString ("Invalid output directory specified"));
					return false;
				}
				
				compilerParameters.OutputAssembly = assemblyNameEntry.Text;
				compilerParameters.OutputDirectory = outputDirectoryEntry.Text;
				compilerParameters.CommandLineParameters = parametersEntry.Text;
				compilerParameters.ExecuteBeforeBuild = executeBeforeEntry.Text;
				compilerParameters.ExecuteAfterBuild = executeAfterEntry.Text;
				compilerParameters.ExecuteScript = executeScriptEntry.Text;
				
				compilerParameters.PauseConsoleOutput = pauseConsoleOutputCheckButton.Active;
				return true;
			}
			
			void SelectFolder(object sender, EventArgs e)
			{
				using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Select the directory in which the assembly will be created"))) {
					if (fdiag.Run () == (int) ResponseType.Ok) {
						outputDirectoryEntry.Text = fdiag.Filename;
					}
				
					fdiag.Hide ();
				}
			}
		
			void SelectFile2(object sender, EventArgs e)
			{
				using (FileSelector fdiag = new FileSelector ("")) {
					//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.SelectMultiple = false;
				
					if(fdiag.Run () == (int) ResponseType.Ok) {
						executeBeforeEntry.Text = fdiag.Filename;
					}

					fdiag.Hide ();
				}
			}
			
			void SelectFile3(object sender, EventArgs e)
			{
				using (FileSelector fdiag = new FileSelector ("")) {
					//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.SelectMultiple = false;
				
					if(fdiag.Run () == (int) ResponseType.Ok) {
						executeAfterEntry.Text = fdiag.Filename;
					}

					fdiag.Hide ();
				}
			}
		
			void SelectFile4(object sender, EventArgs e)
			{
				using (FileSelector fdiag = new FileSelector ("")) {
					//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.SelectMultiple = false;
				
					if(fdiag.Run () == (int) ResponseType.Ok) {
						executeScriptEntry.Text = fdiag.Filename;
					}

					fdiag.Hide ();
				}
			}
		}

		OutputOptionsPanelWidget  widget;

		public override void LoadPanelContents()
		{
			Add (widget = new  OutputOptionsPanelWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			bool result = true;
			result = widget.Store ();
 			return result;
		}
	}

}