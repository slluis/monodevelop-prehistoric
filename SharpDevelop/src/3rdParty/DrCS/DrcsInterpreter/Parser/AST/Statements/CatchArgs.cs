using System;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for CatchArg.
	/// </summary>
	public class CatchArgs
	{
		private readonly CType wantedType;
		public CType WantedType {
			get {
				return wantedType;
			}
		}

		private string ident;
		public string Ident {
			get {
				return ident;
			}
		}

		private Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}

		public CatchArgs(CType t, string id, Location l)
		{
			wantedType = t;
			ident = id;
			loc = l;
		}
	}
}
