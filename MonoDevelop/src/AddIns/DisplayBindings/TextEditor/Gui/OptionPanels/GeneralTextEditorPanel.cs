// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Text;
using Gtk;
using Gnome;
using MonoDevelop.Gui.Widgets;
using Pango;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// General texteditor options panelS.
	/// </summary>
	public class GeneralTextEditorPanel : AbstractOptionPanel
	{

		GeneralTextEditorPanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new GeneralTextEditorPanelWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			widget.Store ((IProperties) CustomizationObject);
			return true;
		}
	
		class GeneralTextEditorPanelWidget : GladeWidgetExtract 
		{	
			StringParserService StringParserService = (
				StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
// 			int encoding = Encoding.UTF8.CodePage;
// 			int selectedIndex = 0;
			
			[Glade.Widget] Label genOptions, fontOptions;
// 					encOptions, encVBox; // if you uncoment change to "," above 
			[Glade.Widget] CheckButton enableCodeCompletionCheckBox, 
					enableFoldingCheckBox, enableDoublebufferingCheckBox;
// 			[Glade.Widget] OptionMenu textEncodingComboBox;
			[Glade.Widget] FontPicker fontNameDisplayTextBox;
			[Glade.Widget] VBox encodingBox;
			
			public GeneralTextEditorPanelWidget (IProperties CustomizationObject) :  base ("texteditoraddin.glade", "GeneralTextEditorPanel")
			{
				encodingBox.Destroy(); // this is a really dirty way of hiding encodingBox, but Hide() doesn't work
				genOptions.Markup = "<b> " + StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.General.GeneralOptionsGroupBox}" ) + "</b>";
				fontOptions.Markup = "<b> " + StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox}" ) + "</b>";
// 				encOptions.Markup = "<b> " + StringParserService.Parse(
// 					"${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox.FileEncodingGroupBox}" ) + "</b>";

				enableCodeCompletionCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.General.CodeCompletionCheckBox}");
				enableCodeCompletionCheckBox.Active = ((IProperties) CustomizationObject).GetProperty(
					"EnableCodeCompletion", true);
				
 				enableFoldingCheckBox.Label = StringParserService.Parse(
 					"${res:Dialog.Options.IDEOptions.TextEditor.General.FoldingCheckBox}");
 				enableFoldingCheckBox.Active = ((IProperties) CustomizationObject).GetProperty("EnableFolding", true);

				enableDoublebufferingCheckBox.Label = StringParserService.Parse(
					"${res:Dialog.Options.IDEOptions.TextEditor.General.DoubleBufferCheckBox}");
 				enableDoublebufferingCheckBox.Active = ((IProperties) CustomizationObject).GetProperty(
					"DoubleBuffer", true);

				fontNameDisplayTextBox.FontName = ((IProperties) CustomizationObject).GetProperty("DefaultFont", "Courier 12").ToString();
				
// 				encVBox.TextWithMnemonic = StringParserService.Parse(
// 					"${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox.FileEncodingLabel}");

// 				Menu m = new Menu ();
// 				foreach (String name in CharacterEncodings.Names) {
// 					m.Append (new MenuItem (name));
// 				}
// 				textEncodingComboBox.Menu = m;
				
// 				int i = 0;
// 				try {
// 					Console.WriteLine("Getting encoding Property");
// 					i = CharacterEncodings.GetEncodingIndex(
// 						(Int32)((IProperties) CustomizationObject).GetProperty("Encoding", encoding));
// 				} catch {
// 					Console.WriteLine("Getting encoding Default");
// 					i = CharacterEncodings.GetEncodingIndex(encoding);
// 				}
				
// 				selectedIndex = i;
// 				encoding = CharacterEncodings.GetEncodingByIndex(i).CodePage;

// 				textEncodingComboBox.Changed += new EventHandler (OnOptionChanged);
			}

			public void Store (IProperties CustomizationObject)
			{
				((IProperties) CustomizationObject).SetProperty (
					"DoubleBuffer", enableDoublebufferingCheckBox.Active);
				((IProperties) CustomizationObject).SetProperty (
					"EnableCodeCompletion", enableCodeCompletionCheckBox.Active);
				((IProperties) CustomizationObject).SetProperty (
					"EnableFolding", enableFoldingCheckBox.Active);
				((IProperties) CustomizationObject).SetProperty (
					"DefaultFont", fontNameDisplayTextBox.FontName);
// 				Console.WriteLine (CharacterEncodings.GetEncodingByIndex (selectedIndex).CodePage);
// 				((IProperties) CustomizationObject).SetProperty (
// 					"Encoding",CharacterEncodings.GetEncodingByIndex (selectedIndex).CodePage);
			}
			
// 			private void OnOptionChanged (object o, EventArgs args)
// 			{
// 				Console.WriteLine (selectedIndex);
// 				selectedIndex = ((OptionMenu) o).History;
// 			}

		}
	}
}
