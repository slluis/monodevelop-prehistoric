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
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Pads;

namespace MonoDevelop.Commands.ProjectBrowser
{
	public class SetAsStartupProject : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			ITreeNavigator nav = browser.GetSelectedNode ();
			if (nav == null) return;
			
			Project project = nav.DataItem as Project;
			if (project == null) return;
			
			Combine combine = nav.GetParentDataItem (typeof(Combine), false) as Combine;
			combine.StartupEntry = project;
			combine.SingleStartupProject = true;
			Runtime.ProjectService.SaveCombine ();
		}
	}
}
