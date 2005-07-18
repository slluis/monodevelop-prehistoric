//
// AddinLoadErrorDialog.cs
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
using Mono.GetOptions;
using Gtk;
using Glade;
using System.Reflection;

using MonoDevelop.Core.AddIns;

namespace MonoDevelop
{
	public class AddinLoadErrorDialog
	{
		[Glade.Widget] Dialog addinLoadErrorDialog;
		[Glade.Widget] Gtk.TreeView errorTree;
		
		public AddinLoadErrorDialog (AddinError[] errors)
		{
			XML glade = new XML (null, "MonoDevelop.Startup.glade", "addinLoadErrorDialog", null);
			glade.Autoconnect (this);
			
			TreeStore store = new TreeStore (typeof(string));
			errorTree.AppendColumn ("Addin", new CellRendererText (), "text", 0);
			errorTree.Model = store;
			
			foreach (AddinError err in errors) {
				TreeIter it = store.AppendValues (Path.GetFileNameWithoutExtension (err.AddinFile));
				store.AppendValues (it, "Full path: " + err.AddinFile);
				store.AppendValues (it, err.Exception.ToString ());
			}
		}
		
		public bool Run ()
		{
			addinLoadErrorDialog.ShowAll ();
			bool res = (((ResponseType)addinLoadErrorDialog.Run ()) == ResponseType.Yes);
			addinLoadErrorDialog.Destroy ();
			return res;
		}
	}
}

