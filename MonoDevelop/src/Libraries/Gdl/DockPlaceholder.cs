// created on 07/06/2004 at 5:43 P

using System;
using System.Collections;
using Gtk;

namespace Gdl
{
	public class DockPlaceholder : DockObject
	{
		private DockObject host;
		private bool sticky;
		private ArrayList placement_stack;
		private int host_detach_handler;
		private int host_dock_handler;
		
		public DockPlaceholder ()
		{
			this.Flags |= (int)Gtk.WidgetFlags.NoWindow;
			this.Flags &= ~((int)Gtk.WidgetFlags.CanFocus);
		}
		
		public DockPlaceholder (string name, DockObject objekt, DockPlacement position, bool sticky) : this ()
		{
			this.Sticky = sticky;
			this.Name = name;
			if (objekt != null) {
				this.Attach (objekt);
				if (position == DockPlacement.None) {
					position = DockPlacement.Center;
				}
				NextPlacement = position;
				if (objekt is Dock) {
					NextPlacement = DockPlacement.Center;
				}
				DoExcursion ();
			}
		}
		
		public DockPlaceholder (DockObject objekt, bool sticky) : this (objekt.Name, objekt, DockPlacement.None, sticky)
		{
		}
		
		public bool Sticky {
			get { return sticky; }
			set { sticky = value; }
		}
		
		public DockObject Host {
			get { return host; }
			set { this.Attach (value); }
		}
		
		public DockPlacement NextPlacement {
			get {
				if (this.placement_stack != null && this.placement_stack.Count != 0)
					return (DockPlacement)this.placement_stack[0];
				return DockPlacement.Center;
			}
			set { 
				if (placement_stack == null)
					placement_stack = new ArrayList ();
				this.placement_stack.Insert (0, value);
			}
		}
		
		protected override void OnAdded (Gtk.Widget widget)
		{
			if (!(widget is DockItem))
				return;
			this.Docking ((DockItem)widget, this.NextPlacement, null);
		}
		
		public override void Detach (bool recursive)
		{
			this.DisconnectHost ();
			this.placement_stack = null;
			this.DockObjectFlags &= ~(DockObjectFlags.Attached);
		}
		
		public override void Reduce ()
		{
		}
		
		public override void Docking (DockObject requestor, DockPlacement position, object other_data)
		{
			if (this.host != null) {
				this.host.Docking (requestor, position, other_data);
			} else {
				if (!this.IsBound) {
					Console.WriteLine ("Attempt to dock a dock object to an unbound placeholder");
					return;
				}
				this.Master.Controller.Docking (requestor, DockPlacement.Floating, null);
			}
		}
		
		public override void Present (DockObject child)
		{
		}
		
		public void DoExcursion ()
		{
			if (this.host != null && !this.Sticky && this.placement_stack != null && this.host.IsCompound) {
				DockPlacement pos;
				DockPlacement stack_pos = this.NextPlacement;
				foreach (Gtk.Widget child in this.host.Children) {
					DockObject item = child as DockObject;
					if (item == null)
						continue;
					pos = stack_pos;
					
					this.host.ChildPlacement (item, ref pos);
					if (pos == stack_pos) {
						this.placement_stack.RemoveAt (0);
						DisconnectHost ();
						ConnectHost (item);
						
						if (!item.InReflow)
							DoExcursion ();
						break;
					}
				}
			}
		}
		
		private void DisconnectHost ()
		{
			//Disconnect from host detach and dock events here.
			this.host = null;
		}
		
		private void ConnectHost (DockObject new_host)
		{
			if (this.host != null)
				DisconnectHost ();
			this.host = new_host;
			//Connect to host detach and dock events here.
		}
		
		public void Attach (DockObject objekt)
		{
			if (objekt == null)
				return;
			
			if (!this.IsBound)
				this.Bind(objekt.Master);
			
			if (objekt.Master != this.Master)
				return;
			
			this.Freeze ();
			
			if (this.host != null)
				this.Detach (false);
			
			ConnectHost (objekt);
			
			this.DockObjectFlags |= DockObjectFlags.Attached;
			this.Thaw ();
		}
	}
}
