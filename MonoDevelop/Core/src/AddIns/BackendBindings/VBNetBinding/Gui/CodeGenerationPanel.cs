// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

using Gtk;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Services;

namespace VBBinding
{
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters = null;
	
		/*
		
		ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null) {
					return true;
				}
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.GetService(typeof(FileUtilityService));
				
				
				compilerParameters.DefineSymbols = ControlDictionary["symbolsTextBox"].Text;
				compilerParameters.MainClass     = ControlDictionary["mainClassTextBox"].Text;
				compilerParameters.Imports       = ControlDictionary["importsTextBox"].Text;
				compilerParameters.RootNamespace = ControlDictionary["RootNamespaceTextBox"].Text;
				
				compilerParameters.Debugmode = ((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked;
				compilerParameters.Optimize = ((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked;
				compilerParameters.GenerateOverflowChecks = ((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked;
				compilerParameters.TreatWarningsAsErrors  = ((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked;
				
				compilerParameters.OptionExplicit = ((CheckBox)ControlDictionary["optionExplicitCheckBox"]).Checked ;
				compilerParameters.OptionStrict = ((CheckBox)ControlDictionary["optionStrictCheckBox"]).Checked;
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			ControlDictionary["symbolsTextBox"].Text = compilerParameters.DefineSymbols;
			ControlDictionary["mainClassTextBox"].Text = compilerParameters.MainClass;
			ControlDictionary["importsTextBox"].Text = compilerParameters.Imports;
			ControlDictionary["RootNamespaceTextBox"].Text = compilerParameters.RootNamespace;
			
			
			((CheckBox)ControlDictionary["generateDebugInformationCheckBox"]).Checked = compilerParameters.Debugmode;
			((CheckBox)ControlDictionary["enableOptimizationCheckBox"]).Checked = compilerParameters.Optimize;
			((CheckBox)ControlDictionary["generateOverflowChecksCheckBox"]).Checked = compilerParameters.GenerateOverflowChecks;
			((CheckBox)ControlDictionary["warningsAsErrorsCheckBox"]).Checked = compilerParameters.TreatWarningsAsErrors;
			
			((CheckBox)ControlDictionary["optionExplicitCheckBox"]).Checked = compilerParameters.OptionExplicit;
			((CheckBox)ControlDictionary["optionStrictCheckBox"]).Checked = compilerParameters.OptionStrict;
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public CodeGenerationPanel() : base(propertyService.DataDirectory + @"\resources\panels\ProjectOptions\VBNetCodeGenerationPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
			
		}
		*/
		
			/* public override bool ReceiveDialogMessage(DialogMessage message)
			{
				if (message == DialogMessage.OK) {
					return widget.Store();
				}
				return true;
			}

		
			void SetValues(object sender, EventArgs e){
				LoadPanelContents();		
			}
			
		public CodeGenerationPanel() : base()
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
		} */


		
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
//			[Glade.Widget] CheckButton generateDebugInformationCheckButton;
 			[Glade.Widget] CheckButton generateXmlOutputCheckButton;
 			[Glade.Widget] SpinButton warningLevelSpinButton;

			//
			// services needed
			//
			StringParserService StringParserService = (StringParserService)ServiceManager.GetService (
				typeof (StringParserService));

			VBCompilerParameters compilerParameters = null;
			
			//static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			

 			public  CodeGenerationPanelWidget(IProperties CustomizationObject) : base ("VB.glade", "CodeGenerationPanel")
 			{	
 				compilerParameters=(VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
				SetValues();
				//CustomizationObjectChanged += new EventHandler(SetValues);
 			}
 			
			public void SetValues(){
				//this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");

				// FIXME: Enable when mbas has this feature
				generateXmlOutputCheckButton.Sensitive = false;
				

				//string[] compileTargets=new string[]{GettextCatalog.GetString ("Executable"),GettextCatalog.GetString("WinEXE"),	GettextCatalog.GetString ("Library"),GettextCatalog.GetString ("Module")};

				Menu CompileTargetMenu = new Menu ();
				CompileTargetMenu.Add(new MenuItem(GettextCatalog.GetString ("Executable")));
				CompileTargetMenu.Add(new MenuItem(GettextCatalog.GetString("WinEXE")));
				CompileTargetMenu.Add(new MenuItem(GettextCatalog.GetString ("Library")));
				CompileTargetMenu.Add(new MenuItem(GettextCatalog.GetString ("Module"))); 
				// FIXME commented until the Module capability is ported
// 				CompileTargetMenu.Append(new MenuItem(
// 								 StringParserService.Parse(
// 									 "${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}")));

				CompileTargetOptionMenu.Menu=CompileTargetMenu;
				CompileTargetOptionMenu.SetHistory ( (uint) compilerParameters.CompileTarget);
				//CompileTargetOptionMenu.Active=(int)compilerParameters.CompileTarget;

				symbolsEntry.Text = compilerParameters.DefineSymbols;
				mainClassEntry.Text = compilerParameters.MainClass;

//				generateDebugInformationCheckButton.Active = compilerParameters.Debugmode;
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
					System.Console.WriteLine("NULL compiler parameters for VBNet!");
					return true;
				}
				//compilerParameters.CompileTarget =  (CompileTarget)  CompileTargetOptionMenu.History;
				compilerParameters.CompileTarget=(CompileTarget)CompileTargetOptionMenu.History;
				compilerParameters.DefineSymbols =  symbolsEntry.Text;
				compilerParameters.MainClass     =  mainClassEntry.Text;

//				compilerParameters.Debugmode                = generateDebugInformationCheckButton.Active;
				compilerParameters.GenerateXmlDocumentation = generateXmlOutputCheckButton.Active;
				compilerParameters.Optimize                 = enableOptimizationCheckButton.Active;
				compilerParameters.UnsafeCode               = allowUnsafeCodeCheckButton.Active;
				compilerParameters.GenerateOverflowChecks   = generateOverflowChecksCheckButton.Active;
				compilerParameters.RunWithWarnings          = ! warningsAsErrorsCheckButton.Active;

				compilerParameters.WarningLevel = warningLevelSpinButton.ValueAsInt;

				return true;
			}
		}//CodeGenerationPanelWidget
				
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
