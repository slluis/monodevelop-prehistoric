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
		static char[] newline = {'\n'};
		static char[] whitespace = {' '};

		Label headlabel, bodylabel;
		Arrow left, right;
		
		public string DescriptionMarkup
		{
			get {
				return headlabel.Text + "\n" + bodylabel.Text;
			}
			
			set {
				string[] parts = value.Split (newline, 2);
				headlabel.Markup = parts[0].Trim (whitespace);
				bodylabel.Markup = (parts.Length == 2 ? parts[1].Trim (whitespace) : String.Empty);
				//QueueDraw ();
			}
		}

		public bool Multiple
		{
			get {
				return left.Visible;
			}

			set {
				left.Visible = value;
				right.Visible = value;
			}
		}
		
		public DeclarationViewWindow () : base (WindowType.Popup)
		{
			headlabel = new Label ("");
			headlabel.LineWrap = false;
			headlabel.Xalign = 0;
			
			bodylabel = new Label ("");
			bodylabel.LineWrap = true;
			bodylabel.Xalign = 0;
			
			VBox vb = new VBox (false, 0);
			vb.PackStart (headlabel, true, true, 0);
			vb.PackStart (bodylabel, true, true, 0);

			left = new Arrow (ArrowType.Left, ShadowType.None);
			right = new Arrow (ArrowType.Right, ShadowType.None);

			HBox hb = new HBox (false, 0);
			hb.Spacing = 4;
			hb.PackStart (left, false, true, 0);
			hb.PackStart (vb, true, true, 0);
			hb.PackStart (right, false, true, 0);

			Frame frame = new Frame ();
			frame.Add (hb);
			
			this.Add (frame);
		}
	}
}
