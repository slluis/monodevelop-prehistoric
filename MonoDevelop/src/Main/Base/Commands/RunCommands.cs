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

using MonoDevelop.Core.Services;
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
			TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
			IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
			if (!taskService.SomethingWentWrong) {
				statusBarService.SetMessage(GettextCatalog.GetString ("Successful"));
			} else {
				StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				statusBarService.SetMessage(String.Format (GettextCatalog.GetString ("{0} errors, {1} warnings"), taskService.Errors.ToString (), taskService.Warnings.ToString ()));
			}
		}
		
		void CompileThread()
		{
			//lock (Compile.CompileLockObject) {
				CombineEntry.BuildProjects = 0;
				CombineEntry.BuildErrors   = 0;
				
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				try {
					if (projectService.CurrentOpenCombine != null) {
						projectService.CompileCombine();
						ShowAfterCompileStatus();
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
							ILanguageBinding binding = languageBindingService.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
							
							if (binding != null) {
								if (binding == null || !binding.CanCompile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName)) {
									IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
									messageService.ShowError(String.Format (GettextCatalog.GetString ("Language binding {0} can't compile {1}"), binding.Language, WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName));
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
								IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
								messageService.ShowError(GettextCatalog.GetString ("No source file for compilation found. Please save unsaved files"));
							}
						}
					}
					taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString (), CombineEntry.BuildErrors.ToString ());
				} catch (Exception e) {
					Console.WriteLine (e);
					IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
					messageService.ShowError(e, GettextCatalog.GetString ("Error while compiling"));
				}
				projectService.OnEndBuild();
			//}
		}
		
		public void RunWithWait()
		{
			CompileThread();
		}
		
		public override void Run()
		{
			lock (CompileLockObject) {
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				
				if (projectService.CurrentOpenCombine != null) {
					taskService.CompilerOutput = String.Empty;
					projectService.OnStartBuild();
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
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
				try {
					
					if (projectService.CurrentOpenCombine != null) {
						projectService.RecompileAll();
						Compile.ShowAfterCompileStatus();
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
							ILanguageBinding binding = languageBindingService.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
							
							if (binding != null) {
								if (binding == null || !binding.CanCompile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName)) {
									IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
									messageService.ShowError(String.Format (GettextCatalog.GetString ("Language binding {0} can't compile {1}"), binding.Language, WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName));
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
								IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
								messageService.ShowError(GettextCatalog.GetString ("No source file for compilation found. Please save unsaved files"));
							}
						}
					}
					taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString(), CombineEntry.BuildErrors.ToString());
				} catch (Exception e) {
					Console.WriteLine (e);
					IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
					messageService.ShowError(e, GettextCatalog.GetString ("Error while compiling"));
				}
				projectService.OnEndBuild();
			}
		}
		
		public override void Run()
		{
//			if (Monitor.TryEnter(Compile.CompileLockObject)) {
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				if (projectService.CurrentOpenCombine != null) {
	
					taskService.CompilerOutput = String.Empty;
					projectService.OnStartBuild();
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
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
				try {
					statusBarService.SetMessage(GettextCatalog.GetString ("Executing"));
					if (projectService.CurrentOpenCombine != null) {
						try {
							if (projectService.NeedsCompiling) {
								projectService.CompileCombine();
							}
							if (taskService.Errors == 0) {
								projectService.OnBeforeStartProject();
								projectService.CurrentOpenCombine.Execute();
							}
							
						} catch (NoStartupCombineDefinedException) {
							IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
							messageService.ShowError(GettextCatalog.GetString ("Cannot execute Run command, cannot find startup project.\nPlease define a startup project for the combine in the combine properties."));
						}
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							new Compile().RunWithWait();
							if (taskService.Errors == 0) {
								LanguageBindingService languageBindingService = (LanguageBindingService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(LanguageBindingService));
								ILanguageBinding binding = languageBindingService.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
								if (binding != null) {
									projectService.OnBeforeStartProject();
									binding.Execute(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName);
								} else {
									IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
									messageService.ShowError(GettextCatalog.GetString ("No runnable executable found."));
								}
							}
						}
					}
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
					messageService.ShowError(e, GettextCatalog.GetString ("Error while running"));
				}
				statusBarService.SetMessage(GettextCatalog.GetString ("Ready"));
				return false;
			}
		}
		
		public override void Run()
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
			if (projectService.CurrentOpenCombine != null) {
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
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
				IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
				
				if (projectService.CurrentSelectedProject != null) {
					try {
						StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
						
						CombineEntry.BuildProjects = 0;
						CombineEntry.BuildErrors   = 0;
						taskService.CompilerOutput = String.Empty;
						taskService.ClearTasks();
			
						projectService.OnStartBuild();
						projectService.CompileProject(projectService.CurrentSelectedProject);
						taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString(), CombineEntry.BuildErrors.ToString());
					} catch (Exception e) {
						IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
						messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Error while compiling project {0}"), projectService.CurrentSelectedProject.Name));
					}
					projectService.OnEndBuild();
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
				TaskService taskService = (TaskService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.GetService(typeof(IResourceService));
				IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.GetService(typeof(IStatusBarService));
				
				if (projectService.CurrentSelectedProject != null) {
					try {
						StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
						
						CombineEntry.BuildProjects = 0;
						CombineEntry.BuildErrors   = 0;
						taskService.CompilerOutput = String.Empty;
						taskService.ClearTasks();
				
						projectService.OnStartBuild();
						projectService.RecompileProject(projectService.CurrentSelectedProject);
						taskService.CompilerOutput += String.Format (GettextCatalog.GetString ("---------------------- Done ----------------------\n\nBuild: {0} succeeded, {1} failed\n"), CombineEntry.BuildProjects.ToString(), CombineEntry.BuildErrors.ToString());
					} catch (Exception e) {
						IMessageService messageService =(IMessageService)ServiceManager.GetService(typeof(IMessageService));
						messageService.ShowError(e, String.Format (GettextCatalog.GetString ("Error while compiling project {0}"), projectService.CurrentSelectedProject.Name));
					}
					projectService.OnEndBuild();
				}					
				Compile.ShowAfterCompileStatus();
			}
		}
	}

	public class GenerateMakefiles : AbstractMenuCommand {
		
		public override void Run () 
		{
			IProjectService projectservice = (IProjectService)ServiceManager.GetService (typeof (IProjectService));
			if (projectservice.CurrentOpenCombine != null) {
				projectservice.GenerateMakefiles ();
			}
		}
	}
}
