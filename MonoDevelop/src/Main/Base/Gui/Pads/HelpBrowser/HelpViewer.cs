using System;

using Gtk;
using Monodoc;

using MonoDevelop.Gui;

namespace MonoDevelop.Gui
{

	public class HelpViewer : AbstractViewContent
	{

		HTML html_viewer = new HTML ();
		string CurrentUrl;

		ScrolledWindow scroller = new ScrolledWindow ();

		public override Gtk.Widget Control {
			get { return scroller; }
		}

		public override string ContentName {
			get { return "Documentation"; }
		}

		public HelpViewer ()
		{
			scroller.Add (html_viewer);
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
