// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;

using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Commands
{
	public class RunTestsInProject : AbstractMenuCommand
	{
		public override void Run()
		{
			if (Runtime.ProjectService.CurrentSelectedProject != null) {
				string assembly = Runtime.ProjectService.CurrentSelectedProject.GetOutputFileName ();
				
				if (!File.Exists(assembly)) {
					Runtime.MessageService.ShowError (GettextCatalog.GetString ("Assembly not Found (Compile the project first)"));
				} else {
					string command = Runtime.FileUtilityService.SharpDevelopRootPath + 
					                 Path.DirectorySeparatorChar + "bin" + 
					                 Path.DirectorySeparatorChar + "nunit" + 
					                 Path.DirectorySeparatorChar + "nunit-gui.exe";
					string args = '"'  + assembly + '"';
					Process.Start(command, args);
				}
			}
		}
	}
	
	public class ViewProjectOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			Project selectedProject = Runtime.ProjectService.CurrentSelectedProject;
			if (selectedProject == null) {
				return;
			}
			
			IAddInTreeNode generalOptionsNode          = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/ProjectOptions/GeneralOptions");
			IAddInTreeNode configurationPropertiesNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/ProjectOptions/ConfigurationProperties");
			
			ProjectOptionsDialog optionsDialog = new ProjectOptionsDialog(selectedProject, generalOptionsNode, configurationPropertiesNode);
			if (optionsDialog.Run() == (int)Gtk.ResponseType.Ok) {
					Runtime.ProjectService.CurrentSelectedProject.NeedsBuilding = true;
			}
			
			Runtime.ProjectService.SaveCombine();
		}
	}
	
	public class DeployProject : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (viewContent.IsDirty) {
					viewContent.Save();
				}
			}
			if (Runtime.ProjectService.CurrentSelectedProject != null) {
				DeployInformation.Deploy (Runtime.ProjectService.CurrentSelectedProject);
			}
		}
	}
	
	public class GenerateProjectDocumentation : AbstractMenuCommand
	{
		public override void Run()
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
