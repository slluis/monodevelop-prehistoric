// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using Gtk;

using Gdl;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using MonoDevelop.Gui.Utils;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui
{
	public class SdiWorkspaceWindow : Dock, IWorkbenchWindow
	{
		Notebook   viewTabControl = null;
		IViewContent content;
		ArrayList    subViewContents = null;

		DockItem mainItem;
		ArrayList    subDockItems = null;
		
		TabLabel tabLabel;
		Widget    tabPage;
		Notebook  tabControl;
		
		string myUntitledTitle     = null;
		string _titleHolder = "";
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public Widget TabPage {
			get {
				return tabPage;
			}
			set {
				tabPage = value;
			}
		}
		
		public string Title {
			get {
				//FIXME: This breaks, Why? --Todd
				//_titleHolder = tabControl.GetTabLabelText (tabPage);
				return _titleHolder;
			}
			set {
				_titleHolder = value;
				string fileName = content.ContentName;
				if (fileName == null) {
					fileName = content.UntitledName;
				}
				
//				if (fileName != null) {
//					IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
//					tabPage.ImageIndex = iconService.GetImageIndexForFile(fileName);
//				}
				OnTitleChanged(null);
			}
		}
		
		public ArrayList SubViewContents {
			get {
				return subViewContents;
			}
		}
		
		public IBaseViewContent ActiveViewContent {
			get {
				if (viewTabControl != null && viewTabControl.CurrentPage > 0) {
					return (IBaseViewContent)subViewContents[viewTabControl.CurrentPage - 1];
				}
				return content;
			}
		}
		
		void ThreadSafeSelectWindow()
		{
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (viewContent != this.content) {
					viewContent.WorkbenchWindow.OnWindowDeselected(EventArgs.Empty);
				}
			}
			OnWindowSelected(EventArgs.Empty);
		}
		
		public void SwitchView(int viewNumber)
		{
			if (viewTabControl != null) {
				this.viewTabControl.CurrentPage = viewNumber;
			}
		}
		
		public void SelectWindow()	
		{
			int toSelect = tabControl.PageNum (this);
			tabControl.CurrentPage = toSelect;
		}
		
		public SdiWorkspaceWindow(IViewContent content, Notebook tabControl, TabLabel tabLabel) : base ()
		{
			this.tabControl = tabControl;
			this.content = content;
			this.tabLabel = tabLabel;
			this.tabPage = content.Control;
			
			content.WorkbenchWindow = this;
			
			content.ContentNameChanged += new EventHandler(SetTitleEvent);
			content.DirtyChanged       += new EventHandler(SetTitleEvent);
			content.BeforeSave         += new EventHandler(BeforeSave);
			content.ContentChanged     += new EventHandler (OnContentChanged);

			mainItem = new DockItem (content.TabPageLabel, content.TabPageLabel, DockItemBehavior.Locked | DockItemBehavior.CantClose | DockItemBehavior.CantIconify);
			mainItem.Add (content.Control);
			mainItem.ShowAll ();
			AddItem (mainItem, DockPlacement.Center);
			SetTitleEvent(null, null);
		}
		
		void BeforeSave(object sender, EventArgs e)
		{
			ISecondaryViewContent secondaryViewContent = ActiveViewContent as ISecondaryViewContent;
			if (secondaryViewContent != null) {
				secondaryViewContent.NotifyBeforeSave();
			}
		}
		
		void LeaveTabPage(object sender, EventArgs e)
		{
			OnWindowDeselected(EventArgs.Empty);
		}
		
		public IViewContent ViewContent {
			get {
				return content;
			}
			set {
				content = value;
			}
		}
		
		public void SetTitleEvent(object sender, EventArgs e)
		{
			if (content == null) {
				return;
			}
		
			string newTitle = "";
			if (content.ContentName == null) {
				if (myUntitledTitle == null) {
					string baseName  = System.IO.Path.GetFileNameWithoutExtension(content.UntitledName);
					int    number    = 1;
					bool   found     = true;
					while (found) {
						found = false;
						foreach (IViewContent windowContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
							string title = windowContent.WorkbenchWindow.Title;
							if (title.EndsWith("*") || title.EndsWith("+")) {
								title = title.Substring(0, title.Length - 1);
							}
							if (title == baseName + number) {
								found = true;
								++number;
								break;
							}
						}
					}
					myUntitledTitle = baseName + number;
				}
				newTitle = myUntitledTitle;
			} else {
				newTitle = System.IO.Path.GetFileName(content.ContentName);
			}
			
			if (content.IsDirty) {
				newTitle += "*";
				IProjectService prj = (IProjectService)ServiceManager.Services.GetService (typeof (IProjectService));
				prj.MarkFileDirty (content.ContentName);
			} else if (content.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Title = newTitle;
			}
		}
		
		public void DetachContent()
		{
			content.ContentNameChanged -= new EventHandler(SetTitleEvent);
			content.DirtyChanged       -= new EventHandler(SetTitleEvent);
			content.BeforeSave         -= new EventHandler(BeforeSave);
			content.ContentChanged     -= new EventHandler (OnContentChanged);
		}

		public void OnContentChanged (object o, EventArgs e)
		{
			if (subViewContents != null) {
				foreach (ISecondaryViewContent subContent in subViewContents)
				{
					subContent.BaseContentChanged ();
				}
			}
		}
		
		public void CloseWindow(bool force, bool fromMenu, int pageNum)
		{
			if (!force && ViewContent != null && ViewContent.IsDirty) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				bool response = messageService.AskQuestion (resourceService.GetString ("MainWindow.SaveChangesMessage"));
				
				switch (response) {
					case true:
						if (content.ContentName == null) {
							while (true) {
								new MonoDevelop.Commands.SaveFileAs().Run();
								if (ViewContent.IsDirty) {
									if (messageService.AskQuestion("Do you really want to discard your changes ?")) {
										break;
									}
								} else {
									break;
								}
							}
							
						} else {
							FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
							fileUtilityService.ObservedSave(new FileOperationDelegate(ViewContent.Save), ViewContent.ContentName , FileErrorPolicy.ProvideAlternative);
						}
						break;
					case false:
						break;
						
					default:
						// considering this to be Cancel
						return;
				}
			}
			if (fromMenu == true) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.RemoveTab (tabControl.CurrentPage);
			} else {
				WorkbenchSingleton.Workbench.WorkbenchLayout.RemoveTab (pageNum);
			}
			OnWindowDeselected(EventArgs.Empty);
			OnCloseEvent(null);
		}
		
		public void AttachSecondaryViewContent(ISecondaryViewContent subViewContent)
		{
			if (subViewContents == null) {
				subViewContents = new ArrayList ();
				subDockItems = new ArrayList ();
			}
	
			mainItem.Behavior = DockItemBehavior.CantClose | DockItemBehavior.CantIconify;
			subViewContents.Add (subViewContent);
			DockItem dockitem = new DockItem (subViewContent.TabPageLabel, subViewContent.TabPageLabel, DockItemBehavior.CantClose | DockItemBehavior.CantIconify);
			dockitem.Add (subViewContent.Control);
			subViewContent.Control.ShowAll ();
			dockitem.ShowAll ();
			subDockItems.Add (dockitem);
			AddItem (dockitem, DockPlacement.Bottom);
			OnContentChanged (null, null);
		}
		
		int oldIndex = -1;
		void viewTabControlIndexChanged(object sender, EventArgs e)
		{
			if (oldIndex > 0) {
				ISecondaryViewContent secondaryViewContent = subViewContents[oldIndex - 1] as ISecondaryViewContent;
				if (secondaryViewContent != null) {
					secondaryViewContent.Deselected();
				}
			}
			
			if (viewTabControl.CurrentPage > 0) {
				ISecondaryViewContent secondaryViewContent = subViewContents[viewTabControl.CurrentPage - 1] as ISecondaryViewContent;
				if (secondaryViewContent != null) {
					secondaryViewContent.Selected();
				}
			}
			oldIndex = viewTabControl.CurrentPage;
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			tabLabel.Label.Text = Title;
			try {
				if (content.ContentName.IndexOfAny (new char[] { '*', '+'}) == -1) {
					tabLabel.Icon.Pixbuf = FileIconLoader.GetPixbufForFile (content.ContentName, 16, 16);
				}
			} catch {
				tabLabel.Icon.Pixbuf = FileIconLoader.GetPixbufForType ("gnome-fs-regular").ScaleSimple (16, 16, Gdk.InterpType.Bilinear);
			}
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnCloseEvent(EventArgs e)
		{
			OnWindowDeselected(e);
			if (CloseEvent != null) {
				CloseEvent(this, e);
			}
		}

		public virtual void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) {
				WindowSelected(this, e);
			}
		}
		public virtual void OnWindowDeselected(EventArgs e)
		{
			if (WindowDeselected != null) {
				WindowDeselected(this, e);
			}
		}
		
		public event EventHandler WindowSelected;
		public event EventHandler WindowDeselected;
				
		public event EventHandler TitleChanged;
		public event EventHandler CloseEvent;
	}
}
