// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;

namespace ResEdit
{
	/// <summary>
	/// This class displays a bitmap in a window, the window and the bitmap can be resized.
	/// </summary>
	class BitmapView : Form
	{
		Bitmap bitmap;
		Font   font;
		
		public BitmapView(string text, Bitmap bitmap, Icon icon)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			font = resourceService.LoadFont("Tahoma", 8);

			this.Text   = text;
			this.bitmap = bitmap;
			
			ClientSize    = new Size(bitmap.Size.Width + 25, bitmap.Size.Height + 25);
			StartPosition = FormStartPosition.CenterScreen;
			TopMost       = true;
			MaximizeBox   = false;
			MinimizeBox   = false;
			Icon          = icon;
			Owner         = (Form)WorkbenchSingleton.Workbench;
		}
		
		protected override void OnResize(EventArgs e)
		{
			Refresh();
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			
			// calculate zoom factor 
			int BitmapWitdh  = ClientSize.Width - 20;
			int BitmapHeight = ClientSize.Height - 20;
			
			float factor1 = (float)BitmapWitdh / bitmap.Width;
			float factor2 = (float)BitmapHeight / bitmap.Height;
			float factor  = Math.Min(factor1, factor2); // always take the minimum zoom factor -> zoomed bitmap fits in the window
			BitmapWitdh  = (int)factor * bitmap.Width;
			BitmapHeight = (int)factor * bitmap.Height;
			
			g.InterpolationMode = InterpolationMode.NearestNeighbor; // Interpolation doesn't look nice for icons, so I turn it off.
			
			// draw white window background
			g.FillRectangle(new SolidBrush(Color.White), e.ClipRectangle);
			
			// calculate bitmap position
			Point p = new Point((ClientSize.Width  - BitmapWitdh) / 2, (ClientSize.Height - BitmapHeight) / 2);
			
			// draw "transparent" color (transparent pixels are DarkCyan)
			g.FillRectangle(Brushes.DarkCyan, p.X, p.Y, BitmapWitdh, BitmapHeight);
			
			// draw Image
			g.DrawImage(bitmap, p.X, p.Y, BitmapWitdh, BitmapHeight);
			
			// draw Image Border
			g.DrawRectangle(Pens.Black, p.X - 1, p.Y - 1, BitmapWitdh + 1, BitmapHeight + 1);
			
			// draw Size
			//g.DrawString("Width: " + bitmap.Size.Width.ToString() + ", Height: " + bitmap.Size.Height.ToString(), 
			//	         font, 
			//	         new SolidBrush(Color.Black), 0, 0);
			
		}
	}
}
