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

using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Conditions;
using MonoDevelop.Core.AddIns.Codons;

using MonoDevelop.Commands;

namespace MonoDevelop.Gui.Components
{
	public interface IStatusUpdate
	{
		void UpdateStatus();
	}
	
	public class SdMenu : Gtk.ImageMenuItem, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
		
		ConditionCollection conditionCollection;
		object caller;
		public string localizedText = String.Empty;
		public ArrayList SubItems = new ArrayList();
		private Gtk.Menu subMenu = null;
		
		public SdMenu(ConditionCollection conditionCollection, object caller, string text) : base()
		{
			this.conditionCollection = conditionCollection;
			this.caller              = caller;
			this.subMenu             = new Gtk.Menu ();
			this.Submenu             = subMenu;

			
			
			localizedText = text;

			Gtk.AccelLabel label = new Gtk.AccelLabel (localizedText);
			label.Xalign = 0;
			label.UseUnderline = true;
			this.Add (label);
			label.AccelWidget = this;

			Activated += new EventHandler (OnDropDown);

			Gtk.AccelGroup accel = new Gtk.AccelGroup ();
			subMenu.AccelGroup = accel;
			((Gtk.Window)WorkbenchSingleton.Workbench).AddAccelGroup (accel);
		}
		
		public void OnDropDown(object ob, System.EventArgs e)
		{
			//foreach (object o in ((Gtk.Menu)Submenu).Children) {
			//
			//	if (o is IStatusUpdate) {
			//		((IStatusUpdate)o).UpdateStatus();
			//	}
			//}
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
				foreach (Gtk.Widget widg in ((Gtk.Menu)Submenu).Children) {
					if (widg is ISubmenuItem) {
						((Gtk.Menu)Submenu).Remove (widg);
						widg.Destroy ();
					}
				}
				foreach (object item in SubItems) {
					if (item is Gtk.MenuItem) {
						if (item is IStatusUpdate) {
							((IStatusUpdate)item).UpdateStatus();
						}
						Append((Gtk.MenuItem)item);
					} else {
						int location = SubItems.IndexOf (item);
						ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
						Gtk.MenuItem[] items = submenuBuilder.BuildSubmenu(conditionCollection, caller);
						foreach (Gtk.MenuItem menuItem in items) {
							subMenu.Insert (menuItem, location);
							location++;
						}
					}
				}
				ShowAll ();
				if (((Gtk.Menu)Submenu).Children.Length == 0) {
					Visible = false;
				}
			}
		}
		
		public void Append (Gtk.Widget item)
		{
			if (item.Parent == null) {
				subMenu.Append (item);
			}
		}
	}
}
