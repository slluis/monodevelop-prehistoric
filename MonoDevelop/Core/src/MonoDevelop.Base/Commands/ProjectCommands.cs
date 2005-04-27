// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Commands
{
	public enum ProjectCommands
	{
		Compile,
		AddNewProject,
		AddNewCombine,
		AddProject,
		AddCombine,
		RemoveFromProject,
		Options,
		AddResource,
		AddReference,
		AddNewFiles,
		AddFiles,
		NewFolder,
		IncludeToProject,
		Build,
		Rebuild,
		SetAsStartupProject,
		GenerateMakefiles,
		Run,
		IncludeInBuild,
		IncludeInDeploy,
		Deploy,
		ConfigurationSelector,
		Debug,
		DebugApplication,
		Stop
	}
	
	public class CompileHandler: CommandHandler
	{
		protected override void Run ()
		{
			IProjectService projectService = Runtime.ProjectService;
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				Runtime.FileService.SaveFile (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
				projectService.BuildFile (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
			}
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null;
		}
	}
	
	public class RunHandler: CommandHandler
	{
		CombineEntry entry;
		string file;
		
		protected override void Run ()
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				entry = Runtime.ProjectService.CurrentSelectedCombineEntry;
				if (entry != null) {
					IAsyncOperation op = Runtime.ProjectService.Build (entry);
					op.Completed += new OperationHandler (ExecuteCombine);
				}
			} else {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					file = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
					IAsyncOperation op = Runtime.ProjectService.ExecuteFile (file);
					op.Completed += new OperationHandler (ExecuteFile);
				}
			}
		}
		
		protected override void Update (CommandInfo info)
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				info.Enabled = Runtime.ProjectService.CurrentSelectedCombineEntry != null && 
								Runtime.ProjectService.CurrentRunOperation.IsCompleted;
			} else {
				info.Enabled = (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null);
			}
		}
		
		void ExecuteCombine (IAsyncOperation op)
		{
			if (op.Success)
				Runtime.ProjectService.Execute (entry);
		}
		
		void ExecuteFile (IAsyncOperation op)
		{
			if (op.Success)
				Runtime.ProjectService.ExecuteFile (file);
		}
	}
	
	public class DebugHandler: CommandHandler
	{
		CombineEntry entry;
		string file;
		
		protected override void Run ()
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				entry = Runtime.ProjectService.CurrentSelectedCombineEntry;
				if (entry != null) {
					IAsyncOperation op = Runtime.ProjectService.Build (entry);
					op.Completed += new OperationHandler (ExecuteCombine);
				}
			} else {
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					file = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
					IAsyncOperation op = Runtime.ProjectService.ExecuteFile (file);
					op.Completed += new OperationHandler (ExecuteFile);
				}
			}
		}
		
		protected override void Update (CommandInfo info)
		{
			if (Runtime.DebuggingService == null) {
				info.Enabled = false;
				return;
			}
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				info.Enabled = Runtime.ProjectService.CurrentSelectedCombineEntry != null && 
								Runtime.ProjectService.CurrentRunOperation.IsCompleted;
			} else {
				info.Enabled = (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null);
			}
		}
		
		void ExecuteCombine (IAsyncOperation op)
		{
			if (op.Success)
				Runtime.ProjectService.Debug (entry);
		}
		
		void ExecuteFile (IAsyncOperation op)
		{
			if (op.Success)
				Runtime.ProjectService.DebugFile (file);
		}
	}
	
	public class DebugApplicationHandler: CommandHandler
	{
		protected override void Run ()
		{
			using (FileSelector fs = new FileSelector (GettextCatalog.GetString ("Application to Debug"))) {
				int response = fs.Run ();
				string name = fs.Filename;
				fs.Hide ();
				if (response == (int)Gtk.ResponseType.Ok)
					Runtime.ProjectService.DebugApplication (name);
			}
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = Runtime.DebuggingService != null &&
							Runtime.ProjectService.CurrentRunOperation.IsCompleted;
		}
	}

	public class BuildHandler: CommandHandler
	{
		protected override void Run ()
		{
			if (Runtime.ProjectService.CurrentSelectedCombineEntry != null)
				Runtime.ProjectService.Build (Runtime.ProjectService.CurrentSelectedCombineEntry);
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = Runtime.ProjectService.CurrentBuildOperation.IsCompleted &&
							(Runtime.ProjectService.CurrentSelectedCombineEntry != null);
		}
	}
	
	public class RebuildHandler: CommandHandler
	{
		protected override void Run ()
		{
			if (Runtime.ProjectService.CurrentSelectedCombineEntry != null)
				Runtime.ProjectService.Rebuild (Runtime.ProjectService.CurrentSelectedCombineEntry);
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = Runtime.ProjectService.CurrentBuildOperation.IsCompleted &&
							(Runtime.ProjectService.CurrentSelectedCombineEntry != null);
		}
	}
	
	public class StopHandler: CommandHandler
	{
		protected override void Run ()
		{
			if (!Runtime.ProjectService.CurrentBuildOperation.IsCompleted)
				Runtime.ProjectService.CurrentBuildOperation.Cancel ();
			if (!Runtime.ProjectService.CurrentRunOperation.IsCompleted)
				Runtime.ProjectService.CurrentRunOperation.Cancel ();
		}
		
		protected override void Update (CommandInfo info)
		{
			info.Enabled = !Runtime.ProjectService.CurrentBuildOperation.IsCompleted ||
							!Runtime.ProjectService.CurrentRunOperation.IsCompleted;
		}
	}
	
	public class GenerateMakefilesHandler: CommandHandler
	{
		protected override void Run ()
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				Runtime.ProjectService.CurrentOpenCombine.GenerateMakefiles ();
			}
		}
	}

	public class GenerateProjectDocumentation : CommandHandler
	{
		protected override void Run ()
		{
			try {
				if (Runtime.ProjectService.CurrentSelectedProject != null) {
					string assembly    = Runtime.ProjectService.CurrentSelectedProject.GetOutputFileName ();
					string projectFile = Path.ChangeExtension(assembly, ".ndoc");
					if (!File.Exists(projectFile)) {
						StreamWriter sw = File.CreateText(projectFile);
						sw.WriteLine("<project>");
						sw.WriteLine("    <assemblies>");
						sw.WriteLine("        <assembly location=\""+ assembly +"\" documentation=\"" + Path.ChangeExtension(assembly, ".xml") + "\" />");
						sw.WriteLine("    </assemblies>");
						/*
						sw.WriteLine("    				    <documenters>");
						sw.WriteLine("    				        <documenter name=\"JavaDoc\">");
						sw.WriteLine("    				            <property name=\"Title\" value=\"NDoc\" />");
						sw.WriteLine("    				            <property name=\"OutputDirectory\" value=\".\\docs\\JavaDoc\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingReturns\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
						sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
						sw.WriteLine("    				            <property name=\"CopyrightHref\" value=\"\" />");
						sw.WriteLine("    				        </documenter>");
						sw.WriteLine("    				        <documenter name=\"MSDN\">");
						sw.WriteLine("    				            <property name=\"OutputDirectory\" value=\".\\docs\\MSDN\" />");
						sw.WriteLine("    				            <property name=\"HtmlHelpName\" value=\"NDoc\" />");
						sw.WriteLine("    				            <property name=\"HtmlHelpCompilerFilename\" value=\"C:\\Program Files\\HTML Help Workshop\\hhc.exe\" />");
						sw.WriteLine("    				            <property name=\"IncludeFavorites\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"Title\" value=\"An NDoc Documented Class Library\" />");
						sw.WriteLine("    				            <property name=\"SplitTOCs\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DefaulTOC\" value=\"\" />");
						sw.WriteLine("    				            <property name=\"ShowVisualBasic\" value=\"True\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
						sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
						sw.WriteLine("                <property name=\"CopyrightHref\" value=\"\" />");
						sw.WriteLine("            </documenter>");
						sw.WriteLine("    				        <documenter name=\"XML\">");
						sw.WriteLine("    				            <property name=\"OutputFile\" value=\".\\docs\\doc.xml\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingReturns\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
						sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
						sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
						sw.WriteLine("    				            <property name=\"CopyrightHref\" value=\"\" />");
						sw.WriteLine("    				        </documenter>");
						sw.WriteLine("    				    </documenters>");*/
						sw.WriteLine("    				</project>");
						sw.Close();
					}
					string command = Runtime.FileUtilityService.SharpDevelopRootPath +
					Path.DirectorySeparatorChar + "bin" +
					Path.DirectorySeparatorChar + "ndoc" +
					Path.DirectorySeparatorChar + "NDocGui.exe";
					string args    = '"' + projectFile + '"';
					
					ProcessStartInfo psi = new ProcessStartInfo(command, args);
					psi.WorkingDirectory = Runtime.FileUtilityService.SharpDevelopRootPath +
					Path.DirectorySeparatorChar + "bin" +
					Path.DirectorySeparatorChar + "ndoc";
					psi.UseShellExecute = false;
					Process p = new Process();
					p.StartInfo = psi;
					p.Start();
				}
			} catch (Exception) {
				//MessageBox.Show("You need to compile the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
	}
}
