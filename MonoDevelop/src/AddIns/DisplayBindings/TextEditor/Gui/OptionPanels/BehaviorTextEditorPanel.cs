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
using MonoDevelop.EditorBindings.FormattingStrategy;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form8.
	/// </summary>
	public class BehaviorTextEditorPanel : AbstractOptionPanel
	{
		// GTK controls
		CheckButton		autoinsertCurlyBraceCheckBox;
		CheckButton		hideMouseCursorCheckBox;
		CheckButton		caretBehindEOLCheckBox;
		CheckButton		auotInsertTemplatesCheckBox;
		CheckButton		convertTabsToSpacesCheckBox;
		Entry			tabSizeTextBox;
		Entry			indentSizeTextBox;
		Gtk.Menu		indentStyleComboBoxMenu;
		Gtk.OptionMenu	indentStyleComboBox;
		Gtk.Menu		mouseWhellDirectionComboBoxMenu;
		Gtk.OptionMenu	mouseWhellDirectionComboBox;
		
		// Services
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		
		public override void LoadPanelContents()
		{
			// set up the form controls instance
			SetupPanelInstance();			
			
			autoinsertCurlyBraceCheckBox.Active = ((IProperties)CustomizationObject).GetProperty("AutoInsertCurlyBracket", true);
			hideMouseCursorCheckBox.Active      = ((IProperties)CustomizationObject).GetProperty("HideMouseCursor", true);
			caretBehindEOLCheckBox.Active       = ((IProperties)CustomizationObject).GetProperty("CursorBehindEOL", false);
			auotInsertTemplatesCheckBox.Active  = ((IProperties)CustomizationObject).GetProperty("AutoInsertTemplates", true);
			
			convertTabsToSpacesCheckBox.Active  = ((IProperties)CustomizationObject).GetProperty("TabsToSpaces", false);
			
			tabSizeTextBox.Text    = ((IProperties)CustomizationObject).GetProperty("TabIndent", 4).ToString();
			indentSizeTextBox.Text = ((IProperties)CustomizationObject).GetProperty("IndentationSize", 4).ToString();
			
			indentStyleComboBoxMenu.Append(MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}")));
			indentStyleComboBoxMenu.Append(MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic}")));
			indentStyleComboBoxMenu.Append(MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart}")));
			
			indentStyleComboBox.SetHistory((uint)(IndentStyle)((IProperties)CustomizationObject).GetProperty("IndentStyle", IndentStyle.Smart));
		
			mouseWhellDirectionComboBoxMenu.Append(MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.NormalMouseDirectionRadioButton}")));
			mouseWhellDirectionComboBoxMenu.Append(MenuItem.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.ReverseMouseDirectionRadioButton}")));
			int selectedIndex = ((IProperties)CustomizationObject).GetProperty("MouseWheelScrollDown", true) ? 0 : 1;
			mouseWhellDirectionComboBox.SetHistory((uint)selectedIndex);
		}
		
		public override bool StorePanelContents()
		{
			((IProperties)CustomizationObject).SetProperty("TabsToSpaces",         convertTabsToSpacesCheckBox.Active);
			((IProperties)CustomizationObject).SetProperty("MouseWheelScrollDown", mouseWhellDirectionComboBox.History == 0);
			
			((IProperties)CustomizationObject).SetProperty("AutoInsertCurlyBracket", autoinsertCurlyBraceCheckBox.Active);
			((IProperties)CustomizationObject).SetProperty("HideMouseCursor",        hideMouseCursorCheckBox.Active);
			((IProperties)CustomizationObject).SetProperty("CursorBehindEOL",        caretBehindEOLCheckBox.Active);
			((IProperties)CustomizationObject).SetProperty("AutoInsertTemplates",    auotInsertTemplatesCheckBox.Active);
			
			((IProperties)CustomizationObject).SetProperty("IndentStyle", indentStyleComboBox.History);
			
			try {
				int tabSize = Int32.Parse(tabSizeTextBox.Text);
				
				// FIX: don't allow to set tab size to zero as this will cause divide by zero exceptions in the text control.
				// Zero isn't a setting that makes sense, anyway.
				if (tabSize > 0) {
					((IProperties)CustomizationObject).SetProperty("TabIndent", tabSize);
				}
			} catch (Exception) {
			}
			
			try {
				((IProperties)CustomizationObject).SetProperty("IndentationSize", Int32.Parse(indentSizeTextBox.Text));
			} catch (Exception) {
			}
			return true;
		}
		
		#region jba added methods
		
		private void SetupPanelInstance()
		{
			Gtk.Frame frame1 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TabsGroupBox}"));
			//
			// set up the Tab
			//
			// instantiate all the controls
			Gtk.VBox vBox1 = new Gtk.VBox(false,2);
			Gtk.Label label1 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TabSizeLabel}"));
			Gtk.Label label2 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentSizeLabel}"));
			Gtk.Label label3 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentLabel}"));
			tabSizeTextBox = new Entry();
			indentSizeTextBox = new Entry();
			indentStyleComboBoxMenu = new Menu();
			indentStyleComboBox = new OptionMenu();
			indentStyleComboBox.Menu = indentStyleComboBoxMenu;
			convertTabsToSpacesCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.ConvertTabsToSpacesCheckBox}"));
				// table to pack the tab settings options 
				Table table1 = new Table(4, 2, false);
				//pack the table
				table1.Attach(label1, 0, 1, 0, 1);
				table1.Attach(tabSizeTextBox, 1, 2, 0, 1);
				table1.Attach(label2, 0, 1, 1, 2);
				table1.Attach(indentSizeTextBox, 1, 2, 1, 2);
				table1.Attach(label3, 2, 4, 0, 1);
				table1.Attach(indentStyleComboBox, 2, 4, 1, 2);
				
			// pack them all
			vBox1.PackStart(table1, false, false, 2);
			vBox1.PackStart(convertTabsToSpacesCheckBox, false, false, 2);
			frame1.Add(vBox1);
			
			//
			// setup the behaviour options
			//
			//instantiate all the controls			
			Gtk.Frame frame2 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.BehaviourGroupBox}"));
			Gtk.VBox vBox2 = new Gtk.VBox(false, 2);
			
			
			autoinsertCurlyBraceCheckBox = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.CurlyBracketCheckBox}"));
			hideMouseCursorCheckBox  = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.HideMouseCheckBox}"));
			caretBehindEOLCheckBox  = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.BehindEOLCheckBox}"));
			auotInsertTemplatesCheckBox  = new CheckButton(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.TemplateInsertCheckBox}"));			
			Label label4 = new Gtk.Label(StringParserService.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.MouseWhellGroupBox}"));
			mouseWhellDirectionComboBoxMenu = new Menu();
			mouseWhellDirectionComboBox = new OptionMenu();
			mouseWhellDirectionComboBox.Menu = mouseWhellDirectionComboBoxMenu;
			// pack them all
			vBox2.PackStart(caretBehindEOLCheckBox, false, false, 2);
			vBox2.PackStart(auotInsertTemplatesCheckBox, false, false, 2);
			vBox2.PackStart(autoinsertCurlyBraceCheckBox, false, false, 2);
			vBox2.PackStart(hideMouseCursorCheckBox, false, false, 2);
			vBox2.PackStart(label4, false, false, 2);
			vBox2.PackStart(mouseWhellDirectionComboBox, false, false, 2);
			frame2.Add(vBox2);
			
			// create the main box
			Gtk.VBox mainBox = new Gtk.VBox(false, 2);
			mainBox.PackStart(frame1, false, false, 2);
			mainBox.PackStart(frame2, false, false, 2);
			
			this.Add(mainBox);
		}
		
		#endregion
	}
}
