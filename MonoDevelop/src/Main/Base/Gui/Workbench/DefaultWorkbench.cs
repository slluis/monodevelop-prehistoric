// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml;

using MonoDevelop.Internal.Project;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Properties;

using MonoDevelop.Core.Services;
using MonoDevelop.Gui.Components;

using MonoDevelop.Services;

namespace MonoDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class DefaultWorkbench : Gtk.Window, IWorkbench
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Views";
		
		PadContentCollection viewContentCollection       = new PadContentCollection();
		ViewContentCollection workbenchContentCollection = new ViewContentCollection();
		
		bool closeAll = false;

		string cur_dbgFilename;
		int    cur_dbgLineNumber;
		
		bool            fullscreen;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		
		private IWorkbenchLayout layout = null;
		
		protected static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));

		public Gtk.MenuBar TopMenu = null;
		
		public bool FullScreen {
			get {
				return fullscreen;
			}
			set {
				fullscreen = value;
				if (fullscreen) {
					this.Fullscreen ();
				} else {
					this.Unfullscreen ();
				}
			}
		}
		
		/*
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
			}
		}*/
		
		EventHandler windowChangeEventHandler;
		
		public IWorkbenchLayout WorkbenchLayout {
			get {
				//FIXME: i added this, we need to fix this shit
				//				if (layout == null) {
				//	layout = new SdiWorkbenchLayout ();
				//	layout.Attach(this);
				//}
				return layout;
			}
			set {
				if (layout != null) {
					layout.ActiveWorkbenchWindowChanged -= windowChangeEventHandler;
					layout.Detach();
				}
				layout = value;
				layout.Attach(this);
				layout.ActiveWorkbenchWindowChanged += windowChangeEventHandler;
			}
		}
		
		public PadContentCollection PadContentCollection {
			get {
				Debug.Assert(viewContentCollection != null);
				return viewContentCollection;
			}
		}
		
		public ViewContentCollection ViewContentCollection {
			get {
				Debug.Assert(workbenchContentCollection != null);
				return workbenchContentCollection;
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				if (layout == null) {
					return null;
				}
				return layout.ActiveWorkbenchwindow;
			}
		}
		
		public DefaultWorkbench() : base ("MonoDevelop")
		{
			Console.WriteLine ("Creating DefaultWorkbench");
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			// FIXME: edit the name in the resource
			//Title = resourceService.GetString("MainWindow.DialogName");
		
			windowChangeEventHandler = new EventHandler(OnActiveWindowChanged);

			WidthRequest = normalBounds.Width;
			HeightRequest = normalBounds.Height;

			DeleteEvent += new Gtk.DeleteEventHandler (OnClosing);
			this.Icon = resourceService.GetBitmap ("Icons.SharpDevelopIcon");
			this.WindowPosition = Gtk.WindowPosition.None;

			IDebuggingService dbgr = (IDebuggingService)ServiceManager.Services.GetService (typeof (IDebuggingService));
			if (dbgr != null) {
				dbgr.StartedEvent += new EventHandler (onDebuggerStarted);
				dbgr.PausedEvent += new EventHandler (onDebuggerPaused);
				dbgr.ResumedEvent += new EventHandler (onDebuggerResumed);		
				dbgr.StoppedEvent += new EventHandler (onDebuggerStopped);
			}
		}

		void onDebuggerStarted (object o, EventArgs e)
		{
			context = WorkbenchContext.Debug;
			ContextChanged (this, new EventArgs());
		}
		
		void onDebuggerPaused (object o, EventArgs e)
		{
			IDebuggingService dbgr = (IDebuggingService)ServiceManager.Services.GetService (typeof (IDebuggingService));
			if (dbgr != null) {
				cur_dbgFilename = dbgr.CurrentFilename;
				cur_dbgLineNumber = dbgr.CurrentLineNumber - 1;

				IFileService fs = (IFileService)ServiceManager.Services.GetService (typeof (IFileService));
				fs.OpenFile (cur_dbgFilename);
				if (ActiveWorkbenchWindow.ViewContent is IDebuggableEditor) 
					((IDebuggableEditor)ActiveWorkbenchWindow.ViewContent).ExecutingAt (cur_dbgLineNumber);
			}
		}

		void onDebuggerResumed (object o, EventArgs e)
		{
			foreach (IViewContent content in ViewContentCollection) {
				if (content.ContentName != null && content.ContentName == cur_dbgFilename) {
					((IDebuggableEditor)content).ClearExecutingAt (cur_dbgLineNumber);
					break;
				}
			}	
		}

		void onDebuggerStopped (object o, EventArgs e)
		{
			foreach (IViewContent content in ViewContentCollection) {
				if (content.ContentName != null && content.ContentName == cur_dbgFilename) {
					((IDebuggableEditor)content).ClearExecutingAt (cur_dbgLineNumber);
					break;
				}
			}
			context = WorkbenchContext.Edit;
			ContextChanged (this, new EventArgs());
		}
		
		public void InitializeWorkspace()
		{
			// FIXME: GTKize
			ActiveWorkbenchWindowChanged += new EventHandler(UpdateMenu);
			
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService fileService = (IFileService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			
			projectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);
			projectService.CombineOpened         += new CombineEventHandler(CombineOpened);

			fileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
			fileService.FileRenamed += new FileEventHandler(CheckRenamedFile);
			
			fileService.FileRemoved += new FileEventHandler(fileService.RecentOpen.FileRemoved);
			fileService.FileRenamed += new FileEventHandler(fileService.RecentOpen.FileRenamed);
			
//			TopMenu.Selected   += new CommandHandler(OnTopMenuSelected);
//			TopMenu.Deselected += new CommandHandler(OnTopMenuDeselected);

			CreateToolBars();
			CreateMainMenu();
		}
				
		//		public void OpenCombine(string filename)
		//		{
		//			Debug.Assert(projectManager != null);
		//			projectManager.ClearCombine();
		//			CloseAllFiles();
		//			projectManager.OpenCombine(filename);
		//			UpdateMenu(null, null);
		//		}
		//
		//		public void SaveCombine()
		//		{
		//			Debug.Assert(projectManager != null);
		//			projectManager.SaveCombine();
		//		}
		//
		//		public void ClearCombine()
		//		{
		//			Debug.Assert(projectManager != null);
		//			projectManager.ClearCombine();
		//		}
		//
		//		public void MarkFileDirty(string filename)
		//		{
		//			Debug.Assert(projectManager != null);
		//			projectManager.MarkFileDirty(filename);
		//		}
		//
		//		public void OpenFile(string fileName)
		//		{
		//			Debug.Assert(fileManager != null);
		//			fileManager.OpenFile(fileName);
		//		}
		//
		//		public void NewFile(string defaultName, string language, string content)
		//		{
		//			Debug.Assert(fileManager != null);
		//			fileManager.NewFile(defaultName, language, content);
		//		}
		
		public void CloseContent(IViewContent content)
		{
			if (propertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				StoreMemento(content);
			}
			if (workbenchContentCollection.Contains(content)) {
				workbenchContentCollection.Remove(content);
			}
		}
		
		public void CloseAllViews()
		{
			try {
				closeAll = true;
				ViewContentCollection fullList = new ViewContentCollection(workbenchContentCollection);
				foreach (IViewContent content in fullList) {
					IWorkbenchWindow window = content.WorkbenchWindow;
					window.CloseWindow(false, true, 0);
				}
			} finally {
				closeAll = false;
				OnActiveWindowChanged(null, null);
			}
		}
		
		public virtual void ShowView(IViewContent content)
		{
			Debug.Assert(layout != null);
			ViewContentCollection.Add(content);
			if (propertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				try {
					IXmlConvertable memento = GetStoredMemento(content);
					if (memento != null) {
						((IMementoCapable)content).SetMemento(memento);
					}
				} catch (Exception e) {
					Console.WriteLine("Can't get/set memento : " + e.ToString());
				}
			}
			
			layout.ShowView(content);
			
			content.WorkbenchWindow.SelectWindow();
			ShowAll ();
			RedrawAllComponents ();
		}
		
		public virtual void ShowPad(IPadContent content)
		{
			PadContentCollection.Add(content);
			
			if (layout != null) {
				layout.ShowPad(content);
			}
		}
		
		public void RedrawAllComponents()
		{
			UpdateMenu(null, null);
			
			foreach (IViewContent content in workbenchContentCollection) {
				content.RedrawContent();
			}
			foreach (IPadContent content in viewContentCollection) {
				content.RedrawContent();
			}
			layout.RedrawAllComponents();
			//statusBarManager.RedrawStatusbar();
		}
		
		public IXmlConvertable GetStoredMemento(IViewContent content)
		{
			if (content != null && content.ContentName != null) {
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string directory = propertyService.ConfigDirectory + "temp";
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
				string fileName = content.ContentName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(System.IO.Path.DirectorySeparatorChar, '.');
				string fullFileName = directory + System.IO.Path.DirectorySeparatorChar + fileName;
				// check the file name length because it could be more than the maximum length of a file name
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (fileUtilityService.IsValidFileName(fullFileName) && File.Exists(fullFileName)) {
					IXmlConvertable prototype = ((IMementoCapable)content).CreateMemento();
					XmlDocument doc = new XmlDocument();
					doc.Load (File.OpenRead (fullFileName));
					
					return (IXmlConvertable)prototype.FromXmlElement((XmlElement)doc.DocumentElement.ChildNodes[0]);
				}
			}
			return null;
		}
		
		public void StoreMemento(IViewContent content)
		{
			if (content.ContentName == null) {
				return;
			}
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "temp";
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<?xml version=\"1.0\"?>\n<Mementoable/>");
			
			XmlAttribute fileAttribute = doc.CreateAttribute("file");
			fileAttribute.InnerText = content.ContentName;
			doc.DocumentElement.Attributes.Append(fileAttribute);
			
			
			IXmlConvertable memento = ((IMementoCapable)content).CreateMemento();
			
			doc.DocumentElement.AppendChild(memento.ToXmlElement(doc));
			
			string fileName = content.ContentName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(System.IO.Path.DirectorySeparatorChar, '.');
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			// check the file name length because it could be more than the maximum length of a file name
			string fullFileName = directory + System.IO.Path.DirectorySeparatorChar + fileName;
			if (fileUtilityService.IsValidFileName(fullFileName)) {
				fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fullFileName, FileErrorPolicy.ProvideAlternative);
			}
		}
		
		// interface IMementoCapable
		public IXmlConvertable CreateMemento()
		{
			WorkbenchMemento memento   = new WorkbenchMemento();
			int x, y, width, height;
			GdkWindow.GetSize (out width, out height);
			GdkWindow.GetRootOrigin (out x, out y);
			if (GdkWindow.State != Gdk.WindowState.Fullscreen && GdkWindow.State != Gdk.WindowState.Maximized) {
				memento.Bounds             = new Rectangle (x, y, width, height);
			} else {
				memento.Bounds = normalBounds;
			}

			memento.FullScreen         = fullscreen;
			return memento;
		}
		
		public void SetMemento(IXmlConvertable xmlMemento)
		{
			if (xmlMemento != null) {
				WorkbenchMemento memento = (WorkbenchMemento)xmlMemento;
				
				normalBounds = memento.Bounds;
				Move (normalBounds.X, normalBounds.Y);
				Resize (normalBounds.Width, normalBounds.Height);
				FullScreen  = memento.FullScreen;
			}
		}
		
		protected /*override*/ void OnResize(EventArgs e)
		{
			// FIXME: GTKize
			
		}
		
		protected /*override*/ void OnLocationChanged(EventArgs e)
		{
			// FIXME: GTKize
		}
		
		void CheckRemovedFile(object sender, FileEventArgs e)
		{
			if (e.IsDirectory) {
				foreach (IViewContent content in ViewContentCollection) {
					if (content.ContentName.StartsWith(e.FileName)) {
						content.WorkbenchWindow.CloseWindow(true, true, 0);
					}
				}
			} else {
				foreach (IViewContent content in ViewContentCollection) {
					if (content.ContentName != null &&
					    content.ContentName == e.FileName) {
						content.WorkbenchWindow.CloseWindow(true, true, 0);
						return;
					}
				}
			}
		}
		
		void CheckRenamedFile(object sender, FileEventArgs e)
		{
			if (e.IsDirectory) {
				foreach (IViewContent content in ViewContentCollection) {
					if (content.ContentName.StartsWith(e.SourceFile)) {
						content.ContentName = e.TargetFile + content.ContentName.Substring(e.SourceFile.Length);
					}
				}
			} else {
				foreach (IViewContent content in ViewContentCollection) {
					if (content.ContentName != null &&
					    content.ContentName == e.SourceFile) {
						content.ContentName = e.TargetFile;
						return;
					}
				}
			}
		}
		
