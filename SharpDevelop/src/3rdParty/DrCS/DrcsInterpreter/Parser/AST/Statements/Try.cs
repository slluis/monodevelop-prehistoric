using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Try.
	/// </summary>
	public class Try : Statement {
		private readonly Block block, finallyBlock;
		public Block Block {
			get{ 
				return block;
			}
		}

		public Block FinallyBlock {
			get {
				return finallyBlock;
			}
		}

		private readonly ArrayList specCatchList;
		public ArrayList SpecCatchList {
			get {
				return specCatchList;
			}
		}
		
		private Catch generalCatch;
		public Catch GeneralCatch {
			get {
				return generalCatch;
			}
		}

		//
		// specific, general and fini might all be null.
		//
		public Try (Block block, ArrayList sList, Catch gen, Block finBlock, Location l) {
			this.block = block;
			this.specCatchList = sList;
			this.generalCatch = gen;
			this.finallyBlock = finBlock;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			bool ok = true;
			
			if (General != null)
				if (!General.Block.Resolve (ec))
					ok = false;

			foreach (Catch c in Specific){
				if (!c.Block.Resolve (ec))
					ok = false;
			}

			if (!Block.Resolve (ec))
				ok = false;

			if (Fini != null)
				if (!Fini.Resolve (ec))
					ok = false;
			
			return ok;
		}
		*/		
		
		public override object execute(IStatementVisitor v, object inp) {
			return v.forTry(this,inp);
		}
	}

}
