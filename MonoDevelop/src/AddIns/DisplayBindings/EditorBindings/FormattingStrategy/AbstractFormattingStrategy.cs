using Gdk;
using Gtk;
using GtkSharp;
using System;

namespace MonoDevelop.EditorBindings.FormattingStrategy {
	public abstract class AbstractFormattingStrategy {
		
		public abstract bool KeyPressed (TextView view, TextBuffer buffer, KeyPressEventArgs args);
		
		public string GetLineIndentation (TextIter line)
		{
			line.LineOffset = 0;
			
			// `if iter was already on line 0, but not at the start of the line, iter is
			// snapped to the start of the line and the function returns TRUE.'
			
			if (! line.BackwardLine ())
				return "";
			
			line.LineOffset = 0;
			
			TextIter end = line;
			
			while (! end.StartsWord () && ! end.EndsLine ())
				end.ForwardChar ();
			
			return line.GetSlice (end);
		}
	}
}