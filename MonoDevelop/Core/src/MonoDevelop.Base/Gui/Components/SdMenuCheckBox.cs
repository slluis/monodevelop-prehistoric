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
using MonoDevelop.Services;

namespace MonoDevelop.Gui.Components
{
	public class SdMenuCheckBox : Gtk.CheckMenuItem, IStatusUpdate
	{
		static StringParserService stringParserService = Runtime.StringParserService;
			
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		string localizedText = String.Empty;
		ICheckableMenuCommand menuCommand;

		object _tag;

		public object Tag {
			get { return _tag; }
			set { _tag = value; }
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
	
		public SdMenuCheckBox (string label) : base ()
		{
			Toggled += new EventHandler (OnClick);
			Gtk.AccelLabel child = new Gtk.AccelLabel (label);
			child.Xalign = 0;
			child.UseUnderline = true;
			((Gtk.Container)this).Child = child;
			child.AccelWidget = this;
		}
	
		public SdMenuCheckBox(ConditionCollection conditionCollection, object caller, string label) : this(stringParserService.Parse(label))
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		public SdMenuCheckBox(ConditionCollection conditionCollection, object caller, string label, ICheckableMenuCommand menuCommand) : this(stringParserService.Parse(label))
		{
			this.menuCommand         = menuCommand;
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		protected void OnClick(object o, EventArgs e)
		{
			if (menuCommand != null) {
				menuCommand.IsChecked = Active;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.Sensitive = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}

			if (menuCommand != null) {
				Active = menuCommand.IsChecked;
			}
		}
	}
}
