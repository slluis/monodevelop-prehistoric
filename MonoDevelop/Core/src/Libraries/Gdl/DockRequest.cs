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
		private int x, y, width, height;
		private object extra;
		
		public DockRequest ()
		{
		}
		
		public DockRequest (DockRequest copy)
		{
			applicant = copy.Applicant;
			target = copy.Target;
			x = copy.X;
			y = copy.Y;
			width = copy.Width;
			height = copy.Height;
			position = copy.Position;
			
			extra = copy.Extra;
		}
		
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

		public int X {
			get {
				return x;
			}
			set {
				x = value;
			}
		}
		
		public int Y {
			get {
				return y;
			}
			set {
				y = value;
			}
		}
		
		public int Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}
		
		public int Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}
						
		public object Extra {
			get { return extra; }
			set { extra = value; }
		}
	}
}
