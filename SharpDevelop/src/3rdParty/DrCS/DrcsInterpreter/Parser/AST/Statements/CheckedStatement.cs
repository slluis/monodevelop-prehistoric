using System;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Checked. 
	/// NOTE: Everything is checked.
	/// </summary>
	public class CheckedStatement : Statement {
		private readonly Block block;
		public Block Block {
			get {
				return block;
			}
		}
		
		public CheckedStatement (Block b, Location l) {
			block = b;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			return Block.Resolve (ec);
		}
		*/


		public override object execute(IStatementVisitor v, object inp) {
			return v.forCheckedStatement(this, inp);
		}
	}

}
