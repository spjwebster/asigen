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

namespace ActionScript.Parse {

	/// <summary>
	/// Base token class
	/// </summary>
	public class Token {
		private string body;
		
		protected Token(string body) {
			this.body = body;
		}
		
		public string Body {
			get {
				return this.body;
			}
		}
		
		public override string ToString() {
			return this.Body;
		}
	}
	
	
	/// <summary>
	/// Token to represent whitespace
	/// </summary>
	public class WhiteSpaceToken : Token {
		public WhiteSpaceToken(string body) : base (body) {
			// Do nothing
		}
	}
	
	
	/// <summary>
	/// Token to represent identifier
	/// </summary>
	public class IdentifierToken : Token {
		public IdentifierToken(string body) : base (body) {
			// Do nothing
		}
	}
	
	
	/// <summary>
	/// Base token for comments
	/// </summary>
	public class CommentToken : Token {
		public CommentToken(string body) : base (body) {
			// Do nothing
		}
	}
	
	
	/// <summary>
	/// Token to represent single line comments
	/// </summary>
	public class SingleLineCommentToken : CommentToken {
		public SingleLineCommentToken(string body) : base (body) {
			// Do nothing
		}
		
		public override string ToString() {
			return "//" + this.Body;
		}
	}
	
	
	/// <summary>
	/// Token to represent multi line comments
	/// </summary>
	public class MultiLineCommentToken : CommentToken {
		public MultiLineCommentToken(string body) : base (body) {
			// Do nothing
		}
		
		public override string ToString() {
			return "/*" + this.Body + "*/";
		}
	}
	
	public class KeywordToken : Token {
		public KeywordToken(string body) : base (body) {
			// Do nothing
		}
	}
	
	public class PunctuationToken : Token {
		public PunctuationToken(string body) : base (body) {
			// Do nothing
		}		
	}

	
	/// <summary>
	/// Token to represent string literal
	/// </summary>
	public class StringLiteralToken : Token {
		public StringLiteralToken(string body) : base (body) {
			// Do nothing
		}
		
		public override string ToString() {
			return "\"" + this.Body + "\"";
		}
		
		public string escape(string text) {
			text = text.Replace("\\", "\\\\");
			text = text.Replace("\"", "\\\"");
			text = text.Replace("\n", "\\n");
			text = text.Replace("\t", "\\t");
			
			return text;
		}
		
		public string unescape(string text) {
			text = text.Replace("\\n", "\n");
			text = text.Replace("\\t", "\t");
			text = text.Replace("\\\\", "\\");

			return text;
		}
	}
	
	
}