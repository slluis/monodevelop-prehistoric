using System;
using Rice.Drcsharp.Parser.AST.Expressions;

namespace Rice.Drcsharp.Parser.AST.Visitors
{
	/// <summary>
	/// Summary description for IExpressionVisitor.
	/// </summary>
	public interface IExpressionVisitor {
		object forArrayCreation(ArrayCreation a, object inp);
		//object forArrayInitializer(ArrayInitializer a, object inp);
		object forAs(As a, object inp);
		object forAssignment(Assignment a, object inp);
		object forBaseAccess(BaseAccess b, object inp);
		object forBaseIndexerAccess(BaseIndexerAccess b, object inp);
		object forBinary(Binary b, object inp);
		object forBoolConstant(BoolConstant b, object inp);
		object forBoolLit(BoolLit b, object inp);
		object forBoxedCast(BoxedCast b, object inp);
		//public object forByteLit(ByteLit b, object inp); //maybe not needed
		object forCharConstant(CharConstant c, object inp);
		object forCharLit(CharLit c, object inp); 
		object forCheckedExpression(CheckedExpression c, object inp);
		object forClassCast(ClassCast c, object inp);
		object forComposedCast(ComposedCast c, object inp);
		object forCompoundAssignment(CompoundAssignment c, object inp);
		object forConditional(Conditional c, object inp);
		object forConstant(Constant c, object inp);
		//object forConvCast(ConvCast c, object inp);
		object forDecimalConstant(DecimalConstant d, object inp);
		object forDecimalLit(DecimalLit d, object inp);
		object forDoubleConstant(DoubleConstant d, object inp);
		object forDoubleLit(DoubleLit d, object inp);
		object forElementAccess(ElementAccess e, object inp);
		object forEmptyCast(EmptyCast e, object inp);
		object forEmptyExpression(EmptyExpression e, object inp);
		object forEnumConstant(EnumConstant e, object inp);
		object forFloatConstant(FloatConstant f, object inp);
		object forFloatLit(FloatLit f, object inp);
		object forIntConstant(IntConstant i, object inp);
		object forIntLit(IntLit i, object inp);
		object forInvocation(Invocation i, object inp);
		object forIs(Is i, object inp);
		object forLongConstant(LongConstant l, object inp);
		object forLongLit(LongLit l, object inp);
		object forMemberAccess(MemberAccess m, object inp);
		object forNew(New n, object inp);
		object forNullLit(NullLit n, object inp);
		object forParenExpr(ParenExpr p, object inp);
		//public object forSByteLit(SByteLit s, object inp); //maybe not needed
		//public object forShortLit(ShortLit s, object inp); //maybe not needed
		object forSimpleName(SimpleName s, object inp);
		object forSizeOf(SizeOf s, object inp);
		//object forStackAlloc(StackAlloc s, object inp);
		object forStringConstant(StringConstant s, object inp);
		object forStringLit(StringLit s, object inp);
		object forThis(This t, object inp);
		object forTypeOf(TypeOf t, object inp);
		object forUIntConstant(UIntConstant u, object inp);
		object forUIntLit(UIntLit u, object inp);
		object forULongConstant(ULongConstant u, object inp);
		object forULongLit(ULongLit u, object inp);
		object forUnary(Unary u, object inp);
		object forUnaryMutator(UnaryMutator u, object inp);
		object forUnboxCast(UnboxCast u, object inp);
		object forUncheckedExpression(UncheckedExpression u, object inp);
		//public object forUShortLit(UShortLit u, object inp); //maybe not needed

	}
}
