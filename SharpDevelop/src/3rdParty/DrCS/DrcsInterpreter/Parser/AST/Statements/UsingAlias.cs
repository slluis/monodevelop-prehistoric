using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for UsingAlias.
	/// </summary>
	public class UsingAlias : Statement
	{
		private string alias;
		public string Alias {
			get{
				return alias;
			}
		}

		private string defn;
		public string Defn {
			get {
				return defn;
			}
		}

		public UsingAlias(string alias, string defn, Location l)
		{
			this.alias = alias;
			this.defn = defn;
			loc = l;
		}

		public override object execute(IStatementVisitor v, object inp) {
			return v.forUsingAlias(this, inp);
		}

	}
}
