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
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui.Components;

namespace MonoDevelop.Services
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
			contextMenu.Show();
			return contextMenu;
		}
		
		public void ShowContextMenu(object owner, string addInTreePath, Gtk.Widget parent)
		{
			//CreateContextMenu(owner, addInTreePath).Show(parent, new Point(x, y));
			CreateContextMenu(owner, addInTreePath).Popup(null, null, null, IntPtr.Zero, 0, Gtk.Global.CurrentEventTime);
		}
		
		class QuickInsertMenuHandler
		{
			Gtk.Editable targetControl;
			string      text;
			
			public QuickInsertMenuHandler(Gtk.Editable targetControl, string text)
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
				// insert at current cursor position, deleting any selections
				int tempInt = targetControl.Position;
				targetControl.DeleteSelection();
				targetControl.InsertText(text, ref tempInt);
			}
		}
		
		class QuickInsertHandler
		{
			Gtk.Button               popupControl;
			Gtk.Menu quickInsertMenu;
			
			//public QuickInsertHandler(Control popupControl, CommandBarContextMenu quickInsertMenu)
			public QuickInsertHandler(Gtk.Button popupControl, Gtk.Menu quickInsertMenu)
			{
				this.popupControl    = popupControl;
				this.quickInsertMenu = quickInsertMenu;
				
				popupControl.Clicked += new EventHandler(showQuickInsertMenu);
			}
			
			void showQuickInsertMenu(object sender, EventArgs e)
			{
				//Point cords = new Point(popupControl.Width, 0);
				//quickInsertMenu.Show(popupControl, cords);
				quickInsertMenu.Popup(null, null, null, IntPtr.Zero, 0, Gtk.Global.CurrentEventTime);
			}
		}
		
		//public void CreateQuickInsertMenu(TextBoxBase targetControl, Control popupControl, string[,] quickInsertMenuItems)		
		public void CreateQuickInsertMenu(Gtk.Editable targetControl, Gtk.Button popupControl, string[,] quickInsertMenuItems)
		{
			//CommandBarContextMenu contextMenu = new CommandBarContextMenu();
			Gtk.Menu contextMenu = new Gtk.Menu();
			for (int i = 0; i < quickInsertMenuItems.GetLength(0); ++i) {
				if (quickInsertMenuItems[i, 0] == "-") {
					contextMenu.Append(new SdMenuSeparator());
				} else {
					SdMenuCommand cmd = new SdMenuCommand(this,
					                                      Runtime.StringParserService.Parse (quickInsertMenuItems[i, 0]),
					                                      new QuickInsertMenuHandler(targetControl, quickInsertMenuItems[i, 1]).EventHandler);
					contextMenu.Append(cmd);
				}
			}
			new QuickInsertHandler(popupControl, contextMenu);
			
			contextMenu.ShowAll();
		}
	}
}
