// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 2 "CSharpExpressionParser.jay"
using System.Text;
using System.IO;
using System.Collections;
using System;

namespace Debugger.Frontend
{
#if NET_2_0
	public class CSharpExpressionParser
	{
		EvaluationContext current_context;
		MyTextReader reader;
		Tokenizer lexer;

		protected bool yacc_verbose_flag = false;

		public bool Verbose {
			set {
				yacc_verbose_flag = value;
			}

			get {
				return yacc_verbose_flag;
			}
		}

#line default

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
      System.Console.Write (message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        System.Console.Write (" "+expected[n]);
        System.Console.WriteLine ();
    } else
      System.Console.WriteLine (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
  internal yydebug.yyDebug debug;

  protected static  int yyFinal = 13;
  public static  string [] yyRule = {
    "$accept : parse_expression",
    "parse_expression : primary_expression",
    "primary_expression : expression",
    "primary_expression : expression ASSIGN expression",
    "expression : NUMBER",
    "expression : INTEGER",
    "expression : STRING",
    "expression : THIS",
    "expression : CATCH",
    "expression : BASE DOTDOT IDENTIFIER",
    "expression : BASE DOT IDENTIFIER",
    "expression : variable_or_type_name",
    "expression : PERCENT IDENTIFIER",
    "expression : STAR expression",
    "expression : AMPERSAND expression",
    "expression : expression OPEN_BRACKET expression CLOSE_BRACKET",
    "expression : expression OPEN_PARENS argument_list CLOSE_PARENS",
    "expression : NEW variable_or_type_name OPEN_PARENS argument_list CLOSE_PARENS",
    "expression : OPEN_PARENS variable_or_type_name CLOSE_PARENS expression",
    "expression : OPEN_PARENS expression CLOSE_PARENS",
    "argument_list :",
    "argument_list : argument_list_0",
    "argument_list_0 : expression",
    "argument_list_0 : argument_list_0 COMMA expression",
    "variable_or_type_name : IDENTIFIER",
    "variable_or_type_name : expression DOT IDENTIFIER",
    "variable_or_type_name : expression DOTDOT IDENTIFIER",
    "variable_or_type_name : expression MINUS OP_GT IDENTIFIER",
  };
  protected static  string [] yyNames = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"QUIT","EOF","NONE","ERROR",
    "IDENTIFIER","INTEGER","NUMBER","STRING","HASH","AT","PERCENT",
    "DOLLAR","DOT","DOTDOT","BANG","COMMA","ASSIGN","STAR","PLUS","MINUS",
    "DIV","OPEN_PARENS","CLOSE_PARENS","OPEN_BRACKET","CLOSE_BRACKET",
    "OP_LT","OP_GT","COLON","AMPERSAND","LENGTH","LOWER","UPPER","NEW",
    "THIS","BASE","CATCH",
  };

