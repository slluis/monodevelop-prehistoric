// created on 12/18/2004 at 16:28
using System;
using System.Threading;
using System.Diagnostics;

namespace MonoDevelop.Services
{
	public delegate void ProcessEventHandler(object sender, string message);
	
	public class ProcessWrapper : Process
	{
		private Thread captureOutputThread;
		private Thread captureErrorThread;
		AutoResetEvent endEventOut = new AutoResetEvent (false);
		AutoResetEvent endEventErr = new AutoResetEvent (false);
		
		public new void Start ()
		{
			base.Start ();
			
			captureOutputThread = new Thread (new ThreadStart(CaptureOutput));
			captureOutputThread.IsBackground = true;
			captureOutputThread.Start ();
			
			captureErrorThread = new Thread (new ThreadStart(CaptureError));
			captureErrorThread.IsBackground = true;
			captureErrorThread.Start ();
		}
		
		public void Abort ()
		{
			captureOutputThread.Abort ();
			captureErrorThread.Abort ();
		}
		
		public void WaitForOutput ()
		{
			WaitForExit ();
			WaitHandle.WaitAll (new WaitHandle[] {endEventOut, endEventErr});
		}
		
		private void CaptureOutput ()
		{
			string s;

			if (OutputStreamChanged != null)
			{
				while ((s = StandardOutput.ReadLine()) != null) {
					if (OutputStreamChanged != null)
						OutputStreamChanged (this, s);
				}
			}
			endEventOut.Set ();
		}
		
		private void CaptureError ()
		{
			string s;
			
			if (ErrorStreamChanged != null)
			{
				while ((s = StandardError.ReadLine()) != null) {
					if (ErrorStreamChanged != null)
						ErrorStreamChanged (this, s);
				}					
			}
			endEventErr.Set ();
		}
	
		public event ProcessEventHandler OutputStreamChanged;
		public event ProcessEventHandler ErrorStreamChanged;
	}
}
