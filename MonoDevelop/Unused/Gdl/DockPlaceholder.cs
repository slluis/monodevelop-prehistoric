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
		private ArrayList placementStack;
		private int hostDetachHandler;
		private int hostDockHandler;

		protected DockPlaceholder (IntPtr raw) : base (raw) { }
		
		public DockPlaceholder (string name, DockObject obj,
					DockPlacement position, bool sticky)
		{
			WidgetFlags |= WidgetFlags.NoWindow;
			WidgetFlags &= ~WidgetFlags.CanFocus;

			Sticky = sticky;
			Name = name;

			if (obj != null) {
				Attach (obj);

				if (position == DockPlacement.None)
					position = DockPlacement.Center;

				NextPlacement = position;
				if (obj is Dock)
					NextPlacement = DockPlacement.Center;

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
				if (placementStack != null && placementStack.Count != 0)
					return (DockPlacement)placementStack[0];
				return DockPlacement.Center;
			}
			set { 
				if (placementStack == null)
					placementStack = new ArrayList ();
				placementStack.Insert (0, value);
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
			DisconnectHost ();
			placementStack = null;
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
						placementStack.RemoveAt (0);
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
