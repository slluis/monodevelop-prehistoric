// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Services
{
	public class MenuService : AbstractService
	{/*
		void ContextMenuPopupHandler(object sender, EventArgs e)
		{
			CommandBarContextMenu contextMenu = (CommandBarContextMenu)sender;
			foreach (object o in contextMenu.Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		*/
		public Gtk.Menu CreateContextMenu(object owner, string addInTreePath)
		{
			ArrayList buildItems = AddInTreeSingleton.AddInTree.GetTreeNode(addInTreePath).BuildChildItems(owner);
			//CommandBarContextMenu contextMenu = new CommandBarContextMenu();
			//contextMenu.Popup += new EventHandler(ContextMenuPopupHandler);
			
			Gtk.Menu contextMenu = new Gtk.Menu();
			foreach (object item in buildItems) {
				if (item is Gtk.MenuItem) {
					//contextMenu.Items.Add((CommandBarItem)item);
					contextMenu.Append((Gtk.MenuItem)item);
				} else {
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					foreach(Gtk.MenuItem mi in submenuBuilder.BuildSubmenu(null, owner)) {
						contextMenu.Append(mi);
					}
					//contextMenu.Items.AddRange(submenuBuilder.BuildSubmenu(null, owner));
				}
			}
			contextMenu.ShowAll();
			return contextMenu;
		}
		
		public void ShowContextMenu(object owner, string addInTreePath, Gtk.Widget parent)
		{
			//CreateContextMenu(owner, addInTreePath).Show(parent, new Point(x, y));
			CreateContextMenu(owner, addInTreePath).Popup(null, null, null, IntPtr.Zero, 3, Gtk.Global.CurrentEventTime);
		}
		/*
		class QuickInsertMenuHandler
		{
			TextBoxBase targetControl;
			string      text;
			
			public QuickInsertMenuHandler(TextBoxBase targetControl, string text)
			{
				this.targetControl = targetControl;
				this.text          = text;
			}
			
			public EventHandler EventHandler {
				get {
					return new EventHandler(PopupMenuHandler);
				}
			}
			void PopupMenuHandler(object sender, EventArgs e)
			{
				targetControl.SelectedText += text;
			}
		}
		
		class QuickInsertHandler
		{
			Control               popupControl;
			CommandBarContextMenu quickInsertMenu;
			
			public QuickInsertHandler(Control popupControl, CommandBarContextMenu quickInsertMenu)
			{
				this.popupControl    = popupControl;
				this.quickInsertMenu = quickInsertMenu;
				
				popupControl.Click += new EventHandler(showQuickInsertMenu);
			}
			
			void showQuickInsertMenu(object sender, EventArgs e)
			{
				Point cords = new Point(popupControl.Width, 0);
				quickInsertMenu.Show(popupControl, cords);
			}
		}
		
		public void CreateQuickInsertMenu(TextBoxBase targetControl, Control popupControl, string[,] quickInsertMenuItems)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			CommandBarContextMenu contextMenu = new CommandBarContextMenu();
			for (int i = 0; i < quickInsertMenuItems.GetLength(0); ++i) {
				if (quickInsertMenuItems[i, 0] == "-") {
					contextMenu.Items.Add(new SdMenuSeparator());
				} else {
					SdMenuCommand cmd = new SdMenuCommand(this,
					                                      stringParserService.Parse(quickInsertMenuItems[i, 0]),
					                                      new QuickInsertMenuHandler(targetControl, quickInsertMenuItems[i, 1]).EventHandler);
					contextMenu.Items.Add(cmd);
				}
			}
			new QuickInsertHandler(popupControl, contextMenu);
		}*/
	}
}
