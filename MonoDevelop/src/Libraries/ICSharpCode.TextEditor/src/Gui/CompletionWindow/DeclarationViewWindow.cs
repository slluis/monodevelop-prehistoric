// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Collections;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;
using ICSharpCode.TextEditor;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public class DeclarationViewWindow : Gtk.Widget
	{
		static GLib.GType type;
		string description = String.Empty;
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				if (Visible) {
					QueueDraw ();
				}
			}
		}
		
		static DeclarationViewWindow ()
		{
			type = RegisterGType (typeof (DeclarationViewWindow));
		}
		
		public DeclarationViewWindow() : base (type)
		{
#if !GTK		
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			
//			Enabled         = false;
			Size            = new Size(0, 0);
			
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
#endif
		}

#if !GTK
		protected override void OnPaint(PaintEventArgs pe)
		{
			TipPainterTools.DrawHelpTipFromCombinedDescription
				(this, pe.Graphics, Font, null, description);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			if (description != null && description.Length > 0) {
				pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
			}
		}
#endif
	}
}