  /** index-checked interface to yyNames[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
  public static string yyname (int token) {
    if ((token < 0) || (token > yyNames.Length)) return "[illegal]";
    string name;
    if ((name = yyNames[token]) != null) return name;
    return "[unknown]";
  }

  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected string[] yyExpecting (int state) {
    int token, n, len = 0;
    bool[] ok = new bool[yyNames.Length];

    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyNames.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyNames[token] != null) {
          ++ len;
          ok[token] = true;
        }

    string [] result = new string[len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = yyNames[token];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
 internal Object yyparse (yyParser.yyInput yyLex, Object yyd)
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
  internal Object yyparse (yyParser.yyInput yyLex)
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
        yyStates.CopyTo (i, 0);
        yyStates = i;
        Object[] o = new Object[yyVals.Length+yyMax];
        yyVals.CopyTo (o, 0);
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
#line 75 "CSharpExpressionParser.jay"
  {
		return yyVals[0+yyTop];
	  }
  break;
case 3:
#line 83 "CSharpExpressionParser.jay"
  {
		yyVal = new AssignmentExpression ((Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop]);
	  }
  break;
case 4:
#line 90 "CSharpExpressionParser.jay"
  {
		yyVal = new NumberExpression ((long) yyVals[0+yyTop]);
	  }
  break;
case 5:
#line 94 "CSharpExpressionParser.jay"
  {
		yyVal = new NumberExpression ((int) yyVals[0+yyTop]);
	  }
  break;
case 6:
#line 98 "CSharpExpressionParser.jay"
  {
		yyVal = new StringExpression ((string) yyVals[0+yyTop]);
	  }
  break;
case 7:
#line 102 "CSharpExpressionParser.jay"
  {
		yyVal = new ThisExpression ();
	  }
  break;
case 8:
#line 106 "CSharpExpressionParser.jay"
  {
    //		yyVal = new CatchExpression ();
	  }
  break;
case 9:
#line 110 "CSharpExpressionParser.jay"
  {
		yyVal = new MemberAccessExpression (new BaseExpression (), "." + ((string) yyVals[0+yyTop]));
	  }
  break;
case 10:
#line 114 "CSharpExpressionParser.jay"
  {
		yyVal = new MemberAccessExpression (new BaseExpression (), (string) yyVals[0+yyTop]);
	  }
  break;
case 12:
#line 119 "CSharpExpressionParser.jay"
  {
    //		yyVal = new RegisterExpression ((string) yyVals[0+yyTop], 0);
	  }
  break;
case 13:
#line 123 "CSharpExpressionParser.jay"
  {
		yyVal = new PointerDereferenceExpression ((Expression) yyVals[0+yyTop], false);
	  }
  break;
case 14:
#line 127 "CSharpExpressionParser.jay"
  {
		yyVal = new AddressOfExpression ((Expression) yyVals[0+yyTop]);
	  }
  break;
case 15:
#line 131 "CSharpExpressionParser.jay"
  {
		yyVal = new ArrayAccessExpression ((Expression) yyVals[-3+yyTop], (Expression) yyVals[-1+yyTop]);
	  }
  break;
case 16:
#line 135 "CSharpExpressionParser.jay"
  {
		yyVal = new InvocationExpression ((Expression) yyVals[-3+yyTop], ((Expression []) yyVals[-1+yyTop]));
	  }
  break;
case 17:
#line 139 "CSharpExpressionParser.jay"
  {
		yyVal = new NewExpression ((Expression) yyVals[-3+yyTop], ((Expression []) yyVals[-1+yyTop]));
	  }
  break;
case 18:
#line 143 "CSharpExpressionParser.jay"
  {
		yyVal = new CastExpression ((Expression) yyVals[-2+yyTop], (Expression) yyVals[0+yyTop]);
	  }
  break;
case 19:
#line 147 "CSharpExpressionParser.jay"
  {
		yyVal = yyVals[-1+yyTop];
	  }
  break;
case 20:
#line 154 "CSharpExpressionParser.jay"
  {
		yyVal = new Expression [0];
	  }
  break;
case 21:
#line 158 "CSharpExpressionParser.jay"
  {
		Expression[] args = new Expression [((ArrayList) yyVals[0+yyTop]).Count];
		((ArrayList) yyVals[0+yyTop]).CopyTo (args, 0);

		yyVal = args;
	  }
  break;
case 22:
#line 168 "CSharpExpressionParser.jay"
  {
		ArrayList args = new ArrayList ();
		args.Add (yyVals[0+yyTop]);

		yyVal = args;
	  }
  break;
case 23:
#line 175 "CSharpExpressionParser.jay"
  {
		ArrayList args = (ArrayList) yyVals[-2+yyTop];
		args.Add (yyVals[0+yyTop]);

		yyVal = args;
	  }
  break;
case 24:
#line 185 "CSharpExpressionParser.jay"
  {
		yyVal = new SimpleNameExpression ((string) yyVals[0+yyTop]);
	  }
  break;
case 25:
#line 189 "CSharpExpressionParser.jay"
  { 
		yyVal = new MemberAccessExpression ((Expression) yyVals[-2+yyTop], (string) yyVals[0+yyTop]);
	  }
  break;
case 26:
#line 193 "CSharpExpressionParser.jay"
  { 
		yyVal = new MemberAccessExpression ((Expression) yyVals[-2+yyTop], "." + (string) yyVals[0+yyTop]);
	  }
  break;
case 27:
#line 197 "CSharpExpressionParser.jay"
  {
		Expression expr = new PointerDereferenceExpression ((Expression) yyVals[-3+yyTop], true);
		yyVal = new MemberAccessExpression (expr, (string) yyVals[0+yyTop]);
	  }
  break;
#line default
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
    0,    1,    1,    2,    2,    2,    2,    2,    2,    2,
    2,    2,    2,    2,    2,    2,    2,    2,    2,    4,
    4,    5,    5,    3,    3,    3,    3,
  };
   static  short [] yyLen = {           2,
    1,    1,    3,    1,    1,    1,    1,    1,    3,    3,
    1,    2,    2,    2,    4,    4,    5,    4,    3,    0,
    1,    1,    3,    1,    3,    3,    4,
  };
   static  short [] yyDefRed = {            0,
   24,    5,    4,    6,    0,    0,    0,    0,    0,    7,
    0,    8,    0,    1,    0,   11,   12,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   19,    0,    0,   10,    9,   25,   26,    0,    0,
    0,    0,    0,    0,    0,    0,   27,   16,    0,   15,
   17,    0,
  };
  protected static  short [] yyDgoto  = {            13,
   14,   41,   16,   42,   43,
  };
  protected static  short [] yySindex = {         -246,
    0,    0,    0,    0, -255, -246, -246, -246, -246,    0,
 -245,    0,    0,    0, -215,    0,    0, -198, -210, -253,
 -198, -198, -256, -232, -230, -227, -225, -246, -243, -246,
 -246,    0, -246, -246,    0,    0,    0,    0, -198, -224,
 -198, -241, -223, -228, -198, -222,    0,    0, -246,    0,
    0, -198,
  };
  protected static  short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   47,    0,    0,    1,    0, -195,
    4,    0, -257,    0,    0,    0,    0,    0,    0, -217,
    0,    0,    0, -217,    0,    0,    0,    0,   56,    0,
 -272,    0, -212,    0,   14,    0,    0,    0,    0,    0,
    0, -252,
  };
  protected static  short [] yyGindex = {            0,
    0,    2,   -4,   30,    0,
  };
  protected static  short [] yyTable = {            22,
   13,   15,   20,   14,   23,   17,   22,   18,   19,   21,
   22,   11,   11,   18,    1,    2,    3,    4,   11,   23,
    5,   34,   11,   24,   25,   33,   23,    6,   35,   39,
   36,    7,   44,   37,   45,   38,   47,   48,    8,   40,
   26,   27,    9,   10,   11,   12,    2,   29,   49,   30,
   52,   31,   50,   26,   27,    3,   51,   28,   26,   27,
   29,   20,   30,   46,   31,   29,   21,   30,   32,   31,
   26,   27,    0,   11,   11,    0,    0,   29,    0,   30,
   11,   31,   11,    0,   11,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   13,   13,    0,   14,   14,    0,    0,   13,
    0,   13,   14,    0,   14,   18,   18,    0,    0,    0,
    0,    0,   18,    0,   18,
  };
  protected static  short [] yyCheck = {           272,
    0,    0,    7,    0,    9,  261,  279,    6,    7,    8,
    9,  269,  270,    0,  261,  262,  263,  264,  276,  272,
  267,  278,  280,  269,  270,  279,  279,  274,  261,   28,
  261,  278,   31,  261,   33,  261,  261,  279,  285,  283,
  269,  270,  289,  290,  291,  292,    0,  276,  272,  278,
   49,  280,  281,  269,  270,    0,  279,  273,  269,  270,
  276,  279,  278,   34,  280,  276,  279,  278,  279,  280,
  269,  270,   -1,  269,  270,   -1,   -1,  276,   -1,  278,
  276,  280,  278,   -1,  280,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  272,  273,   -1,  272,  273,   -1,   -1,  279,
   -1,  281,  279,   -1,  281,  272,  273,   -1,   -1,   -1,
   -1,   -1,  279,   -1,  281,
  };

#line 204 "CSharpExpressionParser.jay"

public CSharpExpressionParser (EvaluationContext context, string name)
{
	this.reader = new MyTextReader ();
	this.current_context = context;

	lexer = new Tokenizer (context, reader, name);
}

public Expression Parse (string text)
{
	try {
		reader.Text = text;
		lexer.restart ();
		if (yacc_verbose_flag)
			return (Expression) yyparse (lexer, new yydebug.yyDebugSimple ());
		else
			return (Expression) yyparse (lexer);
	} catch (Exception e){
		// Please do not remove this, it is used during debugging
		// of the grammar
		//
// 		current_context.Error (lexer.location + "  : Parsing error ");
// 		current_context.Error (e.ToString ());
		return null;
	}
}

/* end end end */
}
#line default
namespace yydebug {
        using System;
	 internal interface yyDebug {
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
  public const int QUIT = 257;
  public const int EOF = 258;
  public const int NONE = 259;
  public const int ERROR = 260;
  public const int IDENTIFIER = 261;
  public const int INTEGER = 262;
  public const int NUMBER = 263;
  public const int STRING = 264;
  public const int HASH = 265;
  public const int AT = 266;
  public const int PERCENT = 267;
  public const int DOLLAR = 268;
  public const int DOT = 269;
  public const int DOTDOT = 270;
  public const int BANG = 271;
  public const int COMMA = 272;
  public const int ASSIGN = 273;
  public const int STAR = 274;
  public const int PLUS = 275;
  public const int MINUS = 276;
  public const int DIV = 277;
  public const int OPEN_PARENS = 278;
  public const int CLOSE_PARENS = 279;
  public const int OPEN_BRACKET = 280;
  public const int CLOSE_BRACKET = 281;
  public const int OP_LT = 282;
  public const int OP_GT = 283;
  public const int COLON = 284;
  public const int AMPERSAND = 285;
  public const int LENGTH = 286;
  public const int LOWER = 287;
  public const int UPPER = 288;
  public const int NEW = 289;
  public const int THIS = 290;
  public const int BASE = 291;
  public const int CATCH = 292;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  internal class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  internal interface yyInput {
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
#endif
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog
