// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;

namespace ICSharpCode.SharpRefactory.Parser.VB
{
	public class Keywords
	{
		static readonly string[] keywordList = {
			"ADDHANDLER",
			"ADDRESSOF",
			"ALIAS",
			"AND",
			"ANDALSO",
			"ANSI",
			"AS",
			"ASSEMBLY",
			"AUTO",
			"BINARY",
			"BOOLEAN",
			"BYREF",
			"BYTE",
			"BYVAL",
			"CALL",
			"CASE",
			"CATCH",
			"CBOOL",
			"CBYTE",
			"CCHAR",
			"CDATE",
			"CDBL",
			"CDEC",
			"CHAR",
			"CINT",
			"CLASS",
			"CLNG",
			"COBJ",
			"COMPARE",
			"CONST",
			"CSHORT",
			"CSNG",
			"CSTR",
			"CTYPE",
			"DATE",
			"DECIMAL",
			"DECLARE",
			"DEFAULT",
			"DELEGATE",
			"DIM",
			"DIRECTCAST",
			"DO",
			"DOUBLE",
			"EACH",
			"ELSE",
			"ELSEIF",
			"END",
			"ENDIF",
			"ENUM",
			"ERASE",
			"ERROR",
			"EVENT",
			"EXIT",
			"EXPLICIT",
			"FALSE",
			"FINALLY",
			"FOR",
			"FRIEND",
			"FUNCTION",
			"GET",
			"GETTYPE",
			"GOSUB",
			"GOTO",
			"HANDLES",
			"IF",
			"IMPLEMENTS",
			"IMPORTS",
			"IN",
			"INHERITS",
			"INTEGER",
			"INTERFACE",
			"IS",
			"LET",
			"LIB",
			"LIKE",
			"LONG",
			"LOOP",
			"ME",
			"MOD",
			"MODULE",
			"MUSTINHERIT",
			"MUSTOVERRIDE",
			"MYBASE",
			"MYCLASS",
			"NAMESPACE",
			"NEW",
			"NEXT",
			"NOT",
			"NOTHING",
			"NOTINHERITABLE",
			"NOTOVERRIDABLE",
			"OBJECT",
			"OFF",
			"ON",
			"OPTION",
			"OPTIONAL",
			"OR",
			"ORELSE",
			"OVERLOADS",
			"OVERRIDABLE",
			"OVERRIDE",
			"OVERRIDES",
			"PARAMARRAY",
			"PRESERVE",
			"PRIVATE",
			"PROPERTY",
			"PROTECTED",
			"PUBLIC",
			"RAISEEVENT",
			"READONLY",
			"REDIM",
			"REMOVEHANDLER",
			"RESUME",
			"RETURN",
			"SELECT",
			"SET",
			"SHADOWS",
			"SHARED",
			"SHORT",
			"SINGLE",
			"STATIC",
			"STEP",
			"STOP",
			"STRICT",
			"STRING",
			"STRUCTURE",
			"SUB",
			"SYNCLOCK",
			"TEXT",
			"THEN",
			"THROW",
			"TO",
			"TRUE",
			"TRY",
			"TYPEOF",
			"UNICODE",
			"UNTIL",
			"VARIANT",
			"WEND",
			"WHEN",
			"WHILE",
			"WITH",
			"WITHEVENTS",
			"WRITEONLY",
			"XOR",
		};
		
		static Hashtable keywords = new Hashtable();
		
		static Keywords()
		{
			for (int i = 0; i < keywordList.Length; ++i) {
				keywords.Add(keywordList[i], i + Tokens.AddHandler);
			}
		}
		
		public static bool IsKeyword(string identifier)
		{
			return keywords[identifier.ToUpper()] != null;
		}
		
		public static int GetToken(string keyword)
		{
			return (int)keywords[keyword.ToUpper()];
		}
	}
}
