using System;
using System.Collections;

namespace MonoDevelop.Gui {
	public class TreeView: Gtk.TreeView {
		private Gtk.TreeView view;
		private Gtk.TreeStore store;
		private TreeNodeCollection nodes;
		private bool updating = false;
		
		public TreeView() {
			store = new Gtk.TreeStore(typeof(string), typeof(Gdk.Pixbuf), typeof(TreeNode));
			this.Model = store;

			HeadersVisible = false;
			Gtk.TreeViewColumn complete_column = new Gtk.TreeViewColumn ();
			complete_column.Title = "column";

			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			complete_column.PackStart (pix_render, false);
			complete_column.AddAttribute (pix_render, "pixbuf", 1);
			
			Gtk.CellRendererText text_render = new Gtk.CellRendererText ();
			complete_column.PackStart (text_render, true);
			complete_column.AddAttribute (text_render, "text", 0);
	
			AppendColumn (complete_column);

			nodes = new TreeNodeCollection();
			nodes.TreeNodeCollectionChanged += new TreeNodeCollectionChangedHandler(OnTreeChanged);
            nodes.NodeInserted += new NodeInsertedHandler(OnNodeInserted);
			nodes.NodeRemoved += new NodeRemovedHandler(OnNodeRemoved);
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
				throw new NotImplementedException();
			}
		}
		
		public bool LabelEdit {
			get {
				return false;
			}
			set {
				// TODO
			}
		}
		
		public void BeginUpdate() {
//			updating = true;
		}
		
		public void EndUpdate() {
/*			if (updating == true) {
				UpdateStore(store);
			}
			updating = false;
*/
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
	}
}
