// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using Gtk;
using MonoDevelop.Gui.Widgets;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class ChooseLocationPanel : AbstractWizardPanel
	{
		IProperties properties;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		RadioButton specifyLocationRadioButton;
		RadioButton sharpDevelopDirRadioButton;
		FolderEntry fEntry;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.Cancel) {
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty);
			} else if (message == DialogMessage.Next || message == DialogMessage.OK) {
				string path = null;				
				if (specifyLocationRadioButton.Active) {
					path = fEntry.Path.TrimEnd (System.IO.Path.DirectorySeparatorChar);
				} else if (sharpDevelopDirRadioButton.Active) {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
					path = propertyService.DataDirectory + 
					       System.IO.Path.DirectorySeparatorChar + "CodeCompletionData"; 
				} else {
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					path = propertyService.ConfigDirectory + "CodeCompletionTemp";
				}
				
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory", path);
				propertyService.SetProperty ("SharpDevelop.CodeCompletion.DataDirectory", path);
				propertyService.SaveProperties ();
			}
	    		return true;
		}
		
		void SetEnableStatus(object sender, EventArgs e)
		{
			try
			{
				fEntry.Sensitive = specifyLocationRadioButton.Active;
			}
			catch
			{
			}
			
			SetFinishedState(sender, e);
		}
		
		void SetFinishedState(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			try
			{
				EnableFinish = EnableNext = !specifyLocationRadioButton.Active ||
  			                            (fileUtilityService.IsValidFileName(fEntry.Path) && 
  			                            Directory.Exists(fEntry.Path));
			}
			catch
			{
			}
		}
		
		void SetValues(object sender, EventArgs e)
		{
			properties = (IProperties)CustomizationObject;
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public ChooseLocationPanel() : base()
		{
			VBox mainVBox = new VBox (false, 0);
			NextWizardPanelID = "CreateDatabasePanel";
			
			fEntry = new FolderEntry ("Choose completion database location");
			fEntry.DefaultPath = Environment.GetEnvironmentVariable ("HOME");
			fEntry.PathChanged += new EventHandler (SetFinishedState);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			RadioButton appDirRadioButton = new RadioButton ("Use current user's application directory");
			appDirRadioButton.Active = true;
			appDirRadioButton.Toggled += new EventHandler (SetEnableStatus);
			sharpDevelopDirRadioButton = new RadioButton (appDirRadioButton, "Use SharpDevelop application directory");
			sharpDevelopDirRadioButton.Toggled += new EventHandler (SetEnableStatus);
			specifyLocationRadioButton = new RadioButton (appDirRadioButton, "Specify code completion database location");
			specifyLocationRadioButton.Toggled += new EventHandler (SetEnableStatus);

			TextView t = new TextView ();
			t.Buffer.Text = resourceService.GetString ("Dialog.Wizards.CodeCompletionDatabaseWizard.ChooseLocationPanel.DescriptionText");
			t.Editable = false;
			t.CursorVisible = false;
			t.WrapMode = Gtk.WrapMode.Word;
			
			HBox hbox = new HBox (false, 0);
			hbox.PackStart (fEntry);
		
			mainVBox.PackStart (t, true, true, 0);
			mainVBox.PackStart (specifyLocationRadioButton, false, true, 6);
			mainVBox.PackStart (hbox, false, true, 6);
			mainVBox.PackStart (sharpDevelopDirRadioButton, false, true, 6);
			mainVBox.PackStart (appDirRadioButton, false, true, 6);
			this.Add (mainVBox);
		
			SetFinishedState(this, EventArgs.Empty);
			SetEnableStatus(this, EventArgs.Empty);
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}
}
