using System;

namespace Rice.Drcsharp.Parser.AST.Visitors
{
	/// <summary>
	/// Summary description for IASTVisitor.
	/// </summary>
	public interface IASTVisitor : IStatementVisitor, IExpressionVisitor
	{
	}
}
