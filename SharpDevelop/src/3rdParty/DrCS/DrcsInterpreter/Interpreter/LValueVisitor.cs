using System;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;
using System.Reflection;
using System.Collections;


namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for LValueVisitor.
	/// </summary>
	public class LValueVisitor : InterpretationVisitor {
		protected InterpretationVisitor iv; 

		public LValueVisitor(InterpretationVisitor iv) : base() {
			name = "LValueVisitor";
			this.env = iv.Env;
			this.tm = iv.Tm;
			this.iv = iv;
		}

		protected virtual ObjectInfo primToObj(Expression e, object inp) {
			object ret = e.execute(iv, inp);
			return new ObjectInfo(ret, ret.GetType(), null);
		}

		public override object forArrayCreation(ArrayCreation a, object inp) {
			return notSupported();
		}		

		public override object forAssignment(Assignment a, object inp){
			return notSupported();
		}

		public override object forBinary(Binary b, object inp){
			return primToObj(b, inp);
		}
		
		public override object forBoolLit(BoolLit b, object inp){
			return primToObj(b, inp);
		}

		public override object forCharLit(CharLit c, object inp){
			return primToObj(c, inp);
		} 
		
		public override object forClassCast(ClassCast c, object inp){
			return notSupported();
		}

		public override object forComposedCast(ComposedCast c, object inp){
			return notSupported();
		}
		
		public override object forDecimalLit(DecimalLit d, object inp){
			return primToObj(d, inp);
		}

		public override object forDoubleLit(DoubleLit d, object inp){
			return primToObj(d, inp);
		}

		//TBI
		public override object forElementAccess(ElementAccess e, object inp){
			DB.ep("<LValue: forElementAccess>");
			//x[y] = z;
			//return object info

			object primExp = e.Expr.execute(iv, inp);
			if(primExp == null) {
				throw new InterpreterException("null reference in element access", e.Loc);
			}
			
			Type primType = primExp.GetType();
			DB.ep("primExp == " + primExp);
			DB.ep("primType == " + primType);

			if(primExp is VariableInfo) {
				DB.ep("primExp is VariableInfo");

				primType = ((VariableInfo)primExp).Type;
				primExp = ((VariableInfo)primExp).Value;
				DB.ep("primExp == " + primExp);
				DB.ep("primType == " + primType);
			}

			//if primExp is arrayType...
			if(primType.IsArray) {
				DB.ep("primType is array_type");
				Array arr = (Array)primExp;
				
				//process indexers
				ArrayList indList = e.ExprList;
				ArrayList procIndList = new ArrayList();
				foreach(Expression expr in indList) {
					DB.ep("expr == " + expr);
					DB.ep("exprType == " + expr.GetType().FullName);
					object ind = extractVariableValue(expr.execute(iv,inp));
					DB.ep("index == " + ind);
					Type indType = ind.GetType();
					DB.ep("indexType == " + indType);
					if(TypeManager.IsWholeNumber(indType)) {
						int indexer = 0;
						try { 
							indexer = Convert.ToInt32(ind); 
						}
						catch(OverflowException) {
							throw new InterpreterException("indexer too large for array", e.Loc);
						}
						procIndList.Add(indexer);
					}					
					else {
						throw new InterpreterException("invalid type for array indexer", e.Loc);
					}
				}
	
				//check that rank is correct
				if(arr.Rank != procIndList.Count) {
					throw new InterpreterException("indexer rank and array rank do not match", e.Loc);
				}
					
				//check that each indexer is within bounds
				//can skip this if let runtime take care of it
				for(int i = 0; i< procIndList.Count ; i++) {
					int index = (int)procIndList[i];
					int lower = arr.GetLowerBound(i);
					int upper = arr.GetUpperBound(i);
					if(!(index >=lower && index <=upper)) {
						throw new InterpreterException("Array index out of range",e.Loc);
					}
				}

				//convert to int[]
				//				int[] intArr = new int[procIndList.Count];
				//				for(int i =0 ; i<procIndList.Count; i++) {
				//					intArr[i] = (int)procIndList[i];
				//				}
				//object ret = arr.GetValue(intArr);
				DB.ep("</LValue: forElementAccess>");
				return new ArrayInfo(arr, arr.GetType(), null, procIndList.ToArray());
			}
				//else it's an indexer access
				//primExp should be an object instance
			else {
				DB.ep("property access");
				//use type.InvokeMember("Item", BindingFlags.GetProperty
				
				//process indexers
				ArrayList indList = e.ExprList;
				ArrayList procIndList = new ArrayList();
				foreach(Expression expr in indList) {
					object ind = extractVariableValue(expr.execute(iv,inp));
					procIndList.Add(ind);
				}
				
				
				//should be propertyInfo
				//check for setter?
				
				//must have an instance
				//want this to be BindingFlags.SetProperty
				//object retVal = primType.InvokeMember("Item", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, primExp, procIndList.ToArray() );
				MemberInfo[] mi = primType.GetMember("Item",ibFlags);
				if(mi.Length == 0 || !(mi[0] is PropertyInfo)) {
					throw new InterpreterException("invalid property access", e.Loc);
				}
				DB.ep("</LValue: forElementAccess>");
				return new ArrayInfo(primExp, primType, mi[0], procIndList.ToArray());
			}

		}

		public override object forEmptyCast(EmptyCast e, object inp){
			return notSupported();
		}
		
		public override object forFloatLit(FloatLit f, object inp){
			return primToObj(f, inp);
		}
		
		public override object forIntLit(IntLit i, object inp){
			return primToObj(i, inp);
		}

		public override object forInvocation(Invocation i, object inp){
			DB.ip("<LValue: forInvocation>");
			//x.get().blah = y;
			//memberAccess(...).memberAccess(...)

			//need to get back objectInfo
			//need method group or delegate group
			Info methods = (Info)i.Expr.execute(this, inp);
			//should be objectInfo - want both MethodInfo (for method name), and Type to call InvokeMember
			Type t = null;
			string methodName = null;
			object instance = null;
			if(methods is ObjectInfo) {
				ObjectInfo oi = (ObjectInfo)methods;
				t = oi.Type;
				methodName = oi.Info.Name;
				instance = oi.Value;
			}
			else {
				throw new InterpreterException("method could not be found", i.Loc);
			}

			DB.ip("processing argument list");
			//process argument list - keeping track of ref and out parameters
			ArrayList argList = i.Arguments;
			DB.ip("argList == " + argList);
			ArrayList procArgList = new ArrayList();
			if(argList != null) {
				foreach(Argument arg in argList) {
					//get back either prim/obj or VariableInfo
					procArgList.Add(extractVariableValue(arg.Expr.execute(iv, inp)));
				}
			}
			DB.ip("got here");
			object[] oa = procArgList.ToArray();
			object ret = t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, instance, oa);
			
			DB.ip("</LValue: forInvocation>");
			return new ObjectInfo(ret, ret.GetType(), null);
		}

		public override object forLongLit(LongLit l, object inp){
			return primToObj(l, inp);
		}

		
		public override object forMemberAccess(MemberAccess m, object inp){
			DB.mp("<Lvalue: MemberAccess>");
			//return namespaceinfo, typeinfo or ObjectInfo only
			Info leftInfo = (Info)m.Expr.execute(this, inp);
			//could be typeinfo, or namespaceinfo or objectinfo or variableinfo
			object target = leftInfo.Value;
			string ident = m.Ident;
			Type ttype = target.GetType();
			//ttype.GetMember(ident, BindingFlags.Public);

			//could return a namespaceinfo
			if(leftInfo is NamespaceInfo) {
				DB.mp("leftInfo is NamespaceInfo");
				string ns = (string)leftInfo.Value;
				string ns2 = ns + "." + ident;
				if(tm.IsValidNamespace(ns2)) {
					return new NamespaceInfo(ns2);
				}
			
				//has to be a type or we throw a MissingTypeOrNamespaceException
				return new TypeInfo(tm.LookupType(ns2));
			}

				//could return a typeinfo
			else if(leftInfo is TypeInfo) {
				DB.mp("leftInfo is TypeInfo");
				//if E is prim-type or type and member lookup in I in E has match
				Type type = (Type)leftInfo.Value;
				MemberInfo[] mia = type.GetMember(ident, sbFlags);
				if(mia == null) {
					throw new InterpreterException("unknown member access, '" + type.FullName + "' has no member '" + ident + "'", m.Loc);
				}
				MemberInfo mi = mia[0];
				//    if I is a type, then it's a type
				string maybeType = type.FullName + "." + ident;
				if(tm.IsValidType(maybeType)) {
					return new TypeInfo(tm.LookupType(maybeType));
				}

				//    if I is a method, then return a method group
				//throw exception except in lValue
				if(mi.MemberType == MemberTypes.Method) {
					return new ObjectInfo(null, type, mi);
				}

				//    if I is a static property, then return property access
				if(mi.MemberType == MemberTypes.Property) {
					PropertyInfo pi = (PropertyInfo)mi;
					object prop = pi.GetValue(null, null);
					return new ObjectInfo(null, type, pi, prop, prop.GetType()); //was (prop, prop.GetType(), pi)
				}

				//    if I is static field, if readonly....
				// catches the enum case and also the constant case
				if(mi.MemberType == MemberTypes.Field) {
					FieldInfo fi = (FieldInfo)mi;
					object field = fi.GetValue(null);
					return new ObjectInfo(null, type, fi, field, field.GetType());
				}

				//    if I is static event, then event access
				if(mi.MemberType == MemberTypes.Event) {
					EventInfo ei = (EventInfo)mi;
					new TypeInfo(ei.EventHandlerType);
					//return ei.EventHandlerType;
				}

				//    if I is a constant, value of constant
				//Atrribute Literal
				//    if I is enum member, value of enum member
				//check that ident is a field of the enum

				throw new InterpreterException("invalid member access", m.Loc);
				//    else error
			}

				//could return objectinfo as in case of Methodsinfos
			else if(leftInfo is VariableInfo || leftInfo is ObjectInfo || leftInfo is ArrayInfo) {
				DB.mp("leftInfo is VariableInfo or ObjectInfo or ArrayInfo");
				//if E is property access, indexer access, variable, or value of type T and I is in T
				Type type = leftInfo.Type;
				DB.mp("type == " + type);
				object instance = leftInfo.Value;
				//!!!!!! change???
				DB.mp("instance == " + instance);
				
				if(leftInfo is ArrayInfo) {
					DB.mp("leftInfo is ArrayInfo");
					//x[y].Foo
					//instance is array, but need to access item
					ArrayInfo ai = (ArrayInfo)leftInfo;
					//debug:
					if(DB.mdb) {
						object o = ai.Value;
						DB.mp("ai.Value == " + o);
					}
					
					if(ai.Info == null) { //got prim type array
						DB.mp("got prim type array");
						Array arr = (Array)ai.Value;
						int[] indexers = objToIntArray(ai.Indexers);
						instance = arr.GetValue(indexers);
						type = instance.GetType();
					}
					else { //got element access 
						DB.mp("got element access");
						object[] indexers = ai.Indexers;
						type = ai.Type;
						instance = ai.Value;
						instance = type.InvokeMember("Item", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, instance,indexers );
						type = instance.GetType();
					}
					 
					//Array arr = (Array)ai.Value;
					//int[] indexers = objToIntArray(ai.Indexers);
					//instance = arr.GetValue(indexers);
					//type = ai.Type;
					DB.mp("type == " + type);
					DB.mp("instance == " + instance);


				}
				else if(leftInfo is ObjectInfo) {
					DB.mp("leftInfo is ObjectInfo");
					ObjectInfo oi = (ObjectInfo)leftInfo;
					type = oi.CurrType;
					instance = oi.CurrValue;
					DB.mp("type == " + type);
					DB.mp("instance == " + instance);
				}
				
				DB.mp("getting methods: ident == " + ident);

				MemberInfo[] mia = type.GetMember(ident, ibFlags);
				
				if(mia.Length == 0 || mia == null) {
					throw new InterpreterException("unknown member access, '" + type.FullName + "' has no member '" + ident + "'", m.Loc);
				}

				//DB.mp("print out mia");
				//foreach(object obj in mia) {
				//	DB.mp("mia == " + obj);
				//}

				//DB.mp("about to call mia[0]");
				MemberInfo mi = mia[0];

				DB.mp("mi == " + mi);
				//    if E is a property or indexer access, then return the value //ok

				//    if I is a method, then return method group (objectinfo - in LValue only)
				if(mi.MemberType == MemberTypes.Method) {
					DB.mp("mi.MemberType == method");
					DB.mp("</Lvalue: MemberAccess>");
					return new ObjectInfo(instance, type, mi);
					//throw new InterpreterException("a method group cannot be a return value", m.Loc);
				}

				//    if I is instance property, then return value
				if(mi.MemberType == MemberTypes.Property) {
					DB.mp("mi.MemberType == property");
					
					PropertyInfo pi = (PropertyInfo)mi;
					//return new ObjectInfo(pi.GetValue(instance, null), type, pi);
								
					object prop = pi.GetValue(instance, null);
					Type pType = prop == null ? null : prop.GetType();
					DB.mp("</Lvalue: MemberAccess>");
					
					return new ObjectInfo(instance, type, pi, prop, pType);
				}

				//    if T is a class-type, .... result if variable
				if(mi.MemberType == MemberTypes.Field) {
					DB.mp("mi.MemberType == fiels");
					
					FieldInfo fi = (FieldInfo)mi;
					//return new ObjectInfo(fi.GetValue(instance), type, fi);
					object field = fi.GetValue(instance);
					Type fType = field == null? null : field.GetType();
					DB.mp("</Lvalue: MemberAccess>");
					return new ObjectInfo(instance, type, fi, field, fType);
				}

				//    if I is static event, then event access
				if(mi.MemberType == MemberTypes.Event) {
					DB.mp("mi.MemberType == event");
					DB.mp("</Lvalue: MemberAccess>");
					EventInfo ei = (EventInfo)mi;
					return new TypeInfo(ei.EventHandlerType);
				}

				//if T is struct type and I is instance field

				//if I is an instance event....
			}

			//else error
			throw new InterpreterException("invalid member access", m.Loc);

			//return notSupported();
			
		}

		//TBI - prob do nothing
		public override object forNew(New n, object inp){
			return primToObj(n, inp);
		}

		public override object forNullLit(NullLit n, object inp) {
			throw new InterpreterException("Error: 'null' cannot be in an lvalue");
		}

		
		public override object forSimpleName(SimpleName s, object inp){
			//check if type or namespace or variable
			//check scope
			try {
				VariableInfo vi = env.LookupVariable(s.Name);
				return vi;
			}
			catch(UndeclaredVariableException) {
				Info info = null;
				//could be in using-alias directive
				info = env.GetUsingAlias(s.Name);
				if(info != null) {
					return info;
				}
					//could be namespace
				else if(tm.IsValidNamespace(s.Name)) {
					return new NamespaceInfo(s.Name);
				}
					//could be type
				else {
					//if this fails we throw a MissingTypeOrNamespaceException
					Type t = tm.LookupType(s.Name);
					return new TypeInfo(t);
				}				
			}
		}

		public override object forStringLit(StringLit s, object inp){
			return primToObj(s, inp);
		}
		
		public override object forTypeOf(TypeOf t, object inp){
			return new TypeInfo((Type)t.execute(iv, inp));
		}
		
		public override object forUIntLit(UIntLit u, object inp){
			return primToObj(u, inp);
		}
		
		public override object forULongLit(ULongLit u, object inp){
			return primToObj(u, inp);
		}

		public override object forUnary(Unary u, object inp){
			return primToObj(u, inp);
		}

		

		#region NOT_SUPPORTED
		//not supported stuff
		public override object forAs(As a, object inp) {
			return notSupported();
		}
		public override object forBoxedCast(BoxedCast b, object inp){
			return notSupported();
		}	
		public override object forBoolConstant(BoolConstant b, object inp){
			return notSupported();
		}
		public override object forCharConstant(CharConstant c, object inp){
			return notSupported();
		}public override object forConstant(Constant c, object inp){
			 return notSupported();
		 }		
		public override object forDecimalConstant(DecimalConstant d, object inp){
			return notSupported();
		}
		public override object forDoubleConstant(DoubleConstant d, object inp){
			return notSupported();
		}
		public override object forEnumConstant(EnumConstant e, object inp){
			return notSupported();
		}
		public override object forFloatConstant(FloatConstant f, object inp){
			return notSupported();
		}
		public override object forIntConstant(IntConstant i, object inp){
			return notSupported();
		}
		public override object forLongConstant(LongConstant l, object inp){
			return notSupported();
		}
		public override object forStringConstant(StringConstant s, object inp){
			return notSupported();
		}
		public override object forCheckedExpression(CheckedExpression c, object inp){
			return notSupported();
		}
		public override object forCompoundAssignment(CompoundAssignment c, object inp){
			return notSupported();
		}
		public override object forConditional(Conditional c, object inp){
			return notSupported();
		}
		public override object forParenExpr(ParenExpr p, object inp){
			return notSupported();
		}
		public override object forIs(Is i, object inp){
			return notSupported();
		}
		public override object forUIntConstant(UIntConstant u, object inp){
			return notSupported();
		}
		public override object forULongConstant(ULongConstant u, object inp){
			return notSupported();
		}
		public override object forUncheckedExpression(UncheckedExpression u, object inp){
			return notSupported();
		}
		public override object forThis(This t, object inp){
			return notSupported();
		}
		public override object forSizeOf(SizeOf s, object inp){
			return notSupported();
		}		
		public override object forUnboxCast(UnboxCast u, object inp){
			return notSupported();
		}
		public override object forUnaryMutator(UnaryMutator u, object inp){
			return notSupported();
		}		




		#endregion
	}
}
