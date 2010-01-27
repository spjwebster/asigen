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

namespace AS2Parser.Args {

	public enum NamespaceAttribute {Public, Private};

	public class ImportHandlerEventArgs : EventArgs {
		private string package;
		private string name;
		
		public ImportHandlerEventArgs(string package, string name) : base() {
			this.package = package;
			this.name = name;
		}
		
		public string Package { get { return this.package; } }
		public string Name { get { return this.name; } }
	}
	
	public class ClassHandlerEventArgs : EventArgs {
		private bool isDynamic;
		private string package;
		private string name;
		private string baseClass;
		private string interfaces;

		public ClassHandlerEventArgs(bool isDynamic, string package, string name, string baseClass, string interfaces) : base() {
			this.isDynamic = isDynamic;
			this.package = package;
			this.name = name;
			this.baseClass = baseClass;
			this.interfaces = interfaces;
		}

		public bool IsDynamic { get { return this.isDynamic; } }
		public string Package { get { return this.package; } }
		public string Name { get { return this.name; } }
		public string BaseClass { get { return this.baseClass; } }
		public string Interfaces { get { return this.interfaces; } }
	}
	
	public class InterfaceHandlerEventArgs : EventArgs {
		private string package;
		private string name;
		private string baseInterface;

		public InterfaceHandlerEventArgs(string package, string name, string baseInterface) : base() {
			this.package = package;
			this.name = name;
			this.baseInterface = baseInterface;
		}

		public string Package { get { return this.package; } }
		public string Name { get { return this.name; } }
		public string BaseInterface { get { return this.baseInterface; } }
	}

	public class MethodHandlerEventArgs : EventArgs {
		private NamespaceAttribute ns;
		private bool isStatic;
		private string name;
		private string parameters;
		private string returnType;

		public MethodHandlerEventArgs(NamespaceAttribute ns, bool isStatic, string name, string parameters, string returnType) : base() {
			this.ns = ns;
			this.isStatic = isStatic;
			this.name = name;
			this.parameters = parameters;
			this.returnType = returnType;
		}

		public NamespaceAttribute Namespace { get { return this.ns; } }
		public bool IsStatic { get { return this.isStatic; } }
		public string Name { get { return this.name; } }
		public string Parameters { get { return this.parameters; } }
		public string ReturnType { get { return this.returnType; } }
	}

	public class GetterHandlerEventArgs : MethodHandlerEventArgs {
		public GetterHandlerEventArgs(NamespaceAttribute ns, bool isStatic, string name, string parameters, string returnType)
		  : base(ns, isStatic, name, parameters, returnType) {}
	}

	public class SetterHandlerEventArgs : MethodHandlerEventArgs {
		public SetterHandlerEventArgs(NamespaceAttribute ns, bool isStatic, string name, string parameters, string returnType)
		  : base(ns, isStatic, name, parameters, returnType) {}
	}

	public class PropertyHandlerEventArgs : EventArgs {
		private NamespaceAttribute ns;
		private bool isStatic;
		private string name;
		private string type;

		public PropertyHandlerEventArgs(NamespaceAttribute ns, bool isStatic, string name, string type) : base() {
			this.ns = ns;
			this.isStatic = isStatic;
			this.name = name;
			this.type = type;
		}

		public NamespaceAttribute Namespace { get { return this.ns; } }
		public bool IsStatic { get { return this.isStatic; } }
		public string Name { get { return this.name; } }
		public string Type { get { return this.type; } }
	}
}