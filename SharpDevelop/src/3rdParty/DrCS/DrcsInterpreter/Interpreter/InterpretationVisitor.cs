using System;
using System.Collections;
using System.Reflection;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for InterpretationVisitor.
	/// </summary>
	public class InterpretationVisitor : AASTVisitor {
		protected readonly BindingFlags ibFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		protected readonly BindingFlags sbFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		
		protected TypeManager tm;
		public TypeManager Tm {
			get { return tm; }
		}

		protected Env env;
		public Env Env {
			get { return env; }
		}

		public InterpretationVisitor(TypeManager tm) {
			name = "InterpretationVisitor";
			this.tm = tm;
			//imode = new InterpretationMode();
			this.env = new Env(tm);
		}

		//		public InterpretationVisitor(TypeManager tm, Env e) {
		//			name = "InterpretationVisitor";
		//			this.tm = tm;
		//			this.env = e;
		//		}

		public InterpretationVisitor() {
			name = "InterpretationVisitor";
		}

		public void Reset(TypeManager tm) {
			this.tm = tm;
			this.env = new Env(tm);
		}

		protected Type processCType(CType t) {
			string ctName = t.Name;
			int isArray = ctName.IndexOf('[');
			
			if(isArray != -1) {
				DB.arp("<processCType> is array");
				string rank = ctName.Substring(isArray); //get [][]
				ctName = ctName.Substring(0,isArray); //get int
				Type type = tm.LookupType(ctName); //find System.Int32
				DB.arp("Got type=" + type);
				int[] rs = processRankSpecifiers(rank); //find [1,1]
				for(int i = rs.Length-1; i>=0 ; i--) {
					int dim = rs[i];
					int[] dimL = new int[dim];
					for(int j = 0; j< dimL.Length; j++) {
						dimL[j] = 1;
					}
					Array arr = Array.CreateInstance(type, dimL);
					type = arr.GetType();
				}
				DB.arp("<processCType> type == " + type.FullName);
				return type;
			}			
			else {
				return tm.LookupType(ctName);
			}
		}
		
		protected static int[] processRankSpecifiers(string r) {
			int[] rs;
			int dim = 0;
			int bracket = r.IndexOf('[', 0);
			while(bracket != -1) {
				++dim;
				bracket = r.IndexOf('[', bracket+1);
			}
			rs = new int[dim];
			bracket = 0;
			int rbracket = r.IndexOf(']',0);
			for(int i=0; i< dim; i++) {
				rs[i] = rbracket - bracket;
				if(i+1<dim) {
					bracket = rbracket + 1;
					rbracket = r.IndexOf(']', bracket);
				}
			}
			return rs;
		}

		/****
		 * 
		 *  BEGIN CODE HERE
		 * 
		 ****/
		
		//TBI
		public override object forArrayCreation(ArrayCreation a, object inp) {
			DB.arp("<forArrayCreation>");
			Type t = processCType(a.RequestedType);
			DB.arp("type == " + t.FullName);
			//type 1
			//if(a.Exprs != null) {
			if(!t.IsArray) { // is new non-array-type [ expression-list ] rank-specifiers-opt array-initlizer-opt
				DB.arp("type 1 array");
				ArrayList expList = new ArrayList();
				foreach(Expression exp in a.Exprs) {
					object expr = extractVariableValue(exp.execute(this, inp));
					DB.arp("expression == " + expr);
					Type exprType = expr.GetType();
					DB.arp("expression Type == " + exprType);
					DB.arp("TypeManager.int32_type == " + TypeManager.int32_type);
					if(!(exprType == TypeManager.int32_type ||
						exprType == TypeManager.uint32_type ||
						exprType == TypeManager.int64_type ||
						exprType == TypeManager.uint64_type)) {
						throw new TypeErrorException("expressions in array creation expressions must be of type int, uint, long, or ulong");
					}
					expList.Add(expr);	
				}
				//expList now has the dimensions i.e. [2,2]
				
				if (a.Rank != "") {
					DB.arp("rank != null");
					//have int[10][].....
					//need to get type of int[]
					//create new CType and process???
					string arTypeStr = t.FullName + a.Rank;
					t = processCType(new CType(arTypeStr));
					DB.arp("t now  == " + t);
				}
				
				//convert expList to int[]
				//call Array.CreateInstnace()
				int[] ia = new int[expList.Count];
				try {
					for(int i=0; i<expList.Count; i++) {
						int dim = Convert.ToInt32(expList[i]);
						if(dim < 0)
							throw new IndexOutOfRangeException("cannot have negative array dimension");
						ia[i] = dim;
					}
				}
				catch {
					throw new InterpreterException("array dimension bounds must convert to type int");
				}
			
				Array arr = Array.CreateInstance(t, ia);
				if(a.Initializers == null) {
					DB.arp("</forArrayCreation>");
					return arr;
				}
				//else {
				//fill up the array
				//object[] oarr = processArrayInit(a.Initializers, inp);
				//for(int i = 0; i< oarr.Length; i++) {
						

				//	}

				//expList has the rank and dimensions of this array, but need to check rank-specifiers
				
			}
				//type 2
			else { //is  new array-type array-initializer
				throw new InterpreterException("TBI");
			}
			return null;
		}

		private void fillArray(Array arr, object[] oarr) {

		}

		//		public override object forAs(As a, object inp) {
		//			return null;
		//		}
		
		//TBI
		public override object forAssignment(Assignment a, object inp) {
			DB.asp("<forAssignment>");
			Expression lVal = a.LExpr;
			
			object lValObj = lVal.execute(new LValueVisitor(this), inp);
			Info lValRes = null;

			//must be VariableInfo or Object Info
			if(lValObj is ObjectInfo || lValObj is VariableInfo || lValObj is TypeInfo || lValObj is ArrayInfo) {
				lValRes = (Info)lValObj;
			}
			else {
				throw new InterpreterException("invalid lvalue in assignment", a.Loc);
			}

			DB.asp("lValRes type == " + lValRes.GetType().FullName);

			//be careful, this can be null!!!
			object rValRes = extractVariableValue(a.RExpr.execute(this, inp));
			DB.asp("rValRes == " + rValRes);
			DB.asp("rValRes type == " + (rValRes == null ? "null" : rValRes.GetType().FullName));

			//lvalue must be PropertyInfo, FieldInfo or VariableInfo
			if(lValRes is VariableInfo) {
				VariableInfo v = (VariableInfo)lValRes;
				//note, we are not doing any kind of type checking here!!!
				//make assignment in env
				env.AssignVariable(v.Name, rValRes);
				return rValRes;
			}
			else if(lValRes is ArrayInfo) { //must catch BEFORE ObjectInfo case
				DB.asp("in ArrayInfo case");
				//x[y].Foo
				//instance is array, but need to access item
				ArrayInfo ai = (ArrayInfo)lValRes;
				if(ai.Info == null) { //got prim type array
					DB.asp("got primitive type array");
					Array arr = (Array)ai.Value;
					int[] indexers = objToIntArray(ai.Indexers);
					arr.SetValue(rValRes, indexers);
					return rValRes;
				}
				else { //got element access 
					DB.asp("got element access");
					object[] indexers = new object[ai.Indexers.Length + 1];
					Array.Copy(ai.Indexers, indexers, ai.Indexers.Length);
					indexers[indexers.Length-1] = rValRes;
					Type type = ai.Type;
					object instance = ai.Value;
					instance = type.InvokeMember("Item", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, instance, indexers );
					return rValRes;
				}

			}
			else if (lValRes is ObjectInfo) {
				DB.asp("lValRes is ObjectInfo");
				ObjectInfo oi = (ObjectInfo)lValRes;
				object target = oi.Value;
				MemberInfo mi = oi.Info;
				DB.asp("target == " + target);
				DB.asp("mi == " + mi);

				//Type t = mi.ReflectedType;
				if(mi is PropertyInfo) {
					PropertyInfo pi = (PropertyInfo)mi;
					DB.asp("pi type == " + pi.PropertyType);
					DB.asp("target == " + target);
					DB.asp("rValRes == " + rValRes);
					DB.asp("rValRes type == " + (rValRes == null ? "null" : rValRes.GetType().FullName));
					//object obj = (object)rValRes;
					//DB.asp("pi. == " +
					pi.SetValue(target,rValRes, null);
				}
				else if(mi is FieldInfo) {
					FieldInfo fi = (FieldInfo)mi;
					DB.asp("fi type == " + fi.FieldType);
					fi.SetValue(target,rValRes);
				}
				else
					throw new InterpreterException("invalid lvalue in assignement", a.Loc);
				return rValRes;
			}
			//else if (lValRes is TypeInfo) { //should not get here Type.blah = foo
			//static type accessor/assignment

			//}

			throw new InterpreterException("invalid lvalue in assignment.", a.Loc);
		}

		//		public override object forBaseAccess(BaseAccess b, object inp) {
		//			return null;
		//		}
		//		public override object forBaseIndexerAccess(BaseIndexerAccess b, object inp) {
		//			return null;
		//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="b"></param>
		/// <param name="inp"></param>
		/// <returns></returns>
		/// <remarks>We only allow primitives and strings, no user defined operator overloading</remarks>
		public override object forBinary(Binary b, object inp) {
			DB.bp("<forBinary>");
			object left = null;
			object right = null;
			VariableInfo leftInfo = null;
			VariableInfo rightInfo = null;
			Type lType = null;
			Type rType = null;
			Binary.BiOperator bo = b.Oper;

			//if && or ||, want to transform so don't execute left or right
			if(!(b.Oper == Binary.BiOperator.LogicalAnd || b.Oper == Binary.BiOperator.LogicalOr)) {
				
				//ok, not tranform, so left's check left
				object l = b.Left.execute(this, inp);
				if(l is VariableInfo)
					leftInfo = (VariableInfo)l;
				left = extractVariableValue(l);
				if(leftInfo != null) {
					lType = leftInfo.Type;
				}
				else if(left != null) {
					lType = left.GetType();
				}
				DB.bp("left == " + left);
				DB.bp("lType == " + lType);
				
				//left is null or string
				object r= b.Right.execute(this, inp);
				if(r is VariableInfo)
					rightInfo = (VariableInfo)r;
				right = extractVariableValue(r);
				if(rightInfo != null) {
					rType = rightInfo.Type;
				}
				else if(right != null) {
					rType = right.GetType();
				}
				DB.bp("right == " + right);
				DB.bp("rType == " + rType);

				//if left is string then we can only have BiOp or + == or !=
				if(lType == TypeManager.string_type || rType == TypeManager.string_type ||
					(left == null && right == null)) {
					DB.bp("one is a string or both null");
					
					if(bo == Binary.BiOperator.Addition) {
						DB.bp("going to concatenate");
						if(left == null && right != null) {
							return right.ToString();
						}
						else if(right == null && left != null) {
							return left.ToString();
						}
						else if(left == null && right == null) {
							return "";
						}
						else {
							return left.ToString() + right.ToString();
						}
						//return b.execute(new StringConcatVisitor(this, left), inp);
					}
				}
				if(left == null || right == null || !lType.IsPrimitive || !rType.IsPrimitive) {
					if(bo == Binary.BiOperator.Addition || bo == Binary.BiOperator.Subtraction) { //can be delegate addition
						if(!(lType == TypeManager.delegate_type || rType == TypeManager.delegate_type)) {
							throw new UnsupportedException("User defined overloaded binary operations are not supported");
						}
						if(lType == TypeManager.delegate_type && right == null) {
							return left;
						}
						if(rType == TypeManager.delegate_type && left == null) {
							return right;
						}
						if(lType == TypeManager.delegate_type && rType == TypeManager.delegate_type) {
							//multicast delegates
							Delegate mdL = (Delegate)left;
							Delegate mdR = (Delegate)right;
							if(bo == Binary.BiOperator.Addition) {
								return Delegate.Combine(mdL, mdR);
							}
							else {
								return Delegate.Remove(mdL,mdR);
							}
						}
					}
					if(bo == Binary.BiOperator.Equality) { //==
						if((left == null && right != null) || (right == null && left != null)) {
							DB.bp(left + " == " + right);
							return false;
						}
						else if (left == null && right == null) {
							DB.bp(left + " == " + right);
							return true;
						}
							//insert code here for delegates and enum
							//actually this case handles them all
						else if	(!lType.IsPrimitive || !rType.IsPrimitive) {
							DB.bp(left + " == " + right);
							//return left.Equals(right);
							return left == right;
						}
						
					}
					else if(bo == Binary.BiOperator.Inequality) {
						if((left == null && right != null) ||
							(right == null && left != null)) {
							DB.bp(left + " != " + right);
							return true;
						}
						else if (left == null && right == null) {
							DB.bp(left + " != " + right);
							return false;
						}
							//insert code here for delegates and enum

						else if	(!lType.IsPrimitive || !rType.IsPrimitive) {
							DB.bp(left + " != " + right);
							//return !left.Equals(right);
							return left != right;
						}
					}
					else {
						throw new InterpreterException("unknown binary operation on reference objects", b.Loc);
					}
				}
				
			}
			
			//object[] paramArray = { left , right };
			
			//how to handle enums?
			//how to handle strings? +, ==, !=
			//how to handle delegates? + - ==, etc...
			//check for overloaded methods
			switch(b.Oper) {
				case(Binary.BiOperator.Addition): { //check for string concats and enum and delegates
					//					MethodInfo overload = findMethod(mia, Binary.Oper_names[(int) Binary.BiOperator.Addition]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					//check for string concat
					if(right.GetType() == TypeManager.string_type) {
						return left.ToString() + right.ToString();
					}
					return Binary.BiArithmetic(Binary.BiOperator.Addition, left, right, b.Loc);		
				}
				case(Binary.BiOperator.BitwiseAnd): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.BitwiseAnd]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					//left is a primitive?
					//switch all prim types?
					return Binary.BiArithmetic(Binary.BiOperator.BitwiseAnd, left, right, b.Loc);			
				}
				case(Binary.BiOperator.BitwiseOr): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.BitwiseOr]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.BitwiseOr, left, right, b.Loc);		
				}
				case(Binary.BiOperator.Division): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.Division]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.Division, left, right, b.Loc);	
				}
				case(Binary.BiOperator.Equality): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.Equality]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}

					//what about strings???
					return Binary.BiArithmetic(Binary.BiOperator.Equality, left, right, b.Loc);	
				}
				case(Binary.BiOperator.GreaterThan): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.GreaterThan]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.GreaterThan, left, right, b.Loc);	
				}
				case(Binary.BiOperator.GreaterThanOrEqual): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.GreaterThanOrEqual]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.GreaterThanOrEqual, left, right, b.Loc);	
				}
				case(Binary.BiOperator.Inequality): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.Inequality]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.Inequality, left, right, b.Loc);	
				}
				case(Binary.BiOperator.LeftShift): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.LeftShift]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.LeftShift, left, right, b.Loc);	
				}
				case(Binary.BiOperator.LessThan): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.LessThan]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.LessThan, left, right, b.Loc);	
				}
				case(Binary.BiOperator.LessThanOrEqual): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.LessThanOrEqual]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.LessThanOrEqual, left, right, b.Loc);	
				}
				case(Binary.BiOperator.LogicalAnd): { //can't overload //convert to conditional ?: to short-circuit
					Conditional c = new Conditional(b.Left,b.Right,BoolLit.FalseInstance, b.Loc);
					return c.execute(this, inp);
				}
				case(Binary.BiOperator.LogicalOr): { //can't overload //convert to ?:
					Conditional c = new Conditional(b.Left,BoolLit.TrueInstance,b.Right, b.Loc);
					return c.execute(this, inp);
				}
				case(Binary.BiOperator.Modulus): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.Modulus]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.Modulus, left, right, b.Loc);	
				}
				case(Binary.BiOperator.Multiply): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.Multiply]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.Multiply, left, right, b.Loc);	
				}
				case(Binary.BiOperator.RightShift): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.RightShift]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.RightShift, left, right, b.Loc);	
				}
				case(Binary.BiOperator.Subtraction): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.Subtraction]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.Subtraction, left, right, b.Loc);	
				}
				case(Binary.BiOperator.ExclusiveOr): {
					//					MethodInfo overload = findMethod(mia,Binary.Oper_names[(int) Binary.BiOperator.ExclusiveOr]);
					//					if(overload != null) { // have an overload
					//						return overload.Invoke(left, paramArray);
					//					}
					return Binary.BiArithmetic(Binary.BiOperator.ExclusiveOr, left, right, b.Loc);	
				}
				default:
					throw new InterpreterException("unknown binary operator", b.Loc);
			}
			
		}	
		
		//		protected MethodInfo findMethod(MethodInfo[] mia, string name) {
		//			foreach(MethodInfo mi in mia) {
		//				if(mi.Name.Equals(name))
		//					return mi;
		//			}
		//			return null;
		//		}

	
		public override object forBoolLit(BoolLit b, object inp) {
			return b.Value;
		}

		public override object forCharLit(CharLit c, object inp) {
			return c.Value;
		}

		public override object forCheckedExpression(CheckedExpression c, object inp) {
			return c.Expr.execute(this, inp);
		}

		//TBI
		public override object forClassCast(ClassCast c, object inp) {

			return null;
		}

		//this is for like int[][]
		public override object forComposedCast(ComposedCast c, object inp) {
			return null;
		}

		public override object forCompoundAssignment(CompoundAssignment c, object inp) {
			//make a binary and then execute
			//then make an assignment
			//OR transform to assignment with rvalue as binary
			Binary b = new Binary(c.Op, c.LVal , c.RVal, c.Loc);
			Assignment a = new Assignment(c.LVal, b, c.Loc);
			return a.execute(this,inp);
		}

		public override object forConditional(Conditional c, object inp) {
			DB.condp("<forConditional>");
			object testResult = extractVariableValue(c.Expr.execute(this, inp));
			DB.condp("testResult == " + testResult);

			if(testResult is bool) {
				if((bool)testResult) { //true so left
					DB.condp("trueExpr");
					return c.TrueExpr.execute(this, inp);
				}
				else {
					DB.condp("falseExpr");
					return c.FalseExpr.execute(this, inp);
				}
			}
			else {
				throw new InterpreterException("test expression of a conditional must evaluate to a boolean");
			}
		}

		
		public override object forDecimalLit(DecimalLit d, object inp) {
			return d.Value;
		}
		
		public override object forDoubleLit(DoubleLit d, object inp) {
			return d.Value;
		}

		public override object forElementAccess(ElementAccess e, object inp) {
			//execute this or LValue???
			//must be an instance so can be 'this'
			object primExp = e.Expr.execute(this, inp);
			if(primExp == null) {
				throw new InterpreterException("null reference in element access", e.Loc);
			}
			
			Type primType = primExp.GetType();
			if(primExp is VariableInfo) {
				primType = ((VariableInfo)primExp).Type;
				primExp = ((VariableInfo)primExp).Value;
			}

			//if primExp is arrayType...
			if(primType.IsArray) {
				Array arr = (Array)primExp;
				
				//process indexers
				ArrayList indList = e.ExprList;
				ArrayList procIndList = new ArrayList();
				foreach(Expression expr in indList) {
					object ind = extractVariableValue(expr.execute(this,inp));
					Type indType = ind.GetType();
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
				int[] intArr = new int[procIndList.Count];
				for(int i =0 ; i<procIndList.Count; i++) {
					intArr[i] = (int)procIndList[i];
				}

				return arr.GetValue(intArr);
			}
				//else it's an indexer access
			else {
				//use type.InvokeMember("Item", BindingFlags.GetProperty
				
				//process indexers
				ArrayList indList = e.ExprList;
				ArrayList procIndList = new ArrayList();
				foreach(Expression expr in indList) {
					object ind = extractVariableValue(expr.execute(this,inp));
					procIndList.Add(ind);
				}
				
				//must have an instance
				object retVal = primType.InvokeMember("Item", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, primExp, procIndList.ToArray() );
				return retVal;
			}
		}

		
		//TBI
		public override object forEmptyCast(EmptyCast e, object inp) {
			return null;
		}		
		
		public override object forFloatLit(FloatLit f, object inp) {
			return f.Value;
		}
		
		public override object forIntLit(IntLit i, object inp) {
			return i.Value;
		}

		
		//need to worry about delegate invocation
		public override object forInvocation(Invocation i, object inp) {
			DB.ip("<forInvocation>");
			//need to get back objectInfo
			//need method group or delegate group
			Info methods = (Info)i.Expr.execute(new LValueVisitor(this), inp);
			//should be objectInfo - want both MethodInfo (for method name), and Type to call InvokeMember
			Type t = null;
			string methodName = null;
			object instance = null;
			bool isDel = false;
			if(methods is ObjectInfo) {
				DB.ip("got back ObjectInfo");
				ObjectInfo oi = (ObjectInfo)methods;
				t = oi.Type;
				methodName = oi.Info.Name;
				instance = oi.Value;
			}
			else if(methods is VariableInfo) {
				DB.ip("got back VariableInfo, have delegate invocation");
				//have delegate invocation
				VariableInfo vi = (VariableInfo)methods;
				isDel = true;
				instance = vi.Value;
				if(instance.GetType() != TypeManager.delegate_type) {
					throw new InterpreterException("invalid invocation", i.Loc);
				}
			}
			else {
				throw new InterpreterException("method could not be found", i.Loc);
			}

			DB.ip("processing argument list");
			//process argument list - note NOT keeping track of ref and out parameters
			ArrayList argList = i.Arguments;
			ArrayList procArgList = new ArrayList();
			if(argList != null) {
				foreach(Argument arg in argList) {
					//get back either prim/obj or VariableInfo
					procArgList.Add(extractVariableValue(arg.Expr.execute(this, inp)));
				}
			}
			object[] oa = procArgList.ToArray();
			if(!isDel) {
				DB.ip("</forInvocation>");
				return t.InvokeMember(methodName,BindingFlags.InvokeMethod, null,instance, oa);
			}
			else { 
				Delegate d = (Delegate)instance;
				DB.ip("invoking delegate");
				DB.ip("</forInvocation>");
				return d.DynamicInvoke(oa);
			}
			//return ret;
		}

		public override object forIs(Is i, object inp) {
			object test = i.Left.execute(this,inp);  //expression
			CType wanted = i.WantedType;
			//process wanted into a type
			Type type = processCType(wanted);
			if(test.GetType() == type) {
				return true;
			}
			return false;
		}
		
		public override object forLongLit(LongLit l, object inp) {
			return l.Value;
		}
		public override object forMemberAccess(MemberAccess m, object inp) {
			DB.mp("<MemberAccess>");
			//E.I
			//check type then namespace
			//eval left hand side
			Info leftInfo = (Info)m.Expr.execute(new LValueVisitor(this), inp);
			//should be ObjectInfo, NamespaceInfo, TypeInfo, or VariableInfo, ArrayInfo
			if(leftInfo == null) {
				throw new InterpreterException("Null reference detected", m.Loc);
			}

			string ident = m.Ident;
			DB.mp("ident == " + ident);

			//result is namespace, error
			if(leftInfo is NamespaceInfo) {
				string ns = (string)leftInfo.Value;
				string ns2 = ns + "." + ident;
				if(tm.IsValidNamespace(ns2)) {
					throw new InterpreterException("cannot access namespace", m.Loc);
				}
			
				//has to be a type or we throw a MissingTypeOrNamespaceException
				return tm.LookupType(ns2);
			}

			else if(leftInfo is TypeInfo) {
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
					return tm.LookupType(maybeType);
				}

				//    if I is a method, then return a method group
				//throw exception except in lValue
				if(mi.MemberType == MemberTypes.Method) {
					throw new InterpreterException("a method group cannot be a return value", m.Loc);
				}

				//    if I is a static property, then return property access
				if(mi.MemberType == MemberTypes.Property) { // how to tell if static!!!
					PropertyInfo pi = (PropertyInfo)mi;
					if(pi.CanRead) {
						return pi.GetValue(null, null);
					}
					else {
						throw new InterpreterException("property '" + ident + "' cannot be read", m.Loc);
					}
				}

				//    if I is static field, if readonly....
				// catches the enum case and also the constant case
				if(mi.MemberType == MemberTypes.Field) {
					FieldInfo fi = (FieldInfo)mi;
					return fi.GetValue(null);
				}

				//    if I is static event, then event access
				if(mi.MemberType == MemberTypes.Event) {
					EventInfo ei = (EventInfo)mi;
					return ei.EventHandlerType;
				}

				//    if I is a constant, value of constant
				//Atrribute Literal
				//    if I is enum member, value of enum member
				//check that ident is a field of the enum

				throw new InterpreterException("invalid member access", m.Loc);
				//    else error
			}

				//VariableInfo or ObjectInfo
			else if(leftInfo is VariableInfo || leftInfo is ObjectInfo || leftInfo is ArrayInfo) {
				//if E is property access, indexer access, variable, or value of type T and I is in T
				
				Type type = leftInfo.Type;
				DB.mp("leftInfo.Type == " + type);
				object instance = leftInfo.Value;
				DB.mp("leftInfo.Value (instance) == " + instance);
				if(leftInfo is ArrayInfo) {
					DB.mp("leftInfo is ArrayInfo");
					//x[y].Foo
					//instance is array, but need to access item
					ArrayInfo ai = (ArrayInfo)leftInfo;
					if(ai.Info == null) { //got prim type array
						Array arr = (Array)ai.Value;
						int[] indexers = objToIntArray(ai.Indexers);
						instance = arr.GetValue(indexers);
						type = instance.GetType();
					}
					else { //got element access 
						object[] indexers = ai.Indexers;
						type = ai.Type;
						instance = ai.Value;
						instance = type.InvokeMember("Item", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, instance,indexers );
						type = instance.GetType();
					}
				}
				else if(leftInfo is ObjectInfo) {
					DB.mp("leftInfo is ObjectInfo");
					ObjectInfo oi = (ObjectInfo)leftInfo;
					type = oi.CurrType;
					instance = oi.CurrValue;
					DB.mp("leftInfo.Type == " + type);
					DB.mp("leftInfo.Value (instance) == " + instance);
				}

				MemberInfo[] mia = type.GetMember(ident, ibFlags);
				if(mia == null) {
					throw new InterpreterException("unknown member access, '" + type.FullName + "' has no member '" + ident + "'", m.Loc);
				}
				MemberInfo mi = mia[0];
				//    if E is a property or indexer access, then return the value //ok
				//invoke get-accessor

				//    if I is a method, then return method group (objectinfo - in LValue only)
				if(mi.MemberType == MemberTypes.Method) {
					throw new InterpreterException("a method group cannot be a return value", m.Loc);
				}

				//    if I is instance property, then return value
				if(mi.MemberType == MemberTypes.Property) {
					PropertyInfo pi = (PropertyInfo)mi;
					if(pi.CanRead) {
						return pi.GetValue(instance, null);
					}
					else {
						throw new InterpreterException("property '" + ident + "' cannot be read", m.Loc);
					}
				}

				//    if T is a class-type, .... result if variable
				if(mi.MemberType == MemberTypes.Field) {
					FieldInfo fi = (FieldInfo)mi;
					return fi.GetValue(instance);
				}

				//    if I is static event, then event access
				if(mi.MemberType == MemberTypes.Event) {
					EventInfo ei = (EventInfo)mi;
					return ei.EventHandlerType;
				}
				//if T is struct type and I is instance field

				//if I is an instance event....


				//else error
				throw new InterpreterException("invalid member access", m.Loc);
			}

			throw new InterpreterException("invalid member access", m.Loc);
			//eval left in lval mode?
			//get back objectInfo
			//ask for member
			//if still in lvalmode, return passer.
			//else return value of access
		}
		
		
		//TBI
		public override object forNew(New n, object inp){
			//process CType
			Type t = processCType(n.WantedType);
			//not array type
			n.Type = t;
			if(t.IsAbstract) {
				throw new InterpreterException("cannot create instance of abstract class '" + t.FullName + "'", n.Loc);
			}
			
			//delegate creation
			if(t.IsSubclassOf(TypeManager.delegate_type) || t == TypeManager.delegate_type) {
				ArrayList argList = n.Arguments;
				//should just be one argument
				if(argList.Count != 1) {
					throw new InterpreterException("invalid delegate creation expression", n.Loc);
				}
				Argument arg = (Argument)argList[0];
				//should be either method group or another delegate type
				object ret = arg.Expr.execute(new LValueVisitor(this), inp);
				//can be ObjectInfo or VariableInfo
				if(ret is ObjectInfo) { //then we have method name and maybe instance
					ObjectInfo oi = (ObjectInfo)ret;
					if(!(oi.Info is MethodInfo)) {
						throw new InterpreterException("invalid method name in delegate creation", n.Loc);
					}
					string method = oi.Info.Name;
					object instance = oi.Value;
					if(t.IsSubclassOf(TypeManager.multicast_delegate_type) || t == TypeManager.multicast_delegate_type) {
						if(instance == null) { //static
							return MulticastDelegate.CreateDelegate(t, (MethodInfo)oi.Info);
						}
						else {
							return MulticastDelegate.CreateDelegate(t, instance, method);
						}
					}
					else {
						if(instance == null) {
							return Delegate.CreateDelegate(t, (MethodInfo)oi.Info);
						}
						else {
							return Delegate.CreateDelegate(t,instance, method);
						}
					}
				}
				else if(ret is VariableInfo) { //we need to make a clone of the delegate
					VariableInfo vi = (VariableInfo)ret;
					Delegate d = (Delegate)vi.Value;
					return d.Clone();
				}

				//call Delegate.CreateInstance(...)
			}

			ArrayList procArgList = new ArrayList();
			if(n.Arguments != null) {
				ArrayList argList = n.Arguments;
				foreach(Argument arg in argList) {
					procArgList.Add(extractVariableValue(arg.Expr.execute(this, inp)));
				}
			}
			object retVal = t.InvokeMember("",BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, null, procArgList.ToArray());
            		
			return retVal;
		}

		public override object forNullLit(NullLit n, object inp) {
			return null;
		}
		
		public override object forParenExpr(ParenExpr p, object inp) {
			object obj = p.Expr.execute(this, inp);
			if(obj is NamespaceInfo || obj is TypeInfo) {
				throw new InterpreterException("a parenthesized expression cannot be a namespace or type", p.Loc);
			}
			return obj;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="inp"></param>
		/// <returns>a VariableInfo, otherwise, exception</returns>
		public override object forSimpleName(SimpleName s, object inp) {			
			VariableInfo vi = env.LookupVariable(s.Name);
			return vi;
		}
		
		public override object forStringLit(StringLit s, object inp) {
			return s.Value;
		}

		
		//TBI
		public override object forThis(This t, object inp){
			return null;
		}

		
		public override object forTypeOf(TypeOf t, object inp){
			//process CType
			Type type = processCType(t.QueriedType);
			//return tm.LookupType(t.QueriedType);
			return type;
		}
		
		public override object forUIntLit(UIntLit u, object inp) {
			return u.Value;
		}
		
		public override object forULongLit(ULongLit u, object inp) {
			return u.Value;
		}
		public override object forUnary(Unary u, object inp){
			DB.up("<forUnary>");
			// + - ! ~
			Unary.UnaryOperator uo = u.Oper;
			object uExpr = extractVariableValue(u.Expr.execute(this, inp));
			if(uExpr == null) {
				throw new InterpreterException("cannot perform unary operation on null", u.Loc);
			}
			Type uType = uExpr.GetType();  
			if(!uType.IsPrimitive) {
				throw new InterpreterException("unary operations only supported for primitive types", u.Loc);
			}

			switch(uo) {
				case(Unary.UnaryOperator.UnaryPlus): {
					if(uType == TypeManager.bool_type) {
						throw new InterpreterException("cannot use + on bool type", u.Loc);
					}
					if(uType == TypeManager.decimal_type ||
						uType == TypeManager.double_type ||
						uType == TypeManager.float_type ||
						uType == TypeManager.uint64_type ||
						uType == TypeManager.int64_type ||
						uType == TypeManager.uint32_type ||
						uType == TypeManager.int32_type) {
						return uExpr;
					}
					else { //convert to int
						int conv = Convert.ToInt32(uExpr);
						return conv;
					}
				}	
				case(Unary.UnaryOperator.UnaryNegation): {
					if(uType == TypeManager.bool_type) {
						throw new InterpreterException("cannot use - on bool type", u.Loc);
					}
					if(uType == TypeManager.decimal_type) {
						return 0 - (decimal)uExpr;
					}
					if(uType == TypeManager.double_type) {
						double d = (double)uExpr;
						return -d;
					}
					if(uType == TypeManager.float_type) {
						float f = (float)uExpr;
						return -f;
					}
					if(uType == TypeManager.uint64_type) {
						throw new InterpreterException("cannot apply - to type ulong", u.Loc);
					}
					if(uType == TypeManager.int64_type) {
						long l = (long)uExpr;
						return -l;
					}
					if(uType == TypeManager.uint32_type) {
						long l = Convert.ToInt64(uExpr);
						return -l;
					}
					if(uType == TypeManager.int32_type) {
						int i = (int)uExpr;
						return -i;
					}
					else { //convert to int
						int conv = Convert.ToInt32(uExpr);
						return -conv;
					}
				}
				case(Unary.UnaryOperator.LogicalNot): {
					if(uType == TypeManager.bool_type) {
						return !(bool)uExpr;
					}
					else {
						throw new InterpreterException("! can only be applied to bool types", u.Loc);
					}
				}
				case(Unary.UnaryOperator.OnesComplement): {
					//what about enumeration types?
					if(uType == TypeManager.bool_type) {
						throw new InterpreterException("cannot use ~ on bool type", u.Loc);
					}
					if(uType == TypeManager.decimal_type) {
						throw new InterpreterException("cannot use ~ on decimal type", u.Loc);
					}
					if(uType == TypeManager.double_type) {
						throw new InterpreterException("cannot use ~ on double type", u.Loc);
					}
					if(uType == TypeManager.float_type) {
						throw new InterpreterException("cannot use ~ on float type", u.Loc);
					}
					if(uType == TypeManager.uint64_type) {
						ulong ul = (ulong)uExpr;
						return ~ul;
					}
					if(uType == TypeManager.int64_type) {
						long l = (long)uExpr;
						return ~l;
					}
					if(uType == TypeManager.uint32_type) {
						uint ui = (uint)uExpr;
						return ~ui;
					}
					if(uType == TypeManager.int32_type) {
						int i = (int)uExpr;
						return ~i;
					}
					else { //convert to int
						int conv = Convert.ToInt32(uExpr);
						return ~conv;
					}
				}
				case(Unary.UnaryOperator.AddressOf):
				case(Unary.UnaryOperator.Indirection):
					throw new InterpreterException("unary operator: '" + uo + "' is unsupported", u.Loc);
				default:
					throw new InterpreterException("weird, unknown unary operator");
			}

			
		}
		
		public override object forUnaryMutator(UnaryMutator u, object inp) {
			int one = 1;
			UnaryMutator.UMode um = u.Mode;
			//should be variableInfo or ObjectInfo
			//			object lVal = u.Expr.execute(new LValueVisitor(this), inp);
			//change to assignment x++ == x = x + 1;? ret x?
			switch(um) {
				case(UnaryMutator.UMode.PostDecrement): {
					//x-- == x = x-1;
					Binary b = new Binary(Binary.BiOperator.Subtraction, u.Expr, new IntLit(one), u.Loc);
					Assignment a = new Assignment(u.Expr, b, u.Loc);
					object ret = extractVariableValue(u.Expr.execute(this, inp));
					a.execute(this, inp);
					return ret;
				}
				case(UnaryMutator.UMode.PostIncrement): {
					//x++ == x = x + 1;
					Binary b = new Binary(Binary.BiOperator.Addition, u.Expr, new IntLit(one), u.Loc);
					Assignment a = new Assignment(u.Expr, b, u.Loc);
					object ret = extractVariableValue(u.Expr.execute(this, inp));
					a.execute(this, inp);
					return ret;
				}
				case(UnaryMutator.UMode.PreDecrement): {
					//--x == x = x-1
					Binary b = new Binary(Binary.BiOperator.Subtraction, u.Expr, new IntLit(one), u.Loc);
					Assignment a = new Assignment(u.Expr, b, u.Loc);
					return a.execute(this, inp);
				}
				case(UnaryMutator.UMode.PreIncrement): {
					//++x
					Binary b = new Binary(Binary.BiOperator.Addition, u.Expr, new IntLit(one), u.Loc);
					Assignment a = new Assignment(u.Expr, b, u.Loc);
					return a.execute(this, inp);
				}
				default:
					throw new InterpreterException("unknown unary mutator mode, should not happen",u.Loc);
			}
		}
		
	
		
		public override object forUncheckedExpression(UncheckedExpression u, object inp){
			return u.Expr.execute(this, inp);
		}

		protected virtual object extractVariableValue(object obj) {
			if(obj is VariableInfo) {
				return ((VariableInfo)obj).Value;
			}
			return obj;
		}

		protected int[] objToIntArray(object[] o) {
			int[] arr = new int[o.Length];
			for(int i = 0; i <o.Length; i++) {
				arr[i] = (int)o[i];
			}
			return arr;
		}

#region MAYBE_NOT_NEEDED
		public override object forBoolConstant(BoolConstant b, object inp){
			return null;
		}
		public override object forUnboxCast(UnboxCast u, object inp){
			return null;
		}
		public override object forULongConstant(ULongConstant u, object inp){
			return null;
		}
		public override object forUIntConstant(UIntConstant u, object inp){
			return null;
		}
		public override object forStringConstant(StringConstant s, object inp){
			return null;
		}
		public override object forSizeOf(SizeOf s, object inp){
			return null;
		}
		public override object forLongConstant(LongConstant l, object inp){
			return null;
		}
		public override object forIntConstant(IntConstant i, object inp){
			return null;
		}
		public override object forFloatConstant(FloatConstant f, object inp) {
			return null;
		}
		public override object forEmptyExpression(EmptyExpression e, object inp){
			return null;
		}
		public override object forBoxedCast(BoxedCast b, object inp){
			return null;
		}
		public override object forCharConstant(CharConstant c, object inp){
			return null;
		}
		public override object forConstant(Constant c, object inp) {
			return null;
		}
		
		public override object forDecimalConstant(DecimalConstant d, object inp){
			return null;
		}
		public override object forDoubleConstant(DoubleConstant d, object inp){
			return null;
		}
		public override object forEnumConstant(EnumConstant e, object inp) {
			return null;
		}
		#endregion

#region Statements_Stuff

		public override object forBlock(Block b, object inp) {
			env.NewScope();
			object retVal = null;
			try {
				foreach(Statement s in b.Statements) {
					retVal = s.execute(this, inp);
				}
			}
			catch(Exception) {
				throw;
			}
			finally {
				env.LeaveScope();
			}
			return retVal;
		}

		public override object forBreak(Break b, object inp){
			throw new BreakException(b.Loc);
		}

		public override object forCheckedStatement(CheckedStatement c, object inp){
			return c.Block.execute(this, inp);
		}

		public override object forContinue(Continue c, object inp){
			throw new ContinueException(c.Loc);
		}
		
		public override object forDo(Do d, object inp){
			return d.execute(new LoopVisitor(this), inp);
		}
		public override object forEmptyStatement(EmptyStatement e, object inp){
			return null;
		}
		public override object forExpressionStatement(ExpressionStatement e, object inp){
			return e.Expr.execute(this, inp);
		}
		
		public override object forFor(For f, object inp){
			return f.execute(new LoopVisitor(this), inp);
		}
		public override object forForeach(Foreach f, object inp){
			return f.execute(new LoopVisitor(this), inp);
		}

		public override object forGoto(Goto g, object inp){
			throw new InterpreterException("goto not allowed in non switch statement context", g.Loc);
		}

		public override object forIf(If i, object inp){
			object test = i.Expr.execute(this, inp);
			if(!(test is System.Boolean)) {
				throw new InterpreterException("test statement in 'if' statement must evaluate to a bool");
			}
			if((bool)test)
				return i.TrueStatement.execute(this, inp);
			else if(i.FalseStatement != null) {
				return i.FalseStatement.execute(this, inp);
			}
			else
				return null;
		}

		public override object forLabeledStatement(LabeledStatement l, object inp){
			//not right
			env.AddLabeledStatement(l.Label_name, l.Statement);
			return l.Statement.execute(this, inp);
		}

		public override object forLocalConstDecl(LocalConstDecl l, object inp){
			Type typeDecl = processCType(l.WantedType);
			foreach(VariableDeclaration varDecl in l.VarDecls) {
				string id = varDecl.Ident;
				if(varDecl.ExpressionOrArrayInit != null) { 
					//variable is initialized
					object val = null;
					if(varDecl.ExpressionOrArrayInit is Expression ) {
						val = ((Expression)varDecl.ExpressionOrArrayInit).execute(new VarDeclVisitor(this), typeDecl);
					}
					else if(varDecl.ExpressionOrArrayInit is ArrayList) {
						throw new NotSupportedException("array initializers are not currently supported");
						//val = processArrayInit((ArrayList)varDecl.ExpressionOrArrayInit, inp);
					}
					VariableInfo vi = new VariableInfo(id, typeDecl, val);
					env.AddVariable(vi);
				}
				else { //no initialization
					VariableInfo vi = new VariableInfo(id, typeDecl);
					env.AddVariable(vi);
				}
			}
			return null;
		}

		public override object forLocalVarDecl(LocalVarDecl l, object inp){
			//remember, can just declare, not have to initialize
			Type typeDecl = processCType(l.WantedType);
			foreach(VariableDeclaration varDecl in l.VarDecls) {
				string id = varDecl.Ident;
				if(varDecl.ExpressionOrArrayInit != null) { 
					//variable is initialized
					object val = null;
					if(varDecl.ExpressionOrArrayInit is Expression ) {
						val = ((Expression)varDecl.ExpressionOrArrayInit).execute(new VarDeclVisitor(this), typeDecl);
					}
					else if(varDecl.ExpressionOrArrayInit is ArrayList) {
						throw new NotSupportedException("array initializers are not currently supported");
						//val = processArrayInit((ArrayList)varDecl.ExpressionOrArrayInit, inp);
					}
					VariableInfo vi = new VariableInfo(id, typeDecl, val);
					env.AddVariable(vi);
				}
				else { //no initialization
					VariableInfo vi = new VariableInfo(id, typeDecl);
					env.AddVariable(vi);
				}
			}

			return null;
		}
	
		//this should return an Array? or object?
		/*
				public object processArrayInit(ArrayList al, object inp) {
					//want to know rank and length

					ArrayList retList = new ArrayList();
					for(int i = 0; i<al.Count; i++ ) {
						object obj = al[i];
						object ret = null;
						if(obj is Expression) {
							ret = ((Expression)obj).execute(this, inp);
						}
						else if(obj is ArrayList) {
							ret = processArrayInit((ArrayList)obj, inp);
						}
						else {
							throw new InterpreterException("error in array initialization");
						}
						retList.Add(ret);
					}
					return retList.ToArray();
				}

				public int[] preprocessArrayInit(ArrayList al, object inp) {
					//either it's another ArrayList(array initializer) 
					//or expression ... could be arraycreation or other expression
					//want to know what is the rank and size and type of this array



				}

				private int[] findArrayInfo(ArrayList al, object inp, Type tIn, out Type tOut) {
					//return value lenght is rank, each index is length of dimension
					//return type of this level, all types must be the same
					ArrayList rankList = new ArrayList();
					int length = 0;
					int rank = 0;
					Type type = null;

					for(int i = 0; i<al.Count; ++i) {
						object obj = al[i];
						if(obj is ArrayList) {
							//increase rank, add length
							//processArrayInitializer.... get back rank and lengh and type???
						}
						else {
							//obj must be expression
							object arItem = ((Expression)obj).execute(this, inp);
							//incr length
						}
					
				
					}

				}
		*/
		public override object forLock(Lock l, object inp) {
			return notSupported();
		}

		public override object forReturn(Return r, object inp) {
			if(r.Expr != null)
				return r.Expr.execute(this, inp);
			return null;
		}

		public override object forSwitch(Switch s, object inp){
			return s.execute(new SwitchVisitor(this), inp);
		}

		public override object forThrow(Throw t, object inp) {
			if(t.Expr == null) {
				throw new InterpreterException("unexpected throw statement", t.Loc);
			}
			else {
				throw (Exception)t.Expr.execute(this, inp);
			}
		}

		public override object forTry(Try t, object inp){
			return t.execute(new TryVisitor(this), inp);
		}

		public override object forUncheckedStatement(UncheckedStatement u, object inp){
			return u.Block.execute(this, inp);
		}

		public override object forUnsafe(Unsafe u, object inp){
			throw new InterpreterException("unsafe code is not supported");
		}

		
		//TBI
		public override object forUsingAlias(UsingAlias u, object inp){
			
			throw new InterpreterException("TBI");
		}

		//check
		public override object forUsingNamespace(UsingNamespace u, object inp){
			tm.AddUsedNamespace(u.Space);

			return null;
		}

		//TBI
		public override object forUsingStatement(UsingStatement u, object inp){
			//perform transformation?
			return null;
		}

		public override object forWhile(While w, object inp){
			return w.execute(new LoopVisitor(this), inp);
		}

#endregion


	}

	
}
