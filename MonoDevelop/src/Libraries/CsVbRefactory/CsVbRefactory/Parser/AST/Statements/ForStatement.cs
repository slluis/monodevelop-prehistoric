using System;
using System.Collections;

namespace MonoDevelop.CsVbRefactory.Parser.AST 
{
	public class ForStatement : Statement
	{
		ArrayList  initializers = new ArrayList(); // EmbeddedStatement OR list of StatmentExpressions
		Expression condition;
		ArrayList  iterator = new ArrayList();     // list of StatmentExpressions
		Statement  embeddedStatement;
		
		public ArrayList Initializers {
			get {
				return initializers;
			}
		}
		
		public Expression Condition {
			get {
				return condition;
			}
			set {
				condition = value;
			}
		}
		
		public ArrayList Iterator {
			get {
				return iterator;
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
		
		public ForStatement(ArrayList initializers, Expression condition, ArrayList iterator, Statement embeddedStatement)
		{
			if (initializers != null) {
				this.initializers = initializers;
			}
			this.condition = condition;
			if (iterator != null) {
				this.iterator = iterator;
			}
			this.embeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ForStatement: Initializers{0}, Condition={1}, iterator{2}, EmbeddedStatement={3}]", 
			                     GetCollectionString(initializers),
			                     condition,
			                     GetCollectionString(iterator),
			                     embeddedStatement);
		}
	}
}
