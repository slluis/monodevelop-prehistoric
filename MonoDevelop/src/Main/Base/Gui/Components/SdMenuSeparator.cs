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
//using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdMenuSeparator : Gtk.SeparatorMenuItem, IStatusUpdate 
	{
		object caller;
		ConditionCollection conditionCollection;

		string key;

		public string Key {
			get { return key; }
			set { key = value; }
		}
		
		public SdMenuSeparator(string id)
		{
			key = id;
		}
		
		public SdMenuSeparator(string id, ConditionCollection conditionCollection, object caller) : this (id)
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.Sensitive = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
		}
	}
}
