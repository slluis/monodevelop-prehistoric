// created on 06/06/2004 at 10:09 P
using System;
using Gtk;

namespace Gdl
{
	public class DockItem : DockObject
	{

		private static bool has_grip;
		
		private Gtk.Widget child;
		private DockItemBehavior behavior;
		private Gtk.Orientation orientation;
		private uint resize = 1;
		private int dragoff_x;
		private int dragoff_y;
		private Gtk.Menu menu;
		private bool grip_shown;
		private Gtk.Widget grip;
		private uint grip_size;
		private Gtk.Widget tab_label;
		private int preferred_width;
		private int preferred_height;
		private DockPlaceholder ph;
		private int start_x;
		private int start_y;
		
		static DockItem ()
		{
			Gtk.Rc.ParseString ("style \"gdl-dock-item-default\" {\n" +
			                    "xthickness = 0\n" +
			                    "ythickness = 0\n" + 
			                    "}\n" + 
			                    "class \"Gdl_DockItem\" " +
			                    "style : gtk \"gdl-dock-item-default\"\n");
		}
		
		public override bool IsCompound {
			get { return false; }
		}
		
		public Gtk.Orientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}
		
		public bool Resize {
			get { return this.resize; }
			set {
				this.resize = value;
				this.QueueResize ();
			}
		}
		
		public DockItemBehavior Behavior {
			get { return behavior; }
			set {
				DockItemBehavior old_beh = this.behavior;
				this.behavior = value;
				if (((old_beh ^ this.behavior) & DockItemBehavior.Locked) != 0) {
					/* PORT THIS:
					                if (GDL_DOCK_OBJECT_GET_MASTER (item))
                    g_signal_emit_by_name (GDL_DOCK_OBJECT_GET_MASTER (item),
                                           "layout_changed");
                g_object_notify (g_object, "locked");
                gdl_dock_item_showhide_grip (item);
                */
                }
			}
		}
		
		public bool Locked {
			get { return !((this.behavior & DockItemBehavior.Locked) != 0); }
			set {
				DockItemBehavior old_beh = this.behavior;
				if (value)
					this.behavior |= DockItemBehavior.Locked;
				else
					this.behavior &= ~(DockItemBehavior.Locked);
				if ((old_beh ^ this.behavior) != 0) {
					//PORT THIS:
					//gdl_dock_item_showhide_grip (item /*this*/);
					//g_object_notify (g_object, "behavior");
					//if (GDL_DOCK_OBJECT_GET_MASTER (item))
					//    g_signal_emit_by_name (GDL_DOCK_OBJECT_GET_MASTER (item)), "layout_changed");
				}
			}
		}
		
		public int PreferredWidth {
			get { return this.preferred_width; }
			set { this.preferred_width = value; }
		}
		
		public int PreferredHeight {
			get { return this.preferred_height; }
			set { this.preferred_height = value; }
		}
	}
}