/*
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

namespace AS2Parser {

	public class ParserRegex {
		// Code parsing regular expressions

		/**
		 * Pattern for balanced brace matching.
		*/
		/*private static string BraceMatch = @"((\{(?>[^{}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))\})|;)";*/
		public static char OpenBlock = '{';
		public static char CloseBlock = '}';

		/**
		 * Regex for parsing import statements.
		 *
		 * Groups:
		 *   - package          package
		 *   - name             class name
		 */
		public static Regex Include = new Regex(@"\G#include[\s\n]+(.*?);", RegexOptions.Compiled);
		
		/**
		 * Regex for parsing import statements.
		 *
		 * Groups:
		 *   - package          package
		 *   - name             class name
		 */
		public static Regex Import = new Regex(@"\Gimport[\s\n]*((?<package>[a-zA-Z0-9$_.]+)\.)(?<name>[a-zA-Z0-9$*_]+)[\s\n]*;", RegexOptions.Compiled);
		
		/**
		 * Regex for parsing method definitions.
		 *
		 * Groups:
		 *   - package          package
		 *   - name             name of interface
		 *	 - base             base interface - '[basepackage].[baseclass]'
		 *   - basepackage      base interface package
		 *   - basename         base interface name
		*/
		public static Regex Interface = new Regex(@"\G((?<intrinsic>intrinsic)[\s\n]+)?interface\s+((?<package>[a-zA-Z0-9$_.]+)\.)?(?<name>[a-zA-Z0-9$_]+)(\s+extends\s+((?<basepackage>[a-zA-Z0-9$_.]+)\.)?(?<basename>[a-zA-Z0-9$_]+))?", RegexOptions.Compiled);

		/**
		 * Regex for parsing class definitions.
		 *
		 * Groups:
		 *   - attribute1       'dynamic' or ''
		 *   - class            fully qualified class - '[package].[name]'
		 *   - package          package
		 *   - name             name of class
		 *	 - base             fully qualified base class - '[basepackage].[baseclass]'
		 *   - basepackage      base class package
		 *   - basename         base class name
		 *   - interfaces		comma delimited list of implemented interfaces
		*/
		public static Regex Class = new Regex(@"\G((?<intrinsic>intrinsic)[\s\n]+)?((?<attribute1>dynamic)[\s\n+])?class[\s\n]+((?<package>[a-zA-Z0-9$_.]+)\.)?(?<name>[a-zA-Z0-9$_]+)([\s\n]+extends[\s\n]+((?<basepackage>[a-zA-Z0-9$_.]+)\.)?(?<basename>[a-zA-Z0-9$_]+))?([\s\n]+implements[\s\n]+(?<interfaces>([a-zA-Z0-9$_.]+\s*,?\s*)+))?[\s\n]*\{", RegexOptions.Compiled);

		/**
		 * Regex for parsing method definitions.
		 *
		 * Groups:
		 *   - attribute1       'public', 'private' or 'static'
		 *   - attribute2       'public', 'private' or 'static'
		 *   - name             name of method
		 *	 - params           method parameters
		 *   - return			fully qualified datatype - '[typepackage].[typeclass]'
		 *   - returnpackage    datatype package
		 *   - returnclass      datatype class
		*/
		public static Regex Method = new Regex(@"\G[\s\n]*((?<attribute1>(public|private|static))[\s\n]+)?((?<attribute2>(public|private|static))[\s\n]+)?function[\s\n]+(?<name>[a-zA-Z0-9$_]+)[\s\n]*\((?<params>[^)]*)\)([\s\n]*:[\s\n]*((?<returnpackage>[a-zA-Z0-9$_.]+)\.)?(?<returnclass>[a-zA-Z0-9$_]+))?[\s\n]*", RegexOptions.Compiled);

		/**
		 * Regex for parsing getter and setter definitions.
		 *
		 * Groups:
		 *   - attribute1       'public', 'private' or 'static'
		 *   - attribute2       'public', 'private' or 'static'
		 *	 - attribute3		'get' or 'set'
		 *   - name             name of getter/setter
		 *	 - type             fully qualified datatype - '[typepackage].[typeclass]'
		 *   - typepackage      datatype package
		 *   - typeclass        datatype class
		*/
		public static Regex GetterSetter = new Regex(@"\G((?<attribute1>(public|private|static))[\s\n]+)?((?<attribute2>(public|private|static))[\s\n]+)?function[\s\n]+((?<attribute3>(get|set))[\s\n]+)(?<name>[a-zA-Z0-9$_]+)[\s\n]*\((?<params>[^)]*)\)([\s\n]*:[\s\n]*((?<returnpackage>[a-zA-Z0-9$_.]+)\.)?(?<returnclass>[a-zA-Z0-9$_]+))?[\s\n]*", RegexOptions.Compiled);
		
		/**
		 * Regex for parsing property definitions.
		 *
		 * Groups:
		 *   - attribute1		'public', 'private' or 'static'
		 *   - attribute2		'public', 'private' or 'static'
		 *   - name:			name of property
		 *	 - type:			datatype of property - '[typepackage].[typeclass]'
		 *   - typepackage:		datatype package
		 *   - typeclass:		datatype class
		*/
		public static Regex Property = new Regex(@"\G((?<attribute1>(public|private|static))[\s\n]+)?((?<attribute2>(public|private|static))[\s\n]+)?var[\s\n]+(?<name>[a-zA-Z0-9$_]+)([\s\n]*:[\s\n]*(?<type>((?<typepackage>[a-zA-Z0-9$_.]+)\.)?(?<typeclass>[a-zA-Z0-9$_]+)))?([\s\n]*=[\s\n]*.*?)?;", RegexOptions.Compiled);
		
		/**
		 * Regex for parsing fully qualifies package into [package].[class] parts.
		 *
		 * Groups:
		 *   - package          package
		 *   - name             class name
		*/
		public static Regex PackageParts = new Regex(@"((?<package>[a-zA-Z0-9$_.]+)\.)?(?<name>[a-zA-Z0-9$_]+)", RegexOptions.Compiled);

		/**
		* Regex for parsing whitespace
		*/
		public static Regex Whitespace = new Regex(@"\G[\s\n]+", RegexOptions.Compiled);

		/**
		* Regex for parsing string literals
		*/
		public static Regex StringLiteral = new Regex("\\G(\"([^\"\\]*(\\\\.[^\"\\]*)*)\"|'([^\\']*(\\\\.[^\\']*)*)')", RegexOptions.Compiled);

		/**
		 * Regex for parsing line comments
		*/
		public static Regex LineComment = new Regex(@"\G//.*?(\n|$)", RegexOptions.Compiled);
		
		/** 
		 * Regex for parsing block comments
		*/
		public static Regex Comment = new Regex(@"\G/\*(.|\n)*?\*/", RegexOptions.Compiled);
	}
}