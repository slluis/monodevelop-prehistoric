using System;

namespace Gdl
{
	public delegate void DockedHandler (object o, DockedArgs args);
	
	public class DockedArgs : EventArgs {
		private DockObject requestor;
		private DockPlacement position;
	
		public DockedArgs (DockObject requestor, DockPlacement position)
		{
			this.requestor = requestor;
			this.position = position;
		}
	
		public DockObject Requestor {
			get {
				return requestor;
			}
		}
		
		public DockPlacement Position {
			get {
				return position;
			}
		}
	}
}
