// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Core.Services;

using Gtk;

namespace CSharpBinding
{
	public class ChooseRuntimePanel : AbstractOptionPanel
	{
		CSharpCompilerParameters config = null;
		// FIXME: set the right rb groups
		RadioButton msnetRadioButton = new RadioButton ("Msnet");
		RadioButton monoRadioButton = new RadioButton ("Mono");
		RadioButton mintRadioButton = new RadioButton ("Mint");
		RadioButton cscRadioButton = new RadioButton ("CSC");
		RadioButton mcsRadioButton = new RadioButton ("MCS");
		
		public override void LoadPanelContents()
		{
			this.config = (CSharpCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			msnetRadioButton.Active = config.NetRuntime == NetRuntime.MsNet;
			monoRadioButton.Active  = config.NetRuntime == NetRuntime.Mono;
			mintRadioButton.Active  = config.NetRuntime == NetRuntime.MonoInterpreter;
			
			cscRadioButton.Active = config.CsharpCompiler == CsharpCompiler.Csc;
			mcsRadioButton.Active = config.CsharpCompiler == CsharpCompiler.Mcs;
		}
		
		public override bool StorePanelContents()
		{
			if (msnetRadioButton.Active) {
				config.NetRuntime =  NetRuntime.MsNet;
			} else if (monoRadioButton.Active) {
				config.NetRuntime =  NetRuntime.Mono;
			} else {
				config.NetRuntime =  NetRuntime.MonoInterpreter;
			}
			config.CsharpCompiler = cscRadioButton.Active ? CsharpCompiler.Csc : CsharpCompiler.Mcs;
			
			return true;
		}
	}
}
