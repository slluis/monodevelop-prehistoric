using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for UsingStatement.
	/// </summary>
	public class UsingStatement : Statement {

		/// <summary>
		/// expression or LocalVarDecl
		/// </summary>
		private object exprOrDecl;
		public object ExprOrDecl {
			get {
				return exprOrDecl;
			}
		}
		
		private Statement statement;
		public Statement Statement {
			get {
				return statement;
			}
		}
	

		public UsingStatement(object eord, Statement stmt, Location l) {
			exprOrDecl = eord;
			statement = stmt;
			loc = l;
		}
		
		/*
		public override bool Resolve (EmitContext ec) {
			return Statement.Resolve (ec);
		}
		*/
	
		public override object execute(IStatementVisitor v, object inp) {
			return v.forUsingStatement(this, inp);
		}
	}

}
