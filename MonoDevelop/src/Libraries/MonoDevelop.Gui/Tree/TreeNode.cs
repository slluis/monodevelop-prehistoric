using System;
using System.Collections;

namespace MonoDevelop.Gui {
	public class TreeNode {
		internal TreeView treeView = null;
		internal TreeNode parent = null;
		private TreeNodeCollection nodes;
		private string text;
		private Gdk.Pixbuf image, opened_image, closed_image;
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
				UpdateBacking ();
			}
		}
		
		public Gdk.Pixbuf Image {
			get {
				if (image == null)
					return closed_image;
				return image;
			}
			set {
				image = value;
				UpdateBacking ();
			}
		}
		
		public Gdk.Pixbuf OpenedImage {
			get {
				return opened_image != null ? opened_image : image;
			}
			set {
				opened_image = value;
				UpdateBacking ();
			}
		}
		
		public Gdk.Pixbuf ClosedImage {
			get {
				return closed_image != null ? closed_image : image;
			}
			set {
				closed_image = value;
				UpdateBacking ();
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
		
		public void UpdateBacking ()
		{
			if (TreeView == null)
				return;
			
			TreeView.Model.SetValue (TreeIter, 0, text);
			if (image != null)        TreeView.Model.SetValue (TreeIter, 1, image);
			if (opened_image != null) TreeView.Model.SetValue (TreeIter, 3, opened_image);
			if (closed_image != null) TreeView.Model.SetValue (TreeIter, 4, closed_image);
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
			if (parent != null) {
				parent.Nodes.Remove(this);
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
					throw new Exception("Error calculating iter for path " + TreePath);
				}
				return iter;
			}
		}
		
		public virtual void BeginEdit ()
		{
			TreeView.text_render.Editable = TreeView.canEdit;
			TreeView.SetCursor (new Gtk.TreePath (TreePath), TreeView.complete_column, true);
			TreeView.GrabFocus ();
		}
		
		public void Sort ()
		{
			Nodes.Sort ();
		}
		public void Sort (IComparer c)
		{
			Nodes.Sort (c);
		}
	}
}
