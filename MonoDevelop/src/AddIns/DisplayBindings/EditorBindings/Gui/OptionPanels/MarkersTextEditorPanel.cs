// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using Gtk;

namespace MonoDevelop.EditorBindings.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form9.
	/// </summary>
	public class MarkersTextEditorPanel : AbstractOptionPanel
	{
		// Gtk Controls
		CheckButton showLineNumberCheckBox;
		CheckButton showInvalidLinesCheckBox;
		CheckButton showBracketHighlighterCheckBox;
		CheckButton showErrorsCheckBox;
		CheckButton showHRulerCheckBox;
		CheckButton showEOLMarkersCheckBox;
		CheckButton showVRulerCheckBox;
		CheckButton showTabCharsCheckBox;
		CheckButton showSpaceCharsCheckBox;
		Entry vRulerRowTextBox;
		OptionMenu lineMarkerStyleComboBox;
		Menu lineMarkerStyleComboBoxMenu;
		
		// Services
		FileUtilityService FileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		ResourceService ResourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public override void LoadPanelContents()
		{
			// set up the form controls instance
			SetupPanelInstance();
			
			showLineNumberCheckBox.Active         = ((IProperties)CustomizationObject).GetProperty("ShowLineNumbers", true);
			showInvalidLinesCheckBox.Active       = ((IProperties)CustomizationObject).GetProperty("ShowInvalidLines", true);
			showBracketHighlighterCheckBox.Active = ((IProperties)CustomizationObject).GetProperty("ShowBracketHighlight", true);
			showErrorsCheckBox.Active             = ((IProperties)CustomizationObject).GetProperty("ShowErrors", true);
			showHRulerCheckBox.Active             = ((IProperties)CustomizationObject).GetProperty("ShowHRuler", false);
			showEOLMarkersCheckBox.Active         = ((IProperties)CustomizationObject).GetProperty("ShowEOLMarkers", false);
			showVRulerCheckBox.Active             = ((IProperties)CustomizationObject).GetProperty("ShowVRuler", false);
			showTabCharsCheckBox.Active           = ((IProperties)CustomizationObject).GetProperty("ShowTabs", false);
			showSpaceCharsCheckBox.Active         = ((IProperties)CustomizationObject).GetProperty("ShowSpaces", false);
			
			vRulerRowTextBox.Text = ((IProperties)CustomizationObject).GetProperty("VRulerRow", 80).ToString();
			
			lineMarkerStyleComboBoxMenu.Append(MenuItem.NewWithLabel(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.None")));
			lineMarkerStyleComboBoxMenu.Append(MenuItem.NewWithLabel(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.FullRow")));
			//lineMarkerStyleComboBox.SetHistory((uint)(LineViewerStyle)((IProperties)CustomizationObject).GetProperty("LineViewerStyle", LineViewerStyle.None));
		}
		
		public override bool StorePanelContents()
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
				((IProperties)CustomizationObject).SetProperty("VRulerRow", Int32.Parse(vRulerRowTextBox.Text));
			} catch (Exception) {
			}
			
			//FIXME: This is commented out for right now
			//((IProperties)CustomizationObject).SetProperty("LineViewerStyle", (LineViewerStyle)lineMarkerStyleComboBox.History);
			
			return true;
		}
		
		#region jba added methods
		
		private void SetupPanelInstance()
		{
			Gtk.Frame frame1 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.MarkersGroupBox}"));
			//
			// set up the Tab
			//
			// instantiate all the controls
			Gtk.VBox vBox1 = new Gtk.VBox(false,2);
			Gtk.Label label1 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.AtRowLabel}"));
			Gtk.Label label2 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.LineMarkerLabel}"));
			showLineNumberCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.LineNumberCheckBox}"));
			showInvalidLinesCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.InvalidLinesCheckBox}"));
			showBracketHighlighterCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.HiglightBracketCheckBox}"));
			showErrorsCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.UnderLineErrorsCheckBox}"));
			showHRulerCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.HorizontalRulerCheckBox}"));
			showEOLMarkersCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.EOLMarkersCheckBox}"));
			showVRulerCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.VerticalRulerCheckBox}"));
			showTabCharsCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.TabsCheckBox}"));
			showSpaceCharsCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Markers.SpacesCheckBox}"));
			vRulerRowTextBox = new Entry();
			lineMarkerStyleComboBox = new OptionMenu();
			lineMarkerStyleComboBoxMenu = new Menu();
			lineMarkerStyleComboBox.Menu = lineMarkerStyleComboBoxMenu;	
			
				// table to pack the tab settings options 
				Table table1 = new Table(3, 2, false);
				//pack the table
				table1.Attach(showHRulerCheckBox, 0, 1, 0, 1);
				table1.Attach(showVRulerCheckBox, 0, 1, 1, 2);
				table1.Attach(label1, 1, 2, 1, 2);
				table1.Attach(vRulerRowTextBox, 2, 3, 1, 2);
				
			// pack them all
			vBox1.PackStart(label1, false, false, 2);
			vBox1.PackStart(table1, false, false, 2);
			vBox1.PackStart(label2, false, false, 2);
			vBox1.PackStart(lineMarkerStyleComboBox, false, false, 2);
			vBox1.PackStart(showLineNumberCheckBox, false, false, 2);
			vBox1.PackStart(showErrorsCheckBox, false, false, 2);
			vBox1.PackStart(showBracketHighlighterCheckBox, false, false, 2);
			vBox1.PackStart(showInvalidLinesCheckBox, false, false, 2);
			vBox1.PackStart(showEOLMarkersCheckBox, false, false, 2);
			vBox1.PackStart(showSpaceCharsCheckBox, false, false, 2);
			vBox1.PackStart(showTabCharsCheckBox, false, false, 2);
			frame1.Add(vBox1);
			
			this.Add(frame1);
		}
		
		#endregion
	}
}
