// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public interface IStatusUpdate
	{
		void UpdateStatus();
		string Key {
			get;
			set;
		}
	}
	
	public class SdMenu : Gtk.ImageMenuItem, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		ConditionCollection conditionCollection;
		object caller;
		string localizedText = String.Empty;
		public ArrayList SubItems = new ArrayList();
		private Gtk.Menu subMenu = null;

		private string key;

		public string Key {
			get { return key; }
			set { key = value; }
		}
		
		public SdMenu(ConditionCollection conditionCollection, object caller, string text) : base()
		{
			this.conditionCollection = conditionCollection;
			this.caller              = caller;
			this.subMenu             = new Gtk.Menu ();
			this.Submenu             = subMenu;

			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService (typeof (IResourceService));
			
			
			if (text.StartsWith("${")) {
				localizedText = resourceService.GetString (text);
			} else {
				localizedText = text;
			}

			localizedText = localizedText.Replace ('&', '_');

			Gtk.AccelLabel label = new Gtk.AccelLabel (localizedText);
			label.Xalign = 0;
			label.UseUnderline = true;
			this.Add (label);
			label.AccelWidget = this;

			Activated += new EventHandler (OnDropDown);

			Gtk.AccelGroup accel = new Gtk.AccelGroup ();
			subMenu.AccelGroup = accel;
			((Gtk.Window)WorkbenchSingleton.Workbench).AddAccelGroup (accel);
			key = text;
		}
		
		public void OnDropDown(object ob, System.EventArgs e)
		{
			foreach (object o in ((Gtk.Menu)Submenu).Children) {
			
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
			UpdateStatus ();
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.Sensitive = (failedAction != ConditionFailedAction.Disable);
				this.Visible = (failedAction != ConditionFailedAction.Exclude);
			}
			
			if (Visible) {
				//foreach (Gtk.Widget widg in ((Gtk.Menu)Submenu).Children) {
				//	((Gtk.Menu)Submenu).Remove (widg);
				//}
				foreach (object item in SubItems) {
					if (item is Gtk.MenuItem) {
						if (item is IStatusUpdate) {
							((IStatusUpdate)item).UpdateStatus();
						}
						Append((Gtk.MenuItem)item);
					} else {
						ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
						Gtk.MenuItem[] items = submenuBuilder.BuildSubmenu(conditionCollection, caller);
						foreach (Gtk.MenuItem menuItem in items) {
							Append (menuItem);
						}
						
					}
				}
				ShowAll ();
			}
		}

		public void Append (Gtk.Widget item)
		{
			try {
				if (item.Parent == null || (item as IStatusUpdate == null)) {
					foreach (IStatusUpdate obj in subMenu.Children)
					{
						if (obj.Key == ((IStatusUpdate)item).Key)
							return;
					}
					subMenu.Append (item);
				}
			} catch { }
		}
	}
}