//		protected void OnTopMenuSelected(MenuCommand mc)
//		{
//			IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
//			
//			statusBarService.SetMessage(mc.Description);
//		}
//		
//		protected void OnTopMenuDeselected(MenuCommand mc)
//		{
//			SetStandardStatusBar(null, null);
//		}
		
		protected /*override*/ void OnClosing(object o, Gtk.DeleteEventArgs e)
		{
			if (Close()) {
	                        propertyService.SetProperty("SharpDevelop.Workbench.WorkbenchMemento", WorkbenchSingleton.Workbench.CreateMemento());
				Gtk.Application.Quit ();
			} else {
				e.RetVal = true;
			}
		}
		
		protected /*override*/ void OnClosed(EventArgs e)
		{
			layout.Detach();
			foreach (IPadContent content in PadContentCollection) {
				content.Dispose();
			}
		}
		
		public bool Close() 
		{
			IProjectService projectService = (IProjectService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (projectService != null)
			{
				projectService.SaveCombinePreferences();
				while (WorkbenchSingleton.Workbench.ViewContentCollection.Count > 0) 
				{
					IViewContent content = WorkbenchSingleton.Workbench.ViewContentCollection[0];
					content.WorkbenchWindow.CloseWindow(false, true, 0);
					if (WorkbenchSingleton.Workbench.ViewContentCollection.Contains(content)) 
					{
						return false;
					}
				}
				projectService.CloseCombine(false);
			}
			
			// TODO : Dirty Files Dialog
			//			foreach (IViewContent content in ViewContentCollection) {
				//				if (content.IsDirty) {
					//					MonoDevelop.Gui.Dialogs.DirtyFilesDialog dfd = new MonoDevelop.Gui.Dialogs.DirtyFilesDialog();
			//					e.Cancel = dfd.ShowDialog() == DialogResult.Cancel;
			//					return;
			//				}
			//			}
			OnClosed (null);
			return true;
		}
		
		void CombineOpened(object sender, CombineEventArgs e)
		{
			UpdateMenu(null, null);			
		}
		
		void SetProjectTitle(object sender, ProjectEventArgs e)
		{
			UpdateMenu(null, null);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (e.Project != null) {
				Title = String.Concat(e.Project.Name, " - ", "MonoDevelop");
			} else {
				Title = "MonoDevelop";
			}
		}
		
		void SetStandardStatusBar(object sender, EventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage(GettextCatalog.GetString ("Ready"));
		}
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (!closeAll && ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}

		private Gtk.Toolbar[] toolbars = null;
		public Gtk.Toolbar[] ToolBars {
			get {
				return toolbars;
			}
			set {
				toolbars = value;
			}
		}
		
		public IPadContent GetPad(Type type)
		{
			foreach (IPadContent pad in PadContentCollection) {
				if (pad.GetType() == type) {
					return pad;
				}
			}
			return null;
		}
		void CreateMainMenu()
		{
			TopMenu = new Gtk.MenuBar ();
			object[] items = (object[])(AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath).BuildChildItems(this)).ToArray(typeof(object));
			foreach (object item in items) {
				TopMenu.Append ((Gtk.Widget)item);
			}
			UpdateMenu (null, null);
		}
		
		public void UpdateMenu(object sender, EventArgs e)
		{
			// update menu
			foreach (object o in TopMenu.Children) {
				if (o is SdMenu) {
					((SdMenu)o).OnDropDown(null, null);
				}
			}
			
			UpdateToolbars();
		}
		
		public void UpdateToolbars()
		{
			foreach (Gtk.Toolbar toolbar in ToolBars) {
				foreach (object item in toolbar.Children) {
					if (item is IStatusUpdate) {
						((IStatusUpdate)item).UpdateStatus();
					}
				}
			}
		}
		
		// this method simply copies over the enabled state of the toolbar,
		// this assumes that no item is inserted or removed.
		// TODO : make this method more add-in tree like, currently with Windows.Forms
		//        toolbars this is not possible. (drawing fragments, slow etc.)
		
		void CreateToolBars()
		{
			if (ToolBars == null) {
				ToolbarService toolBarService = (ToolbarService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(ToolbarService));
				Gtk.Toolbar[] toolBars = toolBarService.CreateToolbars();
				ToolBars = toolBars;
			}
		}
		
		public void UpdateViews(object sender, EventArgs e)
		{
			IPadContent[] contents = (IPadContent[])(AddInTreeSingleton.AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this)).ToArray(typeof(IPadContent));
			foreach (IPadContent content in contents) {
				ShowPad(content);
			}
		}
			// Handle keyboard shortcuts


		public event EventHandler ActiveWorkbenchWindowChanged;

		/// Context switching specific parts
		WorkbenchContext context = WorkbenchContext.Edit;
		
		public WorkbenchContext Context {
			get {
				return context;
			}
		}

		public event EventHandler ContextChanged;
	}
}

