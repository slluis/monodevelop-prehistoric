// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using MonoDevelop.Core.AddIns;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;

using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads.ProjectBrowser;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddNewProjectToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				NewProjectDialog npdlg = new NewProjectDialog(false);
				if (npdlg.Run() == (int)Gtk.ResponseType.Ok) {
					node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)node.Combine.AddEntry(npdlg.NewProjectLocation)));
					projectService.SaveCombine();
				}
			}
		}
	}
		
	public class AddNewCombineToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				NewProjectDialog npdlg = new NewProjectDialog(false);
				if (npdlg.Run() == (int)Gtk.ResponseType.Ok) {
					node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)node.Combine.AddEntry(npdlg.NewCombineLocation)));
					projectService.SaveCombine();
				}
			}
		}
	}
	
	public class AddProjectToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				Gtk.FileSelection fdiag = new Gtk.FileSelection ("Add a Project");
					//fdiag.AddExtension    = true;
					StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
					//fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.ProjectFiles}|*.prjx|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.SelectMultiple = false;
					//fdiag.CheckFileExists = true;
					if (fdiag.Run () == (int) Gtk.ResponseType.Ok) {
						object obj = node.Combine.AddEntry(fdiag.Filename);
						if (obj is IProject) {
							node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
						} else {
							node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
						}
						projectService.SaveCombine();
					}

					fdiag.Hide ();
			}
		}
	}
		
	public class AddCombineToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				Gtk.FileSelection fdiag = new Gtk.FileSelection ("Add a Combine");
					//fdiag.AddExtension    = true;
					StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
					//fdiag.Filter = stringParserService.Parse("${res:SharpDevelop.FileFilter.CombineFiles}|*.cmbx|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.SelectMultiple = false;
					//fdiag.CheckFileExists = true;
					if (fdiag.Run () == (int) Gtk.ResponseType.Ok) {
						object obj = node.Combine.AddEntry(fdiag.Filename);
						if (obj is IProject) {
							node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
						} else {
							node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
						}
						projectService.SaveCombine();
					}
					fdiag.Hide ();
			}
		}
	}
	
	public class CombineOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			
			if (node != null) {
				DefaultProperties defaultProperties = new DefaultProperties();
				defaultProperties.SetProperty("Combine", node.Combine);
				TreeViewOptions optionsDialog = new TreeViewOptions(defaultProperties,
				                                                           AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/CombineOptions"));
			//		optionsDialog.SetDefaultSize = new Size(700, 450);
			//		optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
			//				
			//		optionsDialog.TransientFor = (Gtk.Window)WorkbenchSingleton.Workbench;
					optionsDialog.Run ();
			//		optionsDialog.Hide ();
					projectService.SaveCombine();
				}
			}
	}
}
