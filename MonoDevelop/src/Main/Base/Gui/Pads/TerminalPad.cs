using System;

using MonoDevelop.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui;

using Gtk;
using Vte;

namespace MonoDevelop.Gui.Pads
{
	public class TerminalPad : IPadContent
	{
		Frame frame = new Frame ();
		VScrollbar vscroll;
		Terminal term;

		ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService(typeof(IResourceService));
		TaskService taskService = (TaskService) MonoDevelop.Core.Services.ServiceManager.Services.GetService (typeof (TaskService));
		IProjectService projectService = (IProjectService) ServiceManager.Services.GetService (typeof (IProjectService));
		PropertyService propertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));
		
		public Widget Control {
			get {
				return frame;
			}
		}
		
		public string Title {
			get {
				//FIXME: check the string
				return GettextCatalog.GetString ("Output");
			}
		}
		
		public string Icon {
			get {
				return MonoDevelop.Gui.Stock.OutputIcon;
			}
		}
		
		public void Dispose ()
		{
		}
		
		public void RedrawContent ()
		{
			OnTitleChanged (null);
			OnIconChanged (null);
		}
		
		public TerminalPad ()
		{
			//FIXME look up most of these in GConf
			term = new Terminal ();
			term.ScrollOnKeystroke = true;
			term.CursorBlinks = true;
			term.MouseAutohide = true;
			term.FontFromString = "monospace 12";
			term.Encoding = "UTF-8";
			term.BackspaceBinding = TerminalEraseBinding.Auto;
			term.DeleteBinding = TerminalEraseBinding.Auto;
			term.Emulation = "xterm";

			Gdk.Color fgcolor = new Gdk.Color (0, 0, 0);
			Gdk.Color bgcolor = new Gdk.Color (0xff, 0xff, 0xff);
			Gdk.Colormap colormap = Gdk.Colormap.System;
			colormap.AllocColor (ref fgcolor, true, true);
			colormap.AllocColor (ref bgcolor, true, true);
			term.SetColors (fgcolor, bgcolor, fgcolor, 16);
			term.SetSize (50, 5);

			term.Commit += new Vte.CommitHandler (OnTermCommit);
			term.RestoreWindow += new EventHandler (OnRestoreWindow);
			term.ChildExited += new EventHandler (OnChildExited);
			term.LowerWindow += new EventHandler (OnTermLower);
			term.RaiseWindow += new EventHandler (OnTermRaise);
//			term.TextModified += new EventHandler (OnTermTextModified);

			vscroll = new VScrollbar (term.Adjustment);

			HBox hbox = new HBox ();
			hbox.PackStart (term, true, true, 0);
			hbox.PackStart (vscroll, false, true, 0);

			frame.ShadowType = Gtk.ShadowType.In;
			frame.Add (hbox);
			
			taskService.CompilerOutputChanged += new EventHandler (SetOutput);
			projectService.StartBuild += new EventHandler (SelectMessageView);
			projectService.CombineClosed += new CombineEventHandler (OnCombineClosed);
			projectService.CombineOpened += new CombineEventHandler (OnCombineOpen);
		}

/*
		void OnTermTextModified (object o, EventArgs args)
		{
		}
*/
		void OnChildExited (object o, EventArgs args)
		{
			// full reset
			term.Reset (true, true);
		}

		void OnTermLower (object o, EventArgs args)
		{
			Console.WriteLine ("term lower");
		}

		void OnTermRaise (object o, EventArgs args)
		{
			Console.WriteLine ("term raise");
		}

		void OnRestoreWindow (object o, EventArgs args)
		{
			Console.WriteLine ("restore window");
		}

		void OnTermCommit (object o, Vte.CommitArgs args)
		{
			Terminal t = (Terminal) o;
        	if (args.P0 == "\r")
        	{
            	//FIXME: maybe a setting somewhere
            	t.Feed ("\r\n");
        	    return;
    	    }
                                                                         
	        t.Feed (args.P0);
		}
		
		void OnCombineOpen (object sender, CombineEventArgs e)
		{
			term.Reset (true, true);
		}
		
		void OnCombineClosed (object sender, CombineEventArgs e)
		{
			term.Reset (true, false);
		}
		
		void SelectMessageView (object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible (this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
			}
			else { 
				if ((bool) propertyService.GetProperty ("SharpDevelop.ShowOutputWindowAtBuild", true)) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad (this);
					WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
				}
			}
		}
		
		void SetOutput2 ()
		{
			term.Feed (taskService.CompilerOutput.Replace ("\n", "\r\n"));
		}
		
		void SetOutput (object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible (this)) {
				SetOutput2 ();
			}
			else {
				term.Feed (taskService.CompilerOutput.Replace ("\n", "\r\n"));
			}
		}
		
		protected virtual void OnIconChanged (EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged (this, e);
			}
		}

		protected virtual void OnTitleChanged (EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged (this, e);
			}
		}

		public event EventHandler IconChanged;
		public event EventHandler TitleChanged;

		public void BringToFront()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible (this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad (this);
			}

			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
		}
	}
}

