// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using ICSharpCode.SharpRefactory.Parser;

namespace ICSharpCode.SharpRefactory.Parser.VB
{
	public class Token
	{
		public int kind;
		
		public int col;
		public int line;
		
		public object    literalValue = null;
		public string    val;
		public Token     next;
		public ArrayList specials;
		
		public Point EndLocation {
			get {
				return new Point(line, col + val.Length);
			}
		}
		
		public Point Location {
			get {
				return new Point(line, col);
			}
		}
		
		public Token()
		{
		}
		
		public Token(int kind)
		{
			this.kind = kind;
		}
		
		public Token(int kind, int col, int line, string val)
		{
			this.kind = kind;
			this.col  = col;
			this.line = line;
			this.val  = val;
		}
		
		public Token(int kind, int col, int line, string val, object literalValue)
		{
			this.kind         = kind;
			this.col          = col;
			this.line         = line;
			this.val          = val;
			this.literalValue = literalValue;
		}
	}
	
	public class Lexer
	{
		IReader reader;
		static  Hashtable keywords = new Hashtable();
		
		int col  = 1;
		int line = 1;
		
		bool lineEnd = false;
		
		Errors errors   = new Errors();
		
		SpecialTracker specialTracker = new SpecialTracker();
		
		Token lastToken = null;
		Token curToken  = null;
		Token peekToken = null;
		
		public Errors Errors {
			get {
				return errors;
			}
		}
		
		public Token Token {
			get {
				return lastToken;
			}
		}
		
		public Token LookAhead {
			get {
				return curToken;
			}
		}
		
		public void StartPeek()
		{
			peekToken = curToken;
		}
		
		public Token Peek()
		{
			if (peekToken.next == null) {
				peekToken.next = Next();
				peekToken.next.specials = this.specialTracker.RetrieveSpecials();
			}
			peekToken = peekToken.next;
			return peekToken;
		}
		
		public Token NextToken()
		{
			if (curToken == null) {
				curToken = Next();
				curToken.specials = this.specialTracker.RetrieveSpecials();
				return curToken;
			}
			
			if (lastToken != null && lastToken.specials != null) {
				curToken.specials.InsertRange(0, lastToken.specials);
			}
			
			lastToken = curToken;
			
			if (curToken.next == null) {
				curToken.next = Next();
				curToken.next.specials = this.specialTracker.RetrieveSpecials();
			}
			
			curToken  = curToken.next;
			//Console.WriteLine("XX" + curToken.val + "\t" + curToken.kind);
			return curToken;
		}
		
		public ArrayList RetrieveSpecials()
		{
			if (lastToken == null) {
				return this.specialTracker.RetrieveSpecials();
			}
			
			Debug.Assert(lastToken.specials != null);
			
			ArrayList tmp = lastToken.specials;
			lastToken.specials = null;
			return tmp;
		}
		
		public Lexer(IReader reader)
		{
			this.reader = reader;
		}
		
