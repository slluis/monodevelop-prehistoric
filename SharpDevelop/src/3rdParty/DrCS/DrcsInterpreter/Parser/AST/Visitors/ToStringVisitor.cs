using System;
using System.Text;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Visitors
{
	/// <summary>
	/// Summary description for ToStringVisitor.
	/// </summary>
	public class ToStringVisitor : AASTVisitor
	{
		private static ToStringVisitor instance = null;
		
		private static ToStringVisitor Instance {
			get {
				if(null == instance)
					instance = new ToStringVisitor();
				return instance;
			}
		}
		private ToStringVisitor() {
			sb = new StringBuilder();
			tabCount = 0;
		}

		private StringBuilder sb;
		private int tabCount;

		private void insertTabStrNew(string s) {
			sb.Append(tabs());
			sb.Append(s);
			sb.Append("\n");
		}

		private void insertTabStr(string s) {
			sb.Append(tabs());
			sb.Append(s);
		}

		private void insertStr(string s) {
			sb.Append(s);
		}

		private void insertStrNew(string s) {
			sb.Append(s);
			sb.Append("\n");
		}

		private void reset() {
			sb.Length = 0;
			tabCount = 0;
		}

		private string tabs() {
			switch(tabCount) {
				case 0:
					return "";
				case 1:
					return "\t";
				case 2:
					return "\t\t";
				case 3:
					return "\t\t\t";
				case 4:
					return "\t\t\t\t";
				default:
					string s = "\t\t\t\t";
					for(int i = 4; i < tabCount; i++) {
						s+= "\t";
					}
					return s;
			}
		}
		
		private void decTab() {
			--tabCount;
		}

		private void incTab() {
			++tabCount;
		}

		/// <summary>
		/// Gets a string representation of an ASTNode
		/// </summary>
		/// <param name="a">an ASTNode</param>
		/// <returns>string representation of the ASTNode</returns>
		/// <remarks>This method is the only way to print the ASTNode as there is no
		/// way to create an instance of this class.</remarks>
		public static string ASTToString(ASTNode a) {
			ToStringVisitor me = Instance;
			me.reset();
			a.execute(me, null);
			return me.sb.ToString();
		}
		
		public override object forBlock(Block b, object inp) {
			insertTabStrNew("{");
			incTab();
			
			ArrayList a = b.Statements;
			executeStatementList(a, inp);

			decTab();
			insertTabStrNew("}");

			return null;
		}

		public override object forBreak(Break b, object inp) {
			insertTabStrNew("break;");
			return null;
		}

		public override object forCheckedStatement(CheckedStatement c, object inp) {
			insertTabStrNew("checked");
			Block b = c.Block;
			b.execute(this, inp);
			
			return null;
		}

		public override object forContinue(Continue c, object inp) {
			insertTabStrNew("continue;");
			return null;
		}
		//public override object forDeclaration(Declaration d, object inp);
		public override object forDo(Do d, object inp) {
			//DB.db("ToString::forDo");
			insertTabStrNew("do");
			incTab();
			Statement s = d.EmbeddedStatement;
			s.execute(this, inp);
			decTab();
			insertTabStr("while( ");
			d.Expr.execute(this, inp);
			insertStrNew(" );");

			return null;
		}
		public override object forEmptyStatement(EmptyStatement e, object inp) {
			insertTabStrNew(";");
			return null;
		}

		public override object forExpressionStatement(ExpressionStatement e, object inp) {
			e.Expr.execute(this, inp);
			insertStrNew(";");
			return null;
		}

		public override object forFor(For f, object inp) {
			insertTabStr("for( ");
			
			//for initializer
			object o = f.DeclOrInit;
			if(o is Statement) {
				((Statement)o).execute(this, inp);
			}
			else if(o != null) { //is ArrayList
				ArrayList a = (ArrayList)o;
				executeExpressionList(a, inp);
			}
			insertStr(";");
			
			//for condition
			Expression e = f.Test;
			if(e != null)
				e.execute(this,null);
			insertStr(";");

			//for iterator
			ArrayList forIt = f.ForIterator;
			if(forIt != null)
				executeExpressionList(forIt, inp);
			insertStrNew(" )");
			
			incTab();

			Statement s = f.Statement;
			s.execute(this, inp);

			return null;		
		}

		public override object forForeach(Foreach f, object inp) {
			insertTabStr("foreach( ");
			
			insertStr(f.WantedType.Name + " ");
			insertStr(f.LocalVariable + " in ");

			((Expression)f.Expr).execute(this, inp);
			insertStr(" )");

			incTab();
			
			f.Statement.execute(this, inp);

			decTab();

			return null;
		}

		public override object forGoto(Goto g, object inp) {
			insertTabStr("goto ");

			switch(g.GType) {
				case(Goto.GotoType.Ident):
					insertStrNew(g.Ident + " ;");
					break;
				case(Goto.GotoType.Case):
					insertStr("case ");
					g.ConstExpr.execute(this,null);
					insertStrNew(";");
					break;
				case(Goto.GotoType.Default):
					insertStrNew("default;");
					break;
				default:
					throw new Exception("unknown goto type");
			}
			return null;
		}

		public override object forIf(If i, object inp) {
			insertTabStr("if( ");
			i.Expr.execute(this, inp);

			insertStrNew(" )");
			incTab();

			i.TrueStatement.execute(this, inp);
			
			decTab();

			if(null != i.FalseStatement) {
				insertTabStr("else ");
				incTab();
				i.FalseStatement.execute(this, inp);
				decTab();
			}

			return null;
		}

		public override object forLabeledStatement(LabeledStatement l, object inp) {
			insertTabStrNew(l.Label_name + " : ");
			l.Statement.execute(this, inp);
			return null;
		}

		public override object forLocalConstDecl(LocalConstDecl l, object inp) {
			insertTabStr("const ");
			insertStr(l.WantedType.Name + " ");

			ArrayList al = l.VarDecls;
			for(int i = 0; i < al.Count; i++ ) {
				VariableDeclaration vd = (VariableDeclaration)al[i];
				insertStr(vd.Ident);
				if(null != vd.ExpressionOrArrayInit) {
					insertStr(" = ");
					Expression exp = (Expression)vd.ExpressionOrArrayInit;
					exp.execute(this, inp);
				}
				if(i+1 != al.Count)
					insertStr(", ");
			}

			insertStrNew(";");
			return null;
		}

		public override object forLocalVarDecl(LocalVarDecl l, object inp) {
			insertTabStr(l.WantedType.Name + " ");

			ArrayList al = l.VarDecls;
			for(int i = 0; i < al.Count; i++ ) {
				VariableDeclaration vd = (VariableDeclaration)al[i];
				insertStr(vd.Ident);
				if(null != vd.ExpressionOrArrayInit) {
					insertStr(" = ");
					if(vd.ExpressionOrArrayInit is Expression) {
						Expression exp = (Expression)vd.ExpressionOrArrayInit;
						exp.execute(this, inp);
					}
					else {
						ArrayList ai = (ArrayList)vd.ExpressionOrArrayInit;
						printArrayInit(ai,inp);
					}

				}
				if(i+1 != al.Count)
					insertStr(", ");
			}

			insertStrNew(";");
			return null;
		}

		public void printArrayInit(ArrayList al, object inp) {
			insertStr("{");
			for(int i = 0; i<al.Count; i++) {
				object obj = al[i];
				if(obj is Expression) {
					((Expression)obj).execute(this, inp);
				}
				else if(obj is ArrayList) {
					printArrayInit((ArrayList)obj, inp);
				}
				insertStr(",");
			}
			insertStr("}");
		}

		public override object forLock(Lock l, object inp) {
			insertTabStr("lock( ");
			l.Expr.execute(this, inp);
			insertStrNew(" )");
			incTab();
			l.Statement.execute(this, inp);
			decTab();

			return null;
		}

		public override object forReturn(Return r, object inp) {
			insertTabStr("return ");
			if(null != r.Expr) {
				r.Expr.execute(this, inp);
			}
			insertStrNew(";");
			return null;
		}
		//public override object forStatementExpression(StatementExpression s, object inp) {
			
		//}
		public override object forSwitch(Switch s, object inp) {
			insertTabStr("switch( ");
			s.Expr.execute(this, inp);
			insertStrNew(" ) {");
			incTab();
			//switch sections
			ArrayList al = s.Sections;
			for(int i = 0; i < al.Count; i++ ){
				SwitchSection ss = (SwitchSection)al[i];
				//switch labels
				ArrayList labels = ss.Labels;
				for(int j = 0; j<labels.Count; j++) {
					SwitchLabel sl = (SwitchLabel)labels[j];
					if(null != sl.Expr) {
						insertTabStr("case ");
						sl.Expr.execute(this, inp);
						insertStrNew(":");
					}
					else {
						insertTabStr("default :");
					}
				}
				incTab();
				//statements
				ArrayList stmts = ss.StatementList;
				executeStatementList(stmts, inp);
				decTab();
			}
			decTab();
			insertTabStrNew("}");
			return null;
		}

		public override object forThrow(Throw t, object inp) {
			insertTabStr("throw ");
			if(null != t.Expr) {
				t.Expr.execute(this, inp);
			}
			insertStrNew(";");

			return null;
		}

		public override object forTry(Try t, object inp) {
			insertTabStr("try ");
			t.Block.execute(this, inp);

			if(null != t.SpecCatchList) {
				ArrayList scl = t.SpecCatchList;
				for(int i = 0; i<scl.Count; i++) {
					Catch c = (Catch)scl[i];
					insertTabStr("catch ");
					insertStr("( " + c.CatchArgs.WantedType.ToString());
					if(null != c.CatchArgs.Ident) {
						insertStrNew(" " + c.CatchArgs.Ident + " )");
					}
					else {
						insertStrNew(" )");
					}
					c.Block.execute(this, inp);
				}
			}
			if(null != t.GeneralCatch) {
				insertTabStrNew("catch");
				t.GeneralCatch.Block.execute(this,null);
			}
			if(null != t.FinallyBlock) {
				insertTabStrNew("finally");
				t.FinallyBlock.execute(this,null);
			}

			return null;			
		}

		public override object forUncheckedStatement(UncheckedStatement u, object inp) {
			insertTabStrNew("unchecked");
			Block b = u.Block;
			b.execute(this, inp);
			
			return null;
		}
		
		public override object forUnsafe(Unsafe u, object inp) {
			insertTabStrNew("unsafe");
			Block b = u.Block;
			b.execute(this, inp);
			
			return null;
		}

		public override object forUsingAlias(UsingAlias u, object inp) {
			insertTabStrNew("using " + u.Alias + " = " + u.Defn + ";");
			return null;
		}

		public override object forUsingNamespace(UsingNamespace u, object inp) {
			insertTabStrNew("using " + u.Space + ";");
			return null;
		}

		public override object forUsingStatement(UsingStatement u, object inp) {
			insertTabStr("using( ");
			object o = u.ExprOrDecl;
			if(o is Statement) {
				((Statement)o).execute(this, inp);
			}
			else {
				((Expression)o).execute(this, inp);
			}
			insertStrNew(" )");

			u.Statement.execute(this, inp);
			return null;
		}
		public override object forWhile(While w, object inp) {
			insertTabStr("while( ");
			w.Expr.execute(this,null);
			insertStrNew(" )");
			w.Statement.execute(this, inp);

			return null;
		}
		
		
		public override object forArrayCreation(ArrayCreation a, object inp) {
			insertStr("new ");
			insertStr(a.RequestedType.ToString() + " ");
			
			if(null != a.Exprs) {
				insertStr("[");
				executeExpressionList(a.Exprs, inp);
				insertStr("] ");
			}			
	
			insertStr(a.Rank + " ");
			if(a.Initializers != null) {
				printArrayInitializer(a.Initializers, inp);
			}

			return null;
		}

		private void printArrayInitializer(ArrayList al, object inp) {
			insertStr("{ ");
			for(int i = 0; i < al.Count; i++) {
				object obj = al[i];

				if(obj is Expression) {
					((Expression)obj).execute(this, inp);
				}
				else {
					//obj is ArrayList
					printArrayInitializer((ArrayList)obj, inp);
				}
				if(i+1 <al.Count)
					insertStr(", ");
			}
			insertStr("}");
		}

//		public override object forArrayInitializer(ArrayInitializer a, object inp) {
//			ArrayList al = a.ExprList;
//		}

		public override object forAs(As a, object inp) {
			a.Left.execute(this, inp);
			insertStr(" as ");
			insertStr(a.Type.ToString());
			return null;
		}

		public override object forAssignment(Assignment a, object inp) {
			a.LExpr.execute(this, inp);
			insertStr(" = ");
			a.RExpr.execute(this, inp);
			return null;
		}
		public override object forBaseAccess(BaseAccess b, object inp) {
			insertStr("base." + b.Ident);
			return null;
		}

		public override object forBaseIndexerAccess(BaseIndexerAccess b, object inp) {
			insertStr("base [");
			executeExpressionList(b.ExpList, inp);
			insertStr("]");
			return null;
		}
		public override object forBinary(Binary b, object inp) {
			b.Left.execute(this, inp);
			insertStr(" " + Binary.OperName(b.Oper) + " ");
			b.Right.execute(this, inp);

			return null;
		}

		public override object forBoolConstant(BoolConstant b, object inp) {
			if(b.Value)
				insertStr("true ");
			else
				insertStr("false ");

			return null;
		}

		public override object forBoolLit(BoolLit b, object inp) {
			if(b.Value)
				insertStr("true ");
			else
				insertStr("false ");

			return null;
		}

		public override object forBoxedCast(BoxedCast b, object inp) {
			return ((EmptyCast)b).execute(this, inp);
		}

		public override object forCharConstant(CharConstant c, object inp) {
			insertStr(Char.ToString(c.Value));
			return null;
		}

		public override object forCharLit(CharLit c, object inp) {
			insertStr(Char.ToString(c.Value));
			return null;
		}

		public override object forCheckedExpression(CheckedExpression c, object inp) {
			insertStr("checked(");
			c.Expr.execute(this,inp);
			insertStr(")");

			return null;
		}
		public override object forClassCast(ClassCast c, object inp) {
			insertStr("(");
			c.ExpectedType.execute(this, inp);
			insertStr(")");

			c.Expr.execute(this, inp);
			return null;
		}

		public override object forComposedCast(ComposedCast c, object inp) {
			insertStr("(");
			c.Expr.execute(this, inp);
			insertStr(c.Rank);
			insertStr(")");
			return null;
		}

		public override object forCompoundAssignment(CompoundAssignment c, object inp) {
			c.LVal.execute(this, inp);
			insertStr(Binary.OperName(c.Op));
			c.RVal.execute(this, inp);

			return null;
		}

		public override object forConditional(Conditional c, object inp) {
			c.Expr.execute(this, inp);
			insertStr(" ? ");
			c.TrueExpr.execute(this, inp);
			insertStr(" : ");
			c.FalseExpr.execute(this, inp);

			return null;
		}

		public override object forConstant(Constant c, object inp) {
			insertStr("" + c.GetValue());
			return null;
		}

		//public override object forConvCast(ConvCast c, object inp) {
	//		return ((EmptyCast)c).execute(this, inp);
	//	}

		public override object forDecimalConstant(DecimalConstant d, object inp) {
			insertStr("" + d.Value);
			return null;
		}

		public override object forDecimalLit(DecimalLit d, object inp) {
			insertStr("" + d.Value);
			return null;
		}

		public override object forDoubleConstant(DoubleConstant d, object inp) {
			insertStr("" + d.Value);
			return null;
		}

		public override object forDoubleLit(DoubleLit d, object inp) {
			insertStr("" + d.Value);
			return null;
		}

		public override object forElementAccess(ElementAccess e, object inp) {
			e.Expr.execute(this, inp);
			insertStr("[");
			executeExpressionList(e.ExprList, inp);
			insertStr("] ");
			return null;
		}

		public override object forEmptyCast(EmptyCast e, object inp) {
			insertStr("(");
			insertStr(e.Type.ToString());
			insertStr(")");

			e.Expr.execute(this, inp);
			return null;
		}

		public override object forEmptyExpression(EmptyExpression e, object inp) {
			throw new UnsupportedException("should never call on EmptyExpression");
		}

		public override object forEnumConstant(EnumConstant e, object inp) {
			throw new UnsupportedException("should never call on EnumConstant");
		}

		public override object forFloatConstant(FloatConstant f, object inp) {
			insertStr("" + f.Value);
			return null;
		}

		public override object forFloatLit(FloatLit f, object inp) {
			insertStr("" + f.Value);
			return null;
		}

		public override object forIntConstant(IntConstant i, object inp) {
			insertStr("" + i.Value);
			return null;
		}

		public override object forIntLit(IntLit i, object inp) {
			insertStr("" + i.Value);
			return null;
		}

		public override object forInvocation(Invocation i, object inp) {
			i.Expr.execute(this, inp);
			insertStr("(");
			if(null != i.Arguments) {
				processArgumentList(i.Arguments, inp);
			}
			insertStr(")");
			return null;
		}

		public override object forIs(Is i, object inp) {
			i.Left.execute(this, inp);
			insertStr(" is ");
			insertStr(i.Type.ToString());
			return null;
		}
		public override object forLongConstant(LongConstant l, object inp) {
			insertStr("" + l.Value);
			return null;
		}
		public override object forLongLit(LongLit l, object inp) {
			insertStr("" + l.Value);
			return null;
		}
		public override object forMemberAccess(MemberAccess m, object inp) {
			m.Expr.execute(this, inp);
			insertStr("." + m.Ident);
			return null;
		}

		public override object forNew(New n, object inp) {
			insertStr("new ");
			insertStr(n.WantedType.Name);
			insertStr(" (");
			if(null != n.Arguments) {
				processArgumentList(n.Arguments, inp);
			}
			insertStr(")");
			return null;
		}
		
		public override object forNullLit(NullLit n, object inp) {
			insertStr("null");
			return null;
		}

		public override object forParenExpr(ParenExpr p, object inp) {
			insertStr("(");
			p.Expr.execute(this, inp);
			insertStr(")");

			return null;
		}

		public override object forSimpleName(SimpleName s, object inp) {
			insertStr(s.Name);
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="inp"></param>
		/// <returns></returns>
		/// <remarks>currently not parsed in</remarks>
		public override object forSizeOf(SizeOf s, object inp) {
			insertStr("sizeof(");
			insertStr(s.QueriedType.Name);
			insertStr(")");

			return null;
		}

//		object forStackAlloc(StackAlloc s, object inp) {
//			
//		}
		public override object forStringConstant(StringConstant s, object inp) {
			insertStr("\"" + s.Value + "\"");
			return null;
		}
		public override object forStringLit(StringLit s, object inp) {
			insertStr("\"" + s.Value + "\"");
			return null;
		}

		public override object forThis(This t, object inp) {
			insertStr("this");
			return null;
		}

		public override object forTypeOf(TypeOf t, object inp) {
			insertStr("typeof(");
			insertStr(t.QueriedType.Name);
			insertStr(")");

			return null;
		}

		public override object forUIntConstant(UIntConstant u, object inp) {
			insertStr(u.Value + "U");
			return null;
		}

		public override object forUIntLit(UIntLit u, object inp) {
			insertStr(u.Value + "U");
			return null;
		}

		public override object forULongConstant(ULongConstant u, object inp) {
			insertStr(u.Value + "UL");
			return null;
		}

		public override object forULongLit(ULongLit u, object inp) {
			insertStr(u.Value + "UL");
			return null;
		}

		public override object forUnary(Unary u, object inp) {
			insertStr(Unary.OperName(u.Oper));
			u.Expr.execute(this, inp);
			return null;
		}

		public override object forUnaryMutator(UnaryMutator u, object inp) {
			switch(u.Mode) {
				case UnaryMutator.UMode.PostDecrement:
					u.Expr.execute(this, inp);
					insertStr("--");
					break;
				case UnaryMutator.UMode.PostIncrement:
					u.Expr.execute(this, inp);
					insertStr("++");
					break;
				case UnaryMutator.UMode.PreDecrement:
					insertStr("--");
					u.Expr.execute(this, inp);
					break;
				case UnaryMutator.UMode.PreIncrement:
					insertStr("++");
					u.Expr.execute(this, inp);
					break;
				default:
					throw new Exception("unknown unary type");
			}
			return null;
		}
			
		public override object forUnboxCast(UnboxCast u, object inp) {
			return ((EmptyCast)u).execute(this, inp);
		}
					 
		public override object forUncheckedExpression(UncheckedExpression u, object inp) {
			insertStr("unchecked(");
			u.Expr.execute(this,inp);
			insertStr(")");

			return null;
		}

		private void executeStatementList(ArrayList al, object inp) {
			for(int i = 0; i < al.Count; i++ ) {
				Statement s = (Statement)al[i];
				s.execute(this, inp);
			}
		}

		private void executeExpressionList(ArrayList al, object inp) {
			for(int i = 0; i < al.Count; i++ ) {
				Expression e = (Expression)al[i];
				e.execute(this, inp);
				if(i+1< al.Count)
					insertStr(", ");
			}
		}

		private void processArgumentList(ArrayList al, object inp) {
			for(int j=0; j< al.Count ; ++j) {
				Argument a = (Argument)al[j];
				if(Argument.ArgType.Out == a.AType) {
					insertStr("out ");
					a.Expr.execute(this, inp);
				}
				else if(Argument.ArgType.Ref == a.AType) {
					insertStr("ref ");
					a.Expr.execute(this, inp);
				}
				else {
					a.Expr.execute(this, inp);
				}
				if(j+1<al.Count)
					insertStr(", ");
			}
		}
	}
}
