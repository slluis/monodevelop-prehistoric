// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

//
//
// FIX ME
// We need to do the compile in the background
//

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using Gtk;
using System.Diagnostics;

using MonoDevelop.Core.AddIns;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using System.CodeDom.Compiler;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Services;

namespace MonoDevelop.Commands
{
	public class Compile : AbstractMenuCommand
	{
		public override void Run()
		{ 
			IProjectService projectService = Runtime.ProjectService;
			if (projectService.CurrentOpenCombine != null) {
				projectService.BuildActiveCombine ();
			} else {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					new SaveFile().Run();
					projectService.BuildFile (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
				}
			}
		}
	}
	
	public class CompileAll : AbstractMenuCommand
	{
		public override void Run()
		{ 
			IProjectService projectService = Runtime.ProjectService;
			if (projectService.CurrentOpenCombine != null) {
				projectService.RebuildActiveCombine ();
			} else {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					new SaveFile().Run();
					projectService.BuildFile (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
				}
			}
		}
	}
	
	public class RunCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				IAsyncOperation op = Runtime.ProjectService.BuildActiveCombine ();
				op.Completed += new OperationHandler (ExecuteCombine);
			} else {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					IAsyncOperation op = Runtime.ProjectService.ExecuteFile (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
					op.Completed += new OperationHandler (ExecuteFile);
				}
			}
		}
		
		void ExecuteCombine (IAsyncOperation op)
		{
			if (op.Success)
				Runtime.ProjectService.ExecuteActiveCombine ();
		}
		
		void ExecuteFile (IAsyncOperation op)
		{
			if (op.Success)
				Runtime.ProjectService.ExecuteActiveCombine ();
		}
	}
	
	public class BuildCurrentProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Runtime.ProjectService.BuildActiveProject ();
		}
	}
	
	public class RebuildCurrentProject : AbstractMenuCommand
	{
		public override void Run()
		{
			Runtime.ProjectService.RebuildActiveProject ();
		}
	}

	public class GenerateMakefiles : AbstractMenuCommand {
		
		public override void Run () 
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				Runtime.ProjectService.CurrentOpenCombine.GenerateMakefiles ();
			}
		}
	}
}
