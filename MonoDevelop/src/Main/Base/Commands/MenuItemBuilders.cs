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
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.AddIns.Conditions;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;

using MonoDevelop.Services;
using MonoDevelop.Gui;
using MonoDevelop.Gui.Components;
using MonoDevelop.Gui.ErrorHandlers;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Internal.Project;

using MonoDevelop.Internal.ExternalTool;
using MonoDevelop.Gui.Pads.ProjectBrowser;

namespace MonoDevelop.Commands
{
	public class RecentFilesMenuBuilder : ISubmenuBuilder
	{
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentFile.Count > 0) {
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					items[i] = new SdMenuCommand(null, null, accelaratorKeyPrefix + recentOpen.RecentFile[i].ToString(), new EventHandler(LoadRecentFile));
					items[i].Tag = recentOpen.RecentFile[i].ToString();
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadFileDescription"),
					                                          new string[,] { {"FILE", recentOpen.RecentFile[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(null, null, resourceService.GetString("Dialog.Componnents.RichMenuItem.NoRecentFilesString"));
			defaultMenu.Sensitive = false;
			
			return new SdMenuCommand[] { defaultMenu };
		}
		
		void LoadRecentFile(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(item.Tag.ToString());
		}
	}
	
	public class RecentProjectsMenuBuilder : ISubmenuBuilder
	{
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentProject.Count > 0) {
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentProject.Count];
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					items[i] = new SdMenuCommand(null, null, accelaratorKeyPrefix + recentOpen.RecentProject[i].ToString(), new EventHandler(LoadRecentProject));
					items[i].Tag = recentOpen.RecentProject[i].ToString();
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadProjectDescription"),
					                                         new string[,] { {"PROJECT", recentOpen.RecentProject[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(null, null, resourceService.GetString("Dialog.Componnents.RichMenuItem.NoRecentProjectsString"));
			defaultMenu.Sensitive = false;
			
			return new SdMenuCommand[] { defaultMenu };
		}
		void LoadRecentProject(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			//FIXME:THIS IS BROKEN!!
			
			string filename = item.Tag.ToString();
			
			try {
				projectService.OpenCombine(filename);
			} catch (Exception ex) {
				CombineLoadError.HandleError(ex, filename);
			}
		}		
	}
	
	public class ToolMenuBuilder : ISubmenuBuilder
	{
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			SdMenuCommand[] items = new SdMenuCommand[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				SdMenuCommand item = new SdMenuCommand(null, null, ToolLoader.Tool[i].ToString(), new EventHandler(ToolEvt));
				item.Description = "Start tool " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
				items[i] = item;
			}
			return items;
		}
		
		void ToolEvt(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			MessageService messageService =(MessageService)ServiceManager.Services.GetService(typeof(MessageService));
			
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text == ToolLoader.Tool[i].ToString()) {
					
					ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
					// set the command
					string command = tool.Command;
					// set the args
					string args = stringParserService.Parse(tool.Arguments);
					// prompt for args if needed
					if (tool.PromptForArguments) {
						args = messageService.GetTextResponse(
							"Enter any arguments you want to use while launching tool, " + tool.MenuCommand + ":",
							"Command Arguments for " + tool.MenuCommand,
							args);
							
						// if user selected cancel string will be null
						if (args == null) {
							args = stringParserService.Parse(tool.Arguments);
						}
					}
					
					// debug command and args
					Console.WriteLine("command : " + command);
					Console.WriteLine("args    : " + args);
					
					// create the process
					try {
						ProcessStartInfo startinfo;
						if (args == null || args.Length == 0) {
							startinfo = new ProcessStartInfo(command);
						} else {
							startinfo = new ProcessStartInfo(command, args);
						}
						
						startinfo.WorkingDirectory = stringParserService.Parse(tool.InitialDirectory);
						
						// FIXME: need to find a way to wire the console output into the output window if specified
						Process.Start(startinfo);
						
					} catch (Exception ex) {						
						messageService.ShowError(ex, "External program execution failed.\nError while starting:\n '" + command + " " + args + "'");
					}
						break;
					}
				}
			}
		}
				
	public class OpenContentsMenuBuilder : ISubmenuBuilder
	{
				
		class MyMenuItem : SdMenuCheckBox
		{
			public MyMenuItem(string name) : base(null, null, name)
			{
				Toggled += new EventHandler (OnClick);
			}
			
