// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using MonoDevelop.Services;
using Gtk;
using Gnome;

namespace MonoDevelop.Gui.Components
{
	public class SdStatusBar : AppBar, IProgressMonitor
	{
		Statusbar txtStatusBarPanel    = new Statusbar ();
		Statusbar cursorStatusBarPanel = new Statusbar ();
		Statusbar modeStatusBarPanel   = new Statusbar ();
		private static GLib.GType gtype;
		
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

		public static new GLib.GType GType {
			get {
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (SdStatusBar));
				return gtype;
			}
		}
		
		// FIXME: actually use a Progressbar
		public SdStatusBar(IStatusBarService manager) : base (false, true, PreferencesType.Never)
		{
			txtStatusBarPanel.HasResizeGrip = false;
			this.PackStart (txtStatusBarPanel);
			
			cursorStatusBarPanel.HasResizeGrip = false;
			this.PackStart (cursorStatusBarPanel, true, true, 0);
				
			modeStatusBarPanel.HasResizeGrip = false;
			this.PackStart (modeStatusBarPanel, true, true, 0);

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
			this.Progress.Visible = true;
		}
		
		public void Worked(int work, string status)
		{
			this.Progress.Fraction = (double) work;
			this.Progress.Text = status;
		}
		
		public void Done()
		{
			SetMessage(oldMessage);
			oldMessage = null;
			this.Progress.Visible = false;
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
	}
}
