// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
 
using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using Gtk;
using MonoDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser; // FIXME REMOVE PEDRO
using ICSharpCode.SharpDevelop.Gui.Pads;				//

namespace ICSharpCode.SharpDevelop.Gui
{	
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFile = propertyService.ConfigDirectory + "MdiLayoutConfig.xml";

		private IWorkbench workbench;
		Window wbWindow;
		Container rootWidget;
		Container toolbarContainer;

		Notebook tabControl;

		//FIXME: this is also an ugly hack
		//VBox mainBox;
		DockingManager dockManager;
		//ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab tabControl = new ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab();

		ArrayList _windows = new ArrayList ();

		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (tabControl == null || tabControl.CurrentPage < 0 || tabControl.CurrentPage >= tabControl.NPages)  {
					return null;
				}
				return (IWorkbenchWindow)_windows[tabControl.CurrentPage];
			}
		}
		
//		void LeftSelectionChanged(object sender, EventArgs e)
//		{
//			if (tabControlLeft.SelectedTab == null) {
//				return;
//			}
//			leftContent.Title = tabControlLeft.SelectedTab.Title;
//		}
//		
//		void BottomSelectionChanged(object sender, EventArgs e)
//		{
//			if (tabControlBottom.SelectedTab == null) {
//				return;
//			}
//			bottomContent.Title = tabControlBottom.SelectedTab.Title;
//		}
		
		public void Attach(IWorkbench wb)
		{
			DefaultWorkbench workbench = (DefaultWorkbench) wb;
			//Console.WriteLine("Call to SdiWorkSpaceLayout.Attach");

			this.workbench = workbench;
			wbWindow = (Window) workbench;
			
			Gtk.VBox mainBox = new VBox (false, 2);
			tabControl = new Notebook();
			
			Gtk.VBox vbox = new VBox (false, 0);
			rootWidget = vbox;

			vbox.PackStart (workbench.TopMenu, false, false, 0);
			if (workbench.ToolBars != null) {
				VBox toolvbox = new VBox (false, 0);
				toolbarContainer = toolvbox;
				for (int i = 0; i < workbench.ToolBars.Length; i++) {
					toolvbox.PackStart (workbench.ToolBars[i], false, false, 0);
				}
				vbox.PackStart(toolvbox, false, false, 0);
			}
			
			vbox.PackStart(mainBox);

			workbench.Add (vbox);
			
			//dockManager = new DockingManager(wbWindow);
			dockManager = new DockingManager(mainBox, tabControl);

/*
			wbForm = (Form)workbench;
			wbForm.Controls.Clear();

			tabControl.Dock = DockStyle.Fill;
			tabControl.ShrinkPagesToFit = true;
			tabControl.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
			wbForm.Controls.Add(tabControl);

			dockManager = new DockingManager(wbForm, Crownwood.Magic.Common.VisualStyle.IDE);
			
			
//			Control firstControl = null;
*/
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			mainBox.PackEnd (statusBarService.Control, false, true, 0);
/*			
			wbForm.Add (statusBarService.Control);
			((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(((DefaultWorkbench)workbench).TopMenu);
			foreach (CommandBar toolBar in ((DefaultWorkbench)workbench).ToolBars) {
				((DefaultWorkbench)workbench).commandBarManager.CommandBars.Add(toolBar);
			}
			wbForm.Controls.Add(((DefaultWorkbench)workbench).commandBarManager);
			
			wbForm.Menu = null;
			dockManager.InnerControl = tabControl;
			dockManager.OuterControl = statusBarService.Control;
*/
			
			foreach (IViewContent content in workbench.ViewContentCollection) {
				ShowView(content);
			}
			
			foreach (IPadContent content in workbench.PadContentCollection) {
				ShowPad(content);
			}

			// FIXME: GTKize
			//tabControl.SwitchPage += new EventHandler(ActiveMdiChanged);
			//tabControl.SelectionChanged += new EventHandler(ActiveMdiChanged);
			
			try {
				if (File.Exists(configFile)) {
					// FIXME: GTKize
					//dockManager.LoadConfigFromFile(configFile);
				} else {
					CreateDefaultLayout();
				}
			} catch (Exception) {
				Console.WriteLine("can't load docking configuration, version clash ?");
			}
			RedrawAllComponents();
			//wbWindow.ShowAll ();
		}

		public Gtk.Widget LayoutWidget {
			get { return rootWidget; }
		}
		
		Content GetContent(string padTypeName)
		{
			IPadContent pad = ((IWorkbench)wbWindow).PadContentCollection[padTypeName];
			if (pad != null) {
				return (Content)contentHash[pad];
			}
			return null;
		}
		
		void CreateDefaultLayout()
		{
			Console.WriteLine("Creating Default Layout...");
			WindowContent leftContent   = null;
			WindowContent rightContent  = null;
			WindowContent bottomContent = null;
			
			string[] leftContents = new string[] {
				"ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView",
				"ICSharpCode.SharpDevelop.Gui.Pads.ClassScout",
				"ICSharpCode.SharpDevelop.Gui.Pads.FileScout",
				"ICSharpCode.SharpDevelop.Gui.Pads.SideBarView"
			};
			string[] rightContents = new string[] {
				"ICSharpCode.SharpDevelop.Gui.Pads.PropertyPad",
				"ICSharpCode.SharpDevelop.Gui.Pads.HelpBrowser"
			};
			string[] bottomContents = new string[] {
				"ICSharpCode.SharpDevelop.Gui.Pads.OpenTaskView",
				"ICSharpCode.SharpDevelop.Gui.Pads.CompilerMessageView"
			};
			
			foreach (string typeName in leftContents) {
				Content c = GetContent(typeName);
				if (c != null) {
					if (leftContent == null) {
						leftContent = dockManager.AddContentWithState(c, DockState.Left) as WindowContent;
					} else {
						dockManager.AddContentToWindowContent(c, leftContent);
					}
				}
			}
			
			foreach (string typeName in bottomContents) {
				Content c = GetContent(typeName);
				if (c != null) {
					if (bottomContent == null) {
						bottomContent = dockManager.AddContentWithState(c, DockState.Bottom) as WindowContent;
					} else {
						dockManager.AddContentToWindowContent(c, bottomContent);
					}
				}
			}
			
			foreach (string typeName in rightContents) {
				Content c = GetContent(typeName);
				if (c != null) {
					if (rightContent == null) {
						rightContent = dockManager.AddContentWithState(c, DockState.Right) as WindowContent;
					} else {
						dockManager.AddContentToWindowContent(c, rightContent);
					}
				}
			}
			//Console.WriteLine(" Default Layout created.");
		}		

		public void Detach()
		{
			Console.WriteLine("Call to SdiWorkSpaceLayout.Detach");
			rootWidget.Remove(((DefaultWorkbench)workbench).TopMenu);
			foreach (Gtk.Widget w in toolbarContainer.Children) {
				toolbarContainer.Remove(w);
			}
			rootWidget.Remove(toolbarContainer);
			wbWindow.Remove(rootWidget);

/*			try {
				if (dockManager != null) {
					dockManager.SaveConfigToFile(configFile);
				}
				
				foreach (Crownwood.Magic.Controls.TabPage page in tabControl.TabPages) {
					SdiWorkspaceWindow f = (SdiWorkspaceWindow)page.Tag;
					f.DetachContent();
					f.ViewContent = null;
				}
				
				tabControl.TabPages.Clear();
				tabControl.Controls.Clear();
				
				if (dockManager != null) {
					dockManager.Contents.Clear();
				}
				
				wbForm.Controls.Clear();
			} catch (Exception) {}
*/
		}
		
