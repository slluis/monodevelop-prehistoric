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

	public interface ISubmenuItem
	{
	}
	
	public class RecentFilesMenuBuilder : ISubmenuBuilder
	{

		class RFMItem : SdMenuCommand, ISubmenuItem {
			public RFMItem (ConditionCollection a, object b, string c) : base (a, b, c) {
			}
			public RFMItem (ConditionCollection a, object b, string c, EventHandler d) : base (a, b, c, d) {
			}
		}
		
		
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentFile.Count > 0) {
				RFMItem[] items = new RFMItem[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					items[i] = new RFMItem(null, null, accelaratorKeyPrefix + recentOpen.RecentFile[i].ToString(), new EventHandler(LoadRecentFile));
					items[i].Tag = recentOpen.RecentFile[i].ToString();
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadFileDescription"),
					                                          new string[,] { {"FILE", recentOpen.RecentFile[i].ToString()} });
				}
				return items;
			}
			
			RFMItem defaultMenu = new RFMItem(null, null, GettextCatalog.GetString("recent files"));
			defaultMenu.Sensitive = false;
			
			return new RFMItem[] { defaultMenu };
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
		class RPMItem : SdMenuCommand, ISubmenuItem {
			public RPMItem (ConditionCollection a, object b, string c) : base (a, b, c) {
			}
			public RPMItem (ConditionCollection a, object b, string c, EventHandler d) : base (a, b, c, d) {
			}
		}
		
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentProject.Count > 0) {
				RPMItem[] items = new RPMItem[recentOpen.RecentProject.Count];
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					items[i] = new RPMItem(null, null, accelaratorKeyPrefix + recentOpen.RecentProject[i].ToString(), new EventHandler(LoadRecentProject));
					items[i].Tag = recentOpen.RecentProject[i].ToString();
					items[i].Description = String.Format (GettextCatalog.GetString ("load solution {0}"), recentOpen.RecentProject[i].ToString ());
				}
				return items;
			}
			
			RPMItem defaultMenu = new RPMItem(null, null, GettextCatalog.GetString ("recent solutions"));
			defaultMenu.Sensitive = false;
			
			return new RPMItem[] { defaultMenu };
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
		class TMItem : SdMenuCommand, ISubmenuItem {
			public TMItem (ConditionCollection a, object b, string c, EventHandler d) : base (a, b, c, d) {
			}
		}
		
		public Gtk.MenuItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			TMItem[] items = new TMItem[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				TMItem item = new TMItem(null, null, ToolLoader.Tool[i].ToString(), new EventHandler(ToolEvt));
				item.Description = GettextCatalog.GetString ("Start tool") + " " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
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
						args = messageService.GetTextResponse(String.Format (GettextCatalog.GetString ("Enter any arguments you want to use while launching tool, {0}:"), tool.MenuCommand), String.Format (GettextCatalog.GetString ("Command Arguments for {0}"), tool.MenuCommand), args);
							
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
						startinfo.UseShellExecute = false;
						
						startinfo.WorkingDirectory = stringParserService.Parse(tool.InitialDirectory);
						
						// FIXME: need to find a way to wire the console output into the output window if specified
						Process.Start(startinfo);
						
					} catch (Exception ex) {								messageService.ShowError(ex, String.Format (GettextCatalog.GetString ("External program execution failed.\nError while starting:\n '{0} {1}'"), command, args));
					}
					break;
				}
			}
		}
	}
	
	public class OpenContentsMenuBuilder : ISubmenuBuilder
	{
				
		class MyMenuItem : SdMenuCheckBox, ISubmenuItem
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
			Gtk.MenuItem[] items = new Gtk.MenuItem[contentCount];
			for (int i = 0; i < contentCount; ++i) {
				IViewContent content = (IViewContent)WorkbenchSingleton.Workbench.ViewContentCollection[i];
				
				MyMenuItem item = new MyMenuItem(content.WorkbenchWindow.Title);
				item.Tag = content.WorkbenchWindow;
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == content.WorkbenchWindow) {
					item.Active = true;
				} else {
					item.Active = false;
				}
				item.Description = GettextCatalog.GetString ("Activate this window");
				if (i + 1 <= 9) {
					string accel_path = "<MonoDevelop>/MainWindow/OpenContents_" + (i + 1).ToString ();
					if (!Gtk.Accel.MapLookupEntry (accel_path, new Gtk.AccelKey ())) {
						Gtk.Accel.MapAddEntry (accel_path, Gdk.Keyval.FromName ((i + 1).ToString ()), Gdk.ModifierType.Mod1Mask);
					} else {
						Gtk.Accel.MapChangeEntry (accel_path, Gdk.Keyval.FromName ((i + 1).ToString()), Gdk.ModifierType.Mod1Mask, true);
					}
					item.AccelPath = accel_path;
				}
				items[i] = item;
			}
			return items;
		}
	}
	
	public class IncludeFilesBuilder : ISubmenuBuilder
	{
		public ProjectBrowserView browser;
		
		MyMenuItem includeInCompileItem;
		MyMenuItem includeInDeployItem;
		
		class MyMenuItem : SdMenuCheckBox, ISubmenuItem
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
			includeInCompileItem = new MyMenuItem(this, GettextCatalog.GetString ("Compile"), new EventHandler(ChangeCompileInclude));
			includeInDeployItem  = new MyMenuItem(this, GettextCatalog.GetString ("Deploy"), new EventHandler(ChangeDeployInclude));
			
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
		class MyMenuItem : SdMenuCheckBox, ISubmenuItem
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
			
			return (MyMenuItem[])items.ToArray(typeof(MyMenuItem));
		}
	}

	public class LayoutsMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : SdMenuCheckBox, ISubmenuItem
		{
			string layoutName;
			
			bool IsCurrentLayout {
				get {
					return (WorkbenchSingleton.Workbench.WorkbenchLayout.CurrentLayout == layoutName);
				}
			}
			
			public MyMenuItem (string name) : base (null, null, name)
			{
				this.layoutName = name;
				Active = IsCurrentLayout;
				Toggled += new EventHandler (OnClick);
			}
			
			protected new void OnClick(object o, EventArgs e)
			{
				WorkbenchSingleton.Workbench.WorkbenchLayout.CurrentLayout = layoutName;
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
				string[] layouts = wb.WorkbenchLayout.Layouts;
				Array.Sort (layouts);
				foreach (string layout in layouts) {
					items.Add (new MyMenuItem (layout));
				}
			}
			
			return (MyMenuItem[]) items.ToArray (typeof (MyMenuItem));
		}
	}
}
