// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

using MonoDevelop.TextEditor.Document;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Core.Properties;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Gui.Dialogs;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.EditorBindings.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form9.
	/// </summary>

	public class MarkersTextEditorPanel : AbstractOptionPanel
	{
		MarkersTextEditorPanelWidget widget;
		
		public override void LoadPanelContents()
		{
			// set up the form controls instance
			Add (widget = new MarkersTextEditorPanelWidget ((IProperties) CustomizationObject));	
		}
		
		public override bool StorePanelContents()
		{
			widget.Store ((IProperties) CustomizationObject);
			return true;
		}

		class MarkersTextEditorPanelWidget : GladeWidgetExtract 
		{
			// Services
			FileUtilityService FileUtilityService = (
				FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			StringParserService StringParserService = (
				StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
			ResourceService ResourceService = (
				ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			// Gtk Controls
			[Glade.Widget] Label markersLabel;
			[Glade.Widget] Label characterMarkersLabel;
			[Glade.Widget] Label rulersLabel;
			[Glade.Widget] Label atColumnLabel;
			[Glade.Widget] CheckButton showLineNumberCheckBox;
			[Glade.Widget] CheckButton showInvalidLinesCheckBox;
			[Glade.Widget] CheckButton showBracketHighlighterCheckBox;
			[Glade.Widget] CheckButton showErrorsCheckBox;
			[Glade.Widget] CheckButton showHRulerCheckBox;
			[Glade.Widget] CheckButton showEOLMarkersCheckBox;
			[Glade.Widget] CheckButton showVRulerCheckBox;
			[Glade.Widget] CheckButton showTabCharsCheckBox;
			[Glade.Widget] CheckButton showSpaceCharsCheckBox;
			[Glade.Widget] SpinButton  vRulerRowTextBox;

			public MarkersTextEditorPanelWidget (IProperties CustomizationObject) :  
				base ("EditorBindings.glade", "MarkersTextEditorPanel")
			{
				// Load Text

				// FIXME i8n the following labels:
				markersLabel.Markup =  "<b>Markers</b> ";
				rulersLabel.Markup =  "<b>Rulers</b> ";
				characterMarkersLabel.TextWithMnemonic =  "Character Markers";

				atColumnLabel.TextWithMnemonic = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.AtRowLabel}") + ", ";
				showLineNumberCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.LineNumberCheckBox}");
				showInvalidLinesCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.InvalidLinesCheckBox}");
				showBracketHighlighterCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.HiglightBracketCheckBox}");
				showErrorsCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.UnderLineErrorsCheckBox}");
 				showHRulerCheckBox.Label = StringParserService.Parse(
 					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.HorizontalRulerCheckBox}");
 				showEOLMarkersCheckBox.Label = StringParserService.Parse(
 					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.EOLMarkersCheckBox}");
 				showVRulerCheckBox.Label = StringParserService.Parse(
 					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.VerticalRulerCheckBox}");
 				showTabCharsCheckBox.Label = StringParserService.Parse(
 					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.TabsCheckBox}");
 				showSpaceCharsCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Markers.SpacesCheckBox}");

				// Load Values

				showLineNumberCheckBox.Active         = ((IProperties)CustomizationObject).GetProperty(
					"ShowLineNumbers", true);
				showInvalidLinesCheckBox.Active       = ((IProperties)CustomizationObject).GetProperty(
					"ShowInvalidLines", true);
				showBracketHighlighterCheckBox.Active = ((IProperties)CustomizationObject).GetProperty(
					"ShowBracketHighlight", true);
				showErrorsCheckBox.Active             = ((IProperties)CustomizationObject).GetProperty(
					"ShowErrors", true);
				showHRulerCheckBox.Active             = ((IProperties)CustomizationObject).GetProperty(
					"ShowHRuler", false);
				showEOLMarkersCheckBox.Active         = ((IProperties)CustomizationObject).GetProperty(
					"ShowEOLMarkers", false);
				showVRulerCheckBox.Active             = ((IProperties)CustomizationObject).GetProperty(
					"ShowVRuler", false);
				showTabCharsCheckBox.Active           = ((IProperties)CustomizationObject).GetProperty(
					"ShowTabs", false);
				showSpaceCharsCheckBox.Active         = ((IProperties)CustomizationObject).GetProperty(
					"ShowSpaces", false);
			
				vRulerRowTextBox.Value = ((IProperties)CustomizationObject).GetProperty("VRulerRow", 80);

				// FIXME: reenable these widget's when they're implemented
				showInvalidLinesCheckBox.Sensitive = false;
				showHRulerCheckBox.Sensitive = false;
			}

			public void Store (IProperties CustomizationObject)
			{
				((IProperties)CustomizationObject).SetProperty("ShowInvalidLines",     showInvalidLinesCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowLineNumbers",      showLineNumberCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowBracketHighlight", showBracketHighlighterCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowErrors",           showErrorsCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowHRuler",           showHRulerCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowEOLMarkers",       showEOLMarkersCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowVRuler",           showVRulerCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowTabs",             showTabCharsCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty("ShowSpaces",           showSpaceCharsCheckBox.Active);
				
				try {
					((IProperties)CustomizationObject).SetProperty("VRulerRow", vRulerRowTextBox.Value);
				} 
				catch (Exception) {
				}
			}
		}
	}
}
