// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using Gtk;
using Gdk;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class StatusPanel : Gtk.DrawingArea
	{
		WizardDialog wizard;
		Pixbuf bitmap = null;
		Gdk.GC gc;
		Pango.Layout ly;
		
		Pango.FontDescription smallFont;
		Pango.FontDescription normalFont;
		Pango.FontDescription boldFont;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		public StatusPanel(WizardDialog wizard)
		{
			smallFont  = Style.FontDescription;
			smallFont.Size = (int) (smallFont.Size * 0.75);
			normalFont = Style.FontDescription;
			boldFont   = Style.FontDescription;
			boldFont.Weight = Pango.Weight.Bold;
			
			this.wizard = wizard;
			RequestSize = new Size (198, 400);

			bitmap = resourceService.GetBitmap ("GeneralWizardBackground");

			AddEvents ((int) (Gdk.EventMask.ExposureMask));
			ExposeEvent += new GtkSharp.ExposeEventHandler (OnPaint);
			Realized += new EventHandler (OnRealized);
		}

		protected void OnRealized (object o, EventArgs args)
		{
			gc = new Gdk.GC (GdkWindow);
			ly = new Pango.Layout(PangoContext);
		}
		
		protected void OnPaint(object o, GtkSharp.ExposeEventArgs e)
		{
			GdkWindow.BeginPaintRect (e.Event.area);
				GdkWindow.DrawPixbuf (gc, bitmap, 0, 0, 0, 0, -1, -1, Gdk.RgbDither.None, 0, 0);
				smallFont.Weight = Pango.Weight.Normal;
				ly.FontDescription = smallFont;
				ly.SetText (resourceService.GetString("SharpDevelop.Gui.Dialogs.WizardDialog.StepsLabel"));
				int smallFontHeight = (int)(ly.Size.Height/1024.0f);
				GdkWindow.DrawLayout (gc, 10, 24 - smallFontHeight, ly);
				GdkWindow.DrawLine(gc, 10, 24, WidthRequest - 10, 24);
				
				int curNumber = 0;
				for (int i = 0; i < wizard.WizardPanels.Count; i = wizard.GetSuccessorNumber(i)) {
					IDialogPanelDescriptor descriptor = ((IDialogPanelDescriptor)wizard.WizardPanels[i]);
					ly.FontDescription = smallFont;
					if (wizard.ActivePanelNumber == i) {
						Pango.FontDescription tmpFont = smallFont.Copy ();
						tmpFont.Weight = Pango.Weight.Bold;
						ly.FontDescription = tmpFont;
						
					}
					ly.SetText ((1 + curNumber) + ". " + descriptor.Label);
					GdkWindow.DrawLayout(gc, 10, 40 + curNumber * smallFontHeight, ly);
					++curNumber;
				}
			GdkWindow.EndPaint ();
		}
		
	}
}
