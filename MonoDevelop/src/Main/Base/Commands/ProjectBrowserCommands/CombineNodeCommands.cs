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
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads.ProjectBrowser;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddNewProjectToCombine : AbstractMenuCommand
	{
		NewProjectDialog npdlg;
		ProjectBrowserView browser;
		CombineBrowserNode node;
		MessageService msg;
		IProjectService projectService;

		public override void Run()
		{
			projectService = (IProjectService)ServiceManager.GetService(typeof(IProjectService));
			browser     = (ProjectBrowserView)Owner;
			node        = browser.SelectedNode as CombineBrowserNode;
			msg             = (MessageService)ServiceManager.GetService (typeof (MessageService));
			
			if (node != null) {
				npdlg = new NewProjectDialog(false);
				npdlg.OnOked += new EventHandler (Oked);
			}
		}

		void Oked (object o, EventArgs e)
		{
			try 
			{
				int newNodeIndex = node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)node.Combine.AddEntry(npdlg.NewProjectLocation)));
				projectService.SaveCombine();
			// expand to the new node
				node.Nodes[newNodeIndex].Expand();
			}
			catch
			{
				msg.ShowError (GettextCatalog.GetString ("Invalid Project File"));
			}
		}			
	}
		
	public class AddNewCombineToCombine : AbstractMenuCommand
	{
		IProjectService projectService;
		ProjectBrowserView browser;
		CombineBrowserNode node;
		MessageService msg;
		NewProjectDialog npdlg;

		public override void Run()
		{
			projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			browser = (ProjectBrowserView)Owner;
			node    = browser.SelectedNode as CombineBrowserNode;
			msg         = (MessageService)ServiceManager.GetService (typeof (MessageService));
			
			if (node != null) {
				npdlg = new NewProjectDialog(false);
				npdlg.OnOked += new EventHandler (Oked);
			}
		}

		void Oked (object o, EventArgs e)
		{
			try 
			{
				int newNodeIndex = node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)node.Combine.AddEntry(npdlg.NewCombineLocation)));
				projectService.SaveCombine();
				
				// expand to the new node
				node.Nodes[newNodeIndex].Expand();
			}
			catch
			{
				msg.ShowError (GettextCatalog.GetString ("Invalid Solution File"));
			}
		}
	}
	
	public class AddProjectToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			PropertyService propertyService = (PropertyService)ServiceManager.GetService (typeof (PropertyService));
			
			if (node != null) {
				using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Add a Project"))) {
					StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
					fdiag.SelectMultiple = false;
					if (fdiag.Run () == (int) Gtk.ResponseType.Ok) {
						try {
							object obj = node.Combine.AddEntry(fdiag.Filename);
							int newNodeIndex = -1;
							if (obj is IProject) {
								newNodeIndex = node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
							} else {
								newNodeIndex = node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
							}
							projectService.SaveCombine();
						
							if (newNodeIndex > -1) {
								// expand to the new node
								node.Nodes[newNodeIndex].Expand();
							}
						}
						catch 
						{
							((MessageService)ServiceManager.GetService (typeof (MessageService))).ShowError (GettextCatalog.GetString ("Invalid Project File"));
						}
					}

					fdiag.Hide ();
				}
			}
		}
	}
		
	public class AddCombineToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
			PropertyService propertyService = (PropertyService)ServiceManager.GetService (typeof (PropertyService));
			
			if (node != null) {
				using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Add a Combine"))) {
					StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				
					fdiag.SelectMultiple = false;
					if (fdiag.Run () == (int) Gtk.ResponseType.Ok) {
						try {
							object obj = node.Combine.AddEntry(fdiag.Filename);
							int newNodeIndex = -1;
							if (obj is IProject) {
								newNodeIndex = node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
							} else {
								newNodeIndex = node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
							}
							projectService.SaveCombine();
							
							if (newNodeIndex > -1) {
								// expand to the new node
								node.Nodes[newNodeIndex].Expand();
							}
						}
						catch 
						{
							((MessageService)ServiceManager.GetService (typeof (MessageService))).ShowError (GettextCatalog.GetString ("Invalid Solution File"));
						}
					}

					fdiag.Hide ();
				}
			}
		}
	}
	
	public class CombineOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
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
