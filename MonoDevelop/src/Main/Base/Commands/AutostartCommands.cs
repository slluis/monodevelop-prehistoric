// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ?Â¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Threading;
using System.Runtime.Remoting;
using System.Security.Policy;

using MonoDevelop.Core.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard;
using MonoDevelop.Gui.ErrorHandlers;

using SA = MonoDevelop.SharpAssembly.Assembly;

using MonoDevelop.Internal.Parser;

namespace MonoDevelop.Commands
{
	public class InitializeWorkbenchCommand : AbstractCommand
	{

		public override void Run()
		{
			DefaultWorkbench w = new DefaultWorkbench();
			WorkbenchSingleton.Workbench = w;
			w.InitializeWorkspace();
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			w.UpdateViews(null, null);
			WorkbenchSingleton.CreateWorkspace();
			((Gtk.Window)w).Visible = false;
		}
	}
	
	public class StartCodeCompletionWizard : AbstractCommand
	{

		public static bool generatingCompletionData = false;

		public override void Run()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string path = propertyService.GetProperty ("SharpDevelop.CodeCompletion.DataDirectory", null);
			string codeCompletionProxyFile = Path.Combine (path, "CodeCompletionProxyDataV02.bin");
			//Console.WriteLine("checking for existence of {0}", codeCompletionProxyFile);

			if (!File.Exists (codeCompletionProxyFile)) {
				generatingCompletionData = true;
				RunWizard();
				DefaultParserService parserService = (DefaultParserService)ServiceManager.Services.GetService(typeof(IParserService));
				parserService.LoadProxyDataFile();
			}
		}
		
		void RunWizard()
		{
			if (SplashScreenForm.SplashScreen.Visible) {
				SplashScreenForm.SplashScreen.Hide();
			}
			
			(new GenerateDatabase()).Start();
		}
	}
	
	public class StartParserServiceThread : AbstractCommand
	{
		public override void Run()
		{
			DefaultParserService parserService = (DefaultParserService)ServiceManager.Services.GetService(typeof(DefaultParserService));
			parserService.StartParserThread();
		}
	}
	
	public class StartSharpAssemblyPreloadThread : AbstractCommand
	{
		public override void Run()
		{
			Thread preloadThread = new Thread(new ThreadStart(PreloadThreadStart));
			preloadThread.IsBackground = true;
			preloadThread.Priority = ThreadPriority.Lowest;
			preloadThread.Start();
		}
		
		public void PreloadThreadStart()
		{
			Console.WriteLine("#Assembly: starting preloading thread");
			SA.SharpAssembly.Load("System");
			Console.WriteLine("#Assembly: preloaded system");
			SA.SharpAssembly.Load("System.Xml");
			Console.WriteLine("#Assembly: preloaded system.xml");
			SA.SharpAssembly.Load("System.Windows.Forms");
			Console.WriteLine("#Assembly: preloaded system.windows.forms");
			SA.SharpAssembly.Load("System.Drawing");
			Console.WriteLine("#Assembly: preloaded system.drawing");
			SA.SharpAssembly.Load("System.Data");
			Console.WriteLine("#Assembly: preloaded system.data");
			SA.SharpAssembly.Load("System.Design");			
			Console.WriteLine("#Assembly: preloaded system.design");
			SA.SharpAssembly.Load("System.Web");			
			Console.WriteLine("#Assembly: preloaded system.web");
		}
	}
	
	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		EventHandler idleEventHandler;
		bool isCalled = false;
		
		/// <remarks>
		/// The worst workaround in the whole project
		/// </remarks>
		void ShowTipOfTheDay(object sender, EventArgs e)
		{
			if (isCalled) {
				//Application.Idle -= idleEventHandler;
				return;
			}
			isCalled = true;
			// show tip of the day
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService.GetProperty("MonoDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", false)) {
				ViewTipOfTheDay dview = new ViewTipOfTheDay();
				dview.Run();
			}
		}
		
		public override void Run()
		{

			if (StartCodeCompletionWizard.generatingCompletionData) {
				Gtk.Application.Run ();
				return;
			}
		
			ReflectionClass reflectionClass = new ReflectionClass(typeof(object));
			
			// register string tag provider (TODO: move to add-in tree :)
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			stringParserService.RegisterStringTagProvider(new MonoDevelop.Commands.SharpDevelopStringTagProvider());
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			//idleEventHandler = new EventHandler(ShowTipOfTheDay);
			//Application.Idle += idleEventHandler;
			
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			// load previous combine
			if ((bool)propertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				object recentOpenObj = propertyService.GetProperty("MonoDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is MonoDevelop.Services.RecentOpen) {
					MonoDevelop.Services.RecentOpen recOpen = (MonoDevelop.Services.RecentOpen)recentOpenObj;
					if (recOpen.RecentProject.Count > 0) { 
						projectService.OpenCombine(recOpen.RecentProject[0].ToString());
					}
				}
			}
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				//FIXME: use mimetypes
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
						try {
							projectService.OpenCombine(file);
						} catch (Exception e) {
							CombineLoadError.HandleError(e, file);
						}
						
						break;
					default:
						try {
							IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
							fileService.OpenFile(file);
						
						} catch (Exception e) {
							Console.WriteLine("unable to open file {0} exception was :\n{1}", file, e.ToString());
						}
						break;
				}
			}
			
			((Gtk.Window)WorkbenchSingleton.Workbench).ShowAll ();
			WorkbenchSingleton.Workbench.SetMemento ((IXmlConvertable)propertyService.GetProperty (workbenchMemento, new WorkbenchMemento ()));
			((Gtk.Window)WorkbenchSingleton.Workbench).Visible = true;
			WorkbenchSingleton.Workbench.RedrawAllComponents ();
			((Gtk.Window)WorkbenchSingleton.Workbench).Present ();
		
			// Give Gtk time to display the workbench window before showing the TOTD.
			while (Gtk.Application.EventsPending ())
				Gtk.Application.RunIteration ();
		
			ShowTipOfTheDay (null, null);
		
			// finally run the workbench window ...
			Gtk.Application.Run ();
		}
	}
}
