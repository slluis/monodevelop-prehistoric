using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Invocation of methods or delegates.
	/// </summary>
	public class Invocation : StatementExpression {
		private readonly ArrayList arguments;
		public ArrayList Arguments {
			get {
				return arguments;
			}
		}

		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}
        		
			
		//
		// arguments is an ArrayList, but we do not want to typecast,
		// as it might be null.
		//
		// FIXME: only allow expr to be a method invocation or a
		// delegate invocation (7.5.5)
		//
		public Invocation (Expression expr, ArrayList arguments, Location l) {
			this.expr = expr;
			this.arguments = arguments;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forInvocation(this, inp);
		}

		/*
		public override Expression DoResolve (EmitContext ec) {
			//
			// First, resolve the expression that is used to
			// trigger the invocation
			//
			if (expr is BaseAccess)
				is_base = true;

			expr = expr.Resolve (ec);
			if (expr == null)
				return null;

			if (!(expr is MethodGroupExpr)) {
				Type expr_type = expr.Type;

				if (expr_type != null){
					bool IsDelegate = TypeManager.IsDelegateType (expr_type);
					if (IsDelegate)
						return (new DelegateInvocation (
							this.expr, Arguments, loc)).Resolve (ec);
				}
			}

			if (!(expr is MethodGroupExpr)){
				report118 (loc, this.expr, "method group");
				return null;
			}

			//
			// Next, evaluate all the expressions in the argument list
			//
			if (Arguments != null){
				foreach (Argument a in Arguments){
					if (!a.Resolve (ec, loc))
						return null;
				}
			}

			method = OverloadResolve (ec, (MethodGroupExpr) this.expr, Arguments, loc);

			if (method == null){
				Error (-6, loc,
					"Could not find any applicable function for this argument list");
				return null;
			}

			if (method is MethodInfo)
				type = ((MethodInfo)method).ReturnType;

			if (type.IsPointer){
				if (!ec.InUnsafe){
					UnsafeError (loc);
					return null;
				}
			}
			
			eclass = ExprClass.Value;
			return this;
		}
*/
		

	}
}
