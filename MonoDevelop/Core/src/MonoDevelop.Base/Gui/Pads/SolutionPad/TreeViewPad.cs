//
// TreeViewPad.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;
using System.Xml;
using System.Resources;
using System.Text;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Core.Services;
using MonoDevelop.Internal.Project;
using MonoDevelop.Gui.Dialogs;
using MonoDevelop.Services;
using MonoDevelop.Gui.Widgets;

namespace MonoDevelop.Gui.Pads
{
	/// <summary>
	/// This class implements a project browser.
	/// </summary>
	public class TreeViewPad : IPadContent, IMementoCapable
	{
		string title;
		string icon;
		string id;
		string defaultPosition = "left";
		
		internal const int TextColumn = 0;
		internal const int OpenIconColumn = 1;
		internal const int ClosedIconColumn = 2;
		internal const int DataItemColumn = 3;
		internal const int BuilderChainColumn = 4;
		internal const int WeightColumn = 5;
		internal const int FilledColumn = 6;
		
		NodeBuilder[] builders;
		Hashtable builderChains = new Hashtable ();
		Hashtable nodeHash = new Hashtable ();
		Gtk.TreeView tree;
		Gtk.TreeStore store;
		internal Gtk.TreeViewColumn complete_column;
		internal Gtk.CellRendererText text_render;
		TreeBuilderContext builderContext;
		Hashtable callbacks = new Hashtable ();
		
		TreePadOption[] options;
		TreeOptions globalOptions;
		Hashtable nodeOptions = new Hashtable ();
		
		object dragObject;

		Gtk.Frame contentPanel = new Gtk.Frame();

		private static Gtk.TargetEntry [] target_table = new Gtk.TargetEntry [] {
			new Gtk.TargetEntry ("text/uri-list", 0, 11 ),
			new Gtk.TargetEntry ("text/plain", 0, 22),
			new Gtk.TargetEntry ("application/x-rootwindow-drop", 0, 33)
		};
	
		public string Id {
			get { return id; }
			set { id = value; }
		}
		
		public string DefaultPlacement {
			get { return defaultPosition; }
			set { defaultPosition = value; }
		}

		public Gtk.Widget Control {
			get {
				return contentPanel;
			}
		}

		public void BringToFront() {
			// TODO FIXME
		}
		
		public string Title {
			get { return title; }
			set { title = value; }
		}

		public string Icon {
			get { return icon; }
			set { icon = value; }
		}

		public void RedrawContent()
		{
		}
		
