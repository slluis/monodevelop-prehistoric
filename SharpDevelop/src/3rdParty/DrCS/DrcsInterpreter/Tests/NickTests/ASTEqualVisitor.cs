using System;
using System.Collections;
using Rice.Drcsharp.Parser;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;
using Rice.Drcsharp.Interpreter;
using System.IO;

/*
 * AST visitor that returns whether the host and a given input AST have
 * equal values at all levels.  
 */
public class ASTEqualVisitor : IASTVisitor {

	//Singleton Pattern
	public static ASTEqualVisitor ONLY = new ASTEqualVisitor();
	private ASTEqualVisitor() {}


	private bool assert(bool tester) {
		if (tester) {
			return true;
		} else
			throw new Exception("");
	}

	//helper function that sees if two objects' values are equal to one another--
	//Each object can be of type A such that:
	// type A is one of:
	//  -- Expression
	//  -- ArrayList of A
	private bool expressionOrArrayListEquals(object first, object second) {
		if (first is Expression) {
			return (bool)((Expression)first).execute(this,second);
		} else {
			ArrayList tempFirst = ((ArrayList)first);
			ArrayList tempSecond = ((ArrayList)second);
			if (tempFirst.Count != tempSecond.Count) {
				return false;
			} else {
				for(int i=0;i<tempFirst.Count;i++) {
					if (!expressionOrArrayListEquals(tempFirst[i], tempSecond[i])) {
						return false;
					}
				}
				return true;
			}
		}
	}
				
	#region Expressions
	/*
	 * Dispatch branches for all supported C# Expressions
	 */
	public virtual object forArrayCreation(ArrayCreation a, object inp) {
		ArrayCreation input;

		try {
			assert(inp is ArrayCreation);
			input = (ArrayCreation)inp;
			assert(a.RequestedType.Name.Equals(input.RequestedType.Name));
			if(a.Exprs == null || input.Exprs == null) {
				assert(a.Exprs == input.Exprs);
			} else {
				assert(a.Exprs.Count == input.Exprs.Count);
			
				for(int i=0;i<a.Exprs.Count;i++) {
					assert((bool)((ASTNode)a.Exprs[i]).execute(this, input.Exprs[i]));
				}
			}
			
			if(a.Initializers == null || input.Initializers == null) {
				assert(a.Initializers == input.Initializers);
			} else {
				assert(expressionOrArrayListEquals(a.Initializers, input.Initializers));
			}
			
			assert(a.Rank.Equals(input.Rank));
			
		} catch (Exception) {
			return false;
		}
		return true;
	}



