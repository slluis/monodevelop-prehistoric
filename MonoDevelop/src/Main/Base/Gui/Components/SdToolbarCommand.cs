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

//using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdToolbarCommand : Gtk.Image, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		string localizedText = String.Empty;
		ICommand menuCommand = null;

		string key;

		public string Key {
			get { return key; }
			set { key = value; }
		}
		
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
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService (typeof(IResourceService));

			if (text.StartsWith ("${")) {
				localizedText = resourceService.GetString (text);
			} else {
				localizedText = text;
			}

			localizedText = localizedText.Replace ('&', '_');
		}
		
		public SdToolbarCommand(ConditionCollection conditionCollection, object caller, string label) : this(stringParserService.Parse(label))
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			UpdateStatus();
		}
		
		public SdToolbarCommand(ConditionCollection conditionCollection, object caller, string label, ICommand menuCommand) : this(stringParserService.Parse(label))
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			this.menuCommand = menuCommand;
			UpdateStatus();
		}
		
		public SdToolbarCommand(ConditionCollection conditionCollection, object caller, string label, EventHandler handler) : this(stringParserService.Parse(label))
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			UpdateStatus();
		}
		
		public SdToolbarCommand(object caller, string label, EventHandler handler) : this(stringParserService.Parse(label))
		{
			this.caller = caller;
			UpdateStatus();
		}
		
		// To be called from ToolbarService
		public void ToolbarClicked() {
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
