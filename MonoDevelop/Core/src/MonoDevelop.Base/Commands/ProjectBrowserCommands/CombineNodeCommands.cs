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
using MonoDevelop.Gui.Pads;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddNewProjectToCombine : AbstractMenuCommand
	{
		NewProjectDialog npdlg;
		Combine combine;
		NodePosition position;
		SolutionPad browser;

		public override void Run()
		{
			browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			combine = nav.GetParentDataItem (typeof(Combine), true) as Combine;
			if (combine != null) {
				position = nav.CurrentPosition;
				npdlg = new NewProjectDialog(false);
				npdlg.OnOked += new EventHandler (Oked);
			}
		}

		void Oked (object o, EventArgs e)
		{
			ITreeNavigator nav = browser.GetNodeAtPosition (position);
			nav.Expanded = true;
			
			IProgressMonitor monitor = Runtime.TaskService.GetLoadProgressMonitor ();
			try 
			{
				object ob = combine.AddEntry (npdlg.NewProjectLocation, monitor);
				Runtime.ProjectService.SaveCombine ();
				browser.AddNodeInsertCallback (ob, new TreeNodeCallback (OnEntryInserted));
			}
			catch {
				Runtime.MessageService.ShowError (GettextCatalog.GetString ("Invalid Project File"));
			}
			finally {
				monitor.Dispose ();
				npdlg = null;
			}
		}
		
		void OnEntryInserted (ITreeNavigator nav)
		{
			nav.Selected = true;
			nav.Expanded = true;
		}
	}
		
	public class AddNewCombineToCombine : AbstractMenuCommand
	{
		Combine combine;
		NewProjectDialog npdlg;
		SolutionPad browser;
		NodePosition position;

		public override void Run()
		{
			browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			combine = nav.GetParentDataItem (typeof(Combine), true) as Combine;
			if (combine != null) {
				position = nav.CurrentPosition;
				npdlg = new NewProjectDialog(false);
				npdlg.OnOked += new EventHandler (Oked);
			}
		}

		void Oked (object o, EventArgs e)
		{
			ITreeNavigator nav = browser.GetNodeAtPosition (position);
			nav.Expanded = true;
			
			IProgressMonitor monitor = Runtime.TaskService.GetLoadProgressMonitor ();
			try 
			{
				object ob = combine.AddEntry (npdlg.NewCombineLocation, monitor);
				Runtime.ProjectService.SaveCombine ();
				browser.AddNodeInsertCallback (ob, new TreeNodeCallback (OnEntryInserted));
			}
			catch {
				Runtime.MessageService.ShowError (GettextCatalog.GetString ("Invalid Solution File"));
			}
			finally {
				monitor.Dispose ();
				npdlg = null;
			}
		}
		
		void OnEntryInserted (ITreeNavigator nav)
		{
			nav.Selected = true;
			nav.Expanded = true;
		}
	}
	
	public class AddProjectToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			Combine combine = nav.GetParentDataItem (typeof(Combine), true) as Combine;
			if (combine == null) return;
			
			using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Add a Project"))) {
				fdiag.SelectMultiple = false;
				if (fdiag.Run () == (int) Gtk.ResponseType.Ok) {
					try
					{
						nav.Expanded = true;
						using (IProgressMonitor monitor = Runtime.TaskService.GetLoadProgressMonitor ()) {
							object ob = combine.AddEntry (fdiag.Filename, monitor);
							Runtime.ProjectService.SaveCombine ();
							browser.AddNodeInsertCallback (ob, new TreeNodeCallback (OnEntryInserted));
						}
					}
					catch 
					{
						Runtime.MessageService.ShowError (GettextCatalog.GetString ("Invalid Project File"));
					}
				}

				fdiag.Hide ();
			}
		}
		
		void OnEntryInserted (ITreeNavigator nav)
		{
			nav.Selected = true;
			nav.Expanded = true;
		}
	}
		
	public class AddCombineToCombine : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			Combine combine = nav.GetParentDataItem (typeof(Combine), true) as Combine;
			if (combine == null) return;
			
			using (FileSelector fdiag = new FileSelector (GettextCatalog.GetString ("Add a Combine"))) {
				fdiag.SelectMultiple = false;
				if (fdiag.Run () == (int) Gtk.ResponseType.Ok)
				{
					try
					{
						nav.Expanded = true;
						using (IProgressMonitor monitor = Runtime.TaskService.GetLoadProgressMonitor ()) {
							object ob = combine.AddEntry (fdiag.Filename, monitor);
							Runtime.ProjectService.SaveCombine ();
							browser.AddNodeInsertCallback (ob, new TreeNodeCallback (OnEntryInserted));
						}
					}
					catch 
					{
						Runtime.MessageService.ShowError (GettextCatalog.GetString ("Invalid Solution File"));
					}
				}

				fdiag.Hide ();
			}
		}
		
		void OnEntryInserted (ITreeNavigator nav)
		{
			nav.Selected = true;
			nav.Expanded = true;
		}
	}
	
	public class CombineOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			Combine combine = nav.GetParentDataItem (typeof(Combine), true) as Combine;
			if (combine == null) return;
			
			DefaultProperties defaultProperties = new DefaultProperties();
			defaultProperties.SetProperty ("Combine", combine);
			TreeViewOptions optionsDialog = new TreeViewOptions (defaultProperties,
																	   AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/CombineOptions"));
		//		optionsDialog.SetDefaultSize = new Size(700, 450);
		//		optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
		//				
		//		optionsDialog.TransientFor = (Gtk.Window)WorkbenchSingleton.Workbench;
				optionsDialog.Run ();
		//		optionsDialog.Hide ();
				Runtime.ProjectService.SaveCombine ();
		}
	}
}
