using System;
using System.Collections;

namespace ICSharpCode.CsVbRefactory.Parser.AST 
{
	public class ForeachStatement : Statement
	{
		TypeReference typeReference;
		string        variableName;
		Expression    expression;
		Statement     embeddedStatement;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = value;
			}
		}
		
		public string VariableName {
			get {
				return variableName;
			}
			set {
				variableName = value;
			}
		}
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = value;
			}
		}
		
		public Statement EmbeddedStatement {
			get {
				return embeddedStatement;
			}
			set {
				embeddedStatement = value;
			}
		}
		
		public ForeachStatement(TypeReference typeReference, string variableName, Expression expression, Statement embeddedStatement)
		{
			this.typeReference     = typeReference;
			this.variableName      = variableName;
			this.expression        = expression;
			this.embeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ForeachStatement: TypeReference={0}, Expression={1}, EmbeddedStatement={2}]", 
			                     typeReference,
			                     expression,
			                     embeddedStatement);
		}

	}
}
