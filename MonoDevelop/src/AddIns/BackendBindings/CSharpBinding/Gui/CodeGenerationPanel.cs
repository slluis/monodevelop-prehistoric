// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using Gtk;
using MonoDevelop.Gui.Widgets;

namespace CSharpBinding
{

	//FIXME  i8n 

	public class CodeGenerationPanel : AbstractOptionPanel
	{

		class CodeGenerationPanelWidget : GladeWidgetExtract 
		{
			//
			// Gtk Controls	
			//
 			[Glade.Widget] Entry symbolsEntry;
 			[Glade.Widget] Entry mainClassEntry;
			[Glade.Widget] OptionMenu CompileTargetOptionMenu;
 			[Glade.Widget] CheckButton generateOverflowChecksCheckButton;
			[Glade.Widget] CheckButton allowUnsafeCodeCheckButton;
 			[Glade.Widget] CheckButton enableOptimizationCheckButton;
 			[Glade.Widget] CheckButton warningsAsErrorsCheckButton;
 			[Glade.Widget] CheckButton generateDebugInformationCheckButton;
 			[Glade.Widget] CheckButton generateXmlOutputCheckButton;
 			[Glade.Widget] SpinButton warningLevelSpinButton;

			//
			// services needed
			//
			StringParserService StringParserService = (StringParserService)ServiceManager.Services.GetService (
				typeof (StringParserService));

			CSharpCompilerParameters compilerParameters = null;

 			public  CodeGenerationPanelWidget(IProperties CustomizationObject) : base ("CSharp.glade", "CodeGenerationPanel")
 			{	
				this.compilerParameters = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");

				Menu CompileTargetMenu = new Menu ();
				CompileTargetMenu.Append(new MenuItem(
								 StringParserService.Parse(
									 "${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}")));

				CompileTargetMenu.Append(new MenuItem(
								 StringParserService.Parse(
									 "${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}")));
				// FIXME commented until the Module capability is ported
// 				CompileTargetMenu.Append(new MenuItem(
// 								 StringParserService.Parse(
// 									 "${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}")));

				CompileTargetOptionMenu.Menu = CompileTargetMenu;
				CompileTargetOptionMenu.SetHistory ( (uint) compilerParameters.CompileTarget);


				symbolsEntry.Text = compilerParameters.DefineSymbols;
				mainClassEntry.Text = compilerParameters.MainClass;

				generateDebugInformationCheckButton.Active = compilerParameters.Debugmode;
				generateXmlOutputCheckButton.Active        = compilerParameters.GenerateXmlDocumentation;
				enableOptimizationCheckButton.Active       = compilerParameters.Optimize;
				allowUnsafeCodeCheckButton.Active          = compilerParameters.UnsafeCode;
				generateOverflowChecksCheckButton.Active   = compilerParameters.GenerateOverflowChecks;
				warningsAsErrorsCheckButton.Active         = ! compilerParameters.RunWithWarnings;
				warningLevelSpinButton.Value               = compilerParameters.WarningLevel;				
 			}

			public bool Store ()
			{	
				if (compilerParameters == null) {
					return true;
				}
				compilerParameters.CompileTarget =  (CompileTarget)  CompileTargetOptionMenu.History;
				compilerParameters.DefineSymbols =  symbolsEntry.Text;
				compilerParameters.MainClass     =  mainClassEntry.Text;

				compilerParameters.Debugmode                = generateDebugInformationCheckButton.Active;
				compilerParameters.GenerateXmlDocumentation = generateXmlOutputCheckButton.Active;
				compilerParameters.Optimize                 = enableOptimizationCheckButton.Active;
				compilerParameters.UnsafeCode               = allowUnsafeCodeCheckButton.Active;
				compilerParameters.GenerateOverflowChecks   = generateOverflowChecksCheckButton.Active;
				compilerParameters.RunWithWarnings          = ! warningsAsErrorsCheckButton.Active;

				compilerParameters.WarningLevel = warningLevelSpinButton.ValueAsInt;

				return true;
			}
		}

		CodeGenerationPanelWidget widget;
		
		public override void LoadPanelContents()
		{
			Add (widget = new  CodeGenerationPanelWidget ((IProperties) CustomizationObject));
		}
		
		public override bool StorePanelContents()
		{
			bool result = true;
			result = widget.Store ();
 			return result;
		}
	}
}
