//
// CircleRenderer.cs
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// (C) 2003 Ximian, Inc (http://www.ximian.com)
//
using System;
using GLib;
using Gdk;
using Gtk;

using MonoDevelop.Services;
using MonoDevelop.Core.Services;

namespace MonoDevelop.Services.Nunit
{
	enum CircleColor
	{
		None,
		Failure,
		NotRun,
		Success
	}

	class CircleRenderer
	{
		static TreeCellDataFunc dataFunc = null;
		static Pixbuf [] circles = null;
		static string [] colors = null;
		static ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService (typeof (ResourceService));

		static void Init ()
		{
			if (circles != null)
				return;

			circles = new Pixbuf [4];
			circles [(int) CircleColor.None] = resourceService.GetIcon ("MonoDevelop.Nunit.None.png");
			circles [(int) CircleColor.Failure] = resourceService.GetIcon ("MonoDevelop.Nunit.Red.png");
			circles [(int) CircleColor.NotRun] = resourceService.GetIcon ("MonoDevelop.Nunit.Yellow.png");
			circles [(int) CircleColor.Success] = resourceService.GetIcon ("MonoDevelop.Nuint.Green.png");
		}
		
		static void SetCellData (TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			try {
				CellRendererPixbuf cr = (CellRendererPixbuf) cell;
				cr.Pixbuf = circles [(int) model.GetValue (iter, 0)];
			} catch (Exception e) {
				Console.WriteLine (e);
				Console.WriteLine ();
			}
		}

		public static Gtk.TreeCellDataFunc CellDataFunc {
			get {
				if (dataFunc == null) {
					dataFunc = new TreeCellDataFunc (SetCellData);
					Init ();
				}

				return dataFunc;
			}
		}
	}
}

