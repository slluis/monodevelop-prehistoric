using System.Collections;

using Gdk;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class PixbufList : ArrayList
	{
		public new Pixbuf this[int idx] {
			get {
				return (Pixbuf) base[idx];
			}
			set {
				base[idx] = value;
			}
		}

		public void Add (Pixbuf item) {
			base.Add (item);
		}
		
		public IList Images {
			get {
				return this; // Hack to allow to compile original code
			}
		}
	}
}
