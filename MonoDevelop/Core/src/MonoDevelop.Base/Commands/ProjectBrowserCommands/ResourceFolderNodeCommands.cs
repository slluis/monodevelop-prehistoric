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
using MonoDevelop.Gui.Widgets;

using Gtk;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddResourceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectBrowserView browser = (ProjectBrowserView)Owner;
			FolderNode         node    = browser.SelectedNode as FolderNode;
			
			if (node != null) {
				IProject project = ((ProjectBrowserNode) node.Parent).Project;
				
				show_dialog:
									
				using (FileSelector fs = new FileSelector (GettextCatalog.GetString ("File to Open"))) {
					fs.SelectMultiple = true;
					fs.SetFilename (project.BaseDirectory);
					int response = fs.Run ();
					string [] files = fs.Filenames;
					fs.Hide ();

					if (response != (int)Gtk.ResponseType.Ok)
						return;
				
					foreach (string file in files) {
						if (!System.IO.File.Exists (file)) {
							Runtime.MessageService.ShowError (String.Format (GettextCatalog.GetString ("Resource file '{0}' does not exist"), file));
							goto show_dialog;
						}
					}
				
					foreach (string fileName in files) {
						ProjectFile fileInformation = Runtime.ProjectService.AddFileToProject(project, fileName, BuildAction.EmbedAsResource);
					
						AbstractBrowserNode newResNode = new FileNode(fileInformation);
						newResNode.Image = Stock.ResourceFileIcon;
						node.Nodes.Add (newResNode);
					}

					node.Expand();
					Runtime.ProjectService.SaveCombine ();
				}
			}
		}
	}
}

