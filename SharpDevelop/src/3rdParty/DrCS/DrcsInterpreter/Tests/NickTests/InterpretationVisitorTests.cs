namespace Rice.Drcsharp.Tests.InterpreterTests {

	using NUnit.Framework;
	using System;
	using System.IO;
	using System.Collections;
	using System.Reflection;
	using System.Security.Permissions;
	
	using Rice.Drcsharp.Parser;
	using Rice.Drcsharp.Parser.AST;
	using Rice.Drcsharp.Parser.AST.Expressions;
	using Rice.Drcsharp.Parser.AST.Statements;
	using Rice.Drcsharp.Parser.AST.Visitors;
	using Rice.Drcsharp.Interpreter;

	//Suite of tests for the InterpretationVisitor class
	[TestFixture]
	public class SuiteInterpretationVisitorTest {


		InterpretationVisitor iv;

		//Debugging method to write a given object's ToString() to standard output
		public void write(object inp) {
			System.Console.WriteLine(inp);
		}

		public void Assert(string n, bool cond) {
			Assertion.Assert(n,cond);
		}

		public void Fail(string n) {
			Assertion.Fail(n);
		}

		private ASTNode MakeAST(string n) {
			CSharpParser parser = new CSharpParser("parserName", new StringReader(n));
			parser.parse();
			return parser.retVal;
		}
		
		private class TestingClass {
			public int Testing = 0;
		}

		//Tests for the protected processCType(CType) method -- NOT DONE
		public void TestProcessCType() {

			iv = new InterpretationVisitor(new TypeManager());
			//Reflection stuff
			Type temp = (Type)typeof(InterpretationVisitor).GetMethod("processCType",BindingFlags.Instance | BindingFlags.NonPublic).Invoke(iv,new CType[] {new CType("int[,][]")});
			


		}

		
		//Tests for the Assignment branch
		public void TestForAssignment() {
			IntLit exIntLit = new IntLit(5);
			iv = new InterpretationVisitor(new TypeManager());
			object temp;

			//Invalid left-hand-side case
			try {
				iv.forAssignment(new Assignment(exIntLit,exIntLit,null),null);
				Fail("Didn't report an error in InterpretationVisitor's forAssignment() branch with" +
					" a non-valid left-hand-side.");
			} catch (InterpreterException) {
			}

			
			//VariableInfo LHS case
			iv.Env.AddVariable("x",typeof(int));
			temp = iv.forAssignment(new Assignment(new SimpleName("x",null),exIntLit,null),null);
			Assert("InterpretationVisitor's forAssignment() branch didn't assign a value to to a valid"
				+ " environment variable correctly", 
				iv.Env.LookupVariable("x").Value.Equals(5));
			Assert("InterpretationVisitor's forAssignment() branch didn't return the right value",
				temp.Equals(5));

			//ArrayInfo LHS case -- NOT DONE
			iv.Env.AddVariable("y",typeof(int[]), Array.CreateInstance( typeof(Int32), 6));
			temp = iv.forAssignment((Assignment)MakeAST("y[5]=5"),null);
			Assert("InterpretationVisitor's forAssignment() branch didn't assign a value to to a valid"
				+ " environment variable correctly", 
				((Array)iv.Env.LookupVariable("y").Value).GetValue(5).Equals(5));
			Assert("InterpretationVisitor's forAssignment() branch didn't return the right value",
				temp.Equals(5));

			//Objectinfo LHS cases:
			//PropertyInfo LHS case
			iv.Env.AddVariable("z",typeof(TypeInfo), new TypeInfo(typeof(int)));
			temp = iv.forAssignment((Assignment)MakeAST("z.Type = null"),null);
			Assert("InterpretationVisitor's forAssignment() branch didn't assign a value to to a valid"
				+ " environment variable correctly", 
				((TypeInfo)iv.Env.LookupVariable("z").Value).Type == null);
			Assert("InterpretationVisitor's forAssignment() branch didn't return the right value",
				temp == null);
			//FieldInfo LHS case
			iv.Env.AddVariable("z2",typeof(TestingClass), new TestingClass());
			temp = iv.forAssignment((Assignment)MakeAST("z2.Testing = 1"),null);
			Assert("InterpretationVisitor's forAssignment() branch didn't assign a value to to a valid"
				+ " environment variable correctly", 
				((TestingClass)iv.Env.LookupVariable("z2").Value).Testing == 1);
			Assert("InterpretationVisitor's forAssignment() branch didn't return the right value",
				temp.Equals(1));
		}

		
		//Tests for the Binary branch
		public void TestForBinary() {
			iv = new InterpretationVisitor(new TypeManager());
			object temp;
			
			#region string operations
			//STRING BINARY STUFF

			//null + null case
			temp = iv.forBinary(new Binary(Binary.BiOperator.Addition,NullLit.Instance,NullLit.Instance,null),null);
			Assert("InterpretationVisitor's forBinary() branch return correct value for 'null+null'",
				temp is string && ((string)temp).Equals(""));

			//string + null
			temp = iv.forBinary(new Binary(Binary.BiOperator.Addition,new StringLit("s",null),NullLit.Instance,null),null);
			Assert("InterpretationVisitor's forBinary() branch return correct value for 'string+null'",
				temp is string && ((string)temp).Equals("s"));

			//null + string
			temp = iv.forBinary(new Binary(Binary.BiOperator.Addition,NullLit.Instance,new StringLit("s",null),null),null);
			Assert("InterpretationVisitor's forBinary() branch return correct value for 'null+string'",
				temp is string && ((string)temp).Equals("s"));

			//string + string
			temp = iv.forBinary(new Binary(Binary.BiOperator.Addition,new StringLit("s",null),new StringLit("s",null),null),null);
			Assert("InterpretationVisitor's forBinary() branch return correct value for 'string+string'",
				temp is string && ((string)temp).Equals("ss"));
			#endregion

			#region addition/subtraction of null pointer/non-primitive mixtures
			//TEST FOR ADDITION/SUBTRACTION OF NULL POINTER/NON-PRIMITIVE MIXTURES
			//non-primitive + null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Addition, new New(new CType("ArgumentException"),null,null),NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//null+ non-primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Addition, NullLit.Instance,new New(new CType("ArgumentException"),null,null),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//non-primitive + non-primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Addition, new New(new CType("ArgumentException"),null,null),new New(new CType("ArgumentException"),null,null),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//Primitive + null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Addition, new IntLit(3),NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//null + Primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Addition, NullLit.Instance,new IntLit(3),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//non-primitive - null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Subtraction, new New(new CType("ArgumentException"),null,null),NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//null- non-primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Subtraction, NullLit.Instance,new New(new CType("ArgumentException"),null,null),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//non-primitive - non-primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Subtraction, new New(new CType("ArgumentException"),null,null),new New(new CType("ArgumentException"),null,null),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//Primitive - null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Subtraction, new IntLit(3),NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//null - Primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Subtraction, NullLit.Instance,new IntLit(3),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}

			//null - null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Subtraction, NullLit.Instance,NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with user defined operation correctly");
			} catch (UnsupportedException) {}
			#endregion

			//TEST ADDITION/SUBTRACTION OF DELEGATE TYPES -- NOT DONE
			
			#region equality/inequality operations on non-primitive/null pointer mixtures
			//TEST EQUALITY OF NON-PRIMITIVES/NULL POINTER MIXTURES
			//null == prim
			temp = iv.forBinary(new Binary(Binary.BiOperator.Equality, NullLit.Instance,new IntLit(3),null),null);
			Assert("InterpretationVisitor's forBinary() branch didn't deal with non-primitive/null pointer mixture equality operation correctly",
					temp is bool && (bool)temp == false);
			//prim == null
			temp = iv.forBinary(new Binary(Binary.BiOperator.Equality, new IntLit(3),NullLit.Instance,null),null);
			Assert("InterpretationVisitor's forBinary() branch didn't deal with non-primitive/null pointer mixture equality operation correctly",
				temp is bool && (bool)temp == false);
			//null == null
			temp = iv.forBinary(new Binary(Binary.BiOperator.Equality, NullLit.Instance,NullLit.Instance,null),null);
			Assert("InterpretationVisitor's forBinary() branch didn't deal with non-primitive/null pointer mixture equality operation correctly",
				temp is bool && (bool)temp == true);
			//non-primitive == null -- NOT DONE
			
			//null == non-primitive -- NOT DONE

			//non-primitive == non-primitive -- NOT DONE

			//TEST INEQUALITY OF NON-PRIMITIVES/NULL POINTER MIXTURES
			//null != prim
			temp = iv.forBinary(new Binary(Binary.BiOperator.Inequality, NullLit.Instance,new IntLit(3),null),null);
			Assert("InterpretationVisitor's forBinary() branch didn't deal with non-primitive/null pointer mixture equality operation correctly",
				temp is bool && (bool)temp == true);
			//prim != null
			temp = iv.forBinary(new Binary(Binary.BiOperator.Inequality, new IntLit(3),NullLit.Instance,null),null);
			Assert("InterpretationVisitor's forBinary() branch didn't deal with non-primitive/null pointer mixture equality operation correctly",
				temp is bool && (bool)temp == true);
			//null != null
			temp = iv.forBinary(new Binary(Binary.BiOperator.Inequality, NullLit.Instance,NullLit.Instance,null),null);
			Assert("InterpretationVisitor's forBinary() branch didn't deal with non-primitive/null pointer mixture equality operation correctly",
				temp is bool && (bool)temp == false);
			//non-primitive != null -- NOT DONE
			
			//null != non-primitive -- NOT DONE

			//non-primitive != non-primitive -- NOT DONE

			#endregion

			#region non-valid operations on non-primitive/null pointer mixtures
			//TESTS FOR ANY OTHER BINARY OPERATION ON NON-PRIMITIVE/NULL POINTER MIXTURES
			//(just one invalid operation tested.... more should be tested
			
			//non-primitive * null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Multiply, new New(new CType("ArgumentException"),null,null),NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with invalid binary operation on non-primitive/null-pointer mixture of arguments");
			} catch (InterpreterException) {}

			//null * non-primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Multiply, NullLit.Instance,new New(new CType("ArgumentException"),null,null),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with invalid binary operation on non-primitive/null-pointer mixture of arguments");
			} catch (InterpreterException) {}

			//non-primitive * non-primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Multiply, new New(new CType("ArgumentException"),null,null),new New(new CType("ArgumentException"),null,null),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with invalid binary operation on non-primitive/null-pointer mixture of arguments");
			} catch (InterpreterException) {}

			//Primitive * null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Multiply, new IntLit(3),NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with invalid binary operation on non-primitive/null-pointer mixture of arguments");
			} catch (InterpreterException) {}

			//null * Primitive
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Multiply, NullLit.Instance,new IntLit(3),null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with invalid binary operation on non-primitive/null-pointer mixture of arguments");
			} catch (InterpreterException) {}

			//null * null
			try {
				temp = iv.forBinary(new Binary(Binary.BiOperator.Multiply, NullLit.Instance,NullLit.Instance,null),null);
				Fail("InterpretionVisitor's forBinary() branch didn't deal with invalid binary operation on non-primitive/null-pointer mixture of arguments");
			} catch (InterpreterException) {}
			#endregion
			
			#region all valid operations on primitive inputs
			//TEST ADDITION
			temp = iv.forBinary((Binary)MakeAST("1 + 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't add correctly",
				temp.Equals(2));
			
			//TEST DIVISION
			temp = iv.forBinary((Binary)MakeAST("3 / 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't divide correctly",
				temp.Equals(3));

			//TEST MULTIPLICATION
			temp = iv.forBinary((Binary)MakeAST("5 * 2"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't multiply correctly",
				temp.Equals(10));

			//TEST SUBTRACTION
			temp = iv.forBinary((Binary)MakeAST("1 - 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't subtract correctly",
				temp.Equals(0));

			//TEST BITWISE AND (1)
			temp = iv.forBinary((Binary)MakeAST("3 & 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't bitwise-and correctly",
				temp.Equals(1));

			//TEST BITWISE AND (2)
			temp = iv.forBinary((Binary)MakeAST("0xFFF & 0xF0F"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't bitwise-and correctly",
				temp.Equals(0xF0F));

			//TEST BITWISE OR (1)
			temp = iv.forBinary((Binary)MakeAST("2 | 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't bitwise-or correctly",
				temp.Equals(3));

			//TEST BITWISE OR (2)
			temp = iv.forBinary((Binary)MakeAST("0x0F0 | 0xF0F"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't bitwise-or correctly",
				temp.Equals(0xFFF));

			//TEST EQUALITY bool (1)
			temp = iv.forBinary((Binary)MakeAST("true == true"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(true));

			//TEST EQUALITY bool (2)
			temp = iv.forBinary((Binary)MakeAST("true == false"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(false));

			//TEST EQUALITY float (1)
			temp = iv.forBinary((Binary)MakeAST(".3 == .3"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(true));

			//TEST EQUALITY float (2)
			temp = iv.forBinary((Binary)MakeAST(".3 == .2"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(false));

			//TEST EQUALITY int (1)
			temp = iv.forBinary((Binary)MakeAST("2 == 2"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(true));

			//TEST EQUALITY int (2)
			temp = iv.forBinary((Binary)MakeAST("2 == 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(false));

			//TEST EQUALITY string (1)
			temp = iv.forBinary((Binary)MakeAST("\"a\" == \"a\""), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(true));

			//TEST EQUALITY string (2)
			temp = iv.forBinary((Binary)MakeAST("\"a\" == \"b\""), null);
			Assert("InterpretationVisitor's forBinary() branch didn't equality correctly",
				temp.Equals(false));

			//TEST INEQUALITY bool (1)
			temp = iv.forBinary((Binary)MakeAST("true != true"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(false));

			//TEST INEQUALITY bool (2)
			temp = iv.forBinary((Binary)MakeAST("true != false"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(true));

			//TEST INEQUALITY float (1)
			temp = iv.forBinary((Binary)MakeAST(".3 != .3"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(false));

			//TEST INEQUALITY float (2)
			temp = iv.forBinary((Binary)MakeAST(".3 != .2"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(true));

			//TEST INEQUALITY int (1)
			temp = iv.forBinary((Binary)MakeAST("2 != 2"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(false));

			//TEST INEQUALITY int (2)
			temp = iv.forBinary((Binary)MakeAST("2 != 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(true));

			//TEST INEQUALITY string (1)
			temp = iv.forBinary((Binary)MakeAST("\"a\" != \"a\""), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(false));

			//TEST INEQUALITY string (2)
			temp = iv.forBinary((Binary)MakeAST("\"a\" != \"b\""), null);
			Assert("InterpretationVisitor's forBinary() branch didn't inequality correctly",
				temp.Equals(true));

			//TEST GREATER THAN int (1)
			temp = iv.forBinary((Binary)MakeAST("2 > 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't greater than correctly",
				temp.Equals(true));

			//TEST GREATER THAN int (2)
			temp = iv.forBinary((Binary)MakeAST("1 > 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't greater than correctly",
				temp.Equals(false));

			//TEST GREATER THAN float (1)
			temp = iv.forBinary((Binary)MakeAST("2.0 > 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't greater than correctly",
				temp.Equals(true));

			//TEST GREATER THAN float (2)
			temp = iv.forBinary((Binary)MakeAST("1.0 > 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't greater than correctly",
				temp.Equals(false));

			//TEST LESS THAN int (1)
			temp = iv.forBinary((Binary)MakeAST("0 < 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than correctly",
				temp.Equals(true));

			//TEST LESS THAN int (2)
			temp = iv.forBinary((Binary)MakeAST("1 < 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than correctly",
				temp.Equals(false));

			//TEST LESS THAN float (1)
			temp = iv.forBinary((Binary)MakeAST("0.0 < 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than correctly",
				temp.Equals(true));

			//TEST LESS THAN float (2)
			temp = iv.forBinary((Binary)MakeAST("1.0 < 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than correctly",
				temp.Equals(false));

			//TEST LESS THAN EQUAL int (1)
			temp = iv.forBinary((Binary)MakeAST("1 <= 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than equal to correctly",
				temp.Equals(true));

			//TEST LESS THAN EQUAL int (2)
			temp = iv.forBinary((Binary)MakeAST("0 <= 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than equal to correctly",
				temp.Equals(true));
			
			//TEST LESS THAN EQUAL int (3)
			temp = iv.forBinary((Binary)MakeAST("2 <= 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than equal to correctly",
				temp.Equals(false));

			//TEST LESS THAN EQUAL float (1)
			temp = iv.forBinary((Binary)MakeAST("1.0 <= 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than equal to correctly",
				temp.Equals(true));

			//TEST LESS THAN EQUAL float (2)
			temp = iv.forBinary((Binary)MakeAST("0.0 <= 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than equal to correctly",
				temp.Equals(true));
			
			//TEST LESS THAN EQUAL float (3)
			temp = iv.forBinary((Binary)MakeAST("2.0 <= 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't less than equal to correctly",
				temp.Equals(false));

			//TEST GREATER THAN EQUAL int (1)
			temp = iv.forBinary((Binary)MakeAST("1 >= 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't GREATER than equal to correctly",
				temp.Equals(true));

			//TEST GREATER THAN EQUAL int (2)
			temp = iv.forBinary((Binary)MakeAST("1 >= 0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't GREATER than equal to correctly",
				temp.Equals(true));
			
			//TEST GREATER THAN EQUAL int (3)
			temp = iv.forBinary((Binary)MakeAST("1 >= 2"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't GREATER than equal to correctly",
				temp.Equals(false));

			//TEST GREATER THAN EQUAL float (1)
			temp = iv.forBinary((Binary)MakeAST("1.0 >= 1.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't GREATER than equal to correctly",
				temp.Equals(true));

			//TEST GREATER THAN EQUAL float (2)
			temp = iv.forBinary((Binary)MakeAST("1.0 >= 0.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't GREATER than equal to correctly",
				temp.Equals(true));
			
			//TEST GREATER THAN EQUAL float (3)
			temp = iv.forBinary((Binary)MakeAST("2.0 >= 10.0"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't GREATER than equal to correctly",
				temp.Equals(false));

			//TEST XOR
			temp = iv.forBinary((Binary)MakeAST("7^5"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't exclusive-or correctly",
				temp.Equals(2));

			//TEST MODULUS int
			temp = iv.forBinary((Binary)MakeAST("13%4"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't modulus correctly",
				temp.Equals(1));

			//TEST MODULUS float
			temp = iv.forBinary((Binary)MakeAST("13.0%4"), null);
			System.Console.WriteLine(temp);
			Assert("InterpretationVisitor's forBinary() branch didn't modulus correctly",
				temp.Equals(1.0));

			//TEST LOGICAL AND (1)
			temp = iv.forBinary((Binary)MakeAST("true && true"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-and correctly",
				temp.Equals(true));

			//TEST LOGICAL AND (2)
			temp = iv.forBinary((Binary)MakeAST("true && false"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-and correctly",
				temp.Equals(false));
			
			//TEST LOGICAL AND (3)
			temp = iv.forBinary((Binary)MakeAST("false && true"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-and correctly",
				temp.Equals(false));
			
			//TEST LOGICAL AND (4)
			temp = iv.forBinary((Binary)MakeAST("false && false"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-and correctly",
				temp.Equals(false));

			//TEST LOGICAL OR (1)
			temp = iv.forBinary((Binary)MakeAST("true || true"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-or correctly",
				temp.Equals(true));

			//TEST LOGICAL OR (2)
			temp = iv.forBinary((Binary)MakeAST("false || true"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-or correctly",
				temp.Equals(true));

			//TEST LOGICAL OR (3)
			temp = iv.forBinary((Binary)MakeAST("true || false"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-or correctly",
				temp.Equals(true));

			//TEST LOGICAL OR (4)
			temp = iv.forBinary((Binary)MakeAST("false || false"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't logical-or correctly",
				temp.Equals(false));

			//TEST LEFT SHIFT 
			temp = iv.forBinary((Binary)MakeAST("3 << 1"), null);
			Assert("InterpretationVisitor's forBinary() branch didn't left shift correctly",
				temp.Equals(6));

			//TEST RIGHT SHIFT  
			temp = iv.forBinary((Binary)MakeAST("3 >> 1"), null);
			System.Console.WriteLine(((int)temp));
			Assert("InterpretationVisitor's forBinary() branch didn't right shift correctly",
				temp.Equals(1));
			
			//TEST "OTHER" -?  How?
			#endregion
		}

		
		//Test for the BoolLit branch
		public void TestForBoolLit() {
			object temp = iv.forBoolLit((BoolLit)MakeAST("true"),null);
			Assert("InterpretationVisitor's forBoolLit() branch return correct value",
				temp is bool && ((bool)temp).Equals(true));

			temp = iv.forBoolLit((BoolLit)MakeAST("false"),null);
			Assert("InterpretationVisitor's forBoolLit() branch return correct value",
				temp is bool && ((bool)temp).Equals(false));
		}

		
		//Test for the CharLit branch
		public void TestForCharLit() {
			object temp = iv.forCharLit((CharLit)MakeAST("'c'"),null);
			Assert("InterpretationVisitor's forCharLit() branch return correct value",
				temp is char && ((char)temp).Equals('c'));
		}


		//Tests for the CompoundAssignment branch
		public void TestForCompoundAssignment() {
			iv = new InterpretationVisitor(new TypeManager());
			object temp;

			iv.Env.AddVariable("a", typeof(int), 5);

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a += 5"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't += correctly",
				iv.Env.LookupVariable("a").Value.Equals(10));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a /= 5"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't /= correctly",
				iv.Env.LookupVariable("a").Value.Equals(2));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a -= 1"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't -= correctly",
				iv.Env.LookupVariable("a").Value.Equals(1));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a *= 5"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't *= correctly",
				iv.Env.LookupVariable("a").Value.Equals(5));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a ^= 5"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't ^= correctly",
				iv.Env.LookupVariable("a").Value.Equals(0));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a |= 5"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't |= correctly",
				iv.Env.LookupVariable("a").Value.Equals(5));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a &= 4"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't &= correctly",
				iv.Env.LookupVariable("a").Value.Equals(4));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a %= 3"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't %= correctly",
				iv.Env.LookupVariable("a").Value.Equals(1));
			
			
			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a <<= 1"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't <<= correctly",
				iv.Env.LookupVariable("a").Value.Equals(2));

			temp = iv.forCompoundAssignment((CompoundAssignment)MakeAST("a >>= 1"), null);
			Assert("InterpretationVisitor's forCompoundAssignment() branch didn't >>= correctly",
				iv.Env.LookupVariable("a").Value.Equals(1));
		}

		
		//Test for the ElementAccess branch
		public void TestForElementAccess() {
			iv = new InterpretationVisitor(new TypeManager());
			object temp;

			iv.Env.AddVariable("a", typeof(int[][]), Array.CreateInstance(typeof(int), 5,5));
			iv.forAssignment((Assignment)MakeAST("a[4][4] = 56"), null);
			
			//null access
			try {
				iv.forElementAccess((ElementAccess)MakeAST("(null)[4]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch a null access");
			} catch (InterpreterException) {}

			//indexer too big
			try {
				iv.forElementAccess((ElementAccess)MakeAST("a[4][9000000000000000000000]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch a too-big-a-number access");
			} catch (InterpreterException) {
			} catch (OverflowException) {}

			//indexer not a whole positive integer - 1
			try {
				iv.forElementAccess((ElementAccess)MakeAST("a[4][.5]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch a floating point access");
			} catch (InterpreterException) {}

			//indexer not a whole positive integer - 2
			try {
				iv.forElementAccess((ElementAccess)MakeAST("a[4][-1]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch a negative index access");
			} catch (InterpreterException) {}

			//indexer rank != - 1 
			try {
				iv.forElementAccess((ElementAccess)MakeAST("a[4][4][4]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch an too-many-indeces access");
			} catch (InterpreterException) {}

			//indexer rank != - 2
			try {
				iv.forElementAccess((ElementAccess)MakeAST("a[4]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch an too-few-indeces access");
			} catch (InterpreterException) {}

			//indexer out-of-bounds
			try {
				iv.forElementAccess((ElementAccess)MakeAST("a[4][9]"), null);
				Fail("InterpretationVisitor's forElementAccess() didn't catch an out-of-bounds access");
			} catch (InterpreterException) {}

			temp = iv.forElementAccess((ElementAccess)MakeAST("a[4][4]"), null);
			Assert("InterpretationVisitor's forElementAccess() didn't return the correct value",
				temp.Equals(56));

		
		}
	}
}


