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

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui.Components;
using Gtk;
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	// FIXME 
	// - internationalize 
	//   SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
	//                           @"resources\panels\GeneralProjectOptions.xfrm"));
	// - Name entry can't be empty. It crashes with empty values.

	public class GeneralProjectOptions : AbstractOptionPanel {

		class GeneralProjectOptionsWidget : GladeWidgetExtract {

			// Gtk Controls
			[Glade.Widget] Label informationHeaderLabel;
			[Glade.Widget] Label nameLabel;
			[Glade.Widget] Label descriptionLabel;
			[Glade.Widget] Label onProjectLoadHeaderLabel;
			[Glade.Widget] Entry projectNameEntry;
			[Glade.Widget] TextView projectDescriptionTextView;
			[Glade.Widget] CheckButton newFilesOnLoadCheckButton;
 			[Glade.Widget] CheckButton autoInsertNewFilesCheckButton;
 			[Glade.Widget] CheckButton enableViewStateCheckButton;

			// Services
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (
							typeof (StringParserService));
			IProject project;



			public GeneralProjectOptionsWidget (IProperties CustomizationObject) : base ("Base.glade", "GeneralProjectOptionsPanel")
			{
				this.project = (IProject)((IProperties)CustomizationObject).GetProperty("Project");

				nameLabel.Text = StringParserService.Parse(
					"${res:Dialog.Options.PrjOptions.General.ProjectNameLabel}");
				nameLabel.UseUnderline = true;
				
				descriptionLabel.Text = StringParserService.Parse(
					"${res:Dialog.Options.PrjOptions.General.ProjectDescriptionLabel}");
				descriptionLabel.UseUnderline = true;

				// FIXME: il8n this
				informationHeaderLabel.Markup = "<b>" + "Project Information" + "</b>";
				// FIXME: il8n this
				onProjectLoadHeaderLabel.Markup = "<b>" + "On Project Load" + "</b>";

				newFilesOnLoadCheckButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.PrjOptions.General.SearchNewFileOnLoadCheckBox}");
				autoInsertNewFilesCheckButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.PrjOptions.General.AutoIncludeFilesCheckBox}");
				enableViewStateCheckButton.Label = StringParserService.Parse(
					"${res:Dialog.Options.PrjOptions.General.ViewStateCheckBox}" );

				projectNameEntry.Text = project.Name;
				projectDescriptionTextView.Buffer.Text = project.Description;
				enableViewStateCheckButton.Active = project.EnableViewState;
				
				switch (project.NewFileSearch) 
				{
				case NewFileSearch.None:
					newFilesOnLoadCheckButton.Active = false; 
					autoInsertNewFilesCheckButton.Active = false;
					break;
				case NewFileSearch.OnLoad:
					newFilesOnLoadCheckButton.Active = true; 
					autoInsertNewFilesCheckButton.Active = false;
					break;
				default:
					newFilesOnLoadCheckButton.Active = true; 
					autoInsertNewFilesCheckButton.Active = true;
					break;
				}
				
				newFilesOnLoadCheckButton.Clicked += new EventHandler(AutoLoadCheckBoxCheckedChangeEvent);
				AutoLoadCheckBoxCheckedChangeEvent(null, null);
			
			}			
		void AutoLoadCheckBoxCheckedChangeEvent(object sender, EventArgs e)
		{
			autoInsertNewFilesCheckButton.Sensitive = false;
			autoInsertNewFilesCheckButton.Active = newFilesOnLoadCheckButton.Active;
			
			// FIXME: leave this deselected and checked by default for 0.1 release, uncomment again after release
 			/*
			autoInsertNewFilesCheckButton.Sensitive = newFilesOnLoadCheckButton.Active;
			if (newFilesOnLoadCheckButton.Active == false) 
			{
				autoInsertNewFilesCheckButton.Active = false;
			}
			*/
		}

			
			public void  Store (IProperties CustomizationObject)
			{
				project.Name                 = projectNameEntry.Text;
				project.Description          = projectDescriptionTextView.Buffer.Text;
				project.EnableViewState      = enableViewStateCheckButton.Active;
				
				if (newFilesOnLoadCheckButton.Active) {
					project.NewFileSearch = autoInsertNewFilesCheckButton.Active ?  NewFileSearch.OnLoadAutoInsert : NewFileSearch.OnLoad;
				} else {
					project.NewFileSearch = NewFileSearch.None;
				}
			}
		}
		
		GeneralProjectOptionsWidget widget;

		public override void LoadPanelContents()
		{
			Add (widget = new  GeneralProjectOptionsWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			widget.Store ((IProperties) CustomizationObject);
 			return true;
		}
		

	}
}

