//
// ErrorDialog.cs
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
using Gtk;
using Glade;

namespace MonoDevelop.Gui.Dialogs
{
	internal class ErrorDialog
	{
		[Glade.Widget ("ErrorDialog")] Dialog dialog;
		[Glade.Widget] Button okButton;
		[Glade.Widget] Label descriptionLabel;
		[Glade.Widget] Gtk.TextView detailsTextView;
		
		public ErrorDialog (string message, string details)
		{
			new Glade.XML (null, "Base.glade", "ErrorDialog", null).Autoconnect (this);
			dialog.TransientFor = (Window) WorkbenchSingleton.Workbench;
			descriptionLabel.Text = message;
			detailsTextView.Buffer.Text = details;
			okButton.Clicked += new EventHandler (OnClose);
		}
		
		public void Run ()
		{
			dialog.ShowAll ();
//			dialog.Run ();
		}
		
		void OnClose (object sender, EventArgs args)
		{
			dialog.Destroy ();
		}
	}
}
