// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using Gtk;
using MonoDevelop.Gui;

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
		
		Button browseButton;
		RadioButton specifyLocationRadioButton;
		RadioButton sharpDevelopDirRadioButton;
		Entry locationTextBox = new Entry ();
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			//Console.WriteLine ("Inside ReceiveDialogMessage");
			if (message == DialogMessage.Cancel) {
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty);
			} else if (message == DialogMessage.Next || message == DialogMessage.OK) {
				string path = null;				
				if (specifyLocationRadioButton.Active) {
					path = locationTextBox.Text.TrimEnd (System.IO.Path.DirectorySeparatorChar);
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
				
				//Console.WriteLine ("Setting path to: " + path);
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
				browseButton.Sensitive = locationTextBox.Sensitive = specifyLocationRadioButton.Active;
			}
			catch
			{
			}
			
			SetFinishedState(sender, e);
		}
		
		void BrowseLocationEvent(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog ("choose the location in which you want the code completion files to be generated");
			int response = fd.Run ();
			fd.Hide ();
			if (response == (int) ResponseType.Ok) {
				locationTextBox.Text = fd.Filename;
			}
		}
		
		void SetFinishedState(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			try
			{
				EnableFinish = EnableNext = !specifyLocationRadioButton.Active ||
  			                            (fileUtilityService.IsValidFileName(locationTextBox.Text) && 
  			                            Directory.Exists(locationTextBox.Text));
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
			
			browseButton = new Button ("...");
			browseButton.Clicked += new EventHandler (BrowseLocationEvent);
			
			locationTextBox.Changed += new EventHandler (SetFinishedState);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			//RadioButton appDirRadioButton = new RadioButton ("Use current user's application directory");
			RadioButton appDirRadioButton = RadioButton.NewWithLabel (null, "Use current user's application directory");
			appDirRadioButton.Toggled += new EventHandler (SetEnableStatus);
			sharpDevelopDirRadioButton = new RadioButton (appDirRadioButton);
			sharpDevelopDirRadioButton.Active = true;
			sharpDevelopDirRadioButton.Label = "Use SharpDevelop application directory";
			sharpDevelopDirRadioButton.Toggled += new EventHandler (SetEnableStatus);
			specifyLocationRadioButton = new RadioButton (appDirRadioButton);
			specifyLocationRadioButton.Label = "Specify code completion database location";
			specifyLocationRadioButton.Toggled += new EventHandler (SetEnableStatus);

			TextView t = new TextView ();
			t.Buffer.Text = resourceService.GetString ("Dialog.Wizards.CodeCompletionDatabaseWizard.ChooseLocationPanel.DescriptionText");
			t.Editable = false;
			t.CursorVisible = false;
			t.WrapMode = Gtk.WrapMode.Word;
			
			HBox hbox = new HBox (false, 0);
			hbox.PackStart (locationTextBox);
			hbox.PackStart (browseButton, false, false, 3);
		
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
