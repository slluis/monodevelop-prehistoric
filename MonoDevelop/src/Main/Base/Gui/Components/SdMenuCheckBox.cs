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
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdMenuCheckBox : Gtk.CheckMenuItem, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
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
	
		public SdMenuCheckBox (string label) : base (label)
		{
			Toggled += new EventHandler (OnClick);
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
#if !GTK
			base.OnClick(e);
#endif
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
#if GTK
			// FIXME: GTKize
#else
			Text = stringParserService.Parse(localizedText);
#endif
			if (menuCommand != null) {
				Active = menuCommand.IsChecked;
			}
		}
	}
}
