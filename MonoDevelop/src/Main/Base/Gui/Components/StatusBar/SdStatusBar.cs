// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
//using System.Drawing;
//using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Services;
using Gtk;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdStatusBar : HBox, IProgressMonitor
	{
		ProgressBar statusProgressBar      = new ProgressBar ();
		
		Statusbar txtStatusBarPanel    = new Statusbar ();
		Statusbar cursorStatusBarPanel = new Statusbar ();
		Statusbar modeStatusBarPanel   = new Statusbar ();
		private static GLib.GType type;

		
		public Statusbar CursorStatusBarPanel {
			get {
				return cursorStatusBarPanel;
			}
		}
		
		public Statusbar ModeStatusBarPanel {
			get {
				return modeStatusBarPanel;
			}
		}
	
		bool cancelEnabled;
		
		public bool CancelEnabled {
			get {
				return cancelEnabled;
			}
			set {
				cancelEnabled = value;
			}
		}

		static SdStatusBar ()
		{
			type = RegisterGType (typeof (SdStatusBar));
		}
		
		public SdStatusBar(IStatusBarService manager) : base (type)
		{
			//txtStatusBarPanel.Width = 500;
			//txtStatusBarPanel.AutoSize = StatusBarPanelAutoSize.Spring;
			txtStatusBarPanel.HasResizeGrip = false;
			//txtStatusBarPanel.Push (1, "test");
			this.PackStart (txtStatusBarPanel);
	//		manager.Add (new StatusBarContributionItem("TextPanel", txtStatusBarPanel));
			
			//statusProgressBar.Width  = 200;
			//statusProgressBar.Height = 14;
			//statusProgressBar.Location = new Point(160, 4);
			//statusProgressBar.Minimum = 0;
			statusProgressBar.Visible = false;
			//this.PackStart (statusProgressBar, true, false, 0);
			
			//cursorStatusBarPanel.Width = 200;
			//cursorStatusBarPanel.AutoSize = StatusBarPanelAutoSize.None;
			//cursorStatusBarPanel.Alignment = HorizontalAlignment.Left;
			cursorStatusBarPanel.HasResizeGrip = false;
			//cursorStatusBarPanel.Push (1, "test");
			this.PackStart (cursorStatusBarPanel, true, true, 0);
				
			//modeStatusBarPanel.Width = 44;
			//modeStatusBarPanel.AutoSize = StatusBarPanelAutoSize.None;
			//modeStatusBarPanel.Alignment = HorizontalAlignment.Right;
			modeStatusBarPanel.HasResizeGrip = false;
			//modeStatusBarPanel.Push (1, "test");
			this.PackStart (modeStatusBarPanel, true, true, 0);
			
			
			//manager.Add(new StatusBarContributionItem("ProgressBar", statusProgressBar));
			
			//ShowPanels = true;
			this.ShowAll ();
		}
		
		public void ShowErrorMessage(string message)
		{
			txtStatusBarPanel.Push (1, "Error : " + message);
		}
		
		public void ShowErrorMessage(Image image, string message)
		{
			txtStatusBarPanel.Push (1, "Error : " + message);
		}
		
		public void SetMessage(string message)
		{
			txtStatusBarPanel.Push (1, message);
		}
		
		public void SetMessage(Image image, string message)
		{
			txtStatusBarPanel.Push (1, message);
		}
		
		// Progress Monitor implementation
		string oldMessage = null;
		public void BeginTask(string name, int totalWork)
		{
			//oldMessage = txtStatusBarPanel.Pop (1);
			SetMessage(name);
			//statusProgressBar.Maximum = totalWork;
			statusProgressBar.Visible = true;
		}
		
		public void Worked(int work, string status)
		{
			statusProgressBar.Fraction = (double) work;
			statusProgressBar.Text = status;
		}
		
		public void Done()
		{
			SetMessage(oldMessage);
			oldMessage = null;
			statusProgressBar.Visible = false;
		}
		
		public bool Canceled {
			get {
				return oldMessage == null;
			}
			set {
				Done();
			}
		}
		
		public string TaskName {
			get {
				return "";
			}
			set {
				
			}
		}
		
		//public void Dispose() {
			// FIXME PEDRO
		//}
	}
}
