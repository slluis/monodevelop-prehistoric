//
// MozillaControl - An Html widget that uses GtkMozEmbed#
//
// Author: John Luke  <jluke@cfl.rr.com>
//
// Copyright 2003 John Luke
//

using System;
using GtkMozEmbed;
using GtkMozEmbedSharp;

namespace ICSharpCode.SharpDevelop.Gui.HtmlControl
{
	public class MozillaControl : EmbedWidget, IWebBrowser
	{
		private static GLib.GType type;
		private string html;
		
		static MozillaControl ()
		{
			type = RegisterGType (typeof (MozillaControl));
		}
		
		//FIXME: pick a better path, one of the Environment dirs
		public MozillaControl () : base (type)
		{
			EmbedWidget.SetProfilePath ("/tmp", "MonoDevelop");
		}
		
		public void GoHome ()
		{
			LoadUrl ("about:blank");
		}
		
		public void GoSearch ()
		{
		}
		
		public void Navigate (string Url, ref object Flags, ref object targetFrame, ref object postData, ref object headers)
		{
			// TODO: what is all that other crap for
			LoadUrl (Url);
		}
		
		public void Refresh ()
		{
			this.Reload ((int) ReloadFlags.Reloadnormal);
		}
		
		public void Refresh2 ()
		{
			this.Reload ((int) ReloadFlags.Reloadnormal);
		}
		
		public void Stop ()
		{
			this.StopLoad ();
		}
		
		public void GetApplication ()
		{
		}
		
		public void GetParent ()
		{
		}
		
		public void GetContainer ()
		{
		}
		
		public IHTMLDocument2 GetDocument ()
		{
			return null;
		}

		public string Html
		{
			get { return html; }
			set { html = value; }
		}

		public void DelayedInitialize ()
		{
			if (html.Length > 0)
			{
				this.RenderData (html, "file://", "text/html");
			}
		}
	}
}
