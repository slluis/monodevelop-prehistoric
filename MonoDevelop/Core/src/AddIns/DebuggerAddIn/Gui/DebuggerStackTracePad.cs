using GLib;
using Gtk;
using GtkSharp;
using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Mono.Debugger;
using Mono.Debugger.Languages;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Gui;

namespace MonoDevelop.Debugger
{
	public class DebuggerStackTracePad : Gtk.ScrolledWindow, IPadContent
	{
		StackFrame current_frame;

		Gtk.TreeView tree;
		Gtk.TreeStore store;

		public DebuggerStackTracePad ()
		{
			this.ShadowType = ShadowType.In;

			store = new TreeStore (typeof (string));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn FrameCol = new TreeViewColumn ();
			CellRenderer FrameRenderer = new CellRendererText ();
			FrameCol.Title = "Frame";
			FrameCol.PackStart (FrameRenderer, true);
			FrameCol.AddAttribute (FrameRenderer, "text", 0);
			FrameCol.Resizable = true;
			FrameCol.Alignment = 0.0f;
			tree.AppendColumn (FrameCol);

			Add (tree);
			ShowAll ();

			Runtime.DebuggingService.PausedEvent += new EventHandler (OnPausedEvent);
			Runtime.DebuggingService.ResumedEvent += new EventHandler (OnResumedEvent);
			Runtime.DebuggingService.StoppedEvent += new EventHandler (OnStoppedEvent);
		}

		public void UpdateDisplay ()
		{
			if ((current_frame == null) || (current_frame.Method == null))
				return;

			string[] trace = Runtime.DebuggingService.Backtrace;

			TreeIter it;
			if (!store.GetIterFirst (out it)) {
				foreach (string frame in trace) {
					store.Append (out it);
					store.SetValue (it, 0, frame);
				}
			}
			else {
				for (int i = 0; i < trace.Length; i ++) {
					store.SetValue (it, 0, trace[i]);
					if (i < trace.Length - 1 && !store.IterNext (ref it))
						store.Append (out it);
				}
				/* clear any remaining rows */
				if (store.IterNext (ref it))
					do { } while (store.Remove (ref it));
			}
		}

		protected void OnStoppedEvent (object o, EventArgs args)
		{
			UpdateDisplay ();
		}

		protected void OnResumedEvent (object o, EventArgs args)
		{
			UpdateDisplay ();
		}

		protected void OnPausedEvent (object o, EventArgs args)
		{
			current_frame = (StackFrame)Runtime.DebuggingService.CurrentFrame;
			UpdateDisplay ();
		}

		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.Debugger.DebuggerStackTracePad"; }
		}

		public string DefaultPlacement {
			get { return "Bottom"; }
		}

		public string Title {
			get {
				return "Call Stack";
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
