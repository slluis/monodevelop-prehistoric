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
	public class DebuggerThreadPad : Gtk.ScrolledWindow, IPadContent
	{
		Gtk.TreeView tree;
		Gtk.TreeStore store;

		public DebuggerThreadPad ()
		{
			this.ShadowType = ShadowType.In;

			store = new TreeStore (typeof (int),
					       typeof (int),
					       typeof (string),
					       typeof (string));

			tree = new TreeView (store);
			tree.RulesHint = true;
			tree.HeadersVisible = true;

			TreeViewColumn Col;
			CellRenderer ThreadRenderer;

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "Id";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 0);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "PID";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 1);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "State";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 2);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Col = new TreeViewColumn ();
			ThreadRenderer = new CellRendererText ();
			Col.Title = "Current Location";
			Col.PackStart (ThreadRenderer, true);
			Col.AddAttribute (ThreadRenderer, "text", 3);
			Col.Resizable = true;
			Col.Alignment = 0.0f;
			tree.AppendColumn (Col);

			Add (tree);
			ShowAll ();

			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			dbgr.ThreadStateEvent += new EventHandler (OnThreadEvent);
		}

		void add_thread (Process thread)
		{
			TreeIter iter;
			store.Append (out iter);
			store.SetValue (iter, 0, new GLib.Value (thread.ID));
			store.SetValue (iter, 1, new GLib.Value (thread.PID));
			store.SetValue (iter, 2, new GLib.Value (thread.State.ToString()));
			if (thread.IsStopped)
			  store.SetValue (iter, 3, new GLib.Value (thread.GetBacktrace().Frames[0].SourceAddress.Name));
			else
			  store.SetValue (iter, 3, new GLib.Value (""));
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

			DebuggingService dbgr = (DebuggingService)ServiceManager.GetService (typeof (DebuggingService));
			Process[] threads = dbgr.Threads;

			foreach (Process t in threads)
				if (!t.IsDaemon)
					add_thread (t);
		}

		protected void OnThreadEvent (object o, EventArgs args)
		{
			UpdateDisplay ();
		}

		public Gtk.Widget Control {
			get {
				return this;
			}
		}

		public string Id {
			get { return "MonoDevelop.SourceEditor.Gui.DebuggerThreadPad"; }
		}

		public string DefaultPlacement {
			get { return "Bottom"; }
		}

		public string Title {
			get {
				return "Threads";
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
