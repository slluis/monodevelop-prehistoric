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
	public class SdMenuCommand : Gtk.ImageMenuItem, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		string localizedText = String.Empty;
		ICommand menuCommand = null;
		
		string tag;
		
		public string Tag {
			get { return tag; }
			set { tag = value; }
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
		
		public SdMenuCommand (string text) : base (text)
		{
			localizedText = text;
		}
		
		public SdMenuCommand(ConditionCollection conditionCollection, object caller, string label) : this(stringParserService.Parse(label))
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.Activated += new EventHandler (OnClick);
			UpdateStatus();
		}
		
		public SdMenuCommand(ConditionCollection conditionCollection, object caller, string label, ICommand menuCommand) : this(stringParserService.Parse(label))
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			this.menuCommand = menuCommand;
			this.Activated += new EventHandler (OnClick);
			UpdateStatus();
		}
		
		public SdMenuCommand(ConditionCollection conditionCollection, object caller, string label, EventHandler handler) : this(stringParserService.Parse(label))
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			this.Activated += handler;
			UpdateStatus();
		}
		
		public SdMenuCommand(object caller, string label, EventHandler handler) : this(stringParserService.Parse(label))
		{
			this.caller = caller;
			this.Activated += handler;
			UpdateStatus();
		}

		public void SetAccel (string[] keys, string pathmod)
		{
			Gdk.ModifierType mod = 0;
			string accel_path = "<MonoDevelop>/MainWindow/" + this.Text + keys[keys.Length - 1];
			uint ckey = 0;
			foreach (string key in keys) {
				if (key == "Control") {
					mod |= Gdk.ModifierType.ControlMask;
				} else if (key == "Shift") {
					mod |= Gdk.ModifierType.ShiftMask;
				} else if (key == "Alt") {
					mod |= Gdk.ModifierType.Mod1Mask;
				} else {
					ckey = Gdk.Keyval.FromName (key);
				}
			}
			if (!Gtk.AccelMap.LookupEntry (accel_path, new Gtk.AccelKey()) ) {
				Gtk.AccelMap.AddEntry (accel_path, ckey, mod);
				this.AccelPath = accel_path;
			}
		}
		
		protected void OnClick(object o, EventArgs e)
		{
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
				ShowAll ();
			}
		}
	}
}
