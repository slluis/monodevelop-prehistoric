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
		string description = "";
		
		Label label;
		
		public string Description {
			get {
				return label.Text;
			}
			
			set {
				label.Text = value;
				QueueDraw ();
			}
		}
		
		public DeclarationViewWindow () : base (WindowType.Popup)
		{
			BorderWidth = 4;
			
			Add (label = new Label (description));
		}
	}
}
