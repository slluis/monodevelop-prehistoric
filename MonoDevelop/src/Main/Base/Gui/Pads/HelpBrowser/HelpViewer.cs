using System;
using System.IO;

using Gtk;
using Monodoc;

using MonoDevelop.Gui;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;

namespace MonoDevelop.Gui
{

	public class HelpViewer : AbstractViewContent
	{

		HTML html_viewer = new HTML ();
		string CurrentUrl;

		ScrolledWindow scroller = new ScrolledWindow ();

		MonodocService mds;
		IStatusBarService statusBarService = (IStatusBarService)        MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));

		public override Gtk.Widget Control {
			get { return scroller; }
		}

		public override string ContentName {
			get { return "Documentation"; }
		}

		public HelpViewer ()
		{
			mds = (MonodocService)ServiceManager.Services.GetService (typeof (MonodocService));
	
			html_viewer.LinkClicked += new LinkClickedHandler (LinkClicked);
			html_viewer.UrlRequested += new UrlRequestedHandler (UrlRequested);
			html_viewer.OnUrl += new OnUrlHandler (OnUrl);
			scroller.Add (html_viewer);
		}

		void OnUrl (object sender, OnUrlArgs args)
		{
			if (args.Url == null)
				statusBarService.SetMessage ("");
			else
				statusBarService.SetMessage (args.Url);
		}

		void UrlRequested (object sender, UrlRequestedArgs args)
		{
			Console.WriteLine ("Image requested: " + args.Url);
			Stream s = mds.HelpTree.GetImage (args.Url);
			
			/*if (s == null)
				s = GetResourceImage ("monodoc.png");
				byte [] buffer = new byte [8192];
				int n;

				while ((n = s.Read (buffer, 0, 8192)) != 0) {
				args.Handle.Write (buffer, n);
			}*/
			args.Handle.Close (HTMLStreamStatus.Ok);
		}

		void LinkClicked (object o, LinkClickedArgs args)
		{
			LoadUrl (args.Url);
		}
 
		public void LoadUrl (string url)
		{
			if (url.StartsWith("#"))
			{
				html_viewer.JumpToAnchor(url.Substring(1));
				return;
			}

			Node node;
			
			string res = mds.HelpTree.RenderUrl (url, out node);
			if (res != null) {
				Render (res, node, url);
			}
                }

        	public void Render (string text, Node matched_node, string url)
        	{
        	        CurrentUrl = url;
        	        
			Gtk.HTMLStream stream = html_viewer.Begin ("text/html");
        	        
			stream.Write ("<html><body>");
        	        stream.Write (text);
        	        stream.Write ("</body></html>");
        	        html_viewer.End (stream, HTMLStreamStatus.Ok);
		}

		public override void Load (string s)
		{
		}

	}

}
