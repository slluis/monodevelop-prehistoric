// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;
using GtkSharp;

using ICSharpCode.SharpDevelop.Internal.Undo;
using System.Drawing.Printing;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.HtmlControl;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserPane : AbstractViewContent
	{	
		protected HtmlViewPane htmlViewPane;
		
		public override Widget Control {
			get {
				return htmlViewPane;
			}
		}
		
		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		protected BrowserPane(bool showNavigation) //: base (type)
		{
			htmlViewPane = new HtmlViewPane(showNavigation);
			//htmlViewPane.HtmlControl.TitleChange += new DWebBrowserEvents2_TitleChangeEventHandler(TitleChange);
		}
		
		public BrowserPane() : this(true)
		{
		}
		
		public override void Dispose()
		{
			htmlViewPane.Dispose();
		}
		
		public override void Load(string url)
		{
			htmlViewPane.Navigate(url);
		}
		
		public override void Save(string url)
		{
			Load(url);
		}
		
		//void TitleChange(object sender, DWebBrowserEvents2_TitleChangeEvent e)
		//{
		//	ContentName = e.text;
		//}
	}
	
	public class HtmlViewPane : Gtk.Frame
	{
		HtmlControl htmlControl = null;
		
		VBox   topPanel   = new VBox (false, 2);
		Toolbar toolBar    = new Toolbar ();
		Entry urlTextBox = new Entry ();
		
		bool   isHandleCreated  = false;
		string lastUrl     = null;
		static GLib.GType type;
		
		public HtmlControl HtmlControl {
			get {
				return htmlControl;
			}
		}
		
		static HtmlViewPane ()
		{
			type = RegisterGType (typeof (HtmlViewPane));
		}
		
		public HtmlViewPane(bool showNavigation) : base ()
		{
			//RequestSize = new Size (500, 500);
			
			Shadow = Gtk.ShadowType.None;
			VBox mainbox = new VBox (false, 2);
			
			if (showNavigation) {
				
				//topPanel.RequestSize = new Size (Width, 25);
				//topPanel.Dock = DockStyle.Top;
				
				//toolBar.Dock = DockStyle.None;
				
				Button toolBarBack = new Button ();
				toolBarBack.Child = new Image (Stock.GoBack, IconSize.SmallToolbar);
				toolBarBack.Relief = ReliefStyle.None;
				toolBarBack.Clicked += new EventHandler (OnBackClicked);
				
				Button toolBarForward = new Button ();
				toolBarForward.Child = new Image (Stock.GoForward, IconSize.SmallToolbar);
				toolBarForward.Relief = ReliefStyle.None;
				toolBarForward.Clicked += new EventHandler (OnForwardClicked);
				
				Button toolBarStop = new Button ();
				toolBarStop.Child = new Image (Stock.Stop, IconSize.SmallToolbar);
				toolBarStop.Relief = ReliefStyle.None;
				toolBarStop.Clicked += new EventHandler (OnStopClicked);
				
				Button toolBarRefresh = new Button ();
				toolBarRefresh.Child = new Image (Stock.Refresh, IconSize.SmallToolbar);
				toolBarRefresh.Relief = ReliefStyle.None;
				toolBarRefresh.Clicked += new EventHandler (OnRefreshClicked);
			
				urlTextBox.WidthChars = 50;
				urlTextBox.Activated += new EventHandler (OnEntryActivated);
				
				toolBar.ToolbarStyle = ToolbarStyle.Icons;
				toolBar.IconSize = IconSize.SmallToolbar;
				toolBar.AppendWidget (toolBarBack, "Go Back", "");
				toolBar.AppendWidget (toolBarForward, "Go Forward", "");
				toolBar.AppendWidget (toolBarStop, "Stop Loading", "");
				toolBar.AppendWidget (toolBarRefresh, "Reload page", "");
				toolBar.AppendWidget (urlTextBox, "Location", "");
				
				topPanel.PackStart (toolBar);
				mainbox.PackStart (topPanel, false, false, 2);
			} 
			
			htmlControl = new HtmlControl ();
			htmlControl.ShowAll ();

			mainbox.PackStart (htmlControl);
			
			this.Add (mainbox);
			this.ShowAll ();
		}
		
		//void TitleChange(object sender, DWebBrowserEvents2_TitleChangeEvent e)
		//{
		//	urlTextBox.Text = axWebBrowser.LocationURL;
		//}
		
		void OnEntryActivated (object o, EventArgs args)
		{
			htmlControl.Url = urlTextBox.Text;
		}
		
		public void CreatedWebBrowserHandle(object sender, EventArgs evArgs) 
		{
			isHandleCreated = true;
			if (lastUrl != null) {
				Navigate(lastUrl);
			}
		}
		
		public void Navigate(string name)
		{
			urlTextBox.Text = name;
			htmlControl.Url = name;
		}
		
		private void OnBackClicked (object o, EventArgs args)
		{
			htmlControl.GoBack ();
		}
		
		private void OnForwardClicked (object o, EventArgs args)
		{
			htmlControl.GoForward ();
		}
		
		private void OnStopClicked (object o, EventArgs args)
		{
			htmlControl.Stop ();
		}
		
		private void OnRefreshClicked (object o, EventArgs args)
		{
			htmlControl.Refresh ();
		}
	}
}
