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
using System.ComponentModel;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Gui.Components;

using MonoDevelop.Services;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// Summary description for Form3.
	/// </summary>
	public class DeployFileProjectOptions : AbstractOptionPanel
	{

		// FIXME 
		// - internationalize 
		// 	   SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
		// 			                          @"resources\panels\DeployFileOptions.xfrm"));

		class DeployFileOptionsWidget : GladeWidgetExtract 
		{
		        // Gtk Controls
 			[Glade.Widget] RadioButton projectFileRadioButton;
 			[Glade.Widget] RadioButton compiledAssemblyRadioButton;
 			[Glade.Widget] RadioButton scriptFileRadioButton;
 			[Glade.Widget] Button selectScriptFileButton;
 			[Glade.Widget] Button selectTargetButton;
 			[Glade.Widget] Entry deployTargetEntry;
 			[Glade.Widget] Entry deployScriptEntry;
			[Glade.Widget] VBox deployScriptBox;
			[Glade.Widget] VBox deployTargetBox;
			[Glade.Widget] Gtk.TreeView includeTreeView;
			public ListStore store;

			// Services
			IProject project;
			static FileUtilityService fileUtilityService = (FileUtilityService) ServiceManager.Services.GetService(typeof(FileUtilityService));
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
			public DeployFileOptionsWidget (IProperties CustomizationObject) : 
				base ("Base.glade", "DeployFileOptionsPanel")
			{
				this.project = (IProject)((IProperties)CustomizationObject).GetProperty("Project");

  				projectFileRadioButton.Clicked += new EventHandler(RadioButtonClicked);
  				compiledAssemblyRadioButton.Clicked += new EventHandler(RadioButtonClicked);
  				scriptFileRadioButton.Clicked += new EventHandler(RadioButtonClicked);
				selectScriptFileButton.Clicked += new EventHandler(SelectScriptFileEvent);
				selectTargetButton.Clicked += new EventHandler(SelectTargetFolderEvent);

				store = new ListStore (typeof(bool), typeof(string));
				includeTreeView.Selection.Mode = SelectionMode.None;
				includeTreeView.Model = store;
				CellRendererToggle rendererToggle = new CellRendererToggle ();
				rendererToggle.Activatable = true;
				rendererToggle.Toggled += new ToggledHandler (ItemToggled);
				includeTreeView.AppendColumn ("Choosen", rendererToggle, "active", 0);
				includeTreeView.AppendColumn ("Name", new CellRendererText (), "text", 1);
				TreeIter iter = new TreeIter ();
				
				foreach (ProjectFile info in project.ProjectFiles) {
					if (info.BuildAction != BuildAction.Exclude) {
						string name = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, info.Name);
						iter = store.AppendValues (
							project.DeployInformation.IsFileExcluded(info.Name) ? true : false, name);
					}
				}

				deployTargetEntry.Text = project.DeployInformation.DeployTarget;
				deployScriptEntry.Text = project.DeployInformation.DeployScript;
			
				projectFileRadioButton.Active = project.DeployInformation.DeploymentStrategy == DeploymentStrategy.File;
				compiledAssemblyRadioButton.Active = project.DeployInformation.DeploymentStrategy == DeploymentStrategy.Assembly;
				scriptFileRadioButton.Active = project.DeployInformation.DeploymentStrategy == DeploymentStrategy.Script;
				
				RadioButtonClicked(null, null);
			}

			//FIXME : Finish details in this dialog.
			void SelectScriptFileEvent(object sender, EventArgs e)
			{
				using (FileSelection fs = new FileSelection (GettextCatalog.GetString ("Select your File"))) { // Put correct title 
					fs.Complete("*.txt");
					if ( fs.Run () == (int) ResponseType.Ok) {
 						deployScriptEntry.Text = fs.SelectionEntry.Text;
					}
					fs.Hide ();
				} 
			}
			
			void SelectTargetFolderEvent(object sender, EventArgs e)
			{
				using (FileSelection fs = new FileSelection (GettextCatalog.GetString ("Select the target directory"))) {
					if ( fs.Run () == (int) ResponseType.Ok) {
						deployTargetEntry.Text = fs.SelectionEntry.Text;
					}
					fs.Hide ();
				}
			}
			
			void RadioButtonClicked(object sender, EventArgs e)
			{
 				deployTargetBox.Sensitive = compiledAssemblyRadioButton.Active || projectFileRadioButton.Active;
  				deployScriptEntry.Sensitive = scriptFileRadioButton.Active;
  				selectScriptFileButton.Sensitive = scriptFileRadioButton.Active;
			}
			
			private void ItemToggled (object o, ToggledArgs args)
			{
 				const int column = 0;
 				Gtk.TreeIter iter;
				
				if (store.GetIterFromString(out iter, args.Path)) {
 					bool val = (bool) store.GetValue(iter, column);
 					store.SetValue(iter, column, !val);
 				}
			}
			
			public bool Store () 
			{
				if (deployTargetEntry.Text.Length > 0) {
					if (!fileUtilityService.IsValidFileName(deployTargetEntry.Text)) {
						MessageDialog dialog = new MessageDialog ((Window) WorkbenchSingleton.Workbench, 
								DialogFlags.DestroyWithParent,
								MessageType.Error, 
								ButtonsType.Close, 
								GettextCatalog.GetString ("Invalid deploy target specified"));
						dialog.Run ();
						dialog.Hide ();
						dialog.Dispose ();
						return false;
					}
				}
				
				if (deployScriptEntry.Text.Length > 0) {
					if (!fileUtilityService.IsValidFileName(deployScriptEntry.Text)) {
						MessageDialog dialog = new MessageDialog ((Window) WorkbenchSingleton.Workbench, 
								DialogFlags.DestroyWithParent,
								MessageType.Error, 
								ButtonsType.Close, 
								GettextCatalog.GetString ("Invalid deploy script specified"));
						dialog.Run ();
						dialog.Hide ();
						dialog.Dispose ();
						return false;				
					}
				}
				
				if (!File.Exists(deployScriptEntry.Text)) {
					MessageDialog dialog = new MessageDialog ((Window) WorkbenchSingleton.Workbench, 
							DialogFlags.DestroyWithParent,
							MessageType.Error, 
							ButtonsType.Close, 
							GettextCatalog.GetString ("Deploy script doesn't exists"));
					dialog.Run ();
					dialog.Hide ();
					dialog.Dispose ();
					return false;
 				}
			
 			project.DeployInformation.DeployTarget = deployTargetEntry.Text;
 			project.DeployInformation.DeployScript = deployScriptEntry.Text;
			
 			if (projectFileRadioButton.Active) {
 				project.DeployInformation.DeploymentStrategy = DeploymentStrategy.File;
 			} else if (compiledAssemblyRadioButton.Active) {
 				project.DeployInformation.DeploymentStrategy = DeploymentStrategy.Assembly;
 			} else {
 				project.DeployInformation.DeploymentStrategy = DeploymentStrategy.Script;
 			}

			TreeIter first;	
			store.GetIterFirst(out first);
			TreeIter current = first;
 			project.DeployInformation.ClearExcludedFiles();
			for (int i = 0; i < store.IterNChildren() ; ++i) {
				if ( (bool) store.GetValue(current, 0)){
					project.DeployInformation.AddExcludedFile(fileUtilityService.RelativeToAbsolutePath(
											  project.BaseDirectory, 
											  (string) store.GetValue(current, 1)));
							}
				store.IterNext(out current);
			}
			return true;
			}
		}
		
		DeployFileOptionsWidget widget;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public override void LoadPanelContents()
		{
			Add (widget = new  DeployFileOptionsWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			bool success = widget.Store();
 			return success;
		}
	}
}
