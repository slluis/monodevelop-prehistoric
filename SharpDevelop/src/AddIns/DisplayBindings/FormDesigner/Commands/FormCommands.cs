// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.ComponentModel.Design;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns.Conditions;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

using ICSharpCode.SharpDevelop.FormDesigner;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.FormEditor.Commands
{
	/// <summary>
	/// This is the base class for all designer menu commands
	/// </summary>
	public abstract class AbstractFormDesignerCommand : AbstractMenuCommand
	{
		public abstract CommandID CommandID {
			get;
		}
		
		protected virtual bool CanExecuteCommand(IDesignerHost host)
		{
			return true;
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			try {
				FormDesignerDisplayBindingBase formDesigner = window.ActiveViewContent as FormDesignerDisplayBindingBase;
				
				if (formDesigner != null && CanExecuteCommand(formDesigner.DesignerHost)) {
					IMenuCommandService menuCommandService = (IMenuCommandService)formDesigner.DesignerHost.GetService(typeof(IMenuCommandService));
					menuCommandService.GlobalInvoke(CommandID);
				}
			} catch (Exception e) {
				Console.WriteLine("Got Exception {0}", e);
			}
		}
	}
	
	public class ViewCode : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return;
			}
			
			FormDesignerDisplayBindingBase formDesigner = window.ActiveViewContent as FormDesignerDisplayBindingBase;
			
			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
				
			}
		}
	}
	
	public class DesignerVerbSubmenuBuilder : ISubmenuBuilder
	{
		public CommandBarItem[] BuildSubmenu(ConditionCollection conditionCollection, object owner)
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)owner;
			
			ArrayList items = new ArrayList();
				
			foreach (DesignerVerb verb in menuCommandService.Verbs) {
				items.Add(new ContextMenuCommand(verb));
			}
			
			// add separator at the end of custom designer verbs
			if (items.Count > 0) {
				items.Add(new SdMenuSeparator());
			}
			
			return (CommandBarItem[])items.ToArray(typeof(CommandBarItem));
		}
		
		class ContextMenuCommand : SdMenuCommand
		{
			DesignerVerb verb;
			
			public ContextMenuCommand(DesignerVerb verb) : base(null, null, verb.Text)
			{
				this.IsEnabled = verb.Enabled;
//				this.Checked = verb.Checked;
				
				this.verb = verb;
				Click += new EventHandler(InvokeCommand);
			}
			
			void InvokeCommand(object sender, EventArgs e)
			{
				try {
					verb.Invoke();
				} catch (Exception ex) {
					Console.WriteLine("Got Exception {0}", ex);
				}
			}
		}
	}
	
#region Align Commands	
	public class AlignToGrid : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignToGrid;
			}
		}
	}
	
	public class AlignLeft : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignLeft;
			}
		}
	}
	
	public class AlignRight : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignRight;
			}
		}
	}
	
	public class AlignTop : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignTop;
			}
		}
	}
	
	public class AlignBottom : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignBottom;
			}
		}
	}
	
	public class AlignHorizontalCenters : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignHorizontalCenters;
			}
		}
	}
	
	public class AlignVerticalCenters : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.AlignVerticalCenters;
			}
		}
	}
#endregion

#region Make Same Size Commands
	public class SizeToGrid : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToGrid;
			}
		}
	}
	
	public class SizeToControl : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControl;
			}
		}
	}
	
	public class SizeToControlHeight : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlHeight;
			}
		}
	}
	
	public class SizeToControlWidth : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SizeToControlWidth;
			}
		}
	}
#endregion

#region Horizontal Spacing Commands	
	public class HorizSpaceMakeEqual : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceMakeEqual;
			}
		}
		
		protected override bool CanExecuteCommand(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			return selectionService.SelectionCount > 1;
		}
	}
	
	public class HorizSpaceIncrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceIncrease;
			}
		}
	}
	
	public class HorizSpaceDecrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceDecrease;
			}
		}
	}
	
	public class HorizSpaceConcatenate : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.HorizSpaceConcatenate;
			}
		}
	}
#endregion
	
#region Vertical Spacing Commands
	public class VertSpaceMakeEqual : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceMakeEqual;
			}
		}
		
		protected override bool CanExecuteCommand(IDesignerHost host)
		{
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			return selectionService.SelectionCount > 1;
		}
		
	}
	
	public class VertSpaceIncrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceIncrease;
			}
		}
	}
	
	public class VertSpaceDecrease : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceDecrease;
			}
		}
	}
	
	public class VertSpaceConcatenate : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.VertSpaceConcatenate;
			}
		}
	}
#endregion

#region Center Commands	
	public class CenterHorizontally : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterHorizontally;
			}
		}
	}
	public class CenterVertically : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.CenterVertically;
			}
		}
	}
#endregion
	
#region Order Commands
	public class SendToBack : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.SendToBack;
			}
		}
	}
	
	public class BringToFront : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.BringToFront;
			}
		}
	}
#endregion

#region Tray Commands	
	
	public class LineUpIcons : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LineupIcons;
			}
		}
	}
	
	public class ShowLargeIcons : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.ShowLargeIcons;
			}
		}
	}
#endregion

#region Global Commands	
	public class LockControls : AbstractFormDesignerCommand
	{
		public override CommandID CommandID {
			get {
				return StandardCommands.LockControls;
			}
		}
	}
#endregion

}
