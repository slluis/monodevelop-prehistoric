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
	public abstract class AExpressionVisitor : IExpressionVisitor
	{
		protected string name;

		public AExpressionVisitor(string n) {
			name = n;
		}

		public AExpressionVisitor() {
			name = "AExpressionVisitor";
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
		public virtual object forThis(This t, object inp){
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
	}
}
