using System;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;

namespace Rice.Drcsharp.Parser.AST.Visitors
{
	/// <summary>
	/// Summary description for IStatementVisitor.
	/// </summary>
	public interface IStatementVisitor
	{
		object forBlock(Block b, object inp);
		object forBreak(Break b, object inp);
		object forCheckedStatement(CheckedStatement c, object inp);
		object forContinue(Continue c, object inp);
		//object forDeclaration(Declaration d, object inp);
		object forDo(Do d, object inp);
		object forEmptyStatement(EmptyStatement e, object inp);
		object forExpressionStatement(ExpressionStatement e, object inp);
		//object forFixed(Fixed f, object inp);
		object forFor(For f, object inp);
		object forForeach(Foreach f, object inp);
		object forGoto(Goto g, object inp);
		object forIf(If i, object inp);
		object forLabeledStatement(LabeledStatement l, object inp);
		object forLocalConstDecl(LocalConstDecl l, object inp);
		object forLocalVarDecl(LocalVarDecl l, object inp);
	
		object forLock(Lock l, object inp);
		object forReturn(Return r, object inp);
		object forSwitch(Switch s, object inp);
		object forThrow(Throw t, object inp);
		object forTry(Try t, object inp);
		object forUncheckedStatement(UncheckedStatement u, object inp);
		object forUnsafe(Unsafe u, object inp);
		object forUsingAlias(UsingAlias u, object inp);
		object forUsingNamespace(UsingNamespace u, object inp);
		object forUsingStatement(UsingStatement u, object inp);
		object forWhile(While w, object inp);

	}
}
