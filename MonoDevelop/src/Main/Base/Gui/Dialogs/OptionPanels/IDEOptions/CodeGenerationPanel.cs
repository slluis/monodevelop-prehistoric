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
using MonoDevelop.Gui.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels {
	public class CodeGenerationPanel : AbstractOptionPanel {
		
		class CodeGenerationPanelWidget : GladeWidgetExtract {
			PropertyService p = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
			
			[Glade.Widget] Label hdr_code_generation_options;
			
			[Glade.Widget] CheckButton
				chk_blk_on_same_line,
				chk_else_on_same_line,
				chk_blank_lines,
				chk_full_type_names;
			
			[Glade.Widget] Label hdr_comment_generation_options;
			
			[Glade.Widget] CheckButton
				chk_doc_comments,
				chk_other_comments;
			
			public CodeGenerationPanelWidget () : base ("Base.glade", "CodeGenerationOptionsPanel")
			{
				i18nizeHeader (hdr_code_generation_options, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.CodeGenerationOptionsGroupBox}");
				i18nizeHeader (hdr_comment_generation_options, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.CommentGenerationOptionsGroupBox}");
				
				i18nize (chk_blk_on_same_line, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.StartBlockOnTheSameLineCheckBox}");
				i18nize (chk_else_on_same_line, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.ElseOnClosingCheckBox}");
				i18nize (chk_blank_lines, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.BlankLinesBetweenMembersCheckBox}");
				i18nize (chk_full_type_names, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.UseFullTypeNamesCheckBox}");
				
				i18nize (chk_doc_comments, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.GenerateDocCommentsCheckBox}");
				i18nize (chk_other_comments, "${res:Dialog.Options.IDEOptions.CodeGenerationOptionsPanel.GenerateAdditionalCommentsCheckBox}");
				
				chk_blk_on_same_line.Active   = p.GetProperty("StartBlockOnSameLine", true);
				chk_else_on_same_line.Active  = p.GetProperty("ElseOnClosing", true);
				chk_blank_lines.Active        = p.GetProperty("BlankLinesBetweenMembers", true);
				chk_full_type_names.Active    = p.GetProperty("UseFullyQualifiedNames", true);
			
				chk_doc_comments.Active       = p.GetProperty("GenerateDocumentComments", true);
				chk_other_comments.Active     = p.GetProperty("GenerateAdditionalComments", true);
				
				chk_blk_on_same_line.Sensitive = false;
				chk_else_on_same_line.Sensitive = false;
				chk_blank_lines.Sensitive = false;
				chk_full_type_names.Sensitive = false;
				chk_doc_comments.Sensitive = false;
				chk_other_comments.Sensitive = false;
			}
			
			public void Store ()
			{
				p.SetProperty ("StartBlockOnSameLine",       chk_blk_on_same_line.Active);
				p.SetProperty ("ElseOnClosing",              chk_else_on_same_line.Active);
				p.SetProperty ("BlankLinesBetweenMembers",   chk_blank_lines.Active);
				p.SetProperty ("UseFullyQualifiedNames",     chk_full_type_names.Active);
				
				p.SetProperty ("GenerateDocumentComments",   chk_doc_comments.Active);
				p.SetProperty ("GenerateAdditionalComments", chk_other_comments.Active);
			}
			
			void i18nizeHeader (Label l, string key)
			{
				// TODO: use the real pango stuff
				// otherwise, excaping is a problem
				l.Markup = "<b>" + StringParserService.Parse (key) + "</b>";
				
			}
			
			void i18nize (CheckButton c, string key)
			{
				c.Label = StringParserService.Parse (key);
			}
		}
		
		CodeGenerationPanelWidget widget;
		
		PropertyService PropertyService = (PropertyService)ServiceManager.Services.GetService (typeof (PropertyService));
		const string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		
		public override void LoadPanelContents ()
		{
			Add (widget = new CodeGenerationPanelWidget ());
		}
		
		public override bool StorePanelContents ()
		{
			widget.Store ();
			return true;
		}
	}
}
