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
using MonoDevelop.Gui.Widgets;
using MonoDevelop.Gui.Utils;

namespace MonoDevelop.Gui
{	
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		static string configFile = Runtime.Properties.ConfigDirectory + "DefaultEditingLayout.xml";

		// contains the fully qualified name of the current layout (ie. Edit.Default)
		string currentLayout = "";
		// list of layout names for the current context, without the context prefix
		ArrayList layouts = new ArrayList ();

		private IWorkbench workbench;

		// current workbench context
		WorkbenchContext workbenchContext;
		
		Window wbWindow;
		Container rootWidget;
		Dock dock;
		DockLayout dockLayout;
		DragNotebook tabControl;
		EventHandler contextChangedHandler;

		public SdiWorkbenchLayout () {
			contextChangedHandler = new EventHandler (OnContextChanged);
		}
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (tabControl == null || tabControl.CurrentPage < 0 || tabControl.CurrentPage >= tabControl.NPages)  {
					return null;
				}
				return (IWorkbenchWindow)workbench.ViewContentCollection[tabControl.CurrentPage].WorkbenchWindow;
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
				for (int i = 0; i < workbench.ToolBars.Length; i++) {
					Gtk.HandleBox toolHandleBox = new Gtk.HandleBox ();
					toolHandleBox.Shadow = Gtk.ShadowType.None;
					toolHandleBox.Add (workbench.ToolBars[i]);
					vbox.PackStart (toolHandleBox, false, false, 0);
				}
			}
			
			// Create the docking widget and add it to the window.
			dock = new Dock ();
			DockBar dockBar = new DockBar (dock);
			Gtk.HBox dockBox = new HBox (false, 5);
			dockBox.PackStart (dockBar, false, true, 0);
			dockBox.PackStart (dock, true, true, 0);
			vbox.PackStart (dockBox, true, true, 0);

			// Create the notebook for the various documents.
			tabControl = new DragNotebook ();
			tabControl.Scrollable = true;
			tabControl.SwitchPage += new SwitchPageHandler (ActiveMdiChanged);
			tabControl.TabsReordered += new TabsReorderedHandler (OnTabsReordered);
			DockItem item = new DockItem ("Documents", "Documents",
						      DockItemBehavior.Locked);
			item.PreferredWidth = -2;
			item.PreferredHeight = -2;
			item.Add (tabControl);
			item.ShowAll ();
			dock.AddItem (item, DockPlacement.Center);

			workbench.Add (vbox);
			
			vbox.PackEnd (Runtime.Gui.StatusBar.Control, false, true, 0);
			
			foreach (IViewContent content in workbench.ViewContentCollection)
				ShowView (content);

			// by default, the active pad collection is the full set
			// will be overriden in CreateDefaultLayout() below
			activePadCollection = workbench.PadContentCollection;

			// create DockItems for all the pads
			foreach (IPadContent content in workbench.PadContentCollection)
			{
				AddPad (content, DockPlacement.Left, false);
			}
			
			// FIXME: GTKize
			tabControl.SwitchPage += new SwitchPageHandler(ActiveMdiChanged);
			//tabControl.SelectionChanged += new EventHandler(ActiveMdiChanged);
			
			CreateDefaultLayout();
			//RedrawAllComponents();
			wbWindow.ShowAll ();

			workbench.ContextChanged += contextChangedHandler;
		}

		void OnTabsReordered (Widget widget, int oldPlacement, int newPlacement)
		{
			lock (workbench.ViewContentCollection) {
				IViewContent content = workbench.ViewContentCollection[oldPlacement];
				workbench.ViewContentCollection.RemoveAt (oldPlacement);
				workbench.ViewContentCollection.Insert (newPlacement, content);
				
			}
		}

		void OnContextChanged (object o, EventArgs e)
		{
			SwitchContext (workbench.Context);
			workbench.UpdateMenu (null, null);
		}

		void SwitchContext (WorkbenchContext ctxt)
		{
			PadContentCollection old = activePadCollection;
			
			// switch pad collections
			if (padCollections [ctxt] != null)
				activePadCollection = (PadContentCollection) padCollections [ctxt];
			else
				// this is so, for unkwown contexts, we get the full set of pads
				activePadCollection = workbench.PadContentCollection;

			workbenchContext = ctxt;
			
			// get the list of layouts
			string ctxtPrefix = ctxt.ToString () + ".";
			string[] list = dockLayout.GetLayouts (false);

			layouts.Clear ();
			foreach (string name in list) {
				if (name.StartsWith (ctxtPrefix)) {
					layouts.Add (name.Substring (ctxtPrefix.Length));
				}
			}
			
			// get the default layout for the new context from the property service
			CurrentLayout = Runtime.Properties.GetProperty
				("MonoDevelop.Gui.SdiWorkbenchLayout." + ctxt.ToString (), "Default");
			
			// make sure invalid pads for the new context are not visible
			foreach (IPadContent content in old)
			{
				if (!activePadCollection.Contains (content))
				{
					DockItem item = dock.GetItemByName (content.ToString ());
					if (item != null)
						item.HideItem ();
				}
			}
		}
		
		public Gtk.Widget LayoutWidget {
			get { return rootWidget; }
		}
		
		public string CurrentLayout {
			get {
				return currentLayout.Substring (currentLayout.IndexOf (".") + 1);
			}
			set {
				// save previous layout first
				if (currentLayout != "")
					dockLayout.SaveLayout (currentLayout);
				
				string newLayout = workbench.Context.ToString () + "." + value;

				if (layouts.Contains (value))
				{
					dockLayout.LoadLayout (newLayout);
				}
				else
				{
					if (currentLayout == "")
						// if the layout doesn't exists and we need to
						// load a layout (ie.  we've just been
						// created), load the default so old layout
						// xml files work smoothly
						dockLayout.LoadLayout (null);
					
					// the layout didn't exist, so save it and add it to our list
					dockLayout.SaveLayout (newLayout);
					layouts.Add (value);
				}
				currentLayout = newLayout;

				// persist the selected layout for the current context
				Runtime.Properties.SetProperty ("MonoDevelop.Gui.SdiWorkbenchLayout." +
				                             workbenchContext.ToString (), value);
			}
		}

		public string[] Layouts {
			get {
				string[] result = new string [layouts.Count];
				layouts.CopyTo (result);
				return result;
			}
		}

		// pad collection for the current workbench context
		PadContentCollection activePadCollection;

		// set of PadContentCollection objects for the different workbench contexts
		Hashtable padCollections = new Hashtable ();

		public PadContentCollection PadContentCollection {
			get {
				return activePadCollection;
			}
		}
		
		DockItem GetDockItem (IPadContent content)
		{
			if (activePadCollection.Contains (content))
			{
				DockItem item = dock.GetItemByName (content.ToString ());
				return item;
			}
			return null;
		}
		
		void CreateDefaultLayout()
		{
			string[] commonPads = new string[] {
				"MonoDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView",
				"MonoDevelop.Gui.Pads.ClassScout",
				"MonoDevelop.Gui.Pads.FileScout",
				"MonoDevelop.Gui.Pads.SideBarView",
				"MonoDevelop.Gui.Pads.PropertyPad",
				"MonoDevelop.Gui.Pads.OpenTaskView",
				"MonoDevelop.Gui.Pads.HelpTree",
//				"MonoDevelop.EditorBindings.Gui.Pads.CompilerMessageView",
				//"MonoDevelop.Gui.Pads.TerminalPad",
				"MonoDevelop.Gui.Pads.HelpBrowser",
				"MonoQuery.Pads.MonoQueryView"
			};

			string[] debugPads = new string[] {
				"MonoDevelop.SourceEditor.Gui.DebuggerLocalsPad",
				"MonoDevelop.SourceEditor.Gui.DebuggerStackTracePad",
				"MonoDevelop.SourceEditor.Gui.DebuggerThreadPad"
			};

			string[] editPads = new string[] {
			};

			PadContentCollection collection;
			
			foreach (WorkbenchContext ctxt in Enum.GetValues (typeof (WorkbenchContext)))
			{
				collection = new PadContentCollection ();
				padCollections [ctxt] = collection;
				foreach (string padTypeName in commonPads)
				{
					IPadContent pad = workbench.PadContentCollection [padTypeName];
					if (pad != null)
						collection.Add (pad);
				}
			}
			
			collection = (PadContentCollection) padCollections [WorkbenchContext.Edit];
			foreach (string padTypeName in editPads)
			{
				IPadContent pad = workbench.PadContentCollection [padTypeName];
				if (pad != null)
					collection.Add (pad);
			}
				
			collection = (PadContentCollection) padCollections [WorkbenchContext.Debug];
			foreach (string padTypeName in debugPads)
			{
				IPadContent pad = workbench.PadContentCollection [padTypeName];
				if (pad != null)
					collection.Add (pad);
			}
				
			//Console.WriteLine(" Default Layout created.");
			dockLayout = new DockLayout (dock);
			if (System.IO.File.Exists (configFile)) {
				dockLayout.LoadFromFile (configFile);
			} else {
				dockLayout.LoadFromFile ("../data/options/DefaultEditingLayout.xml");
			}
			
			SwitchContext (workbench.Context);
		}

		public void Detach()
		{
			workbench.ContextChanged -= contextChangedHandler;

			//Console.WriteLine("Call to SdiWorkSpaceLayout.Detach");
			dockLayout.SaveLayout (currentLayout);
			dockLayout.SaveToFile (configFile);
			rootWidget.Remove(((DefaultWorkbench)workbench).TopMenu);
			wbWindow.Remove(rootWidget);
			activePadCollection = null;
		}
		
		void AddPad (IPadContent content, DockPlacement placement, bool extraPad)
		{
			DockItem item = new DockItem (content.ToString (),
								 content.Title,
								 content.Icon,
								 DockItemBehavior.Normal);
								 
			Gtk.Label label = item.Tablabel as Gtk.Label;
			label.UseMarkup = true;

			item.Add (content.Control);
			item.ShowAll ();
			content.TitleChanged += new EventHandler (UpdatePad);
			content.IconChanged += new EventHandler (UpdatePad);
			
			if (extraPad) {
				DockItem ot = dock.GetItemByName ("MonoDevelop.Gui.Pads.OpenTaskView"); 
				if (ot != null && ot.IsAttached) {
					item.DockTo (ot, DockPlacement.Center, 0);
				}
				else {
					ot = dock.GetItemByName ("Documents"); 
					item.DockTo (ot, DockPlacement.Bottom, 0);
				}
			}
			else
				dock.AddItem (item, placement);

			if (!activePadCollection.Contains (content))
				activePadCollection.Add (content);
		}
		
		void UpdatePad (object source, EventArgs args)
		{
			IPadContent content = (IPadContent) source;
			DockItem item = GetDockItem (content);
			if (item != null) {
				Gtk.Label label = item.Tablabel as Gtk.Label;
				label.Markup = content.Title;
				item.LongName = content.Title;
				item.StockId = content.Icon;
			}
		}

		public void ShowPad (IPadContent content)
		{
			DockItem item = GetDockItem (content);
			if (item != null)
				item.ShowItem();
			else {
				AddPad (content, DockPlacement.Bottom, true);
			}
		}
		
		public bool IsVisible (IPadContent padContent)
		{
			DockItem item = GetDockItem (padContent);
			if (item != null)
				return item.IsAttached;
			return false;
		}
		
		public void HidePad (IPadContent padContent)
		{
			DockItem item = GetDockItem (padContent);
			if (item != null)
				item.HideItem();
		}
		
		public void ActivatePad (IPadContent padContent)
		{
			DockItem item = GetDockItem (padContent);
			if (item != null)
				item.Present (null);
		}
		
		public void RedrawAllComponents()
		{
			foreach (IPadContent content in ((IWorkbench)workbench).PadContentCollection) {
				DockItem item = dock.GetItemByName (content.ToString ());
				if (item != null)
					item.LongName = content.Title;
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
			Gtk.Image mimeimage = null;
			if (content.IsUntitled) {
				mimeimage = new Gtk.Image (FileIconLoader.GetPixbufForType ("gnome-fs-regular").ScaleSimple (16, 16, Gdk.InterpType.Bilinear));
			} else {
				mimeimage = new Gtk.Image (FileIconLoader.GetPixbufForFile (content.ContentName, 16, 16));
			}
			
			TabLabel tabLabel = new TabLabel (new Label (), mimeimage != null ? mimeimage : new Gtk.Image (""));
			tabLabel.Button.Clicked += new EventHandler (closeClicked);
			tabLabel.Button.StateChanged += new StateChangedHandler (stateChanged);
			tabLabel.ClearFlag (WidgetFlags.CanFocus);
			SdiWorkspaceWindow sdiWorkspaceWindow = new SdiWorkspaceWindow(content, tabControl, tabLabel);

			sdiWorkspaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
			tabControl.InsertPage (sdiWorkspaceWindow, tabLabel, -1);
		
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
				workbench.ViewContentCollection [pageNum].WorkbenchWindow.CloseWindow (false, false, pageNum);
			}
		}

		public void RemoveTab (int pageNum) {
			tabControl.RemovePage (pageNum);
		}
		
		public void ActiveMdiChanged(object sender, SwitchPageArgs e)
		{
			try {
				if (ActiveWorkbenchwindow.ViewContent.IsUntitled) {
					((Gtk.Window)WorkbenchSingleton.Workbench).Title = "MonoDevelop";
				} else {
					string post = String.Empty;
					if (ActiveWorkbenchwindow.ViewContent.IsDirty) {
						post = "*";
					}
					if (ActiveWorkbenchwindow.ViewContent.HasProject)
					{
						((Gtk.Window)WorkbenchSingleton.Workbench).Title = ActiveWorkbenchwindow.ViewContent.Project.Name + " - " + ActiveWorkbenchwindow.ViewContent.PathRelativeToProject + post + " - MonoDevelop";
					}
					else
					{
						((Gtk.Window)WorkbenchSingleton.Workbench).Title = ActiveWorkbenchwindow.ViewContent.ContentName + post + " - MonoDevelop";
					}
				}
			} catch {
				((Gtk.Window)WorkbenchSingleton.Workbench).Title = "MonoDevelop";
			}
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
