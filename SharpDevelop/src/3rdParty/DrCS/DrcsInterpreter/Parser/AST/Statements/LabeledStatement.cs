using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for LabeledStatement.
	/// </summary>
	public class LabeledStatement : Statement {
		private string label_name;
		public String Label_name {
			get {
				return label_name;
			}
		}

		/*
		private bool defined;
		public bool Defined {
			get {
				return degined;
			}
		}
		

		private Label label;
		public Label Label {
			get {
				return label;
			}
		}
		*/

		private Statement statement;
		public Statement Statement {
			get {
				return statement;
			}
		}

		public LabeledStatement (string label_name, Statement stmt, Location l) {
			this.label_name = label_name;
			this.statement = stmt;
			loc = l;
		}

		/*
		public Label LabelTarget (EmitContext ec) {
			if (defined)
				return label;
			label = ec.ig.DefineLabel ();
			defined = true;

			return label;
		}
		*/

		public override object execute(IStatementVisitor v, object inp) {
			return v.forLabeledStatement(this, inp);
		}
	}
	
}
