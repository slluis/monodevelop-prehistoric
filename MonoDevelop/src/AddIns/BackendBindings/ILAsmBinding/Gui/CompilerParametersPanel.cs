// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using Gtk;

using MonoDevelop.Internal.Project;
using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

namespace MonoDevelop.ILAsmBinding
{
	public class CompilerParametersPanel : AbstractOptionPanel
	{
		ILAsmCompilerParameters compilerParameters = null;
		
		public override void LoadPanelContents()
		{
			this.compilerParameters = (ILAsmCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			//System.Windows.Forms.PropertyGrid grid = new System.Windows.Forms.PropertyGrid();
			//grid.Dock = DockStyle.Fill;
			//grid.SelectedObjects = new object[] { compilerParameters};
			//Controls.Add(grid);
		}
		
		public override bool StorePanelContents()
		{
			return true;
		}
	}
}
