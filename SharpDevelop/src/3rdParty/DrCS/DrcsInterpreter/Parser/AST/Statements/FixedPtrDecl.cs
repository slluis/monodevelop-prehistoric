using System;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for FixedPtrDecl.
	/// </summary>
	public class FixedPtrDecl
	{
		public readonly string Ident;
		public readonly Expression Expr;
		public readonly Location Loc;

		public FixedPtrDecl(string id, Expression exp, Location l)
		{
			Ident = id;
			Expr = exp;
			Loc = l;
			//
		}
	}
}
