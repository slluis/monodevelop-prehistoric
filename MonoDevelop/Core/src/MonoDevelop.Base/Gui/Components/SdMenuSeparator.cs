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
//using Reflector.UserInterface;

namespace MonoDevelop.Gui.Components
{
	public class SdMenuSeparator : Gtk.SeparatorMenuItem, IStatusUpdate 
	{
		object caller;
		ConditionCollection conditionCollection;
		
		public SdMenuSeparator()
		{
			ShowAll ();
		}
		
		public SdMenuSeparator(ConditionCollection conditionCollection, object caller)
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			ShowAll ();
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
