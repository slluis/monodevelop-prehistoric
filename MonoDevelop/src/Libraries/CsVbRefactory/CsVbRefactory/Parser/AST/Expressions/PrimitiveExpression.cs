using System;
using System.Collections;
using System.Globalization;

namespace ICSharpCode.CsVbRefactory.Parser.AST {
	
	public class PrimitiveExpression : Expression
	{
		object val;
		string stringValue;
		
		public object Value {
			get {
				return val;
			}
			set {
				val = value;
			}
		}
		
		public string StringValue {
			get {
				return stringValue;
			}
			set {
				stringValue = value;
			}
		}
		
		public PrimitiveExpression(object val, string stringValue)
		{
			this.val = val;
			this.stringValue = stringValue;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[PrimitiveExpression: StringValue={0}]",
			                     stringValue);
		}
	}
}
