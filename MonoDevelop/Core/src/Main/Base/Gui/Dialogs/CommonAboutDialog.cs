// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;

using MonoDevelop.Gui;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Services;
//using MonoDevelop.Gui.HtmlControl;

using Gdk;
using Gtk;
using Pango;

namespace MonoDevelop.Gui.Dialogs
{
	public class ScrollBox : DrawingArea
	{
		Pixbuf image;
		int scroll = -220;
		uint hndlr;
		Pango.Font font;
		bool initial = true;
		Pango.Layout layout;
		
		internal uint Handler
		{
			get { return hndlr; }
		}

		string[] authors = new string[]
		{
			"Todd Berman",
			"Pedro Abelleira Seco",
			"John Luke",
			"Daniel Kornhauser",
			"Alex Graveley",
			"nricciar",
			"John Bou Antoun",
			"Ben Maurer",
			"Jeroen Zwartepoorte",
			"Gustavo Giráldez",
			"Miguel de Icaza",
			"Inigo Illan",
			"Iain McCoy",
			"Nick Drochak",
			"Paweł Różański",
			"Richard Torkar",
			"Erik Dasque",
			"Paco Martinez",
			"Lluis Sanchez Gual"
		};
		
		public ScrollBox ()
		{
			this.SetSizeRequest (400, 220);
			this.Realized += new EventHandler (OnRealized);
			this.ExposeEvent += new ExposeEventHandler (OnExposed);
			
			image = Runtime.Gui.Resources.GetBitmap ("Icons.AboutImage");
			
			Gtk.Function ScrollHandler = new Gtk.Function (ScrollDown);
			hndlr = Timeout.Add (30, ScrollHandler);
		}

		string CreditText {
			get {
				StringBuilder sb = new StringBuilder ();
				sb.Append ("<b>Ported and developed by:</b>\n");

				foreach (string s in authors)
				{
					sb.Append (s);
					sb.Append ("\n");
				}

				string trans = GettextCatalog.GetString ("translator-credits");
				if (trans != "translator-credits")
				{
					sb.Append ("\n\n<b>Translated by:</b>\n");
					sb.Append (trans);
				}
				return sb.ToString ();
			}
		}
		
		bool ScrollDown ()
		{
			++scroll;
			this.QueueDrawArea (200, 0, 200, 220);
			return true;
		}
		
		private void DrawImage ()
		{
			if (image != null) {
				this.GdkWindow.DrawPixbuf (this.Style.BackgroundGC (StateType.Normal), image, 0, 0, 0, 0, -1, -1, RgbDither.Normal,  0,  0);
			}
		}

		int GetTextHeight ()
		{
			// FIXME: calculate this
			// nameCount * lineHeight
			return 325;
		}
		
		private void DrawText ()
		{
			this.GdkWindow.DrawLayout (this.Style.TextGC (StateType.Normal), 200, 0 - scroll, layout);
	
			if (scroll > GetTextHeight ())
				scroll = -scroll;
		}
		
		protected void OnExposed (object o, ExposeEventArgs args)
		{
			this.DrawImage ();	
			this.DrawText ();
		}

		protected void OnRealized (object o, EventArgs args)
		{
			layout = new Pango.Layout (this.PangoContext);
			// FIXME: this seems wrong but works
			layout.Width = 253952;
			layout.Wrap = Pango.WrapMode.Word;
			FontDescription fd = FontDescription.FromString ("Tahoma 10");
			layout.FontDescription = fd;
			layout.SetMarkup (CreditText);	
		}
	}
	
	public class CommonAboutDialog : Dialog
	{
		
		AuthorAboutTabPage aatp;
		//ChangeLogTabPage changelog;
		ScrollBox aboutPictureScrollBox;
		
		//static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public CommonAboutDialog ()
		{
			this.Title = GettextCatalog.GetString ("About MonoDevelop");
			this.TransientFor = (Gtk.Window) WorkbenchSingleton.Workbench;
			aboutPictureScrollBox = new ScrollBox ();
		
			this.VBox.PackStart (aboutPictureScrollBox, false, false, 0);
		
			Notebook nb = new Notebook ();
			nb.SetSizeRequest (400, 280);
			//nb.SwitchPage += new SwitchPageHandler (OnPageChanged);
			//aatp = new AuthorAboutTabPage ();
			//changelog = new ChangeLogTabPage ();
			VersionInformationTabPage vinfo = new VersionInformationTabPage ();
			
			nb.AppendPage (new AboutMonoDevelopTabPage (), new Label (GettextCatalog.GetString ("About MonoDevelop")));
			//nb.AppendPage (aatp, new Label ("Authors"));
			//nb.AppendPage (changelog, new Label ("ChangeLog"));

			nb.AppendPage (vinfo, new Label (GettextCatalog.GetString ("Version Info")));
			this.VBox.PackStart (nb, true, true, 0);
			this.AddButton (Gtk.Stock.Close, (int) ResponseType.Close);
			this.ShowAll ();
		}
		
		public new int Run ()
		{
			int tmp = base.Run ();
			Timeout.Remove (aboutPictureScrollBox.Handler);
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
