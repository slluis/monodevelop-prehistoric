using System;
using System.Collections;

namespace MonoDevelop.Gui {
	public class TreeView: Gtk.TreeView {
		private Gtk.TreeView view;
		private Gtk.TreeStore store;
		private TreeNodeCollection nodes;
		private bool updating = false;
		internal bool canEdit = false;
		internal Gtk.TreeViewColumn complete_column;
		internal Gtk.CellRendererText text_render;
		
		public TreeView () : this (false)
		{
		}
		
		public TreeView(bool canEdit) {
			store = new Gtk.TreeStore(typeof(string), typeof(Gdk.Pixbuf), typeof(TreeNode));
			this.Model = store;
			this.canEdit = canEdit;

			HeadersVisible = false;
			SearchColumn = 0;
			EnableSearch = true;
			complete_column = new Gtk.TreeViewColumn ();
			complete_column.Title = "column";

			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			complete_column.PackStart (pix_render, false);
			complete_column.AddAttribute (pix_render, "pixbuf", 1);
			
			text_render = new Gtk.CellRendererText ();
			if (canEdit) {
				text_render.Edited += new GtkSharp.EditedHandler (HandleOnEdit);
			}
			complete_column.PackStart (text_render, true);
			complete_column.AddAttribute (text_render, "text", 0);
	
			AppendColumn (complete_column);

			nodes = new TreeNodeCollection();
			nodes.TreeNodeCollectionChanged += new TreeNodeCollectionChangedHandler(OnTreeChanged);
			nodes.NodeInserted += new NodeInsertedHandler(OnNodeInserted);
			nodes.NodeRemoved += new NodeRemovedHandler(OnNodeRemoved);
			
			TestExpandRow += new GtkSharp.TestExpandRowHandler(OnTestExpandRow);
		}

		void HandleOnEdit (object o, GtkSharp.EditedArgs e)
		{
			text_render.Editable = false;
			Gtk.TreeIter iter;
			if (! Model.GetIterFromString (out iter, e.Path))
				throw new Exception("Error calculating iter for path " + e.Path);
			
			OnEdit ((TreeNode) store.GetValue (iter, 2), e.NewText);
		}
		
		protected virtual void OnEdit (TreeNode node, string new_text)
		{
		}
		

		public TreeNodeCollection Nodes {
			get {
				return nodes;
			}
		}

		public Gtk.TreeView View {
			get {
				return this;
			}
		}

		public TreeNode SelectedNode {
			get {
				Gtk.TreeModel foo;
				Gtk.TreeIter iter;
				if (Selection.GetSelected(out foo, out iter) == false) {
					return null;
				}
				return (TreeNode) store.GetValue(iter, 2);
			}
			set {
				Gtk.TreeIter iter;
				if (store.GetIterFirst(out iter) == false) {
					return;
				}
				do {
					if (store.GetValue(iter, 2) == value) {
						Selection.SelectIter(iter);
						return;
					} 
				} while (store.IterNext(out iter) == true);
			}
		}
		
		public bool LabelEdit {
			get {
				return canEdit;
			}
			set {
				canEdit = value;
			}
		}
		
		public void BeginUpdate() {
			//updating = true;
		}
		
		public void EndUpdate() {
			//if (updating == true) {
			//	UpdateStore(store);
			//}
			//updating = false;

		}
				
		internal void OnTreeChanged() {
			if (updating == false) {
				UpdateStore(store);
			}
		}
		
        internal void UpdateStore(Gtk.TreeStore store) {
			store.Clear();
			foreach (TreeNode node in nodes) {
				Gtk.TreeIter it = store.AppendValues(node.Text, node.Image, node);
				AddNodesRecursively(store, it, node);
			}
		}
		
		private void AddNodesRecursively(Gtk.TreeStore store, Gtk.TreeIter it, TreeNode node) {
			foreach(TreeNode nod in node.Nodes) {
				Gtk.TreeIter i = store.AppendValues(it, nod.Text, nod.Image, nod);
				AddNodesRecursively(store, i, nod);
			}
		}

		internal void AddNode(TreeNode parent, TreeNode child) {
			if (parent.TreeView != this) {
				throw new Exception("Wrong tree");
			}
			Gtk.TreeIter i = store.AppendValues(parent.TreeIter, child.Text, child.Image, child);
			AddNodesRecursively(store, i, child);
		}
		
		internal void RemoveNode(TreeNode node) {
			if (node.TreeView != this) {
				throw new Exception("Wrong tree");
			}
			Gtk.TreeIter iter = node.TreeIter;
			store.Remove(ref iter);
		}
		
		private void OnNodeInserted(TreeNode node) {
			node.treeView = this;
			node.parent = null;
			Gtk.TreeIter i = store.AppendValues(node.Text, node.Image, node);
			AddNodesRecursively(store, i, node);
		}

		private void OnNodeRemoved(TreeNode node) {
			RemoveNode(node);
			node.parent = null;
			node.treeView = null;
		}
		
		private TreeNode GetNodeByIter(Gtk.TreeIter iter) {
			TreeNode ret = (TreeNode)store.GetValue(iter, 2);
			return ret;
		}
		
		private void OnTestExpandRow(object sender, GtkSharp.TestExpandRowArgs args) {
			TreeNode node = GetNodeByIter(args.Iter);
			TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(node);
			OnBeforeExpand(e);
			if (e.Cancel == true || node.Nodes.Count == 0) {
				args.RetVal = true;
			} else {
				args.RetVal = false;
			}
		}
		
		protected virtual void OnBeforeExpand(TreeViewCancelEventArgs e) {
			// Nothing
		}
	}
	
	public class TreeViewCancelEventArgs {
		private TreeNode node;
		private bool cancel = false;
		
		public TreeViewCancelEventArgs(TreeNode node) {
			this.node = node;
		}
		
		public TreeNode Node {
			get {
				return node;
			}
		}
		
		public bool Cancel {
			get {
				return cancel;
			}
			set {
				cancel = value;
			}
		}
	}
}
