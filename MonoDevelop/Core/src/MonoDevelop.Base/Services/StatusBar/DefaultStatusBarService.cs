// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Diagnostics;
using Gtk;

using MonoDevelop.Core.Services;
using MonoDevelop.Gui.Components;
using MonoDevelop.Gui;
using MonoDevelop.Services;

namespace MonoDevelop.Services
{
	public class DefaultStatusBarService : GuiSyncAbstractService, IStatusBarService, IProgressMonitor
	{
		SdStatusBar statusBar = null;
		StringParserService stringParserService = Runtime.StringParserService;
		
		public DefaultStatusBarService()
		{
			statusBar = new SdStatusBar(this);
		}
		
		public void Dispose()
		{
			if (statusBar != null) {
				statusBar.Dispose();
				statusBar = null;
			}
		}
		
		public Widget Control {
			get {
				Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public IProgressMonitor ProgressMonitor {
			get { 
				Debug.Assert(statusBar != null);
				return this;
			}
		}
		

		void IProgressMonitor.BeginTask (string name, int totalWork)
		{
			statusBar.BeginTask (name, totalWork);
		}
		
		void IProgressMonitor.Worked (double work, string status)
		{
			statusBar.Worked (work, status);
		}
		
		void IProgressMonitor.Pulse ()
		{
			statusBar.Pulse ();
		}
		
		void IProgressMonitor.Done()
		{
			statusBar.Done ();
		}
		
		bool IProgressMonitor.Canceled {
			get { return statusBar.Canceled; }
			set { statusBar.Canceled = value; }
		}
		
		string IProgressMonitor.TaskName {
			get { return statusBar.TaskName; }
			set { statusBar.TaskName = value; }
		}
		
		public bool CancelEnabled {
			get {
				return statusBar != null && statusBar.CancelEnabled;
			}
			set {
				Debug.Assert(statusBar != null);
				statusBar.CancelEnabled = value;
			}
		}
		
		[AsyncDispatch]
		public void SetCaretPosition (int ln, int col, int ch)
		{
			statusBar.SetCursorPosition (ln, col, ch);
		}
		
		[AsyncDispatch]
		public void SetInsertMode (bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Push (1, insertMode ? GettextCatalog.GetString ("INS") : GettextCatalog.GetString ("OVR"));
		}
		
		[AsyncDispatch]
		public void ShowErrorMessage (string message)
		{
			Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(stringParserService.Parse(message));
		}
		
		[AsyncDispatch]
		public void SetMessage (string message)
		{
			Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(stringParserService.Parse(message));
		}
		
		[AsyncDispatch]
		public void SetMessage(Gtk.Image image, string message)
		{
			Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(image, stringParserService.Parse(message));
		}
		
		bool   wasError    = false;
		string lastMessage = "";
		
		[AsyncDispatch]
		public void RedrawStatusbar()
		{
			if (wasError) {
				ShowErrorMessage(lastMessage);
			} else {
				SetMessage(lastMessage);
			}
		}
		
		[AsyncDispatch]
		public void Update()
		{
			Debug.Assert(statusBar != null);
			/*statusBar.Clear ();
			statusBar.Clear ();
			
			foreach (StatusBarContributionItem item in Items) {
				if (item.Control != null) {
					statusBar.Add (item.Control);
				} else if (item.Panel != null) {
					statusBar.Add (item.Panel);
				} else {
					throw new ApplicationException ("StatusBarContributionItem " + item.ItemID + " has no Control or Panel defined.");
				}
			}*/
		}
	}
}
