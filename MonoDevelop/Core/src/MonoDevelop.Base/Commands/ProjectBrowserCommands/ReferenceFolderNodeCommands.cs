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
using MonoDevelop.Gui.Pads;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class AddReferenceToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator tnav = browser.GetSelectedNode ();
			Project project = tnav.GetParentDataItem (typeof(Project), true) as Project;
			
			if (project != null) {
				SelectReferenceDialog selDialog = new SelectReferenceDialog(project);
				if (selDialog.Run() == (int)Gtk.ResponseType.Ok) {
					ProjectReferenceCollection newRefs = selDialog.ReferenceInformations;
					
					ArrayList toDelete = new ArrayList ();
					foreach (ProjectReference refInfo in project.ProjectReferences)
						if (!newRefs.Contains (refInfo))
							toDelete.Add (refInfo);
					
					foreach (ProjectReference refInfo in toDelete)
							project.ProjectReferences.Remove (refInfo);

					foreach (ProjectReference refInfo in selDialog.ReferenceInformations)
						if (!project.ProjectReferences.Contains (refInfo))
							project.ProjectReferences.Add(refInfo);
					
					Runtime.ProjectService.SaveCombine();
					tnav.Expanded = true;
				}
				selDialog.Hide ();
			}
		}
	}	
}
