using System;
using MonoDevelop.Gui;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.Debugger
{
	public class LocalsPad : VariablePad, IPadContent
	{

		public LocalsPad () : base (true)
		{
		}

		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.Debugger.LocalsPad"; }
		}

		public string DefaultPlacement {
			get { return "Bottom"; }
		}

		public string Title {
			get {
				return "Locals";
			}
		}

		public string Icon {
			get {
				return Stock.OutputIcon;
			}
		}

		public void RedrawContent ()
		{
			UpdateDisplay ();
		}

		public void BringToFront ()
		{
		}

                protected virtual void OnTitleChanged(EventArgs e)
                {
                        if (TitleChanged != null) {
                                TitleChanged(this, e);
                        }
                }
                protected virtual void OnIconChanged(EventArgs e)
                {
                        if (IconChanged != null) {
                                IconChanged(this, e);
                        }
                }
                public event EventHandler TitleChanged;
                public event EventHandler IconChanged;


	}
}
