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

		public override void InitializeService ()
		{
			arrBackgroundQueue = new ArrayList ();
			arrGuiQueue = new ArrayList ();
			thrBackground = new Thread (new ThreadStart (backgroundDispatcher));
			thrBackground.IsBackground = true;
			thrBackground.Priority = ThreadPriority.Lowest;
			thrBackground.Start ();
		}

		public void GuiDispatch (MessageHandler cb)
		{
			arrGuiQueue.Add (new GenericMessageContainer (cb));
			UpdateIdle ();
		}

		public void GuiDispatch (StatefulMessageHandler cb, object state)
		{
			arrGuiQueue.Add (new StatefulMessageContainer (cb, state));
			UpdateIdle ();
		}

		void UpdateIdle ()
		{
			if (iIdle == 0) {
				iIdle = GLib.Idle.Add (new GLib.IdleHandler (guiDispatcher));
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

		public void BackgroundDispatch (MessageHandler cb)
		{
			arrBackgroundQueue.Add (new GenericMessageContainer (cb));
		}

		public void BackgroundDispatch (StatefulMessageHandler cb, object state)
		{
			arrBackgroundQueue.Add (new StatefulMessageContainer (cb, state));
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
			if (msg != null)
				msg.Run ();
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
				if (msg != null)
					msg.Run ();
			}
		}
	}

	public delegate void MessageHandler ();
	public delegate void StatefulMessageHandler (object state);

	class GenericMessageContainer
	{
		MessageHandler callback;

		protected GenericMessageContainer () { }

		public GenericMessageContainer (MessageHandler cb)
		{
			callback = cb;
		}

		public virtual void Run ()
		{
			callback ();
		}
	}

	class StatefulMessageContainer : GenericMessageContainer
	{
		object data;
		StatefulMessageHandler callback;

		public StatefulMessageContainer (StatefulMessageHandler cb, object state)
		{
			data = state;
			callback = cb;
		}
		
		public override void Run ()
		{
			callback (data);
		}
	}
}
