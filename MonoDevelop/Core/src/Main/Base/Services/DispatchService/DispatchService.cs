using System;
using System.Threading;
using System.Collections;

using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Services
{
	public class DispatchService : AbstractService
	{
		ArrayList arrBackgroundQueue;
		ArrayList arrGuiQueue;
		Thread thrBackground;
		uint iIdle = 0;
		GLib.IdleHandler handler;
		static int guiThreadId;
		static GuiSyncContext guiContext;
		const string errormsg = "An exception was thrown while dispatching a method call in the UI thread.";
		internal static bool DispatchDebug;

		public override void InitializeService ()
		{
			guiContext = new GuiSyncContext ();
			SyncContext.SetContext (guiContext);
			
			guiThreadId = AppDomain.GetCurrentThreadId();
			
			handler = new GLib.IdleHandler (guiDispatcher);
			arrBackgroundQueue = new ArrayList ();
			arrGuiQueue = new ArrayList ();
			thrBackground = new Thread (new ThreadStart (backgroundDispatcher));
			thrBackground.IsBackground = true;
			thrBackground.Priority = ThreadPriority.Lowest;
			thrBackground.Start ();
			DispatchDebug = Environment.GetEnvironmentVariable ("MONODEVELOP_DISPATCH_DEBUG") != null;
		}

		public void GuiDispatch (MessageHandler cb)
		{
			arrGuiQueue.Add (new GenericMessageContainer (cb, false));
			UpdateIdle ();
		}

		public void GuiDispatch (StatefulMessageHandler cb, object state)
		{
			arrGuiQueue.Add (new StatefulMessageContainer (cb, state, false));
			UpdateIdle ();
		}

		public void GuiSyncDispatch (MessageHandler cb)
		{
			GenericMessageContainer mc = new GenericMessageContainer (cb, true);
			lock (mc) {
				arrGuiQueue.Add (mc);
				UpdateIdle ();
				Monitor.Wait (mc);
			}
			if (mc.Exception != null)
				throw new Exception (errormsg, mc.Exception);
		}
		
		public void GuiSyncDispatch (StatefulMessageHandler cb, object state)
		{
			StatefulMessageContainer mc = new StatefulMessageContainer (cb, state, true);
			lock (mc) {
				arrGuiQueue.Add (mc);
				UpdateIdle ();
				Monitor.Wait (mc);
			}
			if (mc.Exception != null)
				throw new Exception (errormsg, mc.Exception);
		}

		void UpdateIdle ()
		{
			if (iIdle == 0) {
				iIdle = GLib.Idle.Add (handler);
				/* This code is required because for some
				 * reason the idle handler is run once
				 * before being set, so you get a idle
				 * handler that isnt running, but our code
				 * thinks that it is.
				 */
				if (arrGuiQueue.Count == 0 && iIdle != 0)
					iIdle = 0;
			}
		}
		
		public static bool IsGuiThread
		{
			get { return guiThreadId == AppDomain.GetCurrentThreadId(); }
		}
		
		public static void AssertGuiThread ()
		{
			if (guiThreadId != AppDomain.GetCurrentThreadId())
				throw new InvalidOperationException ("This method can only be called in the GUI thread");
		}
		
		public static Delegate GuiDispatch (Delegate del)
		{
			return guiContext.CreateSynchronizedDelegate (del);
		}
		
		public void BackgroundDispatch (MessageHandler cb)
		{
			arrBackgroundQueue.Add (new GenericMessageContainer (cb, false));
		}

		public void BackgroundDispatch (StatefulMessageHandler cb, object state)
		{
			arrBackgroundQueue.Add (new StatefulMessageContainer (cb, state, false));
			//thrBackground.Resume ();
		}

		private bool guiDispatcher ()
		{
			if (arrGuiQueue.Count == 0) {
				iIdle = 0;
				return false;
			}
			GenericMessageContainer msg = null;
			lock (arrGuiQueue) {
				msg = (GenericMessageContainer)arrGuiQueue[0];
				arrGuiQueue.RemoveAt (0);
			}
			if (msg != null) {
				msg.Run ();
				if (msg.IsSynchronous)
					lock (msg) Monitor.PulseAll (msg);
				else if (msg.Exception != null)
					HandlerError (msg);
			}
			
			if (arrGuiQueue.Count == 0) {
				iIdle = 0;
				return false;
			}
			return true;
		}

		private void backgroundDispatcher ()
		{
			while (true) {
				if (arrBackgroundQueue.Count == 0) {
					Thread.Sleep (500);
					//thrBackground.Suspend ();
					continue;
				}
				GenericMessageContainer msg = null;
				lock (arrBackgroundQueue) {
					msg = (GenericMessageContainer)arrBackgroundQueue[0];
					arrBackgroundQueue.RemoveAt (0);
				}
				if (msg != null) {
					msg.Run ();
					if (msg.Exception != null)
						HandlerError (msg);
				}
			}
		}
		
		private void HandlerError (GenericMessageContainer msg)
		{
			Console.WriteLine (errormsg);
			Console.WriteLine (msg.Exception);
			if (msg.CallerStack != null) {
				Console.WriteLine ("\nCaller stack:");
				Console.WriteLine (msg.CallerStack);
			}
			else
				Console.WriteLine ("\n\nCaller stack not available. Define the environment variable MONODEVELOP_DISPATCH_DEBUG to enable caller stack capture.");
		}
	}

	public delegate void MessageHandler ();
	public delegate void StatefulMessageHandler (object state);

	class GenericMessageContainer
	{
		MessageHandler callback;
		protected Exception ex;
		protected bool isSynchronous;
		protected string callerStack;

		protected GenericMessageContainer () { }

		public GenericMessageContainer (MessageHandler cb, bool isSynchronous)
		{
			callback = cb;
			this.isSynchronous = isSynchronous;
			if (DispatchService.DispatchDebug) callerStack = Environment.StackTrace;
		}

		public virtual void Run ()
		{
			try {
				callback ();
			}
			catch (Exception e) {
				ex = e;
			}
		}
		
		public Exception Exception
		{
			get { return ex; }
		}
		
		public bool IsSynchronous
		{
			get { return isSynchronous; }
		}
		
		public string CallerStack
		{
			get { return callerStack; }
		}
	}

	class StatefulMessageContainer : GenericMessageContainer
	{
		object data;
		StatefulMessageHandler callback;

		public StatefulMessageContainer (StatefulMessageHandler cb, object state, bool isSynchronous)
		{
			data = state;
			callback = cb;
			this.isSynchronous = isSynchronous;
			if (DispatchService.DispatchDebug) callerStack = Environment.StackTrace;
		}
		
		public override void Run ()
		{
			try {
				callback (data);
			}
			catch (Exception e) {
				ex = e;
			}
		}
	}

}
