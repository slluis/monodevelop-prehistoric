// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
 
using System;
using System.Collections;
using Gtk;

namespace MonoDevelop.Gui
{	
	/// <summary>
	/// DockingManager replacement
	/// </summary>
	public class DockingManager
	{
		private ContentList contents = new ContentList();

#if EXPERIMENTAL_DOCKER
		private Docker leftNotebook;
		private Docker rightNotebook;
		private Docker bottomNotebook;
		private Docker topNotebook;
#else
		private Gtk.Notebook leftNotebook;
		private Gtk.Notebook rightNotebook;
		private Gtk.Notebook bottomNotebook;
		private Gtk.Notebook topNotebook;
#endif
		
		private WindowContent leftWC = new WindowContent();
		private WindowContent rightWC = new WindowContent();
		private WindowContent bottomWC = new WindowContent();
		private WindowContent topWC = new WindowContent();
		
		public DockingManager(Gtk.VBox container, Gtk.Widget tabControl) {
#if EXPERIMENTAL_DOCKER
			leftNotebook = new Docker();
			rightNotebook = new Docker();
			bottomNotebook = new Docker();
#else
			leftNotebook = new Gtk.Notebook ();
			rightNotebook = new Gtk.Notebook ();
			bottomNotebook = new Gtk.Notebook ();
#endif

			leftNotebook.TabPos = rightNotebook.TabPos 
				= bottomNotebook.TabPos = Gtk.PositionType.Bottom;
			
			Gtk.VPaned vpaned = new Gtk.VPaned();
			//vpaned.Pack1(rightNotebook, true, true);
			vpaned.Pack1(tabControl, true, true);
			vpaned.Pack2(bottomNotebook, true, true);
			Gtk.HPaned hpaned = new Gtk.HPaned();
			
			hpaned.Pack1(leftNotebook, true, true);
			hpaned.Pack2(vpaned, true, true);
			hpaned.ShowAll();
			hpaned.Position = 150;
			vpaned.Position = 300;
			container.PackStart(hpaned, true, true, 0);
			container.ShowAll();
		}
		
		public ContentList Contents {
			get {
				return contents;
			}
		}
		
		public void ShowContent(Content c) {
			// TODO
		}
		
		public void HideContent(Content c) {
			// TODO
		}
		
		public WindowContent AddContentWithState(Content c, DockState state) {
			switch (state) {
				case DockState.Left:
					AppendContentToNotebook(c, leftNotebook);
					return leftWC;
					
				case DockState.Right:
					AppendContentToNotebook(c, rightNotebook);
					return rightWC;
				
				case DockState.Bottom:
					AppendContentToNotebook(c, bottomNotebook);
					return bottomWC;
					
				case DockState.Top:
					AppendContentToNotebook (c, topNotebook);
					return topWC;
					
				default:
					throw new Exception("Not supported DockState: " + state);
			}
		}
		
		public WindowContent AddContentToWindowContent(Content c, WindowContent w) {
			if (w == leftWC) {
				AppendContentToNotebook(c, leftNotebook);
				return leftWC;
			} else if (w == rightWC) {
				AppendContentToNotebook(c, rightNotebook);
				return rightWC;
			} else if (w == bottomWC) {
				AppendContentToNotebook(c, bottomNotebook);
				return bottomWC;
			} else {
				throw new Exception("Foreign WindowContent");
			}
		}
		
		private void AppendContentToNotebook(Content c, Gtk.Notebook n) {
			Gtk.HBox hb = new Gtk.HBox(false, 0);
			hb.PackStart(new Gtk.Image(c.Image));
			hb.PackStart(new Gtk.Label(c.Title));
			hb.ShowAll();
			n.AppendPage(c.Widget, hb);
			n.ShowAll();
		}
	}
	
	public class ContentList {
		private ArrayList contents = new ArrayList();
		
		public Content Add(Gtk.Widget widget, string title) {
			return Add(widget, title, null);
		}
		
		public Content Add(Gtk.Widget widget, string title, Gdk.Pixbuf image) {
			Content ret = new Content(widget, title, image);
			contents.Add(ret);
			return ret;
		}
		
	}
	
	public class WindowContent {
	
	}
	
	public class Content: WindowContent {
		private Gtk.Widget widget;
		private string title;
		private Gdk.Pixbuf image;
		private string fulltitle;
		
		public Content(Gtk.Widget widget, string title): this(widget, title, null) {
			// Nothing
		}
		
		public Content(Gtk.Widget widget, string title, Gdk.Pixbuf image) {
			this.widget = widget;
			this.title = title;
			this.image = image;
		}
		
		public string Title {
			get {
				return title;
			}
			set {
				title = value;
			}
		}
		
		public string FullTitle {
			get {
				return fulltitle;
			}
			
			set {
				fulltitle = value;
			}
		}
		
		public Gtk.Widget Widget {
			get {
				return widget;
			}
		}
		
		public Gdk.Pixbuf Image {
			get {
				return image;
			}
		}
		
		public bool Visible {
			get {
				return true; // TODO
			}
		}
		
		public void BringToFront() {
			// TODO
		}
	}

	public enum DockState {
		Left,
		Right,
		Bottom,
		Top,
	}
}