		public Token Next()
		{
			while (!reader.Eos()) {
				char ch = reader.GetNext();
				++col;
				if (Char.IsWhiteSpace(ch)) {
					if (ch == '\n') {
						int x = col - 1;
						int y = line;
						++line;
						col = 0;
						if (!lineEnd) {
							lineEnd = true;
							return new Token(Tokens.EOL, x, y, "\n");
						}
					}
					continue;
				}
				if (ch == '_') {
					if (reader.Eos()) {
						errors.Error(line, col, String.Format("No EOF expected after _"));
					}
					ch = reader.GetNext();
					++col;
					if (!Char.IsWhiteSpace(ch)) {
						reader.UnGet();
						--col;
						int x = col;
						int y = line;
						string s = ReadIdent('_');
						lineEnd = false;
						return new Token(Tokens.Identifier, x, y, s);
					}
					while (Char.IsWhiteSpace(ch)) {
						if (ch == '\n') {
							++line;
							col = 0;
							break;
						}
						if (!reader.Eos()) {
							ch = reader.GetNext();
							++col;
						}
					}
					if (ch != '\n') {
						errors.Error(line, col, String.Format("Return expected"));
					}
					continue;
				}
				
				if (ch == '#') {
					ReadPreprocessorDirective();
					continue;
				}
				
				lineEnd = false;
				if (ch == '[') { // Identifier
					if (reader.Eos()) {
						errors.Error(line, col, String.Format("Identifier expected"));
					}
					ch = reader.GetNext();
					++col;
					if (ch == ']' || Char.IsWhiteSpace(ch)) {
						errors.Error(line, col, String.Format("Identifier expected"));
					}
					int x = col - 1;
					int y = line;
					string s = ReadIdent(ch);
					if (reader.Eos()) {
						errors.Error(line, col, String.Format("']' expected"));
					}
					ch = reader.GetNext();
					++col;
					if (!(ch == ']')) {
						errors.Error(line, col, String.Format("']' expected"));
					}
					return new Token(Tokens.Identifier, x, y, s);
				}
				if (Char.IsLetter(ch)) {
					int x = col;
					int y = line;
					string s = ReadIdent(ch);
					if (Keywords.IsKeyword(s)) {
						return new Token(Keywords.GetToken(s), x, y, s);
					}
					
					// handle 'REM' comments 
					if (s.ToUpper() == "REM") {
						ReadComment();
						continue;
					}
						
					return new Token(Tokens.Identifier, x, y, s);
				}
				if (Char.IsDigit(ch)) {
					return ReadDigit(ch, col);
				}
				if (ch == '&') {
					if (reader.Eos()) {
						return ReadOperator('&');
					}
					ch = reader.GetNext();
					++col;
					if (Char.ToUpper(ch) == 'H' || Char.ToUpper(ch) == 'O') {
						reader.UnGet();
						--col;
						return ReadDigit('&', col);
					} else {
						return ReadOperator('&');
					}
				}
				if (ch == '\'') {
					ReadComment();
					continue;
				}
				if (ch == '"') {
					int x = col;
					int y = line;
					string s = ReadString();
					if (!reader.Eos() && (reader.Peek() == 'C' || reader.Peek() == 'c')) {
						reader.GetNext();
						++col;
						if (s.Length != 1) {
							errors.Error(line, col, String.Format("Chars can only have Length 1 "));
						}
						return new Token(Tokens.LiteralCharacter, x, y, String.Concat('"', s , "\"C") , s[0]);
					}
					return new Token(Tokens.LiteralString, x, y,  String.Concat('"', s , '"'), s);
				}
				Token token = ReadOperator(ch);
				if (token != null) {
					return token;
				}
				errors.Error(line, col, String.Format("Unknown char({0}) which can't be read", ch));
			}
			
			return new Token(Tokens.EOF);
		}
		
		string ReadIdent(char ch) 
		{
			StringBuilder s = new StringBuilder(ch.ToString());
			while (!reader.Eos() && (Char.IsLetterOrDigit(ch = reader.GetNext()) || ch == '_')) {
				++col;
				s.Append(ch.ToString());
			}
			++col;
			if (reader.Eos()) {
				--col;
				return s.ToString();
			}
			reader.UnGet();
			--col;
			if (!reader.Eos() && "%&@!#$".IndexOf(Char.ToUpper(reader.Peek())) != -1) {
				reader.GetNext();
				++col;
			}
			return s.ToString();
		}
		
