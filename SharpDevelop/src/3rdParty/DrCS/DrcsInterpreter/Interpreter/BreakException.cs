using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for BreakException.
	/// </summary>
	public class BreakException : InterpreterException {
		public BreakException() : base() {
		}

		public BreakException(Location l) : base(l) {
		}
 
		public override string ToString() {
			return "Unexpected break statement";//base.ToString() + 
		}
	}
}