//		WindowContent leftContent = null;
//		WindowContent bottomContent = null;
		Hashtable contentHash = new Hashtable();
		
	
		public void ShowPad(IPadContent content)
		{
			// FIXME: GTKize			

			if (contentHash[content] == null) {
				IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
				//content.Control.Dock = DockStyle.None;
				Content newContent;
				if (content.Icon != null) {
					//ImageList imgList = new ImageList();
					//imgList.ColorDepth = ColorDepth.Depth32Bit;
					IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
					//imgList.Images.Add(iconService.GetBitmap(content.Icon));
					//newContent = dockManager.Contents.Add(content.Control, content.Title, imgList, 0);
					newContent = dockManager.Contents.Add(content.Control, content.Title, iconService.GetBitmap(content.Icon));
				} else {
					newContent = dockManager.Contents.Add(content.Control, content.Title);
				}
				contentHash[content] = newContent;
			} else {
				Content c = (Content)contentHash[content];
				if (c != null) {
					dockManager.ShowContent(c);
				}
			}
		}
		
		public bool IsVisible(IPadContent padContent)
		{
			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					return content.Visible;
				}
			}

			return false;
		}
		
		public void HidePad(IPadContent padContent)
		{
			// FIXME: GTKize

			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					dockManager.HideContent(content);
				}
			}

		}
		
		public void ActivatePad(IPadContent padContent)
		{
			// FIXME: GTKize

			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					content.BringToFront();
				}
			}
		}
		
		public void RedrawAllComponents()
		{
			// FIXME: GTKize

			//tabControl.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.TabVisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);

			// redraw correct pad content names (language changed).
			//foreach (IPadContent content in ((IWorkbench)wbForm).PadContentCollection) {
			foreach (IPadContent content in ((IWorkbench)workbench).PadContentCollection) {
				Content c = (Content)contentHash[content];
				if (c != null) {
					c.Title = c.FullTitle = content.Title;
				}
			}
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
			// FIXME: GTKize

			SdiWorkspaceWindow f = (SdiWorkspaceWindow)sender;
			if (f.ViewContent != null) {
				((IWorkbench)wbWindow).CloseContent(f.ViewContent);
				ActiveMdiChanged(this, null);
			}

		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{	
			//content.Control.Dock = DockStyle.None;
			//content.Control.Visible = true;
			

			string title = "";
			if (content.IsUntitled) {
				title = content.UntitledName;
			} else {
				title = Path.GetFileName (content.ContentName);
			}
			
			Label label = new Label(title);
			tabControl.AppendPage (content.Control, label);

			SdiWorkspaceWindow sdiWorkspaceWindow = new SdiWorkspaceWindow(content, tabControl, label);

			sdiWorkspaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
			sdiWorkspaceWindow.SwitchView(tabControl.Children.Count - 1);
			_windows.Add (sdiWorkspaceWindow);
			
			tabControl.ShowAll();
			return sdiWorkspaceWindow;
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
