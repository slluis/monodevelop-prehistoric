using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

using expr = Rice.Drcsharp.Parser.AST.Expressions;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Foreach.
	/// </summary>
	public class Foreach : Statement {
		private CType wantedType;
		public CType WantedType {
			get {
				return wantedType;
			}
		}

		private string localVariable;
		public string LocalVariable {
			get {
				return localVariable;
			}
		}
		
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
		
		public Foreach (CType type, string var, Expression expr, Statement stmt, Location l) {
			this.wantedType = type;
			this.localVariable = var;
			this.expr = expr;
			statement = stmt;
			loc = l;
		}
		
		public override object execute(IStatementVisitor v, object inp) {
			return v.forForeach(this, inp);
		}
	}
}
