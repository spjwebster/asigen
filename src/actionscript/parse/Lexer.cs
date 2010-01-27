/*
* ActionScript.Parse - ActionScript 2.0 lexer and parser library
* Based on the ActionScript.Parse library from as2api
*
* Copyright (c) 2005-2010 Steve Webster.
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized
using System.Text.RegularExpressions;

namespace ActionScript.Parse {

	/// <summary>
	/// 
	/// </summary>
	public class Lexer {
		// Static stuff
		static private string[] keywords = new string [] {
			"as","break","case","catch","class","const","continue","default","dynamic",
			"delete","do","else","extends","false","finally","for","function","if","implements",
			"import","in","instanceof","interface","intrinsic","new","null","private",
			"public","return","static","super","switch","this","throw","true","try",
			"typeof","use","var","void","while","with"
		};
		
		private static StringDictionary punctuation = new StringDictionary();
				
		private delegate bool TokenMatcher(string input, out int length);

		private TextReader input;
		private ArrayList tokens;
		private bool eof;
		
		private TokenMatcher matchers;
		
		static Lexer() {
			punctuation.Add("DivideAssign", "/=");
			punctuation.Add("Divide", "/");
			punctuation.Add("BitNot", "~");
			punctuation.Add("RBrace", "}");
			punctuation.Add("OrAssign", "||=");
			punctuation.Add("Or", "||");
			punctuation.Add("BitOrAssign", "|=");
			punctuation.Add("BitOr", "|");
			punctuation.Add("LBrace", "{");
			punctuation.Add("XOrAssign", "^^=");
			punctuation.Add("XOr", "^^");
			punctuation.Add("BitXOrAssign", "^=");
			punctuation.Add("BitXOr", "^");
			punctuation.Add("RBracket", "]");
			punctuation.Add("LBracket", "[");
			punctuation.Add("Hook", "?");
			punctuation.Add("RShiftUnsignedAssign", ">>>=");
			punctuation.Add("RShiftUnsigned", ">>>");
			punctuation.Add("RShiftAssign", ">>=");
			punctuation.Add("RShift", ">>");
			punctuation.Add("GreaterEquals", ">=");
			punctuation.Add("Greater", ">");
			punctuation.Add("Same", "===");
			punctuation.Add("Equals", "==");
			punctuation.Add("Assign", "=");
			punctuation.Add("LessEquals", "<=");
			punctuation.Add("LShiftAssign", "<<=");
			punctuation.Add("LShift", "<<");
			punctuation.Add("Less", "<");
			punctuation.Add("Semicolon", ";");
			punctuation.Add("Colon", ":");
			punctuation.Add("Ellipsis", "...");
			punctuation.Add("Dot", ".");
			punctuation.Add("MinusAssign", "-=");
			punctuation.Add("Decrement", "--");
			punctuation.Add("Minus", "-");
			punctuation.Add("Comma", ",");
			punctuation.Add("PlusAssign", "+=");
			punctuation.Add("Increment", "++");
			punctuation.Add("Plus", "+");
			punctuation.Add("MultiplyAssign", "*=");
			punctuation.Add("Multiply", "*");
			punctuation.Add("RParen", ")");
			punctuation.Add("LParen", "(");
			punctuation.Add("BitAndAssign", "&=");
			punctuation.Add("AndAssign", "&&=");
			punctuation.Add("And", "&&");
			punctuation.Add("BitAnd", "&");
			punctuation.Add("ModuloAssign", "%=");
			punctuation.Add("Modulo", "%");
			punctuation.Add("NotSame", "!==");
			punctuation.Add("NotEquals", "!=");
			punctuation.Add("Not", "!");
		}
		
		public Lexer(TextReader input) {
			// Store input reader
			this.input = input;
			
			// Add matchers in order of precedance
			this.matchers += new TokenMatcher(MatchWhiteSpace);
			this.matchers += new TokenMatcher(MatchSingleLineComment);
			this.matchers += new TokenMatcher(MatchMultiLineComment);
			this.matchers += new TokenMatcher(MatchStringLiteral);
			this.matchers += new TokenMatcher(MatchKeyword);
			this.matchers += new TokenMatcher(MatchIdentifier);
			
			// Reset lexer
			this.Reset();
		}
		
		public void Reset() {
			this.tokens = new ArrayList(200);
			this.eof = false;
		}
		
		public Token Peek() {
			// Fill if necessary
			this.CheckFill();

			if (this.tokens.Count == 0) {
				return null;
			}
			
			return (Token) this.tokens[0];
		}
		
		public Token Next() {
			// Fill if necessary
			this.CheckFill();
			
			if (this.tokens.Count == 0) {
				return null;
			}
			
			Token nextToken = (Token) this.tokens[0];
			this.tokens.Remove(nextToken);
			return nextToken;
		}
		
		public void PutBack(Token token) {
			// Put token back on stack
			this.tokens.Insert(0, token);
		}
		
		private bool MatchWhiteSpace(string input, out int length) {
			length = 0;

			Regex re = new Regex(@"\A[ \t\r\n\f]+", RegexOptions.Compiled);
			Match match = re.Match(input);
			
			if (match.Success) {
				this.emit(new WhiteSpaceToken(match.Groups[0].Value));
				length = match.Length;
				return true;
			}
			
			return false;
		}
		
		private bool MatchSingleLineComment(string input, out int length) {
			length = 0;
			
			Regex re = new Regex(@"\A//([^\n]*)", RegexOptions.Compiled);
			Match match = re.Match(input);
			
			if (match.Success) {
				this.emit(new SingleLineCommentToken(match.Groups[1].Value));
				length = match.Length;
				return true;
			}
			
			return false;
		}
		
		private bool MatchMultiLineComment(string input, out int length) {
			length = 0;
			
			Regex re = new Regex(@"\A/\*(.*?)\*" + "/", RegexOptions.Compiled | RegexOptions.Singleline);
			Match match = re.Match(input);
			
			if (match.Success) {
				this.emit(new MultiLineCommentToken(match.Groups[1].Value));
				length = match.Length;
				return true;
			}
			
			return false;
		}
		
		private bool MatchStringLiteral(string input, out int length) {
			length = 0;
			
			Regex re = new Regex("\\A\"([^\"\\\\]*(\\\\.[^\"\\\\]*)*)\"", RegexOptions.Compiled);
			Match match = re.Match(input);
			
			if (match.Success) {
				this.emit(new StringLiteralToken(match.Groups[1].Value));
				length = match.Length;
				return true;
			}
			
			return false;
		}
		
		private bool MatchKeyword(string input, out int length) {
			length = 0;
			
			foreach (string keyword in Lexer.keywords) {
				// Don't bother with regex if characters don't match
				if (input.StartsWith(keyword)) {
					// Match only keyword
					Regex re = new Regex(@"\A" + keyword + @"\b", RegexOptions.Compiled);
					Match match = re.Match(input);
					
					if (match.Success) {
						this.emit(new KeywordToken(keyword));
						length = keyword.Length;
						//System.Console.WriteLine("match: " + keyword);
						return true;
					}
				}
			}		
			
			return false;	
		}
		
		private bool MatchIdentifier(string input, out int length) {
			length = 0;
			
			Regex re = new Regex(@"\A[a-zA-z$_][a-zA-z0-9$_]*", RegexOptions.Compiled);
			Match match = re.Match(input);
			
			if (match.Success) {
				this.emit(new IdentifierToken(match.Groups[0].Value));
				length = match.Length;
				//System.Console.WriteLine("match");
				return true;
			}
			
			return false;
		}
		
		private void CheckFill() {
			// Don't fill if already full
			if (this.tokens.Count == 0) {
				this.Fill();
			}
		}
			
		private void Fill() {
			
			// Read input data
			// TODO: Make this more efficient using stream methods???
			string data = input.ReadToEnd();
			
			// While there's still data left to parse
			while (data.Length > 0) {
				// Assume no match found
				bool matchFound = false;
				
				// For each of our token matchers
				foreach (TokenMatcher matcher in matchers.GetInvocationList()) {
					// Assume match length is zero
					int matchLength = 0;
					
					// If current token matcher matched input
					if (matcher(data, out matchLength)) {
						// Remove matched data from input
						data = data.Remove(0, matchLength);						
						
						// Flag that we've found a match
						matchFound = true;
						break;
					}
				}
				
				//System.Console.WriteLine("matchFound: " + matchFound);
				
				// If no match was found
				if (!matchFound) {
					// Skip over whatever we couldn't parse
					data = data.Remove(0, 1);
				}
			}
		}
		
		private Token emit(Token token) {
			// Add token to stack
			tokens.Add(token);
			return token;
		}
	}
	
	public class LexerTest {
		public static void Main() {
			long startTime = DateTime.Now.Ticks;
	
			StreamReader input = new StreamReader("test.as");
			
			Lexer lex = new Lexer(input);
			
			Token token;
			
			while((token = lex.Next()) != null) {
				if (!(token is WhiteSpaceToken)) {
					System.Console.WriteLine("Found token: " + token);
				}
			}

			TimeSpan interval = new TimeSpan(DateTime.Now.Ticks - startTime);
			System.Console.WriteLine("Processed in " + interval.TotalSeconds + " seconds");
		}
	}
}