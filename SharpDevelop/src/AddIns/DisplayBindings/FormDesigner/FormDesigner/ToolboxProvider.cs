// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.SharpDevelop.FormDesigner.Gui;
using ICSharpCode.Core.AddIns.Codons;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class ToolboxProvider
	{
		static ToolboxService         toolboxService = null;
		static ITypeResolutionService typeResolutionService = new TypeResolutionService();
		public static ArrayList       SideTabs = new ArrayList();
		
		static ComponentLibraryLoader componentLibraryLoader = new ComponentLibraryLoader();
		
		public static ITypeResolutionService TypeResolutionService {
			get {
				return typeResolutionService;
			}
		}
		
		public static ComponentLibraryLoader ComponentLibraryLoader {
			get {
				return componentLibraryLoader;
			}
		}
		public static ToolboxService ToolboxService {
			get {
				if (toolboxService == null) {
					toolboxService = new ToolboxService();
					ReloadSideTabs(false);
					toolboxService.SelectedItemUsed += new EventHandler(SelectedToolUsedHandler);
				}
				return toolboxService;
			}
		}
		
		static ToolboxProvider()
		{
			LoadToolbox();
		}
		static string componentLibraryFile = "SharpDevelopControlLibrary.sdcl";
		
		static string GlobalConfigFile {
			get {
				PropertyService propertyService = (PropertyService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(PropertyService));
				return propertyService.DataDirectory + Path.DirectorySeparatorChar + 
				       "options" + Path.DirectorySeparatorChar +
				       componentLibraryFile;
			}
		}
		
		static string UserConfigFile {
			get {
				PropertyService propertyService = (PropertyService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(PropertyService));
				return propertyService.ConfigDirectory + Path.DirectorySeparatorChar + componentLibraryFile;
			}
		}
		
		public static void SaveToolbox()
		{
			componentLibraryLoader.SaveToolComponentLibrary(UserConfigFile);
		}
		
		public static void LoadToolbox()
		{
			if (!componentLibraryLoader.LoadToolComponentLibrary(UserConfigFile)) {
				if (!componentLibraryLoader.LoadToolComponentLibrary(GlobalConfigFile)) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowWarning("Can't load side bar component library.\nNo Windows Forms components will be avaiable, please configure\nthe side bar manually.\n(right click on a side bar category->Customize SideBar)");
				}
			}
		}
		
		public static void ReloadSideTabs(bool doInsert)
		{
			bool reInsertTabs = false;
			foreach(AxSideTab tab in SideTabs) {
				if (SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
					SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
					reInsertTabs = true;;
				}
			}
			reInsertTabs &= doInsert;
			
			SideTabs.Clear();
			foreach (Category category in componentLibraryLoader.Categories) {
				if (category.IsEnabled) {
					SideTabDesigner newTab = new SideTabDesigner(SharpDevelopSideBar.SideBar, category, toolboxService);
					SideTabs.Add(newTab);
				}
			}
			SideTabDesigner customTab = new CustomComponentsSideTab(SharpDevelopSideBar.SideBar, "Custom Components", toolboxService);
			SideTabs.Add(customTab);
			if (reInsertTabs) {
				foreach(AxSideTab tab in SideTabs) {
					SharpDevelopSideBar.SideBar.Tabs.Add(tab);
				}
			}
		}
		
		static void SelectedToolUsedHandler(object sender, EventArgs e)
		{
			AxSideTab tab = SharpDevelopSideBar.SideBar.ActiveTab;
			
			// try to add project reference
			if (sender != null && sender is ToolboxService && !(tab is CustomComponentsSideTab)) {
				ToolboxItem selectedItem = (sender as IToolboxService).GetSelectedToolboxItem();
				if (selectedItem != null) {
					if (selectedItem.AssemblyName != null) {
						//We Put the assembly reference into the reference project folder
						IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
						IProject currentProject = projectService.CurrentSelectedProject;
						
						if (currentProject != null) {
							bool isAlreadyInRefFolder = false;
							
							if (currentProject.ProjectType == "C#") {
								foreach (string assembly in DefaultParserService.AssemblyList) {
									if (selectedItem.AssemblyName.FullName.StartsWith(assembly + ",")) {
										isAlreadyInRefFolder = true;
										break;
									}
								}
							}
							
							foreach (ProjectReference refproj in currentProject.ProjectReferences) {
								if (refproj.Reference == selectedItem.AssemblyName.FullName) {
									isAlreadyInRefFolder = true;
								}
							}
							
							if (!isAlreadyInRefFolder) {
								currentProject.ProjectReferences.Add(new ProjectReference(ReferenceType.Gac, selectedItem.AssemblyName.FullName));
								ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView));
								pbv.UpdateCombineTree();
								projectService.SaveCombine();
							}
						} 
					}
				}
			}
			
			if (tab.Items.Count > 0) {
				tab.ChoosedItem = tab.Items[0];
			}
			SharpDevelopSideBar.SideBar.Refresh();
		}
	}
}
