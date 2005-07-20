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

namespace MonoDevelop.Gui.Completion
{
	public class DeclarationViewWindow : Window
	{
		static char[] newline = {'\n'};
		static char[] whitespace = {' '};

		ArrayList overloads;
		int current_overload;

		Label headlabel, bodylabel, helplabel;
		Arrow left, right;
		VBox helpbox;
		
		public string DescriptionMarkup
		{
			get {
			 	if (bodylabel.Text == "")
					return headlabel.Text;
				else
					return headlabel.Text + "\n" + bodylabel.Text;
			}
			
			set {
				if (value == null) {
					headlabel.Markup = "";
					bodylabel.Markup = "";
					return;
				}

				string[] parts = value.Split (newline, 2);
				headlabel.Markup = parts[0].Trim (whitespace);
				bodylabel.Markup = (parts.Length == 2 ? parts[1].Trim (whitespace) : String.Empty);

				headlabel.Visible = headlabel.Text != "";
				bodylabel.Visible = bodylabel.Text != "";
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
				helpbox.Visible = value;
				
				//this could go somewhere better, as long as it's after realization
				headlabel.Visible = headlabel.Text != "";
				bodylabel.Visible = bodylabel.Text != "";
			}
		}

		public void AddOverload (string desc)
		{
			overloads.Add (desc);
			if (overloads.Count == 2) {
				Multiple = true;
			}
			ShowOverload ();
		}

		void ShowOverload ()
		{
			DescriptionMarkup = (string)overloads[current_overload];
			helplabel.Markup = String.Format ("<small>{0} of {1} overloads</small>", current_overload + 1, overloads.Count);
		}

		public void OverloadLeft ()
		{
			if (current_overload == 0)
				current_overload = overloads.Count - 1;
			else
				current_overload--;
			ShowOverload ();
		}

		public void OverloadRight ()
		{
			if (current_overload == overloads.Count - 1)
				current_overload = 0;
			else
				current_overload++;
			ShowOverload ();
		}

		public void Clear ()
		{
			overloads.Clear ();
			Multiple = false;
			DescriptionMarkup = String.Empty;
			current_overload = 0;
		}

		public DeclarationViewWindow () : base (WindowType.Popup)
		{
			overloads = new ArrayList ();
			this.AllowShrink = false;
			this.AllowGrow = false;

			headlabel = new Label ("");
			headlabel.LineWrap = false;
			headlabel.Xalign = 0;
			
			bodylabel = new Label ("");
			bodylabel.LineWrap = true;
			bodylabel.Xalign = 0;

			VBox vb = new VBox (false, 0);
			vb.PackStart (headlabel, false, true, 0);
			vb.PackStart (bodylabel, false, true, 0);

			left = new Arrow (ArrowType.Left, ShadowType.None);
			right = new Arrow (ArrowType.Right, ShadowType.None);

			HBox hb = new HBox (false, 0);
			hb.Spacing = 4;
			hb.PackStart (left, false, true, 0);
			hb.PackStart (vb, true, true, 0);
			hb.PackStart (right, false, true, 0);

			helplabel = new Label ("");
			helplabel.Xpad = 2;
			helplabel.Ypad = 2;
			helplabel.Xalign = 1;
			helplabel.UseMarkup = true;
			helplabel.Markup = "";
			
			helpbox = new VBox (false, 0);
			helpbox.PackStart (new HSeparator (), false, true, 0);
			helpbox.PackStart (helplabel, false, true, 0);
			
			VBox vb2 = new VBox (false, 0);
			vb2.Spacing = 4;
			vb2.PackStart (hb, false, true, 0);
			vb2.PackStart (helpbox, false, true, 0);

			Frame frame = new Frame ();
			frame.Add (vb2);
			
			this.Add (frame);
		}
	}
}
