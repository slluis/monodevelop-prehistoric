// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Text;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.TextEditor.Document;
using MonoDevelop.Gui;

namespace MonoDevelop.DefaultEditor.Commands
{
	public class ExportProjectToHtml : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			if (projectService.CurrentSelectedProject != null) {
			/*
				ExportProjectToHtmlDialog ephd = new ExportProjectToHtmlDialog (projectService.CurrentSelectedProject);
				ephd.TransientFor = (Gtk.Window) WorkbenchSingleton.Workbench;
				ephd.Run ();
				ephd.Hide ();
				ephd.Dispose();*/
			}
		}
	}

}