		public TreeViewPad (string label, string icon, NodeBuilder[] builders, TreePadOption[] options)
		{
			// Create default options
			
			this.options = options;
			globalOptions = new TreeOptions ();
			foreach (TreePadOption op in options)
				globalOptions [op.Id] = op.DefaultValue;
				
			globalOptions.Pad = this;
			
			// Check that there is only one node builder per type
			
			Hashtable bc = new Hashtable ();
			foreach (NodeBuilder nb in builders) {
				TypeNodeBuilder tnb = nb as TypeNodeBuilder;
				if (tnb != null) {
					TypeNodeBuilder other = (TypeNodeBuilder) bc [tnb.NodeDataType];
					if (other != null)
						throw new ApplicationException (string.Format ("The type node builder {0} can't be used in this context because the type {1} is already handled by {2}", nb.GetType(), tnb.NodeDataType, other.GetType()));
					bc [tnb.NodeDataType] = tnb;
				}
				else if (!(nb is NodeBuilderExtension))
					throw new InvalidOperationException (string.Format ("Invalid NodeBuilder type: {0}. NodeBuilders must inherit either from TypeNodeBuilder or NodeBuilderExtension", nb.GetType()));
			}
			
			NodeBuilders = builders;
			Title = label;
			Icon = icon;

			builderContext = new TreeBuilderContext (this);
			
			tree = new Gtk.TreeView ();
			
			/*
			0 -- Text
			1 -- Icon (Open)
			2 -- Icon (Closed)
			3 -- Node Data
			4 -- Builder chain
			5 -- Pango weight
			6 -- Expanded
			*/
			store = new Gtk.TreeStore (typeof (string), typeof (Gdk.Pixbuf), typeof (Gdk.Pixbuf), typeof (object), typeof (object), typeof(int), typeof(bool));
			tree.Model = store;

/*
			tree.EnableModelDragSource (Gdk.ModifierType.Button1Mask, target_table, Gdk.DragAction.Copy | Gdk.DragAction.Move);
			tree.EnableModelDragDest (target_table, Gdk.DragAction.Copy | Gdk.DragAction.Move);
*/

			store.SetDefaultSortFunc (new Gtk.TreeIterCompareFunc (CompareNodes), IntPtr.Zero, null);
			store.SetSortColumnId (/* GTK_TREE_SORTABLE_DEFAULT_SORT_COLUMN_ID */ -1, Gtk.SortType.Ascending);
			
			tree.HeadersVisible = false;
			tree.SearchColumn = 0;
			tree.EnableSearch = true;
			complete_column = new Gtk.TreeViewColumn ();
			complete_column.Title = "column";

			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			complete_column.PackStart (pix_render, false);
			complete_column.AddAttribute (pix_render, "pixbuf", OpenIconColumn);
			complete_column.AddAttribute (pix_render, "pixbuf-expander-open", OpenIconColumn);
			complete_column.AddAttribute (pix_render, "pixbuf-expander-closed", ClosedIconColumn);

			text_render = new Gtk.CellRendererText ();
			text_render.Edited += new Gtk.EditedHandler (HandleOnEdit);
			
			complete_column.PackStart (text_render, true);
			complete_column.AddAttribute (text_render, "text", TextColumn);
			complete_column.AddAttribute (text_render, "weight", WeightColumn);
	
			tree.AppendColumn (complete_column);
			
			Gtk.ScrolledWindow sw = new Gtk.ScrolledWindow ();
			sw.Add(tree);
			contentPanel = new Gtk.Frame();
			contentPanel.Add(sw);
			
			tree.TestExpandRow += new Gtk.TestExpandRowHandler (OnTestExpandRow);
			tree.RowActivated += new Gtk.RowActivatedHandler(OnNodeActivated);
			
			contentPanel.ButtonReleaseEvent += new Gtk.ButtonReleaseEventHandler(OnButtonRelease);
			contentPanel.PopupMenu += OnPopupMenu;
			
			foreach (NodeBuilder nb in builders)
				nb.SetContext (builderContext);
			
/*			tree.DragBegin += new Gtk.DragBeginHandler (OnDragBegin);
			tree.DragDataGet += new Gtk.DragDataGetHandler (OnDragDataGet);
			tree.DragDataReceived += new Gtk.DragDataReceivedHandler (OnDragDataReceived);
			tree.DragDrop += new Gtk.DragDropHandler (OnDragDrop);
			tree.DragEnd += new Gtk.DragEndHandler (OnDragEnd);
			tree.DragLeave += new Gtk.DragLeaveHandler (OnDragLeave);
			tree.DragMotion += new Gtk.DragMotionHandler (OnDragMotion);
*/
		}

/*
		void OnDragBegin (object o, Gtk.DragBeginArgs arg)
		{
			ITreeNavigator nav = GetSelectedNode ();
			dragObject = nav.DataItem;
			Console.WriteLine ("OnDragBegin");
		}
		
		void OnDragDataGet (object o, Gtk.DragDataGetArgs args)
		{
			Console.WriteLine ("OnDragDataGet");
			args.SelectionData.Set (args.Context.Targets[0], 0, new byte[0]);
		}
		
		void OnDragDataReceived (object o, Gtk.DragDataReceivedArgs args)
		{
			Console.WriteLine ("OnDragDataReceived " + args.X + " " + args.Y + " " + args.SelectionData.Length);
			
			if (dragObject != null) {
				bool res = CheckAndDrop (args.X, args.Y, true);
				Gtk.Drag.Finish (args.Context, res, true, args.Time);
			} else {
				if (args.SelectionData.Data.Length > 0) {
					string fullData = System.Text.Encoding.UTF8.GetString (args.SelectionData.Data);
					Console.WriteLine ("file:" + fullData);
				}
				Gtk.Drag.Finish (args.Context, false, true, args.Time);
			}
		}
		
		void OnDragDrop (object o, Gtk.DragDropArgs args)
		{
			Console.WriteLine ("OnDragDrop " + args.X + " " + args.Y);
		}
		
		void OnDragEnd (object o, Gtk.DragEndArgs args)
		{
			dragObject = null;
			Console.WriteLine ("OnDragEnd");
		}
		
		void OnDragLeave (object sender, Gtk.DragLeaveArgs args)
		{
			Console.WriteLine ("OnDragLeave");
		}
		
		[GLib.ConnectBefore]
		void OnDragMotion (object o, Gtk.DragMotionArgs args)
		{
			if (dragObject != null) {
				if (!CheckAndDrop (args.X, args.Y, false)) {
					Gdk.Drag.Status (args.Context, (Gdk.DragAction)0, args.Time);
					args.RetVal = true;
				}
			}
		}
		
		bool CheckAndDrop (int x, int y, bool drop)
		{
			Gtk.TreePath path;
			Gtk.TreeViewDropPosition pos;
			if (!tree.GetDestRowAtPos (x, y, out path, out pos)) return false;
			
			Gtk.TreeIter iter;
			if (!store.GetIter (out iter, path)) return false;
			
			TreeNodeNavigator nav = new TreeNodeNavigator (this, iter);
			Console.WriteLine ("Trying drop to " + nav.NodeName);
			NodeBuilder[] chain = nav.BuilderChain;
			bool foundHandler = false;
			
			foreach (NodeBuilder nb in chain) {
				nb.CommandHandler.SetCurrentNode (nav);
				if (nb.CommandHandler.CanDropNode (dragObject, DragOperation.Copy)) {
					foundHandler = true;
					if (drop) {
						Console.WriteLine ("Droping to " + nb);
						nb.CommandHandler.OnNodeDrop (dragObject, DragOperation.Copy);
					}
				}
			}
			return foundHandler;
		}
*/

		public virtual void Dispose ()
		{
			Clear ();
			foreach (NodeBuilder nb in builders)
				nb.Dispose ();
		}
		
		protected NodeBuilder[] NodeBuilders {
			get { return builders; }
			set { builders = value; }
		}
		
		protected void LoadTree (object nodeObject)
		{
			Clear ();
			TreeBuilder builder = new TreeBuilder (this);
			builder.AddChild (nodeObject, true);
			builder.Expanded = true;
		}
		
		protected void Clear ()
		{
			object[] obs = new object [nodeHash.Count];
			nodeHash.Keys.CopyTo (obs, 0);
			
			foreach (object dataObject in obs)
				UnregisterNode (dataObject, null);

			nodeHash = new Hashtable ();
			store.Clear ();
		}
		
		public ITreeNavigator GetSelectedNode ()
		{
			Gtk.TreeModel foo;
			Gtk.TreeIter iter;
			if (!tree.Selection.GetSelected (out foo, out iter))
				return null;
			
			return new TreeNodeNavigator (this, iter);
		}
		
		public ITreeNavigator GetNodeAtPosition (NodePosition position)
		{
			return new TreeNodeNavigator (this, position._iter);
		}
		
		public ITreeNavigator GetNodeAtObject (object dataObject)
		{
			object it = nodeHash [dataObject];
			if (it == null) return null;
			return new TreeNodeNavigator (this, (Gtk.TreeIter)it);
		}
		
