// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class SetupPanel : AbstractWizardPanel
	{
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		RadioButton useExistingRadioButton;
		RadioButton createNewRadioButton;
		RadioButton skipCreationRadioButton;
		
		void SetSuccessor(object sender, EventArgs e)
		{
			IsLastPanel = skipCreationRadioButton.Active;
			
			if (createNewRadioButton.Active) {
				NextWizardPanelID = "ChooseLocationPanel";
			} else if (useExistingRadioButton.Active) {
				NextWizardPanelID = "UseExistingFilePanel";
			}
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public SetupPanel() : base()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			string text = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.SetupPanel.DescriptionText");
			VBox mainVBox = new VBox (false, 0);

			useExistingRadioButton = new RadioButton ("Use existing code completion database");
			
			createNewRadioButton = new RadioButton (useExistingRadioButton, "Create new code completion database");
			createNewRadioButton.Active = true;
			
			skipCreationRadioButton = new RadioButton (useExistingRadioButton, "Do not create code completion database now");
			TextView t = new TextView ();
			t.Buffer.Text = text;

			t.WrapMode = Gtk.WrapMode.Word;
			t.Editable = false;
			t.CursorVisible = false;

			mainVBox.PackStart (t);
			mainVBox.PackStart (useExistingRadioButton, false, true, 6);
			mainVBox.PackStart (createNewRadioButton, false, true, 6);
			mainVBox.PackStart (skipCreationRadioButton, false, true, 6);
			this.Add (mainVBox);
			
			// FIXME: use an event that is only fired once
			skipCreationRadioButton.Toggled += new EventHandler(SetSuccessor);
			createNewRadioButton.Toggled += new EventHandler(SetSuccessor);
			useExistingRadioButton.Toggled += new EventHandler(SetSuccessor);
		}
	}
}
