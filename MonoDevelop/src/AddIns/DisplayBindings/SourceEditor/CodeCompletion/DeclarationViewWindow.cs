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

using Gtk;

namespace MonoDevelop.SourceEditor.CodeCompletion
{
	public class DeclarationViewWindow : Window
	{
		Label label;
		
		public string DescriptionMarkup
		{
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
			Frame frame = new Frame ();
			frame.Add (label = new Label (""));
			this.Add (frame);
		}
	}
}
