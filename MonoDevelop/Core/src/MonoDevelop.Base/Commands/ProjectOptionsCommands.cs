// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;

using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Internal.Project;

using MonoDevelop.Internal.ExternalTool;

namespace MonoDevelop.Commands
{
	internal class AddProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.AddProjectConfiguration();
			}
		}
	}
	
	internal class RenameProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.RenameProjectConfiguration();
			}
		}
	}
	
	internal class RemoveProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.RemoveProjectConfiguration();
			}
		}
	}
	
	internal class SetActiveProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.SetSelectedConfigurationAsStartup();
			}
		}
	}
}
