// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "cs-parser.jay"
using System.Text;
using System;

namespace Rice.Drcsharp.Parser
{
	using System.Collections;
	using Rice.Drcsharp.Parser.AST;
	using Rice.Drcsharp.Parser.AST.Expressions;
	using Rice.Drcsharp.Parser.AST.Statements;
	using Rice.Drcsharp.Parser.AST.Visitors;

	/// <summary>
	///    The C# Parser
	/// </summary>
	public class CSharpParser {

		Stack oob_stack;
		string name;
		System.IO.TextReader input;
		bool yacc_verbose_flag = false;
		public ASTNode retVal;
		public bool retUsingDirective = false;
		public bool retStatement = false;
		public bool retExpression = false;
#line 29 "-"

  /** simplified error message.
      @see <a href="#yyerror(java.lang.String, java.lang.String[])">yyerror</a>
    */
  public void yyerror (string message) {
    yyerror(message, null);
  }

  /** (syntax) error message.
      Can be overwritten to control message format.
      @param message text to be displayed.
      @param expected vector of acceptable tokens, if available.
    */
  public void yyerror (string message, string[] expected) {
    if ((expected != null) && (expected.Length  > 0)) {
      DB.parsep(message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        DB.parsep (" "+expected[n]);
        
    } else
      DB.parsep (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
  protected yydebug.yyDebug debug;

  protected static  int yyFinal = 58;
  public static  string [] yyRule = {
    "$accept : compilation_unit",
    "compilation_unit : opt_using_directive opt_EOF",
    "compilation_unit : opt_statement opt_EOF",
    "compilation_unit : expression opt_EOF",
    "opt_statement :",
    "opt_statement : statement",
    "opt_EOF :",
    "opt_EOF : EOF",
    "opt_using_directive :",
    "opt_using_directive : using_directive",
    "using_directive : using_alias_directive",
    "using_directive : using_namespace_directive",
    "using_alias_directive : USING IDENTIFIER ASSIGN namespace_or_type_name SEMICOLON",
    "using_namespace_directive : USING namespace_name SEMICOLON",
    "opt_semicolon :",
    "opt_semicolon : SEMICOLON",
    "opt_comma :",
    "opt_comma : COMMA",
    "type_name : namespace_or_type_name",
    "namespace_name : namespace_or_type_name",
    "namespace_or_type_name : IDENTIFIER",
    "namespace_or_type_name : namespace_or_type_name DOT IDENTIFIER",
    "type : type_name",
    "type : builtin_types",
    "type : array_type",
    "builtin_types : OBJECT",
    "builtin_types : STRING",
    "builtin_types : BOOL",
    "builtin_types : DECIMAL",
    "builtin_types : FLOAT",
    "builtin_types : DOUBLE",
    "builtin_types : VOID",
    "builtin_types : integral_type",
    "integral_type : SBYTE",
    "integral_type : BYTE",
    "integral_type : SHORT",
    "integral_type : USHORT",
    "integral_type : INT",
    "integral_type : UINT",
    "integral_type : LONG",
    "integral_type : ULONG",
    "integral_type : CHAR",
    "array_type : type rank_specifiers",
    "primary_expression : literal",
    "primary_expression : IDENTIFIER",
    "primary_expression : parenthesized_expression",
    "primary_expression : member_access",
    "primary_expression : invocation_expression",
    "primary_expression : element_access",
    "primary_expression : post_increment_expression",
    "primary_expression : post_decrement_expression",
    "primary_expression : new_expression",
    "primary_expression : checked_expression",
    "primary_expression : unchecked_expression",
    "primary_expression : pointer_member_access",
    "literal : boolean_literal",
    "literal : integer_literal",
    "literal : real_literal",
    "literal : LITERAL_CHARACTER",
    "literal : LITERAL_STRING",
    "literal : NULL",
    "boolean_literal : TRUE",
    "boolean_literal : FALSE",
    "integer_literal : LITERAL_INTEGER",
    "real_literal : LITERAL_FLOAT",
    "real_literal : LITERAL_DOUBLE",
    "real_literal : LITERAL_DECIMAL",
    "parenthesized_expression : OPEN_PARENS expression CLOSE_PARENS",
    "member_access : primary_expression DOT IDENTIFIER",
    "member_access : predefined_type DOT IDENTIFIER",
    "predefined_type : builtin_types",
    "invocation_expression : primary_expression OPEN_PARENS opt_argument_list CLOSE_PARENS",
    "opt_argument_list :",
    "opt_argument_list : argument_list",
    "argument_list : argument",
    "argument_list : argument_list COMMA argument",
    "argument : expression",
    "argument : REF variable_reference",
    "argument : OUT variable_reference",
    "variable_reference : expression",
    "element_access : primary_expression OPEN_BRACKET expression_list CLOSE_BRACKET",
    "expression_list : expression",
    "expression_list : expression_list COMMA expression",
    "this_access : THIS",
    "base_access : BASE DOT IDENTIFIER",
    "base_access : BASE OPEN_BRACKET expression_list CLOSE_BRACKET",
    "post_increment_expression : primary_expression OP_INC",
    "post_decrement_expression : primary_expression OP_DEC",
    "new_expression : object_or_delegate_creation_expression",
    "new_expression : array_creation_expression",
    "object_or_delegate_creation_expression : NEW type OPEN_PARENS opt_argument_list CLOSE_PARENS",
    "array_creation_expression : NEW type OPEN_BRACKET expression_list CLOSE_BRACKET opt_rank_specifier opt_array_initializer",
    "array_creation_expression : NEW type rank_specifiers array_initializer",
    "array_creation_expression : NEW type error",
    "opt_rank_specifier :",
    "opt_rank_specifier : rank_specifiers",
    "rank_specifiers : rank_specifier",
    "rank_specifiers : rank_specifiers rank_specifier",
    "rank_specifier : OPEN_BRACKET opt_dim_separators CLOSE_BRACKET",
    "opt_dim_separators :",
    "opt_dim_separators : dim_separators",
    "dim_separators : COMMA",
    "dim_separators : dim_separators COMMA",
    "opt_array_initializer :",
    "opt_array_initializer : array_initializer",
    "array_initializer : OPEN_BRACE CLOSE_BRACE",
    "array_initializer : OPEN_BRACE variable_initializer_list opt_comma CLOSE_BRACE",
    "variable_initializer_list : variable_initializer",
    "variable_initializer_list : variable_initializer_list COMMA variable_initializer",
    "typeof_expression : TYPEOF OPEN_PARENS type CLOSE_PARENS",
    "sizeof_expression : SIZEOF OPEN_PARENS type CLOSE_PARENS",
    "checked_expression : CHECKED OPEN_PARENS expression CLOSE_PARENS",
    "unchecked_expression : UNCHECKED OPEN_PARENS expression CLOSE_PARENS",
    "pointer_member_access : primary_expression OP_PTR IDENTIFIER",
    "non_expression_type : builtin_types",
    "non_expression_type : non_expression_type rank_specifier",
    "non_expression_type : expression rank_specifiers",
    "unary_expression : primary_expression",
    "unary_expression : BANG prefixed_unary_expression",
    "unary_expression : TILDE prefixed_unary_expression",
    "unary_expression : OPEN_PARENS expression CLOSE_PARENS unary_expression",
    "unary_expression : OPEN_PARENS non_expression_type CLOSE_PARENS prefixed_unary_expression",
    "prefixed_unary_expression : unary_expression",
    "prefixed_unary_expression : PLUS prefixed_unary_expression",
    "prefixed_unary_expression : MINUS prefixed_unary_expression",
    "prefixed_unary_expression : OP_INC prefixed_unary_expression",
    "prefixed_unary_expression : OP_DEC prefixed_unary_expression",
    "prefixed_unary_expression : STAR prefixed_unary_expression",
    "prefixed_unary_expression : BITWISE_AND prefixed_unary_expression",
    "pre_increment_expression : OP_INC prefixed_unary_expression",
    "pre_decrement_expression : OP_DEC prefixed_unary_expression",
    "multiplicative_expression : prefixed_unary_expression",
    "multiplicative_expression : multiplicative_expression STAR prefixed_unary_expression",
    "multiplicative_expression : multiplicative_expression DIV prefixed_unary_expression",
    "multiplicative_expression : multiplicative_expression PERCENT prefixed_unary_expression",
    "additive_expression : multiplicative_expression",
    "additive_expression : additive_expression PLUS multiplicative_expression",
    "additive_expression : additive_expression MINUS multiplicative_expression",
    "shift_expression : additive_expression",
    "shift_expression : shift_expression OP_SHIFT_LEFT additive_expression",
    "shift_expression : shift_expression OP_SHIFT_RIGHT additive_expression",
    "relational_expression : shift_expression",
    "relational_expression : relational_expression OP_LT shift_expression",
    "relational_expression : relational_expression OP_GT shift_expression",
    "relational_expression : relational_expression OP_LE shift_expression",
    "relational_expression : relational_expression OP_GE shift_expression",
    "relational_expression : relational_expression IS type",
    "relational_expression : relational_expression AS type",
    "equality_expression : relational_expression",
    "equality_expression : equality_expression OP_EQ relational_expression",
    "equality_expression : equality_expression OP_NE relational_expression",
    "and_expression : equality_expression",
    "and_expression : and_expression BITWISE_AND equality_expression",
    "exclusive_or_expression : and_expression",
    "exclusive_or_expression : exclusive_or_expression CARRET and_expression",
    "inclusive_or_expression : exclusive_or_expression",
    "inclusive_or_expression : inclusive_or_expression BITWISE_OR exclusive_or_expression",
    "conditional_and_expression : inclusive_or_expression",
    "conditional_and_expression : conditional_and_expression OP_AND inclusive_or_expression",
    "conditional_or_expression : conditional_and_expression",
    "conditional_or_expression : conditional_or_expression OP_OR conditional_and_expression",
    "conditional_expression : conditional_or_expression",
    "conditional_expression : conditional_or_expression INTERR expression COLON expression",
    "assignment_expression : prefixed_unary_expression ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_MULT_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_DIV_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_MOD_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_ADD_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_SUB_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_SHIFT_LEFT_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_SHIFT_RIGHT_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_AND_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_OR_ASSIGN expression",
    "assignment_expression : prefixed_unary_expression OP_XOR_ASSIGN expression",
    "expression : conditional_expression",
    "expression : assignment_expression",
    "constant_expression : expression",
    "boolean_expression : expression",
    "opt_statement_list :",
    "opt_statement_list : statement_list",
    "statement_list : statement",
    "statement_list : statement_list statement",
    "statement : declaration_statement",
    "statement : labeled_statement",
    "statement : embedded_statement",
    "declaration_statement : local_variable_declaration SEMICOLON",
    "declaration_statement : local_constant_declaration SEMICOLON",
    "local_variable_type : primary_expression opt_rank_specifier",
    "local_variable_type : builtin_types opt_rank_specifier",
    "local_variable_declaration : local_variable_type variable_declarators",
    "local_constant_declaration : CONST local_variable_type constant_declarators",
    "constant_declarators : constant_declarator",
    "constant_declarators : constant_declarators COMMA constant_declarator",
    "constant_declarator : IDENTIFIER ASSIGN constant_expression",
    "variable_declarators : variable_declarator",
    "variable_declarators : variable_declarators COMMA variable_declarator",
    "variable_declarator : IDENTIFIER ASSIGN variable_initializer",
    "variable_declarator : IDENTIFIER",
    "variable_initializer : expression",
    "variable_initializer : array_initializer",
    "labeled_statement : IDENTIFIER COLON statement",
    "embedded_statement : block",
    "embedded_statement : empty_statement",
    "embedded_statement : expression_statement",
    "embedded_statement : selection_statement",
    "embedded_statement : iteration_statement",
    "embedded_statement : jump_statement",
    "embedded_statement : try_statement",
    "embedded_statement : checked_statement",
    "embedded_statement : unchecked_statement",
    "embedded_statement : lock_statement",
    "embedded_statement : using_statement",
    "embedded_statement : unsafe_statement",
    "block : OPEN_BRACE opt_statement_list CLOSE_BRACE",
    "empty_statement : SEMICOLON",
    "expression_statement : statement_expression SEMICOLON",
    "statement_expression : invocation_expression",
    "statement_expression : object_creation_expression",
    "statement_expression : assignment_expression",
    "statement_expression : post_increment_expression",
    "statement_expression : post_decrement_expression",
    "statement_expression : pre_increment_expression",
    "statement_expression : pre_decrement_expression",
    "statement_expression : error",
    "object_creation_expression : object_or_delegate_creation_expression",
    "selection_statement : if_statement",
    "selection_statement : switch_statement",
    "if_statement : if_statement_open if_statement_rest",
    "if_statement_open : IF OPEN_PARENS",
    "if_statement_rest : boolean_expression CLOSE_PARENS embedded_statement",
    "if_statement_rest : boolean_expression CLOSE_PARENS embedded_statement ELSE embedded_statement",
    "$$1 :",
    "switch_statement : SWITCH OPEN_PARENS $$1 expression CLOSE_PARENS switch_block",
    "switch_block : OPEN_BRACE opt_switch_sections CLOSE_BRACE",
    "opt_switch_sections :",
    "opt_switch_sections : switch_sections",
    "switch_sections : switch_section",
    "switch_sections : switch_sections switch_section",
    "switch_section : switch_labels statement_list",
    "switch_labels : switch_label",
    "switch_labels : switch_labels switch_label",
    "switch_label : CASE constant_expression COLON",
    "switch_label : DEFAULT COLON",
    "iteration_statement : while_statement",
    "iteration_statement : do_statement",
    "iteration_statement : for_statement",
    "iteration_statement : foreach_statement",
    "$$2 :",
    "while_statement : WHILE OPEN_PARENS $$2 boolean_expression CLOSE_PARENS embedded_statement",
    "do_statement : DO embedded_statement WHILE OPEN_PARENS boolean_expression CLOSE_PARENS SEMICOLON",
    "$$3 :",
    "for_statement : FOR OPEN_PARENS opt_for_initializer SEMICOLON $$3 opt_for_condition SEMICOLON opt_for_iterator CLOSE_PARENS embedded_statement",
    "opt_for_initializer :",
    "opt_for_initializer : for_initializer",
    "for_initializer : local_variable_declaration",
    "for_initializer : statement_expression_list",
    "opt_for_condition :",
    "opt_for_condition : boolean_expression",
    "opt_for_iterator :",
    "opt_for_iterator : for_iterator",
    "for_iterator : statement_expression_list",
    "statement_expression_list : statement_expression",
    "statement_expression_list : statement_expression_list COMMA statement_expression",
    "$$4 :",
    "foreach_statement : FOREACH OPEN_PARENS type IDENTIFIER IN $$4 expression CLOSE_PARENS embedded_statement",
    "jump_statement : break_statement",
    "jump_statement : continue_statement",
    "jump_statement : goto_statement",
    "jump_statement : return_statement",
    "jump_statement : throw_statement",
    "break_statement : BREAK SEMICOLON",
    "continue_statement : CONTINUE SEMICOLON",
    "goto_statement : GOTO IDENTIFIER SEMICOLON",
    "goto_statement : GOTO CASE constant_expression SEMICOLON",
    "goto_statement : GOTO DEFAULT SEMICOLON",
    "return_statement : RETURN opt_expression SEMICOLON",
    "throw_statement : THROW opt_expression SEMICOLON",
    "opt_expression :",
    "opt_expression : expression",
    "try_statement : TRY block catch_clauses",
    "try_statement : TRY block opt_catch_clauses FINALLY block",
    "try_statement : TRY block error",
    "opt_catch_clauses :",
    "opt_catch_clauses : catch_clauses",
    "catch_clauses : catch_clause",
    "catch_clauses : catch_clauses catch_clause",
    "opt_identifier :",
    "opt_identifier : IDENTIFIER",
    "catch_clause : CATCH opt_catch_args block",
    "opt_catch_args :",
    "opt_catch_args : catch_args",
    "catch_args : OPEN_PARENS type opt_identifier CLOSE_PARENS",
    "checked_statement : CHECKED block",
    "unchecked_statement : UNCHECKED block",
    "unsafe_statement : UNSAFE block",
    "lock_statement : LOCK OPEN_PARENS expression CLOSE_PARENS embedded_statement",
    "$$5 :",
    "using_statement : USING OPEN_PARENS resource_acquisition CLOSE_PARENS $$5 embedded_statement",
    "resource_acquisition : local_variable_declaration",
    "resource_acquisition : expression",
  };
  protected static  string [] yyName = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"'!'",null,null,null,"'%'","'&'",
    null,"'('","')'","'*'","'+'","','","'-'","'.'","'/'",null,null,null,
    null,null,null,null,null,null,null,"':'","';'","'<'","'='","'>'",
    "'?'",null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"'['",null,"']'","'^'",null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,"'{'","'|'","'}'","'~'",null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"EOF","NONE","ERROR","ABSTRACT","AS","ADD","ASSEMBLY","BASE",
    "BOOL","BREAK","BYTE","CASE","CATCH","CHAR","CHECKED","CLASS","CONST",
    "CONTINUE","DECIMAL","DEFAULT","DELEGATE","DO","DOUBLE","ELSE","ENUM",
    "EVENT","EXPLICIT","EXTERN","FALSE","FINALLY","FIXED","FLOAT","FOR",
    "FOREACH","GOTO","IF","IMPLICIT","IN","INT","INTERFACE","INTERNAL",
    "IS","LOCK","LONG","NAMESPACE","NEW","NULL","OBJECT","OPERATOR","OUT",
    "OVERRIDE","PARAMS","PRIVATE","PROTECTED","PUBLIC","READONLY","REF",
    "RETURN","REMOVE","SBYTE","SEALED","SHORT","SIZEOF","STACKALLOC",
    "STATIC","STRING","STRUCT","SWITCH","THIS","THROW","TRUE","TRY",
    "TYPEOF","UINT","ULONG","UNCHECKED","UNSAFE","USHORT","USING",
    "VIRTUAL","VOID","VOLATILE","WHILE","GET","\"get\"","SET","\"set\"",
    "OPEN_BRACE","CLOSE_BRACE","OPEN_BRACKET","CLOSE_BRACKET",
    "OPEN_PARENS","CLOSE_PARENS","DOT","COMMA","COLON","SEMICOLON",
    "TILDE","PLUS","MINUS","BANG","ASSIGN","OP_LT","OP_GT","BITWISE_AND",
    "BITWISE_OR","STAR","PERCENT","DIV","CARRET","INTERR","OP_INC",
    "\"++\"","OP_DEC","\"--\"","OP_SHIFT_LEFT","\"<<\"","OP_SHIFT_RIGHT",
    "\">>\"","OP_LE","\"<=\"","OP_GE","\">=\"","OP_EQ","\"==\"","OP_NE",
    "\"!=\"","OP_AND","\"&&\"","OP_OR","\"||\"","OP_MULT_ASSIGN","\"*=\"",
    "OP_DIV_ASSIGN","\"/=\"","OP_MOD_ASSIGN","\"%=\"","OP_ADD_ASSIGN",
    "\"+=\"","OP_SUB_ASSIGN","\"-=\"","OP_SHIFT_LEFT_ASSIGN","\"<<=\"",
    "OP_SHIFT_RIGHT_ASSIGN","\">>=\"","OP_AND_ASSIGN","\"&=\"",
    "OP_XOR_ASSIGN","\"^=\"","OP_OR_ASSIGN","\"|=\"","OP_PTR","\"->\"",
    "LITERAL_INTEGER","\"int literal\"","LITERAL_FLOAT",
    "\"float literal\"","LITERAL_DOUBLE","\"double literal\"",
    "LITERAL_DECIMAL","\"decimal literal\"","LITERAL_CHARACTER",
    "\"character literal\"","LITERAL_STRING","\"string literal\"",
    "IDENTIFIER","LOWPREC","UMINUS","HIGHPREC",
  };

  /** index-checked interface to yyName[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
  public static string yyname (int token) {
    if ((token < 0) || (token > yyName.Length)) return "[illegal]";
    string name;
    if ((name = yyName[token]) != null) return name;
    return "[unknown]";
  }

  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected string[] yyExpecting (int state) {
    int token, n, len = 0;
    bool[] ok = new bool[yyName.Length];

    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyName.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyName[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyName.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyName[token] != null) {
          ++ len;
          ok[token] = true;
        }

    string [] result = new string[len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = yyName[token];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  public Object yyparse (yyParser.yyInput yyLex, Object yyd)
				 {
    this.debug = (yydebug.yyDebug)yyd;
    return yyparse(yyLex);
  }

  /** initial size and increment of the state/value stack [default 256].
      This is not final so that it can be overwritten outside of invocations
      of yyparse().
    */
  protected int yyMax;

  /** executed at the beginning of a reduce action.
      Used as $$ = yyDefault($1), prior to the user-specified action, if any.
      Can be overwritten to provide deep copy, etc.
      @param first value for $1, or null.
      @return first.
    */
  protected Object yyDefault (Object first) {
    return first;
  }

  /** the generated parser.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  public Object yyparse (yyParser.yyInput yyLex)
				{
    if (yyMax <= 0) yyMax = 256;			// initial size
    int yyState = 0;                                   // state stack ptr
    int [] yyStates = new int[yyMax];	                // state stack 
    Object yyVal = null;                               // value stack ptr
    Object [] yyVals = new Object[yyMax];	        // value stack
    int yyToken = -1;					// current input
    int yyErrorFlag = 0;				// #tks to shift

    int yyTop = 0;
    goto skip;
    yyLoop:
    yyTop++;
    skip:
    for (;; ++ yyTop) {
      if (yyTop >= yyStates.Length) {			// dynamically increase
        int[] i = new int[yyStates.Length+yyMax];
        System.Array.Copy(yyStates, i, 0);
        yyStates = i;
        Object[] o = new Object[yyVals.Length+yyMax];
        System.Array.Copy(yyVals, o, 0);
        yyVals = o;
      }
      yyStates[yyTop] = yyState;
      yyVals[yyTop] = yyVal;
      if (debug != null) debug.push(yyState, yyVal);

      yyDiscarded: for (;;) {	// discarding a token does not change stack
        int yyN;
        if ((yyN = yyDefRed[yyState]) == 0) {	// else [default] reduce (yyN)
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
            if (debug != null)
              debug.lex(yyState, yyToken, yyname(yyToken), yyLex.value());
          }
          if ((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
              && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
            if (debug != null)
              debug.shift(yyState, yyTable[yyN], yyErrorFlag-1);
            yyState = yyTable[yyN];		// shift to yyN
            yyVal = yyLex.value();
            yyToken = -1;
            if (yyErrorFlag > 0) -- yyErrorFlag;
            goto yyLoop;
          }
          if ((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
              && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
            yyN = yyTable[yyN];			// reduce (yyN)
          else
            switch (yyErrorFlag) {
  
            case 0:
              yyerror("syntax error", yyExpecting(yyState));
              if (debug != null) debug.error("syntax error");
              goto case 1;
            case 1: case 2:
              yyErrorFlag = 3;
              do {
                if ((yyN = yySindex[yyStates[yyTop]]) != 0
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
                  if (debug != null)
                    debug.shift(yyStates[yyTop], yyTable[yyN], 3);
                  yyState = yyTable[yyN];
                  yyVal = yyLex.value();
                  goto yyLoop;
                }
                if (debug != null) debug.pop(yyStates[yyTop]);
              } while (-- yyTop >= 0);
              if (debug != null) debug.reject();
              throw new yyParser.yyException("irrecoverable syntax error");
  
            case 3:
              if (yyToken == 0) {
                if (debug != null) debug.reject();
                throw new yyParser.yyException("irrecoverable syntax error at end-of-file");
              }
              if (debug != null)
                debug.discard(yyState, yyToken, yyname(yyToken),
  							yyLex.value());
              yyToken = -1;
              goto yyDiscarded;		// leave stack alone
            }
        }
        int yyV = yyTop + 1-yyLen[yyN];
        if (debug != null)
          debug.reduce(yyState, yyStates[yyV-1], yyN, yyRule[yyN], yyLen[yyN]);
        yyVal = yyDefault(yyV > yyTop ? null : yyVals[yyV]);
        switch (yyN) {
case 1:
#line 205 "cs-parser.jay"
  { 
          if(yyVals[-1+yyTop] != null) {
			retUsingDirective = true; 
		  }
		  retVal = (ASTNode)yyVals[-1+yyTop]; 
		  DB.parsep("parsed a using directive");
		}
  break;
case 2:
#line 213 "cs-parser.jay"
  { 
          if(yyVals[-1+yyTop] != null) {
			retStatement = true;
		  }
		  retVal = (ASTNode)yyVals[-1+yyTop]; 
		  DB.parsep("parsed a statement");
		  
        }
  break;
case 3:
#line 222 "cs-parser.jay"
  {
          if(yyVals[-1+yyTop] != null) {
			retExpression = true;
		  }
		  retVal = (ASTNode)yyVals[-1+yyTop];
		  DB.parsep("parsed a expression");
        }
  break;
case 4:
#line 240 "cs-parser.jay"
  {yyVal = null;}
  break;
case 5:
#line 241 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 8:
#line 250 "cs-parser.jay"
  { yyVal = null; }
  break;
case 9:
#line 251 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 10:
#line 255 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 11:
#line 256 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 12:
#line 262 "cs-parser.jay"
  {
		  /*current_namespace.UsingAlias ((string) $2, (string) $4, lexer.Location);*/
		yyVal = new UsingAlias((string) yyVals[-3+yyTop], (string) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 13:
#line 270 "cs-parser.jay"
  {
		/*current_namespace.Using ((string) $2);*/
		yyVal = new UsingNamespace((string) yyVals[-1+yyTop], lexer.Location);
    }
  break;
case 18:
#line 293 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 19:
#line 297 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 20:
#line 301 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 21:
#line 302 "cs-parser.jay"
  { 
	    yyVal = ((yyVals[-2+yyTop]).ToString ()) + "." + (yyVals[0+yyTop].ToString ()); }
  break;
case 22:
#line 322 "cs-parser.jay"
  {  	/* class_type */
		/* 
	           This does interfaces, delegates, struct_types, class_types, 
	           parent classes, and more! 4.2 
	         */
		yyVal = yyVals[0+yyTop]; 
	  }
  break;
case 23:
#line 329 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 24:
#line 330 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 25:
#line 339 "cs-parser.jay"
  { yyVal = "System.Object"; }
  break;
case 26:
#line 340 "cs-parser.jay"
  { yyVal = "System.String"; }
  break;
case 27:
#line 341 "cs-parser.jay"
  { yyVal = "System.Boolean"; }
  break;
case 28:
#line 342 "cs-parser.jay"
  { yyVal = "System.Decimal"; }
  break;
case 29:
#line 343 "cs-parser.jay"
  { yyVal = "System.Single"; }
  break;
case 30:
#line 344 "cs-parser.jay"
  { yyVal = "System.Double"; }
  break;
case 31:
#line 345 "cs-parser.jay"
  { yyVal = "System.Void"; }
  break;
case 32:
#line 346 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 33:
#line 350 "cs-parser.jay"
  { yyVal = "System.SByte"; }
  break;
case 34:
#line 351 "cs-parser.jay"
  { yyVal = "System.Byte"; }
  break;
case 35:
#line 352 "cs-parser.jay"
  { yyVal = "System.Int16"; }
  break;
case 36:
#line 353 "cs-parser.jay"
  { yyVal = "System.UInt16"; }
  break;
case 37:
#line 354 "cs-parser.jay"
  { yyVal = "System.Int32"; }
  break;
case 38:
#line 355 "cs-parser.jay"
  { yyVal = "System.UInt32"; }
  break;
case 39:
#line 356 "cs-parser.jay"
  { yyVal = "System.Int64"; }
  break;
case 40:
#line 357 "cs-parser.jay"
  { yyVal = "System.UInt64"; }
  break;
case 41:
#line 358 "cs-parser.jay"
  { yyVal = "System.Char"; }
  break;
case 42:
#line 363 "cs-parser.jay"
  {
		  yyVal = (string) yyVals[-1+yyTop] + (string) yyVals[0+yyTop];
	  }
  break;
case 43:
#line 373 "cs-parser.jay"
  {
		/* 7.5.1: Literals*/
		 yyVal = yyVals[0+yyTop];
	  }
  break;
case 44:
#line 385 "cs-parser.jay"
  {
		yyVal = new SimpleName((string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 45:
#line 388 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 46:
#line 389 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 47:
#line 390 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 48:
#line 391 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 49:
#line 394 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 50:
#line 395 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 51:
#line 396 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 52:
#line 399 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 53:
#line 400 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 54:
#line 401 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 55:
#line 405 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 56:
#line 406 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 57:
#line 407 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 58:
#line 408 "cs-parser.jay"
  { yyVal = new CharLit ((char) lexer.Value); }
  break;
case 59:
#line 409 "cs-parser.jay"
  { yyVal = new StringLit ((string) lexer.Value, lexer.Location); }
  break;
case 60:
#line 410 "cs-parser.jay"
  { yyVal = NullLit.Instance; }
  break;
case 61:
#line 414 "cs-parser.jay"
  { yyVal = BoolLit.TrueInstance; }
  break;
case 62:
#line 415 "cs-parser.jay"
  { yyVal = BoolLit.FalseInstance; }
  break;
case 63:
#line 420 "cs-parser.jay"
  { 
		object v = lexer.Value;

		/* */
		/* FIXME: Possible optimization would be to */
		/* compute the *Literal objects directly in the scanner*/
		/**/
		if (v is int)
			yyVal = new IntLit ((Int32) v); 
		else if (v is uint)
			yyVal = new UIntLit ((UInt32) v);
		else if (v is long)
			yyVal = new LongLit ((Int64) v);
		else if (v is ulong)
			yyVal = new ULongLit ((UInt64) v);
		else
			Console.WriteLine ("OOPS.  Unexpected integer literal from scanner"); /*CHECK*/
	  }
  break;
case 64:
#line 441 "cs-parser.jay"
  { yyVal = new FloatLit ((float) lexer.Value); }
  break;
case 65:
#line 442 "cs-parser.jay"
  { yyVal = new DoubleLit ((double) lexer.Value); }
  break;
case 66:
#line 443 "cs-parser.jay"
  { yyVal = new DecimalLit ((decimal) lexer.Value); }
  break;
case 67:
#line 448 "cs-parser.jay"
  { yyVal = new ParenExpr((Expression)yyVals[-1+yyTop], lexer.Location); }
  break;
case 68:
#line 453 "cs-parser.jay"
  {
		yyVal = new MemberAccess ((Expression) yyVals[-2+yyTop], (string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 69:
#line 457 "cs-parser.jay"
  {
		yyVal = new MemberAccess (new SimpleName ((string) yyVals[-2+yyTop], lexer.Location), (string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 70:
#line 463 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 71:
#line 468 "cs-parser.jay"
  {
		if (yyVals[-3+yyTop] == null) {
			Location l = lexer.Location;
			Report.Error ("messed up invocation without primary-expression", l);
		}
		yyVal = new Invocation ((Expression) yyVals[-3+yyTop], (ArrayList) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 72:
#line 478 "cs-parser.jay"
  { yyVal = null; }
  break;
case 73:
#line 479 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 74:
#line 484 "cs-parser.jay"
  { 
		ArrayList list = new ArrayList ();
		list.Add (yyVals[0+yyTop]);
		yyVal = list;
	  }
  break;
case 75:
#line 490 "cs-parser.jay"
  {
		ArrayList list = (ArrayList) yyVals[-2+yyTop];
		list.Add (yyVals[0+yyTop]);
		yyVal = list;
	  }
  break;
case 76:
#line 499 "cs-parser.jay"
  {
		yyVal = new Argument ((Expression) yyVals[0+yyTop], Argument.ArgType.Expression);
	  }
  break;
case 77:
#line 503 "cs-parser.jay"
  { 
		yyVal = new Argument ((Expression) yyVals[0+yyTop], Argument.ArgType.Ref);
	  }
  break;
case 78:
#line 507 "cs-parser.jay"
  { 
		yyVal = new Argument ((Expression) yyVals[0+yyTop], Argument.ArgType.Out);
	  }
  break;
case 79:
#line 513 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 80:
#line 518 "cs-parser.jay"
  {
		yyVal = new ElementAccess ((Expression) yyVals[-3+yyTop], (ArrayList) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 81:
#line 549 "cs-parser.jay"
  {
		ArrayList list = new ArrayList ();
		list.Add (yyVals[0+yyTop]);
		yyVal = list;
	  }
  break;
case 82:
#line 555 "cs-parser.jay"
  {
		ArrayList list = (ArrayList) yyVals[-2+yyTop];
		list.Add (yyVals[0+yyTop]);
		yyVal = list;
	  }
  break;
case 83:
#line 564 "cs-parser.jay"
  {
		yyVal = new This (lexer.Location);
	  }
  break;
case 84:
#line 571 "cs-parser.jay"
  {
		yyVal = new BaseAccess ((string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 85:
#line 575 "cs-parser.jay"
  {
		yyVal = new BaseIndexerAccess ((ArrayList) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 86:
#line 582 "cs-parser.jay"
  {
		yyVal = new UnaryMutator (UnaryMutator.UMode.PostIncrement,
				       (Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 87:
#line 590 "cs-parser.jay"
  {
		yyVal = new UnaryMutator (UnaryMutator.UMode.PostDecrement,
				       (Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 90:
#line 603 "cs-parser.jay"
  {
		yyVal = new New (new CType((string) yyVals[-3+yyTop]), (ArrayList) yyVals[-1+yyTop], lexer.Location);
	}
  break;
case 91:
#line 612 "cs-parser.jay"
  {
		yyVal = new ArrayCreation (new CType((string) yyVals[-5+yyTop]), (ArrayList) yyVals[-3+yyTop], (string) yyVals[-1+yyTop], (ArrayList) yyVals[0+yyTop], 
					lexer.Location);
	  }
  break;
case 92:
#line 617 "cs-parser.jay"
  {
		yyVal = new ArrayCreation (new CType((string) yyVals[-2+yyTop]), (string) yyVals[-1+yyTop], (ArrayList) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 93:
#line 621 "cs-parser.jay"
  {
		Report.Error ("new expression requires () or [] after type", lexer.Location);
	  }
  break;
case 94:
#line 628 "cs-parser.jay"
  {
		yyVal = "";
	  }
  break;
case 95:
#line 632 "cs-parser.jay"
  {
		yyVal = yyVals[0+yyTop];
	  }
  break;
case 96:
#line 639 "cs-parser.jay"
  {
		  yyVal = yyVals[0+yyTop];
	  }
  break;
case 97:
#line 643 "cs-parser.jay"
  {
		  yyVal = (string) yyVals[-1+yyTop] + (string) yyVals[0+yyTop];
	  }
  break;
case 98:
#line 650 "cs-parser.jay"
  {
		yyVal = "[" + (string) yyVals[-1+yyTop] + "]";
	  }
  break;
case 99:
#line 657 "cs-parser.jay"
  {
		yyVal = "";
	  }
  break;
case 100:
#line 661 "cs-parser.jay"
  {
		  yyVal = yyVals[0+yyTop];
	  }
  break;
case 101:
#line 668 "cs-parser.jay"
  {
		yyVal = ",";
	  }
  break;
case 102:
#line 672 "cs-parser.jay"
  {
		yyVal = (string) yyVals[-1+yyTop] + ",";
	  }
  break;
case 103:
#line 679 "cs-parser.jay"
  {
		yyVal = null;
	  }
  break;
case 104:
#line 683 "cs-parser.jay"
  {
		yyVal = yyVals[0+yyTop];
	  }
  break;
case 105:
#line 690 "cs-parser.jay"
  {
		ArrayList list = new ArrayList ();
		yyVal = list;
	  }
  break;
case 106:
#line 695 "cs-parser.jay"
  {
		yyVal = (ArrayList) yyVals[-2+yyTop];
	  }
  break;
case 107:
#line 702 "cs-parser.jay"
  {
		ArrayList list = new ArrayList ();
		list.Add (yyVals[0+yyTop]);
		yyVal = list;
	  }
  break;
case 108:
#line 708 "cs-parser.jay"
  {
		ArrayList list = (ArrayList) yyVals[-2+yyTop];
		list.Add (yyVals[0+yyTop]);
		yyVal = list;
	  }
  break;
case 109:
#line 717 "cs-parser.jay"
  {
		yyVal = new TypeOf (new CType((string) yyVals[-1+yyTop]), lexer.Location);
	  }
  break;
case 110:
#line 723 "cs-parser.jay"
  { 
		yyVal = new SizeOf (new CType((string) yyVals[-1+yyTop]), lexer.Location); /*CHECK*/

/*		note ("Verify type is unmanaged"); */
/*		note ("if (5.8) builtin, yield constant expression");*/
	  }
  break;
case 111:
#line 733 "cs-parser.jay"
  {
		yyVal = new CheckedExpression ((Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 112:
#line 740 "cs-parser.jay"
  {
		yyVal = new UncheckedExpression ((Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 113:
#line 747 "cs-parser.jay"
  {
		yyVal = new Unary (Unary.UnaryOperator.Indirection, (Expression) yyVals[-2+yyTop], lexer.Location);
		/*$$ = new MemberAccess (deref, (string) $3, lexer.Location);*/
	  }
  break;
case 114:
#line 754 "cs-parser.jay"
  {
		yyVal = new SimpleName ((string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 115:
#line 758 "cs-parser.jay"
  {
		yyVal = new ComposedCast ((Expression) yyVals[-1+yyTop], (string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 116:
#line 766 "cs-parser.jay"
  {
		yyVal = new ComposedCast ((Expression) yyVals[-1+yyTop], (string) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 117:
#line 777 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 118:
#line 779 "cs-parser.jay"
  {
		yyVal = new Unary (Unary.UnaryOperator.LogicalNot, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 119:
#line 783 "cs-parser.jay"
  {
		yyVal = new Unary (Unary.UnaryOperator.OnesComplement, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 120:
#line 787 "cs-parser.jay"
  {
		  yyVal = new ClassCast((Expression)yyVals[0+yyTop], (Expression) yyVals[-2+yyTop], lexer.Location);
	  }
  break;
case 121:
#line 791 "cs-parser.jay"
  {
		  yyVal = new ClassCast((Expression)yyVals[0+yyTop], (Expression) yyVals[-2+yyTop], lexer.Location);
	  }
  break;
case 122:
#line 801 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 123:
#line 803 "cs-parser.jay"
  { 
	  	yyVal = new Unary (Unary.UnaryOperator.UnaryPlus, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 124:
#line 807 "cs-parser.jay"
  { 
		yyVal = new Unary (Unary.UnaryOperator.UnaryNegation, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 125:
#line 811 "cs-parser.jay"
  {
		yyVal = new UnaryMutator (UnaryMutator.UMode.PreIncrement, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 126:
#line 815 "cs-parser.jay"
  {
		yyVal = new UnaryMutator (UnaryMutator.UMode.PreDecrement, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 127:
#line 819 "cs-parser.jay"
  {
		yyVal = new Unary (Unary.UnaryOperator.Indirection, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 128:
#line 823 "cs-parser.jay"
  {
		yyVal = new Unary (Unary.UnaryOperator.AddressOf, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 129:
#line 873 "cs-parser.jay"
  {
		yyVal = new UnaryMutator (UnaryMutator.UMode.PreIncrement, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 130:
#line 880 "cs-parser.jay"
  {
		yyVal = new UnaryMutator (UnaryMutator.UMode.PreDecrement, (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 131:
#line 902 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 132:
#line 904 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Multiply, 
			         (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 133:
#line 909 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Division, 
			         (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 134:
#line 914 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Modulus, 
			         (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 135:
#line 942 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 136:
#line 944 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Addition, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 137:
#line 948 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Subtraction, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 138:
#line 954 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 139:
#line 956 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.LeftShift, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 140:
#line 960 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.RightShift, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 141:
#line 966 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 142:
#line 968 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.LessThan, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 143:
#line 972 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.GreaterThan, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 144:
#line 976 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.LessThanOrEqual, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 145:
#line 980 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.GreaterThanOrEqual, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 146:
#line 984 "cs-parser.jay"
  {
		yyVal = new Is ((Expression) yyVals[-2+yyTop], new CType((string) yyVals[0+yyTop]), lexer.Location);
	  }
  break;
case 147:
#line 988 "cs-parser.jay"
  {
		yyVal = new As ((Expression) yyVals[-2+yyTop], new CType((string) yyVals[0+yyTop]), lexer.Location);
	  }
  break;
case 148:
#line 994 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 149:
#line 996 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Equality, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 150:
#line 1000 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.Inequality, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 151:
#line 1006 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 152:
#line 1008 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.BitwiseAnd, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 153:
#line 1014 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 154:
#line 1016 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.ExclusiveOr, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 155:
#line 1022 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 156:
#line 1024 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.BitwiseOr, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 157:
#line 1030 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 158:
#line 1032 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.LogicalAnd, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 159:
#line 1038 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 160:
#line 1040 "cs-parser.jay"
  {
		yyVal = new Binary (Binary.BiOperator.LogicalOr, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 161:
#line 1046 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 162:
#line 1048 "cs-parser.jay"
  {
		yyVal = new Conditional ((Expression) yyVals[-4+yyTop], (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 163:
#line 1055 "cs-parser.jay"
  {
		yyVal = new Assignment ((Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 164:
#line 1059 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.Multiply, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 165:
#line 1064 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.Division, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 166:
#line 1069 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.Modulus, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 167:
#line 1074 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.Addition, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 168:
#line 1079 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.Subtraction, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 169:
#line 1084 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.LeftShift, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 170:
#line 1089 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.RightShift, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 171:
#line 1094 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.BitwiseAnd, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 172:
#line 1099 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.BitwiseOr, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 173:
#line 1104 "cs-parser.jay"
  {
		Location l = lexer.Location;
		yyVal = new CompoundAssignment (Binary.BiOperator.ExclusiveOr, (Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop], l);
	  }
  break;
case 174:
#line 1168 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 175:
#line 1169 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 176:
#line 1173 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 177:
#line 1177 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 178:
#line 1185 "cs-parser.jay"
  { yyVal = null; }
  break;
case 179:
#line 1186 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 180:
#line 1191 "cs-parser.jay"
  {
		ArrayList stl = new ArrayList ();
		stl.Add (yyVals[0+yyTop]);
		yyVal = stl;
	}
  break;
case 181:
#line 1197 "cs-parser.jay"
  {
		ArrayList stl = (ArrayList) yyVals[-1+yyTop];
		stl.Add (yyVals[0+yyTop]);
		yyVal = stl;
    }
  break;
case 182:
#line 1205 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 183:
#line 1206 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 184:
#line 1207 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 185:
#line 1212 "cs-parser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 186:
#line 1213 "cs-parser.jay"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 187:
#line 1226 "cs-parser.jay"
  { 
		/* FIXME: Do something smart here regarding the composition of the type.*/

		/* Ok, the above "primary_expression" is there to get rid of*/
		/* both reduce/reduce and shift/reduces in the grammar, it should*/
		/* really just be "type_name".  If you use type_name, a reduce/reduce*/
		/* creeps up.  If you use qualified_identifier (which is all we need*/
		/* really) two shift/reduces appear.*/
		/* */

		/* So the super-trick is that primary_expression*/
		/* can only be either a SimpleName or a MemberAccess. */
		/* The MemberAccess case arises when you have a fully qualified type-name like :*/
		/* Foo.Bar.Blah i;*/
		/* SimpleName is when you have*/
		/* Blah i;*/
		  
		Expression expr = (Expression) yyVals[-1+yyTop];  
		if (!(expr is SimpleName || expr is MemberAccess)) {
			Report.Error("Invalid Type definition", lexer.Location);
			yyVal = "System.Object";
		}
		
		/**/
		/* So we extract the string corresponding to the SimpleName*/
		/* or MemberAccess*/
		/* */
		/*Console.WriteLine("local_variable_type: primary_expression"); */
		yyVal = GetQualifiedIdentifier (expr) + (string) yyVals[0+yyTop];
	  }
  break;
case 188:
#line 1257 "cs-parser.jay"
  {
		/*Console.WriteLine("<local_variable_type>" + (string) $1 + (string) $2);*/
		
		yyVal = (string) yyVals[-1+yyTop] + (string) yyVals[0+yyTop];
	  }
  break;
case 189:
#line 1266 "cs-parser.jay"
  {
	  /*
	  Console.WriteLine("<local_variable_declaration>");
	  Console.WriteLine("type = " + $1);
	  ArrayList al = (ArrayList)$2;
	  for(int i = 0; i < al.Count; i++) {
	   Console.WriteLine("in loop");
		VariableDeclaration vd = (VariableDeclaration)al[i];
		Console.WriteLine("in loop 2");
		testPrint(10);
		Console.WriteLine("vd.Ident = " + vd.Ident);
		Console.WriteLine("in loop 3");
	  }
	  Console.WriteLine("got here");
	  Console.WriteLine("</local_variable_declaration>");
	  */
	  yyVal = new LocalVarDecl(new CType((string) yyVals[-1+yyTop]), (ArrayList)yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 190:
#line 1288 "cs-parser.jay"
  {
		yyVal = new LocalConstDecl(new CType((string) yyVals[-1+yyTop]), (ArrayList)yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 191:
#line 1295 "cs-parser.jay"
  {
		ArrayList decl = new ArrayList ();
		decl.Add (yyVals[0+yyTop]);
		yyVal = decl;
	  }
  break;
case 192:
#line 1301 "cs-parser.jay"
  {
		ArrayList decls = (ArrayList) yyVals[-2+yyTop];
		decls.Add (yyVals[0+yyTop]);
		yyVal = decls;
	  }
  break;
case 193:
#line 1310 "cs-parser.jay"
  {
		yyVal = new VariableDeclaration ((string) yyVals[-2+yyTop], (Expression)yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 194:
#line 1317 "cs-parser.jay"
  {
		ArrayList decl = new ArrayList ();
		decl.Add (yyVals[0+yyTop]);
		yyVal = decl;
	  }
  break;
case 195:
#line 1323 "cs-parser.jay"
  {
		ArrayList decls = (ArrayList) yyVals[-2+yyTop];
		decls.Add (yyVals[0+yyTop]);
		yyVal = decls;
	  }
  break;
case 196:
#line 1332 "cs-parser.jay"
  {
		yyVal = new VariableDeclaration ((string) yyVals[-2+yyTop], yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 197:
#line 1336 "cs-parser.jay"
  {
		yyVal = new VariableDeclaration ((string) yyVals[0+yyTop], null, lexer.Location);
	  }
  break;
case 198:
#line 1343 "cs-parser.jay"
  {
		yyVal = yyVals[0+yyTop];
	  }
  break;
case 199:
#line 1347 "cs-parser.jay"
  {
		yyVal = yyVals[0+yyTop];
	  }
  break;
case 200:
#line 1358 "cs-parser.jay"
  {
		yyVal = new LabeledStatement((string) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 201:
#line 1365 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 202:
#line 1366 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 203:
#line 1367 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 204:
#line 1368 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 205:
#line 1369 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 206:
#line 1370 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 207:
#line 1371 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 208:
#line 1372 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 209:
#line 1373 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 210:
#line 1374 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 211:
#line 1375 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 212:
#line 1376 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 213:
#line 1393 "cs-parser.jay"
  {
		yyVal = new Block((ArrayList) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 214:
#line 1400 "cs-parser.jay"
  {
		  yyVal = EmptyStatement.Instance;
	  }
  break;
case 215:
#line 1407 "cs-parser.jay"
  {
		yyVal =  new ExpressionStatement((StatementExpression)yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 216:
#line 1417 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 217:
#line 1418 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 218:
#line 1419 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 219:
#line 1420 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 220:
#line 1421 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 221:
#line 1422 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 222:
#line 1423 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 223:
#line 1424 "cs-parser.jay"
  { Report.Error ("Expecting ';'", lexer.Location); }
  break;
case 224:
#line 1428 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 225:
#line 1432 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 226:
#line 1433 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 227:
#line 1438 "cs-parser.jay"
  {
		yyVal = yyVals[0+yyTop];
	  }
  break;
case 228:
#line 1445 "cs-parser.jay"
  {
	   	oob_stack.Push (lexer.Location);
	  }
  break;
case 229:
#line 1452 "cs-parser.jay"
  { 
		Location l = (Location) oob_stack.Pop ();
		yyVal = new If ((Expression) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], l);
	  }
  break;
case 230:
#line 1457 "cs-parser.jay"
  {
		Location l = (Location) oob_stack.Pop ();
		yyVal = new If ((Expression) yyVals[-4+yyTop], (Statement) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], l);
	  }
  break;
case 231:
#line 1465 "cs-parser.jay"
  { 
		oob_stack.Push (lexer.Location);
	  }
  break;
case 232:
#line 1469 "cs-parser.jay"
  {
		yyVal = new Switch ((Expression) yyVals[-2+yyTop], (ArrayList) yyVals[0+yyTop], (Location) oob_stack.Pop ());
	  }
  break;
case 233:
#line 1476 "cs-parser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	  }
  break;
case 234:
#line 1483 "cs-parser.jay"
  {
	  	Report.Error ("Empty switch block", lexer.Location); 
	}
  break;
case 235:
#line 1486 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 236:
#line 1491 "cs-parser.jay"
  {
		ArrayList sections = new ArrayList ();
		sections.Add (yyVals[0+yyTop]);
		yyVal = sections;
	  }
  break;
case 237:
#line 1497 "cs-parser.jay"
  {
		ArrayList sections = (ArrayList) yyVals[-1+yyTop];
		sections.Add (yyVals[0+yyTop]);
		yyVal = sections;
	  }
  break;
case 238:
#line 1506 "cs-parser.jay"
  {
		yyVal = new SwitchSection ((ArrayList) yyVals[-1+yyTop], (ArrayList) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 239:
#line 1513 "cs-parser.jay"
  {
		ArrayList labels = new ArrayList ();
		labels.Add (yyVals[0+yyTop]);
		yyVal = labels;
	  }
  break;
case 240:
#line 1519 "cs-parser.jay"
  {
		ArrayList labels = (ArrayList) (yyVals[-1+yyTop]);
		labels.Add (yyVals[0+yyTop]);
		yyVal = labels;
	  }
  break;
case 241:
#line 1527 "cs-parser.jay"
  { yyVal = new SwitchLabel ((Expression) yyVals[-1+yyTop], lexer.Location); }
  break;
case 242:
#line 1528 "cs-parser.jay"
  { yyVal = new SwitchLabel (null, lexer.Location); }
  break;
case 243:
#line 1533 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 244:
#line 1534 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 245:
#line 1535 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 246:
#line 1536 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 247:
#line 1541 "cs-parser.jay"
  {
		oob_stack.Push (lexer.Location);
	}
  break;
case 248:
#line 1545 "cs-parser.jay"
  {
		Location l = (Location) oob_stack.Pop ();
		yyVal = new While ((Expression) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], l);
	}
  break;
case 249:
#line 1553 "cs-parser.jay"
  {
		yyVal = new Do ((Statement) yyVals[-5+yyTop], (Expression) yyVals[-2+yyTop], lexer.Location);
	  }
  break;
case 250:
#line 1560 "cs-parser.jay"
  {
		oob_stack.Push (lexer.Location);
	  }
  break;
case 251:
#line 1564 "cs-parser.jay"
  {
		Location l = (Location) oob_stack.Pop ();
		yyVal = new For ((object) yyVals[-7+yyTop], (Expression) yyVals[-4+yyTop], (ArrayList) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], l);
	  }
  break;
case 252:
#line 1571 "cs-parser.jay"
  { yyVal = null; }
  break;
case 253:
#line 1572 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 254:
#line 1576 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 255:
#line 1577 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 256:
#line 1581 "cs-parser.jay"
  { yyVal = null; }
  break;
case 257:
#line 1582 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 258:
#line 1586 "cs-parser.jay"
  { yyVal = null; }
  break;
case 259:
#line 1587 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 260:
#line 1591 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 261:
#line 1596 "cs-parser.jay"
  {
		ArrayList sel = new ArrayList ();
		sel.Add (yyVals[0+yyTop]);
		yyVal = sel;
	  }
  break;
case 262:
#line 1602 "cs-parser.jay"
  {
		ArrayList sel = (ArrayList) yyVals[-2+yyTop];
		sel.Add (yyVals[0+yyTop]);
		yyVal = sel;
	  }
  break;
case 263:
#line 1611 "cs-parser.jay"
  {
		oob_stack.Push (lexer.Location);
	  }
  break;
case 264:
#line 1615 "cs-parser.jay"
  {
		Location l = (Location) oob_stack.Pop ();
		Foreach f = new Foreach (new CType((string) yyVals[-6+yyTop]), (string)yyVals[-5+yyTop], (Expression) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], l);
		yyVal = f;
	  }
  break;
case 265:
#line 1623 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 266:
#line 1624 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 267:
#line 1625 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 268:
#line 1626 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 269:
#line 1627 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 270:
#line 1632 "cs-parser.jay"
  {
		yyVal = new Break (lexer.Location);
	  }
  break;
case 271:
#line 1639 "cs-parser.jay"
  {
		yyVal = new Continue (lexer.Location);
	  }
  break;
case 272:
#line 1646 "cs-parser.jay"
  {
		yyVal = new Goto ((string) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 273:
#line 1650 "cs-parser.jay"
  {
		yyVal = new Goto ((Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 274:
#line 1654 "cs-parser.jay"
  {
		yyVal = new Goto (lexer.Location);
	  }
  break;
case 275:
#line 1661 "cs-parser.jay"
  {
		yyVal = new Return ((Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 276:
#line 1668 "cs-parser.jay"
  {
		yyVal = new Throw ((Expression) yyVals[-1+yyTop], lexer.Location);
	  }
  break;
case 277:
#line 1674 "cs-parser.jay"
  { yyVal = null; }
  break;
case 278:
#line 1675 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 279:
#line 1680 "cs-parser.jay"
  {
		Catch g = null;
		ArrayList s = new ArrayList ();
		
		foreach (Catch cc in (ArrayList) yyVals[0+yyTop]) {
			if (cc.CatchArgs.WantedType == null)
				g = cc;
			else
				s.Add (cc);
		}

		/* Now s contains the list of specific catch clauses*/
		/* and g contains the general one.*/
		
		yyVal = new Try ((Block) yyVals[-1+yyTop], s, g, null, lexer.Location);
	}
  break;
case 280:
#line 1697 "cs-parser.jay"
  {
		Catch g = null;
		ArrayList s = new ArrayList ();
		ArrayList catch_list = (ArrayList) yyVals[-2+yyTop];

		if (catch_list != null) {
			foreach (Catch cc in catch_list) {
				if (cc.CatchArgs.WantedType == null) 
					g = cc;     		/*CHECK - only 1 general clause and has to be last*/
				else
					s.Add (cc);
			}
		}
		
		yyVal = new Try ((Block) yyVals[-3+yyTop], s, g, (Block) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 281:
#line 1714 "cs-parser.jay"
  {
		Report.Error ("Expected catch or finally", lexer.Location);
	  }
  break;
case 282:
#line 1720 "cs-parser.jay"
  { yyVal = null; }
  break;
case 283:
#line 1721 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 284:
#line 1726 "cs-parser.jay"
  {
		ArrayList l = new ArrayList ();
		l.Add (yyVals[0+yyTop]);
		yyVal = l;
	  }
  break;
case 285:
#line 1732 "cs-parser.jay"
  {
		ArrayList l = (ArrayList) yyVals[-1+yyTop];
		l.Add (yyVals[0+yyTop]);
		yyVal = l;
	  }
  break;
case 286:
#line 1740 "cs-parser.jay"
  { yyVal = null; }
  break;
case 287:
#line 1741 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 288:
#line 1746 "cs-parser.jay"
  {
	yyVal = new Catch ((CatchArgs)yyVals[-1+yyTop], (Block) yyVals[0+yyTop], lexer.Location);
	}
  break;
case 289:
#line 1752 "cs-parser.jay"
  { yyVal = null; }
  break;
case 290:
#line 1753 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 291:
#line 1758 "cs-parser.jay"
  {
	yyVal = new CatchArgs(new CType((string) yyVals[-2+yyTop]), (string)yyVals[-1+yyTop], lexer.Location);
	}
  break;
case 292:
#line 1765 "cs-parser.jay"
  {
		yyVal = new CheckedStatement ((Block) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 293:
#line 1772 "cs-parser.jay"
  {
		yyVal = new UncheckedStatement ((Block) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 294:
#line 1779 "cs-parser.jay"
  {
		yyVal = new Unsafe ((Block) yyVals[0+yyTop], lexer.Location);
	}
  break;
case 295:
#line 1822 "cs-parser.jay"
  {
		yyVal = new Lock ((Expression) yyVals[-2+yyTop], (Statement) yyVals[0+yyTop], lexer.Location);
	  }
  break;
case 296:
#line 1829 "cs-parser.jay"
  {
		oob_stack.Push (lexer.Location);
  	  }
  break;
case 297:
#line 1833 "cs-parser.jay"
  {
		yyVal = new UsingStatement (yyVals[-3+yyTop], (Statement) yyVals[-1+yyTop], (Location) oob_stack.Pop ());
	  }
  break;
case 298:
#line 1839 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 299:
#line 1840 "cs-parser.jay"
  { yyVal = yyVals[0+yyTop]; }
  break;
#line 2232 "-"
        }
        yyTop -= yyLen[yyN];
        yyState = yyStates[yyTop];
        int yyM = yyLhs[yyN];
        if (yyState == 0 && yyM == 0) {
          if (debug != null) debug.shift(0, yyFinal);
          yyState = yyFinal;
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
            if (debug != null)
               debug.lex(yyState, yyToken,yyname(yyToken), yyLex.value());
          }
          if (yyToken == 0) {
            if (debug != null) debug.accept(yyVal);
            return yyVal;
          }
          goto yyLoop;
        }
        if (((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
            && (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
          yyState = yyTable[yyN];
        else
          yyState = yyDgoto[yyM];
        if (debug != null) debug.shift(yyStates[yyTop], yyState);
	 goto yyLoop;
      }
    }
  }

   static  short [] yyLhs  = {              -1,
    0,    0,    0,    3,    3,    2,    2,    1,    1,    6,
    6,    7,    8,   11,   11,   12,   12,   13,   10,    9,
    9,   14,   14,   14,   15,   15,   15,   15,   15,   15,
   15,   15,   17,   17,   17,   17,   17,   17,   17,   17,
   17,   16,   19,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   19,   19,   20,   20,   20,   20,   20,   20,
   31,   31,   32,   33,   33,   33,   21,   22,   22,   34,
   23,   35,   35,   36,   36,   37,   37,   37,   38,   24,
   39,   39,   40,   41,   41,   25,   26,   27,   27,   42,
   43,   43,   43,   44,   44,   18,   18,   47,   48,   48,
   49,   49,   45,   45,   46,   46,   50,   50,   52,   53,
   28,   29,   30,   54,   54,   54,   55,   55,   55,   55,
   55,   56,   56,   56,   56,   56,   56,   56,   57,   58,
   59,   59,   59,   59,   60,   60,   60,   61,   61,   61,
   62,   62,   62,   62,   62,   62,   62,   63,   63,   63,
   64,   64,   65,   65,   66,   66,   67,   67,   68,   68,
   69,   69,   70,   70,   70,   70,   70,   70,   70,   70,
   70,   70,   70,    4,    4,   71,   72,   73,   73,   74,
   74,    5,    5,    5,   75,   75,   80,   80,   78,   79,
   82,   82,   83,   81,   81,   84,   84,   51,   51,   76,
   77,   77,   77,   77,   77,   77,   77,   77,   77,   77,
   77,   77,   85,   86,   87,   97,   97,   97,   97,   97,
   97,   97,   97,   98,   88,   88,   99,  101,  102,  102,
  103,  100,  104,  105,  105,  106,  106,  107,  108,  108,
  109,  109,   89,   89,   89,   89,  114,  110,  111,  117,
  112,  115,  115,  119,  119,  116,  116,  118,  118,  121,
  120,  120,  122,  113,   90,   90,   90,   90,   90,  123,
  124,  125,  125,  125,  126,  127,  128,  128,   91,   91,
   91,  130,  130,  129,  129,  132,  132,  131,  133,  133,
  134,   92,   93,   96,   94,  136,   95,  135,  135,
  };
   static  short [] yyLen = {           2,
    2,    2,    2,    0,    1,    0,    1,    0,    1,    1,
    1,    5,    3,    0,    1,    0,    1,    1,    1,    1,
    3,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    2,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    3,    3,    3,    1,
    4,    0,    1,    1,    3,    1,    2,    2,    1,    4,
    1,    3,    1,    3,    4,    2,    2,    1,    1,    5,
    7,    4,    3,    0,    1,    1,    2,    3,    0,    1,
    1,    2,    0,    1,    2,    4,    1,    3,    4,    4,
    4,    4,    3,    1,    2,    2,    1,    2,    2,    4,
    4,    1,    2,    2,    2,    2,    2,    2,    2,    2,
    1,    3,    3,    3,    1,    3,    3,    1,    3,    3,
    1,    3,    3,    3,    3,    3,    3,    1,    3,    3,
    1,    3,    1,    3,    1,    3,    1,    3,    1,    3,
    1,    5,    3,    3,    3,    3,    3,    3,    3,    3,
    3,    3,    3,    1,    1,    1,    1,    0,    1,    1,
    2,    1,    1,    1,    2,    2,    2,    2,    2,    3,
    1,    3,    3,    1,    3,    3,    1,    1,    1,    3,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    3,    1,    2,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    2,    2,    3,    5,
    0,    6,    3,    0,    1,    1,    2,    2,    1,    2,
    3,    2,    1,    1,    1,    1,    0,    6,    7,    0,
   10,    0,    1,    1,    1,    0,    1,    0,    1,    1,
    1,    3,    0,    9,    1,    1,    1,    1,    1,    2,
    2,    3,    4,    3,    3,    3,    0,    1,    3,    5,
    3,    0,    1,    1,    2,    0,    1,    3,    0,    1,
    4,    2,    2,    2,    5,    0,    6,    1,    1,
  };
   static  short [] yyDefRed = {            0,
  223,   27,    0,   34,   41,    0,    0,    0,   28,    0,
   30,   62,   29,    0,    0,    0,    0,   37,    0,   39,
    0,   60,   25,    0,   33,   35,   26,    0,    0,   61,
    0,   38,   40,    0,    0,   36,    0,   31,    0,    0,
    0,  214,    0,    0,    0,    0,    0,    0,    0,    0,
   63,   64,   65,   66,   58,   59,    0,    0,    0,    0,
    0,    5,    9,   10,   11,    0,   32,    0,   43,   45,
   46,    0,   48,    0,    0,   51,   52,   53,   54,   55,
   56,   57,    0,    0,   89,  122,    0,  221,  222,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  174,
    0,  182,  183,  184,    0,    0,    0,  201,  202,  203,
  204,  205,  206,  207,  208,  209,  210,  211,  212,    0,
  217,  225,  226,    0,  243,  244,  245,  246,  265,  266,
  267,  268,  269,  270,    0,  292,    0,    0,    0,   44,
    0,   47,   49,   50,   88,    0,  271,    0,   70,    0,
    0,  218,    0,    0,    0,    0,    0,    0,  228,    0,
   20,    0,   22,    0,   23,   24,    0,    0,  278,  175,
    0,  231,    0,    0,    0,  293,  294,    0,    0,    0,
    0,  247,  180,    0,    0,    0,    0,    0,  119,  123,
  124,  118,  128,  127,    0,    0,    0,    7,    1,    2,
    3,    0,    0,  188,   96,    0,    0,    0,   86,   87,
    0,  187,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  185,  186,    0,    0,  194,
  215,  177,    0,  227,    0,    0,    0,    0,  191,    0,
    0,  254,  261,    0,  253,    0,    0,  176,    0,  274,
  272,    0,    0,   93,    0,    0,    0,  125,  126,  275,
    0,  276,  281,    0,    0,    0,  284,    0,  299,  298,
    0,    0,   13,    0,  213,  181,    0,    0,    0,  115,
  200,  101,    0,    0,   97,   81,    0,    0,    0,   76,
    0,    0,   74,   68,  113,   69,  163,  164,  165,  166,
  167,  168,  169,  170,  171,  173,  172,  132,  134,  133,
  131,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  111,   67,    0,    0,    0,  250,    0,    0,
    0,  273,    0,   21,    0,    0,    0,   92,    0,    0,
    0,  290,  285,    0,  112,  296,    0,    0,  120,  121,
   98,  102,   80,    0,   79,   78,   77,   71,    0,    0,
  198,  199,  196,  195,    0,  193,  192,    0,    0,  262,
  263,  295,    0,   90,  105,    0,  107,    0,    0,  288,
  280,    0,   12,    0,   82,   75,  162,    0,    0,  257,
    0,    0,    0,    0,    0,    0,  232,  287,    0,  297,
  248,  230,  249,    0,    0,   91,  104,  108,  106,    0,
    0,    0,    0,  236,    0,  239,  291,    0,    0,  259,
    0,    0,  242,  233,  237,    0,  240,    0,  264,  241,
  251,
  };
  protected static  short [] yyDgoto  = {            58,
   59,  199,   60,  252,  183,   63,   64,   65,  162,  181,
    0,  425,  163,  164,  149,  166,   67,  203,  150,   69,
   70,   71,  142,   73,  143,  144,   76,   77,   78,   79,
   80,   81,   82,   83,  311,  312,  313,  386,  307,    0,
    0,  145,   85,  212,  436,  392,  205,  303,  304,  406,
  393,    0,    0,  188,   86,   87,   88,   89,   90,   91,
   92,   93,   94,   95,   96,   97,   98,   99,  100,  170,
  269,  253,  184,  185,  102,  103,  104,  105,  106,  107,
  249,  258,  259,  250,  108,  109,  110,  111,  112,  113,
  114,  115,  116,  117,  118,  119,  120,  121,  122,  123,
  124,  254,  281,  427,  442,  443,  444,  445,  446,  125,
  126,  127,  128,  294,  264,  421,  399,  448,  265,  266,
  450,  422,  129,  130,  131,  132,  133,  171,  285,  286,
  287,  429,  371,  372,  291,  412,
  };
  protected static  short [] yySindex = {         2930,
    0,    0, -315,    0,    0, -261, 4240, -306,    0, 3248,
    0,    0,    0, -285, -274, -254, -260,    0, -195,    0,
  -87,    0,    0, 4064,    0,    0,    0, -191, 4064,    0,
 -249,    0,    0, -255, -249,    0, -314,    0, -188, 3089,
 4064,    0, 4064, 4064, 4064, 4064, 4064, 4064, 4064, 4064,
    0,    0,    0,    0,    0,    0, -190,    0, -114, -114,
 -114,    0,    0,    0,    0, -175,    0, -293,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, -174,    0,    0,    0, -101,    0,    0, -214,
 -197, -292, -213, -228, -186, -189, -183, -200, -330,    0,
    0,    0,    0,    0, -171, -168, -233,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, -162,
    0,    0,    0, 4064,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 4064,    0, -155, -148, 4064,    0,
 -293,    0,    0,    0,    0, -220,    0, -143,    0, -284,
 -101,    0, -130, 3384,  -87, 4064, -139, -135,    0, 4064,
    0, -131,    0, -239,    0,    0, 4064, 4064,    0,    0,
 -133,    0, -129, -202, 4064,    0,    0, 4064, -136, -131,
 -127,    0,    0, -118, 3089, -297,    0, -224,    0,    0,
    0,    0,    0,    0,    0,    0, 3089,    0,    0,    0,
    0, -121, -175,    0,    0, 3792, 3520, -185,    0,    0,
 -184,    0, -181, 4064, 4064, 4064, 4064, 4064, 4064, 4064,
 4064, 4064, 4064, 4064, 4064, 4064, 4064, 4064, 4064, 4064,
 4064,  -87,  -87, 4064, 4064, 4064, 4064, 4064, 4064, 4064,
 4064, 4064, 4064, 4064, 4064,    0,    0, -126, -117,    0,
    0,    0, -113,    0, -107, -104, -119, -105,    0, 4064,
 -100,    0,    0,  -98,    0, -102, -322,    0,  -95,    0,
    0,  -90, -158,    0, 3792, 3520, -242,    0,    0,    0,
 4064,    0,    0,  -88,   -4,  -20,    0,  -82,    0,    0,
  -80, -152,    0, 4064,    0,    0, 4200, -175, 4064,    0,
    0,    0,  -75,  -78,    0,    0, -253, 4064, 4064,    0,
  -74,  -77,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -214, -214, -197, -197, -175, -175, -292, -292, -292,
 -292, -213, -213, -228, -186, -189, -183,  -76, -200, 3928,
 -233, 3248,    0,    0, 4064, -220, 4064,    0, 3384,  -17,
 -175,    0, 3248,    0, -237,  -71, 3656,    0,  -70,  -87,
 -249,    0,    0, -249,    0,    0, -209,  -68,    0,    0,
    0,    0,    0, 4064,    0,    0,    0,    0, 3520, 4064,
    0,    0,    0,    0,    8,    0,    0,  -59, 4064,    0,
    0,    0, -175,    0,    0,  -57,    0,  -64, -316,    0,
    0, 3248,    0, 3248,    0,    0,    0, 3248,  -61,    0,
  -55, 4064,  -48, 3928,  -45, -198,    0,    0,  -43,    0,
    0,    0,    0, 3384,  -42,    0,    0,    0,    0, 4064,
  -44,  -41, -198,    0, 2771,    0,    0,  -40, -102,    0,
 3248,  -34,    0,    0,    0, 3089,    0, 3248,    0,    0,
    0,
  };
  protected static  short [] yyRindex = {            6,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -23,    0,    0,    0,    0,  -23,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -35,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1289,    0,  319,  319,
  319,    0,    0,    0,    0, -317,    0,  282,    0,    0,
    0,  993,    0, 1068, 1143,    0,    0,    0,    0,    0,
    0,    0,    0, 1218,    0,    0, 1772,    0,    0, 1849,
 2023, 2183,  356,  223, 2571, 2619,   45,  410,   29,    0,
    4,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  -89,    0,    0,    0,    0,    0,    0,    0,    0, 1553,
    0,    0,    0,  -21,    0,    0,    0,    0,    0,    0,
    0, 1372,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   48,    0,    0,    0,    0, -205,  -15,
    0,    0,    0,    0,   -9,    0, -277,    0,    0,    0,
    0,    0,    0,    0, 1630, 1701,    0,    0,    0,    0,
    0,   -8,  760,    0,    0,   -8,   -7,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, -250, -230,    0,
    0,    0,    0,    0,    0,    0,    0,  -13,    0,    0,
    0,    0,    0,    0,    0,  -12,    0,    0,    0,    0,
    0,    0,    0,    0,   -8,   -7, -235,    0,    0,    0,
    0,    0,    0,    3,  514,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1480,   -5,    0,    0,
    0,    0,    0,   -2,    0,    0,    0,    0,    0,    0,
    0,   -1,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 1892, 1936, 2069, 2141, 2433, 2478, 2228, 2294, 2336,
 2381,  541, 2520, 2546, 2596, 2644, 2668,    0,  720,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 1437,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  681,    0,    0,    0,    5,    0,
    0,    0,  839,    0,    0,    7,    0,    0,   14,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  918,   21,    0,   25,    0,    0,    0,    0,
    0,    0,    0,   28,    0,    0,    0,    0,    0,    0,
    0,    0,   34,    0,    0,    0,    0,    0,   37,    0,
    0,    0,    0,    0,    0, -225,    0,    0,    0,    0,
    0,
  };
  protected static  short [] yyGindex = {            0,
    0,   56,    0,  343,    9,    0,    0,    0,  -30,    0,
    0,    0,    0, -142,   19,    0,    0, -125,   18,    0,
    0,    0,    1,    0,    2,   10,    0,    0,    0,    0,
    0,    0,    0,    0,   70,    0,  -39,   40,   76,    0,
    0,   36,    0,  -66,    0, -262, -180,    0,    0,    0,
 -332,    0,    0,    0,   58,   86,    0,    0,  -62, -103,
  -97,  -69,  119,  142,  145,  150,  155,    0,    0,   71,
 -328, -278,    0,  -37,    0,    0,   55, -110,    0,  394,
    0,    0,   46,   52,   -3,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, -149,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  -38,    0,  -36,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  -28,
    0,    0,    0,    0,    0,    0,    0,  375,    0,    0,
  126,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyTable = {           204,
   72,   74,  136,  175,  263,    4,  180,  300,   62,   75,
   72,   74,  267,  156,  368,  378,  274,   68,   66,   75,
   42,  157,  305,  202,  141,   66,  396,  174,  161,  202,
  176,  177,   70,  178,  407,   84,  244,  134,  277,  165,
   72,   74,  238,  262,  157,   84,  147,  232,  202,   75,
  238,  297,  206,  283,  207,  245,  208,   68,   66,  187,
  298,  260,  154,  207,  153,  208,  284,  290,  114,  440,
  101,  114,   70,  155,  209,   84,  210,  441,  398,  230,
  152,  231,   40,  209,  233,  210,  135,  159,   40,  336,
  337,  438,  175,  383,   40,  151,  305,  384,  197,  360,
  197,  367,  197,  202,   94,  428,  275,  179,  276,  403,
  152,  452,   42,  384,  211,  200,  201,  305,  189,  238,
  420,  202,  189,  211,  299,  151,  334,  335,  189,  190,
  191,  192,  193,  194,  195,  196,  338,  339,  340,  341,
  273,  361,  198,  413,   20,  234,  235,   20,  225,  226,
  227,  238,  160,  239,   72,   74,  172,  228,  229,  182,
  437,  197,  236,   75,  237,  332,  333,  158,  342,  343,
  202,   68,   66,  165,  240,  213,  241,    2,  242,    4,
  305,  246,    5,  243,  247,   72,   74,    9,  248,   84,
  251,   11,  135,  296,   75,   68,   66,   72,   74,  175,
   13,  257,   68,   66,  178,  301,   75,   18,  261,  400,
  361,  361,   20,  270,   68,   66,   23,  271,  273,  280,
   84,  292,  151,  282,  152,  293,  295,  409,   25,  302,
   26,  350,   84,  351,   27,  352,  314,  315,  355,  151,
  316,  353,   32,   33,  354,  356,   36,  357,  359,   38,
  165,  165,  278,  279,  358,  152,  214,  362,  363,  370,
  175,  377,    4,  364,  284,  374,  375,  152,  376,  161,
  151,  381,  382,  389,  388,  390,  401,  404,  408,  426,
  414,  117,  151,  361,  263,  161,  215,  418,  216,  419,
  217,  433,  218,  424,  219,  367,  220,  434,  221,  439,
  222,  157,  223,  454,  224,  447,  451,  453,  458,  178,
  328,  329,  330,  331,  331,  331,  331,  460,    6,  331,
  331,  331,  331,  331,  331,  331,  331,  331,  331,  277,
  331,  252,   94,  282,  161,  179,  423,   19,   99,  190,
  255,   72,   61,  116,  100,  366,  289,   73,  387,  416,
  365,   16,   72,   74,  379,  148,  218,  256,  344,   72,
   74,   75,  286,   72,   74,   17,  169,  410,   75,  234,
  411,  169,   75,  161,  161,  161,  258,  161,  235,  161,
  161,  161,  345,  186,  380,  260,  346,   84,  165,  157,
  157,  157,  347,  157,   84,  157,  157,  157,   84,  349,
  146,  397,  394,  173,  455,  449,  395,  456,  457,  159,
  373,  157,   72,   74,   72,   74,    0,  402,   72,   74,
    0,   75,  152,   75,    0,    0,    0,   75,  157,  152,
  157,    0,    0,  152,   72,   74,    0,  151,    0,    0,
    0,    0,    0,   75,  151,   72,   74,   84,  151,   84,
    0,   72,   74,   84,   75,    0,   72,   74,   72,   74,
   75,    0,   68,   66,  296,   75,  430,   75,  431,   84,
    0,    0,  432,   68,   66,    0,    0,  255,    0,  151,
   84,  256,  152,    0,  152,    0,   84,    0,  152,    0,
    0,   84,    0,   84,    0,    0,    0,  151,  268,  151,
    0,    0,  272,  151,  152,  459,    0,    0,    0,    0,
    0,    0,  461,  279,    0,  152,    0,  288,    0,  151,
  289,  152,    0,    0,    0,    0,  152,    0,  152,    0,
  151,    0,    0,    0,    0,    0,  151,    0,  117,    0,
  149,  151,  117,  151,    0,    0,    0,    0,  306,  310,
    0,    0,    0,    0,    0,    0,  317,  318,  319,  320,
  321,  322,  323,  324,  325,  326,  327,  151,  151,  151,
    0,  151,    0,  151,  151,  151,    0,    0,    0,  117,
    0,    0,    0,  151,  151,    0,  348,    0,  151,  151,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  306,    0,    0,    0,  151,    0,  151,    0,
    0,    0,  148,    0,    0,    0,    0,  306,  310,    0,
    0,    0,    0,  369,    0,    0,    0,    0,    0,    0,
  117,    0,    0,    0,    0,    0,  117,  117,    0,  117,
  117,  117,  117,  117,  117,  117,  117,  117,  117,    0,
  385,  385,    0,  117,    0,  117,    0,  117,    0,  117,
    0,  117,    0,  117,    0,  117,  159,  117,    0,  117,
    0,  117,    0,  117,    0,  117,    0,  117,    0,  117,
  229,  117,    0,  117,    0,  117,    0,  117,    0,    0,
    0,    0,  391,    0,    0,    0,    0,  268,    0,    0,
  148,  148,  148,   94,  148,    0,  148,  148,  148,  391,
    0,    0,    0,    0,    0,    0,  148,  148,    0,  160,
    0,  148,  148,    0,    0,    0,  415,    0,    0,    0,
    0,  310,  417,    0,    0,  148,    0,  148,    0,  148,
    0,  148,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  159,  159,  159,    0,  159,   95,
  159,  159,  159,    0,  435,    0,  391,    0,    0,  279,
  279,    0,    0,    0,    0,    0,  159,    0,  279,  279,
  279,  279,  268,  279,  279,    0,  279,  279,  279,  279,
    0,  279,  279,  279,    0,  159,    0,  149,  279,  283,
    0,  279,  279,  279,  279,  279,    0,    0,  279,    0,
    0,    0,  279,  279,    0,  279,  279,  279,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  279,    0,  279,
    0,  279,    0,    0,    0,  279,    0,  279,   94,  279,
  279,  279,    0,  279,  279,  279,  279,  279,  279,    0,
  279,    0,  279,    0,    0,    0,    0,  279,  279,    0,
    0,  279,    0,    0,    0,    0,  279,  279,  279,  279,
  279,    0,    0,    0,  279,    0,  279,    0,    0,    0,
    0,  279,    0,  279,    0,  149,  149,  149,    0,  149,
    0,  149,  149,  149,    0,    0,    0,    0,    0,    0,
    0,  149,  149,    0,    0,    0,  149,  149,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  103,    0,    0,
  149,    0,  149,  279,  149,  279,  149,  279,    0,  279,
    0,  279,    0,  279,    0,  279,  229,  229,    0,    0,
    0,    0,    0,    0,    0,  229,  229,  229,  229,    0,
  229,  229,    0,  229,  229,  229,  229,    0,  229,  229,
    0,    0,    0,    0,    0,  229,    0,    0,  229,  229,
  229,  229,  229,    0,    0,  229,  160,    0,    0,  229,
  229,    0,  229,  229,  229,    0,    0,    0,    0,    0,
    0,    0,   47,    0,  229,    0,  229,    0,  229,    0,
    0,    0,  229,    0,  229,    0,  229,  229,  229,    0,
  229,  229,  229,  229,  229,  229,   95,  229,    0,  229,
   95,    0,    0,    0,  229,  229,    0,    0,  229,    0,
    0,    0,    0,  229,  229,  229,  229,  229,    0,    0,
    0,  229,    0,  229,    0,    0,    0,    0,  229,    0,
  229,    0,    0,    0,    0,    0,    0,   95,    0,    0,
    0,    0,    0,    0,  160,  160,  160,   49,  160,    0,
  160,  160,  160,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  160,    0,    0,    0,
  229,    0,  229,    0,  229,   94,  229,    0,  229,   94,
  229,    0,  229,   95,   95,  160,   95,   95,   95,   95,
   95,   95,   95,    0,   95,   95,    0,   95,   95,   95,
   95,   95,   95,   95,   95,   95,   95,   95,    0,   95,
    0,   95,    0,   95,    0,   95,   94,   95,    0,   95,
    0,   95,   50,   95,    0,   95,    0,   95,    0,   95,
    0,   95,    0,   95,    0,   95,    0,   95,    0,   95,
    0,   95,    0,   95,    0,   95,    0,   95,    0,    0,
    0,    0,    0,    0,  103,    0,    0,    0,  103,    0,
    0,   95,   94,   94,    0,   94,   94,   94,   94,   94,
   94,   94,    0,   94,   94,    0,   94,   94,   94,   94,
   94,   94,   94,   94,   94,   94,   94,    0,   94,    0,
   94,    0,   94,    0,   94,  103,   94,   88,   94,    0,
   94,    0,   94,    0,   94,    0,   94,    0,   94,    0,
   94,    0,   94,    0,   94,    0,   94,    0,   94,    0,
   94,    0,   94,    0,   94,    0,   94,    0,    0,   47,
    0,    0,    0,   47,    0,    0,    0,    0,    0,    0,
   94,    0,  103,  103,  103,  103,  103,  103,  103,  103,
  103,    0,  103,  103,    0,  103,  103,  103,  103,  103,
  103,  103,  103,  103,  103,  103,    0,  103,   44,  103,
   47,  103,    0,  103,    0,  103,    0,  103,    0,  103,
    0,  103,    0,  103,    0,  103,    0,  103,    0,  103,
    0,  103,    0,  103,    0,  103,    0,  103,    0,  103,
    0,  103,    0,  103,   49,  103,    0,    0,   49,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   47,  103,
   47,  216,   47,  216,    0,  216,    0,   47,   47,    0,
   47,   47,   47,   47,   47,   47,   47,   47,   47,   47,
   47,    0,   47,    0,   47,   49,   47,    0,   47,    0,
   47,   18,   47,    0,   47,    0,   47,    0,   47,    0,
   47,    0,   47,    0,   47,    0,   47,    0,   47,    0,
   47,    0,   47,    0,   47,    0,   47,    0,   47,   50,
   47,    0,    0,   50,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   49,   47,   49,  219,   49,  219,    0,
  219,    0,   49,   49,    0,   49,   49,   49,   49,   49,
   49,   49,   49,   49,   49,   49,   42,   49,    0,   49,
   50,   49,    0,   49,    0,   49,    0,   49,    0,   49,
    0,   49,    0,   49,    0,   49,    0,   49,    0,   49,
    0,   49,    0,   49,    0,   49,    0,   49,    0,   49,
    0,   49,    0,   49,   88,   49,    0,    0,   88,   67,
    0,    0,    0,    0,    0,    0,    0,    0,   50,   49,
   50,  220,   50,  220,    0,  220,    0,   50,   50,    0,
   50,   50,   50,   50,   50,   50,   50,   50,   50,   50,
   50,    0,   50,    0,   50,   88,   50,    0,   50,    0,
   50,    0,   50,    0,   50,    0,   50,    0,   50,    0,
   50,    0,   50,    0,   50,    0,   50,    0,   50,    0,
   50,    0,   50,    0,   50,   44,   50,    0,   50,   44,
   50,    0,  117,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   88,   50,   88,  224,   88,  224,    0,
  224,    0,   88,   88,    0,   88,   88,   88,   88,   88,
   88,   88,   88,   88,   88,   88,   44,   88,    0,   88,
    0,   88,    0,   88,    0,   88,    0,   88,    0,   88,
    0,   88,    0,   88,    0,   88,    0,   88,    0,   88,
    0,   88,    0,   88,    0,   88,    0,   88,    0,   88,
    0,   88,    0,   88,    0,   88,    0,   18,   18,  125,
    0,    0,   18,    0,   44,    0,   44,    0,   44,   88,
    0,    0,    0,   44,   44,    0,   44,   44,   44,   44,
   44,   44,   44,   44,   44,   44,   44,    0,   44,    0,
   44,    0,   44,    0,   44,    0,   44,    0,   44,   18,
   44,    0,   44,    0,   44,    0,   44,    0,   44,    0,
   44,    0,   44,    0,   44,    0,   44,    0,   44,    0,
   44,    0,   44,   42,   44,    0,   44,   42,    0,    0,
  126,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   44,    0,    0,    0,    0,    0,   18,   18,   18,   18,
   18,    0,   18,   18,   18,    0,    0,    0,    0,    0,
   18,   18,   18,   18,   42,    0,   67,   18,   18,    0,
   67,    0,    0,    0,    0,    0,    0,   18,    0,   18,
    0,   18,    0,   18,    0,   18,    0,   18,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  131,    0,    0,    0,    0,    0,   67,    0,    0,
    0,   42,    0,   42,    0,   42,    0,   42,   42,   42,
    0,    0,    0,   18,    0,   42,   42,   42,   42,    0,
    0,    0,   42,   42,    0,    0,    0,    0,    0,  117,
    0,    0,   42,  117,   42,    0,   42,    0,   42,    0,
   42,    0,   42,    0,   67,   67,   67,    0,   67,   67,
   67,   67,   67,    0,   67,   67,    0,   67,   67,   67,
   67,   67,   67,   67,   67,   67,   67,   67,  135,   67,
  117,   67,    0,   67,    0,   67,    0,   67,   42,   67,
    0,   67,    0,   67,    0,   67,    0,   67,    0,   67,
    0,   67,    0,   67,    0,   67,    0,   67,    0,   67,
    0,   67,    0,   67,    0,   67,  125,   67,    0,    0,
  125,  136,    0,    0,    0,    0,    0,  117,    0,  117,
    0,  117,    0,  117,  117,  117,    0,  117,  117,    0,
  117,  117,  117,  117,  117,  117,  117,  117,  117,  117,
    0,    0,    0,    0,  117,    0,  117,  125,  117,    0,
  117,    0,  117,    0,  117,  137,  117,    0,  117,    0,
  117,    0,  117,    0,  117,    0,  117,    0,  117,    0,
  117,    0,  117,    0,  117,    0,  117,  126,  117,    0,
    0,  126,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  129,    0,
  129,    0,  129,    0,  125,  125,    0,  125,  125,  125,
  125,  125,  125,  125,  125,  125,  125,    0,  126,    0,
    0,  125,    0,  125,    0,  125,    0,  125,    0,  125,
    0,  125,    0,  125,    0,  125,    0,  125,    0,  125,
    0,  125,  138,  125,    0,  125,    0,  125,  131,  125,
    0,  125,  131,  125,    0,  125,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  130,
    0,  130,    0,  130,    0,  126,  126,    0,  126,  126,
  126,  126,  126,  126,  126,  126,  126,  126,  139,  131,
    0,    0,  126,    0,  126,    0,  126,    0,  126,    0,
  126,    0,  126,    0,  126,    0,  126,    0,  126,    0,
  126,    0,  126,    0,  126,    0,  126,    0,  126,    0,
  126,    0,  126,    0,  126,  135,  126,    0,    0,  135,
    0,    0,    0,    0,    0,    0,  131,  131,  131,    0,
  131,    0,  131,  131,  131,    0,  131,  131,    0,    0,
  131,  131,  131,  131,  131,  131,  131,  131,  131,    0,
  140,    0,    0,  131,    0,  131,  135,  131,  136,  131,
    0,  131,  136,  131,    0,  131,    0,  131,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  141,    0,    0,    0,    0,    0,    0,  136,
    0,    0,  137,  135,  135,  135,  137,  135,    0,  135,
  135,  135,    0,  135,  135,    0,    0,  135,  135,  135,
  135,    0,    0,    0,  135,  135,    0,    0,    0,    0,
  135,    0,  135,    0,  135,    0,  135,  142,  135,    0,
  135,    0,  135,  137,  135,    0,  136,  136,  136,    0,
  136,    0,  136,  136,  136,    0,  136,  136,    0,    0,
  136,  136,  136,  136,    0,    0,    0,  136,  136,    0,
    0,    0,    0,  136,    0,  136,    0,  136,    0,  136,
    0,  136,    0,  136,    0,  136,    0,  136,    0,  138,
  137,  137,  137,  138,  137,    0,  137,  137,  137,    0,
  137,  137,    0,  143,  137,  137,  137,  137,    0,    0,
    0,  137,  137,    0,    0,    0,    0,  137,    0,  137,
    0,  137,    0,  137,    0,  137,    0,  137,    0,  137,
  138,  137,    0,    0,    0,  139,    0,    0,    0,  139,
    0,    0,    0,    0,    0,  144,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  139,  138,  138,  138,
    0,  138,    0,  138,  138,  138,    0,    0,    0,    0,
  145,  138,  138,  138,  138,    0,    0,    0,  138,  138,
    0,    0,    0,    0,  138,    0,  138,  140,  138,    0,
  138,  140,  138,    0,  138,    0,  138,    0,  138,    0,
    0,    0,    0,  139,  139,  139,    0,  139,    0,  139,
  139,  139,    0,    0,    0,    0,    0,  139,  139,  139,
  139,    0,  147,    0,  139,  139,    0,    0,  140,  141,
  139,    0,  139,  141,  139,    0,  139,    0,  139,    0,
  139,    0,  139,    0,  139,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  146,    0,    0,
  141,    0,    0,    0,  142,  140,  140,  140,  142,  140,
    0,  140,  140,  140,    0,    0,    0,    0,    0,  140,
  140,  140,  140,    0,    0,    0,  140,  140,    0,    0,
    0,    0,  140,    0,  140,    0,  140,    0,  140,  150,
  140,    0,  140,    0,  140,  142,  140,  141,  141,  141,
    0,  141,    0,  141,  141,  141,    0,    0,    0,    0,
    0,  141,  141,  141,  141,  152,    0,    0,  141,  141,
  143,    0,    0,    0,  143,    0,    0,    0,  141,    0,
  141,    0,  141,    0,  141,    0,  141,    0,  141,    0,
  153,    0,  142,  142,  142,    0,  142,    0,  142,  142,
  142,    0,    0,    0,    0,    0,  142,  142,  142,  142,
    0,  143,  144,  142,  142,  154,  144,    0,    0,    0,
    0,    0,    0,  142,    0,  142,    0,  142,    0,  142,
    0,  142,    0,  142,    0,    0,    0,    0,  155,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  144,    0,    0,    0,  145,  143,  143,
  143,  145,  143,  156,  143,  143,  143,    0,    0,    0,
    0,    0,  143,  143,  143,  143,    0,    0,    0,  143,
  143,    0,    0,    0,    0,    0,    0,  158,    0,  143,
    0,  143,    0,  143,    0,  143,    0,  143,  145,  143,
  144,  144,  144,    0,  144,    0,  144,  144,  144,  147,
    0,    0,    0,  147,  144,  144,  144,  144,    0,    0,
    0,  144,  144,    0,    0,    0,    0,    0,    0,    0,
    0,  144,    0,  144,    0,  144,    0,  144,    0,  144,
    0,  144,    0,    0,    0,  145,  145,  145,    0,  145,
  147,  145,  145,  145,  146,    0,    0,    0,  146,  145,
  145,  145,  145,    0,    0,    0,  145,  145,    0,    0,
    0,    0,    0,    0,    0,    0,  145,    0,  145,    0,
  145,    0,  145,    0,  145,    0,  145,    0,    0,    0,
    0,    0,    0,    0,    0,  146,  150,  147,    0,  147,
    0,  147,    0,  147,  147,  147,    0,    0,    0,    0,
    0,  147,  147,  147,  147,    0,    0,    0,  147,  147,
    0,    0,  152,    0,    0,    0,    0,    0,  147,    0,
  147,    0,  147,    0,  147,    0,  147,    0,  147,    0,
    0,    0,  146,    0,  146,    0,  146,  153,  146,  146,
  146,    0,    0,    0,    0,    0,  146,  146,  146,  146,
    0,    0,    0,  146,  146,    0,    0,    0,    0,    0,
    0,    0,  154,  146,    0,  146,    0,  146,    0,  146,
    0,  146,    0,  146,  150,  150,  150,    0,  150,    0,
  150,  150,  150,    0,    0,  155,    0,    0,    0,    0,
  150,  150,    0,    0,    0,  150,  150,    0,    0,    0,
  152,  152,  152,    0,  152,    0,  152,  152,  152,  150,
  156,  150,    0,  150,    0,  150,  152,  152,    0,    0,
    0,  152,  152,    0,    0,  153,  153,  153,    0,  153,
    0,  153,  153,  153,  158,    0,    0,    0,    0,  152,
    0,  152,  153,    0,    0,    0,  153,  153,    0,    0,
  154,  154,  154,    0,  154,    0,  154,  154,  154,    0,
    0,    0,    0,    0,  153,    0,  153,  154,    0,    0,
    0,  154,  154,  155,  155,  155,    0,  155,    0,  155,
  155,  155,    0,    0,    0,    0,    0,    0,    0,  154,
  155,  154,    0,    0,    0,  155,    0,    0,  156,  156,
  156,    0,  156,    0,  156,  156,  156,    0,    0,    0,
    0,    0,  155,    0,  155,  156,    0,    0,    0,    0,
  156,    0,  158,  158,  158,    0,  158,    0,  158,  158,
  158,    0,    0,    0,    0,    0,    1,  156,    0,  156,
    0,    0,    0,    0,  158,    2,    3,    4,  440,    0,
    5,    6,    0,    7,    8,    9,  441,    0,   10,   11,
    0,  158,    0,  158,    0,   12,    0,    0,   13,   14,
   15,   16,   17,    0,    0,   18,    0,    0,    0,   19,
   20,    0,   21,   22,   23,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   24,    0,   25,    0,   26,    0,
    0,    0,   27,    0,   28,    0,   29,   30,   31,    0,
   32,   33,   34,   35,   36,  148,    0,   38,    0,   39,
    0,    0,    0,    0,   40,    0,    0,    0,   41,    0,
    0,    0,    0,   42,   43,   44,   45,   46,    0,    0,
    0,   47,    0,   48,    0,    0,    0,    0,   49,    0,
   50,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   51,    0,   52,    0,   53,    1,   54,    0,   55,    0,
   56,    0,   57,    0,    2,    3,    4,    0,    0,    5,
    6,    0,    7,    8,    9,    0,    0,   10,   11,    0,
    0,    0,    0,    0,   12,    0,    0,   13,   14,   15,
   16,   17,    0,    0,   18,    0,    0,    0,   19,   20,
    0,   21,   22,   23,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   24,    0,   25,    0,   26,    0,    0,
    0,   27,    0,   28,    0,   29,   30,   31,    0,   32,
   33,   34,   35,   36,   37,    0,   38,    0,   39,    0,
    0,    0,    0,   40,    0,    0,    0,   41,    0,    0,
    0,    0,   42,   43,   44,   45,   46,    0,    0,    0,
   47,    0,   48,    0,    0,    0,    0,   49,    0,   50,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   51,
    0,   52,    0,   53,    1,   54,    0,   55,    0,   56,
    0,   57,    0,    2,    3,    4,    0,    0,    5,    6,
    0,    7,    8,    9,    0,    0,   10,   11,    0,    0,
    0,    0,    0,   12,    0,    0,   13,   14,   15,   16,
   17,    0,    0,   18,    0,    0,    0,   19,   20,    0,
   21,   22,   23,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   24,    0,   25,    0,   26,    0,    0,    0,
   27,    0,   28,    0,   29,   30,   31,    0,   32,   33,
   34,   35,   36,  148,    0,   38,    0,   39,    0,    0,
    0,    0,   40,    0,    0,    0,   41,    0,    0,    0,
    0,   42,   43,   44,   45,   46,    0,    0,    0,   47,
    0,   48,    0,    0,    0,    0,   49,    0,   50,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   51,    0,
   52,    0,   53,    1,   54,    0,   55,    0,   56,    0,
   57,    0,    2,    3,    4,    0,    0,    5,    6,    0,
    0,    8,    9,    0,    0,   10,   11,    0,    0,    0,
    0,    0,   12,    0,    0,   13,   14,   15,   16,   17,
    0,    0,   18,    0,    0,    0,   19,   20,    0,   21,
   22,   23,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   24,    0,   25,    0,   26,    0,    0,    0,   27,
    0,   28,    0,   29,   30,   31,    0,   32,   33,   34,
   35,   36,  148,    0,   38,    0,   39,    0,    0,    0,
    0,   40,    0,    0,    0,   41,    0,    0,    0,    0,
   42,   43,   44,   45,   46,    0,    0,    0,   47,    0,
   48,    0,    0,    0,    0,   49,    0,   50,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    1,
    0,    0,    0,    0,    0,    0,    0,    0,    2,    0,
    4,    0,    0,    5,  137,    0,    0,   51,    9,   52,
    0,   53,   11,   54,    0,   55,    0,   56,   12,  140,
    0,   13,    0,    0,    0,    0,    0,    0,   18,    0,
    0,    0,    0,   20,    0,   21,   22,   23,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   25,
    0,   26,    0,    0,    0,   27,    0,    0,    0,    0,
   30,    0,    0,   32,   33,  138,    0,   36,    0,    0,
   38,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   41,    0,    0,    0,    0,    0,   43,   44,   45,
   46,    0,    0,    0,   47,    0,   48,    0,    0,    0,
    0,   49,    0,   50,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    2,    0,    4,    0,    0,    5,
  137,    0,    0,   51,    9,   52,    0,   53,   11,   54,
    0,   55,    0,   56,   12,  140,    0,   13,    0,    0,
    0,    0,    0,    0,   18,    0,    0,    0,    0,   20,
    0,   21,   22,   23,    0,  308,    0,    0,    0,    0,
    0,    0,  309,    0,    0,   25,    0,   26,    0,    0,
    0,   27,    0,    0,    0,    0,   30,    0,    0,   32,
   33,  138,    0,   36,    0,    0,   38,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   41,    0,    0,
    0,    0,    0,   43,   44,   45,   46,    0,    0,    0,
   47,    0,   48,    0,    0,    0,    0,  167,    0,  168,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    2,    0,    4,    0,    0,    5,  137,    0,    0,   51,
    9,   52,    0,   53,   11,   54,    0,   55,    0,   56,
   12,  140,    0,   13,    0,    0,    0,    0,    0,    0,
   18,    0,    0,    0,    0,   20,    0,   21,   22,   23,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   25,    0,   26,    0,    0,    0,   27,    0,    0,
    0,    0,   30,    0,    0,   32,   33,  138,    0,   36,
    0,    0,   38,    0,    0,    0,    0,    0,    0,  367,
  405,    0,    0,   41,    0,    0,    0,    0,    0,   43,
   44,   45,   46,    0,    0,    0,   47,    0,   48,    0,
    0,    0,    0,  167,    0,  168,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    2,    0,    4,    0,
    0,    5,  137,    0,    0,   51,    9,   52,    0,   53,
   11,   54,    0,   55,    0,   56,   12,  140,    0,   13,
    0,    0,    0,    0,    0,    0,   18,    0,    0,    0,
    0,   20,    0,   21,   22,   23,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   25,    0,   26,
    0,    0,    0,   27,    0,    0,    0,    0,   30,    0,
    0,   32,   33,  138,    0,   36,    0,    0,   38,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   41,
    0,    0,  302,    0,    0,   43,   44,   45,   46,    0,
    0,    0,   47,    0,   48,    0,    0,    0,    0,  167,
    0,  168,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    2,    0,    4,    0,    0,    5,  137,    0,
    0,   51,    9,   52,    0,   53,   11,   54,    0,   55,
    0,   56,   12,  140,    0,   13,    0,    0,    0,    0,
    0,    0,   18,    0,    0,    0,    0,   20,    0,   21,
   22,   23,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   25,    0,   26,    0,    0,    0,   27,
    0,    0,    0,    0,   30,    0,    0,   32,   33,  138,
    0,   36,    0,    0,   38,    0,    0,    0,    0,    0,
    0,  367,    0,    0,    0,   41,    0,    0,    0,    0,
    0,   43,   44,   45,   46,    0,    0,    0,   47,    0,
   48,    0,    0,    0,    0,  167,    0,  168,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    2,    0,
    4,    0,    0,    5,  137,    0,    0,   51,    9,   52,
    0,   53,   11,   54,    0,   55,    0,   56,   12,  140,
    0,   13,    0,    0,    0,    0,    0,    0,   18,    0,
    0,    0,    0,   20,    0,   21,   22,   23,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   25,
    0,   26,    0,    0,    0,   27,    0,    0,    0,    0,
   30,    0,    0,   32,   33,  138,    0,   36,    0,    0,
   38,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   41,    0,    0,    0,    0,    0,   43,   44,   45,
   46,    0,    0,    0,   47,    0,   48,    0,    0,    0,
    0,  167,    0,  168,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    2,    0,    4,    0,    0,    5,
  137,    0,    0,   51,    9,   52,    0,   53,   11,   54,
    0,   55,    0,   56,   12,  140,    0,   13,    0,    0,
    0,    0,    0,    0,   18,    0,    0,    0,    0,   20,
    0,   21,   22,   23,    2,    0,    4,    0,    0,    5,
  137,    0,    0,    0,    9,   25,    0,   26,   11,    0,
    0,   27,    0,    0,   12,    0,   30,   13,    0,   32,
   33,  138,    0,   36,   18,    0,   38,    0,    0,   20,
    0,   21,   22,   23,    0,    0,    0,   41,    0,    0,
    0,    0,    0,   43,    0,   25,   46,   26,    0,    0,
    0,   27,    0,    0,    0,    0,   30,    0,    0,   32,
   33,  138,    0,   36,    0,    0,   38,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  139,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   51,
    0,   52,    0,   53,    0,   54,    0,   55,    0,   56,
    0,  140,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   51,
    0,   52,    0,   53,    0,   54,    0,   55,    0,   56,
    0,  140,
  };
  protected static  short [] yyCheck = {            66,
    0,    0,    6,    0,  154,    0,   37,  188,    0,    0,
   10,   10,  155,  268,  277,  294,  256,    0,    0,   10,
  256,  276,  203,  346,    7,    7,  355,   31,    0,  346,
   34,   35,  350,  348,  367,    0,  367,  353,  164,   21,
   40,   40,  268,  154,    0,   10,  353,  261,  346,   40,
  276,  349,  346,  256,  348,  386,  350,   40,   40,   41,
  186,  346,  348,  348,   10,  350,  269,  178,  346,  268,
    0,  349,  350,  348,  368,   40,  370,  276,  357,  372,
   10,  374,  344,  368,  298,  370,  348,  348,  344,  232,
  233,  424,  348,  347,  344,   10,  277,  351,  349,  422,
  351,  344,  353,  346,  422,  422,  346,  422,  348,  347,
   40,  440,  348,  351,  408,   60,   61,  298,  349,  345,
  399,  346,  353,  408,  349,   40,  230,  231,   43,   44,
   45,   46,   47,   48,   49,   50,  234,  235,  236,  237,
  350,  267,  257,  353,  350,  359,  360,  353,  363,  364,
  365,  380,  348,  382,  154,  154,  348,  355,  356,  348,
  423,  352,  376,  154,  378,  228,  229,  422,  238,  239,
  346,  154,  154,  155,  361,  350,  366,  265,  362,  267,
  361,  353,  270,  384,  353,  185,  185,  275,  422,  154,
  353,  279,  348,  185,  185,  178,  178,  197,  197,  348,
  288,  422,  185,  185,  348,  197,  197,  295,  339,  359,
  336,  337,  300,  353,  197,  197,  304,  353,  350,  353,
  185,  358,    0,  353,  154,  353,  345,  370,  316,  351,
  318,  358,  197,  351,  322,  349,  422,  422,  358,  154,
  422,  349,  330,  331,  349,  351,  334,  348,  351,  337,
  232,  233,  167,  168,  353,  185,  358,  353,  349,  348,
  257,  292,  257,  422,  269,  286,  349,  197,  349,  422,
  185,  347,  351,  351,  349,  352,  294,  349,  349,  344,
  349,    0,  197,  409,  434,  257,  388,  280,  390,  349,
  392,  353,  394,  351,  396,  344,  398,  353,  400,  345,
  402,  257,  404,  345,  406,  349,  349,  352,  349,  345,
  225,  226,  227,  228,  229,  230,  231,  352,    0,  234,
  235,  236,  237,  238,  239,  240,  241,  242,  243,  353,
  245,  353,  422,  286,  422,  345,  403,  353,  347,  353,
  353,  349,    0,  349,  347,  276,  344,  349,  309,  389,
  275,  345,  352,  352,  297,    0,  353,  353,  240,  359,
  359,  352,  349,  363,  363,  345,   24,  371,  359,  345,
  374,   29,  363,  345,  346,  347,  349,  349,  345,  351,
  352,  353,  241,   41,  299,  349,  242,  352,  370,  345,
  346,  347,  243,  349,  359,  351,  352,  353,  363,  245,
    7,  356,  351,   29,  443,  434,  352,  445,  445,    0,
  285,  367,  412,  412,  414,  414,   -1,  363,  418,  418,
   -1,  412,  352,  414,   -1,   -1,   -1,  418,  384,  359,
  386,   -1,   -1,  363,  434,  434,   -1,  352,   -1,   -1,
   -1,   -1,   -1,  434,  359,  445,  445,  412,  363,  414,
   -1,  451,  451,  418,  445,   -1,  456,  456,  458,  458,
  451,   -1,  445,  445,  456,  456,  412,  458,  414,  434,
   -1,   -1,  418,  456,  456,   -1,   -1,  135,   -1,  257,
  445,  139,  412,   -1,  414,   -1,  451,   -1,  418,   -1,
   -1,  456,   -1,  458,   -1,   -1,   -1,  412,  156,  414,
   -1,   -1,  160,  418,  434,  451,   -1,   -1,   -1,   -1,
   -1,   -1,  458,    0,   -1,  445,   -1,  175,   -1,  434,
  178,  451,   -1,   -1,   -1,   -1,  456,   -1,  458,   -1,
  445,   -1,   -1,   -1,   -1,   -1,  451,   -1,  257,   -1,
    0,  456,  261,  458,   -1,   -1,   -1,   -1,  206,  207,
   -1,   -1,   -1,   -1,   -1,   -1,  214,  215,  216,  217,
  218,  219,  220,  221,  222,  223,  224,  345,  346,  347,
   -1,  349,   -1,  351,  352,  353,   -1,   -1,   -1,  298,
   -1,   -1,   -1,  361,  362,   -1,  244,   -1,  366,  367,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  260,   -1,   -1,   -1,  384,   -1,  386,   -1,
   -1,   -1,  257,   -1,   -1,   -1,   -1,  275,  276,   -1,
   -1,   -1,   -1,  281,   -1,   -1,   -1,   -1,   -1,   -1,
  349,   -1,   -1,   -1,   -1,   -1,  355,  356,   -1,  358,
  359,  360,  361,  362,  363,  364,  365,  366,  367,   -1,
  308,  309,   -1,  372,   -1,  374,   -1,  376,   -1,  378,
   -1,  380,   -1,  382,   -1,  384,  257,  386,   -1,  388,
   -1,  390,   -1,  392,   -1,  394,   -1,  396,   -1,  398,
    0,  400,   -1,  402,   -1,  404,   -1,  406,   -1,   -1,
   -1,   -1,  350,   -1,   -1,   -1,   -1,  355,   -1,   -1,
  345,  346,  347,  422,  349,   -1,  351,  352,  353,  367,
   -1,   -1,   -1,   -1,   -1,   -1,  361,  362,   -1,    0,
   -1,  366,  367,   -1,   -1,   -1,  384,   -1,   -1,   -1,
   -1,  389,  390,   -1,   -1,  380,   -1,  382,   -1,  384,
   -1,  386,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  345,  346,  347,   -1,  349,    0,
  351,  352,  353,   -1,  422,   -1,  424,   -1,   -1,  256,
  257,   -1,   -1,   -1,   -1,   -1,  367,   -1,  265,  266,
  267,  268,  440,  270,  271,   -1,  273,  274,  275,  276,
   -1,  278,  279,  280,   -1,  386,   -1,  257,  285,  286,
   -1,  288,  289,  290,  291,  292,   -1,   -1,  295,   -1,
   -1,   -1,  299,  300,   -1,  302,  303,  304,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  314,   -1,  316,
   -1,  318,   -1,   -1,   -1,  322,   -1,  324,    0,  326,
  327,  328,   -1,  330,  331,  332,  333,  334,  335,   -1,
  337,   -1,  339,   -1,   -1,   -1,   -1,  344,  345,   -1,
   -1,  348,   -1,   -1,   -1,   -1,  353,  354,  355,  356,
  357,   -1,   -1,   -1,  361,   -1,  363,   -1,   -1,   -1,
   -1,  368,   -1,  370,   -1,  345,  346,  347,   -1,  349,
   -1,  351,  352,  353,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  361,  362,   -1,   -1,   -1,  366,  367,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,
  380,   -1,  382,  410,  384,  412,  386,  414,   -1,  416,
   -1,  418,   -1,  420,   -1,  422,  256,  257,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  265,  266,  267,  268,   -1,
  270,  271,   -1,  273,  274,  275,  276,   -1,  278,  279,
   -1,   -1,   -1,   -1,   -1,  285,   -1,   -1,  288,  289,
  290,  291,  292,   -1,   -1,  295,  257,   -1,   -1,  299,
  300,   -1,  302,  303,  304,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,  314,   -1,  316,   -1,  318,   -1,
   -1,   -1,  322,   -1,  324,   -1,  326,  327,  328,   -1,
  330,  331,  332,  333,  334,  335,  257,  337,   -1,  339,
  261,   -1,   -1,   -1,  344,  345,   -1,   -1,  348,   -1,
   -1,   -1,   -1,  353,  354,  355,  356,  357,   -1,   -1,
   -1,  361,   -1,  363,   -1,   -1,   -1,   -1,  368,   -1,
  370,   -1,   -1,   -1,   -1,   -1,   -1,  298,   -1,   -1,
   -1,   -1,   -1,   -1,  345,  346,  347,    0,  349,   -1,
  351,  352,  353,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  367,   -1,   -1,   -1,
  410,   -1,  412,   -1,  414,  257,  416,   -1,  418,  261,
  420,   -1,  422,  344,  345,  386,  347,  348,  349,  350,
  351,  352,  353,   -1,  355,  356,   -1,  358,  359,  360,
  361,  362,  363,  364,  365,  366,  367,  368,   -1,  370,
   -1,  372,   -1,  374,   -1,  376,  298,  378,   -1,  380,
   -1,  382,    0,  384,   -1,  386,   -1,  388,   -1,  390,
   -1,  392,   -1,  394,   -1,  396,   -1,  398,   -1,  400,
   -1,  402,   -1,  404,   -1,  406,   -1,  408,   -1,   -1,
   -1,   -1,   -1,   -1,  257,   -1,   -1,   -1,  261,   -1,
   -1,  422,  344,  345,   -1,  347,  348,  349,  350,  351,
  352,  353,   -1,  355,  356,   -1,  358,  359,  360,  361,
  362,  363,  364,  365,  366,  367,  368,   -1,  370,   -1,
  372,   -1,  374,   -1,  376,  298,  378,    0,  380,   -1,
  382,   -1,  384,   -1,  386,   -1,  388,   -1,  390,   -1,
  392,   -1,  394,   -1,  396,   -1,  398,   -1,  400,   -1,
  402,   -1,  404,   -1,  406,   -1,  408,   -1,   -1,  257,
   -1,   -1,   -1,  261,   -1,   -1,   -1,   -1,   -1,   -1,
  422,   -1,  345,  346,  347,  348,  349,  350,  351,  352,
  353,   -1,  355,  356,   -1,  358,  359,  360,  361,  362,
  363,  364,  365,  366,  367,  368,   -1,  370,    0,  372,
  298,  374,   -1,  376,   -1,  378,   -1,  380,   -1,  382,
   -1,  384,   -1,  386,   -1,  388,   -1,  390,   -1,  392,
   -1,  394,   -1,  396,   -1,  398,   -1,  400,   -1,  402,
   -1,  404,   -1,  406,  257,  408,   -1,   -1,  261,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  346,  422,
  348,  349,  350,  351,   -1,  353,   -1,  355,  356,   -1,
  358,  359,  360,  361,  362,  363,  364,  365,  366,  367,
  368,   -1,  370,   -1,  372,  298,  374,   -1,  376,   -1,
  378,    0,  380,   -1,  382,   -1,  384,   -1,  386,   -1,
  388,   -1,  390,   -1,  392,   -1,  394,   -1,  396,   -1,
  398,   -1,  400,   -1,  402,   -1,  404,   -1,  406,  257,
  408,   -1,   -1,  261,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  346,  422,  348,  349,  350,  351,   -1,
  353,   -1,  355,  356,   -1,  358,  359,  360,  361,  362,
  363,  364,  365,  366,  367,  368,    0,  370,   -1,  372,
  298,  374,   -1,  376,   -1,  378,   -1,  380,   -1,  382,
   -1,  384,   -1,  386,   -1,  388,   -1,  390,   -1,  392,
   -1,  394,   -1,  396,   -1,  398,   -1,  400,   -1,  402,
   -1,  404,   -1,  406,  257,  408,   -1,   -1,  261,    0,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  346,  422,
  348,  349,  350,  351,   -1,  353,   -1,  355,  356,   -1,
  358,  359,  360,  361,  362,  363,  364,  365,  366,  367,
  368,   -1,  370,   -1,  372,  298,  374,   -1,  376,   -1,
  378,   -1,  380,   -1,  382,   -1,  384,   -1,  386,   -1,
  388,   -1,  390,   -1,  392,   -1,  394,   -1,  396,   -1,
  398,   -1,  400,   -1,  402,  257,  404,   -1,  406,  261,
  408,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  346,  422,  348,  349,  350,  351,   -1,
  353,   -1,  355,  356,   -1,  358,  359,  360,  361,  362,
  363,  364,  365,  366,  367,  368,  298,  370,   -1,  372,
   -1,  374,   -1,  376,   -1,  378,   -1,  380,   -1,  382,
   -1,  384,   -1,  386,   -1,  388,   -1,  390,   -1,  392,
   -1,  394,   -1,  396,   -1,  398,   -1,  400,   -1,  402,
   -1,  404,   -1,  406,   -1,  408,   -1,  256,  257,    0,
   -1,   -1,  261,   -1,  346,   -1,  348,   -1,  350,  422,
   -1,   -1,   -1,  355,  356,   -1,  358,  359,  360,  361,
  362,  363,  364,  365,  366,  367,  368,   -1,  370,   -1,
  372,   -1,  374,   -1,  376,   -1,  378,   -1,  380,  298,
  382,   -1,  384,   -1,  386,   -1,  388,   -1,  390,   -1,
  392,   -1,  394,   -1,  396,   -1,  398,   -1,  400,   -1,
  402,   -1,  404,  257,  406,   -1,  408,  261,   -1,   -1,
    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  422,   -1,   -1,   -1,   -1,   -1,  345,  346,  347,  348,
  349,   -1,  351,  352,  353,   -1,   -1,   -1,   -1,   -1,
  359,  360,  361,  362,  298,   -1,  257,  366,  367,   -1,
  261,   -1,   -1,   -1,   -1,   -1,   -1,  376,   -1,  378,
   -1,  380,   -1,  382,   -1,  384,   -1,  386,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,    0,   -1,   -1,   -1,   -1,   -1,  298,   -1,   -1,
   -1,  345,   -1,  347,   -1,  349,   -1,  351,  352,  353,
   -1,   -1,   -1,  422,   -1,  359,  360,  361,  362,   -1,
   -1,   -1,  366,  367,   -1,   -1,   -1,   -1,   -1,  257,
   -1,   -1,  376,  261,  378,   -1,  380,   -1,  382,   -1,
  384,   -1,  386,   -1,  345,  346,  347,   -1,  349,  350,
  351,  352,  353,   -1,  355,  356,   -1,  358,  359,  360,
  361,  362,  363,  364,  365,  366,  367,  368,    0,  370,
  298,  372,   -1,  374,   -1,  376,   -1,  378,  422,  380,
   -1,  382,   -1,  384,   -1,  386,   -1,  388,   -1,  390,
   -1,  392,   -1,  394,   -1,  396,   -1,  398,   -1,  400,
   -1,  402,   -1,  404,   -1,  406,  257,  408,   -1,   -1,
  261,    0,   -1,   -1,   -1,   -1,   -1,  345,   -1,  347,
   -1,  349,   -1,  351,  352,  353,   -1,  355,  356,   -1,
  358,  359,  360,  361,  362,  363,  364,  365,  366,  367,
   -1,   -1,   -1,   -1,  372,   -1,  374,  298,  376,   -1,
  378,   -1,  380,   -1,  382,    0,  384,   -1,  386,   -1,
  388,   -1,  390,   -1,  392,   -1,  394,   -1,  396,   -1,
  398,   -1,  400,   -1,  402,   -1,  404,  257,  406,   -1,
   -1,  261,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  349,   -1,
  351,   -1,  353,   -1,  355,  356,   -1,  358,  359,  360,
  361,  362,  363,  364,  365,  366,  367,   -1,  298,   -1,
   -1,  372,   -1,  374,   -1,  376,   -1,  378,   -1,  380,
   -1,  382,   -1,  384,   -1,  386,   -1,  388,   -1,  390,
   -1,  392,    0,  394,   -1,  396,   -1,  398,  257,  400,
   -1,  402,  261,  404,   -1,  406,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  349,
   -1,  351,   -1,  353,   -1,  355,  356,   -1,  358,  359,
  360,  361,  362,  363,  364,  365,  366,  367,    0,  298,
   -1,   -1,  372,   -1,  374,   -1,  376,   -1,  378,   -1,
  380,   -1,  382,   -1,  384,   -1,  386,   -1,  388,   -1,
  390,   -1,  392,   -1,  394,   -1,  396,   -1,  398,   -1,
  400,   -1,  402,   -1,  404,  257,  406,   -1,   -1,  261,
   -1,   -1,   -1,   -1,   -1,   -1,  345,  346,  347,   -1,
  349,   -1,  351,  352,  353,   -1,  355,  356,   -1,   -1,
  359,  360,  361,  362,  363,  364,  365,  366,  367,   -1,
    0,   -1,   -1,  372,   -1,  374,  298,  376,  257,  378,
   -1,  380,  261,  382,   -1,  384,   -1,  386,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,  298,
   -1,   -1,  257,  345,  346,  347,  261,  349,   -1,  351,
  352,  353,   -1,  355,  356,   -1,   -1,  359,  360,  361,
  362,   -1,   -1,   -1,  366,  367,   -1,   -1,   -1,   -1,
  372,   -1,  374,   -1,  376,   -1,  378,    0,  380,   -1,
  382,   -1,  384,  298,  386,   -1,  345,  346,  347,   -1,
  349,   -1,  351,  352,  353,   -1,  355,  356,   -1,   -1,
  359,  360,  361,  362,   -1,   -1,   -1,  366,  367,   -1,
   -1,   -1,   -1,  372,   -1,  374,   -1,  376,   -1,  378,
   -1,  380,   -1,  382,   -1,  384,   -1,  386,   -1,  257,
  345,  346,  347,  261,  349,   -1,  351,  352,  353,   -1,
  355,  356,   -1,    0,  359,  360,  361,  362,   -1,   -1,
   -1,  366,  367,   -1,   -1,   -1,   -1,  372,   -1,  374,
   -1,  376,   -1,  378,   -1,  380,   -1,  382,   -1,  384,
  298,  386,   -1,   -1,   -1,  257,   -1,   -1,   -1,  261,
   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  298,  345,  346,  347,
   -1,  349,   -1,  351,  352,  353,   -1,   -1,   -1,   -1,
    0,  359,  360,  361,  362,   -1,   -1,   -1,  366,  367,
   -1,   -1,   -1,   -1,  372,   -1,  374,  257,  376,   -1,
  378,  261,  380,   -1,  382,   -1,  384,   -1,  386,   -1,
   -1,   -1,   -1,  345,  346,  347,   -1,  349,   -1,  351,
  352,  353,   -1,   -1,   -1,   -1,   -1,  359,  360,  361,
  362,   -1,    0,   -1,  366,  367,   -1,   -1,  298,  257,
  372,   -1,  374,  261,  376,   -1,  378,   -1,  380,   -1,
  382,   -1,  384,   -1,  386,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,
  298,   -1,   -1,   -1,  257,  345,  346,  347,  261,  349,
   -1,  351,  352,  353,   -1,   -1,   -1,   -1,   -1,  359,
  360,  361,  362,   -1,   -1,   -1,  366,  367,   -1,   -1,
   -1,   -1,  372,   -1,  374,   -1,  376,   -1,  378,    0,
  380,   -1,  382,   -1,  384,  298,  386,  345,  346,  347,
   -1,  349,   -1,  351,  352,  353,   -1,   -1,   -1,   -1,
   -1,  359,  360,  361,  362,    0,   -1,   -1,  366,  367,
  257,   -1,   -1,   -1,  261,   -1,   -1,   -1,  376,   -1,
  378,   -1,  380,   -1,  382,   -1,  384,   -1,  386,   -1,
    0,   -1,  345,  346,  347,   -1,  349,   -1,  351,  352,
  353,   -1,   -1,   -1,   -1,   -1,  359,  360,  361,  362,
   -1,  298,  257,  366,  367,    0,  261,   -1,   -1,   -1,
   -1,   -1,   -1,  376,   -1,  378,   -1,  380,   -1,  382,
   -1,  384,   -1,  386,   -1,   -1,   -1,   -1,    0,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  298,   -1,   -1,   -1,  257,  345,  346,
  347,  261,  349,    0,  351,  352,  353,   -1,   -1,   -1,
   -1,   -1,  359,  360,  361,  362,   -1,   -1,   -1,  366,
  367,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,  376,
   -1,  378,   -1,  380,   -1,  382,   -1,  384,  298,  386,
  345,  346,  347,   -1,  349,   -1,  351,  352,  353,  257,
   -1,   -1,   -1,  261,  359,  360,  361,  362,   -1,   -1,
   -1,  366,  367,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  376,   -1,  378,   -1,  380,   -1,  382,   -1,  384,
   -1,  386,   -1,   -1,   -1,  345,  346,  347,   -1,  349,
  298,  351,  352,  353,  257,   -1,   -1,   -1,  261,  359,
  360,  361,  362,   -1,   -1,   -1,  366,  367,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  376,   -1,  378,   -1,
  380,   -1,  382,   -1,  384,   -1,  386,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  298,  257,  345,   -1,  347,
   -1,  349,   -1,  351,  352,  353,   -1,   -1,   -1,   -1,
   -1,  359,  360,  361,  362,   -1,   -1,   -1,  366,  367,
   -1,   -1,  257,   -1,   -1,   -1,   -1,   -1,  376,   -1,
  378,   -1,  380,   -1,  382,   -1,  384,   -1,  386,   -1,
   -1,   -1,  345,   -1,  347,   -1,  349,  257,  351,  352,
  353,   -1,   -1,   -1,   -1,   -1,  359,  360,  361,  362,
   -1,   -1,   -1,  366,  367,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  257,  376,   -1,  378,   -1,  380,   -1,  382,
   -1,  384,   -1,  386,  345,  346,  347,   -1,  349,   -1,
  351,  352,  353,   -1,   -1,  257,   -1,   -1,   -1,   -1,
  361,  362,   -1,   -1,   -1,  366,  367,   -1,   -1,   -1,
  345,  346,  347,   -1,  349,   -1,  351,  352,  353,  380,
  257,  382,   -1,  384,   -1,  386,  361,  362,   -1,   -1,
   -1,  366,  367,   -1,   -1,  345,  346,  347,   -1,  349,
   -1,  351,  352,  353,  257,   -1,   -1,   -1,   -1,  384,
   -1,  386,  362,   -1,   -1,   -1,  366,  367,   -1,   -1,
  345,  346,  347,   -1,  349,   -1,  351,  352,  353,   -1,
   -1,   -1,   -1,   -1,  384,   -1,  386,  362,   -1,   -1,
   -1,  366,  367,  345,  346,  347,   -1,  349,   -1,  351,
  352,  353,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  384,
  362,  386,   -1,   -1,   -1,  367,   -1,   -1,  345,  346,
  347,   -1,  349,   -1,  351,  352,  353,   -1,   -1,   -1,
   -1,   -1,  384,   -1,  386,  362,   -1,   -1,   -1,   -1,
  367,   -1,  345,  346,  347,   -1,  349,   -1,  351,  352,
  353,   -1,   -1,   -1,   -1,   -1,  256,  384,   -1,  386,
   -1,   -1,   -1,   -1,  367,  265,  266,  267,  268,   -1,
  270,  271,   -1,  273,  274,  275,  276,   -1,  278,  279,
   -1,  384,   -1,  386,   -1,  285,   -1,   -1,  288,  289,
  290,  291,  292,   -1,   -1,  295,   -1,   -1,   -1,  299,
  300,   -1,  302,  303,  304,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  314,   -1,  316,   -1,  318,   -1,
   -1,   -1,  322,   -1,  324,   -1,  326,  327,  328,   -1,
  330,  331,  332,  333,  334,  335,   -1,  337,   -1,  339,
   -1,   -1,   -1,   -1,  344,   -1,   -1,   -1,  348,   -1,
   -1,   -1,   -1,  353,  354,  355,  356,  357,   -1,   -1,
   -1,  361,   -1,  363,   -1,   -1,   -1,   -1,  368,   -1,
  370,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  410,   -1,  412,   -1,  414,  256,  416,   -1,  418,   -1,
  420,   -1,  422,   -1,  265,  266,  267,   -1,   -1,  270,
  271,   -1,  273,  274,  275,   -1,   -1,  278,  279,   -1,
   -1,   -1,   -1,   -1,  285,   -1,   -1,  288,  289,  290,
  291,  292,   -1,   -1,  295,   -1,   -1,   -1,  299,  300,
   -1,  302,  303,  304,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  314,   -1,  316,   -1,  318,   -1,   -1,
   -1,  322,   -1,  324,   -1,  326,  327,  328,   -1,  330,
  331,  332,  333,  334,  335,   -1,  337,   -1,  339,   -1,
   -1,   -1,   -1,  344,   -1,   -1,   -1,  348,   -1,   -1,
   -1,   -1,  353,  354,  355,  356,  357,   -1,   -1,   -1,
  361,   -1,  363,   -1,   -1,   -1,   -1,  368,   -1,  370,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  410,
   -1,  412,   -1,  414,  256,  416,   -1,  418,   -1,  420,
   -1,  422,   -1,  265,  266,  267,   -1,   -1,  270,  271,
   -1,  273,  274,  275,   -1,   -1,  278,  279,   -1,   -1,
   -1,   -1,   -1,  285,   -1,   -1,  288,  289,  290,  291,
  292,   -1,   -1,  295,   -1,   -1,   -1,  299,  300,   -1,
  302,  303,  304,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  314,   -1,  316,   -1,  318,   -1,   -1,   -1,
  322,   -1,  324,   -1,  326,  327,  328,   -1,  330,  331,
  332,  333,  334,  335,   -1,  337,   -1,  339,   -1,   -1,
   -1,   -1,  344,   -1,   -1,   -1,  348,   -1,   -1,   -1,
   -1,  353,  354,  355,  356,  357,   -1,   -1,   -1,  361,
   -1,  363,   -1,   -1,   -1,   -1,  368,   -1,  370,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  410,   -1,
  412,   -1,  414,  256,  416,   -1,  418,   -1,  420,   -1,
  422,   -1,  265,  266,  267,   -1,   -1,  270,  271,   -1,
   -1,  274,  275,   -1,   -1,  278,  279,   -1,   -1,   -1,
   -1,   -1,  285,   -1,   -1,  288,  289,  290,  291,  292,
   -1,   -1,  295,   -1,   -1,   -1,  299,  300,   -1,  302,
  303,  304,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  314,   -1,  316,   -1,  318,   -1,   -1,   -1,  322,
   -1,  324,   -1,  326,  327,  328,   -1,  330,  331,  332,
  333,  334,  335,   -1,  337,   -1,  339,   -1,   -1,   -1,
   -1,  344,   -1,   -1,   -1,  348,   -1,   -1,   -1,   -1,
  353,  354,  355,  356,  357,   -1,   -1,   -1,  361,   -1,
  363,   -1,   -1,   -1,   -1,  368,   -1,  370,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  256,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  265,   -1,
  267,   -1,   -1,  270,  271,   -1,   -1,  410,  275,  412,
   -1,  414,  279,  416,   -1,  418,   -1,  420,  285,  422,
   -1,  288,   -1,   -1,   -1,   -1,   -1,   -1,  295,   -1,
   -1,   -1,   -1,  300,   -1,  302,  303,  304,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  316,
   -1,  318,   -1,   -1,   -1,  322,   -1,   -1,   -1,   -1,
  327,   -1,   -1,  330,  331,  332,   -1,  334,   -1,   -1,
  337,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,   -1,   -1,   -1,   -1,   -1,  354,  355,  356,
  357,   -1,   -1,   -1,  361,   -1,  363,   -1,   -1,   -1,
   -1,  368,   -1,  370,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  265,   -1,  267,   -1,   -1,  270,
  271,   -1,   -1,  410,  275,  412,   -1,  414,  279,  416,
   -1,  418,   -1,  420,  285,  422,   -1,  288,   -1,   -1,
   -1,   -1,   -1,   -1,  295,   -1,   -1,   -1,   -1,  300,
   -1,  302,  303,  304,   -1,  306,   -1,   -1,   -1,   -1,
   -1,   -1,  313,   -1,   -1,  316,   -1,  318,   -1,   -1,
   -1,  322,   -1,   -1,   -1,   -1,  327,   -1,   -1,  330,
  331,  332,   -1,  334,   -1,   -1,  337,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,   -1,   -1,
   -1,   -1,   -1,  354,  355,  356,  357,   -1,   -1,   -1,
  361,   -1,  363,   -1,   -1,   -1,   -1,  368,   -1,  370,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  265,   -1,  267,   -1,   -1,  270,  271,   -1,   -1,  410,
  275,  412,   -1,  414,  279,  416,   -1,  418,   -1,  420,
  285,  422,   -1,  288,   -1,   -1,   -1,   -1,   -1,   -1,
  295,   -1,   -1,   -1,   -1,  300,   -1,  302,  303,  304,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  316,   -1,  318,   -1,   -1,   -1,  322,   -1,   -1,
   -1,   -1,  327,   -1,   -1,  330,  331,  332,   -1,  334,
   -1,   -1,  337,   -1,   -1,   -1,   -1,   -1,   -1,  344,
  345,   -1,   -1,  348,   -1,   -1,   -1,   -1,   -1,  354,
  355,  356,  357,   -1,   -1,   -1,  361,   -1,  363,   -1,
   -1,   -1,   -1,  368,   -1,  370,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  265,   -1,  267,   -1,
   -1,  270,  271,   -1,   -1,  410,  275,  412,   -1,  414,
  279,  416,   -1,  418,   -1,  420,  285,  422,   -1,  288,
   -1,   -1,   -1,   -1,   -1,   -1,  295,   -1,   -1,   -1,
   -1,  300,   -1,  302,  303,  304,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  316,   -1,  318,
   -1,   -1,   -1,  322,   -1,   -1,   -1,   -1,  327,   -1,
   -1,  330,  331,  332,   -1,  334,   -1,   -1,  337,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
   -1,   -1,  351,   -1,   -1,  354,  355,  356,  357,   -1,
   -1,   -1,  361,   -1,  363,   -1,   -1,   -1,   -1,  368,
   -1,  370,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  265,   -1,  267,   -1,   -1,  270,  271,   -1,
   -1,  410,  275,  412,   -1,  414,  279,  416,   -1,  418,
   -1,  420,  285,  422,   -1,  288,   -1,   -1,   -1,   -1,
   -1,   -1,  295,   -1,   -1,   -1,   -1,  300,   -1,  302,
  303,  304,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  316,   -1,  318,   -1,   -1,   -1,  322,
   -1,   -1,   -1,   -1,  327,   -1,   -1,  330,  331,  332,
   -1,  334,   -1,   -1,  337,   -1,   -1,   -1,   -1,   -1,
   -1,  344,   -1,   -1,   -1,  348,   -1,   -1,   -1,   -1,
   -1,  354,  355,  356,  357,   -1,   -1,   -1,  361,   -1,
  363,   -1,   -1,   -1,   -1,  368,   -1,  370,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  265,   -1,
  267,   -1,   -1,  270,  271,   -1,   -1,  410,  275,  412,
   -1,  414,  279,  416,   -1,  418,   -1,  420,  285,  422,
   -1,  288,   -1,   -1,   -1,   -1,   -1,   -1,  295,   -1,
   -1,   -1,   -1,  300,   -1,  302,  303,  304,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  316,
   -1,  318,   -1,   -1,   -1,  322,   -1,   -1,   -1,   -1,
  327,   -1,   -1,  330,  331,  332,   -1,  334,   -1,   -1,
  337,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,   -1,   -1,   -1,   -1,   -1,  354,  355,  356,
  357,   -1,   -1,   -1,  361,   -1,  363,   -1,   -1,   -1,
   -1,  368,   -1,  370,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  265,   -1,  267,   -1,   -1,  270,
  271,   -1,   -1,  410,  275,  412,   -1,  414,  279,  416,
   -1,  418,   -1,  420,  285,  422,   -1,  288,   -1,   -1,
   -1,   -1,   -1,   -1,  295,   -1,   -1,   -1,   -1,  300,
   -1,  302,  303,  304,  265,   -1,  267,   -1,   -1,  270,
  271,   -1,   -1,   -1,  275,  316,   -1,  318,  279,   -1,
   -1,  322,   -1,   -1,  285,   -1,  327,  288,   -1,  330,
  331,  332,   -1,  334,  295,   -1,  337,   -1,   -1,  300,
   -1,  302,  303,  304,   -1,   -1,   -1,  348,   -1,   -1,
   -1,   -1,   -1,  354,   -1,  316,  357,  318,   -1,   -1,
   -1,  322,   -1,   -1,   -1,   -1,  327,   -1,   -1,  330,
  331,  332,   -1,  334,   -1,   -1,  337,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  410,
   -1,  412,   -1,  414,   -1,  416,   -1,  418,   -1,  420,
   -1,  422,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  410,
   -1,  412,   -1,  414,   -1,  416,   -1,  418,   -1,  420,
   -1,  422,
  };

#line 1844 "cs-parser.jay"

Expression DecomposeQI (string name, Location loc)
{
	Expression o;

	if (name.IndexOf ('.') == -1){
			return new SimpleName (name, loc);
	} else {
		int pos = name.LastIndexOf (".");
		string left = name.Substring (0, pos);
		string right = name.Substring (pos + 1);

		o = DecomposeQI (left, loc);
		return new MemberAccess (o, right, loc);
	}
}

// <summary>
//  This method is used to get at the complete string representation of
//  a fully-qualified type name, hiding inside a MemberAccess ;-)
//  This is necessary because local_variable_type admits primary_expression
//  as the type of the variable. So we do some extra checking
// </summary>
string GetQualifiedIdentifier (Expression expr)
{
	if (expr is SimpleName)
		return ((SimpleName)expr).Name;
	else if (expr is MemberAccess)
		return GetQualifiedIdentifier (((MemberAccess)expr).Expr) + "." + ((MemberAccess) expr).Ident;
	else 
		throw new Exception ("Expr has to be either SimpleName or MemberAccess! (" + expr + ")");
	
}

void note (string s)
{
	Console.WriteLine("NOTE:" + s ); // Used to put annotations
}

Tokenizer lexer;

public Tokenizer Lexer {
	get {
		return lexer;
	}
}		   


public CSharpParser (string name, System.IO.TextReader input)
{
	this.name = name;
	this.input = input;
	oob_stack = new Stack ();

	lexer = new Tokenizer (input, name);
}

public void Reset(System.IO.TextReader input) {
	this.input = input;
	oob_stack = new Stack();
	
	lexer.Reset(input);
}

public int parse ()
{
	//global_errors = 0;
	try {
		if (yacc_verbose_flag)
			yyparse (lexer, new yydebug.yyDebugSimple ());
		else
			yyparse (lexer);
	} catch (Exception){
		// Console.WriteLine ("Fatal error: " + name);
		// Console.WriteLine (lexer.location);

		// 
		// Please do not remove this, it is used during debugging
		// of the grammar
		//
		//Console.WriteLine (lexer.location + "  : Parsing error ");
		//Console.WriteLine (e);
		//global_errors++;
		throw;
	}
	return 0;
	//return global_errors;
}

}
#line 3535 "-"
namespace yydebug {
        using System;
	 public interface yyDebug {
		 void push (int state, Object value);
		 void lex (int state, int token, string name, Object value);
		 void shift (int from, int to, int errorFlag);
		 void pop (int state);
		 void discard (int state, int token, string name, Object value);
		 void reduce (int from, int to, int rule, string text, int len);
		 void shift (int from, int to);
		 void accept (Object value);
		 void error (string message);
		 void reject ();
	 }
	 
	 class yyDebugSimple : yyDebug {
		 void println (string s){
			 Console.WriteLine (s);
		 }
		 
		 public void push (int state, Object value) {
			 println ("push\tstate "+state+"\tvalue "+value);
		 }
		 
		 public void lex (int state, int token, string name, Object value) {
			 println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		 }
		 
		 public void shift (int from, int to, int errorFlag) {
			 switch (errorFlag) {
			 default:				// normally
				 println("shift\tfrom state "+from+" to "+to);
				 break;
			 case 0: case 1: case 2:		// in error recovery
				 println("shift\tfrom state "+from+" to "+to
					     +"\t"+errorFlag+" left to recover");
				 break;
			 case 3:				// normally
				 println("shift\tfrom state "+from+" to "+to+"\ton error");
				 break;
			 }
		 }
		 
		 public void pop (int state) {
			 println("pop\tstate "+state+"\ton error");
		 }
		 
		 public void discard (int state, int token, string name, Object value) {
			 println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		 }
		 
		 public void reduce (int from, int to, int rule, string text, int len) {
			 println("reduce\tstate "+from+"\tuncover "+to
				     +"\trule ("+rule+") "+text);
		 }
		 
		 public void shift (int from, int to) {
			 println("goto\tfrom state "+from+" to "+to);
		 }
		 
		 public void accept (Object value) {
			 println("accept\tvalue "+value);
		 }
		 
		 public void error (string message) {
			 println("error\t"+message);
		 }
		 
		 public void reject () {
			 println("reject");
		 }
		 
	 }
}
// %token constants
 class Token {
  public const int EOF = 257;
  public const int NONE = 258;
  public const int ERROR = 259;
  public const int ABSTRACT = 260;
  public const int AS = 261;
  public const int ADD = 262;
  public const int ASSEMBLY = 263;
  public const int BASE = 264;
  public const int BOOL = 265;
  public const int BREAK = 266;
  public const int BYTE = 267;
  public const int CASE = 268;
  public const int CATCH = 269;
  public const int CHAR = 270;
  public const int CHECKED = 271;
  public const int CLASS = 272;
  public const int CONST = 273;
  public const int CONTINUE = 274;
  public const int DECIMAL = 275;
  public const int DEFAULT = 276;
  public const int DELEGATE = 277;
  public const int DO = 278;
  public const int DOUBLE = 279;
  public const int ELSE = 280;
  public const int ENUM = 281;
  public const int EVENT = 282;
  public const int EXPLICIT = 283;
  public const int EXTERN = 284;
  public const int FALSE = 285;
  public const int FINALLY = 286;
  public const int FIXED = 287;
  public const int FLOAT = 288;
  public const int FOR = 289;
  public const int FOREACH = 290;
  public const int GOTO = 291;
  public const int IF = 292;
  public const int IMPLICIT = 293;
  public const int IN = 294;
  public const int INT = 295;
  public const int INTERFACE = 296;
  public const int INTERNAL = 297;
  public const int IS = 298;
  public const int LOCK = 299;
  public const int LONG = 300;
  public const int NAMESPACE = 301;
  public const int NEW = 302;
  public const int NULL = 303;
  public const int OBJECT = 304;
  public const int OPERATOR = 305;
  public const int OUT = 306;
  public const int OVERRIDE = 307;
  public const int PARAMS = 308;
  public const int PRIVATE = 309;
  public const int PROTECTED = 310;
  public const int PUBLIC = 311;
  public const int READONLY = 312;
  public const int REF = 313;
  public const int RETURN = 314;
  public const int REMOVE = 315;
  public const int SBYTE = 316;
  public const int SEALED = 317;
  public const int SHORT = 318;
  public const int SIZEOF = 319;
  public const int STACKALLOC = 320;
  public const int STATIC = 321;
  public const int STRING = 322;
  public const int STRUCT = 323;
  public const int SWITCH = 324;
  public const int THIS = 325;
  public const int THROW = 326;
  public const int TRUE = 327;
  public const int TRY = 328;
  public const int TYPEOF = 329;
  public const int UINT = 330;
  public const int ULONG = 331;
  public const int UNCHECKED = 332;
  public const int UNSAFE = 333;
  public const int USHORT = 334;
  public const int USING = 335;
  public const int VIRTUAL = 336;
  public const int VOID = 337;
  public const int VOLATILE = 338;
  public const int WHILE = 339;
  public const int GET = 340;
  public const int get = 341;
  public const int SET = 342;
  public const int set = 343;
  public const int OPEN_BRACE = 344;
  public const int CLOSE_BRACE = 345;
  public const int OPEN_BRACKET = 346;
  public const int CLOSE_BRACKET = 347;
  public const int OPEN_PARENS = 348;
  public const int CLOSE_PARENS = 349;
  public const int DOT = 350;
  public const int COMMA = 351;
  public const int COLON = 352;
  public const int SEMICOLON = 353;
  public const int TILDE = 354;
  public const int PLUS = 355;
  public const int MINUS = 356;
  public const int BANG = 357;
  public const int ASSIGN = 358;
  public const int OP_LT = 359;
  public const int OP_GT = 360;
  public const int BITWISE_AND = 361;
  public const int BITWISE_OR = 362;
  public const int STAR = 363;
  public const int PERCENT = 364;
  public const int DIV = 365;
  public const int CARRET = 366;
  public const int INTERR = 367;
  public const int OP_INC = 368;
  public const int OP_DEC = 370;
  public const int OP_SHIFT_LEFT = 372;
  public const int OP_SHIFT_RIGHT = 374;
  public const int OP_LE = 376;
  public const int OP_GE = 378;
  public const int OP_EQ = 380;
  public const int OP_NE = 382;
  public const int OP_AND = 384;
  public const int OP_OR = 386;
  public const int OP_MULT_ASSIGN = 388;
  public const int OP_DIV_ASSIGN = 390;
  public const int OP_MOD_ASSIGN = 392;
  public const int OP_ADD_ASSIGN = 394;
  public const int OP_SUB_ASSIGN = 396;
  public const int OP_SHIFT_LEFT_ASSIGN = 398;
  public const int OP_SHIFT_RIGHT_ASSIGN = 400;
  public const int OP_AND_ASSIGN = 402;
  public const int OP_XOR_ASSIGN = 404;
  public const int OP_OR_ASSIGN = 406;
  public const int OP_PTR = 408;
  public const int LITERAL_INTEGER = 410;
  public const int LITERAL_FLOAT = 412;
  public const int LITERAL_DOUBLE = 414;
  public const int LITERAL_DECIMAL = 416;
  public const int LITERAL_CHARACTER = 418;
  public const int LITERAL_STRING = 420;
  public const int IDENTIFIER = 422;
  public const int LOWPREC = 423;
  public const int UMINUS = 424;
  public const int HIGHPREC = 425;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  public class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  public interface yyInput {
    /** move on to next token.
        @return false if positioned beyond tokens.
        @throws IOException on input error.
      */
    bool advance (); // throws java.io.IOException;
    /** classifies current token.
        Should not be called if advance() returned false.
        @return current %token or single character.
      */
    int token ();
    /** associated with current token.
        Should not be called if advance() returned false.
        @return value for token().
      */
    Object value ();
  }
 }
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog
