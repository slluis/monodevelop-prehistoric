using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Do.
	/// </summary>
	public class Do : Statement {
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}


		private readonly Statement  embeddedStatement;
		public Statement EmbeddedStatement {
			get {
				return embeddedStatement;
			}
		}

		
		public Do (Statement statement, Expression boolExpr, Location l) {
			expr = boolExpr;
			embeddedStatement = statement;
			loc = l;
		}
		 
		public override object execute(IStatementVisitor v, object inp) {
			return v.forDo(this, inp);
		}
	}


}
