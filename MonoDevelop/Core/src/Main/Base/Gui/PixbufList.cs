using System.Collections;
using MonoDevelop.Core.Services;
using Gdk;

namespace MonoDevelop.Gui
{
	public class PixbufList : ArrayList
	{
		public PixbufList (params string [] resIcons) : base (resIcons.Length)
		{
			ResourceService rs = (ResourceService) ServiceManager.GetService (typeof (IResourceService));
			
			foreach (string s in resIcons)
				Add (rs.GetBitmap (s));
		}
		
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
