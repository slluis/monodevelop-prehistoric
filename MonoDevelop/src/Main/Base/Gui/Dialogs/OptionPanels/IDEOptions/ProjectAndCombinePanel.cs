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

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Services;

//using System.Windows.Forms;
using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class ProjectAndCombinePanel : AbstractOptionPanel
	{
		//
		// Gtk controls
		//
		Gnome.FileEntry projectLocationTextBox;
		Gtk.RadioButton saveChangesRadioButton;
		Gtk.RadioButton promptChangesRadioButton;
		Gtk.RadioButton noSaveRadioButton;
		Gtk.CheckButton loadPrevProjectCheckBox;
		Gtk.CheckButton showTaskListCheckBox;
		Gtk.CheckButton showOutputCheckBox;
		
		// service instances needed
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		FileUtilityService FileUtilityService = (FileUtilityService)ServiceManager.Services.GetService (typeof (FileUtilityService));
		MessageService MessageService = (MessageService)ServiceManager.Services.GetService (typeof (MessageService));
		
		public override void LoadPanelContents()
		{
			// set up the form controls instance
			SetupPanelInstance();
			
			// read properties
			projectLocationTextBox.GtkEntry.Text = PropertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SharpDevelop Projects")).ToString();
			
			BeforeCompileAction action = (BeforeCompileAction)PropertyService.GetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			
			System.Console.WriteLine("action is " + action.ToString());
			System.Console.WriteLine("Comparison 1 is " + ((int)BeforeCompileAction.SaveAllFiles));
			System.Console.WriteLine("Comparison 2 is " + ((int)BeforeCompileAction.PromptForSave));
			System.Console.WriteLine("Comparison 3 is " + ((int)BeforeCompileAction.Nothing));
			
			saveChangesRadioButton.Active = action.Equals(BeforeCompileAction.SaveAllFiles);
			promptChangesRadioButton.Active = action.Equals(BeforeCompileAction.PromptForSave);
			noSaveRadioButton.Active = action.Equals(BeforeCompileAction.Nothing);
			
			loadPrevProjectCheckBox.Active = (bool)PropertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false);

			showTaskListCheckBox.Active = (bool)PropertyService.GetProperty("SharpDevelop.ShowTaskListAfterBuild", true);
			showOutputCheckBox.Active = (bool)PropertyService.GetProperty("SharpDevelop.ShowOutputWindowAtBuild", true);
			
		}
		
		public override bool StorePanelContents()
		{
			// check for correct settings
			string projectPath = projectLocationTextBox.GtkEntry.Text;
			if (projectPath.Length > 0) {
				if (!FileUtilityService.IsValidFileName(projectPath)) {
					MessageService.ShowError("Invalid project path specified");
					return false;
				}
			}
			
			// set properties
			PropertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", projectPath);
			
			if (saveChangesRadioButton.Active) {
				PropertyService.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			} else if (promptChangesRadioButton.Active) {
				PropertyService.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.PromptForSave);
			} else if (noSaveRadioButton.Active) {
				PropertyService.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.Nothing);
			}
			
			PropertyService.SetProperty("SharpDevelop.LoadPrevProjectOnStartup", loadPrevProjectCheckBox.Active);
			
			PropertyService.SetProperty("SharpDevelop.ShowTaskListAfterBuild", showTaskListCheckBox.Active);
			PropertyService.SetProperty("SharpDevelop.ShowOutputWindowAtBuild", showOutputCheckBox.Active);
			
			return true;
		}
		
		
		private void SetupPanelInstance()
		{
			Gtk.Frame frame1 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.SettingsGroupBox}"));
			//
			// set up the Project Settings options
			//
			// instantiate all the controls
			Gtk.VBox vBox1 = new Gtk.VBox(false,2);
			Gtk.Label label1 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.ProjectLocationLabel}"));
				// make the location text box and button
				Gtk.HBox hBox1 = new Gtk.HBox(false,2);
				projectLocationTextBox = new Gnome.FileEntry ("", "Choose Location");
				projectLocationTextBox.DirectoryEntry = true;
				hBox1.PackStart(projectLocationTextBox, true, true, 2);
			loadPrevProjectCheckBox = new Gtk.CheckButton (StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.LoadPrevProjectCheckBox}"));
			// pack them all
			vBox1.PackStart(label1, false, false, 2);
			vBox1.PackStart(hBox1, false, false, 2);
			vBox1.PackStart(loadPrevProjectCheckBox, false, false, 2);
			frame1.Add(vBox1);
			
			//
			// setup the save options
			//
			//instantiate all the controls
			Gtk.Frame frame2 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.BuildAndRunGroupBox}"));
			Gtk.VBox vBox2 = new Gtk.VBox(false, 2);
			saveChangesRadioButton = new RadioButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.SaveChangesRadioButton}"));
			promptChangesRadioButton = new RadioButton(saveChangesRadioButton, StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.PromptToSaveRadioButton}"));
			noSaveRadioButton = new RadioButton (promptChangesRadioButton, StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.DontSaveRadioButton}"));
			showOutputCheckBox = new CheckButton (StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.ShowOutputPadCheckBox}"));;
			showTaskListCheckBox = new CheckButton (StringParserService.Parse("${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.ShowTaskListPadCheckBox}"));
			// pack them all
			vBox2.PackStart(saveChangesRadioButton, false, false, 2);
			vBox2.PackStart(promptChangesRadioButton, false, false, 2);
			vBox2.PackStart(noSaveRadioButton, false, false, 2);
			vBox2.PackStart(showOutputCheckBox, false, false, 2);
			vBox2.PackStart(showTaskListCheckBox, false, false, 2);
			frame2.Add(vBox2);
			
			// create the main box
			Gtk.VBox mainBox = new Gtk.VBox(false, 2);
			mainBox.PackStart(frame1, false, false, 2);
			mainBox.PackStart(frame2, false, false, 2);
			
			this.Add(mainBox);		
		}		
	}
}
