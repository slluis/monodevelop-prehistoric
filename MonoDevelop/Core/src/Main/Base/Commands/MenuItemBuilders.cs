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

using Freedesktop.RecentFiles;

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
			RecentOpen recentOpen = Runtime.FileService.RecentOpen;
			
			if (recentOpen.RecentFile != null && recentOpen.RecentFile.Length > 0) {
				RFMItem[] items = new RFMItem[recentOpen.RecentFile.Length];
				
				for (int i = 0; i < recentOpen.RecentFile.Length; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					RecentItem ri = recentOpen.RecentFile[i];
					string label = ri.Private == null ? ri.ToString () : ri.Private;
					items[i] = new RFMItem (null, null, accelaratorKeyPrefix + label.Replace ("_", "__"), new EventHandler (LoadRecentFile));
					items[i].Tag = ri.ToString ();
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
			Runtime.FileService.OpenFile (item.Tag.ToString());
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
			RecentOpen recentOpen = Runtime.FileService.RecentOpen;
			
			if (recentOpen.RecentProject != null && recentOpen.RecentProject.Length > 0) {
				RPMItem[] items = new RPMItem[recentOpen.RecentProject.Length];
				for (int i = 0; i < recentOpen.RecentProject.Length; ++i) {
					string accelaratorKeyPrefix = i < 10 ? "&" + ((i + 1) % 10).ToString() + " " : "";
					RecentItem ri = recentOpen.RecentProject[i];
					string label = ri.Private == null ? ri.ToString () : ri.Private;
					items[i] = new RPMItem(null, null, accelaratorKeyPrefix + label.Replace ("_", "__"), new EventHandler(LoadRecentProject));
					items[i].Tag = ri.ToString ();
					items[i].Description = String.Format (GettextCatalog.GetString ("load solution {0}"), ri.ToString ());
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
			
			//FIXME:THIS IS BROKEN!!
			
			string filename = item.Tag.ToString();
			
			try {
				Runtime.ProjectService.OpenCombine(filename);
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
			StringParserService stringParserService = Runtime.StringParserService;
			
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text == ToolLoader.Tool[i].ToString()) {
					
					ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
					// set the command
					string command = tool.Command;
					// set the args
					string args = stringParserService.Parse(tool.Arguments);
					// prompt for args if needed
					if (tool.PromptForArguments) {
						args = Runtime.MessageService.GetTextResponse(String.Format (GettextCatalog.GetString ("Enter any arguments you want to use while launching tool, {0}:"), tool.MenuCommand), String.Format (GettextCatalog.GetString ("Command Arguments for {0}"), tool.MenuCommand), args);
							
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
						string workingDirectory = stringParserService.Parse(tool.InitialDirectory);
						if (tool.UseOutputPad)
							Runtime.ProcessService.StartProcess (command, args, workingDirectory, new ProcessEventHandler (OnStdOut), new ProcessEventHandler (OnStdErr), new EventHandler (OnExited));
						else
							Runtime.ProcessService.StartProcess (command, args, workingDirectory, new EventHandler (OnExited));
					} catch (Exception ex) {
						Runtime.MessageService.ShowError(ex, String.Format (GettextCatalog.GetString ("External program execution failed.\nError while starting:\n '{0} {1}'"), command, args));
					}
					break;
				}
			}
		}
		
		private void OnStdOut (object sender, string s)
		{
			lock (Runtime.TaskService.CompilerOutput)
			{
				Runtime.TaskService.CompilerOutput += s + Environment.NewLine;
			}
		}
		
		private void OnStdErr (object sender, string s)
		{
			lock (Runtime.TaskService.CompilerOutput)
			{
				Runtime.TaskService.CompilerOutput += s + Environment.NewLine;
			}
		}
		
		private void OnExited (object sender, EventArgs e)
		{
			ProcessWrapper wrapper = (ProcessWrapper)sender;
			
			lock (Runtime.TaskService.CompilerOutput)
			{ 
				if (wrapper.ExitCode == 0) {
					Runtime.TaskService.CompilerOutput += string.Format("{0}Process {1} has completed succesfully.{0}", Environment.NewLine, wrapper.ProcessName); 
				} else {
					Runtime.TaskService.CompilerOutput += string.Format("{0}Process {1} has exited with errorcode {2}.{0}", Environment.NewLine, wrapper.ProcessName, wrapper.ExitCode);
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
					if (!Gtk.AccelMap.LookupEntry (accel_path, new Gtk.AccelKey ())) {
						Gtk.AccelMap.AddEntry (accel_path, Gdk.Keyval.FromName ((i + 1).ToString ()), Gdk.ModifierType.Mod1Mask);
					} else {
						Gtk.AccelMap.ChangeEntry (accel_path, Gdk.Keyval.FromName ((i + 1).ToString()), Gdk.ModifierType.Mod1Mask, true);
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
			Runtime.ProjectService.SaveCombine();
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
			Runtime.ProjectService.SaveCombine();
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
