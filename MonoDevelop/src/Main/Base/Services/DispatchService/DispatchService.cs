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

		public void GuiDispatch (MessageHandler cb, object state)
		{
			arrGuiQueue.Add (new MessageContainer (cb, state));
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

		public void BackgroundDispatch (MessageHandler cb, object state)
		{
			arrBackgroundQueue.Add (new MessageContainer (cb, state));
			//thrBackground.Resume ();
		}

		private bool guiDispatcher ()
		{
			if (arrGuiQueue.Count == 0) {
				iIdle = 0;
				return false;
			}
			MessageContainer msg = null;
			lock (arrGuiQueue) {
				msg = (MessageContainer)arrGuiQueue[0];
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
				MessageContainer msg = null;
				lock (arrBackgroundQueue) {
					msg = (MessageContainer)arrBackgroundQueue[0];
					arrBackgroundQueue.RemoveAt (0);
				}
				if (msg != null)
					msg.Run ();
			}
		}
	}

	public delegate void MessageHandler (object state);

	class MessageContainer
	{
		object data;
		MessageHandler callback;

		public MessageContainer (MessageHandler cb, object state)
		{
			data = state;
			callback = cb;
		}
		
		public void Run ()
		{
			callback (data);
		}
	}
}
