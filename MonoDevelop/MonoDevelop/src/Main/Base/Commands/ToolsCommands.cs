// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Gui;
using MonoDevelop.Gui.Dialogs;

namespace MonoDevelop.Commands
{
	public class OptionsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.GetService(typeof(PropertyService));
			TreeViewOptions optionsDialog = new TreeViewOptions((IProperties)propertyService.GetProperty("MonoDevelop.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()),
			                                                           AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Dialogs/OptionsDialog"));
			//optionsDialog.ShowAll ();
			//	optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
				
			//	optionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
			//	optionsDialog.ShowDialog();
			//}
		}
	}
	
	public class ToggleFullscreenCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen = !((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen;
		}
	}
	
	public class NewLayoutCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (NewLayoutDialog dlg = new NewLayoutDialog ()) {
				dlg.Run ();
			}
		}
	}
}
