using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Unsafe.
	/// NOTE: NOT IMPLEMENTED
	/// </summary>
	public class Unsafe : Statement {
		public readonly Block Block;

		public Unsafe (Block b, Location l) {
			Block = b;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			return Block.Resolve (ec);
		}
		*/
	
		public override object execute(IStatementVisitor v, object inp) {
			return v.forUnsafe(this, inp);
		}

	}

}
