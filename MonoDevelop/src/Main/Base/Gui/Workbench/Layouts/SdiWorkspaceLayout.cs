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

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using Gtk;
using Gdl;
using GdlSharp;
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Gui.Utils;

namespace MonoDevelop.Gui
{	
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFile = propertyService.ConfigDirectory + "DefaultEditingLayout.xml";

		private IWorkbench workbench;
		Window wbWindow;
		Container rootWidget;
		Container toolbarContainer;
		Dock dock;
		DockLayout dockLayout;
		Notebook tabControl;

		ArrayList _windows = new ArrayList ();

		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (tabControl == null || tabControl.CurrentPage < 0 || tabControl.CurrentPage >= tabControl.NPages)  {
					return null;
				}
				return (IWorkbenchWindow)_windows[tabControl.CurrentPage];
			}
		}
		
		public void Attach (IWorkbench wb)
		{
			DefaultWorkbench workbench = (DefaultWorkbench) wb;

			this.workbench = workbench;
			wbWindow = (Window) workbench;
			
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
			
			// Create the docking widget and add it to the window.
			dock = new Dock ();
			DockBar dockBar = new DockBar (dock);
			Gtk.HBox dockBox = new HBox (false, 5);
			dockBox.PackStart (dockBar, false, true, 0);
			dockBox.PackStart (dock, true, true, 0);
			vbox.PackStart (dockBox, true, true, 0);

			// Create the notebook for the various documents.
			tabControl = new Notebook ();
			tabControl.Scrollable = true;
			tabControl.ShowTabs = false;
			tabControl.SwitchPage += new SwitchPageHandler (ActiveMdiChanged);
			DockItem item = new DockItem ("Documents", "Documents",
						      DockItemBehavior.Locked);
			item.Add (tabControl);
			item.ShowAll ();
			dock.AddItem (item, DockPlacement.Center);

			workbench.Add (vbox);
			
			IStatusBarService statusBarService = (IStatusBarService) ServiceManager.Services.GetService (typeof (IStatusBarService));
			vbox.PackEnd (statusBarService.Control, false, true, 0);
			
			foreach (IViewContent content in workbench.ViewContentCollection)
				ShowView (content);
			
			foreach (IPadContent content in workbench.PadContentCollection)
				ShowPad (content);

			// FIXME: GTKize
			//tabControl.SwitchPage += new EventHandler(ActiveMdiChanged);
			//tabControl.SelectionChanged += new EventHandler(ActiveMdiChanged);
			
			CreateDefaultLayout();
			RedrawAllComponents();
			wbWindow.ShowAll ();
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
			//Console.WriteLine("Creating Default Layout...");
			WindowContent leftContent   = null;
			WindowContent rightContent  = null;
			WindowContent bottomContent = null;
			
			string[] leftContents = new string[] {
				"MonoDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView",
				"MonoDevelop.Gui.Pads.ClassScout",
				"MonoDevelop.Gui.Pads.FileScout",
				"MonoDevelop.Gui.Pads.SideBarView"
			};
			string[] rightContents = new string[] {
				"MonoDevelop.Gui.Pads.PropertyPad",
				"MonoDevelop.Gui.Pads.HelpBrowser"
			};
			string[] bottomContents = new string[] {
				"MonoDevelop.Gui.Pads.OpenTaskView",
				"MonoDevelop.EditorBindings.Gui.Pads.CompilerMessageView",
				"MonoDevelop.SourceEditor.Gui.DebuggerLocalsPad"
			};
			
			foreach (string typeName in leftContents) {
				Content c = GetContent (typeName);
				if (c != null) {
					DockItem item = new DockItem (typeName, c.Title, c.Image,
								      DockItemBehavior.Normal);
					item.Add (c.Widget);
					item.ShowAll ();
					dock.AddItem (item, DockPlacement.Left);
				}
			}
			
			foreach (string typeName in bottomContents) {
				Content c = GetContent (typeName);
				if (c != null) {
					DockItem item = new DockItem (typeName, c.Title, c.Image,
								      DockItemBehavior.Normal);
					item.Add (c.Widget);
					item.ShowAll ();
					dock.AddItem (item, DockPlacement.Bottom);
				}
			}
			
			foreach (string typeName in rightContents) {
				Content c = GetContent (typeName);
				if (c != null) {
					DockItem item = new DockItem (typeName, c.Title, c.Image,
								      DockItemBehavior.Normal);
					item.Add (c.Widget);
					item.ShowAll ();
					dock.AddItem (item, DockPlacement.Right);
				}
			}
			//Console.WriteLine(" Default Layout created.");
			dockLayout = new DockLayout (dock);
			if (File.Exists (configFile)) {
				dockLayout.LoadFromFile (configFile);
			} else {
				dockLayout.LoadFromFile ("../data/options/DefaultEditingLayout.xml");
			}
			dockLayout.LoadLayout ("__default__");
		}		

		public void Detach()
		{
			//Console.WriteLine("Call to SdiWorkSpaceLayout.Detach");
			dockLayout.SaveToFile (configFile);
			rootWidget.Remove(((DefaultWorkbench)workbench).TopMenu);
			foreach (Gtk.Widget w in toolbarContainer.Children) {
				toolbarContainer.Remove(w);
			}
			rootWidget.Remove(toolbarContainer);
			wbWindow.Remove(rootWidget);
		}

		Hashtable contentHash = new Hashtable();
		
	
		public void ShowPad (IPadContent content)
		{
			if (contentHash[content] == null) {
				/*DockItem item = new DockItem (content.Title,
							      content.Title,
							      DockItemBehavior.Normal);
				item.Add (content.Control);
				item.ShowAll ();
				dock.AddItem (item, DockPlacement.Top);*/
			
				/*IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
				//content.Control.Dock = DockStyle.None;
				Content newContent;
				if (content.Icon != null) {
					//ImageList imgList = new ImageList();
					//imgList.ColorDepth = ColorDepth.Depth32Bit;
					//IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
					//imgList.Images.Add(iconService.GetBitmap(content.Icon));
					//newContent = dockManager.Contents.Add(content.Control, content.Title, imgList, 0);
					//newContent = dockManager.Contents.Add(content.Control, content.Title, iconService.GetBitmap(content.Icon));
				} else {
					//newContent = dockManager.Contents.Add(content.Control, content.Title);
				}*/
				contentHash[content] = new Content (content.Control, content.Title, content.Icon);
			} else {
				Content c = (Content)contentHash[content];
				if (c != null) {
					DockItem item = dock.GetItemByName (content.ToString ());
					item.ShowItem ();
				}
			}
		}
		
		public bool IsVisible(IPadContent padContent)
		{
			if (padContent != null) {
				Content content = (Content)contentHash[padContent];
				if (content != null) {
					DockItem item = dock.GetItemByName (padContent.ToString ());
					if (item != null) {
						return item.IsAttached;
					}
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
					DockItem item = dock.GetItemByName (padContent.ToString ());
					item.HideItem ();
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

			//tabControl.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("MonoDevelop.Gui.TabVisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);

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
			string title = "";
			Gtk.Image mimeimage;
			if (content.IsUntitled) {
				title = content.UntitledName;
				mimeimage = new Gtk.Image (FileIconLoader.GetPixbufForType ("gnome-fs-regular").ScaleSimple (16, 16, Gdk.InterpType.Bilinear));
			} else {
				title = Path.GetFileName (content.ContentName);
				mimeimage = new Gtk.Image (FileIconLoader.GetPixbufForFile (content.ContentName, 16, 16));
			}
			
			TabLabel tabLabel = new TabLabel (new Label (), new Gtk.Image (""));
			tabLabel.Button.Clicked += new EventHandler (closeClicked);
			tabLabel.Button.StateChanged += new StateChangedHandler (stateChanged);
			SdiWorkspaceWindow sdiWorkspaceWindow = new SdiWorkspaceWindow(content, tabControl, tabLabel);

			sdiWorkspaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
			//sdiWorkspaceWindow.SwitchView(tabControl.Children.Length - 1);
			_windows.Add (sdiWorkspaceWindow);
		
			tabControl.AppendPage (sdiWorkspaceWindow, tabLabel);
		
			if (tabControl.NPages > 1)
				tabControl.ShowTabs = true;
			tabControl.ShowAll();
			return sdiWorkspaceWindow;
		}

		void stateChanged (object o, StateChangedArgs e)
		{
			if (((Gtk.Widget)o).State == Gtk.StateType.Active)
				((Gtk.Widget)o).State = Gtk.StateType.Normal;
		}

		void closeClicked (object o, EventArgs e)
		{
			int pageNum = -1;
			Widget parent = ((Widget)o).Parent;
			foreach (Widget child in tabControl.Children) {
				if (tabControl.GetTabLabel (child) == parent) {
					pageNum = tabControl.PageNum (child);
					break;
				}
			}
			if (pageNum != -1) {
				((SdiWorkspaceWindow)_windows [pageNum]).CloseWindow (false, false, pageNum);
			}
		}

		public void RemoveTab (int pageNum) {
			tabControl.RemovePage (pageNum);
			_windows.RemoveAt (pageNum);
			if (tabControl.NPages == 1)
				tabControl.ShowTabs = false;
		}
		
		void ActiveMdiChanged(object sender, SwitchPageArgs e)
		{
			try {
				if (ActiveWorkbenchwindow.ViewContent.IsUntitled) {
					((Gtk.Window)WorkbenchSingleton.Workbench).Title = "MonoDevelop";
				} else {
					((Gtk.Window)WorkbenchSingleton.Workbench).Title = ActiveWorkbenchwindow.ViewContent.ContentName + " - MonoDevelop";
				}
			} catch {
				((Gtk.Window)WorkbenchSingleton.Workbench).Title = "MonoDevelop";
			}
			/*if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}*/
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
