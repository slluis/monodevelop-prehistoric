//
// StatusProgressMonitor.cs
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
using System.IO;
using MonoDevelop.Gui;

namespace MonoDevelop.Services
{
	internal class StatusProgressMonitor: BaseProgressMonitor
	{
		string title;
		Gtk.Image icon;
		bool showErrorDialogs;
		
		public StatusProgressMonitor (string title, string iconName, bool showErrorDialogs)
		{
			this.title = title;
			icon = Runtime.Gui.Resources.GetImage (iconName, Gtk.IconSize.Menu);
			Runtime.Gui.StatusBar.BeginProgress (title);
			Runtime.Gui.StatusBar.SetMessage (icon, title);
		}
		
		protected override void OnProgressChanged ()
		{
			Runtime.Gui.StatusBar.SetMessage (icon, CurrentTask);
			if (!UnknownWork)
				Runtime.Gui.StatusBar.SetProgressFraction (GlobalWork);
		}
		
		protected override void OnCompleted ()
		{
			Runtime.Gui.StatusBar.EndProgress ();

			if (Errors.Count > 0) {
				if (showErrorDialogs) {
					string s = "";
					foreach (string m in Errors)
						s += m + "\n";
					Runtime.MessageService.ShowError (s);
				}
				Gtk.Image img = Runtime.Gui.Resources.GetImage (Stock.Error, Gtk.IconSize.Menu);
				Runtime.Gui.StatusBar.SetMessage (img, Errors [Errors.Count - 1]);
				base.OnCompleted ();
				return;
			}
			
			if (Warnings.Count > 0) {
				if (showErrorDialogs) {
					string s = "";
					foreach (string m in Warnings)
						s += m + "\n";
					Runtime.MessageService.ShowWarning (s);
				}
				
				if (SuccessMessages.Count == 0) {
					Gtk.Image img = Runtime.Gui.Resources.GetImage (Stock.Warning, Gtk.IconSize.Menu);
					Runtime.Gui.StatusBar.SetMessage (img, Warnings [Warnings.Count - 1]);
					base.OnCompleted ();
					return;
				}
			}
			
			if (SuccessMessages.Count > 0)
				Runtime.Gui.StatusBar.SetMessage (SuccessMessages [SuccessMessages.Count - 1]);
			else
				Runtime.Gui.StatusBar.SetMessage (GettextCatalog.GetString ("Ready"));
				
			base.OnCompleted ();
		}
	}
}
