// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using Gtk;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.EditorBindings.FormattingStrategy;

namespace MonoDevelop.EditorBindings.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form8.
	/// </summary>
	public class BehaviorTextEditorPanel : AbstractOptionPanel
	{
		BehaviorTextEditorPanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new BehaviorTextEditorPanelWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			widget.Store ((IProperties) CustomizationObject);
			return true;
		}
		
		class BehaviorTextEditorPanelWidget : GladeWidgetExtract 
		{
			// Services
			StringParserService StringParserService = (
				StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
			
			// GTK controls
			[Glade.Widget] Label			tabsGroupBoxLabel;
			[Glade.Widget] Label			behaviourGroupBoxLabel;
			[Glade.Widget] Label			tabSizeLabel;
			[Glade.Widget] Label			indentSizeLabel;
			[Glade.Widget] Label			indentLabel;
			[Glade.Widget] CheckButton		autoinsertCurlyBraceCheckBox;
			[Glade.Widget] CheckButton		hideMouseCursorCheckBox;
			[Glade.Widget] CheckButton		caretBehindEOLCheckBox;
			[Glade.Widget] CheckButton		autoInsertTemplatesCheckBox;
			[Glade.Widget] CheckButton		convertTabsToSpacesCheckBox;
			[Glade.Widget] RadioButton              noneIndentStyle;
			[Glade.Widget] RadioButton              automaticIndentStyle;
			[Glade.Widget] RadioButton              smartIndentStyle;
			[Glade.Widget] SpinButton               indentAndTabSizeSpinButton;
			
			public BehaviorTextEditorPanelWidget (IProperties CustomizationObject) :  
				base ("EditorBindings.glade", "BehaviorTextEditorPanel")
			{
				// Set up Text
				tabsGroupBoxLabel.Markup = "<b>" + StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TabsGroupBox}")  + "</b>";
				tabSizeLabel.TextWithMnemonic = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TabSizeLabel}");
				indentSizeLabel.TextWithMnemonic = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentSizeLabel}");
				indentLabel.TextWithMnemonic = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentLabel}");
				
				behaviourGroupBoxLabel.Markup = "<b>" +  StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.BehaviourGroupBox}") + "</b>";
				autoinsertCurlyBraceCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.CurlyBracketCheckBox}");
				hideMouseCursorCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.HideMouseCheckBox}");
				caretBehindEOLCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.BehindEOLCheckBox}");
				autoInsertTemplatesCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TemplateInsertCheckBox}");
				convertTabsToSpacesCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.ConvertTabsToSpacesCheckBox}");			
				
				// Set up Value
				autoinsertCurlyBraceCheckBox.Active = ((IProperties)CustomizationObject).GetProperty(
					"AutoInsertCurlyBracket", true);
				hideMouseCursorCheckBox.Active      = ((IProperties)CustomizationObject).GetProperty(
					"HideMouseCursor", true);
				caretBehindEOLCheckBox.Active       = ((IProperties)CustomizationObject).GetProperty(
					"CursorBehindEOL", false);
				autoInsertTemplatesCheckBox.Active  = ((IProperties)CustomizationObject).GetProperty(
					"AutoInsertTemplates", true);
				convertTabsToSpacesCheckBox.Active  = ((IProperties)CustomizationObject).GetProperty(
					"TabsToSpaces", false);
				
				//FIXME: Only one of these should be selected to hold the value
				indentAndTabSizeSpinButton.Value  = ((IProperties)CustomizationObject).GetProperty(
					"TabIndent", 4);
				indentAndTabSizeSpinButton.Value = ((IProperties)CustomizationObject).GetProperty(
					"IndentationSize", 4);

				if (IndentStyle.None.Equals(
					    (IndentStyle) ((IProperties)CustomizationObject).GetProperty(
						    "IndentStyle", IndentStyle.Smart))){
					noneIndentStyle.Active = true;
				}
				else if (IndentStyle.Auto.Equals(
						 (IndentStyle) ((IProperties)CustomizationObject).GetProperty(
							 "IndentStyle", IndentStyle.Smart))){
					automaticIndentStyle.Active = true;
				}
				else if (IndentStyle.Smart.Equals(
						 (IndentStyle) ((IProperties)CustomizationObject).GetProperty(
							 "IndentStyle", IndentStyle.Smart))){
					smartIndentStyle.Active = true;
				}
			}

			public void Store (IProperties CustomizationObject)
			{
				((IProperties)CustomizationObject).SetProperty(
					"TabsToSpaces",           convertTabsToSpacesCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty(
					"AutoInsertCurlyBracket", autoinsertCurlyBraceCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty(
					"HideMouseCursor",        hideMouseCursorCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty(
					"CursorBehindEOL",        caretBehindEOLCheckBox.Active);
				((IProperties)CustomizationObject).SetProperty(
					"AutoInsertTemplates",    autoInsertTemplatesCheckBox.Active);

				if (noneIndentStyle.Active)
					((IProperties)CustomizationObject).SetProperty("IndentStyle", IndentStyle.None);
				else if (automaticIndentStyle.Active)
					((IProperties)CustomizationObject).SetProperty("IndentStyle", IndentStyle.Auto);
				else if (smartIndentStyle.Active)
					((IProperties)CustomizationObject).SetProperty("IndentStyle", IndentStyle.Smart);
				
				//FIXME: Only one of these should be selected to save the value
				((IProperties)CustomizationObject).SetProperty("TabIndent", indentAndTabSizeSpinButton.Value);
				((IProperties)CustomizationObject).SetProperty("IndentationSize", indentAndTabSizeSpinButton.Value);
			}
		}
	}
}
