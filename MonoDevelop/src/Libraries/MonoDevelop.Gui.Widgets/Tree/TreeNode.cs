using System;
using System.Collections;

namespace MonoDevelop.Gui.Widgets {
	public class TreeNode {
		internal TreeView treeView = null;
		internal TreeNode parent = null;
		private TreeNodeCollection nodes;
		private string text;
		private string image, opened_image, closed_image;
		private object tag;
		
		internal Gtk.TreeRowReference row;
		
		public TreeNode ()
		{
			nodes = new TreeNodeCollection();
			nodes.TreeNodeCollectionChanged += new TreeNodeCollectionChangedHandler(OnNodesChanged);
			nodes.NodeInserted += new NodeInsertedHandler(OnNodeInserted);
			nodes.NodeRemoved += new NodeRemovedHandler(OnNodeRemoved);		
		}
				
		public TreeNode(string text, string image) : this()
		{
			this.text = text;
			this.image = image;
		}
		
		public TreeNode (string text): this (text, null) {}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				UpdateBacking ();
			}
		}
		
		public string Image {
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
		
		public string OpenedImage {
			get {
				return opened_image;
			}
			set {
				opened_image = value;
				UpdateBacking ();
			}
		}
		
		public string ClosedImage {
			get {
				return closed_image;
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
				return TreeView != null && TreeView.GetRowExpanded (TreePath);
			}
			set {
				if (TreeView != null)
					TreeView.ExpandRow (TreePath, value);
			}
		}
		
		public void UpdateBacking ()
		{
			if (TreeView == null)
				return;
			
			TreeView.Model.SetValue (TreeIter, 0, text);
			if (image != null)        TreeView.Model.SetValue (TreeIter, 1, image);
			if (opened_image != null) TreeView.Model.SetValue (TreeIter, 3, TreeView.RenderIcon (opened_image));
			if (closed_image != null) TreeView.Model.SetValue (TreeIter, 4, TreeView.RenderIcon (closed_image));
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
				if (treeView != null)
					return treeView;
				
				if (parent == null)
					return null;
				
				return parent.TreeView;
			}
		}

		public bool IsEditing {
			get {
				return false; // FIXME
			}
		}
		
		public void Expand ()
		{
			if (TreeView != null)
				TreeView.ExpandToPath (TreePath);
		}
		
		public void EnsureVisible ()
		{
			Expand (); // TODO
		}
		
		public void Remove ()
		{
			if (parent != null)
				parent.Nodes.Remove(this);
		}

		internal void SetTreeView (TreeView t)
		{
			this.treeView = t;
		}
		
		private void OnNodeInserted (TreeNode node)
		{
			node.parent = this;
			if (TreeView != null)
				TreeView.AddNode (this, node);
		}

		private void OnNodeRemoved(TreeNode node)
		{
			if (TreeView != null) {
				TreeView.RemoveNode(node);
			}
			
			node.parent = null;
		}
		
		private void OnNodesChanged ()
		{
			if (TreeView != null)
				TreeView.OnTreeChanged();
		}
		
		internal Gtk.TreeIter TreeIter {
			get {
				Gtk.TreeIter iter;
				if (! TreeView.Model.GetIter (out iter, TreePath))
					throw new Exception("Error calculating iter for " + this.Text);
				
				return iter;
			}
		}
		
		internal Gtk.TreePath TreePath {
			get {
				if (TreeView == null)
					return null;
				
				if (row == null || ! row.Valid ())
					throw new Exception ("RowReference not valid " + this.Text);

				return row.Path;
			}
		}
		
		public virtual void BeginEdit ()
		{
			TreeView.text_render.Editable = TreeView.canEdit;
			TreeView.SetCursor (TreePath, TreeView.complete_column, true);
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
