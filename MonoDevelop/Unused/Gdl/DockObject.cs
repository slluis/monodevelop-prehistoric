// project created on 04/06/2004 at 6:37 P
using System;
using System.Xml;
using Gtk;

namespace Gdl
{
	public delegate void PropertyChangedHandler (object o, string name);

	public class DockObject : Container
	{	
		private DockObjectFlags flags = DockObjectFlags.Automatic;
		private int freezeCount = 0;
		private DockMaster master;
		private string name;
		private string longName;
		private string stockid;
		private bool reducePending;
		
		public event DetachedHandler Detached;
		public event DockedHandler Docked;
		public event PropertyChangedHandler PropertyChanged;

		protected DockObject (IntPtr raw) : base (raw) { }
		protected DockObject () : base () { }
		
		public DockObjectFlags DockObjectFlags {
			get {
				return flags;
			}
			set {
				flags = value;
				EmitPropertyEvent ("DockObjectFlags");
			}
		}
		
		public bool InDetach {
			get {
				return ((flags & DockObjectFlags.InDetach) != 0);
			}
		}
		
		public bool InReflow {
			get {
				return ((flags & DockObjectFlags.InReflow) != 0);
			}
		}
		
		public bool IsAttached {
			get {
				return ((flags & DockObjectFlags.Attached) != 0);
			}
		}
		
		public bool IsAutomatic {
			get {
				return ((flags & DockObjectFlags.Automatic) != 0);
			}
		}
		
		public bool IsBound {
			get {
				return master != null;
			}
		}
		
		public virtual bool IsCompound {
			get {
				return true;
			}
		}
		
		public bool IsFrozen {
			get {
				return freezeCount > 0;
			}
		}
		
		public string LongName {
			get {
				return longName;
			}
			set {
				longName = value;
				EmitPropertyEvent ("LongName");
			}
		}
		
		public DockMaster Master {
			get {
				return master;
			}
			set {
				if (value != null)
					Bind (master);
				else
					Unbind ();
				EmitPropertyEvent ("Master");
			}
		}
		
		public new string Name {
			get {
				return name;
			}
			set {
				name = value;
				EmitPropertyEvent ("Name");
			}
		}
		
		public DockObject ParentObject {
			get {
				Widget parent = Parent;
				while (parent != null && !(parent is DockObject)) {
					parent = parent.Parent;
				}
				return parent != null ? (DockObject)parent : null;
			}
		}

		public string StockId {
			get {
				return stockid;
			}
			set {
				stockid = value;
				EmitPropertyEvent ("StockId");
			}
		}

		public virtual void FromXml (XmlNode node)
		{
			Console.WriteLine ("override this: {0}", this.GetType ());
		}

		protected override void OnDestroyed ()
		{
			if (IsCompound) {
				/* detach our dock object children if we have some, and even
				   if we are not attached, so they can get notification */
				Freeze ();
				foreach (DockObject child in Children)
					child.Detach (true);
				reducePending = false;
				Thaw ();
			}

			if (IsAttached)
				/* detach ourselves */
				Detach (false);

			if (Master != null)
				/* finally unbind us */
				Unbind ();

			base.OnDestroyed ();
		}
		
		protected override void OnShown ()
		{
			if (IsCompound)
				foreach (Widget child in Children)
					child.Show ();

			base.OnShown ();
		}
		
		protected override void OnHidden ()
		{
			if (IsCompound)
				foreach (Widget child in Children)
					child.Hide ();

			base.OnHidden ();
		}
		
		public virtual void OnDetached (bool recursive)
		{
			/* detach children */
			if (recursive && IsCompound) {
				foreach (DockObject child in Children) {
					child.Detach (recursive);
				}
			}
			
			/* detach the object itself */
			flags &= ~(DockObjectFlags.Attached);
			DockObject parent = ParentObject;
			if (Parent != null && Parent is Container)
				((Container)Parent).Remove (this);

			if (parent != null)
				parent.Reduce ();
		}
		
		public virtual void OnReduce ()
		{
			if (!IsCompound)
				return;
				
			DockObject parent = ParentObject;
			Widget[] children = Children;
			if (children.Length <= 1) {
				if (parent != null)
					parent.Freeze ();

				Freeze ();
				Detach (false);

				foreach (Widget widget in children) {
					DockObject child = widget as DockObject;
					child.flags |= DockObjectFlags.InReflow;
					child.Detach (false);
					if (parent != null)
						parent.Add (child);
					child.flags &= ~(DockObjectFlags.InReflow);
				}
				
				reducePending = false;

				Thaw ();
				if (parent != null)
					parent.Thaw ();
			}
		}
		