		public ITreeNavigator GetRootNode ()
		{
			Gtk.TreeIter iter;
			if (!store.GetIterFirst (out iter)) return null;
			return new TreeNodeNavigator (this, iter);
		}
		
		public void AddNodeInsertCallback (object dataObject, TreeNodeCallback callback)
		{
			if (IsRegistered (dataObject)) {
				callback (GetNodeAtObject (dataObject));
				return;
			}
				
			ArrayList list = callbacks [dataObject] as ArrayList;
			if (list != null)
				list.Add (callback);
			else {
				list = new ArrayList ();
				list.Add (callback);
				callbacks [dataObject] = list;
			}
		}
		
		public void ActivateCurrentItem ()
		{
			ITreeNavigator node = GetSelectedNode ();
			if (node != null) {
				object data = node.DataItem;
				NodeBuilder[] chain = GetBuilderChain (data.GetType ());
				NodePosition pos = node.CurrentPosition;
				foreach (NodeBuilder b in chain) {
					b.CommandHandler.SetCurrentNode (node);
					b.CommandHandler.ActivateItem ();
					node.MoveToPosition (pos);
				}
			}
		}

		public void RemoveCurrentItem ()
		{
			ITreeNavigator node = GetSelectedNode ();
			if (node != null) {
				object data = node.DataItem;
				NodeBuilder[] chain = GetBuilderChain (data.GetType ());
				NodePosition pos = node.CurrentPosition;
				foreach (NodeBuilder b in chain) {
					b.CommandHandler.SetCurrentNode (node);
					b.CommandHandler.RemoveItem ();
					node.MoveToPosition (pos);
				}
			}
		}

		/// <summary>
		/// If you want to edit a node label. Select the node you want to edit and then
		/// call this method, instead of using the LabelEdit Property and the BeginEdit
		/// Method directly.
		/// </summary>
		public void StartLabelEdit()
		{
			Gtk.TreeModel foo;
			Gtk.TreeIter iter;
			if (!tree.Selection.GetSelected (out foo, out iter))
				return;
			
			object data = store.GetValue (iter, TreeViewPad.DataItemColumn);
			
			TypeNodeBuilder nb = GetTypeNodeBuilder (data.GetType ());
			store.SetValue (iter, TreeViewPad.TextColumn, nb.GetNodeName (data));
			
			text_render.Editable = true;
			tree.SetCursor (store.GetPath (iter), complete_column, true);
		}

		void HandleOnEdit (object o, Gtk.EditedArgs e)
		{
			text_render.Editable = false;
			Gtk.TreeIter iter;
			if (!store.GetIterFromString (out iter, e.Path))
				throw new Exception("Error calculating iter for path " + e.Path);

			if (e.NewText == null || e.NewText.Length == 0) {
				return;
			}

			ITreeNavigator nav = new TreeNodeNavigator (this, iter);
			NodePosition pos = nav.CurrentPosition;

			NodeBuilder[] chain = (NodeBuilder[]) store.GetValue (iter, BuilderChainColumn);
			foreach (NodeBuilder b in chain) {
				b.CommandHandler.SetCurrentNode (nav);
				b.CommandHandler.RenameItem (e.NewText);
				nav.MoveToPosition (pos);
			}
		}
		
		public void SaveTreeState (XmlElement el)
		{
			Gtk.TreeIter iter;
			if (!store.GetIterFirst (out iter)) return;
			XmlElement child = SaveTree (el.OwnerDocument, globalOptions, new TreeNodeNavigator (this, iter));
			if (child != null) el.AppendChild (child);
		}
		
		XmlElement SaveTree (XmlDocument doc, ITreeOptions parentOptions, TreeNodeNavigator nav)
		{
			XmlElement el = null;
			ITreeOptions ops = nodeOptions [nav.DataItem] as ITreeOptions;
			
			if (nav.Expanded || ops != null) {
				el = doc.CreateElement ("Node");
				if (ops != null) {
					foreach (TreePadOption op in options) {
						if (parentOptions [op.Id] != ops [op.Id]) {
							XmlElement eop = doc.CreateElement ("Option");
							eop.SetAttribute ("id", op.Id);
							eop.SetAttribute ("value", ops [op.Id].ToString ());
							el.AppendChild (eop);
						}
					}
				}
			}
			
			if (!nav.Filled || !nav.MoveToFirstChild ())
				return el;

			do {
				XmlElement child = SaveTree (doc, ops != null ? ops : parentOptions, nav);
				if (child != null) {
					if (el == null) el = doc.CreateElement ("Node");
					el.AppendChild (child);
				}
			} while (nav.MoveNext ());
			
			nav.MoveToParent ();
			
			if (el != null) {
				el.SetAttribute ("name", nav.NodeName);
				el.SetAttribute ("expanded", nav.Expanded.ToString ());
			}
			return el;
		}
		
		public void RestoreTreeState (XmlElement parent)
		{
			ITreeNavigator nav = GetRootNode ();
			if (nav != null)
				RestoreTree (parent, nav);
		}
		
		void RestoreTree (XmlElement parent, ITreeNavigator nav)
		{
			XmlNodeList nodelist = parent.ChildNodes;
			foreach (XmlNode nod in nodelist) {
				XmlElement el = nod as XmlElement;
				if (el == null) continue;
				if (el.LocalName == "Option") {
					nav.Options [el.GetAttribute ("id")] = bool.Parse (el.GetAttribute ("value"));
				}
			}

			string expanded = parent.GetAttribute ("expanded");
			if (expanded == "" || bool.Parse (expanded))
				nav.Expanded = true;
				
			foreach (XmlNode nod in nodelist) {
				XmlElement el = nod as XmlElement;
				if (el == null) continue;
				if (el.LocalName == "Node") {
					string name = el.GetAttribute ("name");
					if (nav.MoveToChild (name, null)) {
						RestoreTree (el, nav);
						nav.MoveToParent ();
					}
				}
			}
		}
		
