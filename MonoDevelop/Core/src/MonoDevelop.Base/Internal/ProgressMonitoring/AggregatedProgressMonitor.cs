//
// AggregatedProgressMonitor.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.IO;

namespace MonoDevelop.Services
{
	public class AggregatedProgressMonitor: IProgressMonitor, IAsyncOperation
	{
		IProgressMonitor[] monitors;
		LogTextWriter logger;
		
		public AggregatedProgressMonitor (params IProgressMonitor[] monitors)
		{
			this.monitors = monitors;
			logger = new LogTextWriter ();
			logger.TextWritten += new LogTextEventHandler (OnWriteLog);
		}
		
		public void BeginTask (string name, int totalWork)
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.BeginTask (name, totalWork);
		}
		
		public void EndTask ()
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.EndTask ();
		}
		
		public void Step (int work)
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.Step (work);
		}
		
		public TextWriter Log
		{
			get { return logger; }
		}
		
		void OnWriteLog (string text)
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.Log.Write (text);
		}
		
		public void ReportSuccess (string message)
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.ReportSuccess (message);
		}
		
		public void ReportWarning (string message)
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.ReportWarning (message);
		}
		
		public void ReportError (string message, Exception ex)
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.ReportError (message, ex);
		}
		
		public void Dispose ()
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.Dispose ();
		}
		
		public bool IsCancelRequested
		{
			get {
				foreach (IProgressMonitor monitor in monitors)
					if (monitor.IsCancelRequested) return true;
				return false;
			}
		}
		
		public IAsyncOperation AsyncOperation
		{
			get { return this; }
		}
		
		void IAsyncOperation.Cancel ()
		{
			foreach (IProgressMonitor monitor in monitors)
				monitor.AsyncOperation.Cancel ();
		}
		
		void IAsyncOperation.WaitForCompleted ()
		{
			if (IsCompleted) return;
			
			if (Runtime.DispatchService.IsGuiThread) {
				while (!IsCompleted) {
					while (Gtk.Application.EventsPending ())
						Gtk.Application.RunIteration ();
					System.Threading.Thread.Sleep (100);
				}
			} else {
				monitors [0].AsyncOperation.WaitForCompleted ();
			}
		}
		
		public bool IsCompleted {
			get { return monitors [0].AsyncOperation.IsCompleted; }
		}
		
		bool IAsyncOperation.Success { 
			get { return monitors [0].AsyncOperation.Success; }
		}
		
		public event MonitorHandler CancelRequested {
			add { monitors [0].CancelRequested += value; }
			remove { monitors [0].CancelRequested -= value; }
		}
			
		public event OperationHandler Completed {
			add { monitors [0].AsyncOperation.Completed += value; }
			remove { monitors [0].AsyncOperation.Completed -= value; }
		}
	}
}
