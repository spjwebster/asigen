*
* AsiGen - ActionScript 2.0 intrinsic class generator
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
using System.Text.RegularExpressions;
using System.Text;
using AS2Parser.Args;

namespace AS2Parser {

	public delegate void StartHandler(object sender);
	public delegate void CompleteHandler(object sender);
	public delegate void ImportHandler(object sender, ImportHandlerEventArgs e);
	public delegate void ClassHandler(object sender, ClassHandlerEventArgs e);
	public delegate void InterfaceHandler(object sender, InterfaceHandlerEventArgs e);
	public delegate void MethodHandler(object sender, MethodHandlerEventArgs e);
	public delegate void PropertyHandler(object sender, PropertyHandlerEventArgs e);
	public delegate void GetterHandler(object sender, GetterHandlerEventArgs e);
	public delegate void SetterHandler(object sender, SetterHandlerEventArgs e);
	
	/// <description>
	///		SAX-style parser for ActionScript 2.0 file.
	/// </description>
	public class Parser {

		public Parser() {}

		public event StartHandler OnStart;
		public event CompleteHandler OnComplete;
		public event ImportHandler OnImportFound;
		public event ClassHandler OnClassFound;
		public event InterfaceHandler OnInterfaceFound;
		public event MethodHandler OnMethodFound;
		public event PropertyHandler OnPropertyFound;
		public event GetterHandler OnGetterFound;
		public event SetterHandler OnSetterFound;

		public void ParseFile(string filename) {
			// Read in entire file
			string input = File.OpenText(filename).ReadToEnd();
			
			OnStart(this);

			// Start at beginning of file
			int pos = 0;
			int blockLevel = 0;
			
			Match match;
			
			// While not EOF
			while (pos < input.Length) {
				// First match the things we don't want to parse
				match = ParserRegex.Whitespace.Match(input, pos);
				if (match.Success) {
					pos = match.Index + match.Length;
					continue;
				}
				
				match = ParserRegex.LineComment.Match(input, pos);
				if (match.Success) {
					pos = match.Index + match.Length;
					continue;
				}
				
				match = ParserRegex.Comment.Match(input, pos);
				if (match.Success) {
					pos = match.Index + match.Length;
					continue;
				}

				match = ParserRegex.StringLiteral.Match(input, pos);
				if (match.Success) {
					pos = match.Index + match.Length;
					continue;
				}
				
				if (input[pos] == ParserRegex.OpenBlock) {
					blockLevel++;
					pos += 1;
					continue;
				}
				
				if (input[pos] == ParserRegex.CloseBlock) {
					blockLevel--;
					pos += 1;
					continue;
				}

				if (blockLevel > 0) {
					pos += 1;
					continue;
				}
				
				match = ParserRegex.Include.Match(input, pos);
				if (match.Success) {
					pos = match.Index + match.Length;
					continue;
				}

				
				// Now match the stuff we do
				match = ParserRegex.Import.Match(input, pos);
				if (match.Success) {
					ParseImport(match);
					pos = match.Index + match.Length;
					continue;
				}
								
				match = ParserRegex.Class.Match(input, pos);
				if (match.Success) {
					ParseClass(match);
					pos = match.Index + match.Length;
					continue;
				}

				match = ParserRegex.Interface.Match(input, pos);
				if (match.Success) {
					ParseInterface(match);
					pos = match.Index + match.Length;
					continue;
				}

				match = ParserRegex.Method.Match(input, pos);
				if (match.Success) {
					ParseMethod(match);
					pos = match.Index + match.Length;
					continue;
				}

				match = ParserRegex.GetterSetter.Match(input, pos);
				if (match.Success) {
					ParseGetterSetter(match);
					pos = match.Index + match.Length;
					continue;
				}
				
				match = ParserRegex.Property.Match(input, pos);
				if (match.Success) {
					ParseProperty(match);
					pos = match.Index + match.Length;
					continue;
				}
				
				pos++;
			}
			
			OnComplete(this);
		}
		
		private void ParseImport(Match match) {
			string package = match.Groups["package"].Value;
			string name = match.Groups["name"].Value;

			OnImportFound(this, new ImportHandlerEventArgs(package, name));
		}

		private void ParseClass(Match match) {
			string attribute1 = match.Groups["attribute1"].Value;
			string package = match.Groups["package"].Value;
			string name = match.Groups["name"].Value;
			string baseClass = JoinPackageClass(match.Groups["basepackage"].Value, match.Groups["basename"].Value);
			string interfaces = match.Groups["interfaces"].Value.Trim();
			
			bool isDynamic = (attribute1 == "dynamic");

			OnClassFound(this, new ClassHandlerEventArgs(isDynamic, package, name, baseClass, interfaces));
		}

		private void ParseInterface(Match match) {
			string package = match.Groups["package"].Value;
			string name = match.Groups["name"].Value;
			string baseInterface = JoinPackageClass(match.Groups["basepackage"].Value, match.Groups["basename"].Value);

			OnInterfaceFound(this, new InterfaceHandlerEventArgs(package, name, baseInterface));
		}

		private void ParseMethod(Match match) {
			string attribute1 = match.Groups["attribute1"].Value;
			string attribute2 = match.Groups["attribute2"].Value;
			string name = match.Groups["name"].Value;
			string parameters = match.Groups["params"].Value;
			string returnType = JoinPackageClass(match.Groups["returnpackage"].Value, match.Groups["returnclass"].Value);

			NamespaceAttribute ns = NamespaceAttribute.Public;
			if (attribute1 == "private" || attribute2 == "private") {
				ns = NamespaceAttribute.Private;
			}
			
			bool isStatic = (attribute1 == "static" || attribute2 == "static");

			OnMethodFound(this, new MethodHandlerEventArgs(ns, isStatic, name, parameters, returnType));
		}

		private void ParseGetterSetter(Match match) {
			string attribute1 = match.Groups["attribute1"].Value;
			string attribute2 = match.Groups["attribute2"].Value;
			string name = match.Groups["name"].Value;
			string attribute3 = match.Groups["attribute3"].Value;
			string parameters = match.Groups["params"].Value;
			string returnType = JoinPackageClass(match.Groups["returnpackage"].Value, match.Groups["returnclass"].Value);

			NamespaceAttribute ns = NamespaceAttribute.Public;
			if (attribute1 == "private" || attribute2 == "private") {
				ns = NamespaceAttribute.Private;
			}
			
			bool isStatic = (attribute1 == "static" || attribute2 == "static");

			if (attribute3 == "get") {
				OnGetterFound(this, new GetterHandlerEventArgs(ns, isStatic, name, parameters, returnType));
			} else {
				OnSetterFound(this, new SetterHandlerEventArgs(ns, isStatic, name, parameters, returnType));
			}

		}

		private void ParseProperty(Match match) {
			string attribute1 = match.Groups["attribute1"].Value;
			string attribute2 = match.Groups["attribute2"].Value;
			string name = match.Groups["name"].Value;
			string type = match.Groups["type"].Value;

			NamespaceAttribute ns = NamespaceAttribute.Public;
			if (attribute1 == "private" || attribute2 == "private") {
				ns = NamespaceAttribute.Private;
			}
			
			bool isStatic = (attribute1 == "static" || attribute2 == "static");

			OnPropertyFound(this, new PropertyHandlerEventArgs(ns, isStatic, name, type));
		}

		private string JoinPackageClass(string packageName, string className) {
			return packageName.Length > 0 ? packageName + "." + className : className;
		}
	}
}