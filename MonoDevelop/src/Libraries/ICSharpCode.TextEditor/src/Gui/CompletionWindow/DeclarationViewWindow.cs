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

using Gtk;
using GtkSharp;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public class DeclarationViewWindow : Gtk.Window
	{
		Label label;
		
		public string DescriptionMarkup {
			get {
				return label.Text;
			}
			
			set {
				label.Markup = value;
				//QueueDraw ();
			}
		}
		
		public DeclarationViewWindow () : base (WindowType.Popup)
		{
			Gtk.Frame frame = new Gtk.Frame ();
			frame.Add (label = new Label (""));
			Add (frame);
		}
	}
}
