// created on 04/06/2004 at 6:59 P

using System;
using Gtk;
using Gdk;

namespace Gdl
{
	public class DockRequest
	{
		private DockObject applicant;
		private DockObject target;
		private DockPlacement position;
		private Rectangle rect;
		private object extra;
		
		public DockObject Applicant {
			get { return applicant; }
			set { applicant = value; }
		}
		
		public DockObject Target {
			get { return target; }
			set { target = value; }
		}
		
		public DockPlacement Position {
			get { return position; }
			set { position = value; }
		}
		
		public Rectangle Rect {
			get { return rect; }
			set { rect = value; }
		}
		
		public object Extra {
			get { return extra; }
			set { extra = value; }
		}
	}
}