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
using MonoDevelop.Gui.Widgets;

namespace CSharpBinding
{
	
	//FIXME: i8n 

	public class OutputOptionsPanel : AbstractOptionPanel
	{

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
			
			CSharpCompilerParameters compilerParameters;

			public  OutputOptionsPanelWidget(IProperties CustomizationObject) : base ("CSharp.glade", "OutputOptionsPanel")
 			{			
				this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
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
				
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(
					typeof(FileUtilityService));
				if (!fileUtilityService.IsValidFileName(assemblyNameEntry.Text)) {
					//MessageService.ShowError("Invalid assembly name specified");
					return false;
				}
				if (!fileUtilityService.IsValidFileName (outputDirectoryEntry.Text)) {
					//MessageService.ShowError("Invalid output directory specified");
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
				FileSelection fdiag = new FileSelection (
					"${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
				
				if (fdiag.Run () == (int) ResponseType.Ok) {
				outputDirectoryEntry.Text = fdiag.Filename;				
				}
				fdiag.Hide ();
				fdiag.Dispose ();
			}
		
			void SelectFile2(object sender, EventArgs e)
			{
				FileSelection fdiag = new FileSelection ("");
				//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.SelectMultiple = false;
				
				if(fdiag.Run () == (int) ResponseType.Ok) {
					executeBeforeEntry.Text = fdiag.Filename;
				}
				fdiag.Hide ();
				fdiag.Dispose ();			
			}
			
			void SelectFile3(object sender, EventArgs e)
			{
				FileSelection fdiag = new FileSelection ("");
				//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.SelectMultiple = false;
				
				if(fdiag.Run () == (int) ResponseType.Ok) {
					executeAfterEntry.Text = fdiag.Filename;
				}
				fdiag.Hide ();
				fdiag.Dispose ();
			}
		
			void SelectFile4(object sender, EventArgs e)
			{
				FileSelection fdiag = new FileSelection ("");
				//fdiag.Filter = StringParserService.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				fdiag.SelectMultiple = false;
				
				if(fdiag.Run () == (int) ResponseType.Ok) {
					executeScriptEntry.Text = fdiag.Filename;
				}
				fdiag.Hide ();
				fdiag.Dispose ();
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
