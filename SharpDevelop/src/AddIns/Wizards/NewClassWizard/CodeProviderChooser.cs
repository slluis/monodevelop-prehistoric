// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
// using Microsoft.JScript;
using System.CodeDom.Compiler;


namespace NewClassWizard
{
	public enum Language {
		CSharp,
		VisualBasic,
		JScript
	}
	/// <summary>
	/// This class encapsulates the different supported programming languages
	/// no other class should need to concern itself with the language except
	/// by using this class
	/// </summary>
	internal class CodeProviderChooser
	{

		public static ICodeGenerator chooseCodeGenerator( Language language )
		{
            
			switch (language)
			{
				case Language.CSharp :
					return new CSharpCodeProvider().CreateGenerator();

				case Language.VisualBasic :
					return new VBCodeProvider().CreateGenerator();

//				case Language.JScript :
//					return new JScriptCodeProvider().CreateGenerator();
				
				default :
					throw new UnknownLanguageException();

			}

		}

		public static bool IsValidIdentifier( Language language, string identifier ) {
			return chooseCodeGenerator( language ).IsValidIdentifier( identifier );
		}
		
		public static string chooseDefaultFileExt( Language language )
		{
			switch (language)
			{
				case Language.CSharp :
					return ".cs";

				case Language.VisualBasic :
					return ".vb";

				case Language.JScript :
					return ".js";				

				default :
					throw new UnknownLanguageException();
			}
		}

		public static string LanguageName( Language language )
		{
			switch (language)
			{
				case Language.CSharp :
					return "C#";

				case Language.VisualBasic :
					return "Visual Basic";

				case Language.JScript :
					return "JScript";				

				default :
					throw new UnknownLanguageException();
			}
		}
		

		public static string StringFromLanguage( Language language )
		{
			switch (language)
			{
				case Language.CSharp :
					return "C#";

				case Language.VisualBasic :
					return "VBNET";

				case Language.JScript :
					return "JScript";	
				
				default :
					throw new UnknownLanguageException();
			}
		}		
		
		public static Language LanguageFromString( string language )
		{
			switch (language)
			{
				case "C#" :
					return Language.CSharp;
				
				case "VBNET" :
					return Language.VisualBasic;
				
				case "JScript" :
					return Language.JScript;
				
				default :
					throw new UnknownLanguageException( "The language " + language + " is not supported by this wizard!" );
			}
		}		
	}
}
