// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.TextEditor.Document
{
	/// <summary>
	/// This iterator iterates on a text buffer strategy.
	/// </summary>
	public interface ITextIterator
	{
		/// <value>
		/// Gets the current char this is the same as 
		/// GetCharRelative(0)
		/// </value>
		/// <exception cref="System.InvalidOperationException">
		/// If this method is called before the first MoveAhead or after 
		/// MoveAhead or after MoveAhead returns false.
		/// </exception>
		char Current {
			get;
		}
		
		/// <value>
		/// The current position=offset of the text iterator cursor
		/// </value>
		int Position {
			get;
			set;
		}
		
		/// <remarks>
		/// Gets a char relative to the current position (negative values
		/// will work too).
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">
		/// If this method is called before the first MoveAhead or after 
		/// MoveAhead or after MoveAhead returns false.
		/// Returns Char.MinValue if the relative position is outside the
		/// text limits.
		/// </exception>
		char GetCharRelative(int offset);
		
		/// <remarks>
		/// Moves the iterator position numChars
		/// </remarks>
		bool MoveAhead(int numChars);
		
		string ReadToEnd ();
		
		/// <remarks>
		/// Rests the iterator
		/// </remarks>
		void Reset();
		
		void Replace (int length, string pattern);
		
		void Close ();
	}
}
