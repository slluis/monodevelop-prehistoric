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

			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			dbgr.PausedEvent += new EventHandler (OnPausedEvent);
			dbgr.ResumedEvent += new EventHandler (OnResumedEvent);
			dbgr.StoppedEvent += new EventHandler (OnStoppedEvent);
		}

		void add_frame (string frame)
		{
			TreeIter iter;
			store.Append (out iter);
			store.SetValue (iter, 0, new GLib.Value (frame));
		}

		Hashtable iters = null;

		public void CleanDisplay ()
		{
			store.Clear ();
			iters = new Hashtable ();
		}

		public void UpdateDisplay ()
		{
			CleanDisplay ();

			if ((current_frame == null) || (current_frame.Method == null))
				return;

			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			string[] trace = dbgr.Backtrace;

			foreach (string frame in trace)
				add_frame (frame);
		}

		protected void OnStoppedEvent (object o, EventArgs args)
		{
			CleanDisplay ();
		}

		protected void OnResumedEvent (object o, EventArgs args)
		{
			CleanDisplay ();
		}

		protected void OnPausedEvent (object o, EventArgs args)
		{
			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			current_frame = (StackFrame)dbgr.CurrentFrame;
			UpdateDisplay ();
		}



		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.SourceEditor.Gui.DebuggeStackTracePad"; }
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
