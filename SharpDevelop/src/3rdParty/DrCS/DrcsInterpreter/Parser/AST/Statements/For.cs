using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for For.
	/// </summary>
	public class For : Statement {

		private readonly object declOrInit;

		/// <summary>
		/// this is either a LocalVarDecl or an ArrayList containing statement-expressions
		/// </summary>
		public object DeclOrInit {
			get {
				return declOrInit;
			}
		}

		
		private readonly Expression test;
		public Expression Test {
			get {
				return test;
			}
		}
		private readonly ArrayList forIterator;
		public ArrayList ForIterator {
			get {
				return forIterator;
			}
		}

		private readonly Statement statement;
		public Statement Statement {
			get {
				return statement;
			}
		}
		
		public For(object declOrInit, Expression test, ArrayList forIts, Statement stmt, Location l) {
			this.declOrInit = declOrInit;
			this.test = test;
			this.forIterator = forIts;
			this.statement = stmt;
			loc = l;
		}
		
		public override object execute(IStatementVisitor v, object inp) {
			return v.forFor(this, inp);
		}
	}
	
}
