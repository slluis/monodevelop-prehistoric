// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	class MenuCommandService : IMenuCommandService
	{
		IDesignerHost host;
		ArrayList     commands = new ArrayList();
		ArrayList     verbs    = new ArrayList();
		DesignPanel   panel;
		
		public DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbCollection = CreateDesignerVerbCollection();
				verbCollection.AddRange((DesignerVerb[])verbs.ToArray(typeof(DesignerVerb)));
				return verbCollection;
			}
		}
		
		public MenuCommandService(IDesignerHost host, DesignPanel panel)
		{
			this.panel = panel;
			this.host = host;
		}
		
		public void AddCommand(MenuCommand command)
		{
			Debug.Assert(command != null);
			Debug.Assert(command.CommandID != null);
			Debug.Assert(!commands.Contains(command));
			this.commands.Add(command);
		}
		
		public void AddVerb(DesignerVerb verb)
		{
			Debug.Assert(verb != null);
			this.verbs.Add(verb);
		}
		
		public void RemoveCommand(MenuCommand command)
		{
			Debug.Assert(command != null);
			commands.Remove(command.CommandID);
		}
		
		public void RemoveVerb(DesignerVerb verb)
		{
			Debug.Assert(verb != null);
			verbs.Remove(verb);
		}
		
		public bool GlobalInvoke(CommandID commandID)
		{
			MenuCommand menuCommand = FindCommand(commandID);
			if (menuCommand == null) {
				return false;
			}
			
			menuCommand.Invoke();
			return true;
		}
		
		public MenuCommand FindCommand(CommandID commandID)
		{
//			if (StringType.StrCmp(MenuUtilities.GetCommandNameFromCommandID(commandID), "", false) == 0 && StringType.StrCmp(commandID.ToString(), "74d21313-2aee-11d1-8bfb-00a0c90f26f7 : 12288", false) == 0) {
//				return MenuUtilities.gPropertyGridResetCommand;
//			}
			
			foreach (MenuCommand menuCommand in commands) {
				if (menuCommand.CommandID == commandID) {
					return menuCommand;
				}
			}
			
			foreach (DesignerVerb verb in Verbs) {
				if (verb.CommandID == commandID) {
					return verb;
				}
			}
			return null;
		}
		
		public void ShowContextMenu(CommandID menuID, int x, int y)
		{
			string contextMenuPath = "/SharpDevelop/FormsDesigner/ContextMenus/";
			
			if (menuID == MenuCommands.ComponentTrayMenu) {
				contextMenuPath += "ComponentTrayMenu";
			} else if (menuID == MenuCommands.ContainerMenu) {
				contextMenuPath += "ContainerMenu";
			} else if (menuID == MenuCommands.SelectionMenu) {
				contextMenuPath += "SelectionMenu";
			} else if (menuID == MenuCommands.TraySelectionMenu) {
				contextMenuPath += "TraySelectionMenu";
			} else {
				throw new Exception();
			}
			Point p = panel.PointToClient(new Point(x, y));
			
			MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
			menuService.ShowContextMenu(this, contextMenuPath, panel, p.X, p.Y);
		}
		
		public DesignerVerbCollection CreateDesignerVerbCollection()
		{
			DesignerVerbCollection designerVerbCollection = new DesignerVerbCollection();
			
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			if (selectionService != null && selectionService.SelectionCount == 1) {
				IComponent selectedComponent = selectionService.PrimarySelection as Component;
				if (selectedComponent != null) {
					IDesigner designer = host.GetDesigner((IComponent)selectedComponent);
					if (designer != null) {
						designerVerbCollection.AddRange(designer.Verbs);
					}
				}
				
				if (selectedComponent == host.RootComponent) {
					designerVerbCollection.AddRange((DesignerVerb[])this.verbs.ToArray(typeof(DesignerVerb)));
				}
			}
			return designerVerbCollection;
		}
	}
}
