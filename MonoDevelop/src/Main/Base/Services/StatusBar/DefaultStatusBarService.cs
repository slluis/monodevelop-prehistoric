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
	public class DefaultStatusBarService : AbstractService, IStatusBarService
	{
		SdStatusBar statusBar = null;
		StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
		
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
				return statusBar;
			}
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
		
		public void SetCaretPosition (int ln, int col, int ch)
		{
			statusBar.SetCursorPosition (ln, col, ch);
		}
		
		public void SetInsertMode (bool insertMode)
		{
			StringParserService stringParserService = (StringParserService)ServiceManager.GetService(typeof(StringParserService));
			statusBar.ModeStatusBarPanel.Push (1, insertMode ? GettextCatalog.GetString ("INS") : GettextCatalog.GetString ("OVR"));
		}
		
		public void ShowErrorMessage (string message)
		{
			Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(stringParserService.Parse(message));
		}
		
		public void SetMessage (string message)
		{
			Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(stringParserService.Parse(message));
		}
		
		public void SetMessage(Gtk.Image image, string message)
		{
			Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(image, stringParserService.Parse(message));
		}
		
		bool   wasError    = false;
		string lastMessage = "";
		public void RedrawStatusbar()
		{
			if (wasError) {
				ShowErrorMessage(lastMessage);
			} else {
				SetMessage(lastMessage);
			}
		}
		
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
