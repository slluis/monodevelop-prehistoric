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
			[Glade.Widget] Entry ProjectNameEntry;
			[Glade.Widget] TextView ProjectDescriptionTextView;
			[Glade.Widget] CheckButton NewFilesOnLoadCheckButton;
 			[Glade.Widget] CheckButton AutoInsertNewFilesCheckButton;
 			[Glade.Widget] CheckButton EnableViewStateCheckButton;

			IProject project;

			public GeneralProjectOptionsWidget (IProperties CustomizationObject) : base ("Base.glade", "GeneralProjectOptionsPanel")
			{
				this.project = (IProject)((IProperties)CustomizationObject).GetProperty("Project");
				
				ProjectNameEntry.Text = project.Name;
				ProjectDescriptionTextView.Buffer.Text = project.Description;
				EnableViewStateCheckButton.Active = project.EnableViewState;
				
				switch (project.NewFileSearch) 
				{
				case NewFileSearch.None:
					NewFilesOnLoadCheckButton.Active = false; 
					AutoInsertNewFilesCheckButton.Active = false;
					break;
				case NewFileSearch.OnLoad:
					NewFilesOnLoadCheckButton.Active = true; 
					AutoInsertNewFilesCheckButton.Active = false;
					break;
				default:
					NewFilesOnLoadCheckButton.Active = true; 
					AutoInsertNewFilesCheckButton.Active = true;
					break;
				}
				
				NewFilesOnLoadCheckButton.Clicked += new EventHandler(AutoLoadCheckBoxCheckedChangeEvent);
				AutoLoadCheckBoxCheckedChangeEvent(null, null);
			
			}			
		void AutoLoadCheckBoxCheckedChangeEvent(object sender, EventArgs e)
		{
			AutoInsertNewFilesCheckButton.Sensitive = false;
			AutoInsertNewFilesCheckButton.Active = NewFilesOnLoadCheckButton.Active;
			
			// FIXME: leave this deselected and checked by default for 0.1 release, uncomment again after release
 			/*
			AutoInsertNewFilesCheckButton.Sensitive = NewFilesOnLoadCheckButton.Active;
			if (NewFilesOnLoadCheckButton.Active == false) 
			{
				AutoInsertNewFilesCheckButton.Active = false;
			}
			*/
		}

			
			public void  Store (IProperties CustomizationObject)
			{
				project.Name                 = ProjectNameEntry.Text;
				project.Description          = ProjectDescriptionTextView.Buffer.Text;
				project.EnableViewState      = EnableViewStateCheckButton.Active;
				
				if (NewFilesOnLoadCheckButton.Active) {
					project.NewFileSearch = AutoInsertNewFilesCheckButton.Active ?  NewFileSearch.OnLoadAutoInsert : NewFileSearch.OnLoad;
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

