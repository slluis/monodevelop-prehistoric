using System;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Visitors
{
	/// <summary>
	/// Summary description for AStatementVisitor.
	/// </summary>
	public abstract class AStatementVisitor : IStatementVisitor
	{
		protected string name;
		
		public AStatementVisitor(string n) {
			name = n;
		}

		public AStatementVisitor() {
			name = "AStatementVisitor";
		}

		private object notSupported() {
			throw new UnsupportedException("this function is not supported in visitor:" + name);
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
		
		public virtual object forDo(Do d, object inp){
			return notSupported();
		}
		public virtual object forEmptyStatement(EmptyStatement e, object inp){
			return notSupported();
		}
		public virtual object forExpressionStatement(ExpressionStatement e, object inp){
			return notSupported();
		}
		
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
