using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for ContinueException.
	/// </summary>
	public class ContinueException : InterpreterException {
		public ContinueException() : base() {
			//
			// TODO: Add constructor logic here
			//
		}

		public ContinueException(Location l) : base(l) {
		}

		public override string ToString() {
			return "Unexpected continue statement";//base.ToString() + "
		}
	}
}
