using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for SwitchSection.
	/// </summary>
	public class SwitchSection {
		// An array of SwitchLabels.
		private readonly ArrayList labels;
		public ArrayList Labels {
			get {
				return labels;
			}
		}

		private readonly ArrayList stmtList;
		public ArrayList StatementList {
			get {
				return stmtList;
			}
		}
		
		private readonly Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}

		public SwitchSection (ArrayList labels, ArrayList stmtL, Location l) {
			this.labels = labels;
			this.stmtList = stmtL;
			loc = l;
		}
	}
	
}
