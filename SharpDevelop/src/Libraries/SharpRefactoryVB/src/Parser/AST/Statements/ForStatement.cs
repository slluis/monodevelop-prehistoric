using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class ForStatement : Statement
	{
		Expression start;
		Expression end;
		Expression step;
		Statement embeddedStatement;
		ArrayList nextExpressions;
		string variableName;
		TypeReference type;
		
		public string VariableName
		{
			get {
				return variableName;
			}
			set {
				variableName = value;
			}
		}
		
		public TypeReference Type
		{
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		public ArrayList NextExpressions
		{
			get {
				return nextExpressions;
			}
			set {
				nextExpressions = value;
			}
		}
		
		public Expression Start
		{
			get {
				return start;
			}
			set {
				start = value;
			}
		}
		
		public Expression End
		{
			get {
				return end;
			}
			set {
				end = value;
			}
		}
		
		public Expression Step
		{
			get {
				return step;
			}
			set {
				step = value;
			}
		}
		
		public Statement EmbeddedStatement
		{
			get {
				return embeddedStatement;
			}
			set {
				embeddedStatement = value;
			}
		}
		
		public ForStatement(string variableName, TypeReference type, Expression start, Expression end, Expression step, Statement embeddedStatement, ArrayList nextExpressions)
		{
			this.start = start;
			this.nextExpressions = nextExpressions;
			this.end = end;
			this.step = step;
			this.embeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
