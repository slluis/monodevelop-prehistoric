// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Resources;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.HtmlControl;
using Gdk;
using Gtk;
using GtkSharp;
using Pango;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ScrollBox : DrawingArea
	{
		static GLib.GType type;
		Pixbuf image;
		string text;
		int scroll = -220;
		uint hndlr;
		Pango.Font font;
		Drawable dr;
		bool initial = true;
		Gdk.GC gc;
		
		public int ScrollY {
			get {
				return scroll;
			}
			set {
				scroll = value;
			}
		}
		
		public Pixbuf Image {
			get {
				return image;
			}
			set {
				image = value;
			}
		}
		
		public string ScrollText {
			get {
				return text;
			}
			set {
				text =  value;
			}
		}
		
		internal uint Handler
		{
			get { return hndlr; }
		}
		
		static ScrollBox ()
		{
			type = RegisterGType (typeof (ScrollBox));
		}
		
		public ScrollBox() : base (type)
		{
			this.RequestSize = new System.Drawing.Size (400, 220);
			this.Realized += new EventHandler (OnRealized);
			this.ExposeEvent += new ExposeEventHandler (OnExposed);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			this.Image = resourceService.GetBitmap ("Icons.AboutImage");
			
			text = "Ported and developed by:\nTodd Berman\nPedro Abelleira Seco\nJohn Luke\nDaniel Kornhauser\norph\nnricciar\njba\nBen Maurer";
			
			//text = "\"The most successful method of programming is to begin a program as simply as possible, test it, and then add to the program until it performs the required job.\"\n    -- PDP8 handbook, Pg 9-64\n\n\n";
			//text = "\"The primary purpose of the DATA statement is to give names to constants; instead of referring to pi as 3.141592653589793 at every\n appearance, the variable PI can be given that value with a DATA statement and used instead of the longer form of the constant. This also simplifies modifying the program, should the value of pi change.\"\n    -- FORTRAN manual for Xerox computers\n\n\n";
			//text = "\"No proper program contains an indication which as an operator-applied occurrence identifies an operator-defining occurrence which as an indication-applied occurrence identifies an indication-defining occurrence different from the one identified by the given indication as an indication- applied occurrence.\"\n   -- ALGOL 68 Report\n\n\n";
			//text = "\"The '#pragma' command is specified in the ANSI standard to have an arbitrary implementation-defined effect. In the GNU C preprocessor, `#pragma' first attempts to run the game rogue; if that fails, it tries to run the game hack; if that fails, it tries to run GNU Emacs displaying the Tower of Hanoi; if that fails, it reports a fatal error. In any case, preprocessing does not continue.\"\n   --From an old GNU C Preprocessor document";
			
			
			Gtk.Function ScrollHandler = new Gtk.Function (ScrollDown);
			hndlr = Timeout.Add (30, ScrollHandler);
		}
		
		bool ScrollDown ()
		{
			++scroll;
			// FIXME: only redraw the right side
			this.QueueDraw ();
			//this.QueueDrawArea (200, 0, 200, 220);
			return true;
		}
		
		private void DrawImage ()
		{
			if (image != null) {
				dr.DrawPixbuf (gc, Image, 0, 0, 0, 0, -1, -1, RgbDither.Normal,  0,  0);
				
			}
		}
		
		private void DrawText ()
		{
			Pango.Layout layout = new Pango.Layout (this.PangoContext);
			// FIXME: this seems wrong but works
			layout.Width = 253952;
			layout.Wrap = Pango.WrapMode.Word;
			FontDescription fd = FontDescription.FromString ("Tahoma 10");
			layout.FontDescription = fd;
			layout.SetText (text);
			dr.DrawLayout (gc, 200, 0 - scroll, layout);

			//Console.WriteLine (layout.Size);
			//Console.WriteLine (layout.Width);
			//Console.WriteLine (layout.LineCount);
				
			if (scroll > 220 ) {
				scroll = -scroll;
			}
		}
		
		protected void OnExposed (object o, ExposeEventArgs args)
		{
			this.DrawImage ();	
			this.DrawText ();
		}

		protected void OnRealized (object o, EventArgs args)
		{
			int xoff;
			int yoff;
			this.GdkWindow.GetInternalPaintInfo (out dr, out xoff, out yoff);
			gc = new Gdk.GC (dr);	
		}
	}
	
	public class CommonAboutDialog : Dialog
	{
		static GLib.GType type;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		AuthorAboutTabPage aatp;
		//ChangeLogTabPage changelog;
		ScrollBox aboutPictureScrollBox;
		
		public ScrollBox ScrollBox {
			get {
				return (ScrollBox) aboutPictureScrollBox;
			}
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		static CommonAboutDialog ()
		{
			type = RegisterGType (typeof (CommonAboutDialog));
		}
		
		public CommonAboutDialog() : base ("About MonoDevelop", (Gtk.Window) WorkbenchSingleton.Workbench, DialogFlags.DestroyWithParent)
		{
			ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService(typeof (IResourceService));
			aboutPictureScrollBox = new ScrollBox ();
		
			this.VBox.PackStart (aboutPictureScrollBox);
			Label copyright = new Label ("(c) 2000-2003 by icsharpcode.net");
			copyright.Justify = Justification.Left;
			this.VBox.PackStart (copyright, false, true, 0);
		
			Notebook nb = new Notebook ();
			nb.RequestSize = new System.Drawing.Size (400, 280);
			//nb.SwitchPage += new SwitchPageHandler (OnPageChanged);
			//aatp = new AuthorAboutTabPage ();
			//changelog = new ChangeLogTabPage ();
			VersionInformationTabPage vinfo = new VersionInformationTabPage ();
			
			nb.AppendPage (new AboutMonoDevelopTabPage (), new Label ("About MonoDevelop"));
			//nb.AppendPage (aatp, new Label ("Authors"));
			//nb.AppendPage (changelog, new Label ("ChangeLog"));
			nb.AppendPage (vinfo, new Label ("Version Info"));
			this.VBox.PackStart (nb);
			this.AddButton (Stock.Close, (int) ResponseType.Close);
			this.ShowAll ();
		}
		
		public new int Run ()
		{
			int tmp = base.Run ();
			Timeout.Remove (ScrollBox.Handler);
			return tmp;
		}
		
		//private void OnPageChanged (object o, SwitchPageArgs args)
		//{
			//if (args.PageNum == 1)
			//{
			//	aatp.DelayedInitialize ();
			//}
			//else if (args.PageNum == 2)
			//{
			//	changelog.DelayedInitialize ();
			//}
		//}
	}
}
