using System;
using MonoDevelop.Gui;

namespace MonoDevelop.SourceEditor.Gui
{
	public class DebuggerLocalsPad : DebuggerVariablePad, IPadContent
	{

		public DebuggerLocalsPad () : base (true)
		{
		}

		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.SourceEditor.Gui.DebuggerLocalsPad"; }
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
				return MonoDevelop.Gui.Stock.OutputIcon;
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
