using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for InterpreterException.
	/// </summary>
	public class InterpreterException : Exception {
		Location loc;

		public InterpreterException() : base() {
		}

		public InterpreterException(Location l) : base() {
			loc = l;
		}

		public InterpreterException(string s) : base (s) {
		}

		public InterpreterException(string s, Location l) : base(s) {
			loc = l;
		}

		public override string ToString() {
			return loc + " " + base.ToString();
		}
	}
}
