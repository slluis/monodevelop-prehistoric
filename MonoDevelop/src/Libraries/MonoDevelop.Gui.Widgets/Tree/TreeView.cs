using System;
using System.Collections;

namespace MonoDevelop.Gui.Widgets {
	public class TreeView: Gtk.TreeView {
		Gtk.TreeView view;
		Gtk.TreeStore store;
		TreeNodeCollection nodes;
		bool updating = false;
		
		internal bool canEdit = false;
		internal Gtk.TreeViewColumn complete_column;
		internal Gtk.CellRendererText text_render;
		
		public TreeView () : this (false)
		{
		}
		
		public TreeView (bool canEdit)
		{
			/*
			0 -- Text
			1 -- Icon
			2 -- Node
			3 -- Expanded Icon
			4 -- Unexpanded Icon
			*/
			store = new Gtk.TreeStore (typeof (string), typeof (Gdk.Pixbuf), typeof (TreeNode), typeof (Gdk.Pixbuf), typeof (Gdk.Pixbuf));
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
			complete_column.AddAttribute (pix_render, "pixbuf-expander-open", 3);
			complete_column.AddAttribute (pix_render, "pixbuf-expander-closed", 4);
			
			text_render = new Gtk.CellRendererText ();
			if (canEdit)
				text_render.Edited += new Gtk.EditedHandler (HandleOnEdit);
			
			complete_column.PackStart (text_render, true);
			complete_column.AddAttribute (text_render, "text", 0);
	
			AppendColumn (complete_column);

			nodes = new TreeNodeCollection ();
			nodes.TreeNodeCollectionChanged += new TreeNodeCollectionChangedHandler (OnTreeChanged);
			nodes.NodeInserted += new NodeInsertedHandler (OnNodeInserted);
			nodes.NodeRemoved += new NodeRemovedHandler (OnNodeRemoved);
			
			TestExpandRow += new Gtk.TestExpandRowHandler (OnTestExpandRow);
		}
		
		public TreeView (bool edit, Gtk.TreeIterCompareFunc cb_compare) : this (edit)
		{
			store.SetDefaultSortFunc (cb_compare, IntPtr.Zero, null);
			store.SetSortColumnId (/* GTK_TREE_SORTABLE_DEFAULT_SORT_COLUMN_ID */ -1, Gtk.SortType.Ascending);
		}

		void HandleOnEdit (object o, Gtk.EditedArgs e)
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
				if (! Selection.GetSelected (out foo, out iter))
					return null;
				
				return (TreeNode) store.GetValue (iter, 2);
			}
			set {			
				Selection.SelectIter (value.TreeIter);
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
		
		public void BeginUpdate() 
		{
			//updating = true;
		}
		
		public void EndUpdate ()
		{
			//if (updating == true) {
			//	UpdateStore(store);
			//}
			//updating = false;

		}
		
		internal void OnTreeChanged ()
		{
			if (updating == false)
				UpdateStore (store);
		}
		
		internal void UpdateStore (Gtk.TreeStore store)
		{
			store.Clear ();
			foreach (TreeNode node in nodes) {
				Gtk.TreeIter i = Append (node);
				AddNodesRecursively (store, i, node);
			}
		}
		
		Gtk.TreeIter Append (Gtk.TreeIter parent, TreeNode new_child)
		{
			if (new_child.row != null) {
				new_child.row.Free ();
				new_child.row = null;
			}
			
			Gtk.TreeIter it = store.Append (parent);
			
			// It is *critical* that we set this first. We will
			// sort after this call, so we must give as much info
			// to the sort function as possible.
			store.SetValue (it, 2, new_child);
			
			if (new_child.Text != null)        store.SetValue (it, 0, new_child.Text);
			if (new_child.Image != null)       store.SetValue (it, 1, new_child.Image);
			if (new_child.OpenedImage != null) store.SetValue (it, 3, new_child.OpenedImage);
			if (new_child.ClosedImage != null) store.SetValue (it, 4, new_child.ClosedImage);
			
			new_child.row = new Gtk.TreeRowReference (store, store.GetPath (it));
			
			return it;
		}
		
		Gtk.TreeIter Append (TreeNode new_child)
		{
			if (new_child.row != null) {
				new_child.row.Free ();
				new_child.row = null;
			}
			
			Gtk.TreeIter it;
			store.Append (out it);
			
			// It is *critical* that we set this first. We will
			// sort after this call, so we must give as much info
			// to the sort function as possible.
			store.SetValue (it, 2, new_child);
			
			if (new_child.Text != null)        store.SetValue (it, 0, new_child.Text);
			if (new_child.Image != null)       store.SetValue (it, 1, new_child.Image);
			if (new_child.OpenedImage != null) store.SetValue (it, 3, new_child.OpenedImage);
			if (new_child.ClosedImage != null) store.SetValue (it, 4, new_child.ClosedImage);
			
			new_child.row = new Gtk.TreeRowReference (store, store.GetPath (it));
			
			return it;
		}
		
		private void AddNodesRecursively (Gtk.TreeStore store, Gtk.TreeIter it, TreeNode node)
		{
			foreach (TreeNode nod in node.Nodes) {
				Gtk.TreeIter i = Append (it, nod);
				AddNodesRecursively (store, i, nod);
			}
		}

		internal void AddNode (TreeNode parent, TreeNode child)
		{
			if (parent.TreeView != this)
				throw new Exception ("Wrong tree");
			
			Gtk.TreeIter i = Append (parent.TreeIter, child);
			
			AddNodesRecursively (store, i, child);
		}
		
		internal void RemoveNode (TreeNode node)
		{
			if (node.TreeView != this)
				throw new Exception ("Wrong tree");
			
			Gtk.TreeIter iter = node.TreeIter;
			store.Remove (ref iter);
		}
		
		private void OnNodeInserted (TreeNode node)
		{
			node.treeView = this;
			node.parent = null;

			
			Gtk.TreeIter i = Append (node);
			AddNodesRecursively(store, i, node);
		}

		private void OnNodeRemoved (TreeNode node)
		{
			RemoveNode (node);
			node.parent = null;
			node.treeView = null;
			if (node.row != null) {
				node.row.Free ();
				node.row = null;
			}
		}
		
		private TreeNode GetNodeByIter (Gtk.TreeIter iter)
		{
			TreeNode ret = (TreeNode)store.GetValue(iter, 2);
			return ret;
		}
		
		private void OnTestExpandRow (object sender, Gtk.TestExpandRowArgs args)
		{
			TreeNode node = GetNodeByIter (args.Iter);
			TreeViewCancelEventArgs e = new TreeViewCancelEventArgs (node);
			OnBeforeExpand (e);
			
			args.RetVal = (e.Cancel == true || node.Nodes.Count == 0);
		}
		
		protected virtual void OnBeforeExpand (TreeViewCancelEventArgs e)
		{
			// Nothing
		}
	}
	
	public class TreeViewCancelEventArgs {
		private TreeNode node;
		private bool cancel = false;
		
		public TreeViewCancelEventArgs (TreeNode node)
		{
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
