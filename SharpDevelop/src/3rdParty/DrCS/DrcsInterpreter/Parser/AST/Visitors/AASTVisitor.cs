using System;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Visitors
{
	/// <summary>
	/// Summary description for AASTVisitor.
	/// </summary>
	public abstract class AASTVisitor : IASTVisitor
	{
		protected string name;

		public AASTVisitor(string n) {
			name = n;
		}

		public AASTVisitor() {
			name = "AASTVisitor";
		}

		protected object notSupported() {
			throw new UnsupportedException("this function is not supported in visitor:" + name);
		}

		public virtual object forArrayCreation(ArrayCreation a, object inp) {
			return notSupported();
		}
		
		public virtual object forAs(As a, object inp) {
			return notSupported();
		}
		public virtual object forAssignment(Assignment a, object inp){
			return notSupported();
		}
		public virtual object forBaseAccess(BaseAccess b, object inp){
			return notSupported();
		}
		public virtual object forBaseIndexerAccess(BaseIndexerAccess b, object inp){
			return notSupported();
		}
		public virtual object forBinary(Binary b, object inp){
			return notSupported();
		}
		public virtual object forBoolConstant(BoolConstant b, object inp){
			return notSupported();
		}
		public virtual object forBoolLit(BoolLit b, object inp){
			return notSupported();
		}
		public virtual object forBoxedCast(BoxedCast b, object inp){
			return notSupported();
		}
		
		public virtual object forCharConstant(CharConstant c, object inp){
			return notSupported();
		}
		public virtual object forCharLit(CharLit c, object inp){
			return notSupported();
		} 
		public virtual object forCheckedExpression(CheckedExpression c, object inp){
			return notSupported();
		}
		public virtual object forClassCast(ClassCast c, object inp){
			return notSupported();
		}
		public virtual object forComposedCast(ComposedCast c, object inp){
			return notSupported();
		}
		public virtual object forCompoundAssignment(CompoundAssignment c, object inp){
			return notSupported();
		}
		public virtual object forConditional(Conditional c, object inp){
			return notSupported();
		}
		public virtual object forConstant(Constant c, object inp){
			return notSupported();
		}
		
		public virtual object forDecimalConstant(DecimalConstant d, object inp){
			return notSupported();
		}
		public virtual object forDecimalLit(DecimalLit d, object inp){
			return notSupported();
		}
		public virtual object forDoubleConstant(DoubleConstant d, object inp){
			return notSupported();
		}
		public virtual object forDoubleLit(DoubleLit d, object inp){
			return notSupported();
		}
		public virtual object forElementAccess(ElementAccess e, object inp){
			return notSupported();
		}
		public virtual object forEmptyCast(EmptyCast e, object inp){
			return notSupported();
		}
		public virtual object forEmptyExpression(EmptyExpression e, object inp){
			return notSupported();
		}
		public virtual object forEnumConstant(EnumConstant e, object inp){
			return notSupported();
		}
		public virtual object forFloatConstant(FloatConstant f, object inp){
			return notSupported();
		}
		public virtual object forFloatLit(FloatLit f, object inp){
			return notSupported();
		}
		public virtual object forIntConstant(IntConstant i, object inp){
			return notSupported();
		}
		public virtual object forIntLit(IntLit i, object inp){
			return notSupported();
		}
		public virtual object forInvocation(Invocation i, object inp){
			return notSupported();
		}
		public virtual object forIs(Is i, object inp){
			return notSupported();
		}
		public virtual object forLongConstant(LongConstant l, object inp){
			return notSupported();
		}
		public virtual object forLongLit(LongLit l, object inp){
			return notSupported();
		}
		public virtual object forMemberAccess(MemberAccess m, object inp){
			return notSupported();
		}
		public virtual object forNew(New n, object inp){
			return notSupported();
		}
		public virtual object forNullLit(NullLit n, object inp){
			return notSupported();
		}
		public virtual object forParenExpr(ParenExpr p, object inp){
			return notSupported();
		}
		public virtual object forSimpleName(SimpleName s, object inp){
			return notSupported();
		}
		public virtual object forSizeOf(SizeOf s, object inp){
			return notSupported();
		}
		
		public virtual object forStringConstant(StringConstant s, object inp){
			return notSupported();
		}
		public virtual object forStringLit(StringLit s, object inp){
			return notSupported();
		}
		public virtual object forThis(This t, object inp) {
			return notSupported();
		}
		
		public virtual object forTypeOf(TypeOf t, object inp){
			return notSupported();
		}
		public virtual object forUIntConstant(UIntConstant u, object inp){
			return notSupported();
		}
		public virtual object forUIntLit(UIntLit u, object inp){
			return notSupported();
		}
		public virtual object forULongConstant(ULongConstant u, object inp){
			return notSupported();
		}
		public virtual object forULongLit(ULongLit u, object inp){
			return notSupported();
		}
		public virtual object forUnary(Unary u, object inp){
			return notSupported();
		}
		public virtual object forUnaryMutator(UnaryMutator u, object inp){
			return notSupported();
		}
		public virtual object forUnboxCast(UnboxCast u, object inp){
			return notSupported();
		}
		public virtual object forUncheckedExpression(UncheckedExpression u, object inp){
			return notSupported();
		}
		public virtual object forBlock(Block b, object inp) {
			return notSupported();
		}

		public virtual object forBreak(Break b, object inp){
			return notSupported();
		}
		public virtual object forCheckedStatement(CheckedStatement c, object inp){
			return notSupported();
		}
		public virtual object forContinue(Continue c, object inp){
			return notSupported();
		}
		//public virtual object Declaration(Declaration d, object inp);
		public virtual object forDo(Do d, object inp){
			return notSupported();
		}
		public virtual object forEmptyStatement(EmptyStatement e, object inp){
			return notSupported();
		}
		public virtual object forExpressionStatement(ExpressionStatement e, object inp){
			return notSupported();
		}
		//public virtual object Fixed(Fixed f, object inp);
		public virtual object forFor(For f, object inp){
			return notSupported();
		}
		public virtual object forForeach(Foreach f, object inp){
			return notSupported();
		}
		public virtual object forGoto(Goto g, object inp){
			return notSupported();
		}
		public virtual object forIf(If i, object inp){
			return notSupported();
		}
		public virtual object forLabeledStatement(LabeledStatement l, object inp){
			return notSupported();
		}
		public virtual object forLocalConstDecl(LocalConstDecl l, object inp){
			return notSupported();
		}
		public virtual object forLocalVarDecl(LocalVarDecl l, object inp){
			return notSupported();
		}
	
		public virtual object forLock(Lock l, object inp){
			return notSupported();
		}
		public virtual object forReturn(Return r, object inp){
			return notSupported();
		}
		public virtual object forSwitch(Switch s, object inp){
			return notSupported();
		}
		public virtual object forThrow(Throw t, object inp){
			return notSupported();
		}
		public virtual object forTry(Try t, object inp){
			return notSupported();
		}
		public virtual object forUncheckedStatement(UncheckedStatement u, object inp){
			return notSupported();
		}
		public virtual object forUnsafe(Unsafe u, object inp){
			return notSupported();
		}
		public virtual object forUsingAlias(UsingAlias u, object inp){
			return notSupported();
		}
		public virtual object forUsingNamespace(UsingNamespace u, object inp){
			return notSupported();
		}
		public virtual object forUsingStatement(UsingStatement u, object inp){
			return notSupported();
		}
		public virtual object forWhile(While w, object inp){
			return notSupported();
		}
	}
}
