using System;

namespace Gdl
{
	public delegate void DetachedHandler (object o, DetachedArgs args);
	
	public class DetachedArgs : EventArgs {
		private bool recursive;
	
		public DetachedArgs (bool recursive)
		{
			this.recursive = recursive;
		}
	
		public bool Recursive {
			get {
				return recursive;
			}
		}
	}
}
