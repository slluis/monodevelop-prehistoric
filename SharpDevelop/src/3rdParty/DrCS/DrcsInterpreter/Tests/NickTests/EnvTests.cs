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


	
	//Suite of tests for the Env class
	[TestFixture]
	public class SuiteEnvTests {
		
		Env env;
		
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
			

		//Tests for AddVariable() methods
		[Test]
		public void TestAddVariable() {
			Type varType = typeof(int);
			env = new Env(null);
			
			Hashtable temp;
			temp = (Hashtable)typeof(Env).GetField("currentScope",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			
			//testing AddVariable(string,Type)
			env.AddVariable("x",varType);
			Assert("AddVariable(string,Type) didn't add a variable correctly",
				((VariableInfo)temp["x"]).Name.Equals("x"));
			Assert("AddVariable(string,Type) didn't add a variable correctly",
				((VariableInfo)temp["x"]).Type == varType);

			try {
				env.AddVariable("x",varType);
				Fail("AddVariable(string,Type) didn't fail when trying to add a duplicate variable");
			} catch (AlreadyDeclaredVariableException) {
			}				
					
			//testing AddVariable(string,Type,object)
			env.AddVariable("y",varType,1);
			Assert("AddVariable(string,Type,object) didn't add a variable correctly",
				((VariableInfo)temp["y"]).Name.Equals("y"));
			Assert("AddVariable(string,Type,object) didn't add a variable correctly",
				((VariableInfo)temp["y"]).Type == varType);
			Assert("AddVariable(string,Type,object) didn't add a variable correctly",
				((VariableInfo)temp["y"]).Value.Equals(1));

			try {
				env.AddVariable("y",varType,1);
				Fail("AddVariable(string,Type,object) didn't fail when trying to add a duplicate variable");
			} catch (AlreadyDeclaredVariableException) {
			}	

			//testing AddVariable(VariableInfo)
			env.AddVariable(new VariableInfo("z",varType,2));
			Assert("AddVariable(string,Type,object) didn't add a variable correctly",
				((VariableInfo)temp["z"]).Name.Equals("z"));
			Assert("AddVariable(string,Type,object) didn't add a variable correctly",
				((VariableInfo)temp["z"]).Type == varType);
			Assert("AddVariable(string,Type,object) didn't add a variable correctly",
				((VariableInfo)temp["z"]).Value.Equals(2));

			try {
				env.AddVariable("z",varType,2);
				Fail("AddVariable(VariableInfo) didn't fail when trying to add a duplicate variable");
			} catch (AlreadyDeclaredVariableException) {
			}
		}

		//Tests for LookupVariable(string) method
		[Test]
		public void TestLookupVariable() {
			env = new Env(null);
			VariableInfo v1 = new VariableInfo("y", typeof(int), null);
			VariableInfo v2 = new VariableInfo("x", typeof(int), null);
			
			//Set up the current scope's variables and the environment stack
			Hashtable currentScope = (Hashtable)typeof(Env).GetField("currentScope",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			Stack envStack = (Stack)typeof(Env).GetField("envStack",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			
			currentScope["x"] = v2;
			envStack.Push(new Hashtable());
			((Hashtable)envStack.Peek())["y"] = v1;

			//Tests
			Assert("LookupVariable(string) didn't find a variable correctly in the current scope",
				env.LookupVariable("x") == v2);
			Assert("LookupVariable(string) didn't find a variable correctly in an outer scope",
				env.LookupVariable("y") == v1);
			try {
				env.LookupVariable("z");
				Fail("LookupVariable(string) didn't report an error when variable not present");
			} catch (UndeclaredVariableException) {
			}
		}

		//Tests for AssignVariable(string,object) method
		[Test]
		public void TestAssignVariable() {
			env = new Env(null);
			VariableInfo v1 = new VariableInfo("y", typeof(int), null);
			Hashtable currentScope = (Hashtable)typeof(Env).GetField("currentScope",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			currentScope["y"] = v1;
				
			//Test
			env.AssignVariable("y",1);
			Assert("AssignVariable(string,object) didn't assign a variable's value correctly",
				((VariableInfo)currentScope["y"]).Value.Equals(1));
		}

		//Tests for NewScope() method
		[Test]
		public void TestNewScope() {
			env = new Env(null);
			Stack envStack = (Stack)typeof(Env).GetField("envStack",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			Hashtable currentScope = (Hashtable)typeof(Env).GetField("currentScope",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			int tempSize = envStack.Count;

			//Test
			env.NewScope();
			Assert("NewScope() didn't put the current scope onto the environment stack correctly",
				envStack.Peek() == currentScope);
			Assert("NewScope() didn't expand the environment stack correctly",
				envStack.Count == tempSize + 1);
			Assert("NewScope() didn't create a new scope correctly",
				((Hashtable)typeof(Env).GetField("currentScope",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env)).Count == 0);
		}

		//Tests for LeaveScope() method
		[Test]
		public void TestLeaveScope() {
			env = new Env(null);
			Stack envStack = (Stack)typeof(Env).GetField("envStack",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);
			Hashtable top = new Hashtable();
			envStack.Push(top);
			int tempSize = envStack.Count;

			//Test
			env.LeaveScope();
			Assert("LeaveScope() didn't reduce the size of the environment stack correctly",
				envStack.Count == tempSize - 1);
			Assert("LeaveScope() didn't set the current scope correctly",
				((Hashtable)typeof(Env).GetField("currentScope",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env)) == top);
			try {
				envStack.Clear();
				env.LeaveScope();
				Fail("LeaveScope() didn't report an error when no outer scope existed");
			} catch (InterpreterException) {
			}
		}

		//Tests for AddLabeledStatement/GetLabeledStatement/GetUsingAlias ommitted
		//as they are just accessor methods.

		//Test for AddUsingAlias(string,string) method
		[Test]
		public void TestAddUsingAlias() {
			//4 cases:  1) A valid type, 2) a valid namespace, 3) an already aliased type/namespace, 
			//          4) an invalid type/namespace

			env = new Env(new TypeManager());
			Hashtable usingAlias = (Hashtable)typeof(Env).GetField("usingAlias",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(env);


			//Case 1
			env.AddUsingAlias("alias1","System.Int32");
			Assert("AddUsingAlias(string,string) didn't deal with a valid type name correctly",
				usingAlias["alias1"] != null && ((TypeInfo)usingAlias["alias1"]).Value == typeof(int));
			
			//Case 2
			env.AddUsingAlias("alias2","System.Reflection");
			Assert("AddUsingAlias(string,string) didn't deal with a valid namespace correctly",
				usingAlias["alias2"] != null && ((String)((NamespaceInfo)usingAlias["alias2"]).Value).Equals("System.Collections"));

			//Case 3
			try {
				env.AddUsingAlias("alias2",null);
				Fail("AddUsingAlias(string,string) didn't report an error when attempting to add an "
					+ "already existing alias");
			} catch (InterpreterException) {
			}

			//Case 4
			try {
				env.AddUsingAlias("bad_alias","System2");
				Fail("AddUsingAlias(string,string) didn't report an error when given a bad namespace");
			} catch (MissingTypeOrNamespaceException) {
			}

		}	
	}
}
