//
// ConfigurationComboBox.cs
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
using MonoDevelop.Services;
using MonoDevelop.Internal.Project;

namespace MonoDevelop.Gui
{
	public class ConfigurationComboBox: Gtk.Alignment
	{
		Gtk.ComboBox combo;
		ConfigurationEventHandler onActiveConfigurationChanged;
		
		public ConfigurationComboBox (): base (0.5f, 0.5f, 1.0f, 0f)
		{
			LeftPadding = 3;
			RightPadding = 3;
			combo = Gtk.ComboBox.NewText ();
			combo.Changed += new EventHandler (OnChanged);
			Add (combo);
			ShowAll ();
			onActiveConfigurationChanged = (ConfigurationEventHandler) Runtime.DispatchService.GuiDispatch (new ConfigurationEventHandler (OnActiveConfigurationChanged));
			Runtime.ProjectService.CombineOpened += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler (OpenCombine));
			Runtime.ProjectService.CombineClosed += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler (CloseCombine));
			Reset ();
		}
		
		void Reset ()
		{
			((Gtk.ListStore)combo.Model).Clear ();
			combo.AppendText ("dummy");
			combo.Active = -1;
			combo.Sensitive = false;
		}
		
		void OpenCombine (object sender, CombineEventArgs e)
		{
			((Gtk.ListStore)combo.Model).Clear ();
			combo.Sensitive = true;
			int active = 0;
			for (int n=0; n < e.Combine.Configurations.Count; n++) {
				IConfiguration c = e.Combine.Configurations [n];
				combo.AppendText (c.Name);
				if (e.Combine.ActiveConfiguration == c)
					active = n;
			}
			combo.Active = active;
			combo.ShowAll ();
			
			e.Combine.ActiveConfigurationChanged += onActiveConfigurationChanged;
		}

		void CloseCombine (object sender, CombineEventArgs e)
		{
			Reset ();
			e.Combine.ActiveConfigurationChanged -= onActiveConfigurationChanged;
		}
		
		void OnActiveConfigurationChanged (object sender, ConfigurationEventArgs e)
		{
			Combine combine = (Combine) e.CombineEntry;
			for (int n=0; n < combine.Configurations.Count; n++) {
				IConfiguration c = combine.Configurations [n];
				if (combine.ActiveConfiguration == c) {
					combo.Active = n;
					break;
				}
			}
		}
		
		protected void OnChanged (object sender, EventArgs args)
		{
			if (Runtime.ProjectService.CurrentOpenCombine != null) {
				Gtk.TreeIter iter;
				if (combo.GetActiveIter (out iter)) {
					string cs = (string) combo.Model.GetValue (iter, 0);
					IConfiguration conf = Runtime.ProjectService.CurrentOpenCombine.GetConfiguration (cs);
					Runtime.ProjectService.CurrentOpenCombine.ActiveConfiguration = conf;
				}
			}
		}
	}
}