		public virtual bool OnDockRequest (int x, int y, ref DockRequest request)
		{
			return false;
		}
		
		public virtual void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
		}
		
		public virtual bool OnReorder (DockObject child, DockPlacement new_position, object data)
		{
			return false;
		}
		
		public virtual void OnPresent (DockObject child)
		{
			Show ();			
		}
		
		public virtual bool OnChildPlacement (DockObject child, ref DockPlacement placement)
		{
			return false;
		}
		
		public bool ChildPlacement (DockObject child, ref DockPlacement placement)
		{
			if (!IsCompound)
				return false;
			
			return OnChildPlacement (child, ref placement);
		}
		
		public void Detach (bool recursive)
		{
			if (!IsAttached)
				return;
				
			/* freeze the object to avoid reducing while detaching children */
			Freeze ();
			
			DockObjectFlags |= DockObjectFlags.InDetach;
			OnDetached (recursive);
			DetachedHandler handler = Detached;
			if (handler != null)
				handler (this, new DetachedArgs (recursive));
			DockObjectFlags &= ~(DockObjectFlags.InDetach);

			Thaw ();		
		}
		
		public void Dock (DockObject requestor, DockPlacement position, object data)
		{
			if (requestor == null || requestor == this)
				return;
				
			if (master == null) {
				Console.WriteLine ("Dock operation requested in a non-bound object {0}.", this);
				Console.WriteLine ("This might break.");
			}

			if (!requestor.IsBound)
				requestor.Bind (Master);

			if (requestor.Master != Master) {
				Console.WriteLine ("Cannot dock {0} to {1} as they belong to different masters.",
						   requestor, this);
				return;
			}

			/* first, see if we can optimize things by reordering */
			if (position != DockPlacement.None) {
				DockObject parent = ParentObject;
				if (OnReorder (requestor, position, data) ||
				    (parent != null && parent.OnReorder (requestor, position, data)))
					return;
			}

			/* freeze the object, since under some conditions it might
			   be destroyed when detaching the requestor */
			Freeze ();

			/* detach the requestor before docking */
			if (requestor.IsAttached)
				requestor.Detach (false);

			/* notify interested parties that an object has been docked. */
			if (position != DockPlacement.None) {
				OnDocked (requestor, position, data);
				DockedHandler handler = Docked;
				if (handler != null) {
					DockedArgs args = new DockedArgs (requestor, position);
					handler (this, args);
				}
			}
			
			Thaw ();
		}
		
		public void Present (DockObject child)
		{
			if (ParentObject != null)
				/* chain the call to our parent */
				ParentObject.Present (this);
			
			OnPresent (child);
		}

		public void Reduce ()
		{
			if (IsFrozen) {
				reducePending = true;
				return;
			}

			OnReduce ();		
		}

		public void Freeze ()
		{
			freezeCount++;
		}
		
		public void Thaw ()
		{
			if (freezeCount < 0) {
				Console.WriteLine ("DockObject.Thaw: freezeCount < 0");
				return;
			}

			freezeCount--;

			if (freezeCount == 0 && reducePending) {
				reducePending = false;
				Reduce ();
			}
		}
		
		public void Bind (DockMaster master)
		{
			if (master == null) {
				Console.WriteLine ("Passed master is null");
				Console.WriteLine (System.Environment.StackTrace);
				return;
			}
			if (this.master == master) {
				Console.WriteLine ("Passed master is this master");
				return;
			}
			if (this.master != null) {
				Console.WriteLine ("Attempt to bind an already bound object");
				return;
			}
			
			master.Add (this);
			this.master = master;
			EmitPropertyEvent ("Master");
		}
		
		public void Unbind ()
		{
			if (IsAttached)
				Detach (true);

			if (master != null) {
				DockMaster _master = master;
				master = null;
				_master.Remove (this);
				EmitPropertyEvent ("Master");
			}
		}
		
		protected void EmitPropertyEvent (string name)
		{
			// Make a local assignment of the handler here to prevent
			// any race conditions if the PropertyChanged value changes
			// to null after the != null check.
			PropertyChangedHandler handler = PropertyChanged;
			if (handler != null)
				handler (this, name);
		}
	}
}
