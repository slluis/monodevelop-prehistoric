// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;

using Gtk;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
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
			
			DotNetProjectConfiguration configuration;

			public  OutputOptionsPanelWidget(IProperties CustomizationObject) : base ("Base.glade", "OutputOptionsPanel")
 			{			
				configuration = (DotNetProjectConfiguration)((IProperties)CustomizationObject).GetProperty("Config");
				browseButton.Clicked += new EventHandler (SelectFolder);
				browseButton2.Clicked += new EventHandler (SelectFile4);
				browseButton3.Clicked += new EventHandler (SelectFile3);
				browseButton4.Clicked += new EventHandler (SelectFile2);
				
				assemblyNameEntry.Text = configuration.OutputAssembly;
				outputDirectoryEntry.Text = configuration.OutputDirectory;
				parametersEntry.Text      = configuration.CommandLineParameters;
				executeScriptEntry.Text   = configuration.ExecuteScript;
 				executeBeforeEntry.Text   = configuration.ExecuteBeforeBuild;
 				executeAfterEntry.Text    = configuration.ExecuteAfterBuild;
				
 				pauseConsoleOutputCheckButton.Active = configuration.PauseConsoleOutput;
			}

			public bool Store ()
			{	
				if (configuration == null) {
					return true;
				}
				
				if (!Runtime.FileUtilityService.IsValidFileName(assemblyNameEntry.Text)) {
					Runtime.MessageService.ShowError (GettextCatalog.GetString ("Invalid assembly name specified"));
					return false;
				}

				if (!Runtime.FileUtilityService.IsValidFileName (outputDirectoryEntry.Text)) {
					Runtime.MessageService.ShowError (GettextCatalog.GetString ("Invalid output directory specified"));
					return false;
				}
				
				configuration.OutputAssembly = assemblyNameEntry.Text;
				configuration.OutputDirectory = outputDirectoryEntry.Text;
				configuration.CommandLineParameters = parametersEntry.Text;
				configuration.ExecuteBeforeBuild = executeBeforeEntry.Text;
				configuration.ExecuteAfterBuild = executeAfterEntry.Text;
				configuration.ExecuteScript = executeScriptEntry.Text;
				configuration.PauseConsoleOutput = pauseConsoleOutputCheckButton.Active;
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
