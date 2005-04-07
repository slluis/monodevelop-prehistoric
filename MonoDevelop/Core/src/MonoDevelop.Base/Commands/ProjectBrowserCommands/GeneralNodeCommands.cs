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
	public class RemoveEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad pad = (SolutionPad) Owner;
			pad.RemoveCurrentItem ();
		}
	}
	
	public class RenameEntryEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad) Owner;
			browser.StartLabelEdit();
		}
	}
	
	public class OpenFileEvent : AbstractMenuCommand
	{
		public override void Run()
		{
			SolutionPad browser = (SolutionPad)Owner;
			browser.ActivateCurrentItem ();
		}
	}
	
	public class CopyNodeCommand: AbstractMenuCommand
	{
		public override void Run()
		{
			TreeViewPad browser = (TreeViewPad)Owner;
			browser.CopyCurrentItem ();
		}
	}
	
	public class CutNodeCommand: AbstractMenuCommand
	{
		public override void Run()
		{
			TreeViewPad browser = (TreeViewPad)Owner;
			browser.CutCurrentItem ();
		}
	}
	
	public class PasteNodeCommand: AbstractMenuCommand
	{
		public override void Run()
		{
			TreeViewPad browser = (TreeViewPad)Owner;
			browser.PasteToCurrentItem ();
		}
	}
}

