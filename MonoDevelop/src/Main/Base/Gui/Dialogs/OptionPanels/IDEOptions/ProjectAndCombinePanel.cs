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
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class ProjectAndCombinePanel : AbstractOptionPanel
	{

		// service instances needed
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		FileUtilityService FileUtilityService = (FileUtilityService)ServiceManager.Services.GetService (typeof (FileUtilityService));
		MessageService MessageService = (MessageService)ServiceManager.Services.GetService (typeof (MessageService));
		
		ProjectAndCombinePanelWidget widget;
		const string projectAndCombineProperty = "SharpDevelop.UI.ProjectAndCombineOptions";

		public override void LoadPanelContents()
		{
			IProperties p = (IProperties) PropertyService.GetProperty (projectAndCombineProperty, new DefaultProperties ());
			Add (widget = new  ProjectAndCombinePanelWidget (p));
		}
		
		public override bool StorePanelContents()
		{
			IProperties p = (IProperties) PropertyService.GetProperty (projectAndCombineProperty, new DefaultProperties ());

			// check for correct settings
			string projectPath = widget.projectLocationTextBox.GtkEntry.Text;
			if (projectPath.Length > 0) {
				if (!FileUtilityService.IsValidFileName(projectPath)) {
					MessageService.ShowError("Invalid project path specified");
					return false;
				}
			}
			p.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", projectPath);

			widget.Store (p);
			PropertyService.SetProperty(projectAndCombineProperty, p);
			return true;
		}
		
		
		public class ProjectAndCombinePanelWidget :  GladeWidgetExtract 
		{
			// service instances needed
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));

			//
			// Gtk controls
			//
			[Glade.Widget] public Gnome.FileEntry projectLocationTextBox;
			[Glade.Widget] public Gtk.RadioButton saveChangesRadioButton, promptChangesRadioButton, noSaveRadioButton;
			[Glade.Widget] public Gtk.CheckButton loadPrevProjectCheckBox, showTaskListCheckBox, showOutputCheckBox;
			[Glade.Widget] public Gtk.Label locationLabel, settingsLabel, buildAndRunOptionsLabel;   

			public  ProjectAndCombinePanelWidget (IProperties p) : base ("Base.glade", "ProjectAndCombinePanel")
			{
				
				settingsLabel.Markup = "<b> " + StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.SettingsGroupBox}") + "</b>";
				//
				// set up the Project Settings options
				//
				locationLabel.TextWithMnemonic = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.ProjectLocationLabel}");
				//projectLocationTextBox = new Gnome.FileEntry ("", "Choose Location");
				projectLocationTextBox.DirectoryEntry = true;
				loadPrevProjectCheckBox.Label = StringParserService.Parse(
				"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.LoadPrevProjectCheckBox}");
			        //
			        // setup the save options
			        //
				buildAndRunOptionsLabel.Markup = "<b> " + StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.BuildAndRunGroupBox}") + "</b>";
				saveChangesRadioButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.SaveChangesRadioButton}");
				promptChangesRadioButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.PromptToSaveRadioButton}");
				noSaveRadioButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.DontSaveRadioButton}");
				showOutputCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.ShowOutputPadCheckBox}");
				showTaskListCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.ProjectAndCombineOptions.ShowTaskListPadCheckBox}");
				// read properties
				projectLocationTextBox.GtkEntry.Text = p.GetProperty(
					"ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", 
					System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
							"MonoDevelopProjects")).ToString();
				BeforeCompileAction action = (BeforeCompileAction) p.GetProperty(
					"SharpDevelop.Services.DefaultParserService.BeforeCompileAction", 
					BeforeCompileAction.SaveAllFiles);
				saveChangesRadioButton.Active = action.Equals(BeforeCompileAction.SaveAllFiles);
				promptChangesRadioButton.Active = action.Equals(BeforeCompileAction.PromptForSave);
				noSaveRadioButton.Active = action.Equals(BeforeCompileAction.Nothing);
				loadPrevProjectCheckBox.Active = (bool)p.GetProperty(
					"SharpDevelop.LoadPrevProjectOnStartup", false);
				showTaskListCheckBox.Active = (bool)p.GetProperty(
					"SharpDevelop.ShowTaskListAfterBuild", true);
				showOutputCheckBox.Active = (bool)p.GetProperty(
					"SharpDevelop.ShowOutputWindowAtBuild", true);
			}
			
			public void Store (IProperties p)
			{
				// set properties
				if (saveChangesRadioButton.Active) {
					p.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", 
							BeforeCompileAction.SaveAllFiles);
				} else if (promptChangesRadioButton.Active) {
					p.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", 
							BeforeCompileAction.PromptForSave);
				} else if (noSaveRadioButton.Active) {
					p.SetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", 
							BeforeCompileAction.Nothing);
				}
				
				p.SetProperty("SharpDevelop.LoadPrevProjectOnStartup", loadPrevProjectCheckBox.Active);
				p.SetProperty("SharpDevelop.ShowTaskListAfterBuild", showTaskListCheckBox.Active);
				p.SetProperty("SharpDevelop.ShowOutputWindowAtBuild", showOutputCheckBox.Active);
			}
		}
	}
}
