using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for If.
	/// </summary>
	public class If : Statement {
		private Expression expr;

		public Expression Expr {
			get {
				return expr;
			}
		}

		private Statement trueStatement;
	
		public Statement TrueStatement {
			get {
				return trueStatement;
			}
		}
	
		private Statement falseStatement;

		public Statement FalseStatement {
			get {
				return falseStatement;
			} 
		}
			 

		
		public If (Expression expr, Statement trueStatement, Location l) {
			this.expr = expr;
			this.trueStatement = trueStatement;
			loc = l;
		}

		public If (Expression expr, Statement trueStatement, Statement falseStatement, Location l) {
			this.expr = expr;
			this.trueStatement = trueStatement;
			this.falseStatement = falseStatement;
			loc = l;
		}
/*
		public override bool Resolve (EmitContext ec) {
			expr = ResolveBoolean (ec, expr, loc);
			if (expr == null){
				return false;
			}
			
			if (TrueStatement.Resolve (ec)){
				if (FalseStatement != null){
					if (FalseStatement.Resolve (ec))
						return true;
					
					return false;
				}
				return true;
			}
			return false;
		}
		
*/		
		public override object execute(IStatementVisitor v, object inp) {
			return v.forIf(this, inp);
		}

	}

}
