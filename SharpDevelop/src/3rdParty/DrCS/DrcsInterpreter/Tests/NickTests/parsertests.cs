

namespace Rice.Drcsharp.Tests.ParserTests {

	using NUnit.Framework;
	using System.IO;
	using System.Collections;

	using Rice.Drcsharp.Parser;
	using Rice.Drcsharp.Parser.AST;
	using Rice.Drcsharp.Parser.AST.Expressions;
	using Rice.Drcsharp.Parser.AST.Statements;
	using Rice.Drcsharp.Parser.AST.Visitors;
	using Rice.Drcsharp.Interpreter;


	
	//Suite of tests for the parser
	[TestFixture]
	public class SuiteParserTests {
		
		CSharpParser parser;
		ASTNode theAST;

		//Debugging method to write a given object's ToString() to standard output
		public void write(object inp) {
			System.Console.WriteLine(inp);
		}
			
		//public static ITest Suite {
		//	get {
		//		TestSuite suite= new TestSuite("Parser Tests Suite");
		//		suite.AddTest(new SuiteParserTest("TestASTProduction"));
		//		return suite;
		//	}
		//}

		//Test to make sure every AST is produced properly in the parser
		[Test]
		public void TestASTProduction() {
			IntLit exIntLit = new IntLit(5); //example Expression (IntLit)
			BoolLit exBoolLit = BoolLit.TrueInstance; //example Expression (BoolLit)
			ArrayList exAList = new ArrayList(); //example ArrayList of Expression
			ArrayList exAAList = new ArrayList(); //example ArrayList of Argument
			exAList.Add(exIntLit);
			exAAList.Add(new Argument(exIntLit, Argument.ArgType.Expression));

			#region Expressions
			/*
			 * First, check all the Expression AST parses
			 */		

			//ArrayCreation1
			parser = new CSharpParser("parserName", new StringReader("new a[5]"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("ArrayCreation(case 1) not created properly by parser", 
				(bool)new ArrayCreation(new CType("a"), exAList, "", null, null).execute(ASTEqualVisitor.ONLY,theAST));

			//ArrayCreation2
			parser = new CSharpParser("parserName", new StringReader("new a[5][,]"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("ArrayCreation(case 2) not created properly by parser", 
				(bool)new ArrayCreation(new CType("a"), exAList, "[,]", null, null).execute(ASTEqualVisitor.ONLY,theAST));

			//ArrayCreation3
			parser = new CSharpParser("parserName", new StringReader("new a[5] {5, 5, 5, 5, 5}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				temp.Add(exIntLit);

				Assertion.Assert("ArrayCreation(case 3) not created properly by parser", 
					(bool)new ArrayCreation(new CType("a"), exAList, "", temp, null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//ArrayCreation4
			parser = new CSharpParser("parserName", new StringReader("new a[] {5,5,5,5,5}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				temp.Add(exIntLit);
				Assertion.Assert("ArrayCreation(case 4) not created properly by parser", 
					(bool)new ArrayCreation(new CType("a"), "[]", temp, null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//ArrayCreation5
			parser = new CSharpParser("parserName", new StringReader("new a[] {{5,5}}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				ArrayList temp2 = new ArrayList();
				temp2.Add(exIntLit);
				temp2.Add(exIntLit);
				temp.Add(temp2);
				Assertion.Assert("ArrayCreation(case 5) not created properly by parser", 
					(bool)new ArrayCreation(new CType("a"), "[]", temp, null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//As
			parser = new CSharpParser("parserName", new StringReader("5 as garbage"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("As not created properly by parser", 
				(bool)new As(exIntLit, new CType("garbage"), null).execute(ASTEqualVisitor.ONLY,theAST));

			//Assignment
			parser = new CSharpParser("parserName", new StringReader("5 = 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Assignment not created properly by parser", 
				(bool)new Assignment(exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//BaseAccess??			

			//BaseIndexerAccess??

			//Binary-'*'
			parser = new CSharpParser("parserName", new StringReader("5 * 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary(*) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Multiply, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'-'
			parser = new CSharpParser("parserName", new StringReader("5 - 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary(-) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Subtraction, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'+'
			parser = new CSharpParser("parserName", new StringReader("5 + 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (+) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Addition, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'/'
			parser = new CSharpParser("parserName", new StringReader("5 / 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (/) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Division, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'%'
			parser = new CSharpParser("parserName", new StringReader("5 % 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (%) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Modulus, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'<<'
			parser = new CSharpParser("parserName", new StringReader("5 << 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (<<) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.LeftShift, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));
			
			//Binary-'>>'
			parser = new CSharpParser("parserName", new StringReader("5 >> 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (>>) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.RightShift, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));
			
			//Binary-'<'
			parser = new CSharpParser("parserName", new StringReader("5 < 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (<) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.LessThan, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'>'
			parser = new CSharpParser("parserName", new StringReader("5 > 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (>) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.GreaterThan, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'<='
			parser = new CSharpParser("parserName", new StringReader("5 <= 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (<=) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.LessThanOrEqual, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'>='
			parser = new CSharpParser("parserName", new StringReader("5 >= 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (>=) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.GreaterThanOrEqual, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'=='
			parser = new CSharpParser("parserName", new StringReader("5 == 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (==) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Equality, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'!='
			parser = new CSharpParser("parserName", new StringReader("5 != 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (!=) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.Inequality, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'&'
			parser = new CSharpParser("parserName", new StringReader("5 & 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (&) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.BitwiseAnd, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'|'
			parser = new CSharpParser("parserName", new StringReader("5 | 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (|) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.BitwiseOr, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'^'
			parser = new CSharpParser("parserName", new StringReader("5 ^ 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (^) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.ExclusiveOr, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'&&'
			parser = new CSharpParser("parserName", new StringReader("5 && 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (&&) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.LogicalAnd, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Binary-'||'
			parser = new CSharpParser("parserName", new StringReader("5 || 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Binary (||) not created properly by parser", 
				(bool)new Binary(Binary.BiOperator.LogicalOr, exIntLit, exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//BoolLit
			parser = new CSharpParser("parserName", new StringReader("true"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("BoolLit not created properly by parser", 
				(bool)BoolLit.TrueInstance.execute(ASTEqualVisitor.ONLY,theAST));

			//BoxedCast?

			//CharLit
			parser = new CSharpParser("parserName", new StringReader("'f'"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("CharLit not created properly by parser", 
				(bool)new CharLit('f').execute(ASTEqualVisitor.ONLY,theAST));

			//CheckedExpression
			parser = new CSharpParser("parserName", new StringReader("checked(5)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("CheckedExpression not created properly by parser", 
				(bool)new CheckedExpression(exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));
			
			//ClassCast
			parser = new CSharpParser("parserName", new StringReader("(aclass)5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("ClassCast not created properly by parser", 
				(bool)new ClassCast(exIntLit, new SimpleName("aclass", null), null).execute(ASTEqualVisitor.ONLY,theAST));

			//ComposedCast?

			//CompoundAssignment-1
			parser = new CSharpParser("parserName", new StringReader("x += 5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("CompoundAssignment not created properly by parser", 
				(bool)new CompoundAssignment(Binary.BiOperator.Addition, new SimpleName("x", null), exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			
			//Constant -- not necessary to test, it's unimplemented and abstract

			//DecimalLit
			parser = new CSharpParser("parserName", new StringReader("5m"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("DecimalLit not created properly by parser", 
				(bool)new DecimalLit(5m).execute(ASTEqualVisitor.ONLY,theAST));

			//DoubleLit
			parser = new CSharpParser("parserName", new StringReader("5.0"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("DoubleLit not created properly by parser", 
				(bool)new DoubleLit(5.0).execute(ASTEqualVisitor.ONLY,theAST));

			//ElementAccess - 1 
			parser = new CSharpParser("parserName", new StringReader("a[5]"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("ElementAccess not created properly by parser", 
				(bool)new ElementAccess(new SimpleName("a",null), exAList, null).execute(ASTEqualVisitor.ONLY,theAST));

			
			//EmptyCast ??

			//EmptyExpression -- not produced by parser

			//FloatLit
			parser = new CSharpParser("parserName", new StringReader("5f"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("FloatLit not created properly by parser", 
				(bool)new FloatLit(5f).execute(ASTEqualVisitor.ONLY,theAST));

			//IntLit
			parser = new CSharpParser("parserName", new StringReader("5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("IntLit not created properly by parser", 
				(bool)exIntLit.execute(ASTEqualVisitor.ONLY,theAST));

			//Invocation1
			parser = new CSharpParser("parserName", new StringReader("a(5)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Invocation (case 1) not created properly by parser", 
				(bool)new Invocation(new SimpleName("a",null), exAAList, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Invocation2
			parser = new CSharpParser("parserName", new StringReader("a()"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Invocation (case 2) not created properly by parser", 
				(bool)new Invocation(new SimpleName("a",null), null, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Is
			parser = new CSharpParser("parserName", new StringReader("5 is a"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Is not created properly by parser", 
				(bool)new Is(exIntLit, new CType("a"), null).execute(ASTEqualVisitor.ONLY,theAST));
			
			//LongLit
			parser = new CSharpParser("parserName", new StringReader("5L"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("LongLit not created properly by parser", 
				(bool)new LongLit(5L).execute(ASTEqualVisitor.ONLY,theAST));

			//MemberAccess
			parser = new CSharpParser("parserName", new StringReader("a.b"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("MemberAccess not created properly by parser", 
				(bool)new MemberAccess(new SimpleName("a", null), "b", null).execute(ASTEqualVisitor.ONLY,theAST));

			//New1
			parser = new CSharpParser("parserName", new StringReader("new a(5)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("New (case 1) not created properly by parser", 
				(bool)new New(new CType("a"), exAAList,null).execute(ASTEqualVisitor.ONLY,theAST));

			//New2
			parser = new CSharpParser("parserName", new StringReader("new a()"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("New (case 2) not created properly by parser", 
				(bool)new New(new CType("a"), null,null).execute(ASTEqualVisitor.ONLY,theAST));

			//NullLit
			parser = new CSharpParser("parserName", new StringReader("null"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("NullLit not created properly by parser", 
				(bool)NullLit.Instance.execute(ASTEqualVisitor.ONLY,theAST));

			//ParenExpr
			parser = new CSharpParser("parserName", new StringReader("(5)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("ParenExpr not created properly by parser", 
				(bool)new ParenExpr(exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//SimpleName
			parser = new CSharpParser("parserName", new StringReader("a"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("SimpleName not created properly by parser", 
				(bool)new SimpleName("a", null).execute(ASTEqualVisitor.ONLY,theAST));

			//SizeOf?? -- gets a parse error
			/*parser = new CSharpParser("parserName", new StringReader("sizeof(a)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("SizeOf not created properly by parser", 
				(bool)new SizeOf(new CType("a"), null).execute(ASTEqualVisitor.ONLY,theAST));
			*/

			//StringLit1
			parser = new CSharpParser("parserName", new StringReader("\"a\""));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("StringLit (case 1) not created properly by parser", 
				(bool)new StringLit("a", null).execute(ASTEqualVisitor.ONLY,theAST));

			//StringLit2
			parser = new CSharpParser("parserName", new StringReader("\"\""));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("StringLit (case 2) not created properly by parser", 
				(bool)new StringLit("", null).execute(ASTEqualVisitor.ONLY,theAST));

			//This??
			/*parser = new CSharpParser("parserName", new StringReader("this"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("This not created properly by parser", 
				(bool)new This(null).execute(ASTEqualVisitor.ONLY,theAST));
			*/
			//TypeOf??
			/*parser = new CSharpParser("parserName", new StringReader("typeof(a)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("TypeOf not created properly by parser", 
				(bool)new TypeOf(new CType("a"), null).execute(ASTEqualVisitor.ONLY,theAST));
			*/
			
			//UIntLit
			parser = new CSharpParser("parserName", new StringReader("5U"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UIntLit not created properly by parser", 
				(bool)new UIntLit(5U).execute(ASTEqualVisitor.ONLY,theAST));

			//ULongLit
			parser = new CSharpParser("parserName", new StringReader("5UL"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("ULongLit not created properly by parser", 
				(bool)new ULongLit(5UL).execute(ASTEqualVisitor.ONLY,theAST));

			//Unary-'+'
			parser = new CSharpParser("parserName", new StringReader("+5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unary (+) not created properly by parser", 
				(bool)new Unary(Unary.UnaryOperator.UnaryPlus,exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Unary-'-'
			parser = new CSharpParser("parserName", new StringReader("-5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unary (-) not created properly by parser", 
				(bool)new Unary(Unary.UnaryOperator.UnaryNegation,exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Unary-'!'
			parser = new CSharpParser("parserName", new StringReader("!5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unary (!) not created properly by parser", 
				(bool)new Unary(Unary.UnaryOperator.LogicalNot,exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Unary-'~'
			parser = new CSharpParser("parserName", new StringReader("~5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unary (~) not created properly by parser", 
				(bool)new Unary(Unary.UnaryOperator.OnesComplement,exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Unary-'*' (pointer indirection)
			parser = new CSharpParser("parserName", new StringReader("*a"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unary ('*'--pointer indirection) not created properly by parser", 
				(bool)new Unary(Unary.UnaryOperator.Indirection,new SimpleName("a", null), null).execute(ASTEqualVisitor.ONLY,theAST));

			//Unary-'&' (address-of)
			parser = new CSharpParser("parserName", new StringReader("&a"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unary ('&'-- address-of) not created properly by parser", 
				(bool)new Unary(Unary.UnaryOperator.AddressOf,new SimpleName("a", null), null).execute(ASTEqualVisitor.ONLY,theAST));


			//UnaryMutator--'++x'
			parser = new CSharpParser("parserName", new StringReader("++5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UnaryMutator (++x) not created properly by parser", 
				(bool)new UnaryMutator(UnaryMutator.UMode.PreIncrement,exIntLit,null).execute(ASTEqualVisitor.ONLY,theAST));

			//UnaryMutator--'--x'
			parser = new CSharpParser("parserName", new StringReader("--5"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UnaryMutator (--x) not created properly by parser", 
				(bool)new UnaryMutator(UnaryMutator.UMode.PreDecrement,exIntLit,null).execute(ASTEqualVisitor.ONLY,theAST));

			//UnaryMutator--'x++'
			parser = new CSharpParser("parserName", new StringReader("5++"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UnaryMutator (x++) not created properly by parser", 
				(bool)new UnaryMutator(UnaryMutator.UMode.PostIncrement,exIntLit,null).execute(ASTEqualVisitor.ONLY,theAST));

			//UnaryMutator--'x--'
			parser = new CSharpParser("parserName", new StringReader("5--"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UnaryMutator (x--) not created properly by parser", 
				(bool)new UnaryMutator(UnaryMutator.UMode.PostDecrement,exIntLit,null).execute(ASTEqualVisitor.ONLY,theAST));

			//UnboxCast??

			//UncheckedExpression
			parser = new CSharpParser("parserName", new StringReader("unchecked(5)"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UncheckedExpression not created properly by parser", 
				(bool)new UncheckedExpression(exIntLit, null).execute(ASTEqualVisitor.ONLY,theAST));

			//UIntLit
			parser = new CSharpParser("parserName", new StringReader("5U"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UIntLit not created properly by parser", 
				(bool)new UIntLit(5U).execute(ASTEqualVisitor.ONLY,theAST));
			#endregion	

			#region Statements
			/*
			 * Now to check all the Statement AST parses
			 */
			
			ArrayList exSList = new ArrayList();
			exSList.Add(EmptyStatement.Instance);
			Block exBlock = new Block(exSList, null);

			//Block1
			parser = new CSharpParser("parserName", new StringReader("{;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Block (case 1) not created properly by parser", 
				(bool)new Block(exSList, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Block2
			parser = new CSharpParser("parserName", new StringReader("{}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Block (case 2) not created properly by parser", 
				(bool)new Block(null, null).execute(ASTEqualVisitor.ONLY,theAST));


			//Break
			parser = new CSharpParser("parserName", new StringReader("break;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Break not created properly by parser", 
				(bool)new Break(null).execute(ASTEqualVisitor.ONLY,theAST));

			//CheckedStatement
			parser = new CSharpParser("parserName", new StringReader("checked {;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("CheckedStatement not created properly by parser", 
				(bool)new CheckedStatement(exBlock, null).execute(ASTEqualVisitor.ONLY,theAST));

			//Continue
			parser = new CSharpParser("parserName", new StringReader("continue;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Continue not created properly by parser", 
				(bool)new Continue(null).execute(ASTEqualVisitor.ONLY,theAST));

			//Do
			parser = new CSharpParser("parserName", new StringReader("do {;} while (true);"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Do not created properly by parser", 
				(bool)new Do(exBlock,exBoolLit,null).execute(ASTEqualVisitor.ONLY,theAST));

			//EmptyStatement
			parser = new CSharpParser("parserName", new StringReader(";"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("EmptyStatement not created properly by parser", 
				(bool)EmptyStatement.Instance.execute(ASTEqualVisitor.ONLY,theAST));

			//ExpressionStatement
			parser = new CSharpParser("parserName", new StringReader("a = 5;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("EmptyStatement not created properly by parser", 
				(bool)new ExpressionStatement(new Assignment(new SimpleName("a", null),exIntLit,null),null).execute(ASTEqualVisitor.ONLY,theAST));


			//For1
			parser = new CSharpParser("parserName", new StringReader("for(;;) ;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("For(case 1) not created properly by parser", 
				(bool)new For(null,null,null,EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));
			

			//For2
			parser = new CSharpParser("parserName", new StringReader("for(a=5;;) ;"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new Assignment(new SimpleName("a", null),exIntLit,null));
				Assertion.Assert("For(case 2) not created properly by parser", 
					(bool)new For(temp,null,null,EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//For3
			parser = new CSharpParser("parserName", new StringReader("for(a=5,b=5;;) ;"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new Assignment(new SimpleName("a", null),exIntLit,null));
				temp.Add(new Assignment(new SimpleName("b", null),exIntLit,null));
				Assertion.Assert("For(case 3) not created properly by parser", 
					(bool)new For(temp,null,null,EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//For4
			parser = new CSharpParser("parserName", new StringReader("for(;true;) ;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("For(case 4) not created properly by parser", 
				(bool)new For(null,exBoolLit,null,EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));


			//For5
			parser = new CSharpParser("parserName", new StringReader("for(;;a=5,b=5) ;"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new Assignment(new SimpleName("a", null),exIntLit,null));
				temp.Add(new Assignment(new SimpleName("b", null),exIntLit,null));
				Assertion.Assert("For(case 5) not created properly by parser", 
					(bool)new For(null,null,temp,EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//For6
			parser = new CSharpParser("parserName", new StringReader("for(int a=5;;) {;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new VariableDeclaration("a",exIntLit,null));
				Assertion.Assert("For(case 6) not created properly by parser", 
					(bool)new For(new LocalVarDecl(new CType("System.Int32"),temp,null),null,null,exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//For7
			parser = new CSharpParser("parserName", new StringReader("for(int[] a= {5};;) {;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				ArrayList temp2 = new ArrayList();
				temp2.Add(exIntLit);
				temp.Add(new VariableDeclaration("a",temp2,null));
				Assertion.Assert("For(case 7) not created properly by parser", 
					(bool)new For(new LocalVarDecl(new CType("System.Int32[]"),temp,null),null,null,exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Foreach
			parser = new CSharpParser("parserName", new StringReader("foreach(int a in a) {;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Foreach not created properly by parser", 
				(bool)new Foreach(new CType("System.Int32"),"a", new SimpleName("a", null),exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));

			//Goto
			parser = new CSharpParser("parserName", new StringReader("goto a;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Goto not created properly by parser", 
				(bool)new Goto("a",null).execute(ASTEqualVisitor.ONLY,theAST));

			//If-1
			parser = new CSharpParser("parserName", new StringReader("if (5) ;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("If(case 1) not created properly by parser", 
				(bool)new If(exIntLit,EmptyStatement.Instance,null,null).execute(ASTEqualVisitor.ONLY,theAST));

			//If-2
			parser = new CSharpParser("parserName", new StringReader("if (5) ; else ;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("If(case 2) not created properly by parser", 
				(bool)new If(exIntLit,EmptyStatement.Instance,EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));


			//LabeledStatement
			parser = new CSharpParser("parserName", new StringReader("a:;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("LabeledStatement not created properly by parser", 
				(bool)new LabeledStatement("a", EmptyStatement.Instance,null).execute(ASTEqualVisitor.ONLY,theAST));

			//LocalConstDecl
			parser = new CSharpParser("parserName", new StringReader("const int a = 5;"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new VariableDeclaration("a",exIntLit,null));
				Assertion.Assert("LocalConstDecl not created properly by parser", 
					(bool)new LocalConstDecl(new CType("System.Int32"),temp,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//LocalVarDecl
			parser = new CSharpParser("parserName", new StringReader("int a =5;"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new VariableDeclaration("a",exIntLit,null));
				Assertion.Assert("LocalConstDecl not created properly by parser", 
					(bool)new LocalVarDecl(new CType("System.Int32"),temp,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Lock
			parser = new CSharpParser("parserName", new StringReader("lock(a) {;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Lock not created properly by parser", 
				(bool)new Lock(new SimpleName("a",null),exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));

			//Return1
			parser = new CSharpParser("parserName", new StringReader("return;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Return (case 1) not created properly by parser", 
				(bool)new Return(null,null).execute(ASTEqualVisitor.ONLY,theAST));

			//Return2
			parser = new CSharpParser("parserName", new StringReader("return 5;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Return (case 2) not created properly by parser", 
				(bool)new Return(exIntLit,null).execute(ASTEqualVisitor.ONLY,theAST));


			//Switch1 -- right now gives a parse error... possible needs to be fixed.
			//parser = new CSharpParser("parserName", new StringReader("switch(a) {}"));
			//parser.parse();
			//theAST = parser.retVal;
			//Assertion.Assert("Switch (case 1) not created properly by parser", 
			//	(bool)new Switch(new SimpleName("a",null),null,null).execute(ASTEqualVisitor.ONLY,theAST));

			//Switch2
			parser = new CSharpParser("parserName", new StringReader("switch(a) {case 1:;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList tempSections = new ArrayList();
				ArrayList tempLabels = new ArrayList();
				ArrayList tempStatements = new ArrayList();
				tempLabels.Add(new SwitchLabel(exIntLit,null));
				tempStatements.Add(EmptyStatement.Instance);
				tempSections.Add(new SwitchSection(tempLabels,tempStatements,null));
				Assertion.Assert("Switch (case 2) not created properly by parser", 
					(bool)new Switch(new SimpleName("a",null),tempSections,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Switch3
			parser = new CSharpParser("parserName", new StringReader("switch(a) {case 1:case 2:;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList tempSections = new ArrayList();
				ArrayList tempLabels = new ArrayList();
				ArrayList tempStatements = new ArrayList();
				tempLabels.Add(new SwitchLabel(exIntLit,null));
				tempLabels.Add(new SwitchLabel(new IntLit(2),null));
				tempStatements.Add(EmptyStatement.Instance);
				tempSections.Add(new SwitchSection(tempLabels,tempStatements,null));
				Assertion.Assert("Switch (case 3) not created properly by parser", 
					(bool)new Switch(new SimpleName("a",null),tempSections,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Switch4
			parser = new CSharpParser("parserName", new StringReader("switch(a) {default:;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList tempSections = new ArrayList();
				ArrayList tempStatements = new ArrayList();
				tempStatements.Add(EmptyStatement.Instance);
				tempSections.Add(new SwitchSection(null,tempStatements,null));
				Assertion.Assert("Switch (case 4) not created properly by parser", 
					(bool)new Switch(new SimpleName("a",null),tempSections,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Throw
			parser = new CSharpParser("parserName", new StringReader("throw a;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Throw not created properly by parser", 
				(bool)new Throw(new SimpleName("a",null),null).execute(ASTEqualVisitor.ONLY,theAST));

			//Try1 -- parser gets null-object-reference error with this input
			//parser = new CSharpParser("parserName", new StringReader("try {;} catch {;}"));
			//parser.parse();
			//theAST = parser.retVal;
			//Assertion.Assert("Try (case 1) not created properly by parser", 
			//	(bool)new Try(exBlock,null,new Catch(null,exBlock,null),null,null).execute(ASTEqualVisitor.ONLY,theAST));

			//Try2
			parser = new CSharpParser("parserName", new StringReader("try {;} catch (a) {;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new Catch(new CatchArgs(new CType("a"),null,null),exBlock,null));
				Assertion.Assert("Try (case 2) not created properly by parser", 
					(bool)new Try(exBlock,temp,null,null,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Try3
			parser = new CSharpParser("parserName", new StringReader("try {;} catch (a a) {;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new Catch(new CatchArgs(new CType("a"),"a",null),exBlock,null));
				Assertion.Assert("Try (case 3) not created properly by parser", 
					(bool)new Try(exBlock,temp,null,null,null).execute(ASTEqualVisitor.ONLY,theAST));
			}

			//Try4
			parser = new CSharpParser("parserName", new StringReader("try {;} catch (a) {;} finally {;}"));
			parser.parse();
			theAST = parser.retVal;
			if (true) {
				ArrayList temp = new ArrayList();
				temp.Add(new Catch(new CatchArgs(new CType("a"),null,null),exBlock,null));
				Assertion.Assert("Try (case 4) not created properly by parser", 
					(bool)new Try(exBlock,temp,null,exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));
			}
			
			//UncheckedStatement
			parser = new CSharpParser("parserName", new StringReader("unchecked {;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UncheckedStatement not created properly by parser", 
				(bool)new UncheckedStatement(exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));

			//Unsafe
			parser = new CSharpParser("parserName", new StringReader("unsafe {;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("Unsafe not created properly by parser", 
				(bool)new Unsafe(exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));

			//UsingAlias
			parser = new CSharpParser("parserName", new StringReader("using a=a;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UsingAlias not created properly by parser", 
				(bool)new UsingAlias("a","a",null).execute(ASTEqualVisitor.ONLY,theAST));

			//UsingNamespace
			parser = new CSharpParser("parserName", new StringReader("using a;"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("UsingNamespace not created properly by parser", 
				(bool)new UsingNamespace("a",null).execute(ASTEqualVisitor.ONLY,theAST));

			//UsingStatement -- parser doesn't parse the statement at all, has 'null' value no matter what
			//parser = new CSharpParser("parserName", new StringReader("using (a) {;}"));
			//parser.parse();
			//theAST = parser.retVal;
			//Assertion.Assert("UsingStatement not created properly by parser", 
			//	(bool)new UsingStatement(new SimpleName("a", null),exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));

			//While
			parser = new CSharpParser("parserName", new StringReader("while (a) {;}"));
			parser.parse();
			theAST = parser.retVal;
			Assertion.Assert("While not created properly by parser", 
				(bool)new While(new SimpleName("a",null),exBlock,null).execute(ASTEqualVisitor.ONLY,theAST));

			#endregion
		}
		
	}
}


