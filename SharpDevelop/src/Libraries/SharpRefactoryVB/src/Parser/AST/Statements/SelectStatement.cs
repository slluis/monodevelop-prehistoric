using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class SelectStatement : BlockStatement
	{
		Expression selectExpression;
		ArrayList  selectSections  = new ArrayList();
		
		public Expression SelectExpression {
			get {
				return selectExpression;
			}
			set {
				selectExpression = value;
			}
		}
		
		public ArrayList SelectSections
		{
			get {
				return selectSections;
			}
			set {
				selectSections = value;
			}
		}
		
		public SelectStatement(Expression selectExpression, ArrayList selectSections)
		{
			this.selectExpression = selectExpression;
			this.selectSections = selectSections;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
	
	public class SelectSection : BlockStatement
	{
		ArrayList caseClauses = new ArrayList();
		
		public ArrayList CaseClauses
		{
			get {
				return caseClauses;
			}
			set {
				caseClauses = value;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
	
	public class CaseClause
	{
		BinaryOperatorType comparisonOperator = BinaryOperatorType.None;
		Expression comparisonExpression;
		Expression boundaryExpression;
		bool isDefaultCase = false;
		
		public bool IsDefaultCase
		{
			get {
				return isDefaultCase;
			}
			set {
				isDefaultCase = value;
			}
		}
		
		public BinaryOperatorType ComparisonOperator
		{
			get {
				return comparisonOperator;
			}
			set {
				comparisonOperator = value;
			}
		}
		
		public Expression ComparisonExpression
		{
			get {
				return comparisonExpression;
			}
			set {
				comparisonExpression = value;
			}
		}
		
		public Expression BoundaryExpression
		{
			get {
				return boundaryExpression;
			}
			set {
				boundaryExpression = value;
			}
		}
		
		public CaseClause(bool isDefaultCase)
		{
			isDefaultCase = isDefaultCase;
		}
		
		public CaseClause(Expression comparisonExpression, Expression boundaryExpression)
		{
			this.boundaryExpression = boundaryExpression;
			this.comparisonExpression = comparisonExpression;
		}
		
		public CaseClause(BinaryOperatorType comparisonOperator, Expression comparisonExpression)
		{
			this.comparisonOperator = comparisonOperator;
			this.comparisonExpression = comparisonExpression;
		}
	}
}