	public virtual object forAs(As a, object inp) {
		As input;

		try {
			assert(inp is As);
			input = (As)inp;
			assert(a.Wantedype.Name.Equals(input.Wantedype.Name));
			assert((bool)a.Left.execute(this, input.Left));
			//assert((bool)a.Right.execute(this, input.Right));
			assert(a.Oper == input.Oper);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forAssignment(Assignment a, object inp) {
		Assignment input;

		try {
			assert(inp is Assignment);
			input = (Assignment)inp;
			assert((bool)a.LExpr.execute(this, input.LExpr));
			assert((bool)a.RExpr.execute(this, input.RExpr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forBaseAccess(BaseAccess b, object inp) {
		BaseAccess input;

		try {
			assert(inp is BaseAccess);
			input = (BaseAccess)inp;
			assert(b.Ident.Equals(input.Ident));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forBaseIndexerAccess(BaseIndexerAccess b, object inp) {
		BaseIndexerAccess input;

		try {
			assert(inp is BaseIndexerAccess);
			input = (BaseIndexerAccess)inp;
			assert(b.ExpList.Count == input.ExpList.Count);
			for(int i=0; i<b.ExpList.Count;i++) {
				assert((bool)((ASTNode)b.ExpList[i]).execute(this, input.ExpList[i]));
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}
	
	public virtual object forBinary(Binary b, object inp) {
		Binary input;
		
		try {
			assert(inp is Binary);
			input = (Binary)inp;
			assert(b.Oper == input.Oper);
			assert((bool)b.Left.execute(this, input.Left));
			assert((bool)b.Right.execute(this, input.Right));
		} catch (Exception) {
			return false;
		}
		return true;
	}


	public virtual object forBoolConstant(BoolConstant b, object inp) {
		throw new ParserException("BoolConstant not supposed to be used!");
	}

	public virtual object forBoolLit(BoolLit b, object inp) {
		BoolLit input;

		try {
			assert(inp is BoolLit);
			input = (BoolLit)inp;
			assert(b.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forBoxedCast(BoxedCast b, object inp) {
		BoxedCast input;

		try {
			assert(inp is BoxedCast);
			input = (BoxedCast)inp;
			assert((bool)b.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forCharConstant(CharConstant c, object inp) {
		throw new ParserException("CharConstant not supposed to be used!");
	}

	public virtual object forCharLit(CharLit c, object inp) {
		CharLit input;

		try {
			assert(inp is CharLit);
			input = (CharLit)inp;
			assert(c.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forCheckedExpression(CheckedExpression c, object inp) {
		CheckedExpression input;

		try {
			assert(inp is CheckedExpression);
			input = (CheckedExpression)inp;
			assert((bool)c.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forClassCast(ClassCast c, object inp) {
		ClassCast input;

		try {
			assert(inp is ClassCast);
			input = (ClassCast)inp;
			assert((bool)c.ExpectedType.execute(this, input.ExpectedType));
			assert((bool)c.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forComposedCast(ComposedCast c, object inp) {
		ComposedCast input;

		try {
			assert(inp is ComposedCast);
			input = (ComposedCast)inp;
			assert((bool)c.Expr.execute(this, input.Expr));
			assert(c.Rank == input.Rank);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forCompoundAssignment(CompoundAssignment c, object inp) {
		CompoundAssignment input;

		try {
			assert(inp is CompoundAssignment);
			input = (CompoundAssignment)inp;
			assert((bool)c.LVal.execute(this, input.LVal));
			assert((bool)c.RVal.execute(this, input.RVal));
			assert(c.Op == input.Op);
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	public virtual object forConditional(Conditional c, object inp) {
		Conditional input;

		try {
			assert(inp is Conditional);
			input = (Conditional)inp;
			assert((bool)c.Expr.execute(this, input.Expr));
			assert((bool)c.FalseExpr.execute(this, input.FalseExpr));
			assert((bool)c.TrueExpr.execute(this, input.TrueExpr));
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	//Abstract class!!!
	public virtual object forConstant(Constant c, object inp) {
		Constant input;

		try {
			assert(inp is Constant);
			input = (Constant)inp;
		} catch (Exception) {
			return false;
		} 
		return true;
	}
	public virtual object forDecimalConstant(DecimalConstant d, object inp) {
		throw new ParserException("DecimalConstant not supposed to be used!");
	}

	public virtual object forDecimalLit(DecimalLit d, object inp) {
		DecimalLit input;

		try {
			assert(inp is DecimalLit);
			input = (DecimalLit)inp;
			assert(d.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forDoubleConstant(DoubleConstant d, object inp) {
		throw new ParserException("DoubleConstant not supposed to be used!");
	}

	public virtual object forDoubleLit(DoubleLit d, object inp) {
		DoubleLit input;

		try {
			assert(inp is DoubleLit);
			input = (DoubleLit)inp;
			assert(d.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forElementAccess(ElementAccess e, object inp) {
		ElementAccess input;

		try {
			assert(inp is ElementAccess);
			input = (ElementAccess)inp;
			assert((bool)e.Expr.execute(this, input.Expr));
			for(int i=0; i<e.ExprList.Count;i++) {
				assert((bool)((ASTNode)e.ExprList[i]).execute(this, input.ExprList[i]));
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forEmptyCast(EmptyCast e, object inp) {
		EmptyCast input;

		try {
			assert(inp is EmptyCast);
			input = (EmptyCast)inp;
			assert((bool)e.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forEmptyExpression(EmptyExpression e, object inp) {
		EmptyExpression input;

		try {
			assert(inp is EmptyExpression);
			input = (EmptyExpression)inp;
		} catch (Exception) {
			return false;
		}
		return true;
	}

	//*Constant classes not implemented
	public virtual object forEnumConstant(EnumConstant e, object inp) {
		throw new ParserException("EnumConstant not supposed to be used!");
	}

	//*Constant classes not implemented
	public virtual object forFloatConstant(FloatConstant f, object inp) {
		throw new ParserException("FloatConstant not supposed to be used!");
	}
	
	public virtual object forFloatLit(FloatLit f, object inp) {
		FloatLit input;

		try {
			assert(inp is FloatLit);
			input = (FloatLit)inp;
			assert(f.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}
	
	//*Constant classes not implemented
	public virtual object forIntConstant(IntConstant i, object inp) {
		throw new ParserException("IntConstant not supposed to be used!");
	}
	
	public virtual object forIntLit(IntLit i, object inp) {
		IntLit input;
	
		try {
			assert(inp is IntLit);
			input = (IntLit)inp;
			assert(i.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forInvocation(Invocation i, object inp) {
		Invocation input;

		try {
			assert(inp is Invocation);
			input = (Invocation)inp;
			if (i.Arguments == null) {
				assert(input.Arguments == null);
			} else {
				assert(i.Arguments.Count == input.Arguments.Count);
				for (int j=0; j<i.Arguments.Count;j++) {
					assert(((Argument)i.Arguments[j]).AType ==
						((Argument)input.Arguments[j]).AType);
					assert((bool)((Argument)i.Arguments[j]).Expr.execute(this,
						((Argument)input.Arguments[j]).Expr));
				}
			}
			assert((bool)i.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forIs(Is i, object inp) {
		Is input;

		try {
			assert(inp is Is);
			input = (Is)inp;
			assert(i.WantedType.Name.Equals(input.WantedType.Name));
			assert((bool)i.Left.execute(this, input.Left));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	//*Constant classes not implemented
	public virtual object forLongConstant(LongConstant l, object inp) {
		throw new ParserException("LongConstant not supposed to be used!");
	}

	public virtual object forLongLit(LongLit l, object inp) {
		LongLit input;

		try {
			assert(inp is LongLit);
			input = (LongLit)inp;
			assert(l.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forMemberAccess(MemberAccess m, object inp) {
		MemberAccess input;

		try {
			assert(inp is MemberAccess);
			input = (MemberAccess)inp;
			assert((bool)m.Expr.execute(this, input.Expr));
			assert(m.Ident.Equals(input.Ident));
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	public virtual object forNew(New n, object inp) {
		New input;

		try {
			assert(inp is New);
			input = (New)inp;
			assert(n.WantedType.Name.Equals(input.WantedType.Name));
			if (n.Arguments == null) {
				assert(input.Arguments == null);
			} else {
				assert(n.Arguments.Count == input.Arguments.Count);
				for (int i=0; i<n.Arguments.Count;i++) {
					assert((bool)((Argument)n.Arguments[i]).Expr.execute(this, 
						((Argument)input.Arguments[i]).Expr));
					assert(((Argument)n.Arguments[i]).AType == 
						((Argument)input.Arguments[i]).AType);
				}
			}
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	public virtual object forNullLit(NullLit n, object inp) {
		NullLit input;

		try {
			assert(inp is NullLit);
			input = (NullLit)inp;
			assert(n.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forParenExpr(ParenExpr p, object inp) {
		ParenExpr input;

		try {
			assert(inp is ParenExpr);
			input = (ParenExpr)inp;
			assert((bool)p.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forSimpleName(SimpleName s, object inp) {
		SimpleName input;

		try {
			assert(inp is SimpleName);
			input = (SimpleName)inp;
			assert(s.Name.Equals(input.Name));
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	public virtual object forSizeOf(SizeOf s, object inp) {
		SizeOf input;

		try {
			assert(inp is SizeOf);
			input = (SizeOf)inp;
			assert(s.QueriedType.Name.Equals(input.QueriedType.Name));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	//*Constant classes not implemented
	public virtual object forStringConstant(StringConstant s, object inp) {
		throw new ParserException("StringConstant not supposed to be used!");
	}

	public virtual object forStringLit(StringLit s, object inp) {
		StringLit input;

		try {
			assert(inp is StringLit);
			input = (StringLit)inp;
			assert(s.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forThis(This t, object inp) {
		This input;

		try {
			assert(inp is This);
			input = (This)inp;
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forTypeOf(TypeOf t, object inp) {
		TypeOf input;

		try {
			assert(inp is TypeOf);
			input = (TypeOf)inp;
			assert(t.QueriedType.Name.Equals(input.QueriedType.Name));
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	//*Constant classes not implemented
	public virtual object forUIntConstant(UIntConstant u, object inp) {
		throw new ParserException("UIntConstant not supposed to be used!");
	}

	public virtual object forUIntLit(UIntLit u, object inp) {
		UIntLit input;

		try {
			assert(inp is UIntLit);
			input = (UIntLit)inp;
			assert(u.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	//*Constant classes not implemented
	public virtual object forULongConstant(ULongConstant u, object inp) {
		throw new ParserException("ULongConstant not supposed to be used!");
	}

	public virtual object forULongLit(ULongLit u, object inp) {
		ULongLit input;

		try {
			assert(inp is ULongLit);
			input = (ULongLit)inp;
			assert(u.Value == input.Value);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forUnary(Unary u, object inp) {
		Unary input;

		try {
			assert(inp is Unary);
			input = (Unary)inp;
			assert((bool)u.Expr.execute(this, input.Expr));
			assert(u.Oper == input.Oper);
		} catch (Exception) {
			return false;
		} 
		return true;
	}

	public virtual object forUnaryMutator(UnaryMutator u, object inp) {
		UnaryMutator input;

		try {
			assert(inp is UnaryMutator);
			input = (UnaryMutator)inp;
			assert((bool)u.Expr.execute(this, input.Expr));
			assert(u.Mode == input.Mode);
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forUnboxCast(UnboxCast u, object inp) {
		UnboxCast input;

		try {
			assert(inp is UnboxCast);
			input = (UnboxCast)inp;
			assert((bool)u.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forUncheckedExpression(UncheckedExpression u, object inp) {
		UncheckedExpression input;

		try {
			assert(inp is UncheckedExpression);
			input = (UncheckedExpression)inp;
			assert((bool)u.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	#endregion

	#region Statements
	/*
	 * Dispatch branches for all supported C# Statements
	 */
	public virtual object forBlock(Block b, object inp) {
		Block input;

		try {
			assert(inp is Block);
			input = (Block)inp;
			if (b.Statements == null) {
				assert(input.Statements == null);
			} else {
				for(int i=0; i<b.Statements.Count;i++) {
					assert((bool)((Statement)b.Statements[i]).execute(this,
						((Statement)input.Statements[i])));
				}
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}

	public virtual object forBreak(Break b, object inp) {
		Break input;

		try {
			assert(inp is Break);
			input = (Break)inp;
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forCheckedStatement(CheckedStatement c, object inp) {
		CheckedStatement input;

		try {
			assert(inp is CheckedStatement);
			input = (CheckedStatement)inp;
			assert((bool)c.Block.execute(this, input.Block));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forContinue(Continue c, object inp) {
		Continue input;

		try {
			assert(inp is Continue);
			input = (Continue)inp;
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forDo(Do d, object inp) {
		Do input;

		try {
			assert(inp is Do);
			input = (Do)inp;
			assert((bool)d.EmbeddedStatement.execute(this, input.EmbeddedStatement));
			assert((bool)d.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forEmptyStatement(EmptyStatement e, object inp) {
		EmptyStatement input;

		try {
			assert(inp is EmptyStatement);
			input = (EmptyStatement)inp;
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forExpressionStatement(ExpressionStatement e, object inp) {
		ExpressionStatement input;

		try {
			assert(inp is ExpressionStatement);
			input = (ExpressionStatement)inp;
			assert((bool)e.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forFor(For f, object inp) {
		For input;

		try {
			assert(inp is For);
			input = (For)inp;
			
			if (f.Test == null || input.Test == null) {
				assert(f.Test == input.Test);
			} else {
				assert((bool)f.Test.execute(this, input.Test));
			}
					
			if (f.ForIterator == null || input.ForIterator == null) {
				assert(f.ForIterator == input.ForIterator);
			} else {
				assert(f.ForIterator.Count == input.ForIterator.Count);
				for(int i=0; i<f.ForIterator.Count;i++) {
					assert((bool)((ASTNode)f.ForIterator[i]).execute(this,
						input.ForIterator[i]));
				}
			}

			assert((bool)f.Statement.execute(this, input.Statement));
			
			if (f.DeclOrInit == null || input.DeclOrInit == null) {
				assert(f.DeclOrInit == input.DeclOrInit);
			} else if (f.DeclOrInit is LocalVarDecl) {
				assert((bool)((LocalVarDecl)f.DeclOrInit).execute(this, 
					((LocalVarDecl)input.DeclOrInit)));
			} else {
				ArrayList temp = (ArrayList)f.DeclOrInit;
				ArrayList temp2 = (ArrayList)input.DeclOrInit;
				assert(temp.Count == temp2.Count);
				for(int i=0; i<temp.Count;i++) {
					assert((bool)((ASTNode)temp[i]).execute(this, ((ASTNode)temp2[i])));
				}
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forForeach(Foreach f, object inp) {
		Foreach input;

		try {
			assert(inp is Foreach);
			input = (Foreach)inp;
			assert((bool)f.Expr.execute(this, input.Expr));
			assert(f.WantedType.Name.Equals(input.WantedType.Name));
			assert((bool)f.Statement.execute(this, input.Statement));
			assert(f.LocalVariable.Equals(input.LocalVariable));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forGoto(Goto g, object inp) {
		Goto input;

		try {
			assert(inp is Goto);
			input = (Goto)inp;
			
			switch (g.GType) {
				case Goto.GotoType.Ident:
					assert(input.GType == Goto.GotoType.Ident);
					assert(g.Ident.Equals(input.Ident));
					break;
				case Goto.GotoType.Case:
					assert(input.GType == Goto.GotoType.Case);
					assert((bool)g.ConstExpr.execute(this, input.ConstExpr));
					break;
				case Goto.GotoType.Default:
					assert(input.GType == Goto.GotoType.Default);
					break;
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forIf(If i, object inp) {
		If input;

		try {
			assert(inp is If);
			input = (If)inp;
			assert((bool)i.Expr.execute(this, input.Expr));
			if (i.FalseStatement == null) {
				assert(input.FalseStatement == null);
			} else {
				assert((bool)i.FalseStatement.execute(this, input.FalseStatement));
			}
			assert((bool)i.TrueStatement.execute(this, input.TrueStatement));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forLabeledStatement(LabeledStatement l, object inp) {
		LabeledStatement input;

		try {
			assert(inp is LabeledStatement);
			input = (LabeledStatement)inp;
			assert((bool)l.Statement.execute(this, input.Statement));
			assert(l.Label_name.Equals(input.Label_name));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forLocalConstDecl(LocalConstDecl l, object inp) {
		LocalConstDecl input;

		try {
			assert(inp is LocalConstDecl);
			input = (LocalConstDecl)inp;
			assert(l.VarDecls.Count == input.VarDecls.Count);
			for(int i=0;i<l.VarDecls.Count;i++) {
				assert(((VariableDeclaration)l.VarDecls[i]).Ident.Equals(((VariableDeclaration)input.VarDecls[i]).Ident));
				assert(expressionOrArrayListEquals(((VariableDeclaration)l.VarDecls[i]).ExpressionOrArrayInit,((VariableDeclaration)input.VarDecls[i]).ExpressionOrArrayInit));
			}
			assert(l.WantedType.Name.Equals(input.WantedType.Name));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forLocalVarDecl(LocalVarDecl l, object inp) {
		LocalVarDecl input;

		try {
			assert(inp is LocalVarDecl);
			input = (LocalVarDecl)inp;
			assert(l.VarDecls.Count == input.VarDecls.Count);
			for(int i=0;i<l.VarDecls.Count;i++) {
				assert(((VariableDeclaration)l.VarDecls[i]).Ident.Equals(((VariableDeclaration)input.VarDecls[i]).Ident));
				assert(expressionOrArrayListEquals(((VariableDeclaration)l.VarDecls[i]).ExpressionOrArrayInit,((VariableDeclaration)input.VarDecls[i]).ExpressionOrArrayInit));
			}
			assert(l.WantedType.Name.Equals(input.WantedType.Name));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forLock(Lock l, object inp) {
		Lock input;

		try {
			assert(inp is Lock);
			input = (Lock)inp;
			assert((bool)l.Expr.execute(this, input.Expr));
			assert((bool)l.Statement.execute(this, input.Statement));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forReturn(Return r, object inp) {
		Return input;

		try {
			assert(inp is Return);
			input = (Return)inp;
			if (r.Expr == null) {
				assert(input.Expr == null);
			} else {
				assert((bool)r.Expr.execute(this, input.Expr));
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forSwitch(Switch s, object inp) {
		Switch input;

		try {
			assert(inp is Switch);
			input = (Switch)inp;
			assert((bool)s.Expr.execute(this, input.Expr));
			if (s.Sections == null) {
				assert(input.Sections == null);
			} else {
				assert(s.Sections.Count == input.Sections.Count);
				for(int i=0;i<s.Sections.Count;i++) {
					SwitchSection temp1 = ((SwitchSection)s.Sections[i]);
					SwitchSection temp2 = ((SwitchSection)s.Sections[i]);
					if (temp1.Labels == null) {
						assert(temp2.Labels == null);
					} else {
						assert(temp1.Labels.Count == temp2.Labels.Count);
						for(int j=0;j<temp1.Labels.Count;j++) {
							assert((bool)((SwitchLabel)temp1.Labels[j]).Expr.execute(this,((SwitchLabel)temp1.Labels[j]).Expr));
						}
						assert(temp1.StatementList.Count == temp2.StatementList.Count);
						for(int j=0;j<temp1.StatementList.Count;j++) {
							assert((bool)((Statement)temp1.StatementList[j]).execute(this,temp1.StatementList[j]));
						}
					}
				}
			}
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forThrow(Throw t, object inp) {
		Throw input;

		try {
			assert(inp is Throw);
			input = (Throw)inp;
			assert((bool)t.Expr.execute(this, input.Expr));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forTry(Try t, object inp) {
		Try input;

		try {
			assert(inp is Try);
			input = (Try)inp;
			assert((bool)t.Block.execute(this, input.Block));
			
			//Finally Block
			if (t.FinallyBlock == null) {
				assert(input.FinallyBlock == null);
			} else {
				assert((bool)t.FinallyBlock.execute(this, input.FinallyBlock));
			}
			
			//Specific Catch list
			if (t.SpecCatchList == null) {
				assert(input.SpecCatchList == null);
			} else {
				assert(t.SpecCatchList.Count == input.SpecCatchList.Count);
				for (int i=0;i<t.SpecCatchList.Count;i++) {
					Catch temp1 = ((Catch)t.SpecCatchList[i]);
					Catch temp2 = ((Catch)input.SpecCatchList[i]);
					assert((bool)temp1.Block.execute(this,temp2.Block));
					if(temp1.CatchArgs.Ident == null) {
						assert(temp2.CatchArgs.Ident == null);
					} else {
						assert(temp1.CatchArgs.Ident.Equals(temp2.CatchArgs.Ident));
					}
					assert(temp1.CatchArgs.WantedType.Name.Equals(temp2.CatchArgs.WantedType.Name));
				}
			}
			
			//General Catch
			if (t.GeneralCatch == null) {
				assert(input.GeneralCatch == null);
			} else {
				assert((bool)t.GeneralCatch.Block.execute(this, input.GeneralCatch.Block));
				System.Console.WriteLine(t.GeneralCatch.CatchArgs == null);
			}
			
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forUncheckedStatement(UncheckedStatement u, object inp) {
		UncheckedStatement input;

		try {
			assert(inp is UncheckedStatement);
			input = (UncheckedStatement)inp;
			assert((bool)u.Block.execute(this, input.Block));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forUnsafe(Unsafe u, object inp) {
		Unsafe input;

		try {
			assert(inp is Unsafe);
			input = (Unsafe)inp;
			assert((bool)u.Block.execute(this, input.Block));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forUsingAlias(UsingAlias u, object inp) {
		UsingAlias input;

		try {
			assert(inp is UsingAlias);
			input = (UsingAlias)inp;
			assert(u.Alias.Equals(input.Alias));
			assert(u.Defn.Equals(input.Defn));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forUsingNamespace(UsingNamespace u, object inp) {
		UsingNamespace input;

		try {
			assert(inp is UsingNamespace);
			input = (UsingNamespace)inp;
			assert(u.Space.Equals(input.Space));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	public virtual object forUsingStatement(UsingStatement u, object inp) {
		UsingStatement input;

		try {
			assert(inp is UsingStatement);
			input = (UsingStatement)inp;
			assert((bool)((ASTNode)u.ExprOrDecl).execute(this, input.ExprOrDecl));
			assert((bool)u.Statement.execute(this, input.Statement));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	
	public virtual object forWhile(While w, object inp) {
		While input;

		try {
			assert(inp is While);
			input = (While)inp;
			assert((bool)w.Expr.execute(this, input.Expr));
			assert((bool)w.Statement.execute(this, input.Statement));
		} catch (Exception) {
			return false;
		}
		return true;
	}
	#endregion
}	
	