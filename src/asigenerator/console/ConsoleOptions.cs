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
using System.Collections;
using Mono.GetOptions;
using ASIGenerator.Core;

namespace ASIGenerator.Console {
	class ConsoleOptions : Options, IGeneratorOptions {
		private ArrayList paths;
		private string output;
		private bool recursive = false;
		private bool publicOnly = false;
		private bool silent = false;
		private bool verbose = false;
		
		public ConsoleOptions(string [] args) {
			paths = new ArrayList();
			output = "asi";
			
			ProcessArgs(args);
		}
	
		public string [] Paths {
			get { return (string []) paths.ToArray(Type.GetType("System.String")); }
		}

		[Option("Output to specified {path}", 'o', "output")]
		public string Output {
			get { return output; }
			set { output = value; }
		}

		[Option("Resursively search paths for class files", 'r', "recursive")]
		public bool Recursive {
			get { return this.recursive; }
			set { this.recursive = value; }
		}
		
		[Option("Only include public definitions", "public")]
		public bool PublicOnly {
			get { return this.publicOnly; }
			set { this.publicOnly = value; }
		}
		
		[Option("Supress all console output", 's', "silent")]
		public bool Silent {
			get { return this.silent; }
			set { this.silent = value; }
		}

		[Option("Show verbose output", 'v', "verbose")]
		public bool Verbose {
			get { return this.verbose; }
			set { this.verbose = value; }
		}
		
		[ArgumentProcessor]
		public void AddPath(string path) {
			paths.Add(path);
		}	
	} 
}