		NodeBuilder[] GetBuilderChain (Type type)
		{
			NodeBuilder[] chain = builderChains [type] as NodeBuilder[];
			if (chain == null)
			{
				ArrayList list = new ArrayList ();
				foreach (NodeBuilder nb in builders) {
					if (nb is TypeNodeBuilder) {
						if (((TypeNodeBuilder)nb).NodeDataType.IsAssignableFrom (type))
							list.Insert (0, nb);
					}
					else if (((NodeBuilderExtension)nb).CanBuildNode (type))
						list.Add (nb);
				}
				
				chain = (NodeBuilder[]) list.ToArray (typeof(NodeBuilder));
				
				if (chain.Length == 0 || !(chain[0] is TypeNodeBuilder))
					chain = null;

				builderChains [type] = chain;
			}
			return chain;
		}
		
		TypeNodeBuilder GetTypeNodeBuilder (Type type)
		{
			NodeBuilder[] chain = GetBuilderChain (type);
			if (chain != null && chain.Length > 0)
				return chain[0] as TypeNodeBuilder;
			return null;
		}
		
		int CompareNodes (Gtk.TreeModel model, Gtk.TreeIter a, Gtk.TreeIter b)
		{
			object o1 = store.GetValue (a, DataItemColumn);
			object o2 = store.GetValue (b, DataItemColumn);
			
			NodeBuilder[] chain1 = (NodeBuilder[]) store.GetValue (a, BuilderChainColumn);
			if (chain1 == null) return 0;
			
			TypeNodeBuilder tb1 = (TypeNodeBuilder) chain1[0];
			int sort = tb1.CompareObjects (o1, o2);
			if (sort != TypeNodeBuilder.DefaultSort) return sort;
			
			NodeBuilder[] chain2 = (NodeBuilder[]) store.GetValue (b, BuilderChainColumn);
			if (chain2 == null) return 0;
			
			if (chain1 == chain2)
				return string.Compare (tb1.GetNodeName (o1), tb1.GetNodeName (o2), true);
				
			TypeNodeBuilder tb2 = (TypeNodeBuilder) chain2[0];
			sort = tb2.CompareObjects (o2, o1);
			if (sort != TypeNodeBuilder.DefaultSort) return sort * -1;

			return string.Compare (tb1.GetNodeName (o1), tb2.GetNodeName (o2), true);
		}
		
		internal void RegisterNode (Gtk.TreeIter it, object dataObject, NodeBuilder[] chain)
		{
			nodeHash [dataObject] = it;

			if (chain == null) chain = GetBuilderChain (dataObject.GetType());
			foreach (NodeBuilder nb in chain)
				nb.OnNodeAdded (dataObject);
		}
		
		internal void UnregisterNode (object dataObject, NodeBuilder[] chain)
		{
			nodeOptions.Remove (dataObject);
			nodeHash.Remove (dataObject);
			if (chain == null) chain = GetBuilderChain (dataObject.GetType());
			foreach (NodeBuilder nb in chain)
				nb.OnNodeRemoved (dataObject);
		}
		
		internal bool IsRegistered (object dataObject)
		{
			return nodeHash.Contains (dataObject);
		}
		
		internal void NotifyInserted (Gtk.TreeIter it, object dataObject)
		{
			if (callbacks.Count > 0) {
				ArrayList list = callbacks [dataObject] as ArrayList;
				if (list != null) {
					ITreeNavigator nav = new TreeNodeNavigator (this, it);
					NodePosition pos = nav.CurrentPosition;
					foreach (TreeNodeCallback callback in list) {
						callback (nav);
						nav.MoveToPosition (pos);
					}
					callbacks.Remove (dataObject);
				}
			}
		}
		
		TreeOptions GetOptions (Gtk.TreeIter iter, bool createSpecificOptions)
		{
			if (nodeOptions.Count == 0) {
				if (createSpecificOptions) {
					object dataObject = store.GetValue (iter, DataItemColumn);
					TreeOptions ops = globalOptions.CloneOptions (dataObject);
					nodeOptions [dataObject] = ops;
					return ops;
				}
				else
					return globalOptions;
			}
			
			TreeOptions result = null;
			Gtk.TreeIter it = iter;
			do {
				if (store.GetValue (it, BuilderChainColumn) != null) {
					object ob = store.GetValue (it, DataItemColumn);
					result = nodeOptions [ob] as TreeOptions;
				}
			} while (result == null && store.IterParent (out it, it));

			if (result == null)
				result = globalOptions;
			
			if (createSpecificOptions && !it.Equals (iter)) {
				object dataObject = store.GetValue (iter, DataItemColumn);
				TreeOptions ops = result.CloneOptions (dataObject);
				nodeOptions [dataObject] = ops;
				return ops;
			} else
				return result;
		}
		
		internal void ClearOptions (Gtk.TreeIter iter)
		{
			if (nodeOptions.Count == 0)
				return;
				
			ArrayList toDelete = new ArrayList ();
			string path = store.GetPath (iter).ToString () + ":";
			
			foreach (object ob in nodeOptions.Keys) {
				Gtk.TreeIter nit = (Gtk.TreeIter) nodeHash [ob];
				string npath = store.GetPath (nit).ToString () + ":";
				if (npath.StartsWith (path))
					toDelete.Add (ob);
			}

			foreach (object ob in toDelete)
				nodeOptions.Remove (ob);
		}

