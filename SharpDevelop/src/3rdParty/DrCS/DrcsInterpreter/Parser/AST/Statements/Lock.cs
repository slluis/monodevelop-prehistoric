using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Lock
	/// </summary>
	/// <remarks>This is not supported.</remarks>
	public class Lock : Statement {
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		private Statement statement;
		public Statement Statement {
			get {
				return statement;
			}
		}

			
		public Lock (Expression expr, Statement stmt, Location l) {
			this.expr = expr;
			this.statement = stmt;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			expr = expr.Resolve (ec);
			return Statement.Resolve (ec) && expr != null;
		}
		*/
		
		public override object execute(IStatementVisitor v, object inp) {
			return v.forLock(this, inp);
		}
	}

}
