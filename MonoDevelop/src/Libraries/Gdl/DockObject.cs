// project created on 04/06/2004 at 6:37 P
using System;
using Gtk;

//FIXME: need to figure out how to handle reduce vmethod vs real_reduce vs
//reduce public api... fucking gobject.

//FIXME: Event emitting doesnt happen, and it needs to.
namespace Gdl
{
	public class DockObject : Container
	{	
		private Container container;
		private DockObjectFlags flags;
		private int freeze_count;
		private DockMaster master;
		private string name;
		private string long_name;
		private string stock_id;
		private bool reduce_pending;
		
		public new string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string LongName {
			get { return long_name; }
			set { long_name = value; }
		}
		
		public string StockId {
			get { return stock_id; }
			set { stock_id = value; }
		}
		
		public DockMaster Master {
			get { return master; }
			set {
				if (value != null)
					Bind (master);
				else
					Unbind ();
			}
		}
		
		protected override void OnShown ()
		{
			if (IsCompound) {
				foreach (Widget child in Children) {
					child.Show ();
				}
			}
			base.OnShown ();
		}
		
		protected override void OnHidden ()
		{
			if (IsCompound) {
				foreach (Widget child in Children) {
					child.Hide ();
				}
			}
			base.OnHidden ();
		}
		
		public virtual void Detach (bool recursive)
		{
			//Detach children
			if (recursive && IsCompound) {
				foreach (DockObject child in Children) {
					child.Detach (recursive);
				}
			}
			//Detach object itself.
			flags &= ~(DockObjectFlags.Attached);
			DockObject parent = ParentObject;
			if (Parent != null && Parent is Container) {
				((Container)Parent).Remove (this);
			}
			if (parent != null)
				parent.Reduce ();
		}
		
		
		public virtual void Reduce ()
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
					if (child == null) continue;
					child.flags |= DockObjectFlags.InReflow;
					child.Detach (false);
					if (parent != null)
						parent.Add (child);
					child.flags &= ~(DockObjectFlags.InReflow);
				}
				reduce_pending = false;
				Thaw ();
				if (parent != null)
					parent.Thaw ();
			}
		}
		
		public virtual bool DockRequest (int x, int y, DockRequest request)
		{
			return false;
		}
		
		public virtual void Docking (DockObject requestor, DockPlacement position, object other_data)
		{
			DockObject parent;
			if (requestor == null)
				return;
				
			if (requestor == this)
				return;
			
			if (master == null) {
				Console.WriteLine ("Dock operation requested in a non-bound object.");
				Console.WriteLine ("This might break.");
			}
			if (!requestor.IsBound)
				requestor.Bind (master);
			if (requestor.Master != master) {
				Console.WriteLine ("Cannot complete dock as they belong to different masters.");
				return;
			}
			//Attempt to optimize the placement with reordering (heh)
			if (position != DockPlacement.None) {
				if (Reorder (requestor, position, other_data) || (ParentObject != null && ParentObject.Reorder (requestor, position, other_data)))
					return;
			}
			Freeze ();
			if (requestor.IsAttached)
				requestor.Detach (false);
			if (position != DockPlacement.None) {
				/*FIXME: port this code: 
				g_signal_emit (object, gdl_dock_object_signals [DOCK], 0,
				requestor, position, other_data);
				*/
			}
			Thaw ();
		}
		
		public virtual bool Reorder (DockObject child, DockPlacement new_position, object other_data)
		{
			return false;
		}
		
		public virtual void Present (DockObject child)
		{
			Show ();
		}
		
		public virtual bool ChildPlacement (DockObject child, ref DockPlacement placement)
		{
			return false;
		}
		
		public virtual bool IsCompound {
			get {
				return true;
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
		
		public bool InReflow {
			get {
				return ((flags & DockObjectFlags.InReflow) != 0);
			}
		}
		
		public void Freeze ()
		{
			freeze_count++;
		}
		
		public void Thaw ()
		{
			freeze_count--;
			if (freeze_count == 0 && reduce_pending) {
				reduce_pending = false;
				Reduce ();
			}
		}
		
		public void Bind (DockMaster _master)
		{
			Console.WriteLine ("About to attempt a bind");
			if (_master == null) {
				Console.WriteLine ("passed master is null");
				return;
			}
			if (master == _master) {
				Console.WriteLine ("passed master is this master");
				return;
			}
			if (master != null) {
				Console.WriteLine ("Attempt to bind an already bound object");
				return;
			}
			_master.Add (this);
			master = _master;
			//g_object_notify (G_OBJECT (object) /*this*/, "master");
		}
		
		public void Unbind ()
		{
			if (IsAttached)
				Detach (true);
			if (master != null) {
				DockMaster _master = master;
				master = null;
				_master.Remove (this);
				//g_object_notify (G_OBJECT (object) /*this*/, "master");
			}
		}
		
		public bool IsBound {
			get {
				return master != null;
			}
		}
		
		public DockObjectFlags DockObjectFlags {
			get { return flags; }
			set { flags = value; }
		}
	}
}
