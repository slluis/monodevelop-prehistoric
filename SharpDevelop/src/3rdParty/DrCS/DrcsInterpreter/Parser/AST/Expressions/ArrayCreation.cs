using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions {
	/// <summary>
	///   Represents an array creation expression.
	/// </summary>
	///
	/// <remarks>
	///   There are two possible scenarios here: one is an array creation
	///   expression that specifies the dimensions and optionally the
	///   initialization data and the other which does not need dimensions
	///   specified but where initialization data is mandatory.
	/// </remarks>
	public class ArrayCreation : Expression {
		private CType requestedType;
		public CType RequestedType {
			get {
				return requestedType;
			}
		}

		private ArrayList exprs;
		public ArrayList Exprs {
			get {
				return exprs;
			}
		}

		private string rank;
		public string Rank {
			get {
				return rank;
			}
		}
		
		ArrayList initializers;
		public ArrayList Initializers {
			get {
				return initializers;
			}
		}

		public ArrayCreation (CType requested_type, ArrayList exprs, string rank, ArrayList initializers, Location l) {
			requestedType = requested_type;
			this.exprs = exprs;
			this.rank          = rank;
			this.initializers  = initializers;
			loc = l;
		}

		public ArrayCreation (CType requested_type, string rank, ArrayList initializers, Location l) {
			requestedType = requested_type;
			this.rank = rank;
			this.initializers = initializers;
			loc = l;

			/*
			Rank = rank.Substring (0, rank.LastIndexOf ("["));

			string tmp = rank.Substring (rank.LastIndexOf ("["));

			dimensions = tmp.Length - 1;
			*/
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forArrayCreation(this, inp);
		}

	}
	
}
