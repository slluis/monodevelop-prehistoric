using System;
using Rice.Drcsharp.Parser.AST.Visitors;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   This kind of cast is used to encapsulate the child
	///   whose type is child.Type into an expression that is
	///   reported to return "return_type".  This is used to encapsulate
	///   expressions which have compatible types, but need to be dealt
	///   at higher levels with.
	///
	///   For example, a "byte" expression could be encapsulated in one
	///   of these as an "unsigned int".  The type for the expression
	///   would be "unsigned int".
	///
	/// </summary>
	public class EmptyCast : Expression {
		protected Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

//		protected Type type;
//		public Type Type {
//			get {
//				return type;
//			}
//		}

		public EmptyCast (Expression expr, Type t) {
			type = t;
			this.expr = expr;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forEmptyCast(this, inp);
		}

	}


	/// <summary>
	///   This kind of cast is used to encapsulate Value Types in objects.
	///
	///   The effect of it is to box the value type emitted by the previous
	///   operation.
	/// </summary>
	public class BoxedCast : EmptyCast {

		public BoxedCast (Expression expr) : base (expr, TypeManager.object_type) {
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forBoxedCast(this, inp);
		}
	}

	/// <summary>
	///   This kind of cast is used to encapsulate a child and cast it
	///   to the class requested
	/// </summary>
	public class ClassCast : EmptyCast {
		private Expression expectedType;
		public Expression ExpectedType {
			get {
				return expectedType;
			}
		}

		public ClassCast (Expression expr, Expression expected_type, Location l) : base (expr, TypeManager.object_type){
			this.expectedType = expected_type;
			loc = l;
		}

		public ClassCast(Expression expr, Type t) : base(expr, t) {}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forClassCast(this, inp);
		}
	}

	/// <summary>
	/// Summary description for ComposedCast.
	/// </summary>
	public class ComposedCast : Expression {
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		private string rank;
		public string Rank {
			get {
				return rank;
			}
		}

		public ComposedCast(Expression e, string r, Location l) {
			expr = e;
			rank = r;
			loc = l;
		}
		
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forComposedCast(this, inp);
		}
	}

	/// <summary>
	/// Summary description for UnboxCast.
	/// </summary>
	public class UnboxCast : EmptyCast {
		public UnboxCast (Expression expr, Type return_type) : base (expr, return_type) {
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forUnboxCast(this, inp);
		}
	}

	/*
	/// <summary>
	/// Summary description for ConvCast.
	/// </summary>
	public class ConvCast : EmptyCast {
		public ConvCast(Expression exp, Type t) : base(exp, t) {}

		public override object execute(IExpressionVisitor v, object inp ) {
			return v.forConvCast(this, inp);
		}

	}

	*/
}
