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
		bool cancelEnabled;
		const int ctx = 1;
		private static GLib.GType gtype;
		
		/*
		public Statusbar CursorStatusBarPanel
		{
			get {
				return cursorStatusBarPanel;
			}
		}*/
		
		public Statusbar ModeStatusBarPanel
		{
			get {
				return modeStatusBarPanel;
			}
		}
		
		public bool CancelEnabled
		{
			get {
				return cancelEnabled;
			}
			set {
				cancelEnabled = value;
			}
		}

		public static new GLib.GType GType
		{
			get {
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (SdStatusBar));
				return gtype;
			}
		}
		
		public SdStatusBar(IStatusBarService manager) : base (true, true, PreferencesType.Never)
		{
			txtStatusBarPanel.HasResizeGrip = false;
			this.PackStart (txtStatusBarPanel);
			
			cursorStatusBarPanel.HasResizeGrip = false;
			this.PackStart (cursorStatusBarPanel, true, true, 0);
				
			modeStatusBarPanel.HasResizeGrip = false;
			this.PackStart (modeStatusBarPanel, true, true, 0);

			Progress.Visible = false;
			Progress.PulseStep = 0.3;
			
			this.ShowAll ();
		}
		
		public void ShowErrorMessage(string message)
		{
			txtStatusBarPanel.Push (ctx, String.Format (GettextCatalog.GetString ("Error : {0}"), message));
		}
		
		public void ShowErrorMessage(Image image, string message)
		{
			txtStatusBarPanel.Push (ctx, String.Format (GettextCatalog.GetString ("Error : {0}"), message));
		}
		
		public void SetCursorPosition (int ln, int col, int ch)
		{
			cursorStatusBarPanel.Push (ctx, String.Format (GettextCatalog.GetString ("ln {0} col {1} ch {2}"), ln, col, ch));
		}
		
		public void SetMessage (string message)
		{
			txtStatusBarPanel.Push (ctx, message);
		}
		
		public void SetMessage (Image image, string message)
		{
			txtStatusBarPanel.Push (ctx, message);
		}
		
		// Progress Monitor implementation
		public void BeginTask (string name, int totalWork)
		{
			SetMessage (name);
			this.Progress.Visible = true;
		}
		
		public void Worked (double work, string status)
		{
			this.Progress.Fraction = work;
			this.Progress.Text = status;
		}
		
		public void Done ()
		{
			txtStatusBarPanel.Pop (ctx);
			this.Progress.Visible = false;
		}

		public void Pulse ()
		{
			this.Progress.Visible = true;
			this.Progress.Pulse ();
		}
		
		
		public bool Canceled
		{
			get {
				return true;
			}
			set {
				Done ();
			}
		}
		
		public string TaskName
		{
			get {
				return "";
			}
			set {
				
			}
		}
	}
}