		Token ReadDigit(char ch, int x)
		{
			StringBuilder sb = new StringBuilder(ch.ToString());
			int y = line;
			string digit = "";
			if (ch != '&') {
				digit += ch;
			}
			
			bool ishex      = false;
			bool isokt      = false;
			bool issingle   = false;
			bool isdouble   = false;
			bool isdecimal  = false;
			
			if (reader.Eos()) {
				if (ch == '&') {
					errors.Error(line, col, String.Format("digit expected"));
				}
				return new Token(Tokens.LiteralInteger, x, y, sb.ToString() ,ch - '0');
			}
			if (ch == '&' && Char.ToUpper(reader.Peek()) == 'H') {
				const string hex = "0123456789ABCDEF";
				sb.Append(reader.GetNext()); // skip 'H'
				++col;
				while (!reader.Eos() && hex.IndexOf(Char.ToUpper(reader.Peek())) != -1) {
					ch = reader.GetNext();
					sb.Append(ch); 
					digit += Char.ToUpper(ch);
					++col;
				}
				ishex = true;
			} else if (!reader.Eos() && ch == '&' && Char.ToUpper(reader.Peek()) == 'O') {
				const string okt = "01234567";
				sb.Append(reader.GetNext()); // skip 'O'
				++col;
				while (!reader.Eos() && okt.IndexOf(Char.ToUpper(reader.Peek())) != -1) {
					ch = reader.GetNext();
					sb.Append(ch); 
					digit += Char.ToUpper(ch);
					++col;
				}
				isokt = true;
			} else {
				while (!reader.Eos() && Char.IsDigit(reader.Peek())) {
					digit += reader.GetNext();
					++col;
				}
			}
			if (!reader.Eos() && "%&SIL".IndexOf(Char.ToUpper(reader.Peek())) != -1 || ishex || isokt) {
				ch = reader.GetNext();
				sb.Append(ch); 
				ch = Char.ToUpper(ch);
				++col;
				if (isokt) {
					long number = 0L;
					for (int i = 0; i < digit.Length; ++i) {
						number = number * 8 + digit[i] - '0';
					}
					if (ch == 'S') {
						return new Token(Tokens.LiteralSingle, x, y, sb.ToString(), (short)number);
					} else if (ch == '%' || ch == 'I') {
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (int)number);
					} else if (ch == '&' || ch == 'L') {
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (long)number);
					} else {
						if (number > int.MaxValue || number < int.MinValue) {
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (long)number);
						} else {
							return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (int)number);
						}
					}
				}
				if (ch == 'S') {
					return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int16.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} else if (ch == '%' || ch == 'I') {
					return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} else if (ch == '&' || ch == 'L') {
					return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
				} else if (ishex) {
					reader.UnGet();
					--col;
					long number = Int64.Parse(digit, NumberStyles.HexNumber);
					if (number > int.MaxValue || number < int.MinValue) {
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), number);
					} else {
						return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), (int)number);
					}
				}
			}
			if (!reader.Eos() && reader.Peek() == '.') { // read floating point number
				isdouble = true; // double is default
				if (ishex || isokt) {
					errors.Error(line, col, String.Format("No hexadecimal or oktadecimal floating point values allowed"));
				}
				digit += reader.GetNext();
				++col;
				while (!reader.Eos() && Char.IsDigit(reader.Peek())){ // read decimal digits beyond the dot
					digit += reader.GetNext();
					++col;
				}
			}
			
			if (!reader.Eos() && Char.ToUpper(reader.Peek()) == 'E') { // read exponent
				isdouble = true;
				digit +=  reader.GetNext();
				++col;
				if (!reader.Eos() && (reader.Peek() == '-' || reader.Peek() == '+')) {
					digit += reader.GetNext();
					++col;
				}
				while (!reader.Eos() && Char.IsDigit(reader.Peek())) { // read exponent value
					digit += reader.GetNext();
					++col;
				}
			}
			
			if (!reader.Eos()) {
				if (Char.ToUpper(reader.Peek()) == 'R' || Char.ToUpper(reader.Peek()) == '#') { // double type suffix (obsolete, double is default)
					reader.GetNext();
					++col;
					isdouble = true;
				} else if (Char.ToUpper(reader.Peek()) == 'D' || Char.ToUpper(reader.Peek()) == '@') { // decimal value
					reader.GetNext();
					++col;
					isdecimal = true;
				} else if (Char.ToUpper(reader.Peek()) == 'F' || Char.ToUpper(reader.Peek()) == '!') { // decimal value
					reader.GetNext();
					++col;
					issingle = true;
				}
			}
			
			if (issingle) {
				NumberFormatInfo mi = new NumberFormatInfo();
				mi.CurrencyDecimalSeparator = ".";
				return new Token(Tokens.LiteralSingle, x, y, sb.ToString(), Single.Parse(digit, mi));
			}
			if (isdecimal) {
				NumberFormatInfo mi = new NumberFormatInfo();
				mi.CurrencyDecimalSeparator = ".";
				return new Token(Tokens.LiteralDecimal, x, y, sb.ToString(), Decimal.Parse(digit, mi));
			}
			if (isdouble) {
				NumberFormatInfo mi = new NumberFormatInfo();
				mi.CurrencyDecimalSeparator = ".";
				return new Token(Tokens.LiteralDouble, x, y, sb.ToString(), Double.Parse(digit, mi));
			}
			return new Token(Tokens.LiteralInteger, x, y, sb.ToString(), Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
		}
		
		void ReadPreprocessorDirective()
		{
			// TODO : Insert into specials ...
			while (!reader.Eos() && reader.GetNext() != '\n') {
				++col;
				// nothing to do
			}
			++line;
			col = 0;
		}
		
		string ReadString()
		{
			char ch = '\0';
			StringBuilder s = new StringBuilder();
			while (!reader.Eos()) {
				ch = reader.GetNext();
				++col;
				if (ch == '"') {
					if (!reader.Eos() && reader.Peek() == '"') {
						s.Append('"');
						reader.GetNext();
						++col;
					} else {
						break;
					}
				} else if (ch == '\n') {
					errors.Error(line, col, String.Format("No return allowed inside String literal"));
				} else {
					s.Append(ch);
				}
			}
			if (ch != '"') {
				errors.Error(line, col, String.Format("End of File reached before String terminated "));
			}
			return s.ToString();
		}
		
		void ReadComment()
		{
			StringBuilder comment = new StringBuilder();
			int x = col;
			int y = line;
			while (!reader.Eos()) {
				char ch = reader.GetNext();
				++col;
				if (ch == '\n') {
					++line;
					col = 0;
					return;
				}
			}
		}
		
		Token ReadOperator(char ch)
		{
			int x = col;
			int y = line;
			switch(ch) {
				case '+':
					return new Token(Tokens.Plus, x, y, "+");
				case '-':
					return new Token(Tokens.Minus, x, y, "-");
				case '*':
					return new Token(Tokens.Times, x, y, "*");
				case '/':
					return new Token(Tokens.Div, x, y, "/");
				case '\\':
					return new Token(Tokens.DivInteger, x, y, "\\");
				case '&':
					return new Token(Tokens.ConcatString, x, y, "&");
//				case '^':
//					return new Token(TokenType.Operator, OperatorType.Pow, x, y);
				case ':':
					if (reader.Peek() != '=') {
						return new Token(Tokens.Colon, x, y, ":");
					}
					reader.GetNext();
					++col;
					return new Token(Tokens.NamedAssign, x, y, ":=");
				case '=':
					return new Token(Tokens.Assign, x, y, "=");
				case '<':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.LessEqual, x, y, "<=");
							case '>':
								++col;
								return new Token(Tokens.NotEqual, x, y, "<>");
							default:
								reader.UnGet();
								return new Token(Tokens.LessThan, x, y, "<");
						}
					}
					return new Token(Tokens.LessThan, x, y, "<");
				case '>':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.GreaterEqual, x, y, "<=");
							default:
								reader.UnGet();
								return new Token(Tokens.GreaterThan, x, y, "<=");
						}
					}
					return new Token(Tokens.GreaterThan, x, y, "<=");
				case ',':
					return new Token(Tokens.Comma, x, y, ",");
				case '.':
					if (Char.IsDigit(reader.Peek())) {
						 reader.UnGet();
						 --col;
						 return ReadDigit('0', col);
					}
					return new Token(Tokens.Dot, x, y, ".");
				case '(':
					return new Token(Tokens.OpenParenthesis, x, y, "(");
				case ')':
					return new Token(Tokens.CloseParenthesis, x, y, ")");
				case '{':
					return new Token(Tokens.OpenCurlyBrace, x, y, "{");
				case '}':
					return new Token(Tokens.CloseCurlyBrace, x, y, "}");
				case '[':
					return new Token(Tokens.OpenSquareBracket, x, y, "[");
				case ']':
					return new Token(Tokens.CloseSquareBracket, x, y, "]");
			}
			return null;
		}
	}
}
