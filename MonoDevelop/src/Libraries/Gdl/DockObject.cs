// project created on 04/06/2004 at 6:37 P
using System;
using Gtk;

//FIXME: need to figure out how to handle reduce vmethod vs real_reduce vs
//reduce public api... fucking gobject.

//FIXME: Event emitting doesnt happen, and it needs to.
namespace Gdl
{
	public class DockObject : Gtk.Container
	{	
		private Gdl.DockObjectFlags flags;
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
					this.Bind (master);
				else
					this.Unbind ();
			}
		}
		
		protected override void OnShown ()
		{
			if (this.IsCompound) {
				foreach (Gtk.Widget child in this.Children) {
					child.Show ();
				}
			}
			base.OnShown ();
		}
		
		protected override void OnHidden ()
		{
			if (this.IsCompound) {
				foreach (Gtk.Widget child in this.Children) {
					child.Hide ();
				}
			}
			base.OnHidden ();
		}
		
		public virtual void Detach (bool recursive)
		{
			//Detach children
			if (recursive && this.IsCompound) {
				foreach (DockObject child in this.Children) {
					child.Detach (recursive);
				}
			}
			//Detach object itself.
			this.flags &= ~(DockObjectFlags.Attached);
			DockObject parent = this.ParentObject;
			if (this.Parent != null && this.Parent is Gtk.Container) {
				((Gtk.Container)this.Parent).Remove (this);
			}
			if (parent != null)
				parent.Reduce ();
		}
		
		
		public virtual void Reduce ()
		{
			if (!this.IsCompound)
				return;
				
			Gdl.DockObject parent = this.ParentObject;
			Gtk.Widget[] children = this.Children;
			if (children.Length <= 1) {
				if (parent != null)
					parent.Freeze ();
				this.Freeze ();
				this.Detach (false);
				foreach (Gtk.Widget widget in children) {
					Gdl.DockObject child = widget as Gdl.DockObject;
					if (child == null) continue;
					child.flags |= Gdl.DockObjectFlags.InReflow;
					child.Detach (false);
					if (parent != null)
						parent.Add (child);
					child.flags &= ~(Gdl.DockObjectFlags.InReflow);
				}
				reduce_pending = false;
				this.Thaw ();
				if (parent != null)
					parent.Thaw ();
			}
		}
		
		public virtual bool DockRequest (int x, int y, DockRequest request)
		{
			return false;
		}
		
		public virtual void Docking (Gdl.DockObject requestor, DockPlacement position, object other_data)
		{
			Gdl.DockObject parent;
			if (requestor == null)
				return;
				
			if (requestor == this)
				return;
			
			if (this.master == null) {
				Console.WriteLine ("Dock operation requested in a non-bound object.");
				Console.WriteLine ("This might break.");
			}
			if (!requestor.IsBound)
				requestor.Bind (this.master);
			if (requestor.Master != this.master) {
				Console.WriteLine ("Cannot complete dock as they belong to different masters.");
				return;
			}
			//Attempt to optimize the placement with reordering (heh)
			if (position != DockPlacement.None) {
				if (this.Reorder (requestor, position, other_data) || (this.ParentObject != null && this.ParentObject.Reorder (requestor, position, other_data)))
					return;
			}
			this.Freeze ();
			if (requestor.IsAttached)
				requestor.Detach (false);
			if (position != Gdl.DockPlacement.None) {
				/*FIXME: port this code: 
				g_signal_emit (object, gdl_dock_object_signals [DOCK], 0,
				requestor, position, other_data);
				*/
			}
			this.Thaw ();
		}
		
		public virtual bool Reorder (DockObject child, DockPlacement new_position, object other_data)
		{
			return false;
		}
		
		public virtual void Present (DockObject child)
		{
			this.Show ();
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
				Widget parent = this.Parent;
				while (parent != null && !(parent is DockObject)) {
					parent = parent.Parent;
				}
				return parent != null ? (DockObject)parent : null;
			}
		}
		
		public bool IsAttached {
			get {
				return ((this.flags & Gdl.DockObjectFlags.Attached) != 0);
			}
		}
		
		public bool IsAutomatic {
			get {
				return ((this.flags & Gdl.DockObjectFlags.Automatic) != 0);
			}
		}
		
		public bool InReflow {
			get {
				return ((this.flags & Gdl.DockObjectFlags.InReflow) != 0);
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
				this.Reduce ();
			}
		}
		
		public void Bind (DockMaster _master)
		{
			if (_master == null)
				return;
			if (this.master == _master)
				return;
			if (this.master != null) {
				Console.WriteLine ("Attempt to bind an already bound object");
				return;
			}
			_master.Add (this);
			this.master = _master;
			//g_object_notify (G_OBJECT (object) /*this*/, "master");
		}
		
		public void Unbind ()
		{
			if (this.IsAttached)
				this.Detach (true);
			if (this.master != null) {
				DockMaster _master = this.master;
				this.master = null;
				_master.Remove (this);
				//g_object_notify (G_OBJECT (object) /*this*/, "master");
			}
		}
		
		public bool IsBound {
			get {
				return this.master != null;
			}
		}
		
		public DockObjectFlags DockObjectFlags {
			get { return this.flags; }
			set { this.flags = value; }
		}
	}
}
