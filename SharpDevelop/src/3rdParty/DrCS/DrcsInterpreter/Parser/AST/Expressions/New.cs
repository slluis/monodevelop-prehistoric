using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///    Implements the new expression 
	///    and a delegate creation expression
	/// </summary>
	public class New : StatementExpression {
		private ArrayList arguments;
		public ArrayList Arguments {
			get {
				return arguments;
			}
		}

		private CType wantedType;
		public CType WantedType {
			get {
				return wantedType;
			}
		}
		
		public New (CType type, ArrayList arguments, Location l) {
			this.wantedType = type;
			this.arguments = arguments;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forNew(this, inp);
		}

		/*
		public override Expression DoResolve (EmitContext ec) {
			type = RootContext.LookupType (ec.DeclSpace, RequestedType, false, loc);
			
			if (type == null)
				return null;
			
			bool IsDelegate = TypeManager.IsDelegateType (type);
			
			if (IsDelegate)
				return (new NewDelegate (type, Arguments, loc)).Resolve (ec);

			if (type.IsInterface || type.IsAbstract){
				Report.Error (
					144, loc, "It is not possible to create instances of interfaces " +
					"or abstract classes");
				return null;
			}
			
			bool is_struct = false;
			is_struct = type.IsSubclassOf (TypeManager.value_type);
			eclass = ExprClass.Value;

			//
			// SRE returns a match for .ctor () on structs (the object constructor), 
			// so we have to manually ignore it.
			//
			if (is_struct && Arguments == null)
				return this;
			
			Expression ml;
			ml = MemberLookupFinal (ec, type, ".ctor",
				MemberTypes.Constructor,
				AllBindingFlags | BindingFlags.DeclaredOnly, loc);

			if (ml == null)
				return null;
			
			if (! (ml is MethodGroupExpr)){
				if (!is_struct){
					report118 (loc, ml, "method group");
					return null;
				}
			}

			if (ml != null) {
				if (Arguments != null){
					foreach (Argument a in Arguments){
						if (!a.Resolve (ec, loc))
							return null;
					}
				}

				method = Invocation.OverloadResolve (ec, (MethodGroupExpr) ml,
					Arguments, loc);
				
			}
			
			if (method == null && !is_struct) {
				Error (1501, loc,
					"New invocation: Can not find a constructor for " +
					"this argument list");
				return null;
			}
			return this;
		}
*/
	}

}
