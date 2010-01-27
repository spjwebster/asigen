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
using System.Collections;
using System.Reflection;
using ASIGenerator.Core;

namespace ASIGenerator.Console {
	class Application {
		
		/******************************************************************************************
		** Entry point                                                                           **
		******************************************************************************************/
		public static void Main(string[] args) {
			Application app = new Application();
			app.run(args);
		}
		

		/******************************************************************************************
		** Private fields                                                                        **
		******************************************************************************************/
		private ConsoleOptions options;
		
		
		/******************************************************************************************
		** Constructor                                                                           **
		******************************************************************************************/
		private Application() {
			// Do nothing
		}
		

		/******************************************************************************************
		** Protected methods                                                                     **
		******************************************************************************************/
		protected void run(string[] args) {
			
			long startTime = DateTime.Now.Ticks;

			// Load options from command-line arguments
			this.options = new ConsoleOptions(args);
			
			if (this.options.Paths.Length > 0) {
				// Output header if not running silent
				if (this.options.Verbose) {
					WriteHeader();
				}
		
				Generator generator = new Generator(options);
				
				generator.OnProgress += new MessageHandler(this.generator_OnProgress);
				generator.OnWarning += new MessageHandler(this.generator_OnWarning);
				generator.OnError += new MessageHandler(this.generator_OnError);
				
				generator.generate();

				if (this.options.Verbose) {
					TimeSpan interval = new TimeSpan(DateTime.Now.Ticks - startTime);
					System.Console.WriteLine("Processed in " + interval.TotalSeconds + " seconds");
				}
				
			} else {
				this.options.DoHelp();
			}
		}
		
		/******************************************************************************************
		** Private methods                                                                       **
		******************************************************************************************/
		private void generator_OnProgress(object sender, string message) {
			if (this.options.Verbose) {
				System.Console.WriteLine(message);
			}
		} 
		
		private void generator_OnWarning(object sender, string message) {
			System.Console.WriteLine("Warning: " + message);
		} 
		
		private void generator_OnError(object sender, string message) {
			System.Console.WriteLine("Error: " + message);
		} 

		private void WriteHeader() {
			System.Console.WriteLine("asigen");
		}
	}
}