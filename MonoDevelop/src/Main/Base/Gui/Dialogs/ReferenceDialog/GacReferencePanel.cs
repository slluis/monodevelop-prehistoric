// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using MSjogren.GacTool.FusionNative;
using MonoDevelop.Internal.Project;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;

using System.IO;

using Gtk;

namespace MonoDevelop.Gui.Dialogs
{
	public class GacReferencePanel : Frame, IReferencePanel
	{
		SelectReferenceDialog selectDialog;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));

		TreeStore store;
		TreeView  treeView;
		
		public GacReferencePanel(SelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			
			store = new TreeStore (typeof (string), typeof (string), typeof(string), typeof(bool), typeof(string));
			treeView = new TreeView (store);

			TreeViewColumn firstColumn = new TreeViewColumn ();
			firstColumn.Title = GettextCatalog.GetString ("Reference Name");
			CellRendererToggle tog_render = new CellRendererToggle ();
			tog_render.Toggled += new Gtk.ToggledHandler (AddReference);
			firstColumn.PackStart (tog_render, false);
			firstColumn.AddAttribute (tog_render, "active", 3);

			CellRendererText text_render = new CellRendererText ();
			firstColumn.PackStart (text_render, true);
			firstColumn.AddAttribute (text_render, "text", 0);
			
			treeView.AppendColumn (firstColumn);
			treeView.AppendColumn (GettextCatalog.GetString ("Version"), new CellRendererText (), "text", 1);
			treeView.AppendColumn (GettextCatalog.GetString ("Path"), new CellRendererText (), "text", 2);

			store.SetSortColumnId (0, SortType.Ascending);
			
			PrintCache();
			ScrolledWindow sc = new ScrolledWindow ();
			sc.AddWithViewport (treeView);
			this.Add (sc);
			Shadow = ShadowType.In;
			ShowAll ();
		}
		
		public void AddReference(object sender, Gtk.ToggledArgs e)
		{
			Gtk.TreeIter iter;
			store.GetIterFromString (out iter, e.Path);
			if ((bool)store.GetValue (iter, 3) == false) {
				store.SetValue (iter, 3, true);
				selectDialog.AddReference(ReferenceType.Gac,
				                          (string)store.GetValue (iter, 0),
				                          (string)store.GetValue (iter, 4));
				
			} else {
				store.SetValue (iter, 3, false);
				selectDialog.RemoveReference (ReferenceType.Gac,
											  (string)store.GetValue (iter, 0),
											  (string)store.GetValue (iter, 4));
			}
		}

		public void SignalRefChange (string refLoc, bool newstate)
		{
			Gtk.TreeIter looping_iter;
			store.GetIterFirst (out looping_iter);
			do {
				if ((string)store.GetValue (looping_iter, 4) == refLoc) {
					store.SetValue (looping_iter, 3, newstate);
					return;
				}
			} while (store.IterNext (ref looping_iter));
		}
		
		void PrintCache()
		{
#if false
			IApplicationContext applicationContext = null;
			IAssemblyEnum assemblyEnum = null;
			IAssemblyName assemblyName = null;
			
			Fusion.CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
				
			while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0) {
				uint nChars = 0;
				assemblyName.GetDisplayName(null, ref nChars, 0);
									
				StringBuilder sb = new StringBuilder((int)nChars);
				assemblyName.GetDisplayName(sb, ref nChars, 0);
				
				string[] info = sb.ToString().Split(',');
				
				string aName    = info[0];
				string aVersion = info[1].Substring(info[1].LastIndexOf('=') + 1);
				ListViewItem item = new ListViewItem(new string[] {aName, aVersion});
				item.Tag = sb.ToString();
				Items.Add(item);
			}
#endif
			#if false
			System.Reflection.MethodInfo gac = typeof (System.Environment).GetMethod ("internalGetGacPath", System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);
			if (gac == null) {
				Console.WriteLine (GettextCatalog.GetString ("ERROR: non-mono runtime detected, please use the mono runtime for this piece of MonoDevelop for the time being"));
				Environment.Exit (1);
			}
			string gac_path = System.IO.Path.Combine ((string)gac.Invoke (null, null), "");
			DirectoryInfo d = new DirectoryInfo (System.IO.Path.Combine (System.IO.Path.Combine (gac_path, "mono"), "gac"));
			foreach (DirectoryInfo namedDir in d.GetDirectories ()) {
				foreach (DirectoryInfo assemblyDir in namedDir.GetDirectories ()) {
					FileInfo[] files = assemblyDir.GetFiles ("*.dll");
					try {
						System.Reflection.AssemblyName an = System.Reflection.AssemblyName.GetAssemblyName (files[0].FullName);
					
						store.AppendValues (an.Name, an.Version.ToString (), System.IO.Path.GetFileName (files[0].FullName), false, an.FullName);
					} catch {
					}
				}
			}
			#endif
			SystemAssemblyService sas = (SystemAssemblyService)ServiceManager.Services.GetService (typeof (SystemAssemblyService));
			foreach (string assemblyPath in sas.AssemblyPaths) {
				try {
					System.Reflection.AssemblyName an = System.Reflection.AssemblyName.GetAssemblyName (assemblyPath);
					store.AppendValues (an.Name, an.Version.ToString (), System.IO.Path.GetFileName (assemblyPath), false, an.FullName);
				}catch {
				}
			}
		}
	}
}
