using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Unchecked.
	/// </summary>
	public class UncheckedStatement : Statement {
		private readonly Block block;
		public Block Block {
			get {
				return block;
			}
		}
		
		public UncheckedStatement (Block b, Location l) {
			block = b;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			return Block.Resolve (ec);
		}
		*/
		
		public override object execute(IStatementVisitor v, object inp) {
			return v.forUncheckedStatement(this, inp);
		}

	}

}
