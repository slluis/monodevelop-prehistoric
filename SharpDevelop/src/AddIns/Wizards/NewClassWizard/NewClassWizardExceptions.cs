// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace NewClassWizard
{
	/// <summary>
	/// Application Excpetion thrown when an unsupported 
	/// programming language is encountered
	/// </summary>
	public class UnknownLanguageException : System.ApplicationException 
	{
		public UnknownLanguageException() : base ( "Unknown programming language" )
		{
		}

		public UnknownLanguageException( string message ) : base( message )
		{
		}

		public UnknownLanguageException( string message, Exception innerException ) : base( message, innerException )
		{
		}
	}

}
