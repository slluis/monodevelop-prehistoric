using System;

namespace Rice.Drcsharp.Parser
{
	/// <summary>
	/// Summary description for EOFException.
	/// </summary>
	public class EOFException : ParserException {

		public EOFException(string message, int line, int column) : base(message) {
			loc = new Location(line, column);
		}
	
		public EOFException(string message, Location l) : base (message) {
			loc = l;
		}

		public EOFException(string message) : base(message) {
		}
	}
}
