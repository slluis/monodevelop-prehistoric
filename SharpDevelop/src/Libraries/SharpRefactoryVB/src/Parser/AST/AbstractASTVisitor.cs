using System;
using System.Collections;
using ICSharpCode.SharpRefactory.Parser.AST.VB;

namespace ICSharpCode.SharpRefactory.Parser.VB
{
	public abstract class AbstractASTVisitor : IASTVisitor
	{
		protected Stack blockStack = new Stack();
		
		public BlockStatement CurrentBlock {
			get {
				if (blockStack.Count == 0) {
					return null;
				}
				return (BlockStatement)blockStack.Peek();
			}
		}
		
		public virtual object Visit(INode node, object data)
		{
			return node.AcceptChildren(this, data);
		}
		
		public virtual object Visit(CompilationUnit compilationUnit, object data)
		{
			if (compilationUnit == null) {
				return data;
			}
			return compilationUnit.AcceptChildren(this, data);
		}
		
		public virtual object Visit(FieldDeclaration fieldDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			return namespaceDeclaration.AcceptChildren(this, data);
		}
		
		public virtual object Visit(ImportsStatement importsStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(ImportsAliasDeclaration importsAliasDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(TypeDeclaration typeDeclaration, object data)
		{
			return typeDeclaration.AcceptChildren(this, data);
		}
		
		public virtual object Visit(DelegateDeclaration delegateDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(VariableDeclaration variableDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(MethodDeclaration methodDeclaration, object data)
		{
			blockStack.Push(methodDeclaration.Body);
			object ret = null;
			if (methodDeclaration.Body != null) {
				methodDeclaration.Body.AcceptChildren(this, data);
			}
			blockStack.Pop();
			return ret;
		}
		
		public virtual object Visit(AttributeSection attributeSection, object data)
		{
			return data;
		}
		
		public virtual object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			return data;
		}
		
		public virtual object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			return parameterDeclarationExpression.AcceptChildren(this, data);
		}
		
		public virtual object Visit(PropertyDeclaration propertyDeclaration, object data)
		{
			if (propertyDeclaration.HasGetRegion) {
				propertyDeclaration.GetRegion.AcceptVisitor(this, data);
			}
			if (propertyDeclaration.HasSetRegion) {
				propertyDeclaration.SetRegion.AcceptVisitor(this, data);
			}
			return data;
		}
		
		public virtual object Visit(PropertyGetRegion propertyGetRegion, object data)
		{
			blockStack.Push(propertyGetRegion.Block);
			object ret = null;
			if (propertyGetRegion.Block != null) {
				ret = propertyGetRegion.Block.AcceptChildren(this, data);
			}
			blockStack.Pop();
			return ret;
		}
		
		public virtual object Visit(PropertySetRegion PropertySetRegion, object data)
		{
			blockStack.Push(PropertySetRegion.Block);
			object ret = null;
			if (PropertySetRegion.Block != null) {
				ret = PropertySetRegion.Block.AcceptChildren(this, data);
			}
			blockStack.Pop();
			return ret;
		}
		
		public virtual object Visit(EventDeclaration eventDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(BlockStatement blockStatement, object data)
		{
			if (blockStatement == null) {
				return null;
			}
			blockStack.Push(blockStatement);
			object ret = blockStatement.AcceptChildren(this, data);
			blockStack.Pop();
			return ret;
		}
		
		public virtual object Visit(Statement statement, object data)
		{
			return data;
		}
		
		public virtual object Visit(AddHandlerStatement addHandlerStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(ReturnStatement returnStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(RaiseEventStatement raiseEventStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(ExitStatement exitStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(OptionStrictDeclaration objectStrictDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(OptionCompareDeclaration objectCompareDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(OptionExplicitDeclaration objectExplicitDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(DeclareDeclaration declareDeclaration, object data)
		{
			return data;
		}
		
		public virtual object Visit(EndStatement endStatement, object data)
		{
			return data;
		}
		
		public virtual object Visit(WhileStatement whileStatement, object data)
		{
			if (whileStatement.EmbeddedStatement == null) {
				return null;
			}
			return whileStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(DoLoopStatement doLoopStatement, object data)
		{
			if (doLoopStatement.EmbeddedStatement == null) {
				return null;
			}
			return doLoopStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		public virtual object Visit(ForStatement forStatement, object data)
		{
			if (forStatement.EmbeddedStatement == null) {
				return null;
			}
			return forStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(ForeachStatement foreachStatement, object data)
		{
			if (foreachStatement.EmbeddedStatement == null) {
				return null;
			}
			return foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		public virtual object Visit(LockStatement lockStatement, object data)
		{
			if (lockStatement.EmbeddedStatement == null) {
				return null;
			}
			return lockStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		public virtual object Visit(ImportsDeclaration importsDeclaration, object data)
		{
			return null;
		}
		
		public virtual object Visit(TryCatchStatement tryCatchStatement, object data)
		{
			if (tryCatchStatement.StatementBlock == null) {
				return null;
			}
			return tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
		}
		public virtual object Visit(ThrowStatement throwStatement, object data)
		{
			if (throwStatement.ThrowExpression == null) {
				return null;
			}
			return throwStatement.ThrowExpression.AcceptVisitor(this, data);
		}
		
		public virtual object Visit(PrimitiveExpression primitiveExpression, object data)
		{
			return data;
		}
		public virtual object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			return data;
		}
		public virtual object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return data;
		}
		public virtual object Visit(InvocationExpression invocationExpression, object data)
		{
			return data;
		}
		public virtual object Visit(IdentifierExpression identifierExpression, object data)
		{
			return data;
		}
		public virtual object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return data;
		}
		public virtual object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			return data;
		}
		public virtual object Visit(AssignmentExpression assignmentExpression, object data)
		{
			return data;
		}
		public virtual object Visit(CastExpression castExpression, object data)
		{
			return data;
		}
		public virtual object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return data;
		}
		public virtual object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return data;
		}
		public virtual object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			return data;
		}
		public virtual object Visit(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			return data;
		}
	}
}
