using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for TypeErrorException.
	/// </summary>
	public class TypeErrorException : InterpreterException {

		public TypeErrorException() : base() {}

		public TypeErrorException(string s) : base(s) {
		 }

		public TypeErrorException(string s, Location l) : base(s, l) {}

	}
}
