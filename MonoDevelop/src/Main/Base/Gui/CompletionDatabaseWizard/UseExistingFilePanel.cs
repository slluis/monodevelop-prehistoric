// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using Gtk;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class UseExistingFilePanel : AbstractWizardPanel
	{
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		IProperties properties;
		Gnome.FileEntry locationTextBox;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			switch (message) {
				case DialogMessage.Activated:
					SetFinishedState(this, EventArgs.Empty);
					break;
				case DialogMessage.Prev:
					EnableFinish = false;
					break;
			}
			return true;
		}
		
		void SetFinishedState(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string path = locationTextBox.GtkEntry.Text;
			EnableFinish = fileUtilityService.IsValidFileName(path) &&
			               Directory.Exists(path) && 
			               File.Exists(fileUtilityService.GetDirectoryNameWithSeparator(path) + "CodeCompletionProxyDataV02.bin");
			if (EnableFinish) {
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
				                       path);
			}
		}
		
		void SetValues(object sender, EventArgs e)
		{
			properties = (IProperties)CustomizationObject;
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public UseExistingFilePanel()
		{
			IsLastPanel       = true;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			VBox mainVBox  = new VBox (false, 0);
			
			TextView textBox = new TextView ();
			textBox.WrapMode = WrapMode.Word;
			textBox.Buffer.Text = resourceService.GetString ("Dialog.Wizards.CodeCompletionDatabaseWizard.UseExistingFilePanel.PanelDescription");
			mainVBox.PackStart (textBox, false, true, 0);
			mainVBox.PackStart (new Label ("Specify location of existing code completion database."));
			
			locationTextBox = new Gnome.FileEntry ("locationTextBox", "choose the location in which you want the code completion files to be generated");
			locationTextBox.DirectoryEntry = true;
			locationTextBox.Modal = true;
			locationTextBox.GtkEntry.WidthChars = 50;
			locationTextBox.Changed += new EventHandler(SetFinishedState);
			HBox hbox = new HBox (false, 0);
			hbox.PackStart (locationTextBox, false, true, 0);
			SetFinishedState(this, EventArgs.Empty);
			CustomizationObjectChanged += new EventHandler(SetValues);
			
			mainVBox.PackStart (hbox, false, true, 6);
			this.Add (mainVBox);
		}
	}
}
