//  ParserException.cs
//  Copyright (C) 2001 Mike Krueger
// 
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


using System;

namespace Rice.Drcsharp.Parser
{
	
	/// <summary>
	/// This exception is thrown, when the parser founds an error 
	/// into a sourcefile.
	/// </summary>
	public class ParserException : Exception
	{
		protected Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}


		public ParserException(string message, int line, int column) : base(message)
		{
			loc = new Location(line, column);
		}
	
		public ParserException(string message, Location l) : base (message) {
			loc = l;
		}

		public ParserException(string message) : base(message) {
		}
		
		public override string ToString() {
			if(null != loc)
				return base.ToString() + " " + loc.ToString();
			else
				return base.ToString();
		}
	}
}
