// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using Gtk;

namespace CSharpBinding
{
	public class OutputOptionsPanel : AbstractOptionPanel
	{
		CSharpCompilerParameters compilerParameters;
		Entry assemblyNameTextBox = new Entry ();
		Entry outputDirectoryTextBox = new Entry ();
		Entry parametersTextBox = new Entry ();
		Entry executeBeforeTextBox = new Entry ();
		Entry executeScriptTextBox = new Entry ();
		Entry executeAfterTextBox = new Entry ();
		CheckButton pauseConsoleOutputCheckBox = new CheckButton ();
		
		public override void LoadPanelContents()
		{
			Button browseButton = new Button ();
			Button browseButton2 = new Button ();
			Button browseButton3 = new Button ();
			Button browseButton4 = new Button ();
			browseButton.Clicked += new EventHandler (SelectFolder);
			browseButton2.Clicked += new EventHandler (SelectFile2);
			browseButton3.Clicked += new EventHandler (SelectFile3);
			browseButton4.Clicked += new EventHandler (SelectFile4);
			
			this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			//Console.WriteLine("SET BLABLUB");
			assemblyNameTextBox.Text = compilerParameters.OutputAssembly;
			outputDirectoryTextBox.Text = compilerParameters.OutputDirectory;
			parametersTextBox.Text      = compilerParameters.CommandLineParameters;
			executeScriptTextBox.Text   = compilerParameters.ExecuteScript;
			executeBeforeTextBox.Text   = compilerParameters.ExecuteBeforeBuild;
			executeAfterTextBox.Text    = compilerParameters.ExecuteAfterBuild;
			
			pauseConsoleOutputCheckBox.Active = compilerParameters.PauseConsoleOutput;
		}
		
		public override bool StorePanelContents()
		{
			//Console.WriteLine("store contents");
			
			if (compilerParameters == null) {
				return true;
			}
			
			//Console.WriteLine("1");
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			if (!fileUtilityService.IsValidFileName(assemblyNameTextBox.Text)) {
				//MessageService.ShowError("Invalid assembly name specified");
				return false;
			}
			if (!fileUtilityService.IsValidFileName (outputDirectoryTextBox.Text)) {
				//MessageService.ShowError("Invalid output directory specified");
				return false;
			}
			
			//Console.WriteLine("2");
			compilerParameters.OutputAssembly = assemblyNameTextBox.Text;
			compilerParameters.OutputDirectory = outputDirectoryTextBox.Text;
			compilerParameters.CommandLineParameters = parametersTextBox.Text;
			compilerParameters.ExecuteBeforeBuild = executeBeforeTextBox.Text;
			compilerParameters.ExecuteAfterBuild = executeAfterTextBox.Text;
			compilerParameters.ExecuteScript = executeScriptTextBox.Text;
			
			compilerParameters.PauseConsoleOutput = pauseConsoleOutputCheckBox.Active;
			return true;
		}
		
		void SelectFolder(object sender, EventArgs e)
		{
			FileSelection fdiag = new FileSelection ("${res:Dialog.Options.PrjOptions.Con        figuration.FolderBrowserDescription}");
			
			if (fdiag.Run () == (int) ResponseType.Ok) {
				outputDirectoryTextBox.Text = fdiag.Filename;				
			}
		}
		
		void SelectFile2(object sender, EventArgs e)
		{
			FileSelection fdiag = new FileSelection ("");
			//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.SelectMultiple = false;
			
			if(fdiag.Run () == (int) ResponseType.Ok) {
				executeBeforeTextBox.Text = fdiag.Filename;
			}
		}
		
		void SelectFile3(object sender, EventArgs e)
		{
			FileSelection fdiag = new FileSelection ("");
			//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.SelectMultiple = false;
			
			if(fdiag.Run () == (int) ResponseType.Ok) {
				executeAfterTextBox.Text = fdiag.Filename;
			}
		}
		
		void SelectFile4(object sender, EventArgs e)
		{
			FileSelection fdiag = new FileSelection ("");
			//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			fdiag.SelectMultiple = false;
			
			if(fdiag.Run () == (int) ResponseType.Ok) {
				executeScriptTextBox.Text = fdiag.Filename;
			}
		}
	}
}
