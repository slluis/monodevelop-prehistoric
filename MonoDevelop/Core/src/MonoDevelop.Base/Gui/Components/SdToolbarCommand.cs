// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;

//using Reflector.UserInterface;

namespace MonoDevelop.Gui.Components
{
	public class SdToolbarCommand : Gtk.Button, IStatusUpdate
	{
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		string localizedText = String.Empty;
		ICommand menuCommand = null;
		
		public ICommand Command {
			get {
				return menuCommand;
			}
			set {
				menuCommand = value;
				UpdateStatus();
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}

		public string Text {
			get {
				return localizedText;
			}
		}
		
		public SdToolbarCommand (string text) : base ()
		{
			localizedText = text;

			Clicked += new EventHandler (ToolbarClicked);
		}
		
		public SdToolbarCommand(ConditionCollection conditionCollection, object caller, string label) : this(label)
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			UpdateStatus();
		}
		
		public SdToolbarCommand(ConditionCollection conditionCollection, object caller, string label, ICommand menuCommand) : this(label)
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			this.menuCommand = menuCommand;
			UpdateStatus();
		}
		
		public SdToolbarCommand(ConditionCollection conditionCollection, object caller, string label, EventHandler handler) : this(label)
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			UpdateStatus();
		}
		
		public SdToolbarCommand(object caller, string label, EventHandler handler) : this(label)
		{
			this.caller = caller;
			UpdateStatus();
		}
		
		// To be called from ToolbarService
		public void ToolbarClicked(object o, EventArgs e) {
			if (menuCommand != null) {
				menuCommand.Run();
			}
		}

		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.Sensitive = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
			if (menuCommand != null && menuCommand is IMenuCommand) {
				Sensitive = ((IMenuCommand)menuCommand).IsEnabled;
			}
		}
	}
}