		internal string GetNamePathFromIter (Gtk.TreeIter iter)
		{
			StringBuilder sb = new StringBuilder ();
			do {
				NodeBuilder[] chain = (NodeBuilder[]) store.GetValue (iter, BuilderChainColumn);
				string name;
				if (chain == null)
					name = (string) store.GetValue (iter, TextColumn);
				else {
					object ob = store.GetValue (iter, DataItemColumn);
					name = ((TypeNodeBuilder)chain[0]).GetNodeName (ob);
				}
				if (sb.Length > 0) sb.Insert (0, '/');
				name = name.Replace ("%","%%");
				name = name.Replace ("/","_%_");
				sb.Insert (0, name);
			} while (store.IterParent (out iter, iter));

			return sb.ToString ();
		}
		
		internal void RefreshNode (object dataObject)
		{
			ITreeBuilder builder = new TreeBuilder (this);
			bool found;
			
			if (dataObject != null) found = builder.MoveToObject (dataObject);
			else found = builder.MoveToRoot ();
			
			if (found) {
				builder.Update ();
				builder.UpdateChildren ();
			}
		}
		
		internal bool GetIterFromNamePath (string path, out Gtk.TreeIter iter)
		{
			if (!store.GetIterFirst (out iter))
				return false;
				
			TreeNodeNavigator nav = new TreeNodeNavigator (this, iter);
			string[] names = path.Split ('/');

			int n = 0;
			bool more;
			do {
				string name = names [n].Replace ("_%_","/");
				name = name.Replace ("%%","%");
				
				if (nav.NodeName == name) {
					iter = nav.CurrentPosition._iter;
					if (++n == names.Length) return true;
					more = nav.MoveToFirstChild ();
				} else
					more = nav.MoveNext ();
			} while (more);

			return false;
		}

		public void StealFocus ()
		{
			GLib.Timeout.Add (20, new GLib.TimeoutHandler (wantFocus));
		}
		bool wantFocus ()
		{
			tree.GrabFocus ();
			StartLabelEdit ();
			return false;
		}

		private void OnTestExpandRow (object sender, Gtk.TestExpandRowArgs args)
		{
			bool filled = (bool) store.GetValue (args.Iter, FilledColumn);
			if (!filled) {
				TreeBuilder nb = new TreeBuilder (this, args.Iter);
				args.RetVal = !nb.FillNode ();
			} else
				args.RetVal = false;
		}

		void ShowPopup ()
		{
			ITreeNavigator tnav = GetSelectedNode ();
			Runtime.ProjectService.CurrentSelectedProject = tnav.GetParentDataItem (typeof(Project), true) as Project;
			Runtime.ProjectService.CurrentSelectedCombine = tnav.GetParentDataItem (typeof(Combine), true) as Combine;
			
			TypeNodeBuilder nb = GetTypeNodeBuilder (tnav.DataItem.GetType ());
			if (nb == null || nb.ContextMenuAddinPath == null) {
				if (options.Length > 0)
					Runtime.Gui.Menus.ShowContextMenu (BuildTreeOptionsMenu (tnav.DataItem));
			} else {
				Gtk.Menu menu = Runtime.Gui.Menus.CreateContextMenu (this, nb.ContextMenuAddinPath);
				if (options.Length > 0) {
					Gtk.MenuItem mi = new Gtk.SeparatorMenuItem ();
					mi.Show ();
					menu.Append (mi);
					
					mi = new Gtk.MenuItem (GettextCatalog.GetString ("Display Options"));
					menu.Append (mi);
					mi.Submenu = BuildTreeOptionsMenu (tnav.DataItem);
					mi.Show ();
				}
				Runtime.Gui.Menus.ShowContextMenu (menu);
			}
		}
		
		Gtk.Menu BuildTreeOptionsMenu (object dataObject)
		{
			ITreeOptions currentOptions = builderContext.GetOptions (dataObject);
			Gtk.Menu omenu = new Gtk.Menu ();

			foreach (TreePadOption op in options) {
				PadCheckMenuItem cmi = new PadCheckMenuItem (op.Label, op.Id);
				cmi.Active = currentOptions [op.Id];
				omenu.Append (cmi);
				cmi.Toggled += new EventHandler (OptionToggled);
			}
			
			omenu.Append (new Gtk.SeparatorMenuItem ());
			
			Gtk.MenuItem mi = new Gtk.MenuItem (GettextCatalog.GetString ("Reset Options"));
			mi.Activated += new EventHandler (ResetOptions);
			omenu.Append (mi);
			omenu.ShowAll ();
			
			return omenu;
		}
		
		void OptionToggled (object sender, EventArgs args)
		{
			Gtk.TreeModel foo;
			Gtk.TreeIter iter;
			if (!tree.Selection.GetSelected (out foo, out iter))
				return;

			PadCheckMenuItem mi = (PadCheckMenuItem) sender;
			GetOptions (iter, true) [mi.Id] = mi.Active;
		}
		
		void ResetOptions (object sender, EventArgs args)
		{
			Gtk.TreeModel foo;
			Gtk.TreeIter iter;
			if (!tree.Selection.GetSelected (out foo, out iter))
				return;

			ClearOptions (iter);
			TreeBuilder tb = new TreeBuilder (this, iter);
			tb.Update ();
			tb.UpdateChildren ();
		}

		void OnPopupMenu (object o, Gtk.PopupMenuArgs args)
		{
			if (GetSelectedNode () != null)
				ShowPopup ();
		}

