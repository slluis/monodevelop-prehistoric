using System;
using System.Collections;

namespace MonoDevelop.CsVbRefactory.Parser.AST 
{
	public class ObjectCreateExpression : Expression
	{
		TypeReference createType;
		ArrayList     parameters; // Expressions
		
		public TypeReference CreateType {
			get {
				return createType;
			}
			set {
				createType = value;
			}
		}
		public ArrayList Parameters {
			get {
				return parameters;
			}
		}
		
		public ObjectCreateExpression(TypeReference createType, ArrayList parameters)
		{
			this.createType = createType;
			if (parameters != null) {
				this.parameters = parameters;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ObjectCreateExpression: CreateType={0}, Parameters={1}]",
			                     createType,
			                     GetCollectionString(parameters));
		}
	}
}
