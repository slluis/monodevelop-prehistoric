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
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class CreateDatabasePanel : AbstractWizardPanel, IProgressMonitor
	{
		IProperties properties;
		GLib.IdleHandler iterate;
		bool finished = false;
		bool began = false;
		int         totalWork = 0;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		CheckButton fastCreationCheckBox = new CheckButton ("Enable fast database creation (=slower code completion)");
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
	    	return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			properties = (IProperties)CustomizationObject;
		}
		
		void StartCreation(object sender, EventArgs e)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (began) {
				SetProgressBarValue(0);
				createButton.Label = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.StartCreationButton");
				EnableCancel = EnablePrevious = true;
				//fastCreationCheckBox.Active = true;
			} else {
				began = true;
				EnableCancel = EnablePrevious = false;
				//fastCreationCheckBox.Active = false;
				iterate = new GLib.IdleHandler (CreateDatabase);
				GLib.Idle.Add (iterate);
				
				createButton.Label = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.CancelCreationButton");
			}
		}
				// changed to work during GLib.Idle
		bool CreateDatabase()
		{
			try {
				DefaultParserService parserService  = (DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DefaultParserService));
				string path  = properties.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty);
				//Console.WriteLine (path);
				if (fastCreationCheckBox.Active) {
					parserService.GenerateCodeCompletionDatabaseFast(path, this);
				} else {
					parserService.GenerateEfficientCodeCompletionDatabase(path, this);
				}
			} catch (Exception e) {
				ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService (typeof (IResourceService));
				MessageService messageService = (MessageService) ServiceManager.Services.GetService (typeof (IMessageService));
				//Console.WriteLine (e.ToString ());
				messageService.ShowError (resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.CreateDbErrorMessage") + "\n" + e.ToString());
				//throw e;
			}
			
			//Console.WriteLine (!finished);
			return !finished;
		}

		Gtk.Button createButton;
		Gtk.ProgressBar progressBar;

		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public CreateDatabasePanel() : base()
		{

			createButton = new Gtk.Button ("Start database creation");
			progressBar = new Gtk.ProgressBar ();
			progressBar.BarStyle = Gtk.ProgressBarStyle.Continuous;
		
			NextWizardPanelID = "CreationSuccessful";
			
			EnableFinish      = false;
			EnableNext        = false;
			CustomizationObjectChanged += new EventHandler(SetValues);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			Gtk.TextView t = new Gtk.TextView ();
			t.Buffer.Text = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.PanelDescription");
			t.WrapMode = Gtk.WrapMode.Word;
			t.Editable = false;
			t.CursorVisible = false;
			
			Gtk.VBox mainbox = new Gtk.VBox (false, 2);
			mainbox.PackStart (t);
			mainbox.PackStart (fastCreationCheckBox, false, false, 2);
			mainbox.PackStart (createButton, false, false, 2);
			mainbox.PackStart (progressBar, false, false, 2);
		
			this.Add (mainbox);
			
			createButton.Clicked += new EventHandler(StartCreation);
		}
		
		void SetButtonFinish(int val)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			createButton.Label = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.FinishedCreationButton");
			createButton.Sensitive = false;
			progressBar.Sensitive = false;
			EnableCancel = EnablePrevious = false;
			EnableFinish = true;
			FinishPanel();
		}
		
		void SetProgressBarLimit(int val)
		{
		}
		void SetProgressBarValue(int val)
		{
		}
		
		delegate void SetValue(int val);
		
		public void BeginTask(string name, int totalWork)
		{
			this.totalWork = totalWork;
		}
		
		
		public void Worked(int work, string status)
		{
			double tmp = (double) ((double)work / (double)totalWork);
			progressBar.Fraction = tmp;
			progressBar.Text = status;

			while(Gtk.Application.EventsPending ()) {
				Gtk.Application.RunIteration (true);
			}
		}
		
		public void Done()
		{
			finished = true;
			progressBar.Fraction = 1;
			SetButtonFinish (1);
		}
		
		public bool Canceled {
			get {
				return false;
			}
			set {
			}
		}
		
		public string TaskName {
			get {
				return String.Empty;
			}
			set {
			}
		}
	}
}
