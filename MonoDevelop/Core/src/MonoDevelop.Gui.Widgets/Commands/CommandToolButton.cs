//
// CommandToolButton.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace MonoDevelop.Commands
{
	public class CommandToolButton: Gtk.ToolButton, ICommandUserItem
	{
		CommandManager commandManager;
		object commandId;
		static Gtk.Tooltips tips = new Gtk.Tooltips ();
		string lastDesc;
		
		public CommandToolButton (object commandId, CommandManager commandManager): base ("")
		{
			this.commandId = commandId;
			this.commandManager = commandManager;
		}
		
		protected override void OnParentSet (Gtk.Widget parent)
		{
			base.OnParentSet (parent);
			if (Parent == null) return;

			((ICommandUserItem)this).Update ();
		}
		
		void ICommandUserItem.Update ()
		{
			if (commandManager != null) {
				CommandInfo cinfo = commandManager.GetCommandInfo (commandId);
				Update (cinfo);
			}
		}
		
		protected override void OnClicked ()
		{
			base.OnClicked ();

			if (commandManager == null)
				throw new InvalidOperationException ();
				
			commandManager.DispatchCommand (commandId);
		}
		
		void Update (CommandInfo cmdInfo)
		{
			if (lastDesc != cmdInfo.Description) {
				SetTooltip (tips, cmdInfo.Description, cmdInfo.Description);
				lastDesc = cmdInfo.Description;
			}
			Label = cmdInfo.Text;
			StockId = cmdInfo.Icon;
			Sensitive = cmdInfo.Enabled;
			Visible = cmdInfo.Visible;
		}
	}
}
