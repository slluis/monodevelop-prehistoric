using System;
using System.Collections;

using MonoDevelop.Services;

namespace MonoDevelop.Gui.Widgets {
    public class TreeNodeCollection: IList {
		private ArrayList list;

		public int Count {
			get { return list != null ? list.Count : 0; }
		}

		public bool IsReadOnly {
			get { return false; }
		}
		
		public virtual TreeNode this[int index] {
			get {
				if (list == null) throw new ArgumentOutOfRangeException( "index" );
				return (TreeNode) list[index];
			}
			set {
				if (list == null) list = new ArrayList ();
				list[index] = value; //TODO
				Changed();
			}
		}
		
		public void Sort ()
		{
			if (list == null) return;
			list.Sort ();
			Changed ();
		}
		
		public void Sort (IComparer c)
		{
			if (list == null) return;
			list.Sort (c);
			Changed ();
		}
		
		public virtual TreeNode Add(string text) 
		{
			TreeNode node =  new TreeNode (text, null);
			Add (node);
			return node;
		}

		public virtual int Add(TreeNode node) 
		{
			if (node == null)
				throw new ArgumentNullException("value");

			if (list == null) list = new ArrayList ();
			Inserted(node);
			int index = list.Add(node);
			return 	index;		
		}

		public virtual void AddRange(TreeNode[] nodes) 
		{
			if (nodes == null)
				throw new ArgumentNullException("nodes");

			foreach (TreeNode node in nodes) {
				Add (node);
			}
		}

		public virtual void Clear() 
		{
			if (list == null) return;
			ArrayList tmp = (ArrayList)list.Clone();
			foreach (TreeNode node in tmp) {
				Remove(node);
			}
		}
		
		public bool Contains(TreeNode node) 
		{
			if (list == null) return false;
			return list.Contains(node);
		}

		public void CopyTo(Array dest, int index) 
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator() 
		{
			if (list == null) return Type.EmptyTypes.GetEnumerator ();
			return list.GetEnumerator();
		}

		public int IndexOf(TreeNode node) 
		{
			if (list == null) return -1;
			return list.IndexOf(node);
		}

		public virtual void Insert(int index, TreeNode node) 
		{
			if (list == null) list = new ArrayList ();

			if ( node == null )
				throw new ArgumentNullException ( "node" );

			if (index < 0 || index > Count )
				throw new ArgumentOutOfRangeException( "index" );

			list.Insert(index, node);
			Inserted(node);
		}

		public void Remove(TreeNode node) 
		{
			if ( node == null )
				throw new ArgumentNullException( "node" );
			if (list == null) return;
			Removed(node);
			list.Remove(node);
		}

		public virtual void RemoveAt(int index) 
		{
			if (list == null || index < 0 || index > Count )
				throw new ArgumentOutOfRangeException( "index" );

			TreeNode node = (TreeNode) list[ index ];
			Removed(node);
			list.RemoveAt(index);
		}

		bool IList.IsReadOnly{
			get{
				return false;
			}
		}
		bool IList.IsFixedSize{
			get{
				return false;
			}
		}

		object IList.this[int index]{
			get{
				if (list == null) throw new ArgumentOutOfRangeException( "index" );
				return list[index];
			}
			set{
				throw new NotImplementedException();
			}
		}

		void IList.Clear(){
			Clear();
		}
		
		int IList.Add(object value){
			return Add((TreeNode) value);
		}

		bool IList.Contains(object value){
			if (list == null) return false;
			return list.Contains(value);
		}

		int IList.IndexOf(object value){
			if (list == null) return -1;
			return list.IndexOf(value);
		}

		void IList.Insert(int index, object value){
			Insert(index, (TreeNode) value);
		}

		void IList.Remove(object value){
			if (value is TreeNode == false) {
				throw new Exception (GettextCatalog.GetString ("Attempt to remove a non node from the tree"));
			}
			Remove((TreeNode)value);
		}

		void IList.RemoveAt(int index){
			RemoveAt(index);
		}

		object ICollection.SyncRoot{
			get{
				throw new NotImplementedException ();
			}
		}
		
		bool ICollection.IsSynchronized{
			get {
				throw new NotImplementedException();
			}
		}

		private void Inserted(TreeNode node) {
			if (NodeInserted != null) {
				NodeInserted(node);
			}
		}
		
		private void Removed(TreeNode node) {
			if (NodeRemoved != null) {
				NodeRemoved(node);
			}
		}
		
		private void Changed() {
			if (TreeNodeCollectionChanged != null) {
				TreeNodeCollectionChanged();
			}
        }
                                                                                                                             
		internal event TreeNodeCollectionChangedHandler TreeNodeCollectionChanged;
		internal event NodeInsertedHandler NodeInserted;
		internal event NodeRemovedHandler NodeRemoved;
    }

	internal delegate void TreeNodeCollectionChangedHandler();
	internal delegate void NodeInsertedHandler(TreeNode node);
	internal delegate void NodeRemovedHandler(TreeNode node);
}
