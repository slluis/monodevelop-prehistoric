using System;
using System.Threading;
using System.IO;
using Gtk;
using GtkSharp;

namespace ICSharpCode.SharpDevelop.Gui.HtmlControl
{
	public class HtmlControl : Widget, IWebBrowserEvents
	{
		public const int OLEIVERB_UIACTIVATE = -4;
				
		IWebBrowser control = null;
		//AxHost.ConnectionPointCookie cookie;
		
		string url           = "";
		string html          = "";
		string cssStyleSheet = "";
		bool windows;
		bool initialized     = false;
		
		public HtmlControl() //: base("8856f961-340a-11d0-a96b-00c04fd705a2")
		{
			if ((int) Environment.OSVersion.Platform != 128)
			{
				windows = true;
				//Console.WriteLine ("using IE for HtmlControl");
			}
			else
			{
				//Console.WriteLine ("using Mozilla for HtmlControl");
			}
			
			AttachInterfaces ();
		}
		
		public virtual void RaiseNavigateComplete(string url)
		{
			BrowserNavigateEventArgs e = new BrowserNavigateEventArgs(url, false);
			if (NavigateComplete != null) {
				NavigateComplete(this, e);
			}
		}
		
		public virtual void RaiseBeforeNavigate(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool cancel)
		{
			if (initialized) {
				BrowserNavigateEventArgs e = new BrowserNavigateEventArgs(url, false);
				if (BeforeNavigate != null) {
					BeforeNavigate(this, e);
				}
				cancel = e.Cancel;
			}
		}
		
		public string CascadingStyleSheet {
			get {
				return cssStyleSheet;
			}
			set {
				cssStyleSheet = value;
				ApplyCascadingStyleSheet();
			}
		}
		
		public string Url {
			set {
				this.url = value;
				if (!windows)
					((MozillaControl) control).LoadUrl (value);
			}
		}
		
		public string Html {
			set {
				this.html = value;
				if (windows)
					ApplyBody(html);
			}
		}
		
		protected void DetachSink()
		{
			try {
			//	this.cookie.Disconnect();
			} catch {
			}
		}
		
		protected void CreateSink()
		{
			try {
			//	this.cookie = new ConnectionPointCookie(this.GetOcx(), this, typeof(IWebBrowserEvents));
			} catch {
			}
		}
		
		protected void AttachInterfaces()
		{
			try {
				if (windows)
				{
					this.control = (IWebBrowser)this.GetOcx();
				}
				else
				{
					this.control = new MozillaControl ();
					//Console.WriteLine ("new MozillaControl");
					((MozillaControl) this.control).Show ();
				}
			} catch {
			}
		}
		
		protected IHTMLDocument2 GetOcx ()
		{
			return null;
			//if (windows)
				//GetOcx ();
		}
		
		protected void OnHandleCreated(EventArgs e)
		{
			//base.OnHandleCreated(e);
			
			NavigateComplete += new BrowserNavigateEventHandler(DelayedInitializeCaller);
			
			object flags       = 0;
			object targetFrame = String.Empty;
			object postData    = String.Empty;
			object headers     = String.Empty;
			this.control.Navigate("about:blank", ref flags, ref targetFrame, ref postData, ref headers);
		}
		
		void DelayedInitializeCaller(object sender, BrowserNavigateEventArgs e)
		{
			//MethodInvoker mi = new MethodInvoker(this.DelayedInitialize);
			//this.BeginInvoke(mi);
			NavigateComplete -= new BrowserNavigateEventHandler(DelayedInitializeCaller);
		}
		
		public void DelayedInitialize()
		{
			initialized = true;
			if (html.Length > 0) {
				ApplyBody(html);
			}
			UIActivate();
			ApplyCascadingStyleSheet();
		}
		
		void UIActivate()
		{
			//this.DoVerb(OLEIVERB_UIACTIVATE);
		}
		
		void ApplyBody(string val)
		{
		    try {
				if (control != null) {
					if (!windows)
					{
						((MozillaControl) control).RenderData (val, "file://", "text/html");
						return;
					}
					
					IHTMLElement el    = null;
					IHTMLDocument2 doc = this.control.GetDocument();
					
					if (doc != null) {
						el = doc.GetBody();
					}
					
					if (el != null) {
						UIActivate();
						el.SetInnerHTML(val);
						return;
					}
					else
					{
						Console.WriteLine ("IHTMLElement is null");
					}
				}
				else
				{
					Console.WriteLine ("control is null");
				}
			} catch {}
		}
		
		void ApplyCascadingStyleSheet()
		{
			if (control != null) {
				IHTMLDocument2 htmlDoc = control.GetDocument();
				if (htmlDoc != null) {
					htmlDoc.CreateStyleSheet(cssStyleSheet, 0);
				}
			}
		}
		
		public Gtk.Widget Control
		{
			get
			{
				if (windows)
					return null; //FIXME
				else
				{	
					return (MozillaControl) control;
				}
			}
		}
		
		public event BrowserNavigateEventHandler BeforeNavigate;
		public event BrowserNavigateEventHandler NavigateComplete;
	}
}
