// Generated File.  Do not modify.
// <c> 2001-2002 Mike Kestner

namespace GtkMozEmbedSharp {

	using System;
	using System.Runtime.InteropServices;
	using GtkSharp;

	internal delegate void voidObjectObjectuintDelegate(IntPtr arg0, out IntPtr arg1, uint arg2, int key);

	internal class voidObjectObjectuintSignal : SignalCallback {

		private static voidObjectObjectuintDelegate _Delegate;

		private IntPtr _raw;
		private uint _HandlerID;

		private static void voidObjectObjectuintCallback(IntPtr arg0, out IntPtr arg1, uint arg2, int key)
		{
			if (!_Instances.Contains(key))
				throw new Exception("Unexpected signal key " + key);

			voidObjectObjectuintSignal inst = (voidObjectObjectuintSignal) _Instances[key];
			SignalArgs args = (SignalArgs) Activator.CreateInstance (inst._argstype);
			args.Args = new object[2];

			// Ok, arg1 is set by the handler...
			// So, we can pass a null, and we'll get an Gtk.Object back
			args.Args[0] = null;
			args.Args[1] = arg2;

			object[] argv = new object[2];
			argv[0] = inst._obj;
			argv[1] = args;
			inst._handler.DynamicInvoke(argv);
			arg1 = ((GLib.Object)args.Args[0]).Handle;				
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern uint g_signal_connect_data(IntPtr obj, String name, voidObjectObjectuintDelegate cb, int key, IntPtr p, int flags);

		public voidObjectObjectuintSignal(GLib.Object obj, IntPtr raw, String name, Delegate eh, Type argstype) : base(obj, eh, argstype)
		{
			if (_Delegate == null) {
				_Delegate = new voidObjectObjectuintDelegate(voidObjectObjectuintCallback);
			}
			_raw = raw;
			_HandlerID = g_signal_connect_data(raw, name, _Delegate, _key, new IntPtr(0), 0);
		}

		[DllImport("libgobject-2.0-0.dll")]
		static extern void g_signal_handler_disconnect (IntPtr instance, uint handler);

		protected override void Dispose (bool disposing)
		{
			_Instances.Remove(_key);
			if(_Instances.Count == 0)
				_Delegate = null;

			g_signal_handler_disconnect (_raw, _HandlerID);
			base.Dispose (disposing);
		}
	}
}
