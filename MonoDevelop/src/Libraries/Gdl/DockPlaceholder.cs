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
			Flags |= (int)WidgetFlags.NoWindow;
			Flags &= ~((int)WidgetFlags.CanFocus);
		}
		
		public DockPlaceholder (string name, DockObject objekt, DockPlacement position, bool sticky) : this ()
		{
			Sticky = sticky;
			Name = name;
			if (objekt != null) {
				Attach (objekt);
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
			set { Attach (value); }
		}
		
		public DockPlacement NextPlacement {
			get {
				if (placement_stack != null && placement_stack.Count != 0)
					return (DockPlacement)placement_stack[0];
				return DockPlacement.Center;
			}
			set { 
				if (placement_stack == null)
					placement_stack = new ArrayList ();
				placement_stack.Insert (0, value);
			}
		}
		
		protected override void OnAdded (Widget widget)
		{
			if (!(widget is DockItem))
				return;
			Dock ((DockItem)widget, NextPlacement, null);
		}
		
		public override void OnDetached (bool recursive)
		{
			DisconnectHost ();
			placement_stack = null;
			DockObjectFlags &= ~(DockObjectFlags.Attached);
		}
		
		public override void OnReduce ()
		{
		}
		
		public override void OnDocked (DockObject requestor, DockPlacement position, object data)
		{
			if (host != null) {
				host.Dock (requestor, position, data);
			} else {
				if (!IsBound) {
					Console.WriteLine ("Attempt to dock a dock object to an unbound placeholder");
					return;
				}
				Master.Controller.Dock (requestor, DockPlacement.Floating, null);
			}
		}
		
		public override void OnPresent (DockObject child)
		{
		}
		
		public void DoExcursion ()
		{
			if (host != null && !Sticky && placement_stack != null && host.IsCompound) {
				DockPlacement pos;
				DockPlacement stack_pos = NextPlacement;
				foreach (Widget child in host.Children) {
					DockObject item = child as DockObject;
					if (item == null)
						continue;
					pos = stack_pos;
					
					host.ChildPlacement (item, ref pos);
					if (pos == stack_pos) {
						placement_stack.RemoveAt (0);
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
			host = null;
		}
		
		private void ConnectHost (DockObject new_host)
		{
			if (host != null)
				DisconnectHost ();
			host = new_host;
			//Connect to host detach and dock events here.
		}
		
		public void Attach (DockObject objekt)
		{
			if (objekt == null)
				return;
			
			if (!IsBound)
				Bind(objekt.Master);
			
			if (objekt.Master != Master)
				return;
			
			Freeze ();
			
			if (host != null)
				Detach (false);
			
			ConnectHost (objekt);
			
			DockObjectFlags |= DockObjectFlags.Attached;
			Thaw ();
		}
	}
}
