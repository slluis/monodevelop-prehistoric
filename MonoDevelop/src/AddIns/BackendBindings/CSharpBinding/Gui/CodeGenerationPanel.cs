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

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		// Gtk Controls
		CheckButton generateAdditonalCommentsCheckBox;
		CheckButton generateDocCommentsCheckBox;
		CheckButton useFullTypeNamesCheckBox; 
		CheckButton blankLinesBetweenMemberCheckBox;
		CheckButton elseOnClosingCheckBox;
		CheckButton startBlockOnTheSameLineCheckBox; 
		
		// Services
		StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
		PropertyService p = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		static readonly string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		public override void LoadPanelContents()
		{
			// set up the form controls instance
			SetupPanelInstance();
			
			
			generateAdditonalCommentsCheckBox.Active = p.GetProperty("GenerateAdditionalComments", true);
			generateDocCommentsCheckBox.Active       = p.GetProperty("GenerateDocumentComments", true);
			useFullTypeNamesCheckBox.Active          = p.GetProperty("UseFullyQualifiedNames", true);
			
			blankLinesBetweenMemberCheckBox.Active   = p.GetProperty("BlankLinesBetweenMembers", true);
			elseOnClosingCheckBox.Active             = p.GetProperty("ElseOnClosing", true);
			startBlockOnTheSameLineCheckBox.Active   = p.GetProperty("StartBlockOnSameLine", true);
		}
		
		public override bool StorePanelContents()
		{
			p.SetProperty("GenerateAdditionalComments", generateAdditonalCommentsCheckBox.Active);
			p.SetProperty("GenerateDocumentComments",   generateDocCommentsCheckBox.Active);
			p.SetProperty("UseFullyQualifiedNames",     useFullTypeNamesCheckBox.Active);
			p.SetProperty("BlankLinesBetweenMembers",   blankLinesBetweenMemberCheckBox.Active);
			p.SetProperty("ElseOnClosing",              elseOnClosingCheckBox.Active);
			p.SetProperty("StartBlockOnSameLine",       startBlockOnTheSameLineCheckBox.Active);
			return true;
		}
		
		private void SetupPanelInstance()
		{
			// instantiate all the controls in the first group
			Gtk.Frame frame1 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.CodeGenerationOptionsGroupBox}"));
			Gtk.VBox vBox1 = new Gtk.VBox(false,2);			
			useFullTypeNamesCheckBox = CheckButton.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.UseFullTypeNamesCheckBox}")); 
			blankLinesBetweenMemberCheckBox = CheckButton.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.BlankLinesBetweenMembersCheckBox}"));
			elseOnClosingCheckBox = CheckButton.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.ElseOnClosingCheckBox}"));
			startBlockOnTheSameLineCheckBox = CheckButton.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.StartBlockOnTheSameLineCheckBox}")); 
			
			// pack all controls in first group
			vBox1.PackStart(startBlockOnTheSameLineCheckBox, false, false, 2);
			vBox1.PackStart(elseOnClosingCheckBox, false, false, 2);
			vBox1.PackStart(blankLinesBetweenMemberCheckBox, false, false, 2);
			vBox1.PackStart(useFullTypeNamesCheckBox, false, false, 2);
			frame1.Add(vBox1);
			
			// instantiate all the controls in the second group
			Gtk.Frame frame2 = new Gtk.Frame(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.CommentGenerationOptionsGroupBox}"));
			Gtk.VBox vBox2 = new Gtk.VBox(false,2);			
			generateAdditonalCommentsCheckBox = CheckButton.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.GenerateAdditionalCommentsCheckBox}"));
			generateDocCommentsCheckBox = CheckButton.NewWithLabel(StringParserService.Parse("${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.GenerateDocCommentsCheckBox}"));
			
			// pack all controls in second group
			vBox2.PackStart(generateDocCommentsCheckBox, false, false, 2);
			vBox2.PackStart(generateAdditonalCommentsCheckBox, false, false, 2);
			frame2.Add(vBox2);
			
			// pack all the groups
			Gtk.VBox mainBox = new Gtk.VBox(false,2);
			mainBox.PackStart(frame1, false, false, 2);
			mainBox.PackStart(frame2, false, false, 2);
			this.Add(mainBox);
		}

	}
}