		private void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 3 && GetSelectedNode() != null) {
				ShowPopup ();
			}
		}

		private void OnNodeActivated (object sender, Gtk.RowActivatedArgs args)
		{
			ActivateCurrentItem ();
		}
		
		public IXmlConvertable CreateMemento ()
		{
			return new TreeViewPadMemento (this);
		}

		public void SetMemento (IXmlConvertable memento)
		{
			((TreeViewPadMemento)memento).Restore (this);
		}

		// ********* Own events
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}

		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

		class PadCheckMenuItem: Gtk.CheckMenuItem
		{
			internal string Id;
			
			public PadCheckMenuItem (string label, string id): base (label) {
				Id = id;
			}
		}

		class TreeBuilderContext: ITreeBuilderContext
		{
			TreeViewPad pad;
			Hashtable icons = new Hashtable ();
			Hashtable composedIcons = new Hashtable ();
			
			internal TreeBuilderContext (TreeViewPad pad)
			{
				this.pad = pad;
			}
			
			public ITreeBuilder GetTreeBuilder ()
			{
				Gtk.TreeIter iter;
				if (!pad.store.GetIterFirst (out iter)) return null;
				return new TreeBuilder (pad, iter);
			}
			
			public ITreeBuilder GetTreeBuilder (object dataObject)
			{
				object pos = pad.nodeHash [dataObject];
				if (pos == null) return null;
				return new TreeBuilder (pad, (Gtk.TreeIter) pos);
			}
			
			public Gdk.Pixbuf GetIcon (string id)
			{
				Gdk.Pixbuf icon = icons [id] as Gdk.Pixbuf;
				if (icon == null) {
					icon = pad.tree.RenderIcon (id, Gtk.IconSize.Menu, "");
					icons [id] = icon;
				}
				return icon;
			}
			
			public Gdk.Pixbuf GetComposedIcon (Gdk.Pixbuf baseIcon, string compositionId)
			{
				Hashtable itable = composedIcons [baseIcon] as Hashtable;
				if (itable == null) return null;
				return itable [compositionId] as Gdk.Pixbuf;
			}
			
			public void CacheComposedIcon (Gdk.Pixbuf baseIcon, string compositionId, Gdk.Pixbuf composedIcon)
			{
				Hashtable itable = composedIcons [baseIcon] as Hashtable;
				if (itable == null) {
					itable = new Hashtable ();
					composedIcons [baseIcon] = itable;
				}
				itable [compositionId] = composedIcon;
			}
			
			public ITreeNavigator GetTreeNavigator (object dataObject)
			{
				object pos = pad.nodeHash [dataObject];
				if (pos == null) return null;
				return new TreeNodeNavigator (pad, (Gtk.TreeIter) pos);
			}
			
			public ITreeOptions GetOptions (object dataObject)
			{
				if (dataObject == null) return pad.globalOptions;
				object pos = pad.nodeHash [dataObject];
				if (pos == null) return pad.globalOptions;
				return new TreeNodeNavigator (pad, (Gtk.TreeIter) pos);
			}
		}
		
		class TreeNodeNavigator: ITreeNavigator, ITreeOptions
		{
			protected TreeViewPad pad;
			protected Gtk.TreeView tree;
			protected Gtk.TreeStore store;
			protected Gtk.TreeIter currentIter;
			
			public TreeNodeNavigator (TreeViewPad pad): this (pad, Gtk.TreeIter.Zero)
			{
			}
			
			public TreeNodeNavigator (TreeViewPad pad, Gtk.TreeIter iter)
			{
				this.pad = pad;
				tree = pad.tree;
				store = pad.store;
				currentIter = iter;
			}
			
			public ITreeNavigator Clone ()
			{
				return new TreeNodeNavigator (pad, currentIter);
			}

			
			public object DataItem {
				get { return store.GetValue (currentIter, TreeViewPad.DataItemColumn); }
			}
			
			public bool Selected {
				get {
					return tree.Selection.IterIsSelected (currentIter);
				}
				set {
					if (value) {
						Gtk.TreeIter parent = currentIter;
						while (store.IterParent (out parent, parent)) {
							Gtk.TreePath path = store.GetPath (parent);
							tree.ExpandRow (path, false);
						}
				
						tree.Selection.SelectIter (currentIter);
						tree.ScrollToCell (store.GetPath (currentIter), null, false, 0, 0);
					}
				}
			}
			
			public NodePosition CurrentPosition {
				get {
					NodePosition pos = new NodePosition ();
					pos._iter = currentIter;
					return pos;
				}
			}
		
			bool ITreeOptions.this [string name] {
				get { return pad.GetOptions (currentIter, false) [name]; }
				set { pad.GetOptions (currentIter, true) [name] = value; }
			}
			
			public bool MoveToPosition (NodePosition position)
			{
				currentIter = (Gtk.TreeIter) position._iter;
				return true;
			}
			
			public bool MoveToRoot ()
			{
				return store.GetIterFirst (out currentIter);
			}
		
			public bool MoveToObject (object dataObject)
			{
				object pos = pad.nodeHash [dataObject];
				if (pos != null) {
					currentIter = (Gtk.TreeIter) pos;
					return true;
				} else
					return false;
			}
		
			public bool MoveToParent ()
			{
				return store.IterParent (out currentIter, currentIter);
			}
			
			public bool MoveToParent (Type dataType)
			{
				Gtk.TreeIter newIter = currentIter;
				while (store.IterParent (out newIter, newIter)) {
					object data = store.GetValue (newIter, TreeViewPad.DataItemColumn);
					if (dataType.IsInstanceOfType (data)) {
						currentIter = newIter;
						return true;
					}
				}
				return false;
			}
			
			public bool MoveToFirstChild ()
			{
				EnsureFilled ();
				Gtk.TreeIter it;
				if (!store.IterChildren (out it, currentIter))
					return false;
				
				currentIter = it;
				return true;
			}
			
			public bool MoveNext ()
			{
				return store.IterNext (ref currentIter);
			}
			
			public bool HasChild (string name, Type dataType)
			{
				if (MoveToChild (name, dataType)) {
					MoveToParent ();
					return true;
				} else
					return false;
			}
			
			public bool HasChildren ()
			{
				EnsureFilled ();
				Gtk.TreeIter it;
				return store.IterChildren (out it, currentIter);
			}
		
			public bool MoveToChild (string name, Type dataType)
			{
				EnsureFilled ();
				Gtk.TreeIter oldIter = currentIter;

				if (!MoveToFirstChild ()) {
					currentIter = oldIter;
					return false;
				}

				do {
					if (name == NodeName) return true;
				} while (MoveNext ());

				currentIter = oldIter;
				return false;
			}
			
			public bool Expanded {
				get { return tree.GetRowExpanded (store.GetPath (currentIter)); }
				set {
					if (value && !Expanded) {
						Gtk.TreePath path = store.GetPath (currentIter);
						tree.ExpandRow (path, false);
					}
					else if (!value && Expanded) {
						Gtk.TreePath path = store.GetPath (currentIter);
						tree.CollapseRow (path);
					}
				}
			}

			public ITreeOptions Options {
				get { return this; }
			}
		
			public void ExpandToNode ()
			{
				Gtk.TreePath path = store.GetPath (currentIter);
				tree.ExpandToPath (path);
			}
			
			public string NodeName {
				get {
					object data = DataItem;
					NodeBuilder[] chain = BuilderChain;
					if (chain != null && chain.Length > 0) return ((TypeNodeBuilder)chain[0]).GetNodeName (data);
					else return store.GetValue (currentIter, TreeViewPad.TextColumn) as string;;
				}
			}
			
			public NodeBuilder[] BuilderChain {
				get { return (NodeBuilder[]) store.GetValue (currentIter, TreeViewPad.BuilderChainColumn); }
			}
			
			public object GetParentDataItem (Type type, bool includeCurrent)
			{
				if (includeCurrent && type.IsInstanceOfType (DataItem))
					return DataItem;

				Gtk.TreeIter it = currentIter;
				while (store.IterParent (out it, it)) {
					object data = store.GetValue (it, TreeViewPad.DataItemColumn);
					if (type.IsInstanceOfType (data))
						return data;
				}
				return null;
			}
		
			void EnsureFilled ()
			{
				if (!(bool) store.GetValue (currentIter, TreeViewPad.FilledColumn))
					new TreeBuilder (pad, currentIter).FillNode ();
			}
			
			public bool Filled {
				get { return (bool) store.GetValue (currentIter, TreeViewPad.FilledColumn); }
			}
		}
		
		
		class TreeBuilder: TreeNodeNavigator, ITreeBuilder
		{
			public TreeBuilder (TreeViewPad pad): base (pad)
			{
			}

			public TreeBuilder (TreeViewPad pad, Gtk.TreeIter iter): base (pad, iter)
			{
			}
			
			void EnsureFilled ()
			{
				if (!(bool) store.GetValue (currentIter, TreeViewPad.FilledColumn))
					FillNode ();
			}
			
			public bool FillNode ()
			{
				store.SetValue (currentIter, TreeViewPad.FilledColumn, true);
				
				Gtk.TreeIter child;
				if (store.IterChildren (out child, currentIter))
					store.Remove (ref child);
				
				NodeBuilder[] chain = (NodeBuilder[]) store.GetValue (currentIter, TreeViewPad.BuilderChainColumn);
				object dataObject = store.GetValue (currentIter, TreeViewPad.DataItemColumn);
				CreateChildren (chain, dataObject);
				return store.IterHasChild (currentIter);
			}
			
			public void Update ()
			{
				object data = store.GetValue (currentIter, TreeViewPad.DataItemColumn);
				NodeBuilder[] chain = (NodeBuilder[]) store.GetValue (currentIter, TreeViewPad.BuilderChainColumn);
				UpdateNode (chain, data);
			}
			
			public void UpdateChildren ()
			{
				object data = store.GetValue (currentIter, TreeViewPad.DataItemColumn);
				NodeBuilder[] chain = (NodeBuilder[]) store.GetValue (currentIter, TreeViewPad.BuilderChainColumn);
				
				if (!(bool) store.GetValue (currentIter, TreeViewPad.FilledColumn)) {
					if (!HasChildNodes (chain, data))
						FillNode ();
					return;
				}
				
				if (!tree.GetRowExpanded (store.GetPath (currentIter))) {
					if (!HasChildNodes (chain, data))
						FillNode ();
					else {
						RemoveChildren (currentIter);
						store.Append (currentIter);	// Dummy node
						store.SetValue (currentIter, TreeViewPad.FilledColumn, false);
					}
					return;
				}

				ArrayList state = new ArrayList ();
				SaveExpandState (currentIter, state);
				RemoveChildren (currentIter);
				CreateChildren (chain, data);
				RestoreExpandState (state);
			}
			
			void RemoveChildren (Gtk.TreeIter it)
			{
				Gtk.TreeIter child;
				while (store.IterChildren (out child, it)) {
					RemoveChildren (child);
					object childData = store.GetValue (child, TreeViewPad.DataItemColumn);
					if (childData != null)
						pad.UnregisterNode (childData, null);
					store.Remove (ref child);
				}
			}
			
			void SaveExpandState (Gtk.TreeIter it, ArrayList state)
			{
				if (tree.GetRowExpanded (store.GetPath (it)))
					state.Add (store.GetValue (it, TreeViewPad.DataItemColumn));
				if (store.IterChildren (out it, it)) {
					do {
						SaveExpandState (it, state);
					} while (store.IterNext (ref it));
				}
			}
			
			void RestoreExpandState (ArrayList state)
			{
				foreach (object ob in state) {
					object pos = pad.nodeHash [ob];
					if (pos == null) continue;
					
					Gtk.TreeIter it = (Gtk.TreeIter) pos;
					Gtk.TreePath p = store.GetPath (it);
					tree.ExpandRow (p, false);
				}
			}
			
			public void Remove ()
			{
				RemoveChildren (currentIter);
				object data = store.GetValue (currentIter, TreeViewPad.DataItemColumn);
				pad.UnregisterNode (data, null);
				store.Remove (ref currentIter);
			}
			
			public void Remove (bool moveToParent)
			{
				Gtk.TreeIter parent;
				store.IterParent (out parent, currentIter);

				Remove ();

				if (moveToParent)
					currentIter = parent;
			}
			
			public void AddChild (object dataObject)
			{
				AddChild (dataObject, false);
			}
			
			public void AddChild (object dataObject, bool moveToChild)
			{
				if (dataObject == null) throw new ArgumentNullException ("dataObject");
				
				if (!currentIter.Equals (Gtk.TreeIter.Zero))
					EnsureFilled ();
					
				if (pad.IsRegistered (dataObject)) return;

				NodeBuilder[] chain = pad.GetBuilderChain (dataObject.GetType ());
				if (chain == null) return;
				
				Gtk.TreeIter oldIter = currentIter;
				NodeAttributes ats = NodeAttributes.None;
				
				foreach (NodeBuilder nb in chain) {
					nb.GetNodeAttributes (this, dataObject, ref ats);
					currentIter = oldIter;
				}
				
				if ((ats & NodeAttributes.Hidden) != 0)
					return;
					
				Gtk.TreeIter it;
				if (!currentIter.Equals (Gtk.TreeIter.Zero))
					it = store.Append (currentIter);
				else
					store.Append (out it);
				
				pad.RegisterNode (it, dataObject, chain);
				
				BuildNode (it, chain, dataObject);
				if (moveToChild)
					currentIter = it;
				else
					currentIter = oldIter;

				pad.NotifyInserted (it, dataObject);
			}
			
			void BuildNode (Gtk.TreeIter it, NodeBuilder[] chain, object dataObject)
			{
				Gtk.TreeIter oldIter = currentIter;
				currentIter = it;
				
				// It is *critical* that we set this first. We will
				// sort after this call, so we must give as much info
				// to the sort function as possible.
				store.SetValue (it, TreeViewPad.DataItemColumn, dataObject);
				store.SetValue (it, TreeViewPad.BuilderChainColumn, chain);
				
				UpdateNode (chain, dataObject);
				
				bool hasChildren = HasChildNodes (chain, dataObject);
				store.SetValue (currentIter, TreeViewPad.FilledColumn, !hasChildren);

				if (hasChildren)
					store.Append (currentIter);	// Dummy node

				currentIter = oldIter;
			}
			
			bool HasChildNodes (NodeBuilder[] chain, object dataObject)
			{
				Gtk.TreeIter citer = currentIter;
				foreach (NodeBuilder nb in chain) {
					bool res = nb.HasChildNodes (this, dataObject);
					currentIter = citer;
					if (res) return true;
				}
				return false;
			}
			
			void UpdateNode (NodeBuilder[] chain, object dataObject)
			{
				Gdk.Pixbuf icon = null;
				Gdk.Pixbuf closedIcon = null;
				string text = string.Empty;
				Gtk.TreeIter citer = currentIter;
				
				foreach (NodeBuilder builder in chain) {
					builder.BuildNode (this, dataObject, ref text, ref icon, ref closedIcon);
					currentIter = citer;
				}
					
				if (closedIcon == null) closedIcon = icon;
				
				SetNodeInfo (currentIter, text, icon, closedIcon);
			}
			
			void SetNodeInfo (Gtk.TreeIter it, string text, Gdk.Pixbuf icon, Gdk.Pixbuf closedIcon)
			{
				store.SetValue (it, TreeViewPad.TextColumn, text);
				if (icon != null) store.SetValue (it, TreeViewPad.OpenIconColumn, icon);
				if (closedIcon != null) store.SetValue (it, TreeViewPad.ClosedIconColumn, closedIcon);
			}

			void CreateChildren (NodeBuilder[] chain, object dataObject)
			{
				Gtk.TreeIter it = currentIter;
				foreach (NodeBuilder builder in chain) {
					builder.BuildChildNodes (this, dataObject);
					currentIter = it;
				}
			}
		}
		
		class TreeOptions: Hashtable, ITreeOptions
		{
			TreeViewPad pad;
			object dataObject;
			
			public TreeOptions ()
			{
			}
			
			public TreeOptions (TreeViewPad pad, object dataObject)
			{
				this.pad = pad;
				this.dataObject = dataObject;
			}
			
			public TreeViewPad Pad {
				get { return pad; }
				set { pad = value; }
			}

			public bool this [string name] {
				get {
					object op = base [name];
					if (op == null) return false;
					return (bool) op;
				}
				set {
					base [name] = value;
					if (pad != null)
						pad.RefreshNode (dataObject);
				}
			}
			
			public TreeOptions CloneOptions (object newDataObject)
			{
				TreeOptions ops = new TreeOptions (pad, null);
				ops.pad = pad;
				ops.dataObject = newDataObject;
				foreach (DictionaryEntry de in this)
					ops [de.Key] = de.Value;
				return ops;
			}
		}
	}
	
	public class TreeViewPadMemento : IXmlConvertable
	{
		TreeViewPad treeView = null;
		XmlElement parent = null;
		
		public TreeViewPadMemento()
		{
		}
		
		public TreeViewPadMemento (TreeViewPad treeView)
		{
			this.treeView = treeView;
		}
		
		public void Restore (TreeViewPad view)
		{
			XmlElement rootNode = parent ["Node"];
			if (rootNode != null) {
				view.RestoreTreeState (rootNode);
			}
		}
		
		public object FromXmlElement (XmlElement element)
		{
			this.parent = element;
			return this;
		}
		
		public XmlElement ToXmlElement (XmlDocument doc)
		{
			Debug.Assert(treeView != null);
			
			XmlElement treenode  = doc.CreateElement ("TreeView");
			treeView.SaveTreeState (treenode);
			return treenode;
		}
	}
	
	public delegate void TreeNodeCallback (ITreeNavigator nav);
}
