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
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml;
using System.ComponentModel.Design.Serialization;
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
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using ICSharpCode.SharpDevelop.Gui.Pads;

using ICSharpCode.SharpDevelop.Gui.ErrorDialogs;
using ICSharpCode.SharpDevelop.FormDesigner.Gui;

using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;

namespace ICSharpCode.SharpDevelop.FormDesigner {
	
	public class UndoHandler
	{
		IDesignerHost host;
		Hashtable     sizePos = new Hashtable();
		
		ICSharpCode.SharpDevelop.Internal.Undo.UndoStack undoStack = new ICSharpCode.SharpDevelop.Internal.Undo.UndoStack();
		
		bool  inUndoRedo     = false;
		int   transactionLevel = 0;
		int   undoOperations   = 0;
		
		public void Reset()
		{
			undoStack.ClearAll();
		}
		
		// BUG ALERT !!!
		// WINDOWS FORMS DESIGNER BUG: 
		// MOVE/RESIZE IN THE DESIGNER --> OldValue == NewValue in change event
		// THEREFORE A WORKAROUND IS NEEDED (AN UGLY WORKAROUND)
		// BUG ALERT 2 !!!
		// SOME MS CODERS DON'T KNOW THE DIFFERENCE BETWEEN REFERENCE AND VALUE !!!
		// CONTROL COLLECTION new/old values are the same !!!!
		void InitSizePosTable()
		{
			sizePos.Clear();
			foreach (IComponent component in host.Container.Components) {
				Control ctrl = component as Control;
				if (ctrl != null) {
					object[] ctrlCol = new object[ctrl.Controls.Count];
					ctrl.Controls.CopyTo(ctrlCol, 0);
					sizePos[component.Site.Name] = new object[] { ctrl.Location, ctrl.Size, ctrlCol};
				}
			}
		}
		
		void ComponentChanged(object sender, ComponentChangedEventArgs ea)
		{
			if (inUndoRedo) {
				return;
			}
			++undoOperations;
			if (sizePos[((IComponent)ea.Component).Site.Name] != null) {
				if (ea.Member.Name == "Location") {
					ea = new ComponentChangedEventArgs(ea.Component, ea.Member, ((object[])sizePos[((IComponent)ea.Component).Site.Name])[0], ea.NewValue);
				} else if (ea.Member.Name == "Size") {
					ea = new ComponentChangedEventArgs(ea.Component, ea.Member, ((object[])sizePos[((IComponent)ea.Component).Site.Name])[1], ea.NewValue);
				} else if (ea.Member.Name == "Controls") {
					ea = new ComponentChangedEventArgs(ea.Component, ea.Member, ((object[])sizePos[((IComponent)ea.Component).Site.Name])[2], ea.NewValue);
				}
			}
			Console.WriteLine("GOT COMPONENT CHANGE EVENT ");
			undoStack.Push(new ComponentChangedUndoAction(host, ea));
		}
		
		void ComponentAdded(object sender, ComponentEventArgs ea)
		{
			if (inUndoRedo) {
				return;
			}
			++undoOperations;
			undoStack.Push(new ComponentAddedUndoAction(host, ea));
		}
		
		void ComponentRemoved(object sender, ComponentEventArgs ea)
		{
			if (inUndoRedo) {
				return;
			}
			++undoOperations;
			string parentName = null;
			foreach (DictionaryEntry entry in sizePos) {
				object[] arr = (object[])entry.Value;
				if (arr[2] != null) {
					foreach (object ctr in ((IList)arr[2])) {
						if (ctr == ea.Component) {
							parentName = entry.Key.ToString();
						}
					}
				}
			}
			undoStack.Push(new ComponentRemovedUndoAction(host, ea, parentName));
		}
		SelectComponentsUndoAction selectComponentsUndoAction = null;
		
		void TransactionOpened(object sender, EventArgs e)
		{
			if (transactionLevel == 0) {
				undoOperations = 0;
				InitSizePosTable();
				selectComponentsUndoAction = new SelectComponentsUndoAction(host, GetSelectedComponentNames(host));
				undoStack.Push(selectComponentsUndoAction);
			}
			++transactionLevel;
		}
		
		void TransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
		{
			--transactionLevel;
			if (transactionLevel == 0 && undoOperations > 0) {
				if (selectComponentsUndoAction != null) {
					selectComponentsUndoAction.SetNewSelection(GetSelectedComponentNames(host));
					selectComponentsUndoAction = null;
				}
				undoStack.UndoLast(undoOperations + 1);
			}
		}
		
		public static ArrayList GetSelectedComponentNames(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			ArrayList names = new ArrayList();
			
			foreach (IComponent component in selectionService.GetSelectedComponents()) {
				if (component.Site != null) {
					names.Add(component.Site.Name);
				} else {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(component + " has no site.");
				}
			}
			return names;
		}
		
		public static void SetSelectedComponentsPerName(IDesignerHost host, ArrayList names)
		{
			ArrayList components = new ArrayList();
			
			foreach (string name in names) {
				if (host.Container.Components[name] != null) {
					components.Add(host.Container.Components[name]);
				} else {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError("Can't select component : Component " + name + " not found.");
				}
			}
			
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			selectionService.SetSelectedComponents(components);
		}

		
		public void Attach(IDesignerHost host)
		{
			this.host = host;
			
			IComponentChangeService componentChangeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += new ComponentChangedEventHandler(ComponentChanged);
			componentChangeService.ComponentAdded   += new ComponentEventHandler(ComponentAdded);
			componentChangeService.ComponentRemoved += new ComponentEventHandler(ComponentRemoved);
			
			host.TransactionOpened += new EventHandler(TransactionOpened);
			host.TransactionClosed += new DesignerTransactionCloseEventHandler(TransactionClosed);
		}
		
		public void Detach()
		{
			IComponentChangeService componentChangeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged -= new ComponentChangedEventHandler(ComponentChanged);
			componentChangeService.ComponentAdded   -= new ComponentEventHandler(ComponentAdded);
			componentChangeService.ComponentRemoved -= new ComponentEventHandler(ComponentRemoved);
			
			host.TransactionOpened -= new EventHandler(TransactionOpened);
			host.TransactionClosed -= new DesignerTransactionCloseEventHandler(TransactionClosed);
		}
		
		public void Undo()
		{
			inUndoRedo = true;
			try {
				undoStack.Undo();
			} catch (Exception e) {
				Console.WriteLine("UndoException : " + e.ToString());
			} finally {
				inUndoRedo = false;
			}
		}
		
		public void Redo()
		{
			inUndoRedo = true;
			try {
				undoStack.Redo();
			} catch (Exception e) {
				Console.WriteLine("UndoException : " + e.ToString());
			} finally {
				inUndoRedo = false;
			}
		}
	}
}
