// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;

using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Components
{
	public class SdToolbarCommand : Gtk.ToolButton, IStatusUpdate
	{
		static Tooltips tips = new Tooltips ();
		object caller;
		ConditionCollection conditionCollection;
		string localizedText = String.Empty;
		ICommand menuCommand = null;
		
		public ICommand Command {
			get { return menuCommand; }
			set {
				menuCommand = value;
				UpdateStatus();
			}
		}
		
		public string Text {
			get { return localizedText; }
		}
		
		public SdToolbarCommand (ToolbarItemCodon codon, object caller) : base (null, "")
		{
			this.caller = caller;
			this.IconWidget = Runtime.Gui.Resources.GetImage (codon.Icon, Gtk.IconSize.LargeToolbar);
			this.conditionCollection = codon.Conditions;
			this.localizedText = codon.ToolTip;
			this.Label = this.Text;
			if (codon.Class != null)
				this.Command = (ICommand) codon.AddIn.CreateObject (codon.Class);

			this.SetTooltip (tips, Text, Text);
			this.Clicked += new EventHandler (ToolbarClicked);
			this.ShowAll ();
		}
		
		void ToolbarClicked (object o, EventArgs e) {
			if (menuCommand != null)
				menuCommand.Run ();
		}

		public virtual void UpdateStatus ()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction (caller);
				this.Sensitive = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
			if (menuCommand != null && menuCommand is IMenuCommand)
				this.Sensitive = ((IMenuCommand) menuCommand).IsEnabled;
		}
	}
}
