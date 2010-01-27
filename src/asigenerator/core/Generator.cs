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
using System.Text;
using System.IO;
using AS2Parser;
using AS2Parser.Args;

namespace ASIGenerator.Core {
	public interface IGeneratorOptions {
		string[] Paths {	get; }
		string Output { get; }
		bool Recursive { get; }
		bool PublicOnly { get; }
		bool Silent { get; }
		bool Verbose { get; }
	}

	/**********************************************************************************************
	** Delegates                                                                                 **
	**********************************************************************************************/
	public delegate void MessageHandler(object sender, string message);
	

	class Generator {

		/**********************************************************************************************
		** Events                                                                                    **
		**********************************************************************************************/
		public event MessageHandler OnProgress;
		public event MessageHandler OnError;
		public event MessageHandler OnWarning;
				

		/**********************************************************************************************
		** Fields                                                                                    **
		**********************************************************************************************/
		private Parser parser;
		private StringBuilder buffer;
		private IGeneratorOptions options;
		private string packageName = String.Empty;
		private string filename = String.Empty;


		/**********************************************************************************************
		** Constructor                                                                               **
		**********************************************************************************************/
		public Generator(IGeneratorOptions options) {
			this.options = options;
			this.parser = new Parser();
			
			// Add event handler delegates
			this.parser.OnStart += new StartHandler(Parser_OnStart);
			this.parser.OnComplete += new CompleteHandler(Parser_OnComplete);
			this.parser.OnImportFound += new ImportHandler(Parser_OnImportFound);
			this.parser.OnClassFound += new ClassHandler(Parser_OnClassFound);
			this.parser.OnInterfaceFound += new InterfaceHandler(Parser_OnInterfaceFound);
			this.parser.OnMethodFound += new MethodHandler(Parser_OnMethodFound);
			this.parser.OnGetterFound += new GetterHandler(Parser_OnGetterFound);
			this.parser.OnSetterFound += new SetterHandler(Parser_OnSetterFound);
			this.parser.OnPropertyFound += new PropertyHandler(Parser_OnPropertyFound);
		}


		/**********************************************************************************************
		** Public methods                                                                            **
		**********************************************************************************************/
		public void generate() {
			foreach (string path in this.options.Paths) {
				
				string fullPath = Path.GetFullPath(path);
				
				if (Directory.Exists(fullPath)) {
					this.ParseDirectory(fullPath, this.options.Recursive);
					
				} else if (File.Exists(fullPath) && path.ToLower().EndsWith(".as")) {
					this.ParseClassFile(fullPath);
				}
			}
		}
		

		/**********************************************************************************************
		** Private methods                                                                           **
		**********************************************************************************************/
		public void ParseClassFile(string inputPath) {
			this.filename = inputPath;
			
			try {
				OnProgress(this, "Parsing '" + inputPath + "'");
				this.parser.ParseFile(inputPath);
				
			} catch (FileNotFoundException e) {
				OnError(this, "Couldn't find '" + inputPath + "'");
				System.Console.WriteLine("Couldn't find '" + inputPath + "'");
			}
		}
		
		public void ParseDirectory(string path, bool recursive) {
			string[] files = Directory.GetFiles(path, "*.as");

			foreach (string filename in files) {
				ParseClassFile( Path.Combine(path, filename));
			}
			
			if (recursive) {
				string[] dirPaths = Directory.GetDirectories(path);
				
				foreach (string dirPath in dirPaths) {
					ParseDirectory(dirPath, recursive);
				}
			}
		}
		
		private void WriteBuffer() {
			if (this.packageName == String.Empty) {
				OnWarning(this, "No class or interface definition found in '" + this.filename + "'");
				return;
			}
			
			string outputPath = Path.Combine(Path.GetFullPath(this.options.Output), this.packageName.Replace('.', Path.DirectorySeparatorChar) + ".as");
			string outputDirectory = Path.GetDirectoryName(outputPath);
			
			// Create directory if it doesn't already exist
			if (!Directory.Exists(outputDirectory)) {
				Directory.CreateDirectory(outputDirectory);
			}
			
			// Write file
			StreamWriter outputFile = new StreamWriter(outputPath);
			outputFile.Write(this.buffer.ToString());
			outputFile.Close();
		}		

		
		/**********************************************************************************************
		** Event handlers                                                                            **
		**********************************************************************************************/
		private void Parser_OnImportFound(object sender, ImportHandlerEventArgs importInfo) {
			this.buffer.Append("import ");
			
			if (importInfo.Package.Length > 0) {
				this.buffer.Append(importInfo.Package + ".");
			}
			
			this.buffer.Append(importInfo.Name + ";" + Environment.NewLine);
		}

