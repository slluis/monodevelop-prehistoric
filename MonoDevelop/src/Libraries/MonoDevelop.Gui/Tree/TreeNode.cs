using System;
using System.Collections;

namespace MonoDevelop.Gui {
	public class TreeNode {
		internal TreeView treeView = null;
		internal TreeNode parent = null;
		private TreeNodeCollection nodes;
		private string text;
		private Gdk.Pixbuf image;
		private object tag;
		
		public TreeNode() {
			nodes = new TreeNodeCollection();
			nodes.TreeNodeCollectionChanged += new TreeNodeCollectionChangedHandler(OnNodesChanged);
			nodes.NodeInserted += new NodeInsertedHandler(OnNodeInserted);
			nodes.NodeRemoved += new NodeRemovedHandler(OnNodeRemoved);		
		}
				
		public TreeNode(string text, Gdk.Pixbuf image): this() {
			this.text = text;
			this.image = image;
		}
		
		public TreeNode(string text): this(text, null) {}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public Gdk.Pixbuf Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}

		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public bool IsExpanded {
			get {
				if (TreeView != null) {
					return TreeView.GetRowExpanded(new Gtk.TreePath(TreePath));
				} else {
					return false;
				}
			}
			set {
				if (TreeView != null) {
					TreeView.ExpandRow(new Gtk.TreePath(TreePath), value);
				}
			}
		}
		
		public TreeNodeCollection Nodes {
			get {
				return nodes;
			}
		}

		public TreeNode Parent {
			get {
				return parent;
			}
		}
		
		public TreeView TreeView {
			get {
				if (treeView != null) {
					return treeView;
				}
				if (parent == null) {
					return null;
				}
				return parent.TreeView;
			}
		}

		public bool IsEditing {
			get {
				return false; // FIXME
			}
		}
		
		public void Expand() {
			if (TreeView != null) {
				TreeView.ExpandToPath(new Gtk.TreePath(TreePath));
			}
		}
		
		public void EnsureVisible() {
			Expand(); // TODO
		}
		
		public void Remove() {
			if (TreeView != null) {
				TreeView.RemoveNode(this);
			}
		}

		internal void SetTreeView(TreeView t) {
			this.treeView = t;
		}
		
		private void OnNodeInserted(TreeNode node) {
			node.parent = this;
			if (TreeView != null) {
				TreeView.AddNode(this, node);
			}
		}

		private void OnNodeRemoved(TreeNode node) {
			if (TreeView != null) {
				TreeView.RemoveNode(node);
			}
			node.parent = null;
		}
		
		private void OnNodesChanged() {
			if (TreeView != null) {
				TreeView.OnTreeChanged();
			}
		}
		
		private string TreePath {
			get {
				if (parent == null) {
					return "0";
				}

				string ret = parent.TreePath + ":";
				ret += parent.Nodes.IndexOf(this);
				return ret;
			}
		}
		
		internal Gtk.TreeIter TreeIter {
			get {
				Gtk.TreeIter iter;
				if (TreeView.Model.GetIterFromString(out iter, TreePath) == false) {
					throw new Exception("Error calculating iter");
				}
				return iter;
			}
		}
	}
}
