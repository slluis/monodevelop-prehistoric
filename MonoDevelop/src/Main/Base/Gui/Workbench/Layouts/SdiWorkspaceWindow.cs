// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Threading;
using Gtk;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SdiWorkspaceWindow : IWorkbenchWindow
	{
		Notebook   viewTabControl = null;
		IViewContent content;
		ArrayList    subViewContents = null;
		
		Label     tabLabel;
		Widget    tabPage;
		Notebook tabControl;
		
		string myUntitledTitle     = null;
		string _titleHolder = "";
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public Widget TabPage {
			get {
				return tabPage;
			}
			set {
				tabPage = value;
#if GTK
				// FIXME: GTKize
#else
				content.Control.Dock = DockStyle.Fill;
				if (subViewContents == null) {
					tabPage.Controls.Add(content.Control);
				} else {
					tabPage.Controls.Add(viewTabControl);
				}
#endif
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
#if GTK
			// FIXME: GTKize
#else
			tabPage.Selected = true;
// KSL, Start to fix the focus problem when changing tabs
			content.Control.Focus();
// KSL End
#endif

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
			int toSelect = tabControl.PageNum (tabPage);
			tabControl.CurrentPage = toSelect;
		}
		
		public SdiWorkspaceWindow(IViewContent content, Notebook tabControl, Label label)
		{
			this.tabControl = tabControl;
			this.content = content;
			this.tabLabel = label;
			this.tabPage = content.Control;
			
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			content.WorkbenchWindow = this;

#if GTK
			// FIXME: GTKize
#else
			tabPage.Tag = this;
			tabPage.ImageList = iconService.ImageList;
			tabPage.TabIndexChanged += new EventHandler(LeaveTabPage);
#endif
			
			content.ContentNameChanged += new EventHandler(SetTitleEvent);
			content.DirtyChanged       += new EventHandler(SetTitleEvent);
			content.BeforeSave         += new EventHandler(BeforeSave);
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
					string baseName  = Path.GetFileNameWithoutExtension(content.UntitledName);
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
				newTitle = Path.GetFileName(content.ContentName);
			}
			
			if (content.IsDirty) {
				newTitle += "*";
			} else if (content.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Title = newTitle;
			}
		}
		
		public void DetachContent()
		{
#if !GTK
			tabPage.Control = null;
#endif
			content.ContentNameChanged -= new EventHandler(SetTitleEvent);
			content.DirtyChanged       -= new EventHandler(SetTitleEvent);
			content.BeforeSave         -= new EventHandler(BeforeSave);
		}
		
		public void CloseWindow(bool force, bool fromMenu, int pageNum)
		{
			if (!force && ViewContent != null && ViewContent.IsDirty) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				MessageDialog md = new MessageDialog ((Window) WorkbenchSingleton.Workbench, DialogFlags.Modal | DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, resourceService.GetString ("MainWindow.SaveChangesMessage"));
				int response = md.Run ();
				md.Hide ();
				md.Dispose ();
				
				switch ((ResponseType) response) {
					case ResponseType.Yes:
						if (content.ContentName == null) {
							while (true) {
								new ICSharpCode.SharpDevelop.Commands.SaveFileAs().Run();
								if (ViewContent.IsDirty) {
									IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
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
					case ResponseType.No:
						break;
						
					default:
						// considering this to be Cancel
						return;
				}
			}
			//tabControl.RemovePage (tabControl.CurrentPage);
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
#if GTK
			// FIXME: GTKize
#else
			TabPage newPage;
			
			if (subViewContents == null) {
				subViewContents = new ArrayList();
				
				viewTabControl      = new TabControl();
				viewTabControl.Alignment = TabAlignment.Bottom;
				viewTabControl.Dock = DockStyle.Fill;
				viewTabControl.SelectedIndexChanged += new EventHandler(viewTabControlIndexChanged);
				
				tabPage.Controls.Clear();
				tabPage.Controls.Add(viewTabControl);
				
				newPage = new TabPage(stringParserService.Parse(content.TabPageText));
				newPage.Tag = content;
				content.Control.Dock = DockStyle.Fill;
				newPage.Controls.Add(content.Control);
				viewTabControl.TabPages.Add(newPage);
			}
			subViewContent.WorkbenchWindow = this;
			subViewContents.Add(subViewContent);
			
			newPage = new TabPage(stringParserService.Parse(subViewContent.TabPageText));
			newPage.Tag = subViewContent;
			subViewContent.Control.Dock = DockStyle.Fill;
			newPage.Controls.Add(subViewContent.Control);
			viewTabControl.TabPages.Add(newPage);
#endif
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
			tabLabel.Text = Title;
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
