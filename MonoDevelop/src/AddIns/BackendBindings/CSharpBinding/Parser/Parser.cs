// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using MonoDevelop.Services;
using SharpDevelop.Internal.Parser;
using CSharpBinding.Parser.SharpDevelopTree;
using MonoDevelop.SharpRefactory.Parser;

namespace CSharpBinding.Parser
{
	public class TParser : IParser
	{
		///<summary>IParser Interface</summary> 
		string[] lexerTags;
		public string[] LexerTags {
			set {
				lexerTags = value;
			}
		}
		
		public ICompilationUnitBase Parse(string fileName)
		{
			MonoDevelop.SharpRefactory.Parser.Parser p = new MonoDevelop.SharpRefactory.Parser.Parser();
			
			p.Parse(new Lexer(new FileReader(fileName)));
			
			CSharpVisitor visitor = new CSharpVisitor();
			visitor.Visit(p.compilationUnit, null);
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			return visitor.Cu;
		}
		
		public ICompilationUnitBase Parse(string fileName, string fileContent)
		{
			MonoDevelop.SharpRefactory.Parser.Parser p = new MonoDevelop.SharpRefactory.Parser.Parser();
			p.Parse(new Lexer(new StringReader(fileContent)));
			
			CSharpVisitor visitor = new CSharpVisitor();
			visitor.Visit(p.compilationUnit, null);
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			visitor.Cu.Tag = p.compilationUnit;
			return visitor.Cu;
		}
		
		public ResolveResult Resolve(IParserService parserService, string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			return new Resolver().Resolve(parserService, expression, caretLineNumber, caretColumn, fileName, fileContent);
		}
		
		///////// IParser Interface END
	}
}