		private void Parser_OnStart(object sender) {
			this.buffer = new StringBuilder();
			this.packageName = String.Empty;
		}

		private void Parser_OnClassFound(object sender, ClassHandlerEventArgs classInfo) {
			this.packageName = (classInfo.Package.Length > 0 ? classInfo.Package + "." : "") + classInfo.Name;
			
			this.buffer.Append("intrinsic");
			
			if (classInfo.IsDynamic) {
				this.buffer.Append(" dynamic");
			}
			
			this.buffer.Append(" class ");

			if (classInfo.Package.Length > 0) {
				this.buffer.Append(classInfo.Package + ".");
			}
			
			this.buffer.Append(classInfo.Name);
			
			if (classInfo.BaseClass.Length > 0) {
				this.buffer.Append(" extends " + classInfo.BaseClass);
			}

			if (classInfo.Interfaces.Length > 0) {
				this.buffer.Append(" implements " + classInfo.Interfaces);
			}

			this.buffer.Append(" {" + Environment.NewLine);
		}

		private void Parser_OnInterfaceFound(object sender, InterfaceHandlerEventArgs interfaceInfo) {
			this.packageName = (interfaceInfo.Package.Length > 0 ? interfaceInfo.Package + "." : "") + interfaceInfo.Name;
			
			this.buffer.Append("interface ");
			
			if (interfaceInfo.Package.Length > 0) {
				this.buffer.Append(interfaceInfo.Package + ".");
			}
			
			this.buffer.Append(interfaceInfo.Name);
			
			if (interfaceInfo.BaseInterface.Length > 0) {
				this.buffer.Append(" extends " + interfaceInfo.BaseInterface);
			}

			this.buffer.Append(" {" + Environment.NewLine);
		}

		private void Parser_OnMethodFound(object sender, MethodHandlerEventArgs methodInfo) {
			this.buffer.Append("\t" + methodInfo.Namespace.ToString().ToLower());

			if (methodInfo.IsStatic) {
				this.buffer.Append(" static");
			}

			this.buffer.Append(" function " + methodInfo.Name + "(" + methodInfo.Parameters + ")");

			if (methodInfo.ReturnType.Length > 0) {
				this.buffer.Append(":" + methodInfo.ReturnType);
			}
			
			this.buffer.Append(";" + Environment.NewLine);
		}

		private void Parser_OnGetterFound(object sender, GetterHandlerEventArgs getterInfo) {
			this.buffer.Append("\t" + getterInfo.Namespace.ToString().ToLower());

			if (getterInfo.IsStatic) {
				this.buffer.Append(" static");
			}

			this.buffer.Append(" function get " + getterInfo.Name + "()");

			if (getterInfo.ReturnType.Length > 0) {
				this.buffer.Append(":" + getterInfo.ReturnType);
			}
			
			this.buffer.Append(";" + Environment.NewLine);
		}

		private void Parser_OnSetterFound(object sender, SetterHandlerEventArgs setterInfo) {
			this.buffer.Append("\t" + setterInfo.Namespace.ToString().ToLower());

			if (setterInfo.IsStatic) {
				this.buffer.Append(" static");
			}

			this.buffer.Append(" function set " + setterInfo.Name + "(" + setterInfo.Parameters + ")");

			if (setterInfo.ReturnType.Length > 0) {
				this.buffer.Append(":" + setterInfo.ReturnType);
			}
			
			this.buffer.Append(";" + Environment.NewLine);
		}

		private void Parser_OnPropertyFound(object sender, PropertyHandlerEventArgs propertyInfo) {
			this.buffer.Append("\t" + propertyInfo.Namespace.ToString().ToLower());

			if (propertyInfo.IsStatic) {
				this.buffer.Append(" static");
			}

			this.buffer.Append(" var " + propertyInfo.Name);

			if (propertyInfo.Type.Length > 0) {
				this.buffer.Append(":" + propertyInfo.Type);
			}
			
			this.buffer.Append(";" + Environment.NewLine);
		}

		private void Parser_OnComplete(object sender) {
			this.buffer.Append("}" + Environment.NewLine);
			this.WriteBuffer();
		}
	}
}