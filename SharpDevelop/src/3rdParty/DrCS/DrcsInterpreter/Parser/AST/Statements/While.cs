using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for While.
	/// </summary>
	public class While : Statement {
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		private readonly Statement statement;
		public Statement Statement {
			get {
				return statement;
			}
		}
		
		public While (Expression boolExpr, Statement statement, Location l) {
			this.expr = boolExpr;
			this.statement = statement;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			expr = ResolveBoolean (ec, expr, loc);
			if (expr == null)
				return false;
			
			return Statement.Resolve (ec);
		}
		*/

		public override object execute(IStatementVisitor v, object inp) {
			return v.forWhile(this, inp);
		}
	}

}
