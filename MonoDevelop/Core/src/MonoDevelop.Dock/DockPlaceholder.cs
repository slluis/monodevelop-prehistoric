// created on 07/06/2004 at 5:43 P

using System;
using System.Collections;
using System.Xml;
using Gtk;

namespace Gdl
{
	public class DockPlaceholder : DockObject
	{
		private DockObject host;
		private bool sticky;
		private Stack placementStack;

		protected DockPlaceholder (IntPtr raw) : base (raw) { }
		
		public DockPlaceholder (string name, DockObject obj,
					DockPlacement position, bool sticky)
		{
			WidgetFlags |= WidgetFlags.NoWindow;
			WidgetFlags &= ~(WidgetFlags.CanFocus);

			Sticky = sticky;
			Name = name;

			if (obj != null) {
				Attach (obj);

				if (position == DockPlacement.None)
					position = DockPlacement.Center;

				NextPlacement = position;

				//the top placement will be consumed by the toplevel dock, so add a dummy placement
				if (obj is Dock)
					NextPlacement = DockPlacement.Center;

				// try a recursion
				DoExcursion ();
			}
		}
		
		public DockPlaceholder (DockObject obj, bool sticky) :
			this (obj.Name, obj, DockPlacement.None, sticky) { }
		
		public DockObject Host {
			get {
				return host;
			}
			set {
				Attach (value);
				EmitPropertyEvent ("Host");
			}
		}
		
		[After]
		[Export]
		public DockPlacement NextPlacement {
			get {
				if (placementStack != null && placementStack.Count > 0)
					return (DockPlacement) placementStack.Pop ();
				return DockPlacement.Center;
			}
			set { 
				if (placementStack == null)
					placementStack = new Stack ();
				placementStack.Push (value);
			}
		}

		public bool Sticky {
			get {
				return sticky;
			}
			set {
				sticky = value;
				EmitPropertyEvent ("Sticky");
			}
		}

		protected override void OnDestroyed ()
		{
			if (host != null)
				OnDetached (false);
			base.OnDestroyed ();
		}
		
		protected override void OnAdded (Widget widget)
		{
			if (!(widget is DockItem))
				return;

			Dock ((DockItem)widget, NextPlacement, null);
		}
		
		public override void OnDetached (bool recursive)
		{
			// disconnect handlers
			DisconnectHost ();

			// free the placement stack
			placementStack = null;

			DockObjectFlags &= ~(DockObjectFlags.Attached);
		}
		
		public override void OnReduce ()
		{
			// placeholders are not reduced
		}
		
		public override void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
			if (host != null) {
				// we simply act as a placeholder for our host
				host.Dock (requestor, position, data);
			} else {
				if (!IsBound) {
					Console.WriteLine ("Attempt to dock a dock object to an unbound placeholder");
					return;
				}
				// dock the item as a floating of the controller
				Master.Controller.Dock (requestor, DockPlacement.Floating, null);
			}
		}
		
		public override void OnPresent (DockObject child)
		{
			// do nothing
		}
		
		/*
		* Tries to shrink the placement stack by examining the host's
		* children and see if any of them matches the placement which is at
		* the top of the stack.  If this is the case, it tries again with the
		* new host.
		*/
		public void DoExcursion ()
		{
			if (host != null && !Sticky && placementStack != null && placementStack.Count > 0 && host.IsCompound) {
				DockPlacement pos;
				DockPlacement stack_pos = NextPlacement;
				foreach (Widget child in host.Children) {
					DockObject item = child as DockObject;
					if (item == null)
						continue;
					pos = stack_pos;
					
					host.ChildPlacement (item, ref pos);
					if (pos == stack_pos) {
						// remove the stack position
						if (placementStack.Count > 1)
							placementStack.Pop ();
						DisconnectHost ();

						// connect to the new host
						ConnectHost (item);
						
						// recurse ...
						if (!item.InReflow)
							DoExcursion ();
						break;
					}
				}
			}
		}
		
		private void DisconnectHost ()
		{
			if (host == null)
				return;

			host.Detached -= OnHostDetached;
			host.Docked -= OnHostDocked;

			host = null;
		}
		
		private void ConnectHost (DockObject newHost)
		{
			if (host != null)
				DisconnectHost ();

			host = newHost;

			host.Detached += OnHostDetached;
			host.Docked += OnHostDocked;
		}
		
		public void Attach (DockObject objekt)
		{
			if (objekt == null)
				return;
			
			// object binding
			if (!IsBound)
				Bind(objekt.Master);
			
			if (objekt.Master != Master)
				return;
			
			Freeze ();
			
			// detach from previous host first
			if (host != null)
				Detach (false);
			
			ConnectHost (objekt);
			
			DockObjectFlags |= DockObjectFlags.Attached;
			Thaw ();
		}

		void OnHostDetached (object sender, DetachedArgs a)
		{
			// skip sticky objects
			if (sticky)
				return;

			// go up in the hierarchy
			DockObject newHost = host.ParentObject;

			while (newHost != null) {
				DockPlacement pos = DockPlacement.None;

				// get a placement hint from the new host
				if (newHost.ChildPlacement (host, ref pos))
					NextPlacement = pos;
				else
					Console.WriteLine ("Something weird happened while getting the child placement for {0} from parent {1}", host, newHost);

				// we found a "stable" dock object
				if (newHost.InDetach)
					break;

				newHost = newHost.ParentObject;
			}

			// disconnect host
			DisconnectHost ();

			// the toplevel was detached: we attach ourselves to the
			// controller with an initial placement of floating
			if (newHost == null) {
				newHost = this.Master.Controller;
				NextPlacement = DockPlacement.Floating;
			}

			if (newHost != null)
				ConnectHost (newHost);

			PrintPlacementStack ();
		}

		void OnHostDocked (object sender, DockedArgs a)
		{
			DockObject obj = sender as DockObject;
			// see if the given position is compatible for the stack's top element
			if (!sticky && placementStack != null) {
				DockPlacement pos = NextPlacement;
				if (obj.ChildPlacement (a.Requestor, ref pos))
					DoExcursion ();
			}

			PrintPlacementStack ();
		}

		[System.Diagnostics.Conditional ("DEBUG")]
		void PrintPlacementStack ()
		{
			Console.WriteLine ("-- {0} count {1}", host.Name, placementStack.Count);
			foreach (object o in placementStack.ToArray ())
				Console.WriteLine (o);
		}
	}
}
