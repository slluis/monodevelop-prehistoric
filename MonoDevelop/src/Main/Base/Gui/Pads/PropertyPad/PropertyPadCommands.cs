// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

using MonoDevelop.Core.Properties;
using MonoDevelop.Gui;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.Gui.Pads
{
	public class PropertyPadResetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			//PropertyPad.Grid.ResetSelectedProperty();
		}
	}
	
	public class PropertyPadShowDescriptionCommand : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				//return PropertyPad.Grid.HelpVisible;
				return true;
			}
			set {
				//PropertyPad.Grid.HelpVisible = value;
			}
		}
		
		public override void Run()
		{
			//PropertyPad.Grid.HelpVisible = !PropertyPad.Grid.HelpVisible;
		}
	}
	
}