			protected new void OnClick(object o, EventArgs e)
			{
				((IWorkbenchWindow)Tag).SelectWindow();
				Active = true;
			}
		}

		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			int contentCount = WorkbenchSingleton.Workbench.ViewContentCollection.Count;
			if (contentCount == 0) {
				return new Gtk.MenuItem[] {};
			}
			Gtk.MenuItem[] items = new Gtk.MenuItem[contentCount + 1];
			items[0] = new SdMenuSeparator(null, null);
			for (int i = 0; i < contentCount; ++i) {
				IViewContent content = (IViewContent)WorkbenchSingleton.Workbench.ViewContentCollection[i];
				
				SdMenuCheckBox item = new MyMenuItem(content.WorkbenchWindow.Title);
				item.Tag = content.WorkbenchWindow;
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == content.WorkbenchWindow) {
					item.Active = true;
				} else {
					item.Active = false;
				}
				item.Description = "Activate this window ";
				if (i + 1 <= 9) {
					string accel_path = "<MonoDevelop>/MainWindow/" + content.WorkbenchWindow.Title + (i + 1).ToString ();
					if (!Gtk.Accel.MapLookupEntry (accel_path, new Gtk.AccelKey ())) {
						Gtk.Accel.MapAddEntry (accel_path, Gdk.Keyval.FromName ((i + 1).ToString ()), Gdk.ModifierType.ControlMask);
						item.AccelPath = accel_path;
					} else {
						Gtk.Accel.MapChangeEntry (accel_path, Gdk.Keyval.FromName ((i + 1).ToString()), Gdk.ModifierType.ControlMask, true);
						item.AccelPath = accel_path;
					}
				}
				items[i + 1] = item;
			}
			return items;
		}
	}
	
	public class IncludeFilesBuilder : ISubmenuBuilder
	{
		public ProjectBrowserView browser;
		
		MyMenuItem includeInCompileItem;
		MyMenuItem includeInDeployItem;
		
		class MyMenuItem : SdMenuCheckBox
		{
			IncludeFilesBuilder builder;
			
			public MyMenuItem(IncludeFilesBuilder builder, string name, EventHandler handler) : base(null, null, name)
			{
				base.Toggled += handler;
				this.builder = builder;
			}
			
			public override void UpdateStatus()
			{
				base.UpdateStatus();
				if (builder == null) {
					return;
				}
				AbstractBrowserNode node = builder.browser.SelectedNode as AbstractBrowserNode;
				
				if (node == null) {
					return;
				}
				
				ProjectFile finfo = node.UserData as ProjectFile;
				if (finfo == null) {
					builder.includeInCompileItem.Sensitive = builder.includeInCompileItem.Sensitive = false;
				} else {
					if (!builder.includeInCompileItem.Sensitive) {
						builder.includeInCompileItem.Sensitive = builder.includeInCompileItem.Sensitive = true;
					}
					builder.includeInCompileItem.Active = finfo.BuildAction == BuildAction.Compile;
					builder.includeInDeployItem.Active  = !node.Project.DeployInformation.IsFileExcluded(finfo.Name);
				}
			}
		}
		
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			browser = (ProjectBrowserView)owner;
			includeInCompileItem = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InCompile}", new EventHandler(ChangeCompileInclude));
			includeInDeployItem  = new MyMenuItem(this, "${res:ProjectComponent.ContextMenu.IncludeMenu.InDeploy}",  new EventHandler(ChangeDeployInclude));
			
			return new Gtk.MenuItem[] {
				includeInCompileItem,
				includeInDeployItem
			};
			
		}
		void ChangeCompileInclude(object sender, EventArgs e)
		{
			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
			
			if (node == null) {
				return;
			}
			
			ProjectFile finfo = node.UserData as ProjectFile;
			if (finfo != null) {
				if (finfo.BuildAction == BuildAction.Compile) {
					finfo.BuildAction = BuildAction.Nothing;
				} else {
					finfo.BuildAction = BuildAction.Compile;
				}
			}
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
		
		void ChangeDeployInclude(object sender, EventArgs e)
		{
			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
			
			if (node == null) {
				return;
			}
			
			ProjectFile finfo = node.UserData as ProjectFile;
			if (finfo != null) {
				if (node.Project.DeployInformation.IsFileExcluded(finfo.Name)) {
					node.Project.DeployInformation.RemoveExcludedFile(finfo.Name);
				} else {
					node.Project.DeployInformation.AddExcludedFile(finfo.Name);
				}
			}
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
	}
	
	
	public class ViewMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : SdMenuCheckBox
		{
			IPadContent padContent;
			
			bool IsPadVisible {
				get {
					return WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(padContent); 
				}
			}
			
			public MyMenuItem(IPadContent padContent) : base(null, null, padContent.Title)
			{
				this.padContent = padContent;
				Active = IsPadVisible;
				Toggled += new EventHandler (OnClick);
			}
			
			protected new void OnClick(object o, EventArgs e)
			{
				if (IsPadVisible) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(padContent);
				} else {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(padContent);
				}
				Active = IsPadVisible;
			}
			public override  void UpdateStatus()
			{
				base.UpdateStatus();
			}
		}
		
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			ArrayList items = new ArrayList();
			IWorkbench wb = WorkbenchSingleton.Workbench;
			if (wb.WorkbenchLayout != null)
			{
				PadContentCollection pads = wb.WorkbenchLayout.PadContentCollection;
				foreach (IPadContent padContent in pads) {
					items.Add(new MyMenuItem(padContent));
				}
			}
			
			return (Gtk.MenuItem[])items.ToArray(typeof(Gtk.MenuItem));
		}
	}
}
