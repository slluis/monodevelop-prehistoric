//
// report.cs: report errors and warnings.
//
// Author: Miguel de Icaza (miguel@ximian.com)
//
// (C) 2001 Ximian, Inc. (http://www.ximian.com)
//

//
// FIXME: currently our class library does not support custom number format strings
//
using System;
using System.Collections;
using System.Diagnostics;

namespace Rice.Drcsharp.Parser {

	/// <summary>
	///   This class is used to report errors and warnings t te user.
	/// </summary>
	public class Report {

		private static bool debug = false;
		/// <summary>  
		///   Errors encountered so far
		/// </summary>
		static public int Errors;

		/// <summary>  
		///   Whether errors should be throw an exception
		/// </summary>
		static public bool Fatal = true;

		/// <summary>  
		///   Whether to dump a stack trace on errors. 
		/// </summary>
		static public bool Stacktrace = true;
		
		
		static public void RealError (string msg, Location l)
		{
			Errors++;
			if(debug) {
				Console.Error.WriteLine (msg);
			}
			if (debug && Stacktrace)
				Console.WriteLine (new StackTrace ().ToString ());
			if (Fatal)
				throw new ParserException (msg, l);
		}

		static public void Error (string text, Location l)
		{
			//string msg = String.Format ("{0}({1}) error: {2}", l.Line, l.Column, text);			
			RealError (text, l);
		}


	}
}


