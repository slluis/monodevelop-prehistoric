using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Catch.
	/// </summary>
	public class Catch {
		
		private CatchArgs catchArgs;
		public CatchArgs CatchArgs {
			get {
				return catchArgs;
			}
		}

		private readonly Block block;
		public Block Block {
			get {
				return block;
			}
		}

		private readonly Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}
		

		public Catch (CatchArgs ca, Block block, Location l) {
			catchArgs = ca;
			this.block = block;
			loc = l;
		}
	}
}
