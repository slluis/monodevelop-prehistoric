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
		public static object CompileLockObject = new Compile();
		
		public static void ShowAfterCompileStatus()
		{
			if (!Runtime.TaskService.SomethingWentWrong) {
				Runtime.Gui.StatusBar.SetMessage (GettextCatalog.GetString ("Successful"));
			} else {
				Runtime.Gui.StatusBar.SetMessage (String.Format (GettextCatalog.GetString ("{0} errors, {1} warnings"), Runtime.TaskService.Errors.ToString (), Runtime.TaskService.Warnings.ToString ()));
			}
		}
		
		void CompileThread()
		{
			//lock (Compile.CompileLockObject) {
				CombineEntry.BuildProjects = 0;
				CombineEntry.BuildErrors   = 0;
				
				TaskService taskService = Runtime.TaskService;
				IProjectService projectService = Runtime.ProjectService;
				try {
					if (projectService.CurrentOpenCombine != null) {
						projectService.CompileCombine();
						ShowAfterCompileStatus();
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							ILanguageBinding binding = Runtime.Languages.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
							
							if (binding != null) {
								if (binding == null || !binding.CanCompile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName)) {
									Runtime.MessageService.ShowError(String.Format (GettextCatalog.GetString ("Language binding {0} can't compile {1}"), binding.Language, WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName));
								} else {
									new SaveFile().Run();
									ICompilerResult res = binding.CompileFile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
									taskService.ClearTasks ();
									foreach (CompilerError err in res.CompilerResults.Errors) {
										taskService.AddTask(new Task(null, err));
									}
									taskService.CompilerOutput = res.CompilerOutput;
									taskService.NotifyTaskChange();
									ShowAfterCompileStatus();
								}
							} else {
								Runtime.MessageService.ShowError(GettextCatalog.GetString ("No source file for compilation found. Please save unsaved files"));
							}
						}
					}
					taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString (), CombineEntry.BuildErrors.ToString ());
				} catch (Exception e) {
					Console.WriteLine (e);
					Runtime.MessageService.ShowError(e, GettextCatalog.GetString ("Error while compiling"));
				}
				projectService.OnEndBuild(CombineEntry.BuildErrors == 0);
			//}
		}
		
		public void RunWithWait()
		{
			CompileThread();
		}
		
		public override void Run()
		{
			lock (CompileLockObject) {
				if (Runtime.ProjectService.CurrentOpenCombine != null) {
					Runtime.TaskService.CompilerOutput = String.Empty;
					Runtime.ProjectService.OnStartBuild();
					RunWithWait();
					//Thread t = new Thread(new ThreadStart(CompileThread));
					//t.IsBackground  = true;
					//t.Start();
				}
				
			}
		}
	}
	public class CompileAll : AbstractMenuCommand
	{
		void CompileThread()
		{ 
			lock (Compile.CompileLockObject) {
				CombineEntry.BuildProjects = 0;
				CombineEntry.BuildErrors   = 0;
				TaskService taskService = Runtime.TaskService;
				IProjectService projectService = Runtime.ProjectService;
				try {
					
					if (projectService.CurrentOpenCombine != null) {
						projectService.RecompileAll();
						Compile.ShowAfterCompileStatus();
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							ILanguageBinding binding = Runtime.Languages.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
							
							if (binding != null) {
								if (binding == null || !binding.CanCompile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName)) {
									Runtime.MessageService.ShowError(String.Format (GettextCatalog.GetString ("Language binding {0} can't compile {1}"), binding.Language, WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName));
								} else {
									new SaveFile().Run();
									ICompilerResult res = binding.CompileFile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
									taskService.ClearTasks();
									foreach (CompilerError err in res.CompilerResults.Errors) {
										taskService.AddTask(new Task(null, err));
									}
									taskService.CompilerOutput = res.CompilerOutput;
									taskService.NotifyTaskChange();
									Compile.ShowAfterCompileStatus();
								}
							} else {
								Runtime.MessageService.ShowError(GettextCatalog.GetString ("No source file for compilation found. Please save unsaved files"));
							}
						}
					}
					taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString(), CombineEntry.BuildErrors.ToString());
				} catch (Exception e) {
					Console.WriteLine (e);
					Runtime.MessageService.ShowError (e, GettextCatalog.GetString ("Error while compiling"));
				}
				projectService.OnEndBuild(CombineEntry.BuildErrors == 0);
			}
		}
		
		public override void Run()
		{
//			if (Monitor.TryEnter(Compile.CompileLockObject)) {
				if (Runtime.ProjectService.CurrentOpenCombine != null) {
	
					Runtime.TaskService.CompilerOutput = String.Empty;
					Runtime.ProjectService.OnStartBuild();
					CompileThread ();
					//Thread t = new Thread(new ThreadStart(CompileThread));
					//t.IsBackground  = true;
					//t.Start();
				}
//				Monitor.Exit(Compile.CompileLockObject);
//			}
		}
	}
	
	public class RunCommand : AbstractMenuCommand
	{
		//void RunThread()
		bool RunThread()
		{
			lock (Compile.CompileLockObject) {
				IProjectService projectService = Runtime.ProjectService;
				try {
					Runtime.Gui.StatusBar.SetMessage(GettextCatalog.GetString ("Executing"));
					if (projectService.CurrentOpenCombine != null) {
						try {
							if (projectService.NeedsCompiling) {
								projectService.CompileCombine();
							}
							if (Runtime.TaskService.Errors == 0) {
								projectService.OnBeforeStartProject();
								projectService.CurrentOpenCombine.Execute();
							}
							
						} catch (NoStartupCombineDefinedException) {
							Runtime.MessageService.ShowError(GettextCatalog.GetString ("Cannot execute Run command, cannot find startup project.\nPlease define a startup project for the combine in the combine properties."));
						}
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							new Compile().RunWithWait();
							if (Runtime.TaskService.Errors == 0) {
								ILanguageBinding binding = Runtime.Languages.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
								if (binding != null) {
									projectService.OnBeforeStartProject();
									binding.Execute(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
								} else {
									Runtime.MessageService.ShowError(GettextCatalog.GetString ("No runnable executable found."));
								}
							}
						}
					}
				} catch (Exception e) {
					Runtime.MessageService.ShowError(e, GettextCatalog.GetString ("Error while running"));
				}
				Runtime.Gui.StatusBar.SetMessage(GettextCatalog.GetString ("Ready"));
				return false;
			}
		}
		
		public override void Run()
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				RunThread(); // TODO FIXME PEDRO
				
				//Thread t = new Thread(new ThreadStart(RunThread));
				//t.IsBackground  = true;
				//t.Start();
			}
		}
	}
	
	public class BuildCurrentProject : AbstractMenuCommand
	{
		public override void Run()
		{
			lock (Compile.CompileLockObject) {
				TaskService taskService = Runtime.TaskService;
				IProjectService projectService = Runtime.ProjectService;
				
				if (projectService.CurrentSelectedProject != null) {
					try {
						CombineEntry.BuildProjects = 0;
						CombineEntry.BuildErrors   = 0;
						taskService.CompilerOutput = String.Empty;
						taskService.ClearTasks();
			
						projectService.OnStartBuild();
						projectService.CompileProject(projectService.CurrentSelectedProject);
						taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString(), CombineEntry.BuildErrors.ToString());
					} catch (Exception e) {
						Runtime.MessageService.ShowError(e, String.Format (GettextCatalog.GetString ("Error while compiling project {0}"), projectService.CurrentSelectedProject.Name));
					}
					projectService.OnEndBuild(CombineEntry.BuildErrors == 0);
				}
				Compile.ShowAfterCompileStatus();
			}
		}
	}
	
	public class RebuildCurrentProject : AbstractMenuCommand
	{
		public override void Run()
		{
			lock (Compile.CompileLockObject) {
				TaskService taskService = Runtime.TaskService;
				IProjectService projectService = Runtime.ProjectService;
				
				if (projectService.CurrentSelectedProject != null) {
					try {
						CombineEntry.BuildProjects = 0;
						CombineEntry.BuildErrors   = 0;
						taskService.CompilerOutput = String.Empty;
						taskService.ClearTasks();
				
						projectService.OnStartBuild();
						projectService.RecompileProject(projectService.CurrentSelectedProject);
						taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString(), CombineEntry.BuildErrors.ToString());
					} catch (Exception e) {
						Runtime.MessageService.ShowError(e, String.Format (GettextCatalog.GetString ("Error while compiling project {0}"), projectService.CurrentSelectedProject.Name));
					}
					projectService.OnEndBuild(CombineEntry.BuildErrors == 0);
				}					
				Compile.ShowAfterCompileStatus();
			}
		}
	}

	public class GenerateMakefiles : AbstractMenuCommand {
		
		public override void Run () 
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				Runtime.ProjectService.GenerateMakefiles ();
			}
		}
	}
}
