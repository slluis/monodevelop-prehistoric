using System;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for ReturnException.
	/// </summary>
	public class ReturnException : InterpreterException {
		private object retVal;
		public object RetVal {
			get { return retVal; }
		}

		public ReturnException() : base() {
			retVal = null;
		}

		public ReturnException(object ret) : base() {
			//
			// TODO: Add constructor logic here
			//
			retVal = ret;
		}

		
	}
